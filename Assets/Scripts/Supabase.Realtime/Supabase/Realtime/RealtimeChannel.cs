using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Supabase.Realtime.Broadcast;
using Supabase.Realtime.Channel;
using Supabase.Realtime.Exceptions;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;
using Supabase.Realtime.Presence;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime
{
	public class RealtimeChannel : IRealtimeChannel
	{
		internal delegate void BroadcastEventHandler(IRealtimeChannel sender, SocketResponse response);

		internal delegate void PresenceDiffHandler(IRealtimeChannel sender, SocketResponse response);

		internal delegate void PresenceSyncHandler(IRealtimeChannel sender, SocketResponse response);

		private sealed class _003C_003Ec__DisplayClass83_0
		{
			public RealtimeChannel _003C_003E4__this;

			public IRealtimeChannel.StateChangedHandler channelCallback;

			public EventHandler joinPushTimeoutCallback;

			public TaskCompletionSource<IRealtimeChannel> tsc;

			internal void _003CSubscribe_003Eb__0(IRealtimeChannel sender, Constants.ChannelState state)
			{
			}

			internal void _003CSubscribe_003Eb__1(object _1, EventArgs _2)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass86_0
		{
			public TaskCompletionSource<bool> tsc;

			public Push push;

			public IRealtimePush<RealtimeChannel, SocketResponse>.MessageEventHandler messageCallback;

			internal void _003CSend_003Eb__0(IRealtimePush<RealtimeChannel, SocketResponse> _, SocketResponse message)
			{
			}
		}

		private readonly string Topic__BackingField;

		private Constants.ChannelState State__BackingField;

		private readonly ChannelOptions Options__BackingField;

		private BroadcastOptions BroadcastOptions__BackingField;

		private PresenceOptions PresenceOptions__BackingField;

		private readonly List<PostgresChangesOptions> PostgresChangesOptions__BackingField;

		private bool HasJoinedOnce__BackingField;

		public bool IsSubscribed;

		internal Push JoinPush;

		internal Push LastPush;

		internal BroadcastEventHandler BroadcastHandler;

		internal PresenceDiffHandler PresenceDiff;

		internal PresenceSyncHandler PresenceSync;

		private readonly List<Push> _buffer;

		internal readonly IRealtimeSocket Socket;

		private IRealtimePresence _presence;

		private IRealtimeBroadcast _broadcast;

		private RealtimeException _exception;

		private readonly List<IRealtimeChannel.StateChangedHandler> _stateChangedHandlers;

		private readonly List<IRealtimeChannel.MessageReceivedHandler> _messageReceivedHandlers;

		private readonly List<IRealtimeChannel.ErrorEventHandler> _errorEventHandlers;

		private readonly Dictionary<PostgresChangesOptions.ListenType, List<IRealtimeChannel.PostgresChangesHandler>> _postgresChangesHandlers;

		private bool _hasJoinedOnce;

		private readonly Timer _rejoinTimer;

		private bool _isRejoining;

		public bool IsClosed => false;

		public bool IsErrored => false;

		public bool IsJoined => false;

		public bool IsJoining => false;

		public bool IsLeaving => false;

		public string Topic => Topic__BackingField;

		public Constants.ChannelState State
		{
			get
			{
				return State__BackingField;
			}
			private set
			{
				State__BackingField = value;
			}
		}

		public ChannelOptions Options => Options__BackingField;

		public BroadcastOptions BroadcastOptions
		{
			get
			{
				return BroadcastOptions__BackingField;
			}
			private set
			{
				BroadcastOptions__BackingField = value;
			}
		}

		public PresenceOptions PresenceOptions
		{
			get
			{
				return PresenceOptions__BackingField;
			}
			private set
			{
				PresenceOptions__BackingField = value;
			}
		}

		public List<PostgresChangesOptions> PostgresChangesOptions => PostgresChangesOptions__BackingField;

		public bool HasJoinedOnce
		{
			get
			{
				return HasJoinedOnce__BackingField;
			}
			private set
			{
				HasJoinedOnce__BackingField = value;
			}
		}

		private bool CanPush => false;

		public IRealtimeBroadcast Broadcast()
		{
			return null;
		}

		public RealtimeBroadcast<TBroadcastModel> Broadcast<TBroadcastModel>()
		{
			return null;
		}

		public IRealtimePresence Presence()
		{
			return null;
		}

		public RealtimePresence<TPresenceModel> Presence<TPresenceModel>()
		{
			return null;
		}

		public RealtimeChannel(IRealtimeSocket socket, string channelName, ChannelOptions options)
		{
		}

		private void HandleSocketStateChanged(IRealtimeSocket _, Constants.SocketState state)
		{
		}

		public RealtimeBroadcast<TBroadcastResponse> Register<TBroadcastResponse>(bool broadcastSelf = false, bool broadcastAck = false)
		{
			return null;
		}

		public RealtimePresence<TPresenceResponse> Register<TPresenceResponse>(string presenceKey)
		{
			return null;
		}

		public void AddStateChangedHandler(IRealtimeChannel.StateChangedHandler stateChangedHandler)
		{
		}

		public void RemoveStateChangedHandler(IRealtimeChannel.StateChangedHandler stateChangedHandler)
		{
		}

		public void ClearStateChangedHandlers()
		{
		}

		private void NotifyStateChanged(Constants.ChannelState state, bool shouldRejoin = true)
		{
		}

		public void AddMessageReceivedHandler(IRealtimeChannel.MessageReceivedHandler messageReceivedHandler)
		{
		}

		public void RemoveMessageReceivedHandler(IRealtimeChannel.MessageReceivedHandler messageReceivedHandler)
		{
		}

		public void ClearMessageReceivedHandlers()
		{
		}

		private void NotifyMessageReceived(SocketResponse message)
		{
		}

		public void AddPostgresChangeHandler(PostgresChangesOptions.ListenType listenType, IRealtimeChannel.PostgresChangesHandler postgresChangeHandler)
		{
		}

		public void RemovePostgresChangeHandler(PostgresChangesOptions.ListenType listenType, IRealtimeChannel.PostgresChangesHandler postgresChangeHandler)
		{
		}

		public void ClearPostgresChangeHandlers()
		{
		}

		public void AddErrorHandler(IRealtimeChannel.ErrorEventHandler handler)
		{
		}

		public void RemoveErrorHandler(IRealtimeChannel.ErrorEventHandler handler)
		{
		}

		public void ClearErrorHandlers()
		{
		}

		private void NotifyErrorOccurred(RealtimeException exception)
		{
		}

		private void NotifyPostgresChanges(Constants.EventType eventType, PostgresChangesResponse response)
		{
		}

		public IRealtimeChannel Register(PostgresChangesOptions postgresChangesOptions)
		{
			return null;
		}

		public Task<IRealtimeChannel> Subscribe(int timeoutMs = 10000)
		{
			return null;
		}

		public IRealtimeChannel Unsubscribe()
		{
			return null;
		}

		public Push Push(string eventName, string type = null, object payload = null, int timeoutMs = 10000)
		{
			return null;
		}

		public Task<bool> Send(Constants.ChannelEventName eventName, string type, object payload, int timeoutMs = 10000)
		{
			return null;
		}

		public void Rejoin(int timeoutMs = 10000)
		{
		}

		internal void Enqueue(Push push)
		{
		}

		private Push GenerateJoinPush()
		{
			return null;
		}

		private Push GenerateAuthPush()
		{
			return null;
		}

		private void HandleRejoinTimerElapsed(object sender, ElapsedEventArgs e)
		{
		}

		private void SendJoin(int timeoutMs = 10000)
		{
		}

		private void HandleJoinResponse(IRealtimePush<RealtimeChannel, SocketResponse> sender, SocketResponse message)
		{
		}

		internal void HandleSocketMessage(SocketResponse message)
		{
		}

		private void _003CRegister_003Eb__64_0<TBroadcastResponse>(IRealtimeChannel _, SocketResponse response)
		{
		}

		private void _003CRegister_003Eb__65_0<TPresenceResponse>(IRealtimeChannel _, SocketResponse response)
		{
		}

		private void _003CRegister_003Eb__65_1<TPresenceResponse>(IRealtimeChannel _, SocketResponse response)
		{
		}
	}
}
