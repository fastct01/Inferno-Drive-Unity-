namespace Supabase.Storage
{
	public class FileOptions
	{
		private string CacheControl__BackingField;

		private string ContentType__BackingField;

		private bool Upsert__BackingField;

		public string CacheControl
		{
			get
			{
				return CacheControl__BackingField;
			}
			set
			{
				CacheControl__BackingField = value;
			}
		}

		public string ContentType
		{
			get
			{
				return ContentType__BackingField;
			}
			set
			{
				ContentType__BackingField = value;
			}
		}

		public bool Upsert
		{
			get
			{
				return Upsert__BackingField;
			}
			set
			{
				Upsert__BackingField = value;
			}
		}
	}
}
