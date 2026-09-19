using System.Threading.Tasks;
using Supabase.Realtime.Models;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Interfaces
{
	public interface IRealtimeBroadcast
	{
		public delegate void BroadcastEventHandler(IRealtimeBroadcast sender, BaseBroadcast broadcast);

		void AddBroadcastEventHandler(BroadcastEventHandler broadcastEventHandler);

		void RemoveBroadcastEventHandler(BroadcastEventHandler broadcastEventHandler);

		void ClearBroadcastEventHandlers();

		Task<bool> Send(string broadcastEventName, object payload, int timeoutMs = 10000);

		void TriggerReceived(SocketResponse response);
	}
}
