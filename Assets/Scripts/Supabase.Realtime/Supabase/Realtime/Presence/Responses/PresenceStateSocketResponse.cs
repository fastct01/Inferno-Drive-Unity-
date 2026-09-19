using System.Collections.Generic;
using Newtonsoft.Json;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Presence.Responses
{
	public class PresenceStateSocketResponse<TPresence> : SocketResponse<Dictionary<string, PresenceStatePayload<TPresence>>>
	{
		public PresenceStateSocketResponse(JsonSerializerSettings serializerSettings)
			: base((JsonSerializerSettings)null)
		{
		}
	}
}
