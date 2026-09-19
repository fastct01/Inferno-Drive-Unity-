using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase;
using Supabase.Gotrue;
using Supabase.Interfaces;
using Supabase.Realtime;
using Supabase.Storage;
using UnityEngine;

namespace ShadowRun.Services
{
	public sealed class SupabaseBootstrap : MonoBehaviour
	{
		[Serializable]
		private sealed class ProjectConfig
		{
			public string project_url;

			public string publishable_key;
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CInitializeCoreAsync_003Ed__30 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			private IDisposable _003C_003E7__wrap1;

			private TaskAwaiter<ISupabaseClient<User, Session, RealtimeSocket, RealtimeChannel, Bucket, FileObject>> _003C_003Eu__1;

			private TaskAwaiter<bool> _003C_003Eu__2;

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
		private struct _003CRestorePersistedSessionCoreAsync_003Ed__35 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			private string _003CexpectedUserId_003E5__2;

			private IDisposable _003C_003E7__wrap2;

			private TaskAwaiter<Session> _003C_003Eu__1;

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

		private const string ConfigResourceName = "supabase";

		private const string PlaceholderProjectUrl = "YOUR_PROJECT_REF";

		private const string PlaceholderPublishableKey = "YOUR_SUPABASE_PUBLISHABLE_KEY";

		private static Task initializationTask;

		private static Task<bool> sessionRestoreTask;

		private static SupabaseSessionStore sessionStore;

		private static Supabase.Client Client__BackingField;

		private static bool IsReady__BackingField;

		private static string InitializationError__BackingField;

		private static string SessionRestoreError__BackingField;

		public static Supabase.Client Client
		{
			get
			{
				return Client__BackingField;
			}
			private set
			{
				Client__BackingField = value;
			}
		}

		public static bool IsReady
		{
			get
			{
				return IsReady__BackingField;
			}
			private set
			{
				IsReady__BackingField = value;
			}
		}

		public static string InitializationError
		{
			get
			{
				return InitializationError__BackingField;
			}
			private set
			{
				InitializationError__BackingField = value;
			}
		}

		public static string SessionRestoreError
		{
			get
			{
				return SessionRestoreError__BackingField;
			}
			private set
			{
				SessionRestoreError__BackingField = value;
			}
		}

		public static bool HasPersistedSession => false;

		public static string PersistedUserId => null;

		private static void CreateInstance()
		{
		}

		private void Awake()
		{
		}

		public static Task InitializeAsync()
		{
			return null;
		}

		private static Task InitializeCoreAsync()
		{
			return null;
		}

		public static Task<bool> RestorePersistedSessionAsync()
		{
			return null;
		}

		public static void PersistCurrentSession()
		{
		}

		public static void DestroyPersistedSessionExplicitly()
		{
		}

		internal static IDisposable ProtectPersistedSessionDuringRefresh()
		{
			return null;
		}

		private static Task<bool> RestorePersistedSessionCoreAsync()
		{
			return null;
		}

		private static bool IsConfigured(ProjectConfig config)
		{
			return false;
		}

		private static void SetError(string message)
		{
		}
	}
}
