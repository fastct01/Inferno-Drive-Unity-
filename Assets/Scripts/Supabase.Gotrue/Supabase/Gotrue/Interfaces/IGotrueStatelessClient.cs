using System;
using System.Threading.Tasks;
using Supabase.Gotrue.Mfa;

namespace Supabase.Gotrue.Interfaces
{
	public interface IGotrueStatelessClient<TUser, TSession>
	{
		Task<TUser> CreateUser(string serviceRoleToken, StatelessClient.StatelessClientOptions options, AdminUserAttributes attributes);

		Task<TUser> CreateUser(string serviceRoleToken, StatelessClient.StatelessClientOptions options, string email, string password, AdminUserAttributes attributes = null);

		Task<bool> DeleteUser(string uid, string serviceRoleToken, StatelessClient.StatelessClientOptions options);

		Task<TSession> ExchangeCodeForSession(string codeVerifier, string authCode, StatelessClient.StatelessClientOptions options);

		IGotrueApi<TUser, TSession> GetApi(StatelessClient.StatelessClientOptions options);

		Task<TSession> GetSessionFromUrl(Uri uri, StatelessClient.StatelessClientOptions options);

		Task<TUser> GetUser(string serviceRoleToken, StatelessClient.StatelessClientOptions options);

		Task<TUser> GetUserById(string serviceRoleToken, StatelessClient.StatelessClientOptions options, string userId);

		Task<bool> InviteUserByEmail(string email, string serviceRoleToken, StatelessClient.StatelessClientOptions options, InviteUserByEmailOptions inviteOptions = null);

		Task<UserList<User>> ListUsers(string serviceRoleToken, StatelessClient.StatelessClientOptions options, string filter = null, string sortBy = null, Constants.SortOrder sortOrder = Constants.SortOrder.Descending, int? page = null, int? perPage = null);

		Task<TSession> RefreshToken(string accessToken, string refreshToken, StatelessClient.StatelessClientOptions options);

		Task<bool> ResetPasswordForEmail(string email, StatelessClient.StatelessClientOptions options);

		Task<bool> SendMagicLink(string email, StatelessClient.StatelessClientOptions options, SignInOptions signInOptions = null);

		ProviderAuthState SignIn(Constants.Provider provider, StatelessClient.StatelessClientOptions options, SignInOptions signInOptions = null);

		Task<TSession> SignIn(Constants.SignInType type, string identifierOrToken, string password = null, StatelessClient.StatelessClientOptions options = null);

		Task<bool> SignIn(string email, StatelessClient.StatelessClientOptions options, SignInOptions signInOptions = null);

		Task<TSession> SignIn(string email, string password, StatelessClient.StatelessClientOptions options);

		Task<bool> SignOut(string accessToken, StatelessClient.StatelessClientOptions options);

		Task<TSession> SignUp(Constants.SignUpType type, string identifier, string password, StatelessClient.StatelessClientOptions options, SignUpOptions signUpOptions = null);

		Task<TSession> SignUp(string email, string password, StatelessClient.StatelessClientOptions options, SignUpOptions signUpOptions = null);

		Task<TUser> Update(string accessToken, UserAttributes attributes, StatelessClient.StatelessClientOptions options);

		Task<TUser> UpdateUserById(string serviceRoleToken, StatelessClient.StatelessClientOptions options, string userId, AdminUserAttributes userData);

		Task<TSession> VerifyOTP(string phone, string otpToken, StatelessClient.StatelessClientOptions options, Constants.MobileOtpType type = Constants.MobileOtpType.SMS);

		Task<TSession> VerifyOTP(string email, string otpToken, StatelessClient.StatelessClientOptions options, Constants.EmailOtpType type = Constants.EmailOtpType.MagicLink);

		Task<TSession> VerifyTokenHash(string tokenHash, StatelessClient.StatelessClientOptions options, Constants.EmailOtpType type = Constants.EmailOtpType.Email);

		Task<Settings> Settings(StatelessClient.StatelessClientOptions options);

		Task<MfaEnrollResponse> Enroll(string jwt, MfaEnrollParams mfaEnrollParams, StatelessClient.StatelessClientOptions options);

		Task<MfaChallengeResponse> Challenge(string jwt, MfaChallengeParams mfaChallengeParams, StatelessClient.StatelessClientOptions options);

		Task<MfaVerifyResponse> Verify(string jwt, MfaVerifyParams mfaVerifyParams, StatelessClient.StatelessClientOptions options);

		Task<MfaVerifyResponse> ChallengeAndVerify(string jwt, MfaChallengeAndVerifyParams mfaChallengeAndVerifyParams, StatelessClient.StatelessClientOptions options);

		Task<MfaUnenrollResponse> Unenroll(string jwt, MfaUnenrollParams mfaUnenrollParams, StatelessClient.StatelessClientOptions options);

		Task<MfaListFactorsResponse> ListFactors(string jwt, StatelessClient.StatelessClientOptions options);

		Task<MfaGetAuthenticatorAssuranceLevelResponse> GetAuthenticatorAssuranceLevel(string jwt, StatelessClient.StatelessClientOptions options);
	}
}
