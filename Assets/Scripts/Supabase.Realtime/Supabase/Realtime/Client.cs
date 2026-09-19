using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Core.Interfaces;
using Supabase.Realtime.Exceptions;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime
{
	public class Client : IRealtimeClient<RealtimeSocket, RealtimeChannel>, IGettableHeaders
	{
		private sealed class _003C_003Ec__DisplayClass26_0
		{
			public IRealtimeSocket.StateEventHandler socketStateHandler;

			public Client _003C_003E4__this;

			public TaskCompletionSource<IRealtimeClient<RealtimeSocket, RealtimeChannel>> tsc;

			internal void _003CConnectAsync_003Eb__0(IRealtimeSocket sender, Constants.SocketState state)
			{
			}
		}

		private sealed class _003C_003Ec__DisplayClass27_0
		{
			public Client _003C_003E4__this;

			public IRealtimeSocket.StateEventHandler socketStateHandler;

			public IRealtimeSocket.ErrorEventHandler errorEventHandler;

			public Action<IRealtimeClient<RealtimeSocket, RealtimeChannel>, RealtimeException> callback;

			internal void _003CConnect_003Eb__0(IRealtimeSocket sender, Constants.SocketState state)
			{
			}

			internal void _003CConnect_003Eb__1(IRealtimeSocket sender, RealtimeException ex)
			{
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CConnectAsync_003Ed__26 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<IRealtimeClient<RealtimeSocket, RealtimeChannel>> _003C_003Et__builder;

			public Client _003C_003E4__this;

			private _003C_003Ec__DisplayClass26_0 _003C_003E8__1;

			private TaskAwaiter _003C_003Eu__1;

			private TaskAwaiter<IRealtimeClient<RealtimeSocket, RealtimeChannel>> _003C_003Eu__2;

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

		private IRealtimeSocket Socket__BackingField;

		private readonly ClientOptions Options__BackingField;

		private Func<Dictionary<string, string>> _getHeaders__BackingField;

		private string AccessToken__BackingField;

		private readonly string _realtimeUrl;

		private readonly List<IRealtimeClient<RealtimeSocket, RealtimeChannel>.SocketStateEventHandler> _socketEventHandlers;

		private readonly Dictionary<string, RealtimeChannel> _subscriptions;

		public ReadOnlyDictionary<string, RealtimeChannel> Subscriptions => null;

		public IRealtimeSocket Socket
		{
			get
			{
				return Socket__BackingField;
			}
			private set
			{
				Socket__BackingField = value;
			}
		}

		public ClientOptions Options => Options__BackingField;

		private Func<Dictionary<string, string>> _getHeaders
		{
			get
			{
				return _getHeaders__BackingField;
			}
			set
			{
				_getHeaders__BackingField = value;
			}
		}

		public Func<Dictionary<string, string>> GetHeaders
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public JsonSerializerSettings SerializerSettings => null;

		private string AccessToken
		{
			get
			{
				return AccessToken__BackingField;
			}
			set
			{
				AccessToken__BackingField = value;
			}
		}

		public Client(string realtimeUrl, ClientOptions options = null)
		{
		}

		public Task<IRealtimeClient<RealtimeSocket, RealtimeChannel>> ConnectAsync()
		{
			return null;
		}

		public IRealtimeClient<RealtimeSocket, RealtimeChannel> Connect(Action<IRealtimeClient<RealtimeSocket, RealtimeChannel>, RealtimeException> callback = null)
		{
			return null;
		}

		public void AddStateChangedHandler(IRealtimeClient<RealtimeSocket, RealtimeChannel>.SocketStateEventHandler handler)
		{
		}

		public void RemoveStateChangedHandler(IRealtimeClient<RealtimeSocket, RealtimeChannel>.SocketStateEventHandler handler)
		{
		}

		public void ClearStateChangedHandlers()
		{
		}

		private void NotifySocketStateChange(Constants.SocketState stateChanged)
		{
		}

		public void AddDebugHandler(IRealtimeDebugger.DebugEventHandler handler)
		{
		}

		public void RemoveDebugHandler(IRealtimeDebugger.DebugEventHandler handler)
		{
		}

		public void ClearDebugHandlers()
		{
		}

		private void HandleSocketHeartbeat(IRealtimeSocket sender, SocketResponse message)
		{
		}

		public IRealtimeClient<RealtimeSocket, RealtimeChannel> Disconnect(WebSocketCloseStatus code = WebSocketCloseStatus.NormalClosure, string reason = "Programmatic Disconnect")
		{
			return null;
		}

		public void SetAuth(string jwt)
		{
		}

		public RealtimeChannel Channel(string channelName)
		{
			return null;
		}

		public RealtimeChannel Channel(string database = "realtime", string schema = "public", string table = "*", string column = null, string value = null, Dictionary<string, string> parameters = null)
		{
			return null;
		}

		public void Remove(RealtimeChannel channel)
		{
		}

		private void DefaultMessageEncoder(object payload, Action<string> callback)
		{
		}

		private void DefaultMessageDecoder(string payload, Action<SocketResponse> callback)
		{
		}

		private void HandleSocketMessageReceived(IRealtimeSocket sender, SocketResponse message)
		{
		}

		private void HandleSocketStateChanged(IRealtimeSocket sender, Constants.SocketState state)
		{
		}

		private string _003CChannel_003Eb__38_0()
		{
			return null;
		}

		private string _003CChannel_003Eb__39_0()
		{
			return null;
		}
	}
}
