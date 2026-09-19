using UnityEngine;
using UnityEngine.UI;

// Post-source feature namespaces (InfernoLobby.inc, InfernoStyle.inc, InfernoHazards.inc,
// InfernoDriving.inc, InfernoSettings.inc, InfernoGarage.inc). The generated code calls
// these at the hook points listed by tools/apply_lobby_patches.py; the hooks are kept in
// the translated methods from the start, and the bodies are ported in stage 6.
// Until then every hook is a no-op returning the "feature absent" value.
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoStyle
	{
		public static void Frame(ShadowRunPrototype game) { }
		public static void Dispose(ShadowRunPrototype game) { }
		public static bool IsOpen(ShadowRunPrototype game) { return false; }
		public static void HideTrails(ShadowRunPrototype game) { }
		public static void ResetTrails(ShadowRunPrototype game) { }
		public static void TickTrails(ShadowRunPrototype game, float dt) { }
		public static void CloseSheet(ShadowRunPrototype game) { }
		public static void Build(ShadowRunPrototype game) { }
		public static void Dress(ShadowRunPrototype game, Object instance, VehicleDefinition definition, bool preview) { }
	}

	// extern "C" bridge of InfernoGarage.inc (InfernoGarage_IsUnlocked / InfernoGarage_SetUnlocked), used by
	// SupabaseGameManager. IsUnlocked: -1 = not an InfernoGarage vehicle, else 0/1. SetUnlocked: true = handled.
	public static int InfernoGarageIsUnlocked(string vehicleId) { return InfernoGarage.IsUnlocked(vehicleId); }
	public static bool InfernoGarageSetUnlocked(string vehicleId, bool unlocked) { return InfernoGarage.SetUnlocked(vehicleId, unlocked); }

	private static partial class InfernoGarage
	{
		public static int IsUnlocked(string vehicleId) { return -1; }
		public static bool SetUnlocked(string vehicleId, bool unlocked) { return false; }
		public static Sprite Picture(ShadowRunPrototype game, string resource) { return null; }
		public static VehicleDefinition MakeDefinition(ShadowRunPrototype game, string vehicleId) { return null; }
		public static void Decorate(ShadowRunPrototype game, VehicleDefinition definition, GameObject instance) { }
		public static string CasePicture(string vehicleId) { return null; }
		public static string CaseName(string vehicleId) { return null; }
		public static void AppendLockedCaseVehicles(ShadowRunPrototype game, System.Collections.Generic.List<string> list) { }
		public static GarageVehicleCardData[] AppendCards(ShadowRunPrototype game, GarageVehicleCardData[] cards) { return cards; }
	}
}
