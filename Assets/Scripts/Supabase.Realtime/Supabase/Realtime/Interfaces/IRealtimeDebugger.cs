using System;

namespace Supabase.Realtime.Interfaces
{
	public interface IRealtimeDebugger
	{
		public delegate void DebugEventHandler(object sender, string message, Exception exception);

		void AddDebugHandler(DebugEventHandler handler);

		void RemoveDebugHandler(DebugEventHandler handler);

		void ClearDebugHandlers();

		void Log(object sender, string message, Exception exception = null);
	}
}
