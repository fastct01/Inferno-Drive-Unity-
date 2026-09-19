using System.Collections.Generic;

namespace Supabase.Postgrest
{
	public class QueryOptions
	{
		public enum ReturnType
		{
			Minimal = 0,
			Representation = 1
		}

		public enum CountType
		{
			None = 0,
			Exact = 1,
			Planned = 2,
			Estimated = 3
		}

		public enum DuplicateResolutionType
		{
			MergeDuplicates = 0,
			IgnoreDuplicates = 1
		}

		private ReturnType Returning__BackingField;

		private DuplicateResolutionType DuplicateResolution__BackingField;

		private CountType Count__BackingField;

		private bool Upsert__BackingField;

		private string OnConflict__BackingField;

		public ReturnType Returning
		{
			get
			{
				return Returning__BackingField;
			}
			set
			{
				Returning__BackingField = value;
			}
		}

		public DuplicateResolutionType DuplicateResolution
		{
			get
			{
				return DuplicateResolution__BackingField;
			}
			set
			{
				DuplicateResolution__BackingField = value;
			}
		}

		public CountType Count
		{
			get
			{
				return Count__BackingField;
			}
			set
			{
				Count__BackingField = value;
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

		public string OnConflict
		{
			get
			{
				return OnConflict__BackingField;
			}
			set
			{
				OnConflict__BackingField = value;
			}
		}

		public Dictionary<string, string> ToHeaders()
		{
			return null;
		}
	}
}
