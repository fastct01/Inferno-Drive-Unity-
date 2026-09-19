using System.Collections.Generic;
using Supabase.Postgrest.Exceptions;
using Supabase.Postgrest.Interfaces;

namespace Supabase.Postgrest
{
	internal class Debugger
	{
		private static Debugger _instance__BackingField;

		private readonly List<IPostgrestDebugger.DebugEventHandler> _debugListeners;

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

		public void AddDebugHandler(IPostgrestDebugger.DebugEventHandler handler)
		{
		}

		public void RemoveDebugHandler(IPostgrestDebugger.DebugEventHandler handler)
		{
		}

		public void ClearDebugHandlers()
		{
		}

		public void Log(object sender, string message, PostgrestException exception = null)
		{
		}
	}
}
