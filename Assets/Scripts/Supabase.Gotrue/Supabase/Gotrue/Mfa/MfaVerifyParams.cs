namespace Supabase.Gotrue.Mfa
{
	public class MfaVerifyParams
	{
		private string FactorId__BackingField;

		private string ChallengeId__BackingField;

		private string Code__BackingField;

		public string FactorId
		{
			get
			{
				return FactorId__BackingField;
			}
			set
			{
				FactorId__BackingField = value;
			}
		}

		public string ChallengeId
		{
			get
			{
				return ChallengeId__BackingField;
			}
			set
			{
				ChallengeId__BackingField = value;
			}
		}

		public string Code
		{
			get
			{
				return Code__BackingField;
			}
			set
			{
				Code__BackingField = value;
			}
		}
	}
}
