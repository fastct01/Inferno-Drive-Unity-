using System;
using Newtonsoft.Json;

namespace Supabase.Postgrest.Attributes
{
	public class ColumnAttribute : Attribute
	{
		private readonly string ColumnName__BackingField;

		private NullValueHandling NullValueHandling__BackingField;

		private readonly bool IgnoreOnInsert__BackingField;

		private readonly bool IgnoreOnUpdate__BackingField;

		public string ColumnName => ColumnName__BackingField;

		public NullValueHandling NullValueHandling
		{
			get
			{
				return NullValueHandling__BackingField;
			}
			set
			{
				NullValueHandling__BackingField = value;
			}
		}

		public bool IgnoreOnInsert => IgnoreOnInsert__BackingField;

		public bool IgnoreOnUpdate => IgnoreOnUpdate__BackingField;

		public ColumnAttribute(string columnName = null, NullValueHandling nullValueHandling = NullValueHandling.Include, bool ignoreOnInsert = false, bool ignoreOnUpdate = false)
		{
		}
	}
}
