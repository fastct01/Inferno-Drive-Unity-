using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Supabase.Storage
{
	internal static class Helpers
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CMakeRequest_003Ed__4<T> : IAsyncStateMachine where T : class
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

			public HttpMethod method;

			public string url;

			public object data;

			public Dictionary<string, string> headers;

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
		private struct _003CMakeRequest_003Ed__5 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<HttpResponseMessage> _003C_003Et__builder;

			public string url;

			public object data;

			public HttpMethod method;

			public Dictionary<string, string> headers;

			public CancellationToken cancellationToken;

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

		internal static HttpClient HttpRequestClient;

		internal static HttpClient HttpUploadClient;

		internal static HttpClient HttpDownloadClient;

		internal static void Initialize(ClientOptions options)
		{
		}

		public static Task<T> MakeRequest<T>(HttpMethod method, string url, object data = null, Dictionary<string, string> headers = null) where T : class
		{
			return null;
		}

		public static Task<HttpResponseMessage> MakeRequest(HttpMethod method, string url, object data = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}
	}
}
