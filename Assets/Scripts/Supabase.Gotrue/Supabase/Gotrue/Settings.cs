using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class Settings
	{
		private bool? DisableSignup__BackingField;

		private bool? MailerAutoConfirm__BackingField;

		private bool? PhoneAutoConfirm__BackingField;

		private string SmsProvider__BackingField;

		private bool? MFAEnabled__BackingField;

		private bool? SAMLEnabled__BackingField;

		private Dictionary<string, bool> ExternalProviders__BackingField;

		public bool? DisableSignup
		{
			get
			{
				return DisableSignup__BackingField;
			}
			set
			{
				DisableSignup__BackingField = value;
			}
		}

		public bool? MailerAutoConfirm
		{
			get
			{
				return MailerAutoConfirm__BackingField;
			}
			set
			{
				MailerAutoConfirm__BackingField = value;
			}
		}

		public bool? PhoneAutoConfirm
		{
			get
			{
				return PhoneAutoConfirm__BackingField;
			}
			set
			{
				PhoneAutoConfirm__BackingField = value;
			}
		}

		public string SmsProvider
		{
			get
			{
				return SmsProvider__BackingField;
			}
			set
			{
				SmsProvider__BackingField = value;
			}
		}

		public bool? MFAEnabled
		{
			get
			{
				return MFAEnabled__BackingField;
			}
			set
			{
				MFAEnabled__BackingField = value;
			}
		}

		public bool? SAMLEnabled
		{
			get
			{
				return SAMLEnabled__BackingField;
			}
			set
			{
				SAMLEnabled__BackingField = value;
			}
		}

		public Dictionary<string, bool> ExternalProviders
		{
			get
			{
				return ExternalProviders__BackingField;
			}
			set
			{
				ExternalProviders__BackingField = value;
			}
		}
	}
}
