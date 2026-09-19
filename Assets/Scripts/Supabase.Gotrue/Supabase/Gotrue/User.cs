using System;
using System.Collections.Generic;
using Supabase.Gotrue.Mfa;

namespace Supabase.Gotrue
{
	public class User
	{
		private Dictionary<string, object> AppMetadata__BackingField;

		private string Aud__BackingField;

		private DateTime? ConfirmationSentAt__BackingField;

		private DateTime? ConfirmedAt__BackingField;

		private DateTime CreatedAt__BackingField;

		private string Email__BackingField;

		private DateTime? EmailConfirmedAt__BackingField;

		private string Id__BackingField;

		private List<UserIdentity> Identities__BackingField;

		private DateTime? InvitedAt__BackingField;

		private DateTime? LastSignInAt__BackingField;

		private string Phone__BackingField;

		private DateTime? PhoneConfirmedAt__BackingField;

		private DateTime? RecoverySentAt__BackingField;

		private string Role__BackingField;

		private DateTime? UpdatedAt__BackingField;

		private DateTime? BannedUntil__BackingField;

		private bool IsAnonymous__BackingField;

		private List<Factor> Factors__BackingField;

		private Dictionary<string, object> UserMetadata__BackingField;

		public Dictionary<string, object> AppMetadata
		{
			get
			{
				return AppMetadata__BackingField;
			}
			set
			{
				AppMetadata__BackingField = value;
			}
		}

		public string Aud
		{
			get
			{
				return Aud__BackingField;
			}
			set
			{
				Aud__BackingField = value;
			}
		}

		public DateTime? ConfirmationSentAt
		{
			get
			{
				return ConfirmationSentAt__BackingField;
			}
			set
			{
				ConfirmationSentAt__BackingField = value;
			}
		}

		public DateTime? ConfirmedAt
		{
			get
			{
				return ConfirmedAt__BackingField;
			}
			set
			{
				ConfirmedAt__BackingField = value;
			}
		}

		public DateTime CreatedAt
		{
			get
			{
				return CreatedAt__BackingField;
			}
			set
			{
				CreatedAt__BackingField = value;
			}
		}

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

		public DateTime? EmailConfirmedAt
		{
			get
			{
				return EmailConfirmedAt__BackingField;
			}
			set
			{
				EmailConfirmedAt__BackingField = value;
			}
		}

		public string Id
		{
			get
			{
				return Id__BackingField;
			}
			set
			{
				Id__BackingField = value;
			}
		}

		public List<UserIdentity> Identities
		{
			get
			{
				return Identities__BackingField;
			}
			set
			{
				Identities__BackingField = value;
			}
		}

		public DateTime? InvitedAt
		{
			get
			{
				return InvitedAt__BackingField;
			}
			set
			{
				InvitedAt__BackingField = value;
			}
		}

		public DateTime? LastSignInAt
		{
			get
			{
				return LastSignInAt__BackingField;
			}
			set
			{
				LastSignInAt__BackingField = value;
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

		public DateTime? PhoneConfirmedAt
		{
			get
			{
				return PhoneConfirmedAt__BackingField;
			}
			set
			{
				PhoneConfirmedAt__BackingField = value;
			}
		}

		public DateTime? RecoverySentAt
		{
			get
			{
				return RecoverySentAt__BackingField;
			}
			set
			{
				RecoverySentAt__BackingField = value;
			}
		}

		public string Role
		{
			get
			{
				return Role__BackingField;
			}
			set
			{
				Role__BackingField = value;
			}
		}

		public DateTime? UpdatedAt
		{
			get
			{
				return UpdatedAt__BackingField;
			}
			set
			{
				UpdatedAt__BackingField = value;
			}
		}

		public DateTime? BannedUntil
		{
			get
			{
				return BannedUntil__BackingField;
			}
			set
			{
				BannedUntil__BackingField = value;
			}
		}

		public bool IsAnonymous
		{
			get
			{
				return IsAnonymous__BackingField;
			}
			set
			{
				IsAnonymous__BackingField = value;
			}
		}

		public List<Factor> Factors
		{
			get
			{
				return Factors__BackingField;
			}
			set
			{
				Factors__BackingField = value;
			}
		}

		public Dictionary<string, object> UserMetadata
		{
			get
			{
				return UserMetadata__BackingField;
			}
			set
			{
				UserMetadata__BackingField = value;
			}
		}
	}
}
