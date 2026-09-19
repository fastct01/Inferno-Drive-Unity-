using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class ClientOptions
	{
		private string Url__BackingField;

		public Dictionary<string, string> Headers;

		private bool AutoRefreshToken__BackingField;

		private bool DebugRefreshToken__BackingField;

		private int MaximumRefreshWaitTime__BackingField;

		private bool AllowUnconfirmedUserSessions__BackingField;

		public string Url
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

		public bool DebugRefreshToken
		{
			get
			{
				return DebugRefreshToken__BackingField;
			}
			set
			{
				DebugRefreshToken__BackingField = value;
			}
		}

		public int MaximumRefreshWaitTime
		{
			get
			{
				return MaximumRefreshWaitTime__BackingField;
			}
			set
			{
				MaximumRefreshWaitTime__BackingField = value;
			}
		}

		public bool AllowUnconfirmedUserSessions
		{
			get
			{
				return AllowUnconfirmedUserSessions__BackingField;
			}
			set
			{
				AllowUnconfirmedUserSessions__BackingField = value;
			}
		}
	}
}
