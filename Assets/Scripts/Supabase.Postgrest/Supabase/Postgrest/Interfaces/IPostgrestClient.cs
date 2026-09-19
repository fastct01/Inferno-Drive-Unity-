using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Postgrest.Responses;

namespace Supabase.Postgrest.Interfaces
{
	public interface IPostgrestClient : IGettableHeaders
	{
		string BaseUrl { get; }

		ClientOptions Options { get; }

		void AddRequestPreparedHandler(OnRequestPreparedEventHandler handler);

		void RemoveRequestPreparedHandler(OnRequestPreparedEventHandler handler);

		void ClearRequestPreparedHandlers();

		void AddDebugHandler(IPostgrestDebugger.DebugEventHandler handler);

		void RemoveDebugHandler(IPostgrestDebugger.DebugEventHandler handler);

		void ClearDebugHandlers();

		Task<BaseResponse> Rpc(string procedureName, object parameters);

		Task<TModeledResponse> Rpc<TModeledResponse>(string procedureName, object parameters = null);

		IPostgrestTable<T> Table<T>() where T : new();

		IPostgrestTableWithCache<T> Table<T>(IPostgrestCacheProvider cacheProvider) where T : new();
	}
}
