using System;

namespace Supabase.Postgrest.Attributes
{
	public class TableAttribute : Attribute
	{
		private string Name__BackingField;

		public string Name
		{
			get
			{
				return Name__BackingField;
			}
			set
			{
				Name__BackingField = value;
			}
		}

		public TableAttribute(string tableName)
		{
		}
	}
}
