using System;
using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class DebugNotification
	{
		private readonly List<Action<string, Exception>> _debugListeners;

		public void AddDebugListener(Action<string, Exception> listener)
		{
		}

		public void Log(string message, Exception e = null)
		{
		}
	}
}
