using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// [user request 2026-09-21: overlaps] Play-mode layout audit: the game boots itself (AutoStart), the
// test walks every screen state (home with an unlocked and a locked map, garage with an owned and a
// locked card, the STYLE sheet and its tabs, the profile page with no account / the auth form /
// the signed-in actions, the gem shop, the case overlay, the run HUD, pause and game over) and asks
// ShadowRunPrototype.InfernoLayoutAudit for sibling overlaps and texts outside their parents; the
// report must be empty. Same harness as SmokeTests (reflection into the sealed partial class, no
// AudioListener, the scene is never reloaded).
public class LayoutAuditTests
{
	private static readonly List<string> unexpected = new List<string>();
	private static void OnLog(string condition, string stackTrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
		if (condition.Contains("UISprite.psd")) return;   // editor-only builtin lookup, expected (see SmokeTests)
		unexpected.Add(type + ": " + condition);
	}

	private static readonly string[] PrefKeys = { "InfernoDrive.SelectedMap", "InfernoDrive.MapUnlocked.3", "infernoDriveFirstRunTutorialV7" };
	private static readonly Dictionary<string, int?> prefSnapshot = new Dictionary<string, int?>();

	[SetUp]
	public void Watch()
	{
		unexpected.Clear(); LogAssert.ignoreFailingMessages = true; Application.logMessageReceived += OnLog;
		prefSnapshot.Clear();
		foreach (var k in PrefKeys) prefSnapshot[k] = PlayerPrefs.HasKey(k) ? PlayerPrefs.GetInt(k) : (int?)null;
	}

	[TearDown]
	public void Unwatch()
	{
		Application.logMessageReceived -= OnLog;
		LogAssert.ignoreFailingMessages = false;
		foreach (var kv in prefSnapshot) { if (kv.Value.HasValue) PlayerPrefs.SetInt(kv.Key, kv.Value.Value); else PlayerPrefs.DeleteKey(kv.Key); }
		PlayerPrefs.Save();
		if (unexpected.Count > 0) Assert.Fail(unexpected.Count + " unexpected error log(s):\n" + string.Join("\n", unexpected.GetRange(0, Math.Min(10, unexpected.Count))));
	}

	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
	private static readonly Type GameType = Type.GetType("ShadowRunPrototype, Assembly-CSharp");

	private static Component Game()
	{
		return (Component)UnityEngine.Object.FindFirstObjectByType(GameType);
	}

	private static bool rebooted;

	// Same boot as SmokeTests: the instance AutoStart built before the run is torn down once and
	// rebuilt the way AutoStart does; the first-launch auth screen (if enabled) is closed.
	private static IEnumerator Boot()
	{
		LogAssert.ignoreFailingMessages = true;
		yield return null;
		if (!rebooted && Game() != null)
		{
			foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
			{
				string n = root.name.ToLowerInvariant();
				if (n.Contains("test") || n.Contains("runner") || n.Contains("coverage")) continue;
				UnityEngine.Object.Destroy(root);
			}
			yield return null;
			yield return null;
		}
		rebooted = true;
		if (Game() == null) new GameObject("Shadow Run Prototype").AddComponent(GameType);
		yield return Seconds(2f);
		Type authType = Type.GetType("ShadowRun.Services.FirstLaunchAuthScreen, Assembly-CSharp");
		var auth = authType != null ? (Component)UnityEngine.Object.FindFirstObjectByType(authType) : null;
		if (auth != null)
		{
			authType.GetMethod("Close", Any).Invoke(auth, null);
			yield return Seconds(1f);
		}
		Component game = Game();
		if ((bool)Field(game, "running") || (bool)Field(game, "showingGameOver")) { Call(game, "ShowMainMenu"); yield return Seconds(1f); }
	}

	private static object Call(Component game, string method, params object[] args)
	{
		MethodInfo m = null;
		foreach (var candidate in GameType.GetMethods(Any))
			if (candidate.Name == method && candidate.GetParameters().Length == args.Length) { m = candidate; break; }
		Assert.IsNotNull(m, "method " + method);
		return m.Invoke(game, args);
	}

	private static object Field(Component game, string name)
	{
		FieldInfo f = GameType.GetField(name, Any);
		Assert.IsNotNull(f, "field " + name);
		return f.GetValue(game);
	}

	private static void SetField(Component game, string name, object value)
	{
		GameType.GetField(name, Any).SetValue(game, value);
	}

	private static object Nested(string type, string method, params object[] args)
	{
		Type nested = GameType.GetNestedType(type, Any);
		Assert.IsNotNull(nested, "nested type " + type);
		MethodInfo m = null;
		foreach (var candidate in nested.GetMethods(Any))
			if (candidate.Name == method && candidate.GetParameters().Length == args.Length) { m = candidate; break; }
		Assert.IsNotNull(m, type + "." + method);
		return m.Invoke(null, args);
	}

	// Realtime waits (like SmokeTests) with a frame floor, so a throttled editor loop still advances
	// the canvas rebuilds the audit measures.
	private static IEnumerator Seconds(float s)
	{
		float end = Time.realtimeSinceStartup + s;
		int frames = 0;
		while (Time.realtimeSinceStartup < end || frames < 3) { frames++; yield return null; }
	}

