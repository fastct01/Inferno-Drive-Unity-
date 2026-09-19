namespace Supabase.Gotrue.Exceptions
{
	public static class FailureHint
	{
		public enum Reason
		{
			Unknown = 0,
			Offline = 1,
			UserEmailNotConfirmed = 2,
			UserBadMultiple = 3,
			UserBadPassword = 4,
			UserBadLogin = 5,
			UserBadEmailAddress = 6,
			UserBadPhoneNumber = 7,
			UserMissingInformation = 8,
			UserAlreadyRegistered = 9,
			UserTooManyRequests = 10,
			InvalidRefreshToken = 11,
			ExpiredRefreshToken = 12,
			AdminTokenRequired = 13,
			NoSessionFound = 14,
			BadSessionUrl = 15,
			InvalidFlowType = 16,
			SsoDomainNotFound = 17,
			SsoProviderNotFound = 18,
			MfaChallengeUnverified = 19
		}

		public static Reason DetectReason(GotrueException gte)
		{
			return default;
		}
	}
}
