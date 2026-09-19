using System.Collections.Generic;

namespace Supabase.Realtime.Presence
{
	public class PresenceDiffPayload<TPresence>
	{
		private Dictionary<string, PresenceDiffPayloadMeta<TPresence>> Joins__BackingField;

		private Dictionary<string, PresenceDiffPayloadMeta<TPresence>> Leaves__BackingField;

		public Dictionary<string, PresenceDiffPayloadMeta<TPresence>> Joins
		{
			get
			{
				return Joins__BackingField;
			}
			set
			{
				Joins__BackingField = value;
			}
		}

		public Dictionary<string, PresenceDiffPayloadMeta<TPresence>> Leaves
		{
			get
			{
				return Leaves__BackingField;
			}
			set
			{
				Leaves__BackingField = value;
			}
		}
	}
}
