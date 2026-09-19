using System;

namespace Supabase.Storage
{
	public class ClientOptions
	{
		public TimeSpan HttpUploadTimeout;

		public TimeSpan HttpDownloadTimeout;

		public TimeSpan HttpRequestTimeout;
	}
}
