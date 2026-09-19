using System;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Gotrue.Mfa;
using Supabase.Gotrue.Responses;

namespace Supabase.Gotrue.Interfaces
{
	public interface IGotrueApi<TUser, TSession> : IGettableHeaders
	{
		Task<TUser> CreateUser(string jwt, AdminUserAttributes attributes = null);

		Task<BaseResponse> DeleteUser(string uid, string jwt);

		Task<TUser> GetUser(string jwt);

		Task<TUser> GetUserById(string jwt, string userId);

		Task<BaseResponse> InviteUserByEmail(string email, string jwt, InviteUserByEmailOptions options = null);

		Task<UserList<TUser>> ListUsers(string jwt, string filter = null, string sortBy = null, Constants.SortOrder sortOrder = Constants.SortOrder.Descending, int? page = null, int? perPage = null);

		Task<TSession> RefreshAccessToken(string accessToken, string refreshToken);

		Task<BaseResponse> ResetPasswordForEmail(string email);

		Task<ResetPasswordForEmailState> ResetPasswordForEmail(ResetPasswordForEmailOptions options);

		Task<BaseResponse> SendMagicLinkEmail(string email, SignInOptions options = null);

		Task<BaseResponse> SendMobileOTP(string phone);

		Task<TSession> SignInWithIdToken(Constants.Provider provider, string idToken, string accessToken = null, string nonce = null, string captchaToken = null);

		Task<TSession> SignInWithEmail(string email, string password);

		Task<TSession> SignInWithPhone(string phone, string password);

		Task<PasswordlessSignInState> SignInWithOtp(SignInWithPasswordlessEmailOptions options);

		Task<PasswordlessSignInState> SignInWithOtp(SignInWithPasswordlessPhoneOptions options);

		Task<TSession> SignInAnonymously(SignInAnonymouslyOptions options = null);

		Task<SSOResponse> SignInWithSSO(Guid providerId, SignInWithSSOOptions options = null);

		Task<SSOResponse> SignInWithSSO(string domain, SignInWithSSOOptions options = null);

		Task<BaseResponse> SignOut(string jwt, Constants.SignOutScope scope = Constants.SignOutScope.Global);

		Task<TSession> SignUpWithEmail(string email, string password, SignUpOptions options = null);

		Task<TSession> SignUpWithPhone(string phone, string password, SignUpOptions options = null);

		Task<TUser> UpdateUser(string jwt, UserAttributes attributes);

		Task<TUser> UpdateUserById(string jwt, string userId, UserAttributes userData);

		Task<TSession> VerifyMobileOTP(string phone, string token, Constants.MobileOtpType type);

		Task<TSession> VerifyEmailOTP(string email, string token, Constants.EmailOtpType type);

		Task<TSession> VerifyTokenHash(string tokenHash, Constants.EmailOtpType type);

		Task<BaseResponse> Reauthenticate(string userJwt);

		ProviderAuthState GetUriForProvider(Constants.Provider provider, SignInOptions options = null);

		Task<Session> ExchangeCodeForSession(string codeVerifier, string authCode);

		Task<Settings> Settings();

		Task<BaseResponse> GenerateLink(string jwt, GenerateLinkOptions options);

		Task<MfaEnrollResponse> Enroll(string jwt, MfaEnrollParams mfaEnrollParams);

		Task<MfaChallengeResponse> Challenge(string jwt, MfaChallengeParams mfaChallengeParams);

		Task<MfaVerifyResponse> Verify(string jwt, MfaVerifyParams mfaVerifyParams);

		Task<MfaUnenrollResponse> Unenroll(string jwt, MfaUnenrollParams mfaVerifyParams);

		Task<BaseResponse> ListFactors(string jwt, MfaAdminListFactorsParams listFactorsParams);

		Task<MfaAdminDeleteFactorResponse> DeleteFactor(string jwt, MfaAdminDeleteFactorParams deleteFactorParams);

		Task<ProviderAuthState> LinkIdentity(string token, Constants.Provider provider, SignInOptions options);

		Task<bool> UnlinkIdentity(string token, UserIdentity userIdentity);
	}
}
