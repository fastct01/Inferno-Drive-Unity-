using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class AdminUserAttributes : UserAttributes
	{
		private Dictionary<string, object> AppMetadata__BackingField;

		private Dictionary<string, object> UserMetadata__BackingField;

		private bool? EmailConfirm__BackingField;

		private bool? PhoneConfirm__BackingField;

		private string BanDuration__BackingField;

		public Dictionary<string, object> AppMetadata
		{
			get
			{
				return AppMetadata__BackingField;
			}
			set
			{
				AppMetadata__BackingField = value;
			}
		}

		public Dictionary<string, object> UserMetadata
		{
			get
			{
				return UserMetadata__BackingField;
			}
			set
			{
				UserMetadata__BackingField = value;
			}
		}

		public bool? EmailConfirm
		{
			get
			{
				return EmailConfirm__BackingField;
			}
			set
			{
				EmailConfirm__BackingField = value;
			}
		}

		public bool? PhoneConfirm
		{
			get
			{
				return PhoneConfirm__BackingField;
			}
			set
			{
				PhoneConfirm__BackingField = value;
			}
		}

		public string BanDuration
		{
			get
			{
				return BanDuration__BackingField;
			}
			set
			{
				BanDuration__BackingField = value;
			}
		}
	}
}
