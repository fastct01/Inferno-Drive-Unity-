using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Core.Interfaces;
using Supabase.Interfaces;
using Supabase.Postgrest;
using Supabase.Postgrest.Interfaces;
using Supabase.Realtime;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;

namespace Supabase
{
	public class SupabaseTable<TModel> : Table<TModel>, ISupabaseTable<TModel, RealtimeChannel>, IPostgrestTable<TModel>, IGettableHeaders where TModel : new()
	{
		[StructLayout((LayoutKind)3)]
		private struct _003COn_003Ed__5 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<RealtimeChannel> _003C_003Et__builder;

			public SupabaseTable<TModel> _003C_003E4__this;

			public PostgresChangesOptions.ListenType listenType;

			public IRealtimeChannel.PostgresChangesHandler handler;

			private TaskAwaiter<IRealtimeClient<RealtimeSocket, RealtimeChannel>> _003C_003Eu__1;

			private TaskAwaiter<IRealtimeChannel> _003C_003Eu__2;

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

		private RealtimeChannel _channel;

		private readonly IPostgrestClient _postgrestClient;

		private readonly IRealtimeClient<RealtimeSocket, RealtimeChannel> _realtimeClient;

		private readonly string _schema;

		public SupabaseTable(IPostgrestClient postgrestClient, IRealtimeClient<RealtimeSocket, RealtimeChannel> realtimeClient, string schema = "public")
			: base((string)null, (JsonSerializerSettings)null, (Supabase.Postgrest.ClientOptions)null)
		{
		}

		public Task<RealtimeChannel> On(PostgresChangesOptions.ListenType listenType, IRealtimeChannel.PostgresChangesHandler handler)
		{
			return null;
		}
	}
}
