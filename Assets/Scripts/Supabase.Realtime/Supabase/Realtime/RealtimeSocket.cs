using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Realtime.Exceptions;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.Socket;
using Websocket.Client;

namespace Supabase.Realtime
{
	public class RealtimeSocket : IDisposable, IRealtimeSocket, IGettableHeaders
	{
		private sealed class _003C_003Ec__DisplayClass44_0
		{
			public RealtimeSocket _003C_003E4__this;

			public SocketRequest data;

			public Action<string> _003C_003E9__1;

			internal void _003CPush_003Eb__0()
			{
			}

			internal void _003CPush_003Eb__1(string encoded)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass45_0
		{
			public string pingRef;

			public RealtimeSocket _003C_003E4__this;

			public IRealtimeSocket.MessageEventHandler messageHandler;

			public TaskCompletionSource<double> tsc;

			public DateTime start;

			internal void _003CGetLatency_003Eb__0(IRealtimeSocket _, SocketResponse messageResponse)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass51_0
		{
			public RealtimeSocket _003C_003E4__this;

			public ResponseMessage args;

			internal void _003CHandleSocketMessage_003Eb__0(SocketResponse decoded)
			{
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CConnect_003Ed__26 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public RealtimeSocket _003C_003E4__this;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CDisposeConnection_003Ed__56 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public RealtimeSocket _003C_003E4__this;

			private TaskAwaiter<bool> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CEmitHeartbeat_003Ed__48 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public RealtimeSocket _003C_003E4__this;

			private TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private Func<Dictionary<string, string>> GetHeaders__BackingField;

		private readonly List<IRealtimeSocket.StateEventHandler> _socketEventHandlers;

		private readonly List<IRealtimeSocket.MessageEventHandler> _messageEventHandlers;

		private readonly List<IRealtimeSocket.HeartbeatEventHandler> _heartbeatEventHandlers;

		private readonly List<IRealtimeSocket.ErrorEventHandler> _errorEventHandlers;

		private readonly string _endpoint;

		private readonly ClientOptions _options;

		private readonly WebsocketClient _connection;

		private CancellationTokenSource _heartbeatTokenSource;

		private bool _hasSuccessfullyConnectedOnce;

		private bool _hasPendingHeartbeat;

		private string _pendingHeartbeatRef;

		private readonly List<Task> _buffer;

		private bool _isReconnecting;

		private int _reconnectionAttempts;

		public bool IsConnected => false;

		private string EndpointUrl => null;

		public Func<Dictionary<string, string>> GetHeaders
		{
			get
			{
				return GetHeaders__BackingField;
			}
			set
			{
				GetHeaders__BackingField = value;
			}
		}

		Dictionary<string, string> Headers => null;

		public RealtimeSocket(string endpoint, ClientOptions options)
		{
		}

		void IDisposable.Dispose()
		{
		}

		public Task Connect()
		{
			return null;
		}

		public void Disconnect(WebSocketCloseStatus code = WebSocketCloseStatus.NormalClosure, string reason = "")
		{
		}

		public void AddStateChangedHandler(IRealtimeSocket.StateEventHandler handler)
		{
		}

		public void RemoveStateChangedHandler(IRealtimeSocket.StateEventHandler handler)
		{
		}

		private void NotifySocketStateChange(Constants.SocketState newState)
		{
		}

		public void ClearStateChangedHandlers()
		{
		}

		public void AddMessageReceivedHandler(IRealtimeSocket.MessageEventHandler handler)
		{
		}

		public void RemoveMessageReceivedHandler(IRealtimeSocket.MessageEventHandler handler)
		{
		}

		private void NotifyMessageReceived(SocketResponse heartbeat)
		{
		}

		public void ClearMessageReceivedHandlers()
		{
		}

		public void AddHeartbeatHandler(IRealtimeSocket.HeartbeatEventHandler handler)
		{
		}

		public void RemoveHeartbeatHandler(IRealtimeSocket.HeartbeatEventHandler handler)
		{
		}

		private void NotifyHeartbeatReceived(SocketResponse heartbeat)
		{
		}

		public void ClearHeartbeatHandlers()
		{
		}

		public void AddErrorHandler(IRealtimeSocket.ErrorEventHandler handler)
		{
		}

		public void RemoveErrorHandler(IRealtimeSocket.ErrorEventHandler handler)
		{
		}

		public void ClearErrorHandlers()
		{
		}

		private void NotifyErrorOccurred(RealtimeException exception)
		{
		}

		public void Push(SocketRequest data)
		{
		}

		public Task<double> GetLatency()
		{
			return null;
		}

		private void SendHeartbeat()
		{
		}

		private void HandleSocketOpened()
		{
		}

		private Task EmitHeartbeat()
		{
			return null;
		}

		private void HandleSocketReconnectionHappened(ReconnectionInfo reconnectionInfo)
		{
		}

		private void HandleSocketDisconnectionHappened(DisconnectionInfo disconnectionInfo)
		{
		}

		private void HandleSocketMessage(ResponseMessage args)
		{
		}

		private void HandleSocketError(DisconnectionInfo disconnectionInfo = null)
		{
		}

		private void HandleSocketClosed(DisconnectionInfo disconnectionInfo = null)
		{
		}

		public string MakeMsgRef()
		{
			return null;
		}

		public string ReplyEventName(string msgRef)
		{
			return null;
		}

		private void DisposeConnection()
		{
		}

		private void FlushBuffer()
		{
		}

		private ClientWebSocket _003C_002Ector_003Eb__24_0()
		{
			return null;
		}
	}
}
