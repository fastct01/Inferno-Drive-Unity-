using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest.Requests
{
	public class CacheBackedRequest<TModel> : INotifyPropertyChanged where TModel : new()
	{
		public delegate void RemoteModelsPopulatedEventHandler(CacheBackedRequest<TModel> sender);

		[StructLayout((LayoutKind)3)]
		private struct _003CCache_003Ed__36 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public ModeledResponse<TModel> response;

			public CacheBackedRequest<TModel> _003C_003E4__this;

			private DateTime _003CcacheTime_003E5__2;

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

		[StructLayout((LayoutKind)3)]
		private struct _003CInvoke_003Ed__35 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public CacheBackedRequest<TModel> _003C_003E4__this;

			private TaskAwaiter<ModeledResponse<TModel>> _003C_003Eu__1;

			private TaskAwaiter _003C_003Eu__2;

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
		private struct _003CTryLoadFromCache_003Ed__34 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public CacheBackedRequest<TModel> _003C_003E4__this;

			private TaskAwaiter<CachedModel<TModel>> _003C_003Eu__1;

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

		private readonly Func<Task<ModeledResponse<TModel>>> _remoteRequestAction;

		private readonly IPostgrestTableWithCache<TModel> _instance;

		private readonly string CacheKey__BackingField;

		private readonly IPostgrestCacheProvider _cacheProvider;

		private List<TModel> _models;

		private ModeledResponse<TModel> _response;

		private bool _wasCacheHit;

		private DateTime? _cacheTime;

		private bool _wasResponseCached;

		private PropertyChangedEventHandler m_PropertyChanged;

		private RemoteModelsPopulatedEventHandler m_RemoteModelsPopulated;

		private string CacheKey => CacheKey__BackingField;

		public List<TModel> Models
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public ModeledResponse<TModel> Response
		{
			get
			{
				return null;
			}
			protected set
			{
			}
		}

		public bool WasCacheHit
		{
			get
			{
				return false;
			}
			protected set
			{
			}
		}

		public bool WasResponseCached
		{
			get
			{
				return false;
			}
			protected set
			{
			}
		}

		public DateTime? CacheTime
		{
			get
			{
				return null;
			}
			protected set
			{
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public event RemoteModelsPopulatedEventHandler RemoteModelsPopulated
		{
			add
			{
			}
			remove
			{
			}
		}

		public CacheBackedRequest(IPostgrestTableWithCache<TModel> instance, IPostgrestCacheProvider cacheProvider, Func<Task<ModeledResponse<TModel>>> remoteRequestAction)
		{
		}

		internal Task TryLoadFromCache()
		{
			return null;
		}

		internal void Invoke()
		{
		}

		private Task Cache(ModeledResponse<TModel> response)
		{
			return null;
		}

		private void OnPropertyChanged(string propertyName = null)
		{
		}

		private bool SetField<T>(ref T field, T value, string propertyName = null)
		{
			return false;
		}
	}
}
