using System.Collections.Generic;

namespace Supabase.Storage
{
	public class BucketUpsertOptions
	{
		private bool Public__BackingField;

		private string FileSizeLimit__BackingField;

		private List<string> AllowedMimes__BackingField;

		public bool Public
		{
			get
			{
				return Public__BackingField;
			}
			set
			{
				Public__BackingField = value;
			}
		}

		public string FileSizeLimit
		{
			get
			{
				return FileSizeLimit__BackingField;
			}
			set
			{
				FileSizeLimit__BackingField = value;
			}
		}

		public List<string> AllowedMimes
		{
			get
			{
				return AllowedMimes__BackingField;
			}
			set
			{
				AllowedMimes__BackingField = value;
			}
		}
	}
}
