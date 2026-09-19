using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Gotrue.Interfaces;
using Supabase.Gotrue.Mfa;
using Supabase.Gotrue.Responses;

namespace Supabase.Gotrue
{
	public class Api : IGotrueApi<User, Session>, IGettableHeaders
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CLinkIdentity_003Ed__40 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ProviderAuthState> _003C_003Et__builder;

			public Api _003C_003E4__this;

			public Constants.Provider provider;

			public SignInOptions options;

			public string token;

			private ProviderAuthState _003Cstate_003E5__2;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CResetPasswordForEmail_003Ed__30 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ResetPasswordForEmailState> _003C_003Et__builder;

			public ResetPasswordForEmailOptions options;

			public Api _003C_003E4__this;

			private string _003Cverifier_003E5__2;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSignInAnonymously_003Ed__16 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Api _003C_003E4__this;

			public SignInAnonymouslyOptions options;

			private TaskAwaiter<Session> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSignInWithOtp_003Ed__14 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PasswordlessSignInState> _003C_003Et__builder;

			public SignInWithPasswordlessEmailOptions options;

			public Api _003C_003E4__this;

			private string _003Cverifier_003E5__2;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSignInWithOtp_003Ed__15 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PasswordlessSignInState> _003C_003Et__builder;

			public Api _003C_003E4__this;

			public SignInWithPasswordlessPhoneOptions options;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CSignUpWithEmail_003Ed__12 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public string email;

			public string password;

			public Api _003C_003E4__this;

			public SignUpOptions options;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CUnlinkIdentity_003Ed__41 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public Api _003C_003E4__this;

			public UserIdentity userIdentity;

			public string token;

			private TaskAwaiter<BaseResponse> _003C_003Eu__1;

