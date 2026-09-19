namespace Supabase.Realtime
{
	public static class Constants
	{
		public enum SocketState
		{
			Open = 0,
			Close = 1,
			Reconnect = 2,
			Error = 3
		}

		public enum ChannelState
		{
			Closed = 0,
			Errored = 1,
			Joined = 2,
			Joining = 3,
			Leaving = 4
		}

		public enum EventType
		{
			Insert = 0,
			Update = 1,
			Delete = 2,
			Broadcast = 3,
			PresenceState = 4,
			PresenceDiff = 5,
			PostgresChanges = 6,
			System = 7,
			Internal = 8,
			Unknown = 9
		}

		public enum PresenceListenEventTypes
		{
			Sync = 0,
			Join = 1,
			Leave = 2
		}

		public enum ChannelEventName
		{
			Broadcast = 0,
			Presence = 1,
			PostgresChanges = 2
		}

		public const int DefaultTimeout = 10000;

		public static string ChannelEventClose;

		public static string ChannelEventError;

		public const string ChannelEventJoin = "phx_join";

		public const string ChannelEventReply = "phx_reply";

		public const string ChannelEventSystem = "system";

		public const string ChannelEventLeave = "phx_leave";

		public const string PhoenixStatusOk = "ok";

		public const string ChannelEventPostgresChanges = "postgres_changes";

		public const string ChannelEventBroadcast = "broadcast";

		public const string ChannelEventPresenceState = "presence_state";

		public const string ChannelEventPresenceDiff = "presence_diff";

		public const string PhoenixStatusError = "error";

		public const string TransportWebsocket = "websocket";

		public const string ChannelAccessToken = "access_token";
	}
}
