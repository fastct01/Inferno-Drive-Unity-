using System.Threading.Tasks;
using Supabase.Functions.Interfaces;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Responses;
using Supabase.Realtime.Interfaces;
using Supabase.Storage.Interfaces;

namespace Supabase.Interfaces
{
	public interface ISupabaseClient<TUser, TSession, TSocket, TChannel, TBucket, TFileObject>
	{
		IGotrueClient<TUser, TSession> Auth { get; set; }

		IFunctionsClient Functions { get; set; }

		IPostgrestClient Postgrest { get; set; }

		IRealtimeClient<TSocket, TChannel> Realtime { get; set; }

		IStorageClient<TBucket, TFileObject> Storage { get; set; }

		IGotrueAdminClient<User> AdminAuth(string serviceKey);

		ISupabaseTable<TModel, TChannel> From<TModel>() where TModel : new();

		Task<ISupabaseClient<TUser, TSession, TSocket, TChannel, TBucket, TFileObject>> InitializeAsync();

		Task<BaseResponse> Rpc(string procedureName, object parameters);

		Task<TModeledResponse> Rpc<TModeledResponse>(string procedureName, object parameters);
	}
}
