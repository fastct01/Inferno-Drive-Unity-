using System;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Interfaces
{
	public interface IRealtimePush<TChannel, TSocketResponse>
	{
		public delegate void MessageEventHandler(IRealtimePush<TChannel, TSocketResponse> sender, TSocketResponse message);

		TChannel Channel { get; }

		string EventName { get; }

		bool IsSent { get; }

		SocketRequest Message { get; }

		object Payload { get; }

		string Ref { get; }

		IRealtimeSocketResponse Response { get; }

		event EventHandler OnTimeout;

		void AddMessageReceivedHandler(MessageEventHandler handler);

		void RemoveMessageReceivedHandler(MessageEventHandler handler);

		void ClearMessageReceivedHandler();

		void Resend(int timeoutMs = 10000);

		void Send();
	}
}
