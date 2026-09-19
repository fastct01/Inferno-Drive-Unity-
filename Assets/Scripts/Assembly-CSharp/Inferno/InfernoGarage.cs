using System;
using System.Collections.Generic;
using UnityEngine;

// Port of Il2CppOutputProject/Source/il2cppOutput/InfernoGarage.inc — extra garage cars,
// built from the detailed models this export already ships (there is no new art in an
// IL2CPP export): the Mercedes W222 AMG that only drove as traffic becomes a player car,
// and every model gets paint editions (25 in all: the first five of 2026-09-17 and twenty
// more of 2026-09-18, each with its own paint and a second accent colour on rims /
// calipers / piping). Reuses InfernoLobby's helpers; wired into the generated code at the
// hook points of tools/apply_lobby_patches.py. See docs/lobby-implementation.md, "Extra
// garage cars".
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoGarage
	{
		private enum Base { Sport, Cayman, Flying, Infal }

		private sealed class Edition
		{
			public readonly string id;              // vehicle id: PlayerPrefs InfernoDrive.Vehicle.<id>.Unlocked, shadowRunSelectedVehicle
			public readonly Base base_;             // whose definition and prefab to start from
			public readonly string model;           // a different prefab (Resources path) or null for the base's own
			public readonly string name, subtitle, rarity;
			public readonly string icon;            // garage card + case prize picture (Data/Raw PNG, see Picture())
			public readonly float[] paint;          // _BaseColor of the paint material(s) below
			public readonly string[] paintMaterials;
			public readonly string accentMaterial;  // second colour (rims, calipers, piping...) or null
			public readonly float[] accentPaint;
			public readonly uint color, accent;     // card tints
			public readonly float power, speed, handling;
			public readonly string powerValue, speedValue, handlingValue;
			public readonly float accel, steer, grip, drift;   // VehicleDefinition.Arcade* multipliers

			public Edition(string id, Base base_, string model, string name, string subtitle, string rarity, string icon,
				float[] paint, string[] paintMaterials, string accentMaterial, float[] accentPaint, uint color, uint accent,
				float power, float speed, float handling, string powerValue, string speedValue, string handlingValue,
				float accel, float steer, float grip, float drift)
			{
				this.id = id; this.base_ = base_; this.model = model; this.name = name; this.subtitle = subtitle; this.rarity = rarity;
				this.icon = icon; this.paint = paint; this.paintMaterials = paintMaterials; this.accentMaterial = accentMaterial;
				this.accentPaint = accentPaint; this.color = color; this.accent = accent; this.power = power; this.speed = speed;
				this.handling = handling; this.powerValue = powerValue; this.speedValue = speedValue; this.handlingValue = handlingValue;
				this.accel = accel; this.steer = steer; this.grip = grip; this.drift = drift;
			}
		}

		private const string Icons = "Data/Raw/InfernoLobby/cars/";

		private static readonly Edition[] Editions =
		{
			new Edition("amg_w222", Base.Sport, "uploads_files_4881612_uploads_files_3750868_MB.W222.AMG",
				"AMG SOVEREIGN", "Executive Saloon / Class A", "EPIC", "Data/Raw/InfernoLobby/cars/amg_w222.png",
				new[] {0.60f, 0.62f, 0.68f}, new[] {"W222 - Car Color", null}, null, new[] {0.00f, 0.00f, 0.00f}, 0xB8C0D0, 0xE6ECF5,
				0.80f, 0.84f, 0.70f, "800 HP", "290 KM/H", "70%", 1.00f, 0.92f, 1.04f, 0.90f),
			new Edition("car3_gt4rs", Base.Cayman, null,
				"CAYMAN GT4 RS", "Track GT / Class S+", "LEGENDARY", "Data/Raw/InfernoLobby/cars/car3_gt4rs.png",
				new[] {0.80f, 0.04f, 0.05f}, new[] {"Paint", null}, null, new[] {0.00f, 0.00f, 0.00f}, 0xE8283C, 0xFF8A96,
				0.90f, 0.91f, 0.96f, "900 HP", "318 KM/H", "96%", 1.08f, 1.04f, 1.06f, 0.96f),
			new Edition("infal_aurum", Base.Infal, null,
				"P900 AURUM", "Prototype / Class S+", "MYTHIC", "Data/Raw/InfernoLobby/cars/infal_aurum.png",
				new[] {0.85f, 0.62f, 0.18f}, new[] {"dtP900_car_DeTomaso_P900_carpaint1", null}, null, new[] {0.00f, 0.00f, 0.00f}, 0xE0B040, 0xFFE08A,
				0.97f, 0.96f, 0.90f, "970 HP", "334 KM/H", "90%", 1.16f, 0.95f, 0.98f, 1.16f),
			new Edition("sportcar_stealth", Base.Sport, null,
				"PHANTOM STEALTH", "Hypersport / Class S", "LEGENDARY", "Data/Raw/InfernoLobby/cars/sportcar_stealth.png",
				new[] {0.24f, 0.26f, 0.31f}, new[] {"main", null}, null, new[] {0.00f, 0.00f, 0.00f}, 0x8A94A8, 0xC4CCDC,
				0.98f, 0.92f, 0.86f, "980 HP", "318 KM/H", "86%", 1.10f, 1.00f, 1.00f, 1.05f),
			new Edition("flying_neon", Base.Flying, null,
				"AERO WRAITH NEON", "Hover GT / Class A", "EPIC", "Data/Raw/InfernoLobby/cars/flying_neon.png",
				new[] {0.45f, 0.15f, 0.85f}, new[] {"car base", null}, null, new[] {0.00f, 0.00f, 0.00f}, 0x9433FF, 0xD199F5,
				0.76f, 0.85f, 0.74f, "760 HP", "292 KM/H", "74%", 1.10f, 1.08f, 1.10f, 0.80f),
			new Edition("sportcar_inferno", Base.Sport, null,
				"PHANTOM INFERNO", "Hypersport / Class S", "EPIC", "Data/Raw/InfernoLobby/cars/sportcar_inferno.png",
				new[] {0.95f, 0.25f, 0.05f}, new[] {"main", null}, "diski2", new[] {0.06f, 0.06f, 0.07f}, 0xF04A1A, 0xFFB08A,
				0.94f, 0.90f, 0.84f, "940 HP", "312 KM/H", "84%", 1.08f, 0.98f, 0.98f, 1.02f),
			new Edition("sportcar_arctic", Base.Sport, null,
				"PHANTOM ARCTIC", "Hypersport / Class A", "RARE", "Data/Raw/InfernoLobby/cars/sportcar_arctic.png",
				new[] {0.92f, 0.94f, 0.98f}, new[] {"main", null}, "diski2", new[] {0.30f, 0.75f, 0.95f}, 0xD8E8F5, 0xFFFFFF,
				0.86f, 0.88f, 0.90f, "860 HP", "306 KM/H", "90%", 1.02f, 1.04f, 1.06f, 0.92f),
			new Edition("sportcar_venom", Base.Sport, null,
				"PHANTOM VENOM", "Hypersport / Class S", "LEGENDARY", "Data/Raw/InfernoLobby/cars/sportcar_venom.png",
				new[] {0.35f, 0.90f, 0.15f}, new[] {"main", null}, "diski2", new[] {0.06f, 0.06f, 0.07f}, 0x5FE62A, 0xB8FF8A,
				0.96f, 0.93f, 0.86f, "960 HP", "320 KM/H", "86%", 1.12f, 1.00f, 1.00f, 1.06f),
			new Edition("sportcar_royal", Base.Sport, null,
				"PHANTOM ROYAL", "Hypersport / Class S+", "MYTHIC", "Data/Raw/InfernoLobby/cars/sportcar_royal.png",
				new[] {0.35f, 0.08f, 0.60f}, new[] {"main", null}, "diski2", new[] {0.85f, 0.65f, 0.20f}, 0x6E1FBF, 0xC59AFF,
				0.99f, 0.95f, 0.90f, "990 HP", "326 KM/H", "90%", 1.14f, 1.02f, 1.04f, 1.08f),
			new Edition("car3_glacier", Base.Cayman, null,
				"CAYMAN GLACIER", "Track GT / Class A", "RARE", "Data/Raw/InfernoLobby/cars/car3_glacier.png",
				new[] {0.55f, 0.80f, 0.95f}, new[] {"Paint", null}, "BrakeCaliper", new[] {0.10f, 0.35f, 0.90f}, 0x8CCCF2, 0xD6F0FF,
				0.84f, 0.86f, 0.94f, "840 HP", "302 KM/H", "94%", 1.02f, 1.06f, 1.06f, 0.90f),
			new Edition("car3_noir", Base.Cayman, null,
				"CAYMAN NOIR", "Track GT / Class S", "EPIC", "Data/Raw/InfernoLobby/cars/car3_noir.png",
				new[] {0.03f, 0.03f, 0.04f}, new[] {"Paint", null}, "BrakeCaliper", new[] {0.85f, 0.05f, 0.05f}, 0x2A2A30, 0x8A8A96,
				0.90f, 0.90f, 0.95f, "900 HP", "314 KM/H", "95%", 1.06f, 1.04f, 1.06f, 0.98f),
			new Edition("car3_sunset", Base.Cayman, null,
				"CAYMAN SUNSET", "Track GT / Class S", "LEGENDARY", "Data/Raw/InfernoLobby/cars/car3_sunset.png",
				new[] {0.95f, 0.20f, 0.55f}, new[] {"Paint", null}, "BrakeCaliper", new[] {0.98f, 0.85f, 0.20f}, 0xF2338C, 0xFFA6D2,
				0.92f, 0.92f, 0.96f, "920 HP", "320 KM/H", "96%", 1.08f, 1.06f, 1.06f, 1.00f),
			new Edition("car3_racing", Base.Cayman, null,
				"CAYMAN RACING GREEN", "Track GT / Class A", "EPIC", "Data/Raw/InfernoLobby/cars/car3_racing.png",
				new[] {0.02f, 0.30f, 0.14f}, new[] {"Paint", null}, "BrakeCaliper", new[] {0.85f, 0.65f, 0.20f}, 0x0F5A2E, 0x7ACF9A,
				0.88f, 0.89f, 0.95f, "880 HP", "310 KM/H", "95%", 1.04f, 1.06f, 1.08f, 0.94f),
			new Edition("infal_crimson", Base.Infal, null,
				"P900 CRIMSON", "Prototype / Class S", "EPIC", "Data/Raw/InfernoLobby/cars/infal_crimson.png",
				new[] {0.75f, 0.02f, 0.08f}, new[] {"dtP900_car_DeTomaso_P900_carpaint1", null}, "Color", new[] {0.10f, 0.10f, 0.10f}, 0xC00A18, 0xFF7A86,
				0.94f, 0.94f, 0.88f, "940 HP", "326 KM/H", "88%", 1.12f, 0.95f, 0.98f, 1.12f),
			new Edition("infal_cobalt", Base.Infal, null,
				"P900 COBALT", "Prototype / Class S+", "LEGENDARY", "Data/Raw/InfernoLobby/cars/infal_cobalt.png",
				new[] {0.06f, 0.20f, 0.75f}, new[] {"dtP900_car_DeTomaso_P900_carpaint1", null}, "Color", new[] {0.80f, 0.82f, 0.86f}, 0x1A3CC0, 0x8AA6FF,
				0.96f, 0.96f, 0.89f, "960 HP", "332 KM/H", "89%", 1.14f, 0.96f, 0.98f, 1.14f),
			new Edition("infal_onyx", Base.Infal, null,
				"P900 ONYX", "Prototype / Class S+", "MYTHIC", "Data/Raw/InfernoLobby/cars/infal_onyx.png",
				new[] {0.05f, 0.05f, 0.06f}, new[] {"dtP900_car_DeTomaso_P900_carpaint1", null}, "Color", new[] {0.85f, 0.65f, 0.20f}, 0x26262C, 0xD8B860,
				0.99f, 0.98f, 0.90f, "990 HP", "340 KM/H", "90%", 1.18f, 0.96f, 1.00f, 1.18f),
			new Edition("infal_titan", Base.Infal, null,
				"P900 TITANIUM", "Prototype / Class S", "EPIC", "Data/Raw/InfernoLobby/cars/infal_titan.png",
				new[] {0.55f, 0.58f, 0.62f}, new[] {"dtP900_car_DeTomaso_P900_carpaint1", null}, "Color", new[] {0.12f, 0.12f, 0.14f}, 0x8C949E, 0xD0D6DC,
				0.93f, 0.94f, 0.88f, "930 HP", "324 KM/H", "88%", 1.12f, 0.95f, 0.98f, 1.10f),
			new Edition("flying_cyan", Base.Flying, null,
				"AERO WRAITH CYAN", "Hover GT / Class A", "EPIC", "Data/Raw/InfernoLobby/cars/flying_cyan.png",
				new[] {0.10f, 0.80f, 0.90f}, new[] {"car base", null}, "gold piping", new[] {0.90f, 0.92f, 0.95f}, 0x1ACCE6, 0x9AF0FF,
				0.78f, 0.86f, 0.76f, "780 HP", "296 KM/H", "76%", 1.10f, 1.08f, 1.10f, 0.82f),
			new Edition("flying_ember", Base.Flying, null,
				"AERO WRAITH EMBER", "Hover GT / Class S", "LEGENDARY", "Data/Raw/InfernoLobby/cars/flying_ember.png",
				new[] {0.90f, 0.35f, 0.05f}, new[] {"car base", null}, "gold piping", new[] {0.15f, 0.15f, 0.15f}, 0xE6590D, 0xFFB27A,
				0.82f, 0.90f, 0.78f, "820 HP", "304 KM/H", "78%", 1.14f, 1.08f, 1.10f, 0.86f),
			new Edition("flying_ghost", Base.Flying, null,
				"AERO WRAITH GHOST", "Hover GT / Class A", "RARE", "Data/Raw/InfernoLobby/cars/flying_ghost.png",
				new[] {0.85f, 0.88f, 0.95f}, new[] {"car base", null}, "gold piping", new[] {0.70f, 0.72f, 0.78f}, 0xD9E0F2, 0xFFFFFF,
				0.74f, 0.84f, 0.76f, "740 HP", "290 KM/H", "76%", 1.08f, 1.08f, 1.10f, 0.78f),
			new Edition("flying_void", Base.Flying, null,
				"AERO WRAITH VOID", "Hover GT / Class S+", "MYTHIC", "Data/Raw/InfernoLobby/cars/flying_void.png",
				new[] {0.06f, 0.02f, 0.12f}, new[] {"car base", null}, "gold piping", new[] {0.58f, 0.20f, 1.00f}, 0x2A0F4A, 0x9433FF,
				0.86f, 0.92f, 0.80f, "860 HP", "310 KM/H", "80%", 1.16f, 1.10f, 1.12f, 0.88f),
			new Edition("amg_obsidian", Base.Sport, "uploads_files_4881612_uploads_files_3750868_MB.W222.AMG",
				"AMG OBSIDIAN", "Executive Saloon / Class S", "LEGENDARY", "Data/Raw/InfernoLobby/cars/amg_obsidian.png",
				new[] {0.04f, 0.04f, 0.05f}, new[] {"W222 - Car Color", null}, "W222 - Rims", new[] {0.08f, 0.08f, 0.09f}, 0x2A2A30, 0x9A9AA8,
				0.86f, 0.88f, 0.72f, "860 HP", "300 KM/H", "72%", 1.04f, 0.92f, 1.04f, 0.94f),
			new Edition("amg_bordeaux", Base.Sport, "uploads_files_4881612_uploads_files_3750868_MB.W222.AMG",
				"AMG BORDEAUX", "Executive Saloon / Class A", "EPIC", "Data/Raw/InfernoLobby/cars/amg_bordeaux.png",
				new[] {0.40f, 0.05f, 0.12f}, new[] {"W222 - Car Color", null}, "W222 - Rims", new[] {0.63f, 0.63f, 0.66f}, 0x8A1030, 0xE07A96,
				0.82f, 0.85f, 0.70f, "820 HP", "292 KM/H", "70%", 1.02f, 0.92f, 1.04f, 0.92f),
			new Edition("amg_emerald", Base.Sport, "uploads_files_4881612_uploads_files_3750868_MB.W222.AMG",
				"AMG EMERALD", "Executive Saloon / Class S", "LEGENDARY", "Data/Raw/InfernoLobby/cars/amg_emerald.png",
				new[] {0.05f, 0.35f, 0.22f}, new[] {"W222 - Car Color", null}, "W222 - Rims", new[] {0.85f, 0.65f, 0.20f}, 0x0F6A42, 0x7AD9AA,
				0.84f, 0.87f, 0.72f, "840 HP", "296 KM/H", "72%", 1.04f, 0.92f, 1.04f, 0.94f),
			new Edition("amg_pearl", Base.Sport, "uploads_files_4881612_uploads_files_3750868_MB.W222.AMG",
				"AMG PEARL", "Executive Saloon / Class A", "RARE", "Data/Raw/InfernoLobby/cars/amg_pearl.png",
				new[] {0.93f, 0.92f, 0.90f}, new[] {"W222 - Car Color", null}, "W222 - Rims", new[] {0.20f, 0.20f, 0.22f}, 0xEDE9E2, 0xFFFFFF,
				0.80f, 0.84f, 0.70f, "800 HP", "290 KM/H", "70%", 1.00f, 0.92f, 1.04f, 0.90f),
		};

		private static int EditionCount => Editions.Length;

		// ---- strings ----------------------------------------------------------------
		// Case-insensitive equality with an ASCII literal; a trailing " (Instance)" on the
		// managed string (an instantiated material) is ignored.
		private static bool SameName(string s, string ascii)
		{
			if (s == null || ascii == null) return false;
			int n = s.Length;
			const string suffix = " (Instance)";
			int sl = suffix.Length;
			if (n > sl)
			{
				bool tail = true;
				for (int i = 0; i < sl && tail; ++i) tail = s[n - sl + i] == suffix[i];
				if (tail) n -= sl;
			}
			int m = ascii.Length;
			if (n != m) return false;
			for (int i = 0; i < n; ++i)
				if (Lower(s[i]) != Lower(ascii[i])) return false;
			return true;
		}

		private static char Lower(int c)
		{
			return (c >= 'A' && c <= 'Z') ? (char)(c + 32) : (char)c;
		}

		private static bool StartsWith(string s, string ascii)
		{
			int m = ascii.Length;
			if ((s != null ? s.Length : 0) < m) return false;
			for (int i = 0; i < m; ++i) if (s[i] != ascii[i]) return false;
			return true;
		}

		// Utf8(): the C++ narrowed the managed string to ASCII ('?' for non-ASCII) for its
		// fixed char buffers; the C# cache keys on the same narrowed form.
		private static string Utf8(string s, int cap)
		{
			int n = s != null ? s.Length : 0;
			var sb = new System.Text.StringBuilder();
			for (int i = 0; i < n && sb.Length < cap - 1; ++i) { char c = s[i]; sb.Append(c < 128 ? c : '?'); }
			return sb.ToString();
		}

		private static Edition Find(string id)
		{
			for (int i = 0; i < EditionCount; ++i) if (SameName(id, Editions[i].id)) return Editions[i];
			return null;
		}

		private static string[] Strings(params string[] items)
		{
			string[] array = new string[items.Length];
			for (int i = 0; i < items.Length; i++) array[i] = items[i];
			return array;
		}

		// ---- ownership (PlayerPrefs; the cloud profile only has four car columns) ---
		private static string PrefKey(Edition e)
		{
			return "InfernoDrive.Vehicle." + e.id + ".Unlocked";
		}

		private static bool Unlocked(Edition e)
		{
			return PlayerPrefs.GetInt(PrefKey(e), 0) != 0;
		}

		// C bridge for the profile code (InfernoGarage_IsUnlocked / InfernoGarage_SetUnlocked).
		// IsUnlocked: -1 = not one of ours, else 0/1. SetUnlocked: true = handled.
		public static int IsUnlocked(string id)
		{
			Edition e = Find(id);
			return e != null ? (Unlocked(e) ? 1 : 0) : -1;
		}

		public static bool SetUnlocked(string id, bool unlocked)
		{
			Edition e = Find(id);
			if (e == null) return false;
			PlayerPrefs.SetInt(PrefKey(e), unlocked ? 1 : 0);
			PlayerPrefs.Save();
			return true;
		}

		// ---- definitions --------------------------------------------------------------
		// CreateVehicleDefinition(id) asks here first (patched). An edition starts from
		// its base car's definition, then takes its own id, stats and, for the AMG, the
		// traffic prefab with that model's wheel / brake-disc mesh names.
		public static VehicleDefinition MakeDefinition(ShadowRunPrototype game, string id)
		{
			Edition e = Find(id);
			if (e == null) return null;
			VehicleDefinition d = null;
			switch (e.base_)
			{
				case Base.Sport: d = game.CreateDefaultSportCarDefinition(); break;
				case Base.Cayman: d = game.CreateCar3Definition(); break;
				case Base.Flying: d = game.CreateFlyingCarDefinition(); break;
				case Base.Infal: d = game.CreateInfalModelDefinition(); break;
			}
			if (d == null) return null;
			d.Id = e.id;
			d.PlayerModelName = "Player " + e.name + " Model";
			d.GarageModelName = "Garage Preview " + e.name;
			if (e.model != null)
			{
				d.ModelResource = e.model;
				d.WheelMeshNames = Strings("W222.Wheel.Ft.L", "W222.Wheel.Ft.R", "W222.Wheel.Bk.L", "W222.Wheel.Bk.R");
				d.BrakeDiscNames = Strings("W222.Brake.Ft.L", "W222.Brake.Ft.R", "W222.Brake.Bk.L", "W222.Brake.Bk.R");
				d.BrakeLightNames = Strings();           // the lamps are baked into the body's photo textures
				d.BrakeLightMaterialNames = Strings();
				d.BoundsRendererNameKeywords = Strings("W222");   // ignore the Mball helper when fitting
				d.RoadYOffset = 0.05f;
				d.HideOversizedPlaneRenderers = true;
				d.GarageTargetFootprint = 2.6f;
			}
			d.ArcadeAcceleration = e.accel;
			d.ArcadeSteering = e.steer;
			d.ArcadeGrip = e.grip;
			d.ArcadeDrift = e.drift;
			return d;
		}

		// ---- model -------------------------------------------------------------------
		// Called at the end of CreateConfiguredVehicleModel (player car and garage
		// preview alike): repaints the edition's paint material(s) with a per-slot
		// MaterialPropertyBlock (so glass, lights and chrome on the same renderer keep
		// their look, and nothing shared with the base car changes) and hides the
		// traffic prefab's metaball helper.
		public static void Decorate(ShadowRunPrototype game, VehicleDefinition definition, GameObject instance)
		{
			if (definition == null || instance == null) return;
			Edition e = Find(definition.Id);
			if (e == null) return;
			Transform root = instance.transform;
			if (e.model != null)
			{
				Transform ball = root.Find("Mball");
				if (ball != null) ball.gameObject.SetActive(false);
			}
			Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
			if (renderers == null) return;
			Color paint = new Color(e.paint[0], e.paint[1], e.paint[2], 1f);
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
					string name = material.name;
					bool hit = false;
					for (int k = 0; k < 2 && !hit; ++k) hit = e.paintMaterials[k] != null && SameName(name, e.paintMaterials[k]);
					bool accent = !hit && e.accentMaterial != null && SameName(name, e.accentMaterial);
					if (!hit && !accent) continue;
					Color colour = accent ? new Color(e.accentPaint[0], e.accentPaint[1], e.accentPaint[2], 1f) : paint;
					MaterialPropertyBlock block = new MaterialPropertyBlock();
					block.SetColor("_BaseColor", colour);
					block.SetColor("_Color", colour);
					renderer.SetPropertyBlock(block, j);
				}
			}
		}

		// ---- garage cards ---------------------------------------------------------------
		// All editions after the original four (the carousel sizes itself from the array).
		public static GarageVehicleCardData[] AppendCards(ShadowRunPrototype game, GarageVehicleCardData[] cards)
		{
			int n = cards != null ? cards.Length : 0;
			GarageVehicleCardData[] output = new GarageVehicleCardData[n + EditionCount];
			for (int i = 0; i < n; ++i) output[i] = cards[i];
			for (int k = 0; k < EditionCount; ++k)
			{
				Edition e = Editions[k];
				GarageVehicleCardData card = new GarageVehicleCardData();
				card.VehicleId = e.id;
				card.Name = e.name;
				card.Subtitle = e.subtitle;
				card.Rarity = e.rarity;
				card.IconResource = e.icon;
				card.Color = InfernoLobby.Hex(e.color);
				card.Accent = InfernoLobby.Hex(e.accent);
				card.Power = e.power; card.Speed = e.speed; card.Handling = e.handling;
				card.PowerValue = e.powerValue;
				card.SpeedValue = e.speedValue;
				card.HandlingValue = e.handlingValue;
				card.VisualLocked = !Unlocked(e);
				output[n + k] = card;
			}
			return output;
		}

		// ---- cases ------------------------------------------------------------------------
		public static void AppendLockedCaseVehicles(ShadowRunPrototype game, List<string> list)
		{
			if (list == null) return;
			for (int k = 0; k < EditionCount; ++k)
				if (!Unlocked(Editions[k])) list.Add(Editions[k].id);
		}

		public static string CaseName(string id) { Edition e = Find(id); return e != null ? e.name : null; }
		public static string CasePicture(string id) { Edition e = Find(id); return e != null ? e.icon : null; }

		// ---- pictures ---------------------------------------------------------------------
		// LoadResourceSprite / LoadTightMenuArtSprite ask here first (patched): a
		// "Data/Raw/..." resource is a PNG in the app bundle, decoded natively and
		// cached; anything else returns null and the game loads it from Resources.
		private const int Slots = 48;   // 25 editions + room for the style items
		private static readonly string[] paths = new string[Slots];
		private static readonly Sprite[] sprites = new Sprite[Slots];
		private static int count;

		public static Sprite Picture(ShadowRunPrototype game, string resource)
		{
			if (resource == null || !StartsWith(resource, "Data/Raw/")) return null;
			string path = Utf8(resource, 160);
			for (int i = 0; i < count; ++i)
				if (paths[i] == path) return sprites[i];
			if (count >= Slots) return null;
			int slot = count++;
			paths[slot] = path;
			sprites[slot] = null;
			Texture2D texture = InfernoLobby.LoadTexture(path, true);
			if (texture != null)
			{
				Rect full = new Rect(0f, 0f, InfernoLobby.s_lastLoaded[0], InfernoLobby.s_lastLoaded[1]);
				sprites[slot] = Sprite.Create(texture, full, new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect, new Vector4(0f, 0f, 0f, 0f));
			}
			return sprites[slot];
		}
	}
}
