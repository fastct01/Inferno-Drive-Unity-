using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supabase.Core.Interfaces;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest
{
	public class Client : IPostgrestClient, IGettableHeaders
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CRpc_003Ed__20<TModeledResponse> : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<TModeledResponse> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string procedureName;

			public object parameters;

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

		private readonly string BaseUrl__BackingField;

		private readonly ClientOptions Options__BackingField;

		private Func<Dictionary<string, string>> GetHeaders__BackingField;

		public string BaseUrl => BaseUrl__BackingField;

		public ClientOptions Options => Options__BackingField;

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

		public static JsonSerializerSettings SerializerSettings(ClientOptions options = null)
		{
			return null;
		}

		public void AddRequestPreparedHandler(OnRequestPreparedEventHandler handler)
		{
		}

		public void RemoveRequestPreparedHandler(OnRequestPreparedEventHandler handler)
		{
		}

		public void ClearRequestPreparedHandlers()
		{
		}

		public void AddDebugHandler(IPostgrestDebugger.DebugEventHandler handler)
		{
		}

		public void RemoveDebugHandler(IPostgrestDebugger.DebugEventHandler handler)
		{
		}

		public void ClearDebugHandlers()
		{
		}

		public Client(string baseUrl, ClientOptions options = null)
		{
		}

		public IPostgrestTable<T> Table<T>() where T : new()
		{
			return null;
		}

		public IPostgrestTableWithCache<T> Table<T>(IPostgrestCacheProvider cacheProvider) where T : new()
		{
			return null;
		}

		public Task<TModeledResponse> Rpc<TModeledResponse>(string procedureName, object parameters = null)
		{
			return null;
		}

		public Task<BaseResponse> Rpc(string procedureName, object parameters = null)
		{
			return null;
		}
	}
}
