using System.Threading;
using System.Threading.Tasks;
using Supabase.Core.Interfaces;
using Supabase.Postgrest.Requests;

namespace Supabase.Postgrest.Interfaces
{
	public interface IPostgrestTableWithCache<T> : IPostgrestTable<T>, IGettableHeaders where T : new()
	{
		new Task<CacheBackedRequest<T>> Get(CancellationToken cancellationToken = default(CancellationToken));
	}
}
