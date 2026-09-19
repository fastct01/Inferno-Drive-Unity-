using System;

namespace Supabase.Storage
{
	public class UploadSignedUrl
	{
		private readonly Uri SignedUrl__BackingField;

		private readonly string Token__BackingField;

		private readonly string Key__BackingField;

		public Uri SignedUrl => SignedUrl__BackingField;

		public string Token => Token__BackingField;

		public string Key => Key__BackingField;

		public UploadSignedUrl(Uri signedUrl, string token, string key)
		{
		}
	}
}
