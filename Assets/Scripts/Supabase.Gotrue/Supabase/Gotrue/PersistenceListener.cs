using Supabase.Gotrue.Interfaces;

namespace Supabase.Gotrue
{
	public class PersistenceListener : IGotruePersistenceListener<Session>
	{
		private readonly IGotrueSessionPersistence<Session> Persistence__BackingField;

		public IGotrueSessionPersistence<Session> Persistence => Persistence__BackingField;

		public PersistenceListener(IGotrueSessionPersistence<Session> persistence)
		{
		}

		public void EventHandler(IGotrueClient<User, Session> sender, Constants.AuthState stateChanged)
		{
		}
	}
}
