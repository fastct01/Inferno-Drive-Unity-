using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

// Attribute arguments recovered from global-metadata.dat (custom attribute blob at 0xaaefdf):
// [Table("profiles")], [PrimaryKey("id", false)], [Column("crystals")], [Column("car_1")..("car_4")].
namespace ShadowRun.Services
{
	[Table("profiles")]
	public sealed class PlayerProfile : BaseModel
	{
		[PrimaryKey("id", false)]
		public string Id { get; set; }

		[Column("crystals")]
		public int Crystals { get; set; }

		[Column("car_1")]
		public bool Car1 { get; set; }

		[Column("car_2")]
		public bool Car2 { get; set; }

		[Column("car_3")]
		public bool Car3 { get; set; }

		[Column("car_4")]
		public bool Car4 { get; set; }
	}
}
