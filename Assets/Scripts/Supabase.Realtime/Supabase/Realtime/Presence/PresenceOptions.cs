namespace Supabase.Realtime.Presence
{
	public class PresenceOptions
	{
		private string PresenceKey__BackingField;

		public string PresenceKey
		{
			get
			{
				return PresenceKey__BackingField;
			}
			set
			{
				PresenceKey__BackingField = value;
			}
		}

		public PresenceOptions(string presenceKey)
		{
		}
	}
}
