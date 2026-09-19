using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Supabase.Gotrue.Interfaces;
using Supabase.Gotrue.Mfa;
using Supabase.Gotrue.Responses;

namespace Supabase.Gotrue
{
	public class StatelessClient : IGotrueStatelessClient<User, Session>
	{
		public class StatelessClientOptions
		{
			private string Url__BackingField;

			public readonly Dictionary<string, string> Headers;

			private bool AllowUnconfirmedUserSessions__BackingField;

			public string Url
			{
				get
				{
					return Url__BackingField;
				}
				set
				{
					Url__BackingField = value;
				}
			}

			public bool AllowUnconfirmedUserSessions
			{
				get
				{
					return AllowUnconfirmedUserSessions__BackingField;
				}
				set
				{
					AllowUnconfirmedUserSessions__BackingField = value;
				}
			}
		}

		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			public static Func<Factor, bool> _003C_003E9__6_0;

			public static Func<Factor, bool> _003C_003E9__7_0;

			public static Func<string, AmrEntry> _003C_003E9__7_1;

			internal bool _003CListFactors_003Eb__6_0(Factor x)
			{
				return false;
			}

			internal bool _003CGetAuthenticatorAssuranceLevel_003Eb__7_0(Factor factor)
			{
				return false;
			}

			internal AmrEntry _003CGetAuthenticatorAssuranceLevel_003Eb__7_1(string x)
			{
				return null;
			}
		}

		[StructLayout((LayoutKind)3)]
		private struct _003CChallenge_003Ed__2 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaChallengeResponse> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string jwt;

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
		private struct _003CChallengeAndVerify_003Ed__4 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaVerifyResponse> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string jwt;

			public MfaChallengeAndVerifyParams mfaChallengeAndVerifyParams;

			private IGotrueApi<User, Session> _003Capi_003E5__2;

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
		private struct _003CCreateUser_003Ed__27 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<User> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string serviceRoleToken;

			public AdminUserAttributes attributes;

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
		private struct _003CDeleteUser_003Ed__29 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string uid;

			public string serviceRoleToken;

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
		private struct _003CEnroll_003Ed__1 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaEnrollResponse> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string jwt;

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
		private struct _003CExchangeCodeForSession_003Ed__30 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

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
		private struct _003CGetAuthenticatorAssuranceLevel_003Ed__7 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaGetAuthenticatorAssuranceLevelResponse> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string jwt;

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
		private struct _003CGetSessionFromUrl_003Ed__31 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public Uri uri;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			private string _003CaccessToken_003E5__2;

			private string _003CexpiresIn_003E5__3;

			private string _003CrefreshToken_003E5__4;

			private string _003CtokenType_003E5__5;

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
		private struct _003CGetUser_003Ed__25 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<User> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string serviceRoleToken;

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
		private struct _003CGetUserById_003Ed__24 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<User> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string serviceRoleToken;

			public string userId;

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
		private struct _003CInviteUserByEmail_003Ed__21 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string email;

			public string serviceRoleToken;

			public InviteUserByEmailOptions invitationOptions;

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
		private struct _003CListFactors_003Ed__6 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaListFactorsResponse> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string jwt;

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
		private struct _003CListUsers_003Ed__23 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<UserList<User>> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string serviceRoleToken;

			public string filter;

			public string sortBy;

			public Constants.SortOrder sortOrder;

			public int? page;

			public int? perPage;

			private TaskAwaiter<UserList<User>> _003C_003Eu__1;

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
		private struct _003CRefreshToken_003Ed__32 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string accessToken;

			public string refreshToken;

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
		private struct _003CResetPasswordForEmail_003Ed__22 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

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
		private struct _003CSettings_003Ed__0 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Settings> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			private TaskAwaiter<Settings> _003C_003Eu__1;

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
		private struct _003CSignIn_003Ed__11 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string email;

			public SignInOptions signInOptions;

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
		private struct _003CSignIn_003Ed__14 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public StatelessClientOptions options;

			public StatelessClient _003C_003E4__this;

			public Constants.SignInType type;

			public string identifierOrToken;

			public string password;

			private TaskAwaiter<Session> _003C_003Eu__1;

			private TaskAwaiter<BaseResponse> _003C_003Eu__2;

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
		private struct _003CSignOut_003Ed__16 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string accessToken;

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
		private struct _003CSignUp_003Ed__10 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public Constants.SignUpType type;

			public string identifier;

			public string password;

			public SignUpOptions signUpOptions;

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
		private struct _003CUnenroll_003Ed__5 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaUnenrollResponse> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string jwt;

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
		private struct _003CUpdate_003Ed__20 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<User> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string accessToken;

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
		private struct _003CUpdateUserById_003Ed__28 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<User> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string serviceRoleToken;

			public string userId;

			public AdminUserAttributes userData;

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
		private struct _003CVerify_003Ed__3 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaVerifyResponse> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string jwt;

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
		private struct _003CVerifyOTP_003Ed__17 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string phone;

			public string otpToken;

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
		private struct _003CVerifyOTP_003Ed__18 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

			public string email;

			public string otpToken;

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
		private struct _003CVerifyTokenHash_003Ed__19 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<Session> _003C_003Et__builder;

