using Newtonsoft.Json;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime.PostgresChanges
{
	public class PostgresChangesResponse<T> : SocketResponse<PostgresChangesPayload<T>> where T : class
	{
		public PostgresChangesResponse(JsonSerializerSettings serializerSettings)
			: base((JsonSerializerSettings)null)
		{
		}
	}
	public class PostgresChangesResponse : SocketResponse<PostgresChangesPayload<SocketResponsePayload>>
	{
		public PostgresChangesResponse(JsonSerializerSettings serializerSettings)
			: base((JsonSerializerSettings)null)
		{
		}

		public virtual TModel Model<TModel>() where TModel : new()
		{
			return default;
		}

		public virtual TModel OldModel<TModel>() where TModel : new()
		{
			return default;
		}
	}
}
