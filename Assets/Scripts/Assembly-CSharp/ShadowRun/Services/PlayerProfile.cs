using Supabase.Postgrest.Models;

namespace ShadowRun.Services
{
	public sealed class PlayerProfile : BaseModel
	{
		private string Id__BackingField;

		private int Crystals__BackingField;

		private bool Car1__BackingField;

		private bool Car2__BackingField;

		private bool Car3__BackingField;

		private bool Car4__BackingField;

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

		public int Crystals
		{
			get
			{
				return Crystals__BackingField;
			}
			set
			{
				Crystals__BackingField = value;
			}
		}

		public bool Car1
		{
			get
			{
				return Car1__BackingField;
			}
			set
			{
				Car1__BackingField = value;
			}
		}

		public bool Car2
		{
			get
			{
				return Car2__BackingField;
			}
			set
			{
				Car2__BackingField = value;
			}
		}

		public bool Car3
		{
			get
			{
				return Car3__BackingField;
			}
			set
			{
				Car3__BackingField = value;
			}
		}

		public bool Car4
		{
			get
			{
				return Car4__BackingField;
			}
			set
			{
				Car4__BackingField = value;
			}
		}
	}
}
