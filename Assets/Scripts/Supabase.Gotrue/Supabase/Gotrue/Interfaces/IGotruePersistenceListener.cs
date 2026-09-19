namespace Supabase.Gotrue.Interfaces
{
	public interface IGotruePersistenceListener<TSession>
	{
		IGotrueSessionPersistence<TSession> Persistence { get; }

		void EventHandler(IGotrueClient<User, TSession> sender, Constants.AuthState stateChanged);
	}
}
