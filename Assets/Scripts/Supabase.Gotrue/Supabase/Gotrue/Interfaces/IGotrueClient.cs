using System;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Gotrue.Mfa;

namespace Supabase.Gotrue.Interfaces
{
	public interface IGotrueClient<TUser, TSession> : IGettableHeaders
	{
		public delegate void AuthEventHandler(IGotrueClient<TUser, TSession> sender, Constants.AuthState stateChanged);

		bool Online { get; set; }

		TSession CurrentSession { get; }

		TUser CurrentUser { get; }

		ClientOptions Options { get; }

		void SetPersistence(IGotrueSessionPersistence<TSession> persistence);

		void AddStateChangedListener(AuthEventHandler authEventHandler);

		void RemoveStateChangedListener(AuthEventHandler authEventHandler);

		void ClearStateChangedListeners();

		void NotifyAuthStateChange(Constants.AuthState stateChanged);

		Task<TSession> GetSessionFromUrl(Uri uri, bool storeSession = true);

		Task<TSession> RefreshSession();

		Task<bool> ResetPasswordForEmail(string email);

		Task<ResetPasswordForEmailState> ResetPasswordForEmail(ResetPasswordForEmailOptions options);

		Task<TSession> RetrieveSessionAsync();

		Task<bool> SendMagicLink(string email, SignInOptions options = null);

		Task<TSession> SetSession(string accessToken, string refreshToken, bool forceAccessTokenRefresh = false);

		Task<TSession> SignIn(Constants.SignInType type, string identifierOrToken, string password = null, string scopes = null);

		Task<bool> SignIn(string email, SignInOptions options = null);

		Task<TSession> SignIn(string email, string password);

		Task<PasswordlessSignInState> SignInWithOtp(SignInWithPasswordlessEmailOptions options);

		Task<PasswordlessSignInState> SignInWithOtp(SignInWithPasswordlessPhoneOptions options);

		Task<TSession> SignInWithPassword(string email, string password);

		Task<ProviderAuthState> SignIn(Constants.Provider provider, SignInOptions options = null);

		Task<TSession> SignInWithIdToken(Constants.Provider provider, string idToken, string accessToken = null, string nonce = null, string captchaToken = null);

		Task<TSession> SignInAnonymously(SignInAnonymouslyOptions options = null);

		Task<SSOResponse> SignInWithSSO(Guid providerId, SignInWithSSOOptions options = null);

		Task<SSOResponse> SignInWithSSO(string domain, SignInWithSSOOptions options = null);

		Task<TSession> ExchangeCodeForSession(string codeVerifier, string authCode);

		Task<TSession> SignUp(Constants.SignUpType type, string identifier, string password, SignUpOptions options = null);

		Task<TSession> SignUp(string email, string password, SignUpOptions options = null);

		Task<bool> Reauthenticate();

		Task SignOut(Constants.SignOutScope scope = Constants.SignOutScope.Global);

		Task<TUser> Update(UserAttributes attributes);

		Task<TSession> VerifyOTP(string phone, string token, Constants.MobileOtpType type = Constants.MobileOtpType.SMS);

		Task<TSession> VerifyOTP(string email, string token, Constants.EmailOtpType type = Constants.EmailOtpType.MagicLink);

		Task<TSession> VerifyTokenHash(string tokenHash, Constants.EmailOtpType type = Constants.EmailOtpType.Email);

		Task<ProviderAuthState> LinkIdentity(Constants.Provider provider, SignInOptions options);

		Task<bool> UnlinkIdentity(UserIdentity userIdentity);

		void AddDebugListener(Action<string, Exception> listener);

		void LoadSession();

		Task<Settings> Settings();

		Task<TUser> GetUser(string jwt);

		void Debug(string message, Exception e = null);

		void Shutdown();

		Task RefreshToken();

		Task<MfaEnrollResponse> Enroll(MfaEnrollParams mfaEnrollParams);

		Task<MfaChallengeResponse> Challenge(MfaChallengeParams mfaChallengeParams);

		Task<Session> Verify(MfaVerifyParams mfaVerifyParams);

		Task<Session> ChallengeAndVerify(MfaChallengeAndVerifyParams mfaChallengeAndVerifyParams);

		Task<MfaUnenrollResponse> Unenroll(MfaUnenrollParams mfaUnenrollParams);

		Task<MfaListFactorsResponse> ListFactors();

		Task<MfaGetAuthenticatorAssuranceLevelResponse> GetAuthenticatorAssuranceLevel();
	}
}
