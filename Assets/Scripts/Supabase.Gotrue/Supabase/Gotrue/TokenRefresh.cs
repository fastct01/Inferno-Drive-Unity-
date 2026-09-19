using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Supabase.Gotrue.Interfaces;

namespace Supabase.Gotrue
{
	public class TokenRefresh
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CHandleRefreshTimerTick_003Ed__6 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public TokenRefresh _003C_003E4__this;

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

		private readonly Client _client;

		private Timer _refreshTimer;

		public bool Debug;

		public TokenRefresh(Client client)
		{
		}

		public void ManageAutoRefresh(IGotrueClient<User, Session> sender, Constants.AuthState stateChanged)
		{
		}

		private void InitRefreshTimer()
		{
		}

		private void HandleRefreshTimerTick(object _)
		{
		}

		private void CreateNewTimer()
		{
		}

		private TimeSpan GetInterval()
		{
			return default;
		}
	}
}
