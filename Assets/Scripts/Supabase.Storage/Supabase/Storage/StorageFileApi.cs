using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Storage.Interfaces;
using Supabase.Storage.Responses;

namespace Supabase.Storage
{
	public class StorageFileApi : IStorageFileApi<FileObject>
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CCreateSignedUrl_003Ed__19 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public int expiresIn;

			public StorageFileApi _003C_003E4__this;

			public string path;

			public TransformOptions transformOptions;

			private TaskAwaiter<CreateSignedUrlResponse> _003C_003Eu__1;

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
		private struct _003CCreateSignedUrls_003Ed__20 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<List<CreateSignedUrlsResponse>> _003C_003Et__builder;

			public int expiresIn;

			public List<string> paths;

			public StorageFileApi _003C_003E4__this;

			private TaskAwaiter<List<CreateSignedUrlsResponse>> _003C_003Eu__1;

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
		private struct _003CCreateUploadSignedUrl_003Ed__37 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<UploadSignedUrl> _003C_003Et__builder;

			public StorageFileApi _003C_003E4__this;

			public string supabasePath;

			private TaskAwaiter<CreatedUploadSignedUrlResponse> _003C_003Eu__1;

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
		private struct _003CDownloadBytes_003Ed__41 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<byte[]> _003C_003Et__builder;

			public string url;

			public TransformOptions transformOptions;

			public EventHandler<float> onProgress;

			public StorageFileApi _003C_003E4__this;

			private TaskAwaiter<MemoryStream> _003C_003Eu__1;

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
		private struct _003CDownloadFile_003Ed__40 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public string url;

			public TransformOptions transformOptions;

			public EventHandler<float> onProgress;

			public StorageFileApi _003C_003E4__this;

			public string localPath;

			private TaskAwaiter<MemoryStream> _003C_003Eu__1;

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
		private struct _003CList_003Ed__21 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<List<FileObject>> _003C_003Et__builder;

			public SearchOptions options;

			public string path;

			public StorageFileApi _003C_003E4__this;

			private TaskAwaiter<List<FileObject>> _003C_003Eu__1;

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
		private struct _003CMove_003Ed__28 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public StorageFileApi _003C_003E4__this;

			public string fromPath;

			public string toPath;

			private TaskAwaiter<GenericResponse> _003C_003Eu__1;

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
		private struct _003CRemove_003Ed__35 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<FileObject> _003C_003Et__builder;

			public StorageFileApi _003C_003E4__this;

			public string path;

			private TaskAwaiter<List<FileObject>> _003C_003Eu__1;

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
		private struct _003CRemove_003Ed__36 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<List<FileObject>> _003C_003Et__builder;

			public List<string> paths;

			public StorageFileApi _003C_003E4__this;

			private TaskAwaiter<List<FileObject>> _003C_003Eu__1;

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
		private struct _003CUpload_003Ed__22 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public FileOptions options;

			public bool inferContentType;

			public string localFilePath;

			public StorageFileApi _003C_003E4__this;

			public string supabasePath;

			public EventHandler<float> onProgress;

			private TaskAwaiter<string> _003C_003Eu__1;

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
		private struct _003CUpload_003Ed__23 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public FileOptions options;

			public bool inferContentType;

			public string supabasePath;

			public StorageFileApi _003C_003E4__this;

			public byte[] data;

			public EventHandler<float> onProgress;

			private TaskAwaiter<string> _003C_003Eu__1;

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
		private struct _003CUploadOrUpdate_003Ed__38 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public StorageFileApi _003C_003E4__this;

			public string supabasePath;

			public FileOptions options;

			public EventHandler<float> onProgress;

			public string localPath;

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
		private struct _003CUploadOrUpdate_003Ed__39 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public StorageFileApi _003C_003E4__this;

			public string supabasePath;

			public FileOptions options;

			public EventHandler<float> onProgress;

			public byte[] data;

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
		private struct _003CUploadToSignedUrl_003Ed__24 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public FileOptions options;

			public bool inferContentType;

			public string localFilePath;

			public StorageFileApi _003C_003E4__this;

