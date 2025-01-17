﻿#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.Messages.Messages
File: IMessageAdapterWrapper.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.Messages
{
	using System;
	using System.Collections.Generic;

	using Ecng.Common;
	using Ecng.Serialization;

	using StockSharp.Logging;

	/// <summary>
	/// Wrapping based adapter.
	/// </summary>
	public interface IMessageAdapterWrapper : IMessageAdapter
	{
		/// <summary>
		/// Underlying adapter.
		/// </summary>
		IMessageAdapter InnerAdapter { get; set; }
	}

	/// <summary>
	/// Base implementation of <see cref="IMessageAdapterWrapper"/>.
	/// </summary>
	public abstract class MessageAdapterWrapper : Cloneable<IMessageChannel>, IMessageAdapterWrapper
	{
		private IMessageAdapter _innerAdapter;

		/// <summary>
		/// Initialize <see cref="MessageAdapterWrapper"/>.
		/// </summary>
		/// <param name="innerAdapter">Underlying adapter.</param>
		protected MessageAdapterWrapper(IMessageAdapter innerAdapter)
		{
			InnerAdapter = innerAdapter ?? throw new ArgumentNullException(nameof(innerAdapter));
		}

		/// <inheritdoc />
		public IMessageAdapter InnerAdapter
		{
			get => _innerAdapter;
			set
			{
				if (_innerAdapter == value)
					return;

				if (_innerAdapter != null)
					_innerAdapter.NewOutMessage -= InnerAdapterNewOutMessage;

				_innerAdapter = value;

				if (_innerAdapter == null)
					throw new ArgumentException();

				_innerAdapter.NewOutMessage += InnerAdapterNewOutMessage;
			}
		}

		/// <summary>
		/// Control <see cref="InnerAdapter"/> lifetime.
		/// </summary>
		public bool OwnInnerAdapter { get; set; }

		/// <summary>
		/// Process <see cref="InnerAdapter"/> output message.
		/// </summary>
		/// <param name="message">The message.</param>
		protected virtual void InnerAdapterNewOutMessage(Message message)
		{
			if (message.IsBack)
				RaiseNewOutMessage(message);
			else
				OnInnerAdapterNewOutMessage(message);
		}

		/// <summary>
		/// Process <see cref="InnerAdapter"/> output message.
		/// </summary>
		/// <param name="message">The message.</param>
		protected virtual void OnInnerAdapterNewOutMessage(Message message)
		{
			RaiseNewOutMessage(message);
		}

		/// <summary>
		/// To call the event <see cref="NewOutMessage"/>.
		/// </summary>
		/// <param name="message">The message.</param>
		protected void RaiseNewOutMessage(Message message)
		{
			NewOutMessage?.Invoke(message);
		}

		bool IMessageChannel.IsOpened => InnerAdapter.IsOpened;

		void IMessageChannel.Open()
		{
			InnerAdapter.Open();
		}

		void IMessageChannel.Close()
		{
			InnerAdapter.Close();
		}

		event Action IMessageChannel.StateChanged
		{
			add => InnerAdapter.StateChanged += value;
			remove => InnerAdapter.StateChanged -= value;
		}

		/// <summary>
		/// Auto send <see cref="Message.IsBack"/> messages to <see cref="InnerAdapter"/>.
		/// </summary>
		protected virtual bool SendInBackFurther => true;

		/// <inheritdoc />
		public virtual void SendInMessage(Message message)
		{
			if (message.IsBack)
			{
				if (message.Adapter == this)
				{
					message.Adapter = null;
					message.IsBack = false;
				}
				else
				{
					if (SendInBackFurther)
					{
						InnerAdapter.SendInMessage(message);
						return;
					}
				}
			}

			try
			{
				OnSendInMessage(message);
			}
			catch (Exception ex)
			{
				this.AddErrorLog(ex);

				message.HandleErrorResponse(ex, CurrentTime, RaiseNewOutMessage);

				throw;
			}
		}

		/// <summary>
		/// Send message.
		/// </summary>
		/// <param name="message">Message.</param>
		protected virtual void OnSendInMessage(Message message)
		{
			InnerAdapter.SendInMessage(message);
		}

		/// <inheritdoc />
		public virtual event Action<Message> NewOutMessage;

		/// <summary>
		/// Load settings.
		/// </summary>
		/// <param name="storage">Settings storage.</param>
		public virtual void Load(SettingsStorage storage)
		{
			InnerAdapter.Load(storage);
		}

		/// <summary>
		/// Save settings.
		/// </summary>
		/// <param name="storage">Settings storage.</param>
		public virtual void Save(SettingsStorage storage)
		{
			InnerAdapter.Save(storage);
		}

		Guid ILogSource.Id => InnerAdapter.Id;

		string ILogSource.Name
		{
			get => InnerAdapter.Name;
			set => InnerAdapter.Name = value;
		}

		/// <inheritdoc />
		public virtual ILogSource Parent
		{
			get => InnerAdapter.Parent;
			set => InnerAdapter.Parent = value;
		}

		LogLevels ILogSource.LogLevel
		{
			get => InnerAdapter.LogLevel;
			set => InnerAdapter.LogLevel = value;
		}

		/// <inheritdoc />
		public DateTimeOffset CurrentTime => InnerAdapter.CurrentTime;

		bool ILogSource.IsRoot => InnerAdapter.IsRoot;

		event Action<LogMessage> ILogSource.Log
		{
			add => InnerAdapter.Log += value;
			remove => InnerAdapter.Log -= value;
		}

		void ILogReceiver.AddLog(LogMessage message)
		{
			InnerAdapter.AddLog(message);
		}

		/// <inheritdoc />
		public bool CheckTimeFrameByRequest => InnerAdapter.CheckTimeFrameByRequest;

		/// <inheritdoc />
		public ReConnectionSettings ReConnectionSettings => InnerAdapter.ReConnectionSettings;

		/// <inheritdoc />
		public IdGenerator TransactionIdGenerator => InnerAdapter.TransactionIdGenerator;

		/// <inheritdoc />
		public virtual IEnumerable<MessageTypeInfo> PossibleSupportedMessages
		{
			get => InnerAdapter.PossibleSupportedMessages;
			set => InnerAdapter.PossibleSupportedMessages = value;
		}

		/// <inheritdoc />
		public virtual IEnumerable<MessageTypes> SupportedInMessages
		{
			get => InnerAdapter.SupportedInMessages;
			set => InnerAdapter.SupportedInMessages = value;
		}

		/// <inheritdoc />
		public virtual IEnumerable<MessageTypes> SupportedOutMessages
		{
			get => InnerAdapter.SupportedOutMessages;
			set => InnerAdapter.SupportedOutMessages = value;
		}

		/// <inheritdoc />
		public virtual IEnumerable<MarketDataTypes> SupportedMarketDataTypes
		{
			get => InnerAdapter.SupportedMarketDataTypes;
			set => InnerAdapter.SupportedMarketDataTypes = value;
		}

		IDictionary<string, RefPair<SecurityTypes, string>> IMessageAdapter.SecurityClassInfo => InnerAdapter.SecurityClassInfo;

		/// <inheritdoc />
		public TimeSpan HeartbeatInterval
		{
			get => InnerAdapter.HeartbeatInterval;
			set => InnerAdapter.HeartbeatInterval = value;
		}

		/// <inheritdoc />
		public string StorageName => InnerAdapter.StorageName;

		/// <inheritdoc />
		public virtual bool IsNativeIdentifiersPersistable => InnerAdapter.IsNativeIdentifiersPersistable;

		/// <inheritdoc />
		public virtual bool IsNativeIdentifiers => InnerAdapter.IsNativeIdentifiers;

		/// <inheritdoc />
		public virtual bool IsFullCandlesOnly => InnerAdapter.IsFullCandlesOnly;

		/// <inheritdoc />
		public virtual bool IsSupportSubscriptions => InnerAdapter.IsSupportSubscriptions;

		/// <inheritdoc />
		public virtual bool IsSupportCandlesUpdates => InnerAdapter.IsSupportCandlesUpdates;

		/// <inheritdoc />
		public virtual MessageAdapterCategories Categories => InnerAdapter.Categories;

		/// <inheritdoc />
		public virtual OrderCancelVolumeRequireTypes? OrderCancelVolumeRequired => InnerAdapter.OrderCancelVolumeRequired;

		IEnumerable<Tuple<string, Type>> IMessageAdapter.SecurityExtendedFields => InnerAdapter.SecurityExtendedFields;

		/// <inheritdoc />
		public virtual IEnumerable<int> SupportedOrderBookDepths => InnerAdapter.SupportedOrderBookDepths;

		/// <inheritdoc />
		public virtual bool IsSupportOrderBookIncrements => InnerAdapter.IsSupportOrderBookIncrements;

		/// <inheritdoc />
		public virtual bool IsSupportExecutionsPnL => InnerAdapter.IsSupportExecutionsPnL;

		/// <inheritdoc />
		public virtual bool IsSecurityNewsOnly => InnerAdapter.IsSecurityNewsOnly;

		/// <inheritdoc />
		public IEnumerable<Level1Fields> CandlesBuildFrom => InnerAdapter.CandlesBuildFrom;

		Type IMessageAdapter.OrderConditionType => InnerAdapter.OrderConditionType;

		bool IMessageAdapter.HeartbeatBeforConnect => InnerAdapter.HeartbeatBeforConnect;

		IOrderLogMarketDepthBuilder IMessageAdapter.CreateOrderLogMarketDepthBuilder(SecurityId securityId)
			=> InnerAdapter.CreateOrderLogMarketDepthBuilder(securityId);

		/// <inheritdoc />
		public virtual IEnumerable<object> GetCandleArgs(Type candleType, SecurityId securityId, DateTimeOffset? from, DateTimeOffset? to)
			=> InnerAdapter.GetCandleArgs(candleType, securityId, from, to);

		/// <inheritdoc />
		public virtual TimeSpan GetHistoryStepSize(DataType dataType, out TimeSpan iterationInterval)
			=> InnerAdapter.GetHistoryStepSize(dataType, out iterationInterval);

		/// <inheritdoc />
		public virtual bool IsAllDownloadingSupported(DataType dataType)
			=> InnerAdapter.IsAllDownloadingSupported(dataType);

		/// <inheritdoc />
		public virtual bool IsSecurityRequired(DataType dataType)
			=> InnerAdapter.IsSecurityRequired(dataType);

		/// <inheritdoc />
		public virtual void Dispose()
		{
			InnerAdapter.NewOutMessage -= InnerAdapterNewOutMessage;

			if (OwnInnerAdapter)
				InnerAdapter.Dispose();
		}

		/// <inheritdoc />
		public override string ToString() => InnerAdapter.ToString();
	}
}