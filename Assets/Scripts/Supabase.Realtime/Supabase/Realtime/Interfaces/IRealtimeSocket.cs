using System.Net.WebSockets;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Realtime.Exceptions;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Interfaces
{
	public interface IRealtimeSocket : IGettableHeaders
	{
		public delegate void ErrorEventHandler(IRealtimeSocket sender, RealtimeException exception);

		public delegate void StateEventHandler(IRealtimeSocket sender, Constants.SocketState state);

		public delegate void MessageEventHandler(IRealtimeSocket sender, SocketResponse message);

		public delegate void HeartbeatEventHandler(IRealtimeSocket sender, SocketResponse heartbeat);

		bool IsConnected { get; }

		void AddStateChangedHandler(StateEventHandler handler);

		void RemoveStateChangedHandler(StateEventHandler handler);

		void ClearStateChangedHandlers();

		void AddMessageReceivedHandler(MessageEventHandler handler);

		void RemoveMessageReceivedHandler(MessageEventHandler handler);

		void ClearMessageReceivedHandlers();

		void AddHeartbeatHandler(HeartbeatEventHandler handler);

		void RemoveHeartbeatHandler(HeartbeatEventHandler handler);

		void ClearHeartbeatHandlers();

		void AddErrorHandler(ErrorEventHandler handler);

		void RemoveErrorHandler(ErrorEventHandler handler);

		void ClearErrorHandlers();

		Task<double> GetLatency();

		Task Connect();

		void Disconnect(WebSocketCloseStatus code = WebSocketCloseStatus.NormalClosure, string reason = "");

		string MakeMsgRef();

		void Push(SocketRequest data);

		string ReplyEventName(string msgRef);
	}
}
