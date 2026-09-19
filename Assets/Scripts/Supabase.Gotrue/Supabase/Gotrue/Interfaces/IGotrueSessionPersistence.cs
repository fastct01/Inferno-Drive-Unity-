namespace Supabase.Gotrue.Interfaces
{
	public interface IGotrueSessionPersistence<TSession>
	{
		void SaveSession(TSession session);

		void DestroySession();

		TSession LoadSession();
	}
}
