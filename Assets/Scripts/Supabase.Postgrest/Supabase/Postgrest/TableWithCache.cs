using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Core.Interfaces;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Requests;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest
{
	public class TableWithCache<T> : Table<T>, IPostgrestTableWithCache<T>, IPostgrestTable<T>, IGettableHeaders where T : new()
	{
		private sealed class _003C_003Ec__DisplayClass4_0
		{
			public TableWithCache<T> _003C_003E4__this;

			public CancellationToken cancellationToken;

			internal Task<ModeledResponse<T>> _003CGet_003Eb__0()
			{
				return null;
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CGet_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<CacheBackedRequest<T>> _003C_003Et__builder;

			public TableWithCache<T> _003C_003E4__this;

			public CancellationToken cancellationToken;

			private CacheBackedRequest<T> _003CcacheModel_003E5__2;

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

		private readonly IPostgrestCacheProvider CacheProvider__BackingField;

		protected IPostgrestCacheProvider CacheProvider => CacheProvider__BackingField;

		public TableWithCache(string baseUrl, IPostgrestCacheProvider cacheProvider, JsonSerializerSettings serializerSettings, ClientOptions options = null)
			: base((string)null, (JsonSerializerSettings)null, (ClientOptions)null)
		{
		}

		public new Task<CacheBackedRequest<T>> Get(CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}

		private Task<ModeledResponse<T>> _003C_003En__0(CancellationToken cancellationToken = default(CancellationToken))
		{
			return null;
		}
	}
}
