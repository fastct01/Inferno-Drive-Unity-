using System;

namespace Supabase.Gotrue
{
	public class SSOResponse : ProviderAuthState
	{
		public SSOResponse(Uri uri)
			: base(null)
		{
		}
	}
}
