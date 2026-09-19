using System.Collections.Generic;

namespace Supabase.Realtime.Presence
{
	public class PresenceDiffPayloadMeta<TPresence>
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
