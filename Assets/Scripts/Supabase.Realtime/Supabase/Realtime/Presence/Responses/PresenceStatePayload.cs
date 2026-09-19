using System.Collections.Generic;

namespace Supabase.Realtime.Presence.Responses
{
	public class PresenceStatePayload<TPresence>
	{
		private List<TPresence> Metas__BackingField;

		public List<TPresence> Metas
		{
			get
			{
				return Metas__BackingField;
			}
			set
			{
				Metas__BackingField = value;
			}
		}
	}
}
