using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Postgrest.Interfaces;
using Supabase.Realtime.Interfaces;
using Supabase.Realtime.PostgresChanges;

namespace Supabase.Interfaces
{
	public interface ISupabaseTable<TModel, TChannel> : IPostgrestTable<TModel>, IGettableHeaders where TModel : new()
	{
		Task<TChannel> On(PostgresChangesOptions.ListenType listenType, IRealtimeChannel.PostgresChangesHandler handler);
	}
}
