using System.Collections.Generic;
using Supabase.Core.Interfaces;
using Supabase.Storage.Interfaces;

namespace Supabase.Storage
{
	public class Client : StorageBucketApi, IStorageClient<Bucket, FileObject>, IStorageBucketApi<Bucket>, IGettableHeaders
	{
		public Client(string url, Dictionary<string, string> headers = null)
			: base(null, null, null)
		{
		}

		public Client(string url, ClientOptions options, Dictionary<string, string> headers = null)
			: base(null, null, null)
		{
		}

		public IStorageFileApi<FileObject> From(string id)
		{
			return null;
		}
	}
}
