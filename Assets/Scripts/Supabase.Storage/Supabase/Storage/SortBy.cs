namespace Supabase.Storage
{
	public class SortBy
	{
		private string Column__BackingField;

		private string Order__BackingField;

		public string Column
		{
			get
			{
				return Column__BackingField;
			}
			set
			{
				Column__BackingField = value;
			}
		}

		public string Order
		{
			get
			{
				return Order__BackingField;
			}
			set
			{
				Order__BackingField = value;
			}
		}
	}
}
