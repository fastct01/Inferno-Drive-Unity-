using System;
using Websocket.Client;

namespace Supabase.Realtime.Exceptions
{
	public class RealtimeException : Exception
	{
		private FailureHint.Reason Reason__BackingField;

		public FailureHint.Reason Reason
		{
			get
			{
				return Reason__BackingField;
			}
			internal set
			{
				Reason__BackingField = value;
			}
		}

		public RealtimeException(string message)
		{
		}

		public RealtimeException(string message, Exception innerException)
		{
		}

		public static RealtimeException FromDisconnectionInfo(DisconnectionInfo info)
		{
			return null;
		}
	}
}
