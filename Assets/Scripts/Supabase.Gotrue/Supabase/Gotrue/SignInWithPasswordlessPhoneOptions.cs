namespace Supabase.Gotrue
{
	public class SignInWithPasswordlessPhoneOptions : SignInWithPasswordlessOptions
	{
		public enum MessagingChannel
		{
			SMS = 0,
			WHATSAPP = 1
		}

		private string Phone__BackingField;

		private MessagingChannel Channel__BackingField;

		public string Phone
		{
			get
			{
				return Phone__BackingField;
			}
			set
			{
				Phone__BackingField = value;
			}
		}

		public MessagingChannel Channel
		{
			get
			{
				return Channel__BackingField;
			}
			set
			{
				Channel__BackingField = value;
			}
		}

		public SignInWithPasswordlessPhoneOptions(string phone)
		{
		}
	}
}
