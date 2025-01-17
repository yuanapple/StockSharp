namespace StockSharp.Algo
{
	using System;
	using System.Collections.Generic;

	using Ecng.Common;

	using StockSharp.Algo.Candles;
	using StockSharp.BusinessEntities;
	using StockSharp.Logging;
	using StockSharp.Messages;
	using StockSharp.Localization;

	partial class Connector
	{
		/// <inheritdoc />
		public event Action<MyTrade> NewMyTrade;

		/// <inheritdoc />
		public event Action<IEnumerable<MyTrade>> NewMyTrades;

		/// <inheritdoc />
		public event Action<Trade> NewTrade;

		/// <inheritdoc />
		public event Action<IEnumerable<Trade>> NewTrades;

		/// <inheritdoc />
		public event Action<Order> NewOrder;

		/// <inheritdoc />
		public event Action<IEnumerable<Order>> NewOrders;

		/// <inheritdoc />
		public event Action<Order> OrderChanged;

		/// <inheritdoc />
		public event Action<IEnumerable<Order>> NewStopOrders;

		/// <inheritdoc />
		public event Action<IEnumerable<Order>> OrdersChanged;

		/// <inheritdoc />
		public event Action<OrderFail> OrderRegisterFailed;

		/// <inheritdoc />
		public event Action<OrderFail> OrderCancelFailed;

		/// <inheritdoc />
		public event Action<IEnumerable<Order>> StopOrdersChanged;

		/// <inheritdoc />
		public event Action<long, Exception, DateTimeOffset> OrderStatusFailed2;

		/// <inheritdoc />
		public event Action<OrderFail> StopOrderRegisterFailed;

		/// <inheritdoc />
		public event Action<OrderFail> StopOrderCancelFailed;

		/// <inheritdoc />
		public event Action<Order> NewStopOrder;

		/// <inheritdoc />
		public event Action<Order> StopOrderChanged;

		/// <inheritdoc />
		public event Action<Security> NewSecurity;

		/// <inheritdoc />
		public event Action<IEnumerable<OrderFail>> OrdersRegisterFailed;

		/// <inheritdoc />
		public event Action<IEnumerable<OrderFail>> OrdersCancelFailed;

		/// <inheritdoc />
		public event Action<long> MassOrderCanceled;

		/// <inheritdoc />
		public event Action<long, DateTimeOffset> MassOrderCanceled2;

		/// <inheritdoc />
		public event Action<long, Exception> MassOrderCancelFailed;

		/// <inheritdoc />
		public event Action<long, Exception, DateTimeOffset> MassOrderCancelFailed2;

		/// <inheritdoc />
		public event Action<long, Exception> OrderStatusFailed;

		/// <inheritdoc />
		public event Action<IEnumerable<OrderFail>> StopOrdersRegisterFailed;

		/// <inheritdoc />
		public event Action<IEnumerable<OrderFail>> StopOrdersCancelFailed;

		/// <inheritdoc />
		public event Action<IEnumerable<Security>> NewSecurities;

		/// <inheritdoc />
		public event Action<Security> SecurityChanged;

		/// <inheritdoc />
		public event Action<IEnumerable<Security>> SecuritiesChanged;

		/// <inheritdoc />
		public event Action<Portfolio> NewPortfolio;

		/// <inheritdoc />
		public event Action<IEnumerable<Portfolio>> NewPortfolios;

		/// <inheritdoc />
		public event Action<Portfolio> PortfolioChanged;

		/// <inheritdoc />
		public event Action<IEnumerable<Portfolio>> PortfoliosChanged;

		/// <inheritdoc />
		public event Action<Position> NewPosition;

		/// <inheritdoc />
		public event Action<IEnumerable<Position>> NewPositions;

		/// <inheritdoc />
		public event Action<Position> PositionChanged;

		/// <inheritdoc />
		public event Action<IEnumerable<Position>> PositionsChanged;

		/// <inheritdoc />
		public event Action<MarketDepth> NewMarketDepth;

		/// <inheritdoc />
		public event Action<MarketDepth> MarketDepthChanged;

		/// <inheritdoc />
		public event Action<IEnumerable<MarketDepth>> NewMarketDepths;

		/// <inheritdoc />
		public event Action<IEnumerable<MarketDepth>> MarketDepthsChanged;

		/// <inheritdoc />
		public event Action<OrderLogItem> NewOrderLogItem;

		/// <inheritdoc />
		public event Action<IEnumerable<OrderLogItem>> NewOrderLogItems;

		/// <inheritdoc />
		public event Action<TimeSpan> MarketTimeChanged;

		/// <inheritdoc />
		public event Action<News> NewNews;

		/// <inheritdoc />
		public event Action<News> NewsChanged;

		/// <inheritdoc />
		public event Action<Message> NewMessage;

		/// <inheritdoc />
		public event Action Connected;

		/// <inheritdoc />
		public event Action Disconnected;

		/// <inheritdoc />
		public event Action<Exception> ConnectionError;

		/// <inheritdoc />
		public event Action<IMessageAdapter> ConnectedEx;

		/// <inheritdoc />
		public event Action<IMessageAdapter> DisconnectedEx;

		/// <inheritdoc />
		public event Action<IMessageAdapter, Exception> ConnectionErrorEx;

		/// <inheritdoc cref="IConnector" />
		public event Action<Exception> Error;

		/// <inheritdoc />
		public event Action<SecurityLookupMessage, IEnumerable<Security>, Exception> LookupSecuritiesResult;

		/// <inheritdoc />
		public event Action<PortfolioLookupMessage, IEnumerable<Portfolio>, Exception> LookupPortfoliosResult;

		/// <inheritdoc />
		public event Action<BoardLookupMessage, IEnumerable<ExchangeBoard>, Exception> LookupBoardsResult;

		/// <inheritdoc />
		public event Action<SecurityLookupMessage, IEnumerable<Security>, IEnumerable<Security>, Exception> LookupSecuritiesResult2;

		/// <inheritdoc />
		public event Action<PortfolioLookupMessage, IEnumerable<Portfolio>, IEnumerable<Portfolio>, Exception> LookupPortfoliosResult2;

		/// <inheritdoc />
		public event Action<BoardLookupMessage, IEnumerable<ExchangeBoard>, IEnumerable<ExchangeBoard>, Exception> LookupBoardsResult2;

		/// <inheritdoc />
		public event Action<TimeFrameLookupMessage, IEnumerable<TimeSpan>, Exception> LookupTimeFramesResult;
		
		/// <inheritdoc />
		public event Action<TimeFrameLookupMessage, IEnumerable<TimeSpan>, IEnumerable<TimeSpan>, Exception> LookupTimeFramesResult2;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage> MarketDataSubscriptionSucceeded;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage, Exception> MarketDataSubscriptionFailed;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage, MarketDataMessage> MarketDataSubscriptionFailed2;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage> MarketDataUnSubscriptionSucceeded;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage, Exception> MarketDataUnSubscriptionFailed;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage, MarketDataMessage> MarketDataUnSubscriptionFailed2;

		/// <inheritdoc />
		public event Action<Security, MarketDataFinishedMessage> MarketDataSubscriptionFinished;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage, Exception> MarketDataUnexpectedCancelled;

		/// <inheritdoc />
		public event Action<Security, MarketDataMessage> MarketDataSubscriptionOnline;

		/// <inheritdoc />
		public event Action<ExchangeBoard, SessionStates> SessionStateChanged;

		/// <inheritdoc />
		public event Action<Security, IEnumerable<KeyValuePair<Level1Fields, object>>, DateTimeOffset, DateTimeOffset> ValuesChanged;

		/// <inheritdoc />
		public event Action<Subscription, Level1ChangeMessage> Level1Received;

		/// <inheritdoc />
		public event Action<Subscription, Trade> TickTradeReceived;

		/// <inheritdoc />
		public event Action<Subscription, Security> SecurityReceived;

		/// <inheritdoc />
		public event Action<Subscription, ExchangeBoard> BoardReceived;

		/// <inheritdoc />
		public event Action<Subscription, MarketDepth> MarketDepthReceived;

		/// <inheritdoc />
		public event Action<Subscription, OrderLogItem> OrderLogItemReceived;

		/// <inheritdoc />
		public event Action<Subscription, News> NewsReceived;

		/// <inheritdoc />
		public event Action<Subscription, Candle> CandleReceived;

		/// <inheritdoc />
		public event Action<Subscription, MyTrade> OwnTradeReceived;

		/// <inheritdoc />
		public event Action<Subscription, Order> OrderReceived;

		/// <inheritdoc />
		public event Action<Subscription, OrderFail> OrderRegisterFailReceived;

		/// <inheritdoc />
		public event Action<Subscription, OrderFail> OrderCancelFailReceived;

		/// <inheritdoc />
		public event Action<Subscription, Portfolio> PortfolioReceived;

		/// <inheritdoc />
		public event Action<Subscription, Position> PositionReceived;

		/// <inheritdoc />
		public event Action<Subscription> SubscriptionOnline;

		/// <inheritdoc />
		public event Action<Subscription> SubscriptionStarted;

		/// <inheritdoc />
		public event Action<Subscription, Exception> SubscriptionStopped;

		/// <inheritdoc />
		public event Action<Subscription, Exception, bool> SubscriptionFailed;

		/// <summary>
		/// Connection restored.
		/// </summary>
		[Obsolete("Use ConnectionRestored event.")]
		public event Action Restored;

		/// <inheritdoc />
		public event Action<IMessageAdapter> ConnectionRestored;

		/// <inheritdoc />
		public event Action<IMessageAdapter> ConnectionLost;

		/// <summary>
		/// Connection timed-out.
		/// </summary>
		[Obsolete("Use ConnectionError event.")]
		public event Action TimeOut;

		/// <summary>
		/// A new value for processing occurrence event.
		/// </summary>
		public event Action<CandleSeries, Candle> CandleSeriesProcessing;

		/// <summary>
		/// The series processing end event.
		/// </summary>
		public event Action<CandleSeries> CandleSeriesStopped;

		/// <summary>
		/// The series error event.
		/// </summary>
		public event Action<CandleSeries, MarketDataMessage> CandleSeriesError;

		/// <inheritdoc />
		public event Action<Order> OrderInitialized;

		/// <inheritdoc />
		public event Action<long, Exception> ChangePasswordResult;

		private void RaiseOrderInitialized(Order order)
		{
			OrderInitialized?.Invoke(order);
		}

		private void RaiseNewMyTrade(MyTrade trade)
		{
			this.AddInfoLog("New own trade: {0}", trade);

			NewMyTrade?.Invoke(trade);
			NewMyTrades?.Invoke(new[] { trade });
		}

		private void RaiseNewTrade(Trade trade)
		{
			NewTrade?.Invoke(trade);
			NewTrades?.Invoke(new[] { trade });
		}

		private void RaiseNewOrder(Order order)
		{
			NewOrder?.Invoke(order);
			NewOrders?.Invoke(new[] { order });
		}

		private void RaiseOrderChanged(Order order)
		{
			OrderChanged?.Invoke(order);
			OrdersChanged?.Invoke(new[] { order });
		}

		/// <summary>
		/// To call the event <see cref="NewStopOrders"/>.
		/// </summary>
		/// <param name="stopOrder">Stop order that should be passed to the event.</param>
		private void RaiseNewStopOrder(Order stopOrder)
		{
			NewStopOrder?.Invoke(stopOrder);
			NewStopOrders?.Invoke(new[] { stopOrder });
		}

		/// <summary>
		/// To call the event <see cref="StopOrdersChanged"/>.
		/// </summary>
		/// <param name="stopOrder">Stop orders that should be passed to the event.</param>
		private void RaiseStopOrderChanged(Order stopOrder)
		{
			StopOrderChanged?.Invoke(stopOrder);
			StopOrdersChanged?.Invoke(new[] { stopOrder });
		}

		private void RaiseOrderRegisterFailed(OrderFail fail)
		{
			OrderRegisterFailed?.Invoke(fail);
			OrdersRegisterFailed?.Invoke(new[] { fail });
		}

		private void RaiseOrderCancelFailed(OrderFail fail)
		{
			OrderCancelFailed?.Invoke(fail);
			OrdersCancelFailed?.Invoke(new[] { fail });
		}

		/// <summary>
		/// To call the event <see cref="StopOrdersRegisterFailed"/>.
		/// </summary>
		/// <param name="fail">Error information that should be passed to the event.</param>
		private void RaiseStopOrdersRegisterFailed(OrderFail fail)
		{
			StopOrderRegisterFailed?.Invoke(fail);
			StopOrdersRegisterFailed?.Invoke(new[] { fail });
		}

		/// <summary>
		/// To call the event <see cref="StopOrdersCancelFailed"/>.
		/// </summary>
		/// <param name="fail">Error information that should be passed to the event.</param>
		private void RaiseStopOrdersCancelFailed(OrderFail fail)
		{
			StopOrderCancelFailed?.Invoke(fail);
			StopOrdersCancelFailed?.Invoke(new[] { fail });
		}

		private void RaiseMassOrderCanceled(long transactionId, DateTimeOffset time)
		{
			MassOrderCanceled?.Invoke(transactionId);
			MassOrderCanceled2?.Invoke(transactionId, time);
		}

		private void RaiseMassOrderCancelFailed(long transactionId, Exception error, DateTimeOffset time)
		{
			MassOrderCancelFailed?.Invoke(transactionId, error);
			MassOrderCancelFailed2?.Invoke(transactionId, error, time);
		}

		private void RaiseOrderStatusFailed(long transactionId, Exception error, DateTimeOffset time)
		{
			OrderStatusFailed?.Invoke(transactionId, error);
			OrderStatusFailed2?.Invoke(transactionId, error, time);
		}

		private void RaiseNewSecurity(Security security)
		{
			var arr = new[] { security };

            _added?.Invoke(arr);

			NewSecurity?.Invoke(security);
			NewSecurities?.Invoke(arr);
		}

		private void RaiseSecuritiesChanged(Security[] securities)
		{
			SecuritiesChanged?.Invoke(securities);

			var evt = SecurityChanged;

			if (evt == null)
				return;

			foreach (var security in securities)
				evt(security);
		}

		private void RaiseSecurityChanged(Security security)
		{
			SecurityChanged?.Invoke(security);
			SecuritiesChanged?.Invoke(new[] { security });
		}

		private void RaiseNewPortfolio(Portfolio portfolio)
		{
			NewPortfolio?.Invoke(portfolio);
			NewPortfolios?.Invoke(new[] { portfolio });
		}

		private void RaisePortfolioChanged(Portfolio portfolio)
		{
			PortfolioChanged?.Invoke(portfolio);
			PortfoliosChanged?.Invoke(new[] { portfolio });
		}

		private void RaiseNewPosition(Position position)
		{
			NewPosition?.Invoke(position);
			NewPositions?.Invoke(new[] { position });
		}

		private void RaisePositionChanged(Position position)
		{
			PositionChanged?.Invoke(position);
			PositionsChanged?.Invoke(new[] { position });
		}

		private void RaiseNewMarketDepth(MarketDepth marketDepth)
		{
			NewMarketDepth?.Invoke(marketDepth);
			NewMarketDepths?.Invoke(new[] { marketDepth });
		}

		private void RaiseMarketDepthChanged(MarketDepth marketDepth)
		{
			MarketDepthChanged?.Invoke(marketDepth);
			MarketDepthsChanged?.Invoke(new[] { marketDepth });
		}

		/// <summary>
		/// To call the event <see cref="NewNews"/>.
		/// </summary>
		/// <param name="news">News.</param>
		private void RaiseNewNews(News news)
		{
			NewNews?.Invoke(news);
		}

		/// <summary>
		/// To call the event <see cref="NewsChanged"/>.
		/// </summary>
		/// <param name="news">News.</param>
		private void RaiseNewsChanged(News news)
		{
			NewsChanged?.Invoke(news);
		}

		private void RaiseNewOrderLogItem(OrderLogItem item)
		{
			NewOrderLogItem?.Invoke(item);
			NewOrderLogItems?.Invoke(new[] { item });
		}

		/// <summary>
		/// To call the event <see cref="Connected"/>.
		/// </summary>
		private void RaiseConnected()
		{
			ConnectionState = ConnectionStates.Connected;
			Connected?.Invoke();
		}

		/// <summary>
		/// To call the event <see cref="ConnectedEx"/>.
		/// </summary>
		/// <param name="adapter">Adapter, initiated event.</param>
		private void RaiseConnectedEx(IMessageAdapter adapter)
		{
			ConnectedEx?.Invoke(adapter);
		}

		/// <summary>
		/// To call the event <see cref="Disconnected"/>.
		/// </summary>
		private void RaiseDisconnected()
		{
			ConnectionState = ConnectionStates.Disconnected;
			Disconnected?.Invoke();
		}

		/// <summary>
		/// To call the event <see cref="DisconnectedEx"/>.
		/// </summary>
		/// <param name="adapter">Adapter, initiated event.</param>
		private void RaiseDisconnectedEx(IMessageAdapter adapter)
		{
			DisconnectedEx?.Invoke(adapter);
		}

		/// <summary>
		/// To call the event <see cref="ConnectionError"/>.
		/// </summary>
		/// <param name="exception">Error connection.</param>
		private void RaiseConnectionError(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException(nameof(exception));

			ConnectionState = ConnectionStates.Failed;
			ConnectionError?.Invoke(exception);

			this.AddErrorLog(exception);
		}

		/// <summary>
		/// To call the event <see cref="ConnectionErrorEx"/>.
		/// </summary>
		/// <param name="adapter">Adapter, initiated event.</param>
		/// <param name="exception">Error connection.</param>
		private void RaiseConnectionErrorEx(IMessageAdapter adapter, Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException(nameof(exception));

			ConnectionErrorEx?.Invoke(adapter, exception);
		}

		/// <summary>
		/// To call the event <see cref="Connector.Error"/>.
		/// </summary>
		/// <param name="exception">Data processing error.</param>
		protected void RaiseError(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException(nameof(exception));

			ErrorCount++;

			this.AddErrorLog(exception);
			Error?.Invoke(exception);
		}

		/// <summary>
		/// To call the event <see cref="MarketTimeChanged"/>.
		/// </summary>
		/// <param name="diff">The difference in the time since the last call of the event. The first time the event passes the <see cref="TimeSpan.Zero"/> value.</param>
		private void RaiseMarketTimeChanged(TimeSpan diff)
		{
			MarketTimeChanged?.Invoke(diff);
		}

		/// <summary>
		/// To call the event <see cref="LookupSecuritiesResult"/>.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="error">An error of lookup operation. The value will be <see langword="null"/> if operation complete successfully.</param>
		/// <param name="securities">Found instruments.</param>
		/// <param name="newSecurities">Newly created.</param>
		private void RaiseLookupSecuritiesResult(SecurityLookupMessage message, Exception error, Security[] securities, Security[] newSecurities)
		{
			LookupSecuritiesResult?.Invoke(message, securities, error);
			LookupSecuritiesResult2?.Invoke(message, securities, newSecurities, error);
		}

		/// <summary>
		/// To call the event <see cref="LookupBoardsResult"/>.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="error">An error of lookup operation. The value will be <see langword="null"/> if operation complete successfully.</param>
		/// <param name="boards">Found boards.</param>
		/// <param name="newBoards">Newly created.</param>
		private void RaiseLookupBoardsResult(BoardLookupMessage message, Exception error, ExchangeBoard[] boards, ExchangeBoard[] newBoards)
		{
			LookupBoardsResult?.Invoke(message, boards, error);
			LookupBoardsResult2?.Invoke(message, boards, newBoards, error);
		}

		/// <summary>
		/// To call the event <see cref="LookupTimeFramesResult"/>.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="error">An error of lookup operation. The value will be <see langword="null"/> if operation complete successfully.</param>
		/// <param name="timeFrames">Found time-frames.</param>
		/// <param name="newTimeFrames">Newly created.</param>
		private void RaiseLookupTimeFramesResult(TimeFrameLookupMessage message, Exception error, TimeSpan[] timeFrames, TimeSpan[] newTimeFrames)
		{
			LookupTimeFramesResult?.Invoke(message, timeFrames, error);
			LookupTimeFramesResult2?.Invoke(message, timeFrames, newTimeFrames, error);
		}

		/// <summary>
		/// To call the event <see cref="LookupPortfoliosResult"/>.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="error">An error of lookup operation. The value will be <see langword="null"/> if operation complete successfully.</param>
		/// <param name="portfolios">Found portfolios.</param>
		/// <param name="newPortfolios">Newly created.</param>
		private void RaiseLookupPortfoliosResult(PortfolioLookupMessage message, Exception error, Portfolio[] portfolios, Portfolio[] newPortfolios)
		{
			LookupPortfoliosResult?.Invoke(message, portfolios, error);
			LookupPortfoliosResult2?.Invoke(message, portfolios, newPortfolios, error);
		}

		private void RaiseMarketDataSubscriptionSucceeded(MarketDataMessage message, Subscription subscription)
		{
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			var security = subscription.Security;

			var msg = LocalizedStrings.SubscribedOk.Put(security?.Id,
				message.DataType + (message.DataType.IsCandleDataType() ? " " + message.Arg : string.Empty));

			if (message.From != null && message.To != null)
				msg += LocalizedStrings.Str691Params.Put(message.From.Value, message.To.Value);

			this.AddDebugLog(msg + ".");

			MarketDataSubscriptionSucceeded?.Invoke(security, message);

			RaiseSubscriptionStarted(subscription);
		}

		private void RaiseMarketDataSubscriptionFailed(MarketDataMessage origin, MarketDataMessage reply, Subscription subscription)
		{
			if (origin == null)
				throw new ArgumentNullException(nameof(origin));

			if (reply == null)
				throw new ArgumentNullException(nameof(reply));

			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			var security = subscription.Security;
			var error = reply.Error ?? new NotSupportedException(LocalizedStrings.SubscriptionNotSupported.Put(origin));

			if (reply.IsNotSupported)
				this.AddWarningLog(LocalizedStrings.SubscriptionNotSupported, origin);
			else
				this.AddErrorLog(LocalizedStrings.SubscribedError, security?.Id, origin.DataType, error.Message);

			MarketDataSubscriptionFailed?.Invoke(security, origin, error);
			MarketDataSubscriptionFailed2?.Invoke(security, origin, reply);

			RaiseSubscriptionFailed(subscription, error, true);

			if (subscription.CandleSeries != null)
				RaiseCandleSeriesError(subscription.CandleSeries, reply);
		}

		private void RaiseMarketDataUnSubscriptionSucceeded(MarketDataMessage message, Subscription subscription)
		{
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			var security = subscription.Security;

			var msg = LocalizedStrings.UnSubscribedOk.Put(security?.Id,
				message.DataType + (message.DataType.IsCandleDataType() ? " " + message.Arg : string.Empty));

			if (message.From != null && message.To != null)
				msg += LocalizedStrings.Str691Params.Put(message.From.Value, message.To.Value);

			this.AddDebugLog(msg + ".");
			MarketDataUnSubscriptionSucceeded?.Invoke(security, message);

			RaiseSubscriptionStopped(subscription, null);

			if (subscription.CandleSeries != null)
				RaiseCandleSeriesStopped(subscription.CandleSeries);
		}

		private void RaiseMarketDataUnSubscriptionFailed(MarketDataMessage origin, MarketDataMessage reply, Subscription subscription)
		{
			if (origin == null)
				throw new ArgumentNullException(nameof(origin));

			if (reply == null)
				throw new ArgumentNullException(nameof(reply));

			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			var security = subscription.Security;
			var error = reply.Error ?? new NotSupportedException();

			this.AddErrorLog(LocalizedStrings.UnSubscribedError, security?.Id, origin.DataType, error.Message);
			MarketDataUnSubscriptionFailed?.Invoke(security, origin, error);
			MarketDataUnSubscriptionFailed2?.Invoke(security, origin, reply);

			RaiseSubscriptionFailed(subscription, error, false);
		}

		private void RaiseMarketDataSubscriptionFinished(MarketDataFinishedMessage message, Subscription subscription)
		{
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			var security = subscription.Security;

			this.AddDebugLog(LocalizedStrings.SubscriptionFinished, security?.Id, message);
			MarketDataSubscriptionFinished?.Invoke(security, message);

			RaiseSubscriptionStopped(subscription, null);

			if (subscription.CandleSeries != null)
				RaiseCandleSeriesStopped(subscription.CandleSeries);
		}

		private void RaiseMarketDataUnexpectedCancelled(MarketDataMessage message, Exception error, Subscription subscription)
		{
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			if (error == null)
				throw new ArgumentNullException(nameof(error));

			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			var security = subscription.Security;

			this.AddErrorLog(LocalizedStrings.SubscriptionUnexpectedCancelled, security?.Id, message.DataType, error.Message);
			MarketDataUnexpectedCancelled?.Invoke(security, message, error);

			RaiseSubscriptionStopped(subscription, error);

			if (subscription.CandleSeries != null)
				RaiseCandleSeriesStopped(subscription.CandleSeries);
		}

		private void RaiseSubscriptionOnline(Subscription subscription)
		{
			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			SubscriptionOnline?.Invoke(subscription);
		}

		private void RaiseSubscriptionStarted(Subscription subscription)
		{
			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			SubscriptionStarted?.Invoke(subscription);
		}

		private void RaiseSubscriptionStopped(Subscription subscription, Exception error)
		{
			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			SubscriptionStopped?.Invoke(subscription, error);
		}

		private void RaiseSubscriptionFailed(Subscription subscription, Exception error, bool isSubscribe)
		{
			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			if (error == null)
				throw new ArgumentNullException(nameof(error));

			SubscriptionFailed?.Invoke(subscription, error, isSubscribe);
		}

		private void RaiseMarketDataSubscriptionOnline(Subscription subscription)
		{
			if (subscription == null)
				throw new ArgumentNullException(nameof(subscription));

			var security = subscription.Security;
			
			this.AddDebugLog(LocalizedStrings.SubscriptionOnline, security?.Id, subscription.SubscriptionMessage);

			if (subscription.SubscriptionMessage is MarketDataMessage mdMsg)
				MarketDataSubscriptionOnline?.Invoke(security, mdMsg);

			RaiseSubscriptionOnline(subscription);
		}

		/// <summary>
		/// To call the event <see cref="NewMessage"/>.
		/// </summary>
		/// <param name="message">A new message.</param>
		private void RaiseNewMessage(Message message)
		{
			NewMessage?.Invoke(message);
			_newOutMessage?.Invoke(message);
		}

		private void RaiseValuesChanged(Security security, IEnumerable<KeyValuePair<Level1Fields, object>> changes, DateTimeOffset serverTime, DateTimeOffset localTime)
		{
			ValuesChanged?.Invoke(security, changes, serverTime, localTime);
		}

		private void RaiseConnectionLost(IMessageAdapter adapter)
		{
			ConnectionLost?.Invoke(adapter);
		}

		private void RaiseConnectionRestored(IMessageAdapter adapter)
		{
#pragma warning disable 618
			Restored?.Invoke();
#pragma warning restore 618

			if (adapter != null)
				ConnectionRestored?.Invoke(adapter);
		}

		private void RaiseTimeOut()
		{
#pragma warning disable 618
			TimeOut?.Invoke();
#pragma warning restore 618
		}

		private void RaiseCandleSeriesProcessing(CandleSeries series, Candle candle)
		{
			CandleSeriesProcessing?.Invoke(series, candle);
		}

		private void RaiseCandleSeriesStopped(CandleSeries series)
		{
			CandleSeriesStopped?.Invoke(series);
		}

		private void RaiseCandleSeriesError(CandleSeries series, MarketDataMessage reply)
		{
			CandleSeriesError?.Invoke(series, reply);
		}

		private void RaiseSessionStateChanged(ExchangeBoard board, SessionStates state)
		{
			SessionStateChanged?.Invoke(board, state);
		}

		private void RaiseChangePassword(long transactionId, Exception error)
		{
			ChangePasswordResult?.Invoke(transactionId, error);
		}

		private bool RaiseReceived<TEntity>(TEntity entity, ISubscriptionIdMessage message, Action<Subscription, TEntity> evt)
		{
			var anyOnline = false;

			foreach (var subscription in _subscriptionManager.GetSubscriptions(message))
			{
				if (!anyOnline && subscription.State == SubscriptionStates.Online)
					anyOnline = true;

				evt?.Invoke(subscription, entity);
			}

			return anyOnline;
		}
	}
}