using Newtonsoft.Json;
using Supabase.Realtime.Interfaces;

namespace Supabase.Realtime.Socket
{
	public class SocketResponse<T> : SocketResponse where T : class
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

		public SocketResponse(JsonSerializerSettings serializerSettings)
			: base(null)
		{
		}
	}
	public class SocketResponse : IRealtimeSocketResponse
	{
		internal JsonSerializerSettings SerializerSettings;

		private string Topic__BackingField;

		private string _event__BackingField;

		private SocketResponsePayload Payload__BackingField;

		private string Ref__BackingField;

		private string Json__BackingField;

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

		public string _event
		{
			get
			{
				return _event__BackingField;
			}
			set
			{
				_event__BackingField = value;
			}
		}

		public Constants.EventType Event => default;

		public SocketResponsePayload Payload
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

		internal string Json
		{
			get
			{
				return Json__BackingField;
			}
			set
			{
				Json__BackingField = value;
			}
		}

		public SocketResponse(JsonSerializerSettings serializerSettings)
		{
		}
	}
}
