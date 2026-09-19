using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest
{
	internal static class Helpers
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CMakeRequest_003Ed__2<T> : IAsyncStateMachine where T : new()
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ModeledResponse<T>> _003C_003Et__builder;

			public ClientOptions clientOptions;

			public HttpMethod method;

			public string url;

			public JsonSerializerSettings serializerSettings;

			public object data;

			public Dictionary<string, string> headers;

			public CancellationToken cancellationToken;

			public Func<Dictionary<string, string>> getHeaders;

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
		private struct _003CMakeRequest_003Ed__3 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<BaseResponse> _003C_003Et__builder;

			public string url;

			public object data;

			public HttpMethod method;

			public JsonSerializerSettings serializerSettings;

			public Dictionary<string, string> headers;

			public CancellationToken cancellationToken;

			public ClientOptions clientOptions;

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

		private static readonly Guid AppSession;

		public static Task<ModeledResponse<T>> MakeRequest<T>(ClientOptions clientOptions, HttpMethod method, string url, JsonSerializerSettings serializerSettings, object data = null, Dictionary<string, string> headers = null, Func<Dictionary<string, string>> getHeaders = null, CancellationToken cancellationToken = default(CancellationToken)) where T : new()
		{
			return null;
		}

		public static Task<BaseResponse> MakeRequest(ClientOptions clientOptions, HttpMethod method, string url, JsonSerializerSettings serializerSettings, object data = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public static Dictionary<string, string> PrepareRequestHeaders(HttpMethod method, Dictionary<string, string> headers = null, ClientOptions options = null, int rangeFrom = -2147483648, int rangeTo = -2147483648)
		{
			return null;
		}
	}
}
