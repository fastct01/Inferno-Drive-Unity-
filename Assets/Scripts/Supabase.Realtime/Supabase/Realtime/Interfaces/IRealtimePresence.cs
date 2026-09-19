using System.Threading.Tasks;
using Supabase.Realtime.Channel;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Interfaces
{
	public interface IRealtimePresence
	{
		public delegate void PresenceEventHandler(IRealtimePresence sender, EventType eventType);

		public enum EventType
		{
			Sync = 0,
			Join = 1,
			Leave = 2
		}

		Task<Push> Track(object payload, int timeoutMs = 10000);

		Task<Push> Untrack();

		void AddPresenceEventHandler(EventType eventType, PresenceEventHandler handler);

		void RemovePresenceEventHandlers(EventType eventType, PresenceEventHandler handler);

		void ClearPresenceEventHandlers(EventType? eventType = null);

		void TriggerSync(SocketResponse response);

		void TriggerDiff(SocketResponse args);
	}
}