	private static UnityEngine.UI.Button FindButton(string objectName)
	{
		foreach (var b in UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			if (b.gameObject.name == objectName && b.gameObject.activeInHierarchy) return b;
		return null;
	}

	private static bool Click(string objectName)
	{
		var b = FindButton(objectName);
		if (b == null) { Debug.Log("[LayoutAudit] no active button " + objectName); return false; }
		b.onClick.Invoke();
		return true;
	}

	private static readonly List<string> findings = new List<string>();

	// One audit of everything on screen, filed under the state's name.
	private static void Audit(Component game, string state)
	{
		string report = (string)Nested("InfernoLayoutAudit", "ReportAll", game);
		if (string.IsNullOrEmpty(report)) { Debug.Log("[LayoutAudit] " + state + ": clean"); return; }
		string[] lines = report.Split('\n');
		Debug.Log("[LayoutAudit] " + state + ": " + lines.Length + " finding(s)\n" + report);
		for (int i = 0; i < lines.Length; i++) findings.Add(state + ": " + lines[i]);
	}

	[UnityTest]
	public IEnumerator NoOverlapsOnAnyScreen()
	{
		Assert.IsNotNull(GameType, "Assembly-CSharp.ShadowRunPrototype");
		yield return Boot();
		Component game = Game();
		Assert.IsNotNull(game, "ShadowRunPrototype instance");
		findings.Clear();
		const string tutorialKey = "infernoDriveFirstRunTutorialV7";
		PlayerPrefs.SetInt(tutorialKey, 1);

		// --- home: NEON NIGHT (free) and STORM CHASE (locked, price 250)
		int mapBefore = PlayerPrefs.GetInt("InfernoDrive.SelectedMap", 1);
		Call(game, "SelectMenuTab", 1, true);
		PlayerPrefs.SetInt("InfernoDrive.SelectedMap", 1); PlayerPrefs.Save();
		Nested("InfernoLobby", "RefreshMap", game);
		yield return Seconds(0.6f);
		Audit(game, "home map 1");
		PlayerPrefs.DeleteKey("InfernoDrive.MapUnlocked.3");
		PlayerPrefs.SetInt("InfernoDrive.SelectedMap", 3); PlayerPrefs.Save();
		Nested("InfernoLobby", "RefreshMap", game);
		yield return Seconds(0.4f);
		Audit(game, "home map 3 locked");
		PlayerPrefs.SetInt("InfernoDrive.SelectedMap", mapBefore); PlayerPrefs.Save();
		Nested("InfernoLobby", "RefreshMap", game);

		// --- garage: the owned starter, then a locked card (purchase row), then the STYLE sheet's tabs
		Assert.IsTrue(Click("Garage"), "Garage tab");
		yield return Seconds(1f);
		Call(game, "SelectGarageVehicleCard", 0);
		yield return Seconds(0.6f);
		Audit(game, "garage owned card");
		Call(game, "SelectGarageVehicleCard", 1);
		yield return Seconds(0.6f);
		Audit(game, "garage locked card");
		Call(game, "SelectGarageVehicleCard", 0);
		yield return Seconds(0.3f);
		if (Click("StyleButton"))
		{
			yield return Seconds(0.6f);
			Audit(game, "style sheet");
			foreach (string tab in new[] { "Tab Flags", "Tab Trails", "Tab Paint", "Tab Tires" })
			{
				if (!Click(tab)) continue;
				yield return Seconds(0.4f);
				Audit(game, "style sheet " + tab);
			}
			Click("Close");
			yield return Seconds(0.3f);
		}

		// --- profile: no account, the auth form, the signed-in actions card
		Assert.IsTrue(Click("Profile"), "Profile tab");
		yield return Seconds(1f);
		Audit(game, "profile");
		Call(game, "ShowProfileAuthForm", true);
		yield return Seconds(0.6f);
		Audit(game, "profile auth form");
		Call(game, "HideProfileAuthForm");
		yield return Seconds(0.3f);
		var accountRoot = (GameObject)Field(game, "profileAccountRoot");
		if (accountRoot != null)
		{
			accountRoot.SetActive(true);
			Nested("InfernoSettings", "Arrange", game);
			yield return Seconds(0.6f);
			Audit(game, "profile signed-in actions");
			accountRoot.SetActive(false);
			Call(game, "RefreshProfileUi");
			yield return Seconds(0.3f);
		}

		// --- overlays over the home page
		Assert.IsTrue(Click("Home"), "Home tab");
		yield return Seconds(0.6f);
		Call(game, "ShowGemShop");
		yield return Seconds(0.6f);
		Audit(game, "gem shop");
		Call(game, "HideGemShop");
		yield return Seconds(0.3f);
		Call(game, "ShowCaseOverlay");
		yield return Seconds(1.2f);
		Audit(game, "case overlay");
		Call(game, "HideCaseOverlay");
		yield return Seconds(0.4f);

		// --- a run: the HUD, the pause screen, the loss screen
		Call(game, "StartRun");
		yield return Seconds(2f);
		Assert.IsTrue((bool)Field(game, "running"), "running after StartRun");
		Audit(game, "hud");
		Call(game, "PauseRun");
		yield return Seconds(0.6f);
		Assert.IsTrue((bool)Field(game, "paused"), "paused");
		Audit(game, "pause");
		Call(game, "ResumeRun");
		yield return Seconds(0.6f);
		SetField(game, "lifeHalfUnits", 0);
		float wait = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - wait < 8f && !(bool)Field(game, "showingGameOver")) yield return null;
		Assert.IsTrue((bool)Field(game, "showingGameOver"), "game over after losing every life");
		yield return Seconds(0.6f);
		Audit(game, "game over");
		Call(game, "ShowMainMenu");
		yield return Seconds(0.6f);
		Audit(game, "home again");

		if (findings.Count > 0)
			Assert.Fail(findings.Count + " layout finding(s):\n" + string.Join("\n", findings.GetRange(0, Math.Min(40, findings.Count))));
	}
}
