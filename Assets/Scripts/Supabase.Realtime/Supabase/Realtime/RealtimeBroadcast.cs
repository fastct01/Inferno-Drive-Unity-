using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Realtime.Broadcast;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime
{
	public class RealtimeBroadcast<TBroadcastModel> : IRealtimeBroadcast
	{
		private readonly RealtimeChannel _channel;

		private readonly JsonSerializerSettings _serializerSettings;

		private SocketResponse _lastSocketResponse;

		private readonly List<IRealtimeBroadcast.BroadcastEventHandler> _broadcastEventHandlers;

		public TBroadcastModel Current()
		{
			return default;
		}

		public RealtimeBroadcast(RealtimeChannel channel, BroadcastOptions options, JsonSerializerSettings serializerSettings)
		{
		}

		public void AddBroadcastEventHandler(IRealtimeBroadcast.BroadcastEventHandler broadcastEventHandler)
		{
		}

		public void RemoveBroadcastEventHandler(IRealtimeBroadcast.BroadcastEventHandler broadcastEventHandler)
		{
		}

		public void ClearBroadcastEventHandlers()
		{
		}

		private void NotifyBroadcastEventHandlers()
		{
		}

		public void TriggerReceived(SocketResponse response)
		{
		}

		public Task<bool> Send(string broadcastEventName, object payload, int timeoutMs = 10000)
		{
			return null;
		}
	}
}
