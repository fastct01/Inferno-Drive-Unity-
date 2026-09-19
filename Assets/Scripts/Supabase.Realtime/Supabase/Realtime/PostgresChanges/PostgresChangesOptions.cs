using System.Collections.Generic;

namespace Supabase.Realtime.PostgresChanges
{
	public class PostgresChangesOptions
	{
		public enum ListenType
		{
			All = 0,
			Inserts = 1,
			Updates = 2,
			Deletes = 3
		}

		private string Schema__BackingField;

		private string Table__BackingField;

		private string Filter__BackingField;

		private Dictionary<string, string> Parameters__BackingField;

		private readonly ListenType _listenType;

		public string Schema
		{
			get
			{
				return Schema__BackingField;
			}
			set
			{
				Schema__BackingField = value;
			}
		}

		public string Table
		{
			get
			{
				return Table__BackingField;
			}
			set
			{
				Table__BackingField = value;
			}
		}

		public string Filter
		{
			get
			{
				return Filter__BackingField;
			}
			set
			{
				Filter__BackingField = value;
			}
		}

		public Dictionary<string, string> Parameters
		{
			get
			{
				return Parameters__BackingField;
			}
			set
			{
				Parameters__BackingField = value;
			}
		}

		public string Event => null;

		public PostgresChangesOptions(string schema, string table = null, ListenType eventType = ListenType.All, string filter = null, Dictionary<string, string> parameters = null)
		{
		}
	}
}
