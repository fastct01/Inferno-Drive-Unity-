namespace Supabase.Gotrue.Mfa
{
	public class AmrEntry
	{
		private string Method__BackingField;

		private long Timestamp__BackingField;

		public string Method
		{
			get
			{
				return Method__BackingField;
			}
			set
			{
				Method__BackingField = value;
			}
		}

		public long Timestamp
		{
			get
			{
				return Timestamp__BackingField;
			}
			set
			{
				Timestamp__BackingField = value;
			}
		}
	}
}
