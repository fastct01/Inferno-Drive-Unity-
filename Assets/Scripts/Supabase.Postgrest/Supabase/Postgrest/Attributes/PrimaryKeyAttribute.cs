using System;

namespace Supabase.Postgrest.Attributes
{
	public class PrimaryKeyAttribute : Attribute
	{
		private readonly string ColumnName__BackingField;

		private readonly bool ShouldInsert__BackingField;

		public string ColumnName => ColumnName__BackingField;

		public bool ShouldInsert => ShouldInsert__BackingField;

		public PrimaryKeyAttribute(string columnName = null, bool shouldInsert = false)
		{
		}
	}
}
