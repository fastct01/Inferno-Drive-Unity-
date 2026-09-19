namespace Supabase.Gotrue.Mfa
{
	public class TOTP
	{
		private string QrCode__BackingField;

		private string Secret__BackingField;

		private string Uri__BackingField;

		public string QrCode
		{
			get
			{
				return QrCode__BackingField;
			}
			set
			{
				QrCode__BackingField = value;
			}
		}

		public string Secret
		{
			get
			{
				return Secret__BackingField;
			}
			set
			{
				Secret__BackingField = value;
			}
		}

		public string Uri
		{
			get
			{
				return Uri__BackingField;
			}
			set
			{
				Uri__BackingField = value;
			}
		}
	}
}
