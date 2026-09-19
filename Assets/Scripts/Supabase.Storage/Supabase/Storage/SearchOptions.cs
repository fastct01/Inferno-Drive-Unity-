namespace Supabase.Storage
{
	public class SearchOptions
	{
		private int Limit__BackingField;

		private int Offset__BackingField;

		private string Search__BackingField;

		private SortBy SortBy__BackingField;

		public int Limit
		{
			get
			{
				return Limit__BackingField;
			}
			set
			{
				Limit__BackingField = value;
			}
		}

		public int Offset
		{
			get
			{
				return Offset__BackingField;
			}
			set
			{
				Offset__BackingField = value;
			}
		}

		public string Search
		{
			get
			{
				return Search__BackingField;
			}
			set
			{
				Search__BackingField = value;
			}
		}

		public SortBy SortBy
		{
			get
			{
				return SortBy__BackingField;
			}
			set
			{
				SortBy__BackingField = value;
			}
		}
	}
}