			public UploadSignedUrl signedUrl;

			public EventHandler<float> onProgress;

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
		private struct _003CUploadToSignedUrl_003Ed__25 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public FileOptions options;

			public bool inferContentType;

			public UploadSignedUrl signedUrl;

			public StorageFileApi _003C_003E4__this;

			public EventHandler<float> onProgress;

			public byte[] data;

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

		private ClientOptions Options__BackingField;

		private string Url__BackingField;

		private Dictionary<string, string> Headers__BackingField;

		private string BucketId__BackingField;

		public ClientOptions Options
		{
			get
			{
				return Options__BackingField;
			}
			protected set
			{
				Options__BackingField = value;
			}
		}

		protected string Url
		{
			get
			{
				return Url__BackingField;
			}
			set
			{
				Url__BackingField = value;
			}
		}

		protected Dictionary<string, string> Headers
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

		protected string BucketId
		{
			get
			{
				return BucketId__BackingField;
			}
			set
			{
				BucketId__BackingField = value;
			}
		}

		public StorageFileApi(string url, string bucketId, ClientOptions options, Dictionary<string, string> headers = null)
		{
		}

		public StorageFileApi(string url, Dictionary<string, string> headers = null, string bucketId = null)
		{
		}

		public string GetPublicUrl(string path, TransformOptions transformOptions)
		{
			return null;
		}

		public Task<string> CreateSignedUrl(string path, int expiresIn, TransformOptions transformOptions = null)
		{
			return null;
		}

		public Task<List<CreateSignedUrlsResponse>> CreateSignedUrls(List<string> paths, int expiresIn)
		{
			return null;
		}

		public Task<List<FileObject>> List(string path = "", SearchOptions options = null)
		{
			return null;
		}

		public Task<string> Upload(string localFilePath, string supabasePath, FileOptions options = null, EventHandler<float> onProgress = null, bool inferContentType = true)
		{
			return null;
		}

		public Task<string> Upload(byte[] data, string supabasePath, FileOptions options = null, EventHandler<float> onProgress = null, bool inferContentType = true)
		{
			return null;
		}

		public Task<string> UploadToSignedUrl(string localFilePath, UploadSignedUrl signedUrl, FileOptions options = null, EventHandler<float> onProgress = null, bool inferContentType = true)
		{
			return null;
		}

		public Task<string> UploadToSignedUrl(byte[] data, UploadSignedUrl signedUrl, FileOptions options = null, EventHandler<float> onProgress = null, bool inferContentType = true)
		{
			return null;
		}

		public Task<string> Update(string localFilePath, string supabasePath, FileOptions options = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<string> Update(byte[] data, string supabasePath, FileOptions options = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<bool> Move(string fromPath, string toPath)
		{
			return null;
		}

		public Task<string> Download(string supabasePath, string localPath, TransformOptions transformOptions = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<string> Download(string supabasePath, string localPath, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<byte[]> Download(string supabasePath, TransformOptions transformOptions = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<byte[]> Download(string supabasePath, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<string> DownloadPublicFile(string supabasePath, string localPath, TransformOptions transformOptions = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<byte[]> DownloadPublicFile(string supabasePath, TransformOptions transformOptions = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		public Task<FileObject> Remove(string path)
		{
			return null;
		}

		public Task<List<FileObject>> Remove(List<string> paths)
		{
			return null;
		}

		public Task<UploadSignedUrl> CreateUploadSignedUrl(string supabasePath)
		{
			return null;
		}

		private Task<string> UploadOrUpdate(string localPath, string supabasePath, FileOptions options, EventHandler<float> onProgress = null)
		{
			return null;
		}

		private Task<string> UploadOrUpdate(byte[] data, string supabasePath, FileOptions options, EventHandler<float> onProgress = null)
		{
			return null;
		}

		private Task<string> DownloadFile(string url, string localPath, TransformOptions transformOptions = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		private Task<byte[]> DownloadBytes(string url, TransformOptions transformOptions = null, EventHandler<float> onProgress = null)
		{
			return null;
		}

		private string GetFinalPath(string path)
		{
			return null;
		}
	}
}
