namespace Supabase.Gotrue
{
	public class VerifyOTPParams
	{
		private string Email__BackingField;

		private string Phone__BackingField;

		private string Token__BackingField;

		private string Type__BackingField;

		public string Email
		{
			get
			{
				return Email__BackingField;
			}
			set
			{
				Email__BackingField = value;
			}
		}

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

		public string Token
		{
			get
			{
				return Token__BackingField;
			}
			set
			{
				Token__BackingField = value;
			}
		}

		public string Type
		{
			get
			{
				return Type__BackingField;
			}
			set
			{
				Type__BackingField = value;
			}
		}
	}
}
