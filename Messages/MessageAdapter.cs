#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Messages.Messages
File: MessageAdapter.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Messages
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;

	using Ecng.Common;
	using Ecng.Interop;
	using Ecng.Serialization;

	using StockSharp.Logging;
	using StockSharp.Localization;

	/// <summary>
	/// The base adapter converts messages <see cref="Message"/> to the command of the trading system and back.
	/// </summary>
	public abstract class MessageAdapter : BaseLogReceiver, IMessageAdapter, INotifyPropertyChanged
	{
		/// <summary>
		/// Initialize <see cref="MessageAdapter"/>.
		/// </summary>
		/// <param name="transactionIdGenerator">Transaction id generator.</param>
		protected MessageAdapter(IdGenerator transactionIdGenerator)
		{
			Platform = Platforms.AnyCPU;

			TransactionIdGenerator = transactionIdGenerator ?? throw new ArgumentNullException(nameof(transactionIdGenerator));
			SecurityClassInfo = new Dictionary<string, RefPair<SecurityTypes, string>>();

			StorageName = GetType().Namespace.Remove(nameof(StockSharp)).Remove(".");

			Platform = GetType().GetPlatform();

			var attr = GetType().GetAttribute<MessageAdapterCategoryAttribute>();
			if (attr != null)
				Categories = attr.Categories;
		}

		private IEnumerable<MessageTypes> _supportedInMessages = Enumerable.Empty<MessageTypes>();

		/// <inheritdoc />
		[Browsable(false)]
		public virtual IEnumerable<MessageTypes> SupportedInMessages
		{
			get => _supportedInMessages;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				var duplicate = value.GroupBy(m => m).FirstOrDefault(g => g.Count() > 1);
				if (duplicate != null)
					throw new ArgumentException(LocalizedStrings.Str415Params.Put(duplicate.Key), nameof(value));

				_supportedInMessages = value.ToArray();

				OnPropertyChanged(nameof(SupportedInMessages));
			}
		}

		private IEnumerable<MessageTypes> _supportedOutMessages = Enumerable.Empty<MessageTypes>();

		/// <inheritdoc />
		[Browsable(false)]
		public virtual IEnumerable<MessageTypes> SupportedOutMessages
		{
			get => _supportedOutMessages;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				var duplicate = value.GroupBy(m => m).FirstOrDefault(g => g.Count() > 1);
				if (duplicate != null)
					throw new ArgumentException(LocalizedStrings.Str415Params.Put(duplicate.Key), nameof(value));

				_supportedOutMessages = value.ToArray();

				OnPropertyChanged(nameof(SupportedOutMessages));
			}
		}

		private IEnumerable<MessageTypeInfo> _possibleSupportedMessages = Enumerable.Empty<MessageTypeInfo>();

		/// <inheritdoc />
		[Browsable(false)]
		public virtual IEnumerable<MessageTypeInfo> PossibleSupportedMessages
		{
			get => _possibleSupportedMessages;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				var duplicate = value.GroupBy(m => m.Type).FirstOrDefault(g => g.Count() > 1);
				if (duplicate != null)
					throw new ArgumentException(LocalizedStrings.Str415Params.Put(duplicate.Key), nameof(value));

				_possibleSupportedMessages = value;
				OnPropertyChanged(nameof(PossibleSupportedMessages));

				SupportedInMessages = value.Select(t => t.Type).ToArray();
			}
		}

		private IEnumerable<MarketDataTypes> _supportedMarketDataTypes = Enumerable.Empty<MarketDataTypes>();

		/// <inheritdoc />
		[Browsable(false)]
		public virtual IEnumerable<MarketDataTypes> SupportedMarketDataTypes
		{
			get => _supportedMarketDataTypes;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				var duplicate = value.GroupBy(m => m).FirstOrDefault(g => g.Count() > 1);
				if (duplicate != null)
					throw new ArgumentException(LocalizedStrings.Str415Params.Put(duplicate.Key), nameof(value));

				_supportedMarketDataTypes = value.ToArray();
			}
		}

		/// <inheritdoc />
		[Browsable(false)]
		public IDictionary<string, RefPair<SecurityTypes, string>> SecurityClassInfo { get; }

		/// <inheritdoc />
		[Browsable(false)]
		public virtual IEnumerable<Level1Fields> CandlesBuildFrom => Enumerable.Empty<Level1Fields>();

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool CheckTimeFrameByRequest { get; set; }

		private TimeSpan _heartbeatInterval = TimeSpan.Zero;

		/// <inheritdoc />
		[Display(
			ResourceType = typeof(LocalizedStrings),
			Name = LocalizedStrings.Str192Key,
			Description = LocalizedStrings.Str193Key,
			GroupName = LocalizedStrings.Str186Key,
			Order = 300)]
		public TimeSpan HeartbeatInterval
		{
			get => _heartbeatInterval;
			set
			{
				if (value < TimeSpan.Zero)
					throw new ArgumentOutOfRangeException();

				_heartbeatInterval = value;
			}
		}

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsNativeIdentifiersPersistable => true;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsNativeIdentifiers => false;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsFullCandlesOnly => true;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsSupportSubscriptions => true;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsSupportCandlesUpdates => false;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual MessageAdapterCategories Categories { get; }

		/// <inheritdoc />
		[Browsable(false)]
		public virtual string StorageName { get; }

		/// <inheritdoc />
		[Browsable(false)]
		public virtual OrderCancelVolumeRequireTypes? OrderCancelVolumeRequired { get; } = null;

		/// <summary>
		/// Bit process, which can run the adapter.
		/// </summary>
		[Browsable(false)]
		public Platforms Platform { get; protected set; }

		/// <inheritdoc />
		[Browsable(false)]
		public virtual IEnumerable<Tuple<string, Type>> SecurityExtendedFields { get; } = Enumerable.Empty<Tuple<string, Type>>();

		/// <inheritdoc />
		[Browsable(false)]
		public virtual IEnumerable<int> SupportedOrderBookDepths => Enumerable.Empty<int>();

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsSupportOrderBookIncrements => false;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsSupportExecutionsPnL => false;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool IsSecurityNewsOnly => false;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual Type OrderConditionType => GetType()
			.GetAttribute<OrderConditionAttribute>()?
			.ConditionType;

		/// <inheritdoc />
		[Browsable(false)]
		public virtual bool HeartbeatBeforConnect => false;

		/// <inheritdoc />
		[CategoryLoc(LocalizedStrings.Str174Key)]
		public ReConnectionSettings ReConnectionSettings { get; } = new ReConnectionSettings();

		private IdGenerator _transactionIdGenerator;

		/// <inheritdoc />
		[Browsable(false)]
		public IdGenerator TransactionIdGenerator
		{
			get => _transactionIdGenerator;
			set => _transactionIdGenerator = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <inheritdoc />
		public event Action<Message> NewOutMessage;

		bool IMessageChannel.IsOpened => true;

		void IMessageChannel.Open()
		{
		}

		void IMessageChannel.Close()
		{
		}

		event Action IMessageChannel.StateChanged
		{
			add { }
			remove { }
		}

		/// <inheritdoc />
		public void SendInMessage(Message message)
		{
			if (message.Type == MessageTypes.Connect)
			{
				if (!Platform.IsCompatible())
				{
					SendOutMessage(new ConnectMessage
					{
						Error = new InvalidOperationException(LocalizedStrings.Str169Params.Put(GetType().Name, Platform))
					});

					return;
				}
			}

			InitMessageLocalTime(message);

			try
			{
				OnSendInMessage(message);
			}
			catch (Exception ex)
			{
				this.AddErrorLog(ex);

				message.HandleErrorResponse(ex, CurrentTime, SendOutMessage);

				SendOutError(ex);
			}
		}

		/// <summary>
		/// Send message.
		/// </summary>
		/// <param name="message">Message.</param>
		protected abstract void OnSendInMessage(Message message);

		/// <summary>
		/// Send outgoing message and raise <see cref="NewOutMessage"/> event.
		/// </summary>
		/// <param name="message">Message.</param>
		public virtual void SendOutMessage(Message message)
		{
			//// do not process empty change msgs
			//if (!message.IsBack)
			//{
			//	if (message is Level1ChangeMessage l1Msg && l1Msg.Changes.Count == 0)
			//		return;
			//	else if (message is BasePositionChangeMessage posMsg && posMsg.Changes.Count == 0)
			//		return;
			//}

			InitMessageLocalTime(message);

			if (/*message.IsBack && */message.Adapter == null)
				message.Adapter = this;

			switch (message.Type)
			{
				case MessageTypes.TimeFrameLookupResult:
					_timeFrames = ((TimeFrameLookupResultMessage)message).TimeFrames;
					break;
			}

			NewOutMessage?.Invoke(message);
		}

		/// <summary>
		/// Initialize local timestamp <see cref="Message"/>.
		/// </summary>
		/// <param name="message">Message.</param>
		private void InitMessageLocalTime(Message message)
		{
			message.TryInitLocalTime(this);

			switch (message)
			{
				case PositionChangeMessage posMsg when posMsg.ServerTime.IsDefault():
					posMsg.ServerTime = CurrentTime;
					break;
				case ExecutionMessage execMsg when execMsg.ExecutionType == ExecutionTypes.Transaction && execMsg.ServerTime.IsDefault():
					execMsg.ServerTime = CurrentTime;
					break;
			}
		}

		/// <summary>
		/// Send to <see cref="SendOutMessage"/> disconnect message.
		/// </summary>
		/// <param name="expected">Is disconnect expected.</param>
		protected void SendOutDisconnectMessage(bool expected)
		{
			SendOutDisconnectMessage(expected ? null : new InvalidOperationException(LocalizedStrings.Str2551));
		}

		/// <summary>
		/// Send to <see cref="SendOutMessage"/> disconnect message.
		/// </summary>
		/// <param name="error">Error info. Can be <see langword="null"/>.</param>
		protected void SendOutDisconnectMessage(Exception error)
		{
			SendOutMessage(error == null ? (BaseConnectionMessage)new DisconnectMessage() : new ConnectMessage
			{
				Error = error
			});
		}

		/// <summary>
		/// Initialize a new message <see cref="ErrorMessage"/> and pass it to the method <see cref="SendOutMessage"/>.
		/// </summary>
		/// <param name="description">Error details.</param>
		protected void SendOutError(string description)
		{
			SendOutError(new InvalidOperationException(description));
		}

		/// <summary>
		/// Initialize a new message <see cref="ErrorMessage"/> and pass it to the method <see cref="SendOutMessage"/>.
		/// </summary>
		/// <param name="error">Error details.</param>
		protected void SendOutError(Exception error)
		{
			SendOutMessage(error.ToErrorMessage());
		}

		/// <summary>
		/// Initialize a new message <see cref="MarketDataMessage"/> and pass it to the method <see cref="SendOutMessage"/>.
		/// </summary>
		/// <param name="originalTransactionId">ID of the original message for which this message is a response.</param>
		/// <param name="error">Subscribe or unsubscribe error info. To be set if the answer.</param>
		protected void SendOutMarketDataReply(long originalTransactionId, Exception error = null)
		{
			SendOutMessage(new MarketDataMessage
			{
				OriginalTransactionId = originalTransactionId,
				Error = error,
			});
		}

		/// <summary>
		/// Initialize a new message <see cref="MarketDataMessage"/> and pass it to the method <see cref="SendOutMessage"/>.
		/// </summary>
		/// <param name="originalTransactionId">ID of the original message for which this message is a response.</param>
		protected void SendOutMarketDataNotSupported(long originalTransactionId)
		{
			SendOutMessage(new MarketDataMessage { OriginalTransactionId = originalTransactionId, IsNotSupported = true });
		}

		/// <inheritdoc />
		public virtual IOrderLogMarketDepthBuilder CreateOrderLogMarketDepthBuilder(SecurityId securityId)
			=> new OrderLogMarketDepthBuilder(securityId);

		private IEnumerable<TimeSpan> _timeFrames = Enumerable.Empty<TimeSpan>();

		/// <summary>
		/// Get possible time-frames for the specified instrument.
		/// </summary>
		/// <param name="securityId">Security ID.</param>
		/// <param name="from">The initial date from which you need to get data.</param>
		/// <param name="to">The final date by which you need to get data.</param>
		/// <returns>Possible time-frames.</returns>
		protected virtual IEnumerable<TimeSpan> GetTimeFrames(SecurityId securityId, DateTimeOffset? from, DateTimeOffset? to)
			=> _timeFrames;

		/// <inheritdoc />
		public virtual IEnumerable<object> GetCandleArgs(Type candleType, SecurityId securityId, DateTimeOffset? from, DateTimeOffset? to)
		{
			return candleType == typeof(TimeFrameCandleMessage)
				? GetTimeFrames(securityId, from, to).Cast<object>()
				: Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public virtual TimeSpan GetHistoryStepSize(DataType dataType, out TimeSpan iterationInterval)
		{
			if (dataType == null)
				throw new ArgumentNullException(nameof(dataType));

			iterationInterval = TimeSpan.FromSeconds(2);

			if (dataType.IsCandles)
			{
				if (!this.IsMarketDataTypeSupported(dataType.ToMarketDataType().Value))
					return TimeSpan.Zero;

				if (dataType.MessageType == typeof(TimeFrameCandleMessage))
				{
					var tf = (TimeSpan)dataType.Arg;

					if (tf.TotalDays <= 1)
						return TimeSpan.FromDays(30);

					return TimeSpan.MaxValue;
				}
			}

			// by default adapter do not provide historical data except candles
			return TimeSpan.Zero;
		}

		/// <inheritdoc />
		public virtual bool IsAllDownloadingSupported(DataType dataType) => false;

		/// <inheritdoc />
		public virtual bool IsSecurityRequired(DataType dataType) => true;

		/// <inheritdoc />
		public override void Load(SettingsStorage storage)
		{
			Id = storage.GetValue(nameof(Id), Id);
			HeartbeatInterval = storage.GetValue<TimeSpan>(nameof(HeartbeatInterval));

			if (storage.ContainsKey(nameof(SupportedInMessages)) || storage.ContainsKey("SupportedMessages"))
				SupportedInMessages = (storage.GetValue<string[]>(nameof(SupportedInMessages)) ?? storage.GetValue<string[]>("SupportedMessages")).Select(i => i.To<MessageTypes>()).ToArray();
			
			if (storage.ContainsKey(nameof(ReConnectionSettings)))
				ReConnectionSettings.Load(storage.GetValue<SettingsStorage>(nameof(ReConnectionSettings)));

			base.Load(storage);
		}

		/// <inheritdoc />
		public override void Save(SettingsStorage storage)
		{
			storage.SetValue(nameof(Id), Id);
			storage.SetValue(nameof(HeartbeatInterval), HeartbeatInterval);
			storage.SetValue(nameof(SupportedInMessages), SupportedInMessages.Select(t => t.To<string>()).ToArray());
			storage.SetValue(nameof(ReConnectionSettings), ReConnectionSettings.Save());

			base.Save(storage);
		}

		/// <summary>
		/// Create a copy of <see cref="MessageAdapter"/>.
		/// </summary>
		/// <returns>Copy.</returns>
		public virtual IMessageChannel Clone()
		{
			var clone = GetType().CreateInstance<MessageAdapter>(TransactionIdGenerator);
			clone.Load(this.Save());
			return clone;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		private PropertyChangedEventHandler _propertyChanged;

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add => _propertyChanged += value;
			remove => _propertyChanged -= value;
		}

		/// <summary>
		/// Raise <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			_propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	/// <summary>
	/// Special adapter, which transmits directly to the output of all incoming messages.
	/// </summary>
	public class PassThroughMessageAdapter : MessageAdapter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PassThroughMessageAdapter"/>.
		/// </summary>
		/// <param name="transactionIdGenerator">Transaction id generator.</param>
		public PassThroughMessageAdapter(IdGenerator transactionIdGenerator)
			: base(transactionIdGenerator)
		{
		}

		/// <inheritdoc />
		protected override void OnSendInMessage(Message message)
		{
			SendOutMessage(message);
		}
	}
}