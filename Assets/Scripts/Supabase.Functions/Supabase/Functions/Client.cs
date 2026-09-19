using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Functions.Interfaces;

namespace Supabase.Functions
{
	public class Client : IFunctionsClient, IGettableHeaders
	{
		public class InvokeFunctionOptions
		{
			private Dictionary<string, string> Headers__BackingField;

			private Dictionary<string, object> Body__BackingField;

			private TimeSpan HttpTimeout__BackingField;

			public Dictionary<string, string> Headers
			{
				get
				{
					return Headers__BackingField;
				}
				set
				{
					Headers__BackingField = value;
				}
			}

			public Dictionary<string, object> Body
			{
				get
				{
					return Body__BackingField;
				}
				set
				{
					Body__BackingField = value;
				}
			}

			public TimeSpan HttpTimeout
			{
				get
				{
					return HttpTimeout__BackingField;
				}
				set
				{
					HttpTimeout__BackingField = value;
				}
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CHandleRequest_003Ed__10 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<HttpResponseMessage> _003C_003Et__builder;

			public InvokeFunctionOptions options;

			public Client _003C_003E4__this;

			public string token;

			public string url;

			private HttpRequestMessage _003CrequestMessage_003E5__2;

			private HttpResponseMessage _003Cresponse_003E5__3;

			private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

			private TaskAwaiter<string> _003C_003Eu__2;

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
		private struct _003CInvoke_003Ed__8 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string functionName;

			public string token;

			public InvokeFunctionOptions options;

			private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

			private TaskAwaiter<string> _003C_003Eu__2;

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
		private struct _003CInvoke_003Ed__9<T> : IAsyncStateMachine where T : class
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string functionName;

			public string token;

			public InvokeFunctionOptions options;

			private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

			private TaskAwaiter<string> _003C_003Eu__2;

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
		private struct _003CRawInvoke_003Ed__7 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<HttpContent> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string functionName;

			public string token;

			public InvokeFunctionOptions options;

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

		private HttpClient _httpClient;

		private readonly string _baseUrl;

		private Func<Dictionary<string, string>> GetHeaders__BackingField;

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

		public Client(string baseUrl)
		{
		}

		public Task<HttpContent> RawInvoke(string functionName, string token = null, InvokeFunctionOptions options = null)
		{
			return null;
		}

		public Task<string> Invoke(string functionName, string token = null, InvokeFunctionOptions options = null)
		{
			return null;
		}

		public Task<T> Invoke<T>(string functionName, string token = null, InvokeFunctionOptions options = null) where T : class
		{
			return null;
		}

		private Task<HttpResponseMessage> HandleRequest(string url, string token = null, InvokeFunctionOptions options = null)
		{
			return null;
		}
	}
}
