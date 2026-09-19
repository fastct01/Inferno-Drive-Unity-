using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Gotrue.Mfa;
using Supabase.Gotrue.Responses;

namespace Supabase.Gotrue.Interfaces
{
	public interface IGotrueAdminClient<TUser> : IGettableHeaders
	{
		Task<TUser> CreateUser(AdminUserAttributes attributes);

		Task<TUser> CreateUser(string email, string password, AdminUserAttributes attributes = null);

		Task<bool> DeleteUser(string uid);

		Task<TUser> GetUser(string jwt);

		Task<TUser> GetUserById(string userId);

		Task<bool> InviteUserByEmail(string email, InviteUserByEmailOptions options = null);

		Task<UserList<TUser>> ListUsers(string filter = null, string sortBy = null, Constants.SortOrder sortOrder = Constants.SortOrder.Descending, int? page = null, int? perPage = null);

		Task<User> Update(UserAttributes attributes);

		Task<User> UpdateUserById(string userId, AdminUserAttributes userData);

		Task<GenerateLinkResponse> GenerateLink(GenerateLinkOptions options);

		Task<MfaAdminListFactorsResponse> ListFactors(MfaAdminListFactorsParams listFactorsParams);

		Task<MfaAdminDeleteFactorResponse> DeleteFactor(MfaAdminDeleteFactorParams deleteFactorParams);
	}
}
