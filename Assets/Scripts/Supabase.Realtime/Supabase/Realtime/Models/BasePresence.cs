namespace Supabase.Realtime.Models
{
	public class BasePresence
	{
		private string PhoenixRef__BackingField;

		private string PhoenixPrevRef__BackingField;

		public string PhoenixRef
		{
			get
			{
				return PhoenixRef__BackingField;
			}
			set
			{
				PhoenixRef__BackingField = value;
			}
		}

		public string PhoenixPrevRef
		{
			get
			{
				return PhoenixPrevRef__BackingField;
			}
			set
			{
				PhoenixPrevRef__BackingField = value;
			}
		}

		public bool ShouldSerializePhoenixRef()
		{
			return false;
		}

		public bool ShouldSerializePhoenixPrevRef()
		{
			return false;
		}
	}
}
