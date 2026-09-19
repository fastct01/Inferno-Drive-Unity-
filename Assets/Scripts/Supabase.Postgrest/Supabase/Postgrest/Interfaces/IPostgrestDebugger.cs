using Supabase.Postgrest.Exceptions;

namespace Supabase.Postgrest.Interfaces
{
	public interface IPostgrestDebugger
	{
		public delegate void DebugEventHandler(object sender, string message, PostgrestException exception);

		void AddDebugHandler(DebugEventHandler handler);

		void RemoveDebugHandler(DebugEventHandler handler);

		void ClearDebugHandlers();

		void Log(object sender, string message, PostgrestException exception = null);
	}
}
