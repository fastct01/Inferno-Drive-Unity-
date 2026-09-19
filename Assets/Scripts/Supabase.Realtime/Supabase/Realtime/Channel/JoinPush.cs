using System.Collections.Generic;
using Supabase.Realtime.Broadcast;
using Supabase.Realtime.PostgresChanges;
using Supabase.Realtime.Presence;

namespace Supabase.Realtime.Channel
{
	internal class JoinPush
	{
		internal class JoinPushConfig
		{
			private BroadcastOptions Broadcast__BackingField;

			private PresenceOptions Presence__BackingField;

			private List<PostgresChangesOptions> PostgresChanges__BackingField;

			public BroadcastOptions Broadcast
			{
				get
				{
					return Broadcast__BackingField;
				}
				set
				{
					Broadcast__BackingField = value;
				}
			}

			public PresenceOptions Presence
			{
				get
				{
					return Presence__BackingField;
				}
				set
				{
					Presence__BackingField = value;
				}
			}

			public List<PostgresChangesOptions> PostgresChanges
			{
				get
				{
					return PostgresChanges__BackingField;
				}
				set
				{
					PostgresChanges__BackingField = value;
				}
			}
		}

		private JoinPushConfig Config__BackingField;

		public JoinPushConfig Config
		{
			get
			{
				return Config__BackingField;
			}
			private set
			{
				Config__BackingField = value;
			}
		}

		public JoinPush(BroadcastOptions broadcastOptions = null, PresenceOptions presenceOptions = null, List<PostgresChangesOptions> postgresChangesOptions = null)
		{
		}
	}
}
