using System.Threading.Tasks;

namespace Supabase.Postgrest.Interfaces
{
	public interface IPostgrestCacheProvider
	{
		Task<T> GetItem<T>(string key);

		Task SetItem(string key, object value);

		Task ClearItem(string key);

		Task Empty();
	}
}