			private void MoveNext()
			{
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private readonly string Url__BackingField;

		private Func<Dictionary<string, string>> GetHeaders__BackingField;

		private Dictionary<string, string> _headers;

		private string Url => Url__BackingField;

		public Func<Dictionary<string, string>> GetHeaders
		{
			get
			{
				return GetHeaders__BackingField;
			}
			set
			{
				GetHeaders__BackingField = value;
			}
		}

		protected Dictionary<string, string> Headers
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public Api(string url, Dictionary<string, string> headers = null)
		{
		}

		public Task<Session> SignUpWithEmail(string email, string password, SignUpOptions options = null)
		{
			return null;
		}

		public Task<Session> SignInWithEmail(string email, string password)
		{
			return null;
		}

		public Task<PasswordlessSignInState> SignInWithOtp(SignInWithPasswordlessEmailOptions options)
		{
			return null;
		}

		public Task<PasswordlessSignInState> SignInWithOtp(SignInWithPasswordlessPhoneOptions options)
		{
			return null;
		}

		public Task<Session> SignInAnonymously(SignInAnonymouslyOptions options = null)
		{
			return null;
		}

		public Task<Session> SignInWithIdToken(Constants.Provider provider, string idToken, string accessToken = null, string nonce = null, string captchaToken = null)
		{
			return null;
		}

		private Task<SSOResponse> SignInWithSsoInternal(Guid? providerId = null, string domain = null, SignInWithSSOOptions options = null)
		{
			return null;
		}

		public Task<SSOResponse> SignInWithSSO(Guid providerId, SignInWithSSOOptions options = null)
		{
			return null;
		}

		public Task<SSOResponse> SignInWithSSO(string domain, SignInWithSSOOptions options = null)
		{
			return null;
		}

		public Task<BaseResponse> SendMagicLinkEmail(string email, SignInOptions options = null)
		{
			return null;
		}

		public Task<BaseResponse> InviteUserByEmail(string email, string jwt, InviteUserByEmailOptions options = null)
		{
			return null;
		}

		public Task<Session> SignUpWithPhone(string phone, string password, SignUpOptions options = null)
		{
			return null;
		}

		public Task<Session> SignInWithPhone(string phone, string password)
		{
			return null;
		}

		public Task<BaseResponse> SendMobileOTP(string phone)
		{
			return null;
		}

		public Task<Session> VerifyMobileOTP(string phone, string token, Constants.MobileOtpType type)
		{
			return null;
		}

		public Task<Session> VerifyEmailOTP(string email, string token, Constants.EmailOtpType type)
		{
			return null;
		}

		public Task<Session> VerifyTokenHash(string tokenHash, Constants.EmailOtpType type)
		{
			return null;
		}

		public Task<BaseResponse> ResetPasswordForEmail(string email)
		{
			return null;
		}

		public Task<ResetPasswordForEmailState> ResetPasswordForEmail(ResetPasswordForEmailOptions options)
		{
			return null;
		}

		private Dictionary<string, string> CreateAuthedRequestHeaders(string jwt)
		{
			return null;
		}

		public ProviderAuthState GetUriForProvider(Constants.Provider provider, SignInOptions options = null)
		{
			return null;
		}

		public Task<Session> ExchangeCodeForSession(string codeVerifier, string authCode)
		{
			return null;
		}

		public Task<MfaEnrollResponse> Enroll(string jwt, MfaEnrollParams mfaEnrollParams)
		{
			return null;
		}

		public Task<MfaChallengeResponse> Challenge(string jwt, MfaChallengeParams mfaChallengeParams)
		{
			return null;
		}

		public Task<MfaVerifyResponse> Verify(string jwt, MfaVerifyParams mfaVerifyParams)
		{
			return null;
		}

		public Task<MfaUnenrollResponse> Unenroll(string jwt, MfaUnenrollParams mfaUnenrollParams)
		{
			return null;
		}

		public Task<BaseResponse> ListFactors(string jwt, MfaAdminListFactorsParams listFactorsParams)
		{
			return null;
		}

		public Task<MfaAdminDeleteFactorResponse> DeleteFactor(string jwt, MfaAdminDeleteFactorParams deleteFactorParams)
		{
			return null;
		}

		public Task<ProviderAuthState> LinkIdentity(string token, Constants.Provider provider, SignInOptions options)
		{
			return null;
		}

		public Task<bool> UnlinkIdentity(string token, UserIdentity userIdentity)
		{
			return null;
		}

		public Task<BaseResponse> SignOut(string jwt, Constants.SignOutScope scope = Constants.SignOutScope.Global)
		{
			return null;
		}

		public Task<User> GetUser(string jwt)
		{
			return null;
		}

		public Task<User> GetUserById(string jwt, string userId)
		{
			return null;
		}

		public Task<User> UpdateUser(string jwt, UserAttributes attributes)
		{
			return null;
		}

		public Task<UserList<User>> ListUsers(string jwt, string filter = null, string sortBy = null, Constants.SortOrder sortOrder = Constants.SortOrder.Descending, int? page = null, int? perPage = null)
		{
			return null;
		}

		private Dictionary<string, string> TransformListUsersParams(string filter = null, string sortBy = null, Constants.SortOrder sortOrder = Constants.SortOrder.Descending, int? page = null, int? perPage = null)
		{
			return null;
		}

		public Task<User> CreateUser(string jwt, AdminUserAttributes attributes = null)
		{
			return null;
		}

		public Task<User> UpdateUserById(string jwt, string userId, UserAttributes userData)
		{
			return null;
		}

		public Task<BaseResponse> Reauthenticate(string userJwt)
		{
			return null;
		}

		public Task<BaseResponse> DeleteUser(string uid, string jwt)
		{
			return null;
		}

		public Task<Settings> Settings()
		{
			return null;
		}

		public Task<BaseResponse> GenerateLink(string jwt, GenerateLinkOptions options)
		{
			return null;
		}

		public Task<Session> RefreshAccessToken(string accessToken, string refreshToken)
		{
			return null;
		}
	}
}
