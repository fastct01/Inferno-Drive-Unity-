using Websocket.Client;

namespace Supabase.Realtime.Exceptions
{
	public static class FailureHint
	{
		public enum Reason
		{
			Unknown = 0,
			PushTimeout = 1,
			ChannelNotOpen = 2,
			ChannelJoinFailure = 3,
			SocketError = 4,
			ConnectionLost = 5,
			ConnectionStale = 6
		}

		public static Reason Parse(DisconnectionInfo info)
		{
			return default;
		}
	}
}
