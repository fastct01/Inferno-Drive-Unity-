using System;
using System.Collections.Generic;
using System.Timers;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.Channel
{
	public class Push : IRealtimePush<RealtimeChannel, SocketResponse>
	{
		private bool IsSent__BackingField;

		private EventHandler m_OnTimeout;

		private IRealtimeSocketResponse Response__BackingField;

		private readonly RealtimeChannel Channel__BackingField;

		private readonly string Type__BackingField;

		private readonly string EventName__BackingField;

		private readonly object Payload__BackingField;

		private SocketRequest Message__BackingField;

		private string Ref__BackingField;

		private string _msgRefEvent;

		private int _timeoutMs;

		private readonly Timer _timer;

		private readonly IRealtimeSocket _socket;

		private readonly List<IRealtimePush<RealtimeChannel, SocketResponse>.MessageEventHandler> _messageEventHandlers;

		public bool IsSent
		{
			get
			{
				return IsSent__BackingField;
			}
			private set
			{
				IsSent__BackingField = value;
			}
		}

		public IRealtimeSocketResponse Response
		{
			get
			{
				return Response__BackingField;
			}
			private set
			{
				Response__BackingField = value;
			}
		}

		public RealtimeChannel Channel => Channel__BackingField;

		private string Type => Type__BackingField;

		public string EventName => EventName__BackingField;

		public object Payload => Payload__BackingField;

		public SocketRequest Message
		{
			get
			{
				return Message__BackingField;
			}
			private set
			{
				Message__BackingField = value;
			}
		}

		public string Ref
		{
			get
			{
				return Ref__BackingField;
			}
			private set
			{
				Ref__BackingField = value;
			}
		}

		public event EventHandler OnTimeout
		{
			add
			{
			}
			remove
			{
			}
		}

		public Push(IRealtimeSocket socket, RealtimeChannel channel, string eventName, string type = null, object payload = null, int timeoutMs = 10000)
		{
		}

		public void Resend(int timeoutMs = 10000)
		{
		}

		public void Send()
		{
		}

		internal void StartTimeout()
		{
		}

		private void HandleSocketMessageReceived(IRealtimeSocket sender, SocketResponse message)
		{
		}

		public void AddMessageReceivedHandler(IRealtimePush<RealtimeChannel, SocketResponse>.MessageEventHandler handler)
		{
		}

		public void RemoveMessageReceivedHandler(IRealtimePush<RealtimeChannel, SocketResponse>.MessageEventHandler handler)
		{
		}

		private void NotifyMessageReceived(SocketResponse messageResponse)
		{
		}

		public void ClearMessageReceivedHandler()
		{
		}

		private void HandleTimeoutElapsed(object sender, ElapsedEventArgs e)
		{
		}

		private void CancelTimeout()
		{
		}
	}
}
