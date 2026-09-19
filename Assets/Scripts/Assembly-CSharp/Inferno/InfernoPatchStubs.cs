using UnityEngine;
using UnityEngine.UI;

// Post-source feature namespaces (InfernoLobby.inc, InfernoStyle.inc, InfernoHazards.inc,
// InfernoDriving.inc, InfernoSettings.inc, InfernoGarage.inc). The generated code calls
// these at the hook points listed by tools/apply_lobby_patches.py; the hooks are kept in
// the translated methods from the start, and the bodies are ported in stage 6.
// Until then every hook is a no-op returning the "feature absent" value.
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoLobby
	{
		public static bool HandleLockedMap(ShadowRunPrototype game) { return false; }
		public static void BeginRun(ShadowRunPrototype game) { }
		public static void Backdrop(ShadowRunPrototype game, Transform parent) { }
		public static void Build(ShadowRunPrototype game, Transform parent) { }
		public static void Gems(ShadowRunPrototype game, Transform parent) { }
		public static void Tabs(ShadowRunPrototype game, Transform parent) { }
		public static void PositionPages(ShadowRunPrototype game) { }
		public static void Layout(ShadowRunPrototype game) { }
		public static void Show(ShadowRunPrototype game) { }
		public static void RefreshGoalRewards(ShadowRunPrototype game) { }
		// Crystals paid for a claimed daily goal, by goal kind (0 distance, 1 runs, 2 boosts, 3 drift,
		// 4 cars passed, 5 cases opened). The original paid a single crystal.
		private static readonly int[] GoalRewards = { 15, 10, 10, 15, 10, 20 };
		public static int GoalReward(int kind) { return kind >= 0 && kind < 6 ? GoalRewards[kind] : 10; }
		public static void CaseVisibility(ShadowRunPrototype game, bool visible) { }
		public static void BuildGameOver(ShadowRunPrototype game, Transform parent) { }
		public static bool IsHeartResource(string name) { return false; }
		public static bool IsDiamondResource(string name) { return false; }
		public static Sprite HeartSprite() { return null; }
		public static Sprite GemSprite(ShadowRunPrototype game) { return null; }
	}

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

	private static partial class InfernoHazards
	{
		public static void Tick(ShadowRunPrototype game, float dt) { }
		public static void Reset(ShadowRunPrototype game) { }
		public static void Hide(ShadowRunPrototype game) { }
		public static void Dispose(ShadowRunPrototype game) { }
	}

	private static partial class InfernoDriving
	{
		public static void Tick(ShadowRunPrototype game, float dt) { }
		public static void Reset(ShadowRunPrototype game) { }
		public static void Hide(ShadowRunPrototype game) { }
		public static void Dispose(ShadowRunPrototype game) { }
		public static void StylePauseButton(ShadowRunPrototype game) { }
		public static int NextPaintStyle() { return 0; }
	}

	private static partial class InfernoSettings
	{
		public static void Tick(ShadowRunPrototype game) { }
		public static void Build(ShadowRunPrototype game) { }
		public static void Refresh(ShadowRunPrototype game) { }
		public static void Relayout(ShadowRunPrototype game) { }
		public static void StyleAccount(ShadowRunPrototype game, Image panel) { }
		public static void CenterHeader(RectTransform root) { }
		public static float ListenerVolume(ShadowRunPrototype game) { return game.gameSoundEnabled ? 1f : 0f; }
	}

	private static partial class InfernoGarage
	{
		public static Sprite Picture(ShadowRunPrototype game, string resource) { return null; }
		public static VehicleDefinition MakeDefinition(ShadowRunPrototype game, string vehicleId) { return null; }
		public static void Decorate(ShadowRunPrototype game, VehicleDefinition definition, GameObject instance) { }
		public static string CasePicture(string vehicleId) { return null; }
		public static string CaseName(string vehicleId) { return null; }
		public static void AppendLockedCaseVehicles(ShadowRunPrototype game, System.Collections.Generic.List<string> list) { }
		public static void AppendCards(ShadowRunPrototype game, System.Collections.Generic.List<GarageVehicleCardData> cards) { }
	}
}
