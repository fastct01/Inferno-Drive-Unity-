namespace Supabase.Postgrest
{
	public class FullTextSearchConfig
	{
		private string QueryText__BackingField;

		private string Config__BackingField;

		public string QueryText
		{
			get
			{
				return QueryText__BackingField;
			}
			private set
			{
				QueryText__BackingField = value;
			}
		}

		public string Config
		{
			get
			{
				return Config__BackingField;
			}
			private set
			{
				Config__BackingField = value;
			}
		}

		public FullTextSearchConfig(string queryText, string config)
		{
		}
	}
}
