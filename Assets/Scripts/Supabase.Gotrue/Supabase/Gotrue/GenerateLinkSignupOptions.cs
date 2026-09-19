namespace Supabase.Gotrue
{
	public class GenerateLinkSignupOptions : GenerateLinkOptions
	{
		public GenerateLinkSignupOptions(string email, string password)
			: base(default, null)
		{
		}
	}
}
