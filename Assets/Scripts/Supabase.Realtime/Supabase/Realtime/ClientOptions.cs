using System;
using System.Collections.Generic;
using System.Globalization;
using Supabase.Realtime.Socket;

namespace Supabase.Realtime
{
	public class ClientOptions
	{
		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9;

			public static Func<int, TimeSpan> _003C_003E9__35_0;

			internal TimeSpan _003C_002Ector_003Eb__35_0(int tries)
			{
				return default;
			}
		}

		private Action<object, Action<string>> Encode__BackingField;

		private Action<string, Action<SocketResponse>> Decode__BackingField;

		private string Transport__BackingField;

		private TimeSpan Timeout__BackingField;

		private int EventsPerSecond__BackingField;

		private TimeSpan HeartbeatInterval__BackingField;

		private Func<int, TimeSpan> ReconnectAfterInterval__BackingField;

		public readonly Dictionary<string, string> Headers;

		public SocketOptionsParameters Parameters;

		public readonly DateTimeStyles DateTimeStyles;

		private string DateTimeFormat__BackingField;

		public Action<object, Action<string>> Encode
		{
			get
			{
				return Encode__BackingField;
			}
			set
			{
				Encode__BackingField = value;
			}
		}

		public Action<string, Action<SocketResponse>> Decode
		{
			get
			{
				return Decode__BackingField;
			}
			set
			{
				Decode__BackingField = value;
			}
		}

		public string Transport
		{
			get
			{
				return Transport__BackingField;
			}
			set
			{
				Transport__BackingField = value;
			}
		}

		public TimeSpan Timeout
		{
			get
			{
				return Timeout__BackingField;
			}
			set
			{
				Timeout__BackingField = value;
			}
		}

		public int EventsPerSecond
		{
			get
			{
				return EventsPerSecond__BackingField;
			}
			set
			{
				EventsPerSecond__BackingField = value;
			}
		}

		public TimeSpan HeartbeatInterval
		{
			get
			{
				return HeartbeatInterval__BackingField;
			}
			set
			{
				HeartbeatInterval__BackingField = value;
			}
		}

		public Func<int, TimeSpan> ReconnectAfterInterval
		{
			get
			{
				return ReconnectAfterInterval__BackingField;
			}
			set
			{
				ReconnectAfterInterval__BackingField = value;
			}
		}

		public string DateTimeFormat
		{
			get
			{
				return DateTimeFormat__BackingField;
			}
			set
			{
				DateTimeFormat__BackingField = value;
			}
		}
	}
}
