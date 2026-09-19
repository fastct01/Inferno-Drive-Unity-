using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Storage.Interfaces;

namespace Supabase.Storage
{
	public class StorageBucketApi : IStorageBucketApi<Bucket>, IGettableHeaders
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CCreateBucket_003Ed__21 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

			public BucketUpsertOptions options;

			public string id;

			public StorageBucketApi _003C_003E4__this;

			private TaskAwaiter<Bucket> _003C_003Eu__1;

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
		private struct _003CGetBucket_003Ed__20 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Bucket> _003C_003Et__builder;

			public StorageBucketApi _003C_003E4__this;

			public string id;

			private TaskAwaiter<Bucket> _003C_003Eu__1;

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
		private struct _003CUpdateBucket_003Ed__22 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Bucket> _003C_003Et__builder;

			public BucketUpsertOptions options;

			public string id;

			public StorageBucketApi _003C_003E4__this;

			private TaskAwaiter<Bucket> _003C_003Eu__1;

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

		private readonly Dictionary<string, string> _initializedHeaders;

		private Dictionary<string, string> _headers;

		private Func<Dictionary<string, string>> GetHeaders__BackingField;

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

		public Dictionary<string, string> Headers
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

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

		protected StorageBucketApi(string url, ClientOptions options, Dictionary<string, string> headers = null)
		{
		}

		protected StorageBucketApi(string url, Dictionary<string, string> headers = null)
		{
		}

		public Task<List<Bucket>> ListBuckets()
		{
			return null;
		}

		public Task<Bucket> GetBucket(string id)
		{
			return null;
		}

		public Task<string> CreateBucket(string id, BucketUpsertOptions options = null)
		{
			return null;
		}

		public Task<Bucket> UpdateBucket(string id, BucketUpsertOptions options = null)
		{
			return null;
		}

		public Task<GenericResponse> EmptyBucket(string id)
		{
			return null;
		}

		public Task<GenericResponse> DeleteBucket(string id)
		{
			return null;
		}
	}
}
