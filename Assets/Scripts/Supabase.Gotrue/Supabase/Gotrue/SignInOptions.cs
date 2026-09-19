using System.Collections.Generic;

namespace Supabase.Gotrue
{
	public class SignInOptions
	{
		private string RedirectTo__BackingField;

		private string Scopes__BackingField;

		private Dictionary<string, string> QueryParams__BackingField;

		private Constants.OAuthFlowType FlowType__BackingField;

		public string RedirectTo
		{
			get
			{
				return RedirectTo__BackingField;
			}
			set
			{
				RedirectTo__BackingField = value;
			}
		}

		public string Scopes
		{
			get
			{
				return Scopes__BackingField;
			}
			set
			{
				Scopes__BackingField = value;
			}
		}

		public Dictionary<string, string> QueryParams
		{
			get
			{
				return QueryParams__BackingField;
			}
			set
			{
				QueryParams__BackingField = value;
			}
		}

		public Constants.OAuthFlowType FlowType
		{
			get
			{
				return FlowType__BackingField;
			}
			set
			{
				FlowType__BackingField = value;
			}
		}
	}
}
