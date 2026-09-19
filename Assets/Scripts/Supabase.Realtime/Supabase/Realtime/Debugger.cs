using System;
using System.Collections.Generic;
using Supabase.Realtime.Interfaces;

namespace Supabase.Realtime
{
	internal class Debugger : IRealtimeDebugger
	{
		private static Debugger _instance__BackingField;

		private readonly List<IRealtimeDebugger.DebugEventHandler> _debugListeners;

		private static Debugger _instance
		{
			get
			{
				return _instance__BackingField;
			}
			set
			{
				_instance__BackingField = value;
			}
		}

		public static Debugger Instance => null;

		private Debugger()
		{
		}

		public void AddDebugHandler(IRealtimeDebugger.DebugEventHandler handler)
		{
		}

		public void RemoveDebugHandler(IRealtimeDebugger.DebugEventHandler handler)
		{
		}

		public void ClearDebugHandlers()
		{
		}

		public void Log(object sender, string message, Exception exception = null)
		{
		}
	}
}
