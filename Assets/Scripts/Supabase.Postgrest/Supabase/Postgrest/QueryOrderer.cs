namespace Supabase.Postgrest
{
	public class QueryOrderer
	{
		private readonly string ForeignTable__BackingField;

		private readonly string Column__BackingField;

		private readonly Constants.Ordering Ordering__BackingField;

		private readonly Constants.NullPosition NullPosition__BackingField;

		public string ForeignTable => ForeignTable__BackingField;

		public string Column => Column__BackingField;

		public Constants.Ordering Ordering => Ordering__BackingField;

		public Constants.NullPosition NullPosition => NullPosition__BackingField;

		public QueryOrderer(string foreignTable, string column, Constants.Ordering ordering, Constants.NullPosition nullPosition)
		{
		}
	}
}