			public StatelessClient _003C_003E4__this;

			public StatelessClientOptions options;

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

		public Task<Settings> Settings(StatelessClientOptions options)
		{
			return null;
		}

		public Task<MfaEnrollResponse> Enroll(string jwt, MfaEnrollParams mfaEnrollParams, StatelessClientOptions options)
		{
			return null;
		}

		public Task<MfaChallengeResponse> Challenge(string jwt, MfaChallengeParams mfaChallengeParams, StatelessClientOptions options)
		{
			return null;
		}

		public Task<MfaVerifyResponse> Verify(string jwt, MfaVerifyParams mfaVerifyParams, StatelessClientOptions options)
		{
			return null;
		}

		public Task<MfaVerifyResponse> ChallengeAndVerify(string jwt, MfaChallengeAndVerifyParams mfaChallengeAndVerifyParams, StatelessClientOptions options)
		{
			return null;
		}

		public Task<MfaUnenrollResponse> Unenroll(string jwt, MfaUnenrollParams mfaUnenrollParams, StatelessClientOptions options)
		{
			return null;
		}

		public Task<MfaListFactorsResponse> ListFactors(string jwt, StatelessClientOptions options)
		{
			return null;
		}

		public Task<MfaGetAuthenticatorAssuranceLevelResponse> GetAuthenticatorAssuranceLevel(string jwt, StatelessClientOptions options)
		{
			return null;
		}

		public IGotrueApi<User, Session> GetApi(StatelessClientOptions options)
		{
			return null;
		}

		public Task<Session> SignUp(string email, string password, StatelessClientOptions options, SignUpOptions signUpOptions = null)
		{
			return null;
		}

		public Task<Session> SignUp(Constants.SignUpType type, string identifier, string password, StatelessClientOptions options, SignUpOptions signUpOptions = null)
		{
			return null;
		}

		public Task<bool> SignIn(string email, StatelessClientOptions options, SignInOptions signInOptions = null)
		{
			return null;
		}

		public Task<bool> SendMagicLink(string email, StatelessClientOptions options, SignInOptions signInOptions = null)
		{
			return null;
		}

		public Task<Session> SignIn(string email, string password, StatelessClientOptions options)
		{
			return null;
		}

		public Task<Session> SignIn(Constants.SignInType type, string identifierOrToken, string password = null, StatelessClientOptions options = null)
		{
			return null;
		}

		public ProviderAuthState SignIn(Constants.Provider provider, StatelessClientOptions options, SignInOptions signInOptions = null)
		{
			return null;
		}

		public Task<bool> SignOut(string accessToken, StatelessClientOptions options)
		{
			return null;
		}

		public Task<Session> VerifyOTP(string phone, string otpToken, StatelessClientOptions options, Constants.MobileOtpType type = Constants.MobileOtpType.SMS)
		{
			return null;
		}

		public Task<Session> VerifyOTP(string email, string otpToken, StatelessClientOptions options, Constants.EmailOtpType type = Constants.EmailOtpType.MagicLink)
		{
			return null;
		}

		public Task<Session> VerifyTokenHash(string tokenHash, StatelessClientOptions options, Constants.EmailOtpType type = Constants.EmailOtpType.Email)
		{
			return null;
		}

		public Task<User> Update(string accessToken, UserAttributes attributes, StatelessClientOptions options)
		{
			return null;
		}

		public Task<bool> InviteUserByEmail(string email, string serviceRoleToken, StatelessClientOptions options, InviteUserByEmailOptions invitationOptions = null)
		{
			return null;
		}

		public Task<bool> ResetPasswordForEmail(string email, StatelessClientOptions options)
		{
			return null;
		}

		public Task<UserList<User>> ListUsers(string serviceRoleToken, StatelessClientOptions options, string filter = null, string sortBy = null, Constants.SortOrder sortOrder = Constants.SortOrder.Descending, int? page = null, int? perPage = null)
		{
			return null;
		}

		public Task<User> GetUserById(string serviceRoleToken, StatelessClientOptions options, string userId)
		{
			return null;
		}

		public Task<User> GetUser(string serviceRoleToken, StatelessClientOptions options)
		{
			return null;
		}

		public Task<User> CreateUser(string serviceRoleToken, StatelessClientOptions options, string email, string password, AdminUserAttributes attributes = null)
		{
			return null;
		}

		public Task<User> CreateUser(string serviceRoleToken, StatelessClientOptions options, AdminUserAttributes attributes)
		{
			return null;
		}

		public Task<User> UpdateUserById(string serviceRoleToken, StatelessClientOptions options, string userId, AdminUserAttributes userData)
		{
			return null;
		}

		public Task<bool> DeleteUser(string uid, string serviceRoleToken, StatelessClientOptions options)
		{
			return null;
		}

		public Task<Session> ExchangeCodeForSession(string codeVerifier, string authCode, StatelessClientOptions options)
		{
			return null;
		}

		public Task<Session> GetSessionFromUrl(Uri uri, StatelessClientOptions options)
		{
			return null;
		}

		public Task<Session> RefreshToken(string accessToken, string refreshToken, StatelessClientOptions options)
		{
			return null;
		}
	}
}
