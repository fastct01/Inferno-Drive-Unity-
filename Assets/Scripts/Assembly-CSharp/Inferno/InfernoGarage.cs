using System;
using System.Collections.Generic;
using UnityEngine;

// Port of Il2CppOutputProject/Source/il2cppOutput/InfernoGarage.inc — extra garage cars.
// /* [user request 2026-09-21: cars] */ The 25 paint editions it carried (24 recolours of the
// four shipped meshes plus the W222 AMG) are retired: the catalogue is now the ten models of
// InfernoCars (five meshes, five re-styled editions, four rarities, per-car paint and
// upgrades). This class keeps the hook points the generated code and the profile bridge call
// (MakeDefinition / Decorate / AppendCards / AppendLockedCaseVehicles / IsUnlocked /
// SetUnlocked / CaseName / CasePicture) and forwards them to InfernoCars, plus the string
// helpers and the "Data/Raw/..." picture loader the lobby code shares. See
// docs/lobby-implementation.md, "Extra garage cars".
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoGarage
	{
		// ---- strings ----------------------------------------------------------------
		// Case-insensitive equality with an ASCII literal; a trailing " (Instance)" on the
		// managed string (an instantiated material) is ignored.
		public static bool SameName(string s, string ascii)
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

		public static char Lower(int c)
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

		// ---- ownership (PlayerPrefs; the cloud profile only has four car columns) ---
		// C bridge for the profile code (InfernoGarage_IsUnlocked / InfernoGarage_SetUnlocked).
		// IsUnlocked: -1 = not one of ours, else 0/1. SetUnlocked: true = handled.
		/* [user request 2026-09-21: cars] */ // original: looked the id up in Editions
		public static int IsUnlocked(string id) { return InfernoCars.IsUnlocked(id); }
		public static bool SetUnlocked(string id, bool unlocked) { return InfernoCars.SetUnlocked(id, unlocked); }

		// ---- definitions --------------------------------------------------------------
		// CreateVehicleDefinition(id) asks the catalogue first (InfernoCars.MakeDefinition);
		// kept for callers of the old hook.
		public static VehicleDefinition MakeDefinition(ShadowRunPrototype game, string id) { return InfernoCars.MakeDefinition(game, id); }   /* [user request 2026-09-21: cars] */

		// ---- model -------------------------------------------------------------------
		public static void Decorate(ShadowRunPrototype game, VehicleDefinition definition, GameObject instance) { InfernoCars.Decorate(game, definition, instance); }   /* [user request 2026-09-21: cars] */

		// ---- garage cards ---------------------------------------------------------------
		// The editions used to be appended after the original four cards; the catalogue now
		// builds every card itself (InfernoCars.Cards), so this passes the cards through.
		public static GarageVehicleCardData[] AppendCards(ShadowRunPrototype game, GarageVehicleCardData[] cards)   /* [user request 2026-09-21: cars] */
		{
			return cards ?? new GarageVehicleCardData[0];
		}

		// ---- cases ------------------------------------------------------------------------
		// The locked non-cloud cars (the four cloud ids are listed by the caller itself).
		public static void AppendLockedCaseVehicles(ShadowRunPrototype game, List<string> list)   /* [user request 2026-09-21: cars] */
		{
			if (list == null) return;
			List<string> locked = InfernoCars.LockedIds();
			for (int i = 0; i < locked.Count; ++i)
				if (!InfernoCars.IsCloudId(locked[i]) && !list.Contains(locked[i])) list.Add(locked[i]);
		}

		public static string CaseName(string id) { return InfernoCars.CaseName(id); }        /* [user request 2026-09-21: cars] */
		public static string CasePicture(string id) { return InfernoCars.CasePicture(id); }  /* [user request 2026-09-21: cars] */

		// ---- pictures ---------------------------------------------------------------------
		// LoadResourceSprite / LoadTightMenuArtSprite ask here first (patched): a
		// "Data/Raw/..." resource is a PNG in the app bundle, decoded natively and
		// cached; anything else returns null and the game loads it from Resources.
		// "Data/Raw/InfernoCars/<id>.png" is a variant's runtime-rendered icon
		// (InfernoCars.Icon), or its base model's photo until that can be rendered.
		private const int Slots = 48;   // 25 editions + room for the style items
		private static readonly string[] paths = new string[Slots];
		private static readonly Sprite[] sprites = new Sprite[Slots];
		private static int count;

		public static Sprite Picture(ShadowRunPrototype game, string resource)
		{
			if (resource == null || !StartsWith(resource, "Data/Raw/")) return null;
			/* [user request 2026-09-21: cars] */
			string carId = InfernoCars.IdFromPicture(resource);
			if (carId != null)
			{
				Sprite icon = InfernoCars.Icon(game, carId);
				if (icon != null) return icon;
				return game != null ? game.LoadResourceSprite(InfernoCars.FallbackPicture(carId)) : null;
			}
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
