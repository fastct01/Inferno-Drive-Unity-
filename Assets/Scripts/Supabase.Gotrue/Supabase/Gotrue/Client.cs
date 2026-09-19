using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Gotrue.Interfaces;
using Supabase.Gotrue.Mfa;
using Supabase.Gotrue.Responses;

namespace Supabase.Gotrue
{
	public class Client : IGotrueClient<User, Session>, IGettableHeaders
	{
		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			public static Func<Factor, bool> _003C_003E9__73_0;

			public static Func<Factor, bool> _003C_003E9__74_0;

			public static Func<string, AmrEntry> _003C_003E9__74_1;

			internal bool _003CListFactors_003Eb__73_0(Factor x)
			{
				return false;
			}

			internal bool _003CGetAuthenticatorAssuranceLevel_003Eb__74_0(Factor factor)
			{
				return false;
			}

			internal AmrEntry _003CGetAuthenticatorAssuranceLevel_003Eb__74_1(string x)
			{
				return null;
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CChallenge_003Ed__69 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaChallengeResponse> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public MfaChallengeParams mfaChallengeParams;

			private TaskAwaiter<MfaChallengeResponse> _003C_003Eu__1;

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
		private struct _003CChallengeAndVerify_003Ed__71 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public MfaChallengeAndVerifyParams mfaChallengeAndVerifyParams;

			private TaskAwaiter<MfaChallengeResponse> _003C_003Eu__1;

			private TaskAwaiter<MfaVerifyResponse> _003C_003Eu__2;

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
		private struct _003CEnroll_003Ed__68 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaEnrollResponse> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public MfaEnrollParams mfaEnrollParams;

			private TaskAwaiter<MfaEnrollResponse> _003C_003Eu__1;

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
		private struct _003CExchangeCodeForSession_003Ed__55 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string codeVerifier;

			public string authCode;

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
		private struct _003CGetSessionFromUrl_003Ed__53 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Uri uri;

			public Client _003C_003E4__this;

			public bool storeSession;

			private NameValueCollection _003Cquery_003E5__2;

			private string _003CaccessToken_003E5__3;

			private string _003CexpiresIn_003E5__4;

			private string _003CrefreshToken_003E5__5;

			private string _003CtokenType_003E5__6;

			private TaskAwaiter<User> _003C_003Eu__1;

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
		private struct _003CReauthenticate_003Ed__48 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public Client _003C_003E4__this;

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
		private struct _003CRefreshSession_003Ed__51 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			private TaskAwaiter _003C_003Eu__1;

			private TaskAwaiter<User> _003C_003Eu__2;

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
		private struct _003CRefreshToken_003Ed__62 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public string accessToken;

			public string refreshToken;

			public Client _003C_003E4__this;

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
		private struct _003CRefreshToken_003Ed__63 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Client _003C_003E4__this;

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
		private struct _003CResetPasswordForEmail_003Ed__49 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string email;

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
		private struct _003CResetPasswordForEmail_003Ed__50 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ResetPasswordForEmailState> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public ResetPasswordForEmailOptions options;

			private TaskAwaiter<ResetPasswordForEmailState> _003C_003Eu__1;

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
		private struct _003CRetrieveSessionAsync_003Ed__54 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			private TaskAwaiter _003C_003Eu__1;

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
		private struct _003CSetSession_003Ed__52 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string accessToken;

			public string refreshToken;

			public bool forceAccessTokenRefresh;

			private TaskAwaiter<Session> _003C_003Eu__1;

			private Session _003C_003E7__wrap1;

			private Session _003C_003E7__wrap2;

			private TaskAwaiter<User> _003C_003Eu__2;

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
		private struct _003CSignIn_003Ed__29 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string email;

			public SignInOptions options;

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
		private struct _003CSignIn_003Ed__36 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public Constants.SignInType type;

			public string identifierOrToken;

			public string password;

			private TaskAwaiter<Session> _003C_003Eu__1;

			private TaskAwaiter<BaseResponse> _003C_003Eu__2;

			private TaskAwaiter _003C_003Eu__3;

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
		private struct _003CSignInAnonymously_003Ed__40 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

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
		private struct _003CSignInWithIdToken_003Ed__30 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public Constants.Provider provider;

			public string idToken;

			public string accessToken;

			public string nonce;

			public string captchaToken;

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
		private struct _003CSignInWithOtp_003Ed__31 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PasswordlessSignInState> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public SignInWithPasswordlessEmailOptions options;

			private TaskAwaiter<PasswordlessSignInState> _003C_003Eu__1;

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
		private struct _003CSignInWithOtp_003Ed__32 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<PasswordlessSignInState> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public SignInWithPasswordlessPhoneOptions options;

			private TaskAwaiter<PasswordlessSignInState> _003C_003Eu__1;

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
		private struct _003CSignOut_003Ed__46 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public Client _003C_003E4__this;

			public Constants.SignOutScope scope;

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
		private struct _003CSignUp_003Ed__28 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public Constants.SignUpType type;

			public string identifier;

			public string password;

			public SignUpOptions options;

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
		private struct _003CUnenroll_003Ed__72 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaUnenrollResponse> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public MfaUnenrollParams mfaUnenrollParams;

			private TaskAwaiter<MfaUnenrollResponse> _003C_003Eu__1;

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
		private struct _003CUpdate_003Ed__47 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<User> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public UserAttributes attributes;

			private TaskAwaiter<User> _003C_003Eu__1;

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
		private struct _003CVerify_003Ed__70 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public MfaVerifyParams mfaVerifyParams;

			private TaskAwaiter<MfaVerifyResponse> _003C_003Eu__1;

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
		private struct _003CVerifyOTP_003Ed__41 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string phone;

			public string token;

			public Constants.MobileOtpType type;

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
		private struct _003CVerifyOTP_003Ed__42 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string email;

			public string token;

			public Constants.EmailOtpType type;

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
		private struct _003CVerifyTokenHash_003Ed__43 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Client _003C_003E4__this;

			public string tokenHash;

			public Constants.EmailOtpType type;

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

		private readonly IGotrueApi<User, Session> _api;

		private readonly List<IGotrueClient<User, Session>.AuthEventHandler> _authEventHandlers;

		private DebugNotification _debugNotification;

		private IGotruePersistenceListener<Session> _sessionPersistence;

		private readonly TokenRefresh TokenRefresh__BackingField;

		private readonly ClientOptions Options__BackingField;

		private bool Online__BackingField;

		private Session CurrentSession__BackingField;

		public TokenRefresh TokenRefresh => TokenRefresh__BackingField;

		public ClientOptions Options => Options__BackingField;

		public User CurrentUser => null;

		public bool Online
		{
			get
			{
				return Online__BackingField;
			}
			set
			{
				Online__BackingField = value;
			}
		}

		public Session CurrentSession
		{
			get
			{
				return CurrentSession__BackingField;
			}
			private set
			{
				CurrentSession__BackingField = value;
			}
		}

		public Func<Dictionary<string, string>> GetHeaders
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public Client(ClientOptions options = null)
		{
		}

		public void SetPersistence(IGotrueSessionPersistence<Session> persistence)
		{
		}

		public Task<User> GetUser(string jwt)
		{
			return null;
		}

		public void NotifyAuthStateChange(Constants.AuthState stateChanged)
		{
		}

		public void AddStateChangedListener(IGotrueClient<User, Session>.AuthEventHandler authEventHandler)
		{
		}

		public void RemoveStateChangedListener(IGotrueClient<User, Session>.AuthEventHandler authEventHandler)
		{
		}

		public void ClearStateChangedListeners()
		{
		}

		public Task<Session> SignUp(string email, string password, SignUpOptions options = null)
		{
			return null;
		}

		public Task<Session> SignUp(Constants.SignUpType type, string identifier, string password, SignUpOptions options = null)
		{
			return null;
		}

		public Task<bool> SignIn(string email, SignInOptions options = null)
		{
			return null;
		}

		public Task<Session> SignInWithIdToken(Constants.Provider provider, string idToken, string accessToken = null, string nonce = null, string captchaToken = null)
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

		public Task<bool> SendMagicLink(string email, SignInOptions options = null)
		{
			return null;
		}

		public Task<Session> SignIn(string email, string password)
		{
			return null;
		}

		public Task<Session> SignInWithPassword(string email, string password)
		{
			return null;
		}

		public Task<Session> SignIn(Constants.SignInType type, string identifierOrToken, string password = null, string scopes = null)
		{
			return null;
		}

		public Task<ProviderAuthState> SignIn(Constants.Provider provider, SignInOptions options = null)
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

		public Task<Session> SignInAnonymously(SignInAnonymouslyOptions options = null)
		{
			return null;
		}

		public Task<Session> VerifyOTP(string phone, string token, Constants.MobileOtpType type = Constants.MobileOtpType.SMS)
		{
			return null;
		}

		public Task<Session> VerifyOTP(string email, string token, Constants.EmailOtpType type = Constants.EmailOtpType.MagicLink)
		{
			return null;
		}

		public Task<Session> VerifyTokenHash(string tokenHash, Constants.EmailOtpType type = Constants.EmailOtpType.Email)
		{
			return null;
		}

		public Task<ProviderAuthState> LinkIdentity(Constants.Provider provider, SignInOptions options)
		{
			return null;
		}

		public Task<bool> UnlinkIdentity(UserIdentity userIdentity)
		{
			return null;
		}

		public Task SignOut(Constants.SignOutScope scope = Constants.SignOutScope.Global)
		{
			return null;
		}

		public Task<User> Update(UserAttributes attributes)
		{
			return null;
		}

		public Task<bool> Reauthenticate()
		{
			return null;
		}

		public Task<bool> ResetPasswordForEmail(string email)
		{
			return null;
		}

		public Task<ResetPasswordForEmailState> ResetPasswordForEmail(ResetPasswordForEmailOptions options)
		{
			return null;
		}

		public Task<Session> RefreshSession()
		{
			return null;
		}

		public Task<Session> SetSession(string accessToken, string refreshToken, bool forceAccessTokenRefresh = false)
		{
			return null;
		}

		public Task<Session> GetSessionFromUrl(Uri uri, bool storeSession = true)
		{
			return null;
		}

		public Task<Session> RetrieveSessionAsync()
		{
			return null;
		}

		public Task<Session> ExchangeCodeForSession(string codeVerifier, string authCode)
		{
			return null;
		}

		public void AddDebugListener(Action<string, Exception> listener)
		{
		}

		private void UpdateSession(Session session)
		{
		}

		private void DestroySession()
		{
		}

		public Task RefreshToken(string accessToken, string refreshToken)
		{
			return null;
		}

		public Task RefreshToken()
		{
			return null;
		}

		public void LoadSession()
		{
		}

		public Task<Settings> Settings()
		{
			return null;
		}

		public void Debug(string message, Exception e = null)
		{
		}

		public void Shutdown()
		{
		}

		public Task<MfaEnrollResponse> Enroll(MfaEnrollParams mfaEnrollParams)
		{
			return null;
		}

		public Task<MfaChallengeResponse> Challenge(MfaChallengeParams mfaChallengeParams)
		{
			return null;
		}

		public Task<Session> Verify(MfaVerifyParams mfaVerifyParams)
		{
			return null;
		}

		public Task<Session> ChallengeAndVerify(MfaChallengeAndVerifyParams mfaChallengeAndVerifyParams)
		{
			return null;
		}

		public Task<MfaUnenrollResponse> Unenroll(MfaUnenrollParams mfaUnenrollParams)
		{
			return null;
		}

		public Task<MfaListFactorsResponse> ListFactors()
		{
			return null;
		}

		public Task<MfaGetAuthenticatorAssuranceLevelResponse> GetAuthenticatorAssuranceLevel()
		{
			return null;
		}
	}
}
