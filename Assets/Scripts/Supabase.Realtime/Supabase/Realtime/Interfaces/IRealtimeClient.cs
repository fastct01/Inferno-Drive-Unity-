using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Core.Interfaces;
using Supabase.Realtime.Exceptions;

namespace Supabase.Realtime.Interfaces
{
	public interface IRealtimeClient<TSocket, TChannel> : IGettableHeaders
	{
		public delegate void SocketStateEventHandler(IRealtimeClient<TSocket, TChannel> sender, Constants.SocketState state);

		ClientOptions Options { get; }

		JsonSerializerSettings SerializerSettings { get; }

		IRealtimeSocket Socket { get; }

		ReadOnlyDictionary<string, TChannel> Subscriptions { get; }

		void AddStateChangedHandler(SocketStateEventHandler handler);

		void RemoveStateChangedHandler(SocketStateEventHandler handler);

		void ClearStateChangedHandlers();

		void AddDebugHandler(IRealtimeDebugger.DebugEventHandler handler);

		void RemoveDebugHandler(IRealtimeDebugger.DebugEventHandler handler);

		void ClearDebugHandlers();

		TChannel Channel(string channelName);

		TChannel Channel(string database = "realtime", string schema = "public", string table = "*", string column = null, string value = null, Dictionary<string, string> parameters = null);

		IRealtimeClient<TSocket, TChannel> Connect(Action<IRealtimeClient<TSocket, TChannel>, RealtimeException> callback = null);

		Task<IRealtimeClient<TSocket, TChannel>> ConnectAsync();

		IRealtimeClient<TSocket, TChannel> Disconnect(WebSocketCloseStatus code = WebSocketCloseStatus.NormalClosure, string reason = "Programmatic Disconnect");

		void Remove(TChannel channel);

		void SetAuth(string jwt);
	}
}
