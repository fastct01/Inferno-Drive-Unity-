using System.Collections.Generic;
using System.Globalization;

namespace Supabase.Postgrest
{
	public class ClientOptions
	{
		private string Schema__BackingField;

		public readonly DateTimeStyles DateTimeStyles;

		public const string DATE_TIME_FORMAT = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFK";

		private Dictionary<string, string> Headers__BackingField;

		private Dictionary<string, string> QueryParams__BackingField;

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

		public Dictionary<string, string> Headers
		{
			get
			{
				return Headers__BackingField;
			}
			set
			{
				Headers__BackingField = value;
			}
		}

		public Dictionary<string, string> QueryParams
		{
			get
			{
				return QueryParams__BackingField;
			}
			set
			{
				QueryParams__BackingField = value;
			}
		}
	}
}
