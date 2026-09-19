using System;

namespace Supabase.Gotrue
{
	public class Session
	{
		private string AccessToken__BackingField;

		private long ExpiresIn__BackingField;

		private string ProviderToken__BackingField;

		private string ProviderRefreshToken__BackingField;

		private string RefreshToken__BackingField;

		private string TokenType__BackingField;

		private User User__BackingField;

		private DateTime CreatedAt__BackingField;

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

		public long ExpiresIn
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

		public string ProviderToken
		{
			get
			{
				return ProviderToken__BackingField;
			}
			set
			{
				ProviderToken__BackingField = value;
			}
		}

		public string ProviderRefreshToken
		{
			get
			{
				return ProviderRefreshToken__BackingField;
			}
			set
			{
				ProviderRefreshToken__BackingField = value;
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

		public DateTime CreatedAt
		{
			get
			{
				return CreatedAt__BackingField;
			}
			set
			{
				CreatedAt__BackingField = value;
			}
		}

		public DateTime ExpiresAt()
		{
			return default;
		}

		public bool Expired()
		{
			return false;
		}
	}
}
