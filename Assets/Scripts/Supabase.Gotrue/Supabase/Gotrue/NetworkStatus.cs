using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Gotrue.Interfaces;

namespace Supabase.Gotrue
{
	public class NetworkStatus
	{
		public delegate void NetworkListener(bool isNetworkAvailable);

		[StructLayout((LayoutKind)3)]
		private struct _003CPingCheck_003Ed__10 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public string url;

			public NetworkStatus _003C_003E4__this;

			private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

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
		private struct _003CStartAsync_003Ed__12 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public NetworkStatus _003C_003E4__this;

			public string url;

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

		private readonly List<NetworkListener> _listeners;

		public bool Ready;

		private IGotrueClient<User, Session> Client__BackingField;

		public IGotrueClient<User, Session> Client
		{
			get
			{
				return Client__BackingField;
			}
			set
			{
				Client__BackingField = value;
			}
		}

		public void AddListener(NetworkListener listener)
		{
		}

		public void RemoveListener(NetworkListener listener)
		{
		}

		private void NotifyListeners(bool isNetworkAvailable)
		{
		}

		public Task<bool> PingCheck(string url)
		{
			return null;
		}

		private void UpdateStatus(bool isNetworkAvailable)
		{
		}

		public Task<bool> StartAsync(string url)
		{
			return null;
		}

		private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
		}

		~NetworkStatus()
		{
		}
	}
}
