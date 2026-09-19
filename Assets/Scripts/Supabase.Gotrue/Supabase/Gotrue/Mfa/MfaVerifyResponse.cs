namespace Supabase.Gotrue.Mfa
{
	public class MfaVerifyResponse
	{
		private string AccessToken__BackingField;

		private string TokenType__BackingField;

		private int ExpiresIn__BackingField;

		private string RefreshToken__BackingField;

		private User User__BackingField;

		public string AccessToken
		{
			get
			{
				return AccessToken__BackingField;
			}
			set
			{
				AccessToken__BackingField = value;
			}
		}

		public string TokenType
		{
			get
			{
				return TokenType__BackingField;
			}
			set
			{
				TokenType__BackingField = value;
			}
		}

		public int ExpiresIn
		{
			get
			{
				return ExpiresIn__BackingField;
			}
			set
			{
				ExpiresIn__BackingField = value;
			}
		}

		public string RefreshToken
		{
			get
			{
				return RefreshToken__BackingField;
			}
			set
			{
				RefreshToken__BackingField = value;
			}
		}

		public User User
		{
			get
			{
				return User__BackingField;
			}
			set
			{
				User__BackingField = value;
			}
		}
	}
}
