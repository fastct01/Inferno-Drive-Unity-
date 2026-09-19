using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public static class Constants
	{
		public enum SortOrder
		{
			Ascending = 0,
			Descending = 1
		}

		public enum MobileOtpType
		{
			SMS = 0,
			PhoneChange = 1
		}

		public enum EmailOtpType
		{
			Signup = 0,
			Invite = 1,
			MagicLink = 2,
			Recovery = 3,
			EmailChange = 4,
			Email = 5
		}

		public enum Provider
		{
			AnonymousUsers = 0,
			Apple = 1,
			Azure = 2,
			Bitbucket = 3,
			Discord = 4,
			Facebook = 5,
			Figma = 6,
			Fly = 7,
			Github = 8,
			Gitlab = 9,
			Google = 10,
			Kakao = 11,
			KeyCloak = 12,
			LinkedIn = 13,
			LinkedInOIDC = 14,
			Notion = 15,
			Slack = 16,
			Spotify = 17,
			Twitch = 18,
			Twitter = 19,
			WorkOS = 20,
			Zoom = 21
		}

		public enum AuthState
		{
			SignedIn = 0,
			SignedOut = 1,
			UserUpdated = 2,
			PasswordRecovery = 3,
			TokenRefreshed = 4,
			Shutdown = 5,
			MfaChallengeVerified = 6
		}

		public enum SignInType
		{
			Email = 0,
			Phone = 1,
			RefreshToken = 2
		}

		public enum OAuthFlowType
		{
			Implicit = 0,
			PKCE = 1
		}

		public enum SignUpType
		{
			Email = 0,
			Phone = 1
		}

		public enum SignOutScope
		{
			Global = 0,
			Local = 1,
			Others = 2
		}

		public const string GOTRUE_URL = "http://localhost:9999";

		public const string AUDIENCE = "";

		public const int EXPIRY_MARGIN = 60000;

		public const string STORAGE_KEY = "supabase.auth.token";

		public static readonly Dictionary<string, object> CookieOptions;
	}
}
