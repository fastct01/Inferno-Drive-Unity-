using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Gotrue.Responses;

namespace Supabase.Gotrue
{
	public static class Helpers
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CMakeRequest_003Ed__6<T> : IAsyncStateMachine where T : class
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

			public HttpMethod method;

			public string url;

			public object data;

			public Dictionary<string, string> headers;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

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
		private struct _003CMakeRequest_003Ed__7 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<BaseResponse> _003C_003Et__builder;

			public string url;

			public object data;

			public HttpMethod method;

			public Dictionary<string, string> headers;

			private HttpRequestMessage _003CrequestMessage_003E5__2;

			private HttpResponseMessage _003Cresponse_003E5__3;

			private ConfiguredTaskAwaitable<HttpResponseMessage>.ConfiguredTaskAwaiter _003C_003Eu__1;

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

		private static readonly HttpClient Client;

		public static string GenerateNonce()
		{
			return null;
		}

		public static string GeneratePKCENonceVerifier(string codeVerifier)
		{
			return null;
		}

		public static string GenerateSHA256NonceFromRawNonce(string rawNonce)
		{
			return null;
		}

		internal static ProviderAuthState GetUrlForProvider(string url, Constants.Provider provider, SignInOptions options = null)
		{
			return null;
		}

		internal static Uri AddQueryParams(string url, Dictionary<string, string> data)
		{
			return null;
		}

		internal static Task<T> MakeRequest<T>(HttpMethod method, string url, object data = null, Dictionary<string, string> headers = null) where T : class
		{
			return null;
		}

		internal static Task<BaseResponse> MakeRequest(HttpMethod method, string url, object data = null, Dictionary<string, string> headers = null)
		{
			return null;
		}
	}
}
