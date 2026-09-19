using Supabase.Core.Interfaces;

namespace Supabase.Storage.Interfaces
{
	public interface IStorageClient<TBucket, TFileObject> : IStorageBucketApi<TBucket>, IGettableHeaders
	{
		IStorageFileApi<TFileObject> From(string id);
	}
}
