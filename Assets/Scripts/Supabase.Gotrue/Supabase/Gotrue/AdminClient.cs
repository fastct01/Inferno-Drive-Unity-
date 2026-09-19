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
	public class AdminClient : IGotrueAdminClient<User>, IGettableHeaders
	{
		[StructLayout((LayoutKind)3)]
		private struct _003CDeleteFactor_003Ed__19 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaAdminDeleteFactorResponse> _003C_003Et__builder;

			public AdminClient _003C_003E4__this;

			public MfaAdminDeleteFactorParams deleteFactorParams;

			private TaskAwaiter<MfaAdminDeleteFactorResponse> _003C_003Eu__1;

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
		private struct _003CDeleteUser_003Ed__12 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public AdminClient _003C_003E4__this;

			public string uid;

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
		private struct _003CGenerateLink_003Ed__17 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<GenerateLinkResponse> _003C_003Et__builder;

			public AdminClient _003C_003E4__this;

			public GenerateLinkOptions options;

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
		private struct _003CInviteUserByEmail_003Ed__11 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

			public AdminClient _003C_003E4__this;

			public string email;

			public InviteUserByEmailOptions options;

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
		private struct _003CListFactors_003Ed__18 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<MfaAdminListFactorsResponse> _003C_003Et__builder;

			public AdminClient _003C_003E4__this;

			public MfaAdminListFactorsParams listFactorsParams;

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
		private struct _003CUpdate_003Ed__20 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<User> _003C_003Et__builder;

			public AdminClient _003C_003E4__this;

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

		private readonly ClientOptions Options__BackingField;

		private readonly IGotrueApi<User, Session> _api;

		private readonly string _serviceKey;

		public ClientOptions Options => Options__BackingField;

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

		public AdminClient(string serviceKey, ClientOptions options = null)
		{
		}

		public Task<User> GetUserById(string userId)
		{
			return null;
		}

		public Task<User> GetUser(string jwt)
		{
			return null;
		}

		public Task<bool> InviteUserByEmail(string email, InviteUserByEmailOptions options = null)
		{
			return null;
		}

		public Task<bool> DeleteUser(string uid)
		{
			return null;
		}

		public Task<User> CreateUser(string email, string password, AdminUserAttributes attributes = null)
		{
			return null;
		}

		public Task<User> CreateUser(AdminUserAttributes attributes)
		{
			return null;
		}

		public Task<UserList<User>> ListUsers(string filter = null, string sortBy = null, Constants.SortOrder sortOrder = Constants.SortOrder.Descending, int? page = null, int? perPage = null)
		{
			return null;
		}

		public Task<User> UpdateUserById(string userId, AdminUserAttributes userData)
		{
			return null;
		}

		public Task<GenerateLinkResponse> GenerateLink(GenerateLinkOptions options)
		{
			return null;
		}

		public Task<MfaAdminListFactorsResponse> ListFactors(MfaAdminListFactorsParams listFactorsParams)
		{
			return null;
		}

		public Task<MfaAdminDeleteFactorResponse> DeleteFactor(MfaAdminDeleteFactorParams deleteFactorParams)
		{
			return null;
		}

		public Task<User> Update(UserAttributes attributes)
		{
			return null;
		}
	}
}
