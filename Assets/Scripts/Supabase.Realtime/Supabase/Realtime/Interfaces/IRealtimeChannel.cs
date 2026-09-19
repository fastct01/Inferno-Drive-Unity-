using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase.Realtime.Broadcast;
using Supabase.Realtime.Channel;
using Supabase.Realtime.Exceptions;
using Supabase.Realtime.PostgresChanges;
using Supabase.Realtime.Presence;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Interfaces
{
	public interface IRealtimeChannel
	{
		public delegate void MessageReceivedHandler(IRealtimeChannel sender, SocketResponse message);

		public delegate void StateChangedHandler(IRealtimeChannel sender, Constants.ChannelState state);

		public delegate void PostgresChangesHandler(IRealtimeChannel sender, PostgresChangesResponse change);

		public delegate void ErrorEventHandler(IRealtimeChannel sender, RealtimeException exception);

		bool HasJoinedOnce { get; }

		bool IsClosed { get; }

		bool IsErrored { get; }

		bool IsJoined { get; }

		bool IsJoining { get; }

		bool IsLeaving { get; }

		ChannelOptions Options { get; }

		BroadcastOptions BroadcastOptions { get; }

		PresenceOptions PresenceOptions { get; }

		List<PostgresChangesOptions> PostgresChangesOptions { get; }

		Constants.ChannelState State { get; }

		string Topic { get; }

		void AddStateChangedHandler(StateChangedHandler stateChangedHandler);

		void RemoveStateChangedHandler(StateChangedHandler stateChangedHandler);

		void ClearStateChangedHandlers();

		void AddMessageReceivedHandler(MessageReceivedHandler messageReceivedHandler);

		void RemoveMessageReceivedHandler(MessageReceivedHandler messageReceivedHandler);

		void ClearMessageReceivedHandlers();

		void AddPostgresChangeHandler(PostgresChangesOptions.ListenType listenType, PostgresChangesHandler postgresChangeHandler);

		void RemovePostgresChangeHandler(PostgresChangesOptions.ListenType listenType, PostgresChangesHandler postgresChangeHandler);

		void ClearPostgresChangeHandlers();

		void AddErrorHandler(ErrorEventHandler handler);

		void RemoveErrorHandler(ErrorEventHandler handler);

		void ClearErrorHandlers();

		IRealtimeBroadcast Broadcast();

		IRealtimePresence Presence();

		Push Push(string eventName, string type = null, object payload = null, int timeoutMs = 10000);

		void Rejoin(int timeoutMs = 10000);

		Task<bool> Send(Constants.ChannelEventName eventType, string type, object payload, int timeoutMs = 10000);

		RealtimeBroadcast<TBroadcastResponse> Register<TBroadcastResponse>(bool broadcastSelf = false, bool broadcastAck = false);

		RealtimePresence<TPresenceResponse> Register<TPresenceResponse>(string presenceKey);

		IRealtimeChannel Register(PostgresChangesOptions postgresChangesOptions);

		Task<IRealtimeChannel> Subscribe(int timeoutMs = 10000);

		IRealtimeChannel Unsubscribe();
	}
}
