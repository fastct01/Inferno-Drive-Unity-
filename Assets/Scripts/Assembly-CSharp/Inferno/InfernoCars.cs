using System;
using System.Collections.Generic;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// [user request 2026-09-21: cars] The garage catalogue: ten models in four rarities instead of
// the 4 shipped cards + 25 InfernoGarage recolour editions (retired; InfernoGarage forwards
// here). Five are the meshes the export ships (sportcar, W222 AMG, Cayman, P900, flying car),
// five are editions of them: the same mesh with its own factory paint, stance (ride height,
// wheel size) and stats. /* [user request 2026-09-24: plain cars] */ The primitive body kits
// the editions bolted on until now (ApplyKit: arches, wings, splitters, roof rack, bull bar,
// light bar, fins, side pods, targa hoops, underglow, thrusters) are gone - the cars are the
// plain models again, and the edition subtitles lost their "+" classes. original: "five are
// kit-bashed variants of them: body-kit parts built from primitives in ApplyKit (...)".
// Price and rarity go together (carousel = price order); ownership of the six non-cloud
// ids is PlayerPrefs (InfernoDrive.Vehicle.<id>.Unlocked, the editions' key) bridged for
// SupabaseGameManager; the four shipped ids keep their cloud columns. Paint comes from
// InfernoStyle.SelectedPaint (the PAINT tab) or the model's default; the arcade stats are the
// base values UpdatePlayer scales by InfernoUpgrades. Card icons of the variants are rendered at runtime from the
// garage preview world (Icon) and served through InfernoGarage.Picture as
// "Data/Raw/InfernoCars/<id>.png".
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoCars
	{
		public enum Rarity { Common, Rare, Epic, Legendary }

		public const int Count = 10;
		public const string StarterId = "sportcar";
		public const string IconPrefix = "Data/Raw/InfernoCars/";   // InfernoGarage.Picture routes these to Icon()
		public const int IconWidth = 256, IconHeight = 192;

		private enum Base { Sport, Amg, Cayman, Infal, Flying }

		private sealed class Model
		{
			public readonly string id;              // vehicle id: shadowRunSelectedVehicle, InfernoDrive.Vehicle.<id>.Unlocked, InfernoDrive.Upgrade.<id>.*
			public readonly Base base_;             // whose definition and prefab to start from
			public readonly bool variant;           // an edition of a base mesh: own paint / stance / stats, runtime-rendered card icon /* [user request 2026-09-24: plain cars] */ // original: Kit kit (ApplyKit)
			public readonly string name, subtitle;
			public readonly Rarity rarity;
			public readonly int price;              // crystals; 0 = the starter
			public readonly float accel, steer, grip, drift;   // VehicleDefinition.Arcade* before upgrades
			public readonly string[] paintMaterials;           // recoloured by Decorate / InfernoStyle
			public readonly Color defaultPaint;                // the PAINT tab's "DEFAULT" swatch
			public readonly bool paintDefault;                 // apply defaultPaint even without a chosen paint (the editions, the black W222)
			public readonly string icon;                       // card + case picture resource
			public readonly float ride;                        // ride-height change in metres (+ = lift)
			public readonly float wheelScale;

			public Model(string id, Base base_, bool variant, string name, string subtitle, Rarity rarity, int price,
				float accel, float steer, float grip, float drift, string[] paintMaterials, Color defaultPaint, bool paintDefault,
				string icon, float ride, float wheelScale)
			{
				this.id = id; this.base_ = base_; this.variant = variant; this.name = name; this.subtitle = subtitle; this.rarity = rarity;
				this.price = price; this.accel = accel; this.steer = steer; this.grip = grip; this.drift = drift;
				this.paintMaterials = paintMaterials; this.defaultPaint = defaultPaint; this.paintDefault = paintDefault;
				this.icon = icon; this.ride = ride; this.wheelScale = wheelScale;
			}
		}

		private static Color Rgb(float r, float g, float b) { return new Color(r, g, b, 1f); }
		private static string IconPath(string id) { return IconPrefix + id + ".png"; }

		private const string W222Model = "uploads_files_4881612_uploads_files_3750868_MB.W222.AMG";
		private const string SportPaint = "main", AmgPaint = "W222 - Car Color", CaymanPaint = "Paint", InfalPaint = "dtP900_car_DeTomaso_P900_carpaint1", FlyingPaint = "car base";

		// Price order == rarity order (the carousel shows them in this order, starter first).
		/* [user request 2026-09-24: plain cars] */ // editions: variant = true (own paint, stance, rendered icon), subtitles without "+" classes.
		// original: Kit.WideBody / Armour / ClubSport / Spyder / Interceptor; "Wide-body / Class S+", "Armoured / Class A+", "Cup / Class S+", "Open Proto / Class S+"
		private static readonly Model[] Models =
		{
			new Model("sportcar",       Base.Sport,  false,           "PHANTOM GT",       "Hypersport / Class S",   Rarity.Common,    0,    1.04f, 1.00f, 1.02f, 1.00f, new[] { SportPaint },  Rgb(0.106f, 0.262f, 0.416f), false, "sportcar photo", 0f, 1f),
			new Model("amg_w222",       Base.Amg,    false,           "AMG SOVEREIGN",    "Saloon / Class A",       Rarity.Common,    120,  1.00f, 0.92f, 1.04f, 0.90f, new[] { AmgPaint },    Rgb(0.60f, 0.62f, 0.68f),    true,  "Data/Raw/InfernoLobby/cars/amg_w222.png", 0f, 1f),
			new Model("car3",           Base.Cayman, false,           "CAYMAN GT4",       "Track GT / Class S",     Rarity.Rare,      300,  1.00f, 1.10f, 1.14f, 0.90f, new[] { CaymanPaint }, Rgb(1.00f, 0.375f, 0.00f),   false, "porsche photo", 0f, 1f),
			new Model("sportcar_rs",    Base.Sport,  true,            "PHANTOM RS",       "Sport Edition / Class S",Rarity.Rare,      340,  1.10f, 1.04f, 1.06f, 1.02f, new[] { SportPaint },  Rgb(0.86f, 0.08f, 0.12f),    true,  IconPath("sportcar_rs"), -0.04f, 1f),
			new Model("infal_model",    Base.Infal,  false,           "IFNAL P900",       "Prototype / Class S",    Rarity.Epic,      650,  1.12f, 0.94f, 0.96f, 1.14f, new[] { InfalPaint },  Rgb(0.269f, 0.096f, 0.307f), false, "impala photo", 0f, 1f),
			new Model("amg_armour",     Base.Amg,    true,            "SOVEREIGN ARMOUR", "Raised Edition / Class A",Rarity.Epic,      700,  1.02f, 0.90f, 1.10f, 0.86f, new[] { AmgPaint },    Rgb(0.18f, 0.21f, 0.19f),    true,  IconPath("amg_armour"), 0.06f, 1.12f),
			new Model("car3_clubsport", Base.Cayman, true,            "CAYMAN CLUBSPORT", "Cup Edition / Class S",  Rarity.Epic,      750,  1.08f, 1.14f, 1.18f, 0.94f, new[] { CaymanPaint }, Rgb(0.93f, 0.93f, 0.95f),    true,  IconPath("car3_clubsport"), -0.05f, 1f),
			new Model("flying_car",     Base.Flying, false,           "AERO WRAITH",      "Hover GT / Class A",     Rarity.Legendary, 1200, 1.08f, 1.06f, 1.10f, 0.78f, new[] { FlyingPaint }, Rgb(0.906f, 0.906f, 0.906f), false, "flyingcar photo", 0f, 1f),
			new Model("infal_spyder",   Base.Infal,  true,            "P900 SPYDER",      "Gold Edition / Class S", Rarity.Legendary, 1300, 1.16f, 0.98f, 1.00f, 1.16f, new[] { InfalPaint },  Rgb(0.85f, 0.62f, 0.18f),    true,  IconPath("infal_spyder"), 0f, 1f),
			new Model("flying_x",       Base.Flying, true,            "AERO WRAITH X",    "Interceptor / Class X",  Rarity.Legendary, 1500, 1.14f, 1.10f, 1.14f, 0.80f, new[] { FlyingPaint }, Rgb(0.10f, 0.04f, 0.20f),    true,  IconPath("flying_x"), 0f, 1f),
		};

		private static readonly string[] RarityNames = { "COMMON", "RARE", "EPIC", "LEGENDARY" };
		private static readonly Color[] RarityColors =
		{
			new Color(0.62f, 0.68f, 0.78f, 1f),   // Common
			new Color(0.24f, 0.70f, 1.00f, 1f),   // Rare
			new Color(0.76f, 0.30f, 1.00f, 1f),   // Epic
			new Color(1.00f, 0.68f, 0.16f, 1f),   // Legendary
		};
		public static readonly int[] CaseWeights = { 40, 30, 20, 10 };   // case prize share per rarity, %

		// The four ids whose ownership lives in the cloud profile (SupabaseGameManager.IsVehicleUnlocked).
		private static readonly string[] CloudIds = { "sportcar", "flying_car", "infal_model", "car3" };

		private static string[] s_ids;

		// ---- catalogue ----------------------------------------------------------------------------
		public static string[] Ids
		{
			get
			{
				if (s_ids == null)
				{
					s_ids = new string[Models.Length];
					for (int i = 0; i < Models.Length; ++i) s_ids[i] = Models[i].id;
				}
				return s_ids;
			}
		}

		public static bool SameId(string a, string b) { return a != null && b != null && string.Equals(a, b, StringComparison.OrdinalIgnoreCase); }

		private static Model Find(string id)
		{
			if (string.IsNullOrEmpty(id)) return null;
			for (int i = 0; i < Models.Length; ++i) if (SameId(Models[i].id, id)) return Models[i];
			return null;
		}

		public static bool Contains(string id) { return Find(id) != null; }
		public static bool IsCloudId(string id) { for (int i = 0; i < CloudIds.Length; ++i) if (SameId(CloudIds[i], id)) return true; return false; }
		public static string Name(string id) { Model m = Find(id); return m != null ? m.name : null; }
		public static string Subtitle(string id) { Model m = Find(id); return m != null ? m.subtitle : null; }
		public static Rarity RarityOf(string id) { Model m = Find(id); return m != null ? m.rarity : Rarity.Common; }
		public static string RarityName(Rarity r) { return RarityNames[(int)r]; }
		public static Color RarityColor(Rarity r) { return RarityColors[(int)r]; }
		public static int Price(string id) { Model m = Find(id); return m != null ? m.price : -1; }   // 0 for the starter, -1 unknown
		public static bool IsStarter(string id) { return SameId(id, StarterId); }
		public static bool IsVariant(string id) { Model m = Find(id); return m != null && m.variant; }   /* [user request 2026-09-24: plain cars] */ // original: HasKit(id) = kit != Kit.None
		public static string[] PaintMaterials(string id) { Model m = Find(id); return m != null ? (string[])m.paintMaterials.Clone() : new string[0]; }
		public static Color DefaultPaint(string id) { Model m = Find(id); return m != null ? m.defaultPaint : Color.white; }
		public static string CaseName(string id) { Model m = Find(id); return m != null ? m.name : null; }
		public static string CasePicture(string id) { Model m = Find(id); return m != null ? m.icon : null; }

		// The photo shown while a variant's rendered icon does not exist (its base model's photo).
		public static string FallbackPicture(string id)
		{
			Model m = Find(id);
			if (m == null) return "sportcar photo";
			switch (m.base_)
			{
				case Base.Amg: return "Data/Raw/InfernoLobby/cars/amg_w222.png";
				case Base.Cayman: return "porsche photo";
				case Base.Infal: return "impala photo";
				case Base.Flying: return "flyingcar photo";
				default: return "sportcar photo";
			}
		}

		public static string IdFromPicture(string resource)
		{
			if (resource == null || !resource.StartsWith(IconPrefix, StringComparison.Ordinal) || !resource.EndsWith(".png", StringComparison.Ordinal)) return null;
			return resource.Substring(IconPrefix.Length, resource.Length - IconPrefix.Length - 4);
		}

		// ---- card stats (0..1 bars: POWER / HANDLING / NITRO, base stats + upgrade levels) ------------
		public const float StatPerUpgradeLevel = 0.04f;

		public static float BaseStat(string id, InfernoUpgrades.Stat stat)
		{
			Model m = Find(id);
			if (m == null) return 0f;
			switch (stat)
			{
				case InfernoUpgrades.Stat.Engine: return Mathf.Clamp01((m.accel - 0.80f) / 0.40f);
				case InfernoUpgrades.Stat.Handling: return Mathf.Clamp01(((m.steer + m.grip) * 0.5f - 0.80f) / 0.40f);
				default: return Mathf.Clamp01((m.accel * 0.5f + m.drift * 0.5f - 0.78f) / 0.40f);
			}
		}

		public static float Stat(string id, InfernoUpgrades.Stat stat)
		{
			return Mathf.Clamp01(BaseStat(id, stat) + StatPerUpgradeLevel * InfernoUpgrades.Level(id, stat));
		}

		public static GarageVehicleCardData[] Cards(ShadowRunPrototype game)
		{
			GarageVehicleCardData[] cards = new GarageVehicleCardData[Models.Length];
			for (int i = 0; i < Models.Length; ++i)
			{
				Model m = Models[i];
				GarageVehicleCardData card = new GarageVehicleCardData();
				card.VehicleId = m.id;
				card.Name = m.name;
				card.Subtitle = m.subtitle;
				card.Rarity = RarityNames[(int)m.rarity];
				card.IconResource = m.icon;
				Color tint = RarityColors[(int)m.rarity];
				card.Color = tint;
				card.Accent = Color.Lerp(tint, Color.white, 0.45f);
				card.Power = Stat(m.id, InfernoUpgrades.Stat.Engine);
				card.Handling = Stat(m.id, InfernoUpgrades.Stat.Handling);
				card.Speed = Stat(m.id, InfernoUpgrades.Stat.Nitro);   // the NITRO bar
				card.PowerValue = Mathf.RoundToInt((600f + 400f * card.Power) / 10f) * 10 + " HP";
				card.HandlingValue = Mathf.RoundToInt(card.Handling * 100f) + "%";
				card.SpeedValue = Mathf.RoundToInt(card.Speed * 100f) + "%";
				card.VisualLocked = !SupabaseGameManager.IsVehicleUnlocked(m.id);
				cards[i] = card;
			}
			return cards;
		}

		// ---- ownership (PlayerPrefs for the six non-cloud ids; the cloud profile has four car columns) ---
		public static string PrefKey(string id) { return "InfernoDrive.Vehicle." + id + ".Unlocked"; }

		// -1 = not ours (unknown, or one of the four cloud ids that SupabaseGameManager answers), else 0/1.
		public static int IsUnlocked(string id)
		{
			Model m = Find(id);
			if (m == null || IsCloudId(m.id)) return -1;
			return PlayerPrefs.GetInt(PrefKey(m.id), 0) != 0 ? 1 : 0;
		}

		public static bool SetUnlocked(string id, bool unlocked)
		{
			Model m = Find(id);
			if (m == null || IsCloudId(m.id)) return false;
			PlayerPrefs.SetInt(PrefKey(m.id), unlocked ? 1 : 0);
			PlayerPrefs.Save();
			return true;
		}

		public static List<string> LockedIds()   // locked cars in price order
		{
			List<string> locked = new List<string>(Models.Length);
			for (int i = 0; i < Models.Length; ++i)
				if (!SupabaseGameManager.IsVehicleUnlocked(Models[i].id)) locked.Add(Models[i].id);
			return locked;
		}

		// ---- cases ----------------------------------------------------------------------------------
		// Rarity-weighted pick among the locked cars: the tiers that still have a locked car share the
		// roll 40 / 30 / 20 / 10 (renormalised over the present tiers), uniform inside a tier.
		public static string PickCasePrize(float roll01)
		{
			List<string> locked = LockedIds();
			if (locked.Count == 0) return null;
			int[] perTier = new int[RarityNames.Length];
			for (int i = 0; i < locked.Count; ++i) perTier[(int)RarityOf(locked[i])]++;
			int lastTier = -1;
			float total = 0f;
			for (int t = 0; t < perTier.Length; ++t) if (perTier[t] > 0) { total += CaseWeights[t]; lastTier = t; }
			float roll = Mathf.Clamp01(roll01) * total;
			for (int t = 0; t < perTier.Length; ++t)
			{
				if (perTier[t] == 0) continue;
				float weight = CaseWeights[t];
				if (roll >= weight && t != lastTier) { roll -= weight; continue; }
				int index = Mathf.Min(perTier[t] - 1, (int)(Mathf.Clamp01(roll / weight) * perTier[t]));   // uniform inside the tier
				for (int i = 0; i < locked.Count; ++i)
					if ((int)RarityOf(locked[i]) == t && index-- == 0) return locked[i];
			}
			return locked[locked.Count - 1];
		}

		// ---- definitions ----------------------------------------------------------------------------
		// CreateVehicleDefinition(id) asks here first. Every model starts from its base car's definition
		// (the W222 from the sportcar's plus the traffic prefab with that model's wheel / brake-disc
		// mesh names, as the retired InfernoGarage edition did), takes its own id, names, ride height
		// and base arcade stats (the InfernoUpgrades multipliers are applied by UpdatePlayer).
		public static VehicleDefinition MakeDefinition(ShadowRunPrototype game, string id)
		{
			Model m = Find(id);
			if (m == null || game == null) return null;
			VehicleDefinition d = null;
			switch (m.base_)
			{
				case Base.Sport: d = game.CreateDefaultSportCarDefinition(); break;
				case Base.Amg: d = game.CreateDefaultSportCarDefinition(); ApplyW222(d); break;
				case Base.Cayman: d = game.CreateCar3Definition(); break;
				case Base.Infal: d = game.CreateInfalModelDefinition(); break;
				case Base.Flying: d = game.CreateFlyingCarDefinition(); break;
			}
			if (d == null) return null;
			if (!IsCloudId(m.id))
			{
				d.Id = m.id;
				d.PlayerModelName = "Player " + m.name + " Model";
				d.GarageModelName = "Garage Preview " + m.name;
			}
			if (m.variant)   /* [user request 2026-09-24: plain cars] */ // original: m.kit != Kit.None
			{
				d.RoadYOffset += m.ride;
				d.GarageRoadYOffset += m.ride;
			}
			// The catalogue's base stats only: UpdatePlayer multiplies them by the InfernoUpgrades
			// multipliers every frame (and adds the speed bonuses), so they must not be folded in here
			// as well. A bought upgrade still re-creates the active definition (InfernoUpgrades.Buy).
			d.ArcadeAcceleration = m.accel;
			d.ArcadeSteering = m.steer;
			d.ArcadeGrip = m.grip;
			d.ArcadeDrift = m.drift;
			return d;
		}

		private static string[] Strings(params string[] items) { return items; }

		private static void ApplyW222(VehicleDefinition d)
		{
			d.ModelResource = W222Model;
			d.WheelMeshNames = Strings("W222.Wheel.Ft.L", "W222.Wheel.Ft.R", "W222.Wheel.Bk.L", "W222.Wheel.Bk.R");
			d.BrakeDiscNames = Strings("W222.Brake.Ft.L", "W222.Brake.Ft.R", "W222.Brake.Bk.L", "W222.Brake.Bk.R");
			d.BrakeLightNames = Strings();           // the lamps are baked into the body's photo textures
			d.BrakeLightMaterialNames = Strings();
			d.BoundsRendererNameKeywords = Strings("W222");   // ignore the Mball helper when fitting
			d.RoadYOffset = 0.05f;
			d.HideOversizedPlaneRenderers = true;
			d.GarageTargetFootprint = 2.6f;
		}

		// ---- model ------------------------------------------------------------------------------------
		// Called at the end of CreateConfiguredVehicleModel (player car and garage preview alike):
		// hides the traffic prefab's metaball helper, gives the P900's origin-pivoted wheel meshes real
		// pivots, repaints the paint material(s) with a per-slot MaterialPropertyBlock (glass, lights
		// and chrome on the same renderer keep their look; nothing shared with the base car changes),
		// and scales the wheels. /* [user request 2026-09-24: plain cars] */ original: "... and bolts the body kit on" (ApplyKit).
		public static void Decorate(ShadowRunPrototype game, VehicleDefinition definition, GameObject instance)
		{
			if (game == null || definition == null || instance == null) return;
			Model m = Find(definition.Id);
			if (m == null) return;
			Transform root = instance.transform;
			if (m.base_ == Base.Amg)
			{
				Transform ball = root.Find("Mball");
				if (ball != null) ball.gameObject.SetActive(false);
			}
			if (m.base_ == Base.Infal) PivotP900Wheels(root);
			InfernoStyle.PaintChoice paint = InfernoStyle.SelectedPaint(m.id);
			Color colour = paint.set ? paint.color : m.defaultPaint;
			float metallic = paint.set ? paint.metallic : -1f;
			float smoothness = paint.set ? paint.smoothness : -1f;
			if (paint.set || m.paintDefault) ApplyPaint(root, m.paintMaterials, PaintBlock(colour, metallic, smoothness));
			if (m.wheelScale != 1f) ScaleWheels(root, definition, m.wheelScale);
		}

		private static MaterialPropertyBlock PaintBlock(Color colour, float metallic, float smoothness)
		{
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			block.SetColor("_BaseColor", colour);
			block.SetColor("_Color", colour);
			if (metallic >= 0f) block.SetFloat("_Metallic", Mathf.Clamp01(metallic));
			if (smoothness >= 0f) block.SetFloat("_Smoothness", Mathf.Clamp01(smoothness));
			return block;
		}

		private static void ApplyPaint(Transform root, string[] paintMaterials, MaterialPropertyBlock block)
		{
			Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
			if (renderers == null) return;
			for (int i = 0; i < renderers.Length; ++i)
			{
				Renderer renderer = renderers[i];
				if (renderer == null) continue;
				Material[] materials = renderer.sharedMaterials;
				if (materials == null) continue;
				for (int j = 0; j < materials.Length; ++j)
				{
					Material material = materials[j];
					if (material == null) continue;
					bool hit = false;
					for (int k = 0; k < paintMaterials.Length && !hit; ++k) hit = InfernoGarage.SameName(material.name, paintMaterials[k]);
					if (hit) renderer.SetPropertyBlock(block, j);
				}
			}
		}

		private static bool NameListed(string name, string[] list)
		{
			if (list == null) return false;
			for (int i = 0; i < list.Length; ++i) if (SameId(name, list[i])) return true;
			return false;
		}

		private static void ScaleWheels(Transform root, VehicleDefinition definition, float scale)
		{
			Transform[] nodes = root.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < nodes.Length; ++i)
			{
				Transform node = nodes[i];
				if (node == null || node == root) continue;
				if (NameListed(node.name, definition.WheelMeshNames) || NameListed(node.name, definition.BrakeDiscNames))
					node.localScale = node.localScale * scale;
			}
		}

		// ---- P900 wheel pivots ------------------------------------------------------------------------
		// Every mesh of IFNAL_MODEL_P900 is a flat child of the prefab root with its transform at the
		// origin (dtP900:tires_wheelFL_tire_UV1_shared_tire_racing, dtP900:rims_wheelFL_UV2_…,
		// dtP900:emissive_wheelFL_…_brakedisk_…, the motion-blur and normal-map tire copies), so
		// spinning a tire node would swing the wheel around the car. Each corner's meshes (calipers
		// stay put) are re-parented under a pivot at the tire's centre, named P900 Wheel FL/FR/BL/BR —
		// the names CreateInfalModelDefinition lists as WheelMeshNames, so the game spins the front
		// pivots and InfernoStyle tints what hangs under them. Local X is the axle (the tire's thinnest
		// mesh axis), which is what SetPlayerWheelSpin rotates about.
		private static readonly string[] Corners = { "FL", "FR", "BL", "BR" };

		public static string P900WheelName(int corner) { return "P900 Wheel " + Corners[corner]; }

		private static void PivotP900Wheels(Transform root)
		{
			if (root.Find(P900WheelName(0)) != null) return;   // already re-pivoted (the game rebuilds models freely)
			Transform[] nodes = root.GetComponentsInChildren<Transform>(true);
			List<Transform> parts = new List<Transform>();
			for (int c = 0; c < Corners.Length; ++c)
			{
				string tag = "wheel" + Corners[c];
				parts.Clear();
				Transform tire = null;
				for (int i = 0; i < nodes.Length; ++i)
				{
					Transform node = nodes[i];
					if (node == null || node == root || node.parent != root) continue;
					string name = node.name;
					if (name.IndexOf(tag, StringComparison.OrdinalIgnoreCase) < 0) continue;
					if (name.IndexOf("caliper", StringComparison.OrdinalIgnoreCase) >= 0) continue;
					parts.Add(node);
					if (tire == null && name.IndexOf("tires_", StringComparison.OrdinalIgnoreCase) >= 0) tire = node;
				}
				if (parts.Count == 0) continue;
				if (tire == null) tire = parts[0];
				MeshFilter filter = tire.GetComponent<MeshFilter>();
				if (filter == null || filter.sharedMesh == null) continue;
				Bounds mesh = filter.sharedMesh.bounds;   // mesh space == root space: identity node transforms
				bool axleX = mesh.size.x <= mesh.size.z;
				Transform pivot = new GameObject(P900WheelName(c)).transform;
				pivot.SetParent(root, false);
				pivot.localRotation = axleX ? Quaternion.identity : Quaternion.Euler(0f, 90f, 0f);
				pivot.localScale = Vector3.one;
				pivot.position = tire.TransformPoint(mesh.center);
				for (int i = 0; i < parts.Count; ++i) parts[i].SetParent(pivot, true);
			}
		}

		/* [user request 2026-09-24: plain cars] */ // The kit-bashed body parts (Finish / Frame / Builder / Measure / ApplyKit / Build*) that used to live here were removed.

		// ---- runtime icons --------------------------------------------------------------------------------
		// A variant has no photo, so its card / case picture is the edition rendered once from the
		// garage preview world (its lights, layer 29) into a 256x192 sprite. Needs the preview world
		// (MakeGaragePreviewWorld) and a live render pipeline; returns null until both exist, so the
		// callers fall back to the base model's photo and try again later (ShowGarageScreen).
		private static readonly Dictionary<string, Sprite> s_icons = new Dictionary<string, Sprite>();

		public static Sprite Icon(ShadowRunPrototype game, string id)
		{
			Model m = Find(id);
			if (m == null) return null;
			Sprite cached;
			if (s_icons.TryGetValue(m.id, out cached) && cached != null) return cached;
			if (game == null || !m.variant) return null;   // the base cars keep their photos
			Sprite rendered = RenderIcon(game, m);
			if (rendered != null) s_icons[m.id] = rendered;
			return rendered;
		}

		public static bool HasIcon(string id) { Sprite s; return s_icons.TryGetValue(id ?? "", out s) && s != null; }

		private static Sprite RenderIcon(ShadowRunPrototype game, Model m)
		{
			if (game.garagePreviewRoot == null || game.garagePreviewPivot == null || !Application.isPlaying) return null;
			if (Time.frameCount < 2) return null;   // not while the game builds itself in Awake: the pipeline renders nothing before its first frame
			VehicleDefinition definition = MakeDefinition(game, m.id);
			if (definition == null || Resources.Load<GameObject>(definition.ModelResource) == null) return null;
			GameObject root = game.garagePreviewRoot.gameObject;
			bool rootWasActive = root.activeSelf;
			GameObject previewCar = game.garagePreviewVehicleRoot;
			bool carWasActive = previewCar != null && previewCar.activeSelf;
			Transform stage = null;
			GameObject cameraObject = null;
			RenderTexture target = null;
			Sprite sprite = null;
			try
			{
				root.SetActive(true);
				if (previewCar != null) previewCar.SetActive(false);
				stage = new GameObject("Icon Stage").transform;
				stage.SetParent(game.garagePreviewPivot, false);
				stage.localPosition = new Vector3(GaragePreviewHorizontalOffset, 0f, 0f);
				stage.localRotation = Quaternion.Euler(0f, 130f, 0f);   // the preview's opening pose
				GameObject model = game.CreateConfiguredVehicleModel(stage, "Icon " + m.name, definition);
				if (model == null) return null;
				model.SetActive(true);
				model.transform.localPosition = Vector3.zero;
				model.transform.localRotation = Quaternion.Euler(0f, definition.ModelYawOffset, 0f);
				model.transform.localScale = Vector3.one;
				Bounds bounds;
				if (game.TryGetActiveRendererBounds(model.transform, out bounds, definition.BoundsRendererNameKeywords))
				{
					Vector3 centre = stage.InverseTransformPoint(bounds.center);
					Vector3 min = stage.InverseTransformPoint(bounds.min);
					float footprint = Mathf.Max(bounds.size.x, bounds.size.z);
					float scale = footprint > 0.001f ? Mathf.Min(definition.GarageTargetFootprint / footprint, definition.GarageMaximumScale) : 1f;
					model.transform.localScale = Vector3.one * scale;
					model.transform.localPosition = new Vector3(-centre.x * scale + definition.GarageHorizontalOffset, -min.y * scale + definition.GarageRoadYOffset, -centre.z * scale);
				}
				game.SetLayerRecursively(stage.gameObject, GaragePreviewLayer);
				cameraObject = new GameObject("Icon Camera");
				Camera camera = cameraObject.AddComponent<Camera>();
				camera.enabled = false;
				cameraObject.transform.SetParent(game.garagePreviewRoot, false);
				Vector3 focus = game.garagePreviewPivot.localPosition + new Vector3(GaragePreviewHorizontalOffset, 0.42f, 0f);
				cameraObject.transform.localPosition = focus + new Vector3(3.4f, 2.1f, -5.4f);
				cameraObject.transform.LookAt(game.garagePreviewRoot.TransformPoint(focus));
				camera.clearFlags = CameraClearFlags.Color;
				camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
				camera.cullingMask = 1 << GaragePreviewLayer;
				camera.fieldOfView = 30f;
				camera.nearClipPlane = 0.03f;
				camera.farClipPlane = 80f;
				camera.allowHDR = false;
				camera.allowMSAA = false;
				target = new RenderTexture(IconWidth, IconHeight, 16, RenderTextureFormat.ARGB32);
				target.name = "Icon " + m.id;
				target.Create();
				UniversalRenderPipeline.SingleCameraRequest request = new UniversalRenderPipeline.SingleCameraRequest();
				request.destination = target;
				if (RenderPipeline.SupportsRenderRequest(camera, request)) RenderPipeline.SubmitRenderRequest(camera, request);
				else { camera.targetTexture = target; camera.Render(); camera.targetTexture = null; }
				RenderTexture previous = RenderTexture.active;
				RenderTexture.active = target;
				Texture2D texture = new Texture2D(IconWidth, IconHeight, TextureFormat.RGBA32, false);
				texture.ReadPixels(new Rect(0f, 0f, IconWidth, IconHeight), 0, 0);
				RenderTexture.active = previous;
				Color32[] pixels = texture.GetPixels32();
				long alpha = 0;
				for (int i = 0; i < pixels.Length; i += 7) alpha += pixels[i].a;
				if (alpha == 0) { UnityEngine.Object.Destroy(texture); return null; }   // nothing rendered: keep the photo, retry later
				texture.name = "Icon " + m.id;
				texture.filterMode = FilterMode.Bilinear;
				texture.wrapMode = TextureWrapMode.Clamp;
				texture.Apply(false, true);
				sprite = Sprite.Create(texture, new Rect(0f, 0f, IconWidth, IconHeight), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, Vector4.zero);
				sprite.name = "Icon " + m.id;
			}
			catch (Exception e)
			{
				Debug.LogWarning("[InfernoCars] icon render for " + m.id + " failed: " + e.Message);
				sprite = null;
			}
			finally
			{
				if (target != null) { target.Release(); UnityEngine.Object.Destroy(target); }
				if (cameraObject != null) UnityEngine.Object.Destroy(cameraObject);
				if (stage != null) UnityEngine.Object.Destroy(stage.gameObject);
				if (previewCar != null) previewCar.SetActive(carWasActive);
				root.SetActive(rootWasActive);
			}
			return sprite;
		}
	}
}
