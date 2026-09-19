using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Supabase.Storage.Extensions
{
	internal static class HttpClientProgress
	{
		private sealed class _003C_003Ec__DisplayClass0_0
		{
			public IProgress<float> progress;

			public long? contentLength;

			internal void _003CDownloadDataAsync_003Eb__1(long totalBytes)
			{
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CCopyToAsync_003Ed__1 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public int bufferSize;

			public Stream source;

			public Stream destination;

			public CancellationToken cancellationToken;

			public IProgress<long> progress;

			private byte[] _003Cbuffer_003E5__2;

			private long _003CtotalBytesRead_003E5__3;

			private int _003CbytesRead_003E5__4;

			private ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _003C_003Eu__1;

			private ConfiguredTaskAwaitable<int>.ConfiguredTaskAwaiter _003C_003Eu__2;

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
		private struct _003CDownloadDataAsync_003Ed__0 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MemoryStream> _003C_003Et__builder;

			public IProgress<float> progress;

			public Uri uri;

			public Dictionary<string, string> headers;

			public HttpClient client;

			private _003C_003Ec__DisplayClass0_0 _003C_003E8__1;

			public CancellationToken cancellationToken;

			private MemoryStream _003Cdestination_003E5__2;

			private HttpResponseMessage _003Cresponse_003E5__3;

			private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

			private TaskAwaiter<string> _003C_003Eu__2;

			private Stream _003Cdownload_003E5__4;

			private TaskAwaiter<Stream> _003C_003Eu__3;

			private TaskAwaiter _003C_003Eu__4;

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
		private struct _003CUploadAsync_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<HttpResponseMessage> _003C_003Et__builder;

			public Stream stream;

			public Progress<float> progress;

			public Dictionary<string, string> headers;

			public HttpClient client;

			public Uri uri;

			private HttpResponseMessage _003Cresponse_003E5__2;

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

		public static Task<MemoryStream> DownloadDataAsync(HttpClient client, Uri uri, Dictionary<string, string> headers = null, IProgress<float> progress = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		private static Task CopyToAsync(Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		public static Task<HttpResponseMessage> UploadFileAsync(HttpClient client, Uri uri, string filePath, Dictionary<string, string> headers = null, Progress<float> progress = null)
		{
			return null;
		}

		public static Task<HttpResponseMessage> UploadBytesAsync(HttpClient client, Uri uri, byte[] data, Dictionary<string, string> headers = null, Progress<float> progress = null)
		{
			return null;
		}

		public static Task<HttpResponseMessage> UploadAsync(HttpClient client, Uri uri, Stream stream, Dictionary<string, string> headers = null, Progress<float> progress = null)
		{
			return null;
		}

		internal static float _003CDownloadDataAsync_003Eg__GetProgressPercentage_007C0_0(float totalBytes, float currentBytes)
		{
			return 0f;
		}
	}
}
