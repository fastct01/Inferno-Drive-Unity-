using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace Supabase
{
	public class DefaultSupabaseSessionHandler : IGotrueSessionPersistence<Session>
	{
		public void SaveSession(Session session)
		{
		}

		public void DestroySession()
		{
		}

		public Session LoadSession()
		{
			return null;
		}
	}
}
