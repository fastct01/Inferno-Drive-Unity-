using Newtonsoft.Json;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Presence
{
	public class RealtimePresenceDiff<TPresence> : SocketResponse<PresenceDiffPayload<TPresence>>
	{
		public RealtimePresenceDiff(JsonSerializerSettings serializerSettings)
			: base((JsonSerializerSettings)null)
		{
		}
	}
}
