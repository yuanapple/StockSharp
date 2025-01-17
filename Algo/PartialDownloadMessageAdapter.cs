﻿namespace StockSharp.Algo
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using Ecng.Collections;
	using Ecng.Common;

	using StockSharp.Messages;

	/// <summary>
	/// Message adapter that splits large market data requests on smaller.
	/// </summary>
	public class PartialDownloadMessageAdapter : MessageAdapterWrapper
	{
		/// <summary>
		/// Message for iterate action.
		/// </summary>
		[DataContract]
		[Serializable]
		private class PartialDownloadMessage : Message, IOriginalTransactionIdMessage
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="PartialDownloadMessage"/>.
			/// </summary>
			public PartialDownloadMessage()
				: base(ExtendedMessageTypes.PartialDownload)
			{
			}

			[DataMember]
			public long OriginalTransactionId { get; set; }

			/// <inheritdoc />
			public override string ToString()
			{
				return base.ToString() + $",OrigTrId={OriginalTransactionId}";
			}

			/// <summary>
			/// Create a copy of <see cref="PartialDownloadMessage"/>.
			/// </summary>
			/// <returns>Copy.</returns>
			public override Message Clone()
			{
				return CopyTo(new PartialDownloadMessage());
			}

			/// <summary>
			/// Copy the message into the <paramref name="destination" />.
			/// </summary>
			/// <param name="destination">The object, to which copied information.</param>
			/// <returns>The object, to which copied information.</returns>
			private PartialDownloadMessage CopyTo(PartialDownloadMessage destination)
			{
				destination.OriginalTransactionId = OriginalTransactionId;

				this.CopyExtensionInfo(destination);

				return destination;
			}
		}

		private class DownloadInfo
		{
			public MarketDataMessage Origin { get; }

			public long CurrTransId { get; private set; }
			public bool LastIteration => Origin.To != null && _nextFrom >= Origin.To.Value;

			public bool ReplyReceived { get; set; }

			private readonly PartialDownloadMessageAdapter _adapter;
			private readonly TimeSpan _iterationInterval;
			private readonly TimeSpan _step;

			private DateTimeOffset _currFrom;
			private bool _firstIteration;
			private DateTimeOffset _nextFrom;
			private readonly DateTimeOffset _maxFrom;

			public DownloadInfo(PartialDownloadMessageAdapter adapter, MarketDataMessage origin, TimeSpan step, TimeSpan iterationInterval)
			{
				if (step <= TimeSpan.Zero)
					throw new ArgumentOutOfRangeException(nameof(step));

				if (iterationInterval < TimeSpan.Zero)
					throw new ArgumentOutOfRangeException(nameof(iterationInterval));

				_adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
				Origin = origin ?? throw new ArgumentNullException(nameof(origin));
				_step = step;
				_iterationInterval = iterationInterval;

				_maxFrom = origin.To ?? DateTimeOffset.Now;
				_currFrom = origin.From ?? _maxFrom - step;

				_firstIteration = true;
			}

			public void TryUpdateNextFrom(DateTimeOffset last)
			{
				if (_nextFrom < last)
					_nextFrom = last;
			}

			public MarketDataMessage InitNext()
			{
				if (LastIteration)
					throw new InvalidOperationException("LastIteration == true");

				var mdMsg = (MarketDataMessage)Origin.Clone();

				if (_firstIteration)
				{
					_firstIteration = false;

					_nextFrom = _currFrom + _step;

					if (_nextFrom > _maxFrom)
						_nextFrom = _maxFrom;

					mdMsg.TransactionId = _adapter.TransactionIdGenerator.GetNextId();
					mdMsg.From = _currFrom;
					mdMsg.To = _nextFrom;

					CurrTransId = mdMsg.TransactionId;
				}
				else
				{
					_iterationInterval.Sleep();

					if (Origin.To == null && _nextFrom >= _maxFrom)
					{
						// on-line
						mdMsg.From = null;
					}
					else
					{
						_currFrom = _nextFrom;
						_nextFrom += _step;

						if (_nextFrom > _maxFrom)
							_nextFrom = _maxFrom;

						mdMsg.TransactionId = _adapter.TransactionIdGenerator.GetNextId();
						mdMsg.From = _currFrom;
						mdMsg.To = _nextFrom;

						CurrTransId = mdMsg.TransactionId;
					}
				}

				return mdMsg;
			}
		}

		private readonly SyncObject _syncObject = new SyncObject();
		private readonly Dictionary<long, DownloadInfo> _original = new Dictionary<long, DownloadInfo>();
		private readonly Dictionary<long, DownloadInfo> _partialRequests = new Dictionary<long, DownloadInfo>();
		private readonly Dictionary<long, Tuple<long, DownloadInfo>> _unsubscribeRequests = new Dictionary<long, Tuple<long, DownloadInfo>>();
		private readonly Dictionary<long, bool> _liveRequests = new Dictionary<long, bool>();

		/// <summary>
		/// Initializes a new instance of the <see cref="PartialDownloadMessageAdapter"/>.
		/// </summary>
		/// <param name="innerAdapter">Underlying adapter.</param>
		public PartialDownloadMessageAdapter(IMessageAdapter innerAdapter)
			: base(innerAdapter)
		{
		}

		/// <inheritdoc />
		protected override void OnSendInMessage(Message message)
		{
			switch (message.Type)
			{
				case MessageTypes.Reset:
				case MessageTypes.Disconnect:
				{
					lock (_syncObject)
					{
						_partialRequests.Clear();
						_original.Clear();
						_unsubscribeRequests.Clear();
						_liveRequests.Clear();
					}

					break;
				}

				case MessageTypes.OrderStatus:
				case MessageTypes.PortfolioLookup:
				{
					var subscriptionMsg = (ISubscriptionMessage)message;

					if (subscriptionMsg.IsSubscribe)
					{
						var from = subscriptionMsg.From;
						var to = subscriptionMsg.To;

						if (from != null || to != null)
						{
							var step = InnerAdapter.GetHistoryStepSize(DataType.Transactions, out _);

							// adapter do not provide historical request
							if (step == TimeSpan.Zero)
							{
								if (to != null)
								{
									// finishing current history request

									if (message.Type == MessageTypes.PortfolioLookup)
									{
										RaiseNewOutMessage(message.Type.ToResultType().CreateLookupResult(subscriptionMsg.TransactionId));
									}

									return;
								}
								else
								{
									// or sending further only live subscription
									subscriptionMsg.From = null;
									subscriptionMsg.To = null;

									_liveRequests.Add(subscriptionMsg.TransactionId, false);
								}
							}
						}
						else
							_liveRequests.Add(subscriptionMsg.TransactionId, false);
					}

					break;
				}

				case MessageTypes.MarketData:
				{
					var mdMsg = (MarketDataMessage)message;

					if (mdMsg.IsSubscribe)
					{
						var from = mdMsg.From;
						var to = mdMsg.To;

						if (from != null || to != null)
						{
							var step = InnerAdapter.GetHistoryStepSize(mdMsg.ToDataType(), out var iterationInterval);

							// adapter do not provide historical request
							if (step == TimeSpan.Zero)
							{
								if (to != null)
								{
									// finishing current history request
									RaiseNewOutMessage(new MarketDataFinishedMessage { OriginalTransactionId = mdMsg.TransactionId });
									return;
								}
								else
								{
									// or sending further only live subscription
									mdMsg.From = null;
									mdMsg.To = null;

									_liveRequests.Add(mdMsg.TransactionId, false);
									break;
								}
							}

							var info = new DownloadInfo(this, (MarketDataMessage)mdMsg.Clone(), step, iterationInterval);

							message = info.InitNext();

							lock (_syncObject)
							{
								_original.Add(info.Origin.TransactionId, info);
								_partialRequests.Add(info.CurrTransId, info);
							}
						}
						else
							_liveRequests.Add(mdMsg.TransactionId, false);
					}
					else
					{
						lock (_syncObject)
						{
							if (!_original.TryGetValue(mdMsg.OriginalTransactionId, out var info))
								break;

							var transId = TransactionIdGenerator.GetNextId();
							_unsubscribeRequests.Add(transId, Tuple.Create(mdMsg.TransactionId, info));

							mdMsg.OriginalTransactionId = info.CurrTransId;
							mdMsg.TransactionId = transId;
						}
					}

					break;
				}

				case ExtendedMessageTypes.PartialDownload:
				{
					var partialMsg = (PartialDownloadMessage)message;

					lock (_syncObject)
					{
						if (!_original.TryGetValue(partialMsg.OriginalTransactionId, out var info))
							break;

						var mdMsg = info.InitNext();

						if (mdMsg.To == null)
						{
							_liveRequests.Add(mdMsg.TransactionId, true);

							_original.Remove(partialMsg.OriginalTransactionId);
							_partialRequests.RemoveWhere(p => p.Value == info);
						}
						else
							_partialRequests.Add(info.CurrTransId, info);

						message = mdMsg;
					}

					break;
				}
			}
			
			base.OnSendInMessage(message);
		}

		/// <inheritdoc />
		protected override void OnInnerAdapterNewOutMessage(Message message)
		{
			Message extra = null;

			switch (message.Type)
			{
				case MessageTypes.PortfolioLookupResult:
				case MessageTypes.OrderStatus:
				{
					var responseMsg = (IOriginalTransactionIdMessage)message;
					var originId = responseMsg.OriginalTransactionId;

					lock (_syncObject)
					{
						if (_liveRequests.TryGetValue(originId, out var isPartial))
						{
							_liveRequests.Remove(originId);

							if (isPartial)
							{
								if (((IErrorMessage)responseMsg).Error == null)
								{
									// reply was sent prev for first partial request,
									// now sending "online" message
									message = new SubscriptionOnlineMessage
									{
										OriginalTransactionId = originId
									};
								}
							}
							else
							{
								extra = new SubscriptionOnlineMessage
								{
									OriginalTransactionId = originId
								};
							}
						}
					}

					break;
				}

				case MessageTypes.MarketData:
				{
					var responseMsg = (MarketDataMessage)message;
					var originId = responseMsg.OriginalTransactionId;

					lock (_syncObject)
					{
						if (_liveRequests.TryGetValue(originId, out var isPartial))
						{
							_liveRequests.Remove(originId);

							if (isPartial)
							{
								if (responseMsg.IsOk())
								{
									// reply was sent prev for first partial request,
									// now sending "online" message
									message = new SubscriptionOnlineMessage
									{
										OriginalTransactionId = originId
									};
								}
							}
							else
							{
								extra = new SubscriptionOnlineMessage
								{
									OriginalTransactionId = originId
								};
							}

							break;
						}

						long requestId;

						if (!_partialRequests.TryGetValue(originId, out var info))
						{
							if (!_unsubscribeRequests.TryGetValue(originId, out var tuple))
								break;
							
							requestId = tuple.Item1;
							info = tuple.Item2;

							_original.Remove(info.Origin.TransactionId);
							_partialRequests.RemoveWhere(p => p.Value == info);
							_unsubscribeRequests.Remove(originId);
						}
						else
						{
							if (info.ReplyReceived)
								return;

							info.ReplyReceived = true;

							requestId = info.Origin.TransactionId;

							if (!responseMsg.IsOk())
							{
								_original.Remove(requestId);
								_partialRequests.RemoveWhere(p => p.Value == info);
							}
						}
						
						responseMsg.OriginalTransactionId = requestId;
					}

					break;
				}

				case MessageTypes.MarketDataFinished:
				{
					var finishMsg = (MarketDataFinishedMessage)message;

					lock (_syncObject)
					{
						if (_partialRequests.TryGetValue(finishMsg.OriginalTransactionId, out var info))
						{
							var origin = info.Origin;

							if (info.LastIteration)
							{
								_original.Remove(origin.TransactionId);
								_partialRequests.RemoveWhere(p => p.Value == info);

								finishMsg.OriginalTransactionId = origin.TransactionId;
								break;
							}
							
							_partialRequests.Remove(finishMsg.OriginalTransactionId);

							message = new PartialDownloadMessage
							{
								Adapter = this,
								IsBack = true,
								OriginalTransactionId = origin.TransactionId,
							};
						}
					}

					break;
				}

				case MessageTypes.CandleTimeFrame:
				case MessageTypes.CandlePnF:
				case MessageTypes.CandleRange:
				case MessageTypes.CandleRenko:
				case MessageTypes.CandleTick:
				case MessageTypes.CandleVolume:
				{
					TryUpdateSubscriptionResult((CandleMessage)message);
					break;
				}

				case MessageTypes.Execution:
				{
					var execMsg = (ExecutionMessage)message;

					if (!execMsg.IsMarketData())
						break;

					TryUpdateSubscriptionResult(execMsg);
					break;
				}

				case MessageTypes.Level1Change:
				{
					TryUpdateSubscriptionResult((Level1ChangeMessage)message);
					break;
				}

				case MessageTypes.QuoteChange:
				{
					TryUpdateSubscriptionResult((QuoteChangeMessage)message);
					break;
				}
			}

			base.OnInnerAdapterNewOutMessage(message);

			if (extra != null)
				base.OnInnerAdapterNewOutMessage(extra);
		}

		private void TryUpdateSubscriptionResult<TMessage>(TMessage message)
			where TMessage : ISubscriptionIdMessage, IServerTimeMessage
		{
			var originId = message.OriginalTransactionId;

			if (originId == 0)
				return;

			lock (_syncObject)
			{
				if (!_partialRequests.TryGetValue(originId, out var info))
					return;

				info.TryUpdateNextFrom(message.ServerTime);
				message.OriginalTransactionId = info.Origin.TransactionId;
			}
		}

		/// <summary>
		/// Create a copy of <see cref="PartialDownloadMessageAdapter"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public override IMessageChannel Clone()
		{
			return new PartialDownloadMessageAdapter((IMessageAdapter)InnerAdapter.Clone());
		}
	}
}