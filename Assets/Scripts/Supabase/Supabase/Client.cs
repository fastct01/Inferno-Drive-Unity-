using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Functions.Interfaces;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Supabase.Interfaces;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Responses;
using Supabase.Realtime;
using Supabase.Realtime.Interfaces;
using Supabase.Storage;
using Supabase.Storage.Interfaces;

namespace Supabase
{
	public class Client : ISupabaseClient<User, Session, RealtimeSocket, RealtimeChannel, Bucket, FileObject>
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CInitializeAsync_003Ed__26 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ISupabaseClient<User, Session, RealtimeSocket, RealtimeChannel, Bucket, FileObject>> _003C_003Et__builder;

			public Client _003C_003E4__this;

			private TaskAwaiter<Session> _003C_003Eu__1;

			private TaskAwaiter<IRealtimeClient<RealtimeSocket, RealtimeChannel>> _003C_003Eu__2;

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

		private IGotrueClient<User, Session> _auth;

		private IRealtimeClient<RealtimeSocket, RealtimeChannel> _realtime;

		private IFunctionsClient _functions;

		private IPostgrestClient _postgrest;

		private IStorageClient<Bucket, FileObject> _storage;

		private readonly string _supabaseUrl;

		private readonly string _supabaseKey;

		private readonly SupabaseOptions _options;

		public IGotrueClient<User, Session> Auth
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public IRealtimeClient<RealtimeSocket, RealtimeChannel> Realtime
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public IFunctionsClient Functions
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public IPostgrestClient Postgrest
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public IStorageClient<Bucket, FileObject> Storage
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public IGotrueAdminClient<User> AdminAuth(string serviceKey)
		{
			return null;
		}

		public Client(IGotrueClient<User, Session> auth, IRealtimeClient<RealtimeSocket, RealtimeChannel> realtime, IFunctionsClient functions, IPostgrestClient postgrest, IStorageClient<Bucket, FileObject> storage, SupabaseOptions options)
		{
		}

		public Client(string supabaseUrl, string supabaseKey, SupabaseOptions options = null)
		{
		}

		public Task<ISupabaseClient<User, Session, RealtimeSocket, RealtimeChannel, Bucket, FileObject>> InitializeAsync()
		{
			return null;
		}

		private void Auth_StateChanged(object sender, Supabase.Gotrue.Constants.AuthState e)
		{
		}

		public ISupabaseTable<TModel, RealtimeChannel> From<TModel>() where TModel : new()
		{
			return null;
		}

		public Task<BaseResponse> Rpc(string procedureName, object parameters)
		{
			return null;
		}

		public Task<TModeledResponse> Rpc<TModeledResponse>(string procedureName, object parameters)
		{
			return null;
		}

		internal Dictionary<string, string> GetAuthHeaders()
		{
			return null;
		}
	}
}
