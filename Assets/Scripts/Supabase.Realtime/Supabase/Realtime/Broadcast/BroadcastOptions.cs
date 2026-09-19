namespace Supabase.Realtime.Broadcast
{
	public class BroadcastOptions
	{
		private bool BroadcastSelf__BackingField;

		private bool BroadcastAck__BackingField;

		public bool BroadcastSelf
		{
			get
			{
				return BroadcastSelf__BackingField;
			}
			set
			{
				BroadcastSelf__BackingField = value;
			}
		}

		public bool BroadcastAck
		{
			get
			{
				return BroadcastAck__BackingField;
			}
			set
			{
				BroadcastAck__BackingField = value;
			}
		}

		public BroadcastOptions(bool broadcastSelf = false, bool broadcastAck = false)
		{
		}
	}
}
