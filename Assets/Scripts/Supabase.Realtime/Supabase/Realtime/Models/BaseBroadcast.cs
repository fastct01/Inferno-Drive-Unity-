using System.Collections.Generic;

namespace Supabase.Realtime.Models
{
	public class BaseBroadcast<T> : BaseBroadcast where T : class
	{
		private T Payload__BackingField;

		public new T Payload
		{
			get
			{
				return Payload__BackingField;
			}
			set
			{
				Payload__BackingField = value;
			}
		}
	}
	public class BaseBroadcast
	{
		private string Event__BackingField;

		private Dictionary<string, object> Payload__BackingField;

		public string Event
		{
			get
			{
				return Event__BackingField;
			}
			set
			{
				Event__BackingField = value;
			}
		}

		public Dictionary<string, object> Payload
		{
			get
			{
				return Payload__BackingField;
			}
			set
			{
				Payload__BackingField = value;
			}
		}
	}
}
