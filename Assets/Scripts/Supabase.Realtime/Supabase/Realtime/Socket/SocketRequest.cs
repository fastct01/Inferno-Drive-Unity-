namespace Supabase.Realtime.Socket
{
	public class SocketRequest
	{
		private string Type__BackingField;

		private string Topic__BackingField;

		private string Event__BackingField;

		private object Payload__BackingField;

		private string Ref__BackingField;

		private string JoinRef__BackingField;

		public string Type
		{
			get
			{
				return Type__BackingField;
			}
			set
			{
				Type__BackingField = value;
			}
		}

		public string Topic
		{
			get
			{
				return Topic__BackingField;
			}
			set
			{
				Topic__BackingField = value;
			}
		}

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

		public object Payload
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

		public string Ref
		{
			get
			{
				return Ref__BackingField;
			}
			set
			{
				Ref__BackingField = value;
			}
		}

		public string JoinRef
		{
			get
			{
				return JoinRef__BackingField;
			}
			set
			{
				JoinRef__BackingField = value;
			}
		}
	}
}
