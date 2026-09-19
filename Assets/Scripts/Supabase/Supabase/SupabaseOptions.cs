using System.Collections.Generic;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Supabase.Storage;

namespace Supabase
{
	public class SupabaseOptions
	{
		public string Schema;

		private bool AutoRefreshToken__BackingField;

		private bool AutoConnectRealtime__BackingField;

		private IGotrueSessionPersistence<Session> SessionHandler__BackingField;

		public Dictionary<string, string> Headers;

		private Supabase.Storage.ClientOptions StorageClientOptions__BackingField;

		private string AuthUrlFormat__BackingField;

		private string RestUrlFormat__BackingField;

		private string RealtimeUrlFormat__BackingField;

		private string StorageUrlFormat__BackingField;

		private string FunctionsUrlFormat__BackingField;

		public bool AutoRefreshToken
		{
			get
			{
				return AutoRefreshToken__BackingField;
			}
			set
			{
				AutoRefreshToken__BackingField = value;
			}
		}

		public bool AutoConnectRealtime
		{
			get
			{
				return AutoConnectRealtime__BackingField;
			}
			set
			{
				AutoConnectRealtime__BackingField = value;
			}
		}

		public IGotrueSessionPersistence<Session> SessionHandler
		{
			get
			{
				return SessionHandler__BackingField;
			}
			set
			{
				SessionHandler__BackingField = value;
			}
		}

		public Supabase.Storage.ClientOptions StorageClientOptions
		{
			get
			{
				return StorageClientOptions__BackingField;
			}
			set
			{
				StorageClientOptions__BackingField = value;
			}
		}

		public string AuthUrlFormat
		{
			get
			{
				return AuthUrlFormat__BackingField;
			}
			set
			{
				AuthUrlFormat__BackingField = value;
			}
		}

		public string RestUrlFormat
		{
			get
			{
				return RestUrlFormat__BackingField;
			}
			set
			{
				RestUrlFormat__BackingField = value;
			}
		}

		public string RealtimeUrlFormat
		{
			get
			{
				return RealtimeUrlFormat__BackingField;
			}
			set
			{
				RealtimeUrlFormat__BackingField = value;
			}
		}

		public string StorageUrlFormat
		{
			get
			{
				return StorageUrlFormat__BackingField;
			}
			set
			{
				StorageUrlFormat__BackingField = value;
			}
		}

		public string FunctionsUrlFormat
		{
			get
			{
				return FunctionsUrlFormat__BackingField;
			}
			set
			{
				FunctionsUrlFormat__BackingField = value;
			}
		}
	}
}
