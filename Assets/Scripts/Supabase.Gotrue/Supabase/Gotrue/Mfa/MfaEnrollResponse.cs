namespace Supabase.Gotrue.Mfa
{
	public class MfaEnrollResponse
	{
		private string Id__BackingField;

		private string Type__BackingField;

		private TOTP Totp__BackingField;

		private string FriendlyName__BackingField;

		public string Id
		{
			get
			{
				return Id__BackingField;
			}
			set
			{
				Id__BackingField = value;
			}
		}

		public string Type
		{
			get
			{
				return Type__BackingField;
			}
			set
			{
				Type__BackingField = value;
			}
		}

		public TOTP Totp
		{
			get
			{
				return Totp__BackingField;
			}
			set
			{
				Totp__BackingField = value;
			}
		}

		public string FriendlyName
		{
			get
			{
				return FriendlyName__BackingField;
			}
			set
			{
				FriendlyName__BackingField = value;
			}
		}
	}
}
