using Supabase.Realtime.Socket;

namespace Supabase.Realtime.PostgresChanges
{
	public class PostgresChangesPayload<T> where T : class
	{
		private SocketResponsePayload<T> Data__BackingField;

		public SocketResponsePayload<T> Data
		{
			get
			{
				return Data__BackingField;
			}
			set
			{
				Data__BackingField = value;
			}
		}
	}
}
