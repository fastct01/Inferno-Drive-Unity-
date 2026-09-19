using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Play-mode smoke pass: the game boots itself (AutoStart), builds the lobby, walks the three
// dock pages, starts a run, survives several seconds of simulation and reaches game over —
// with no error logs (the test framework fails on unexpected Debug.LogError / exceptions).
// Every stop writes a screenshot to $INFERNO_SHOTS (default /tmp/inferno-shots) for the
// side-by-side parity checklist (docs/remake-verified.txt).
public class SmokeTests
{
	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
	private static readonly Type GameType = Type.GetType("ShadowRunPrototype, Assembly-CSharp");
	private static readonly string ShotDir = Environment.GetEnvironmentVariable("INFERNO_SHOTS") ?? "/tmp/inferno-shots";

	private static Component Game()
	{
		return (Component)UnityEngine.Object.FindFirstObjectByType(GameType);
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

	// Batch mode has no window (WaitForEndOfFrame never returns, ScreenCapture is empty), so a stop is
	// captured by rendering the main camera into a portrait render texture with the overlay canvas
	// temporarily switched to ScreenSpaceCamera, then restored.
	private static IEnumerator Shot(string name)
	{
		yield return null;
		{
			Directory.CreateDirectory(ShotDir);
			Camera cam = Camera.main;
			if (cam == null) { Component g = Game(); if (g != null) cam = (Camera)Field(g, "cam"); }
			if (cam == null) { Debug.Log("[SmokeTests] no main camera for " + name); yield break; }
			var canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
			var restore = new System.Collections.Generic.List<(Canvas, RenderMode, Camera, float)>();
			foreach (var c in canvases)
			{
				if (c.renderMode != RenderMode.ScreenSpaceOverlay || c.transform.parent != null) continue;
				restore.Add((c, c.renderMode, c.worldCamera, c.planeDistance));
				c.renderMode = RenderMode.ScreenSpaceCamera;
				c.worldCamera = cam;
				c.planeDistance = 0.5f;
			}
			var rt = new RenderTexture(1080, 1920, 24, RenderTextureFormat.ARGB32);
			var previous = cam.targetTexture;
			cam.targetTexture = rt;
			cam.Render();
			cam.targetTexture = previous;
			RenderTexture.active = rt;
			var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
			tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
			tex.Apply();
			RenderTexture.active = null;
			foreach (var (c, mode, wc, pd) in restore) { c.renderMode = mode; c.worldCamera = wc; c.planeDistance = pd; }
			File.WriteAllBytes(Path.Combine(ShotDir, name + ".png"), tex.EncodeToPNG());
			Debug.Log("[SmokeTests] screenshot " + name + " " + tex.width + "x" + tex.height + " screen " + Screen.width + "x" + Screen.height);
			UnityEngine.Object.Destroy(tex);
			rt.Release();
			UnityEngine.Object.Destroy(rt);
		}
	}

	private static IEnumerator Seconds(float s)
	{
		float end = Time.realtimeSinceStartup + s;
		while (Time.realtimeSinceStartup < end) yield return null;
	}

#if UNITY_EDITOR
	// Portrait 1080x1920 Game view so the procedural UI lays out as on the phone (only with an editor
	// window; in -batchmode Screen stays 640x480 and only the render-texture captures are portrait).
	private static void SetPortraitGameView()
	{
		try
		{
			var editor = typeof(UnityEditor.Editor).Assembly;
			Type gameViewType = editor.GetType("UnityEditor.GameView");
			Type sizesType = editor.GetType("UnityEditor.GameViewSizes");
			Type sizeType = editor.GetType("UnityEditor.GameViewSize");
			Type sizeKind = editor.GetType("UnityEditor.GameViewSizeType");
			Type groupKind = editor.GetType("UnityEditor.GameViewSizeGroupType");
			object instance = typeof(UnityEditor.ScriptableSingleton<>).MakeGenericType(sizesType).GetProperty("instance").GetValue(null);
			object groupType = sizesType.GetProperty("currentGroupType").GetValue(instance);
			object group = sizesType.GetMethod("GetGroup").Invoke(instance, new[] { groupType });
			Type groupClass = group.GetType();
			int total = (int)groupClass.GetMethod("GetTotalCount").Invoke(group, null);
			int index = -1;
			for (int i = 0; i < total; i++)
			{
				object size = groupClass.GetMethod("GetGameViewSize").Invoke(group, new object[] { i });
				if ((int)sizeType.GetProperty("width").GetValue(size) == 1080 && (int)sizeType.GetProperty("height").GetValue(size) == 1920) { index = i; break; }
			}
			if (index < 0)
			{
				object size = Activator.CreateInstance(sizeType, Enum.Parse(sizeKind, "FixedResolution"), 1080, 1920, "Inferno Portrait");
				groupClass.GetMethod("AddCustomSize").Invoke(group, new[] { size });
				index = total;
			}
			var window = UnityEditor.EditorWindow.GetWindow(gameViewType);
			gameViewType.GetMethod("SizeSelectionCallback", Any).Invoke(window, new object[] { index, null });
			Debug.Log("[SmokeTests] game view size index " + index + " -> " + Screen.width + "x" + Screen.height);
		}
		catch (Exception e)
		{
			Debug.Log("[SmokeTests] game view size not set: " + e.Message);
		}
	}
#endif

	[UnityTest]
	public IEnumerator BootsLobbyPagesRunAndGameOver()
	{
		Assert.IsNotNull(GameType, "Assembly-CSharp.ShadowRunPrototype");
#if UNITY_EDITOR
		SetPortraitGameView();
		yield return null;
#endif
		// AutoStart (RuntimeInitializeOnLoadMethod AfterSceneLoad) created the game when play mode began.
		yield return null;
		if (Game() == null) new GameObject("Shadow Run Prototype").AddComponent(GameType);
		yield return Seconds(2f);
		Component game = Game();
		Assert.IsNotNull(game, "ShadowRunPrototype instance");
		// First launch: the authorization screen sits over the lobby. Capture it, then close it the
		// way a completed sign-in would (its private Close) instead of creating a guest account on
		// the real backend.
		Type authType = Type.GetType("ShadowRun.Services.FirstLaunchAuthScreen, Assembly-CSharp");
		var auth = (Component)UnityEngine.Object.FindFirstObjectByType(authType);
		if (auth != null)
		{
			yield return Shot("00-first-launch-auth");
			authType.GetMethod("Close", Any).Invoke(auth, null);
			yield return Seconds(1f);
		}
		Assert.IsNotNull(Field(game, "menuSceneRoot"), "menuSceneRoot built by MakeUI");
		Assert.IsNotNull(Field(game, "hudRoot"), "hudRoot built by MakeUI");
		Assert.IsFalse((bool)Field(game, "running"), "menu first");
		yield return Shot("01-lobby-drive");

		Call(game, "SelectMenuTab", 0, true);
		yield return Seconds(1f);
		yield return Shot("02-garage");
		Call(game, "SelectMenuTab", 2, true);
		yield return Seconds(1f);
		yield return Shot("03-profile");
		Call(game, "SelectMenuTab", 1, true);
		yield return Seconds(0.5f);

		Call(game, "ShowCaseOverlay");
		yield return Seconds(1f);
		yield return Shot("04-case-overlay");
		Call(game, "HideCaseOverlay");   // the X button; ShowMainMenu leaves the overlay up, as shipped
		yield return Seconds(0.5f);

		Call(game, "ShowGemShop");
		yield return Seconds(1f);
		yield return Shot("05-gem-shop");
		Call(game, "HideGemShop");
		yield return Seconds(0.5f);

		// The first run shows the tutorial (gameplay scale 0 until its stages are passed); the smoke
		// test drives a "returning player" run, the tutorial has its own checks below.
		const string tutorialKey = "infernoDriveFirstRunTutorialV7";
		bool hadTutorial = PlayerPrefs.HasKey(tutorialKey);
		int tutorialValue = PlayerPrefs.GetInt(tutorialKey, 0);
		PlayerPrefs.SetInt(tutorialKey, 1);
		Call(game, "StartRun");
		yield return Seconds(0.5f);
		Assert.IsTrue((bool)Field(game, "running"), "running after StartRun");
		yield return Seconds(6f);
		Assert.Greater((float)Field(game, "distance"), 50f, "the road scrolls");
		Assert.Greater((float)Field(game, "speed"), 10f, "the car accelerates");
		yield return Shot("06-run");

		Call(game, "PauseRun");
		yield return Seconds(0.5f);
		Assert.IsTrue((bool)Field(game, "paused"));
		yield return Shot("07-pause");
		Call(game, "ResumeRun");
		yield return Seconds(1f);
		Assert.IsFalse((bool)Field(game, "paused"));

		SetField(game, "lifeHalfUnits", 0);
		yield return Seconds(2f);
		Assert.IsTrue((bool)Field(game, "showingGameOver"), "game over after losing all lives");
		yield return Shot("08-game-over");

		Call(game, "ShowMainMenu");
		yield return Seconds(1f);
		Assert.IsFalse((bool)Field(game, "running"));
		yield return Shot("09-lobby-again");

		// First-run tutorial: gameplay is frozen until the stages are passed (or time out at 25 s each).
		PlayerPrefs.DeleteKey(tutorialKey);
		Call(game, "StartRun");
		yield return Seconds(1.5f);
		Assert.IsTrue((bool)Field(game, "running"));
		Assert.Less((float)Field(game, "distance"), 1f, "tutorial holds the car");
		yield return Shot("10-tutorial");
		Call(game, "ShowMainMenu");
		yield return Seconds(0.5f);
		if (hadTutorial) PlayerPrefs.SetInt(tutorialKey, tutorialValue); else PlayerPrefs.DeleteKey(tutorialKey);
	}

	// A longer run without input: the distance-driven systems (road crystals 350-650 m, puddles 500-900 m,
	// wrecking balls 900-1300 m, SPEED UP at 1000 m, ramps, high beams, traffic contact) all fire within
	// ~2 km, and the run must stay error-free until game over or the time limit.
	[UnityTest]
	public IEnumerator LongRunReachesTheDistanceSystems()
	{
#if UNITY_EDITOR
		SetPortraitGameView();
		yield return null;
#endif
		yield return null;
		if (Game() == null) new GameObject("Shadow Run Prototype").AddComponent(GameType);
		yield return Seconds(2f);
		Component game = Game();
		Type authType = Type.GetType("ShadowRun.Services.FirstLaunchAuthScreen, Assembly-CSharp");
		var auth = (Component)UnityEngine.Object.FindFirstObjectByType(authType);
		if (auth != null) { authType.GetMethod("Close", Any).Invoke(auth, null); yield return Seconds(1f); }
		if ((bool)Field(game, "running") || (bool)Field(game, "showingGameOver")) { Call(game, "ShowMainMenu"); yield return Seconds(1f); }

		const string tutorialKey = "infernoDriveFirstRunTutorialV7";
		bool hadTutorial = PlayerPrefs.HasKey(tutorialKey);
		int tutorialValue = PlayerPrefs.GetInt(tutorialKey, 0);
		PlayerPrefs.SetInt(tutorialKey, 1);
		Call(game, "StartRun");
		float start = Time.realtimeSinceStartup;
		float maxDistance = 0f, maxSpeed = 0f; int shots = 0; int minLives = 6;
		Type drivingType = GameType.GetNestedType("InfernoDriving", Any);
		Type hazardsType = GameType.GetNestedType("InfernoHazards", Any);
		int frames = 0;
		// the editor may run well under 60 fps and the gameplay dt is capped at 1/30 s, so the budget is
		// in distance (1.5 km covers every distance-driven system) with a generous time limit
		while (Time.realtimeSinceStartup - start < 240f && maxDistance < 1500f && !(bool)Field(game, "showingGameOver"))
		{
			yield return null;
			frames++;
			float distance = (float)Field(game, "distance");
			maxDistance = Mathf.Max(maxDistance, distance);
			maxSpeed = Mathf.Max(maxSpeed, (float)Field(game, "speed"));
			minLives = Mathf.Min(minLives, (int)Field(game, "lifeHalfUnits"));
			if (distance > 300f * (shots + 1) && shots < 6) { shots++; yield return Shot("20-run-" + Mathf.RoundToInt(distance) + "m"); }
		}
		float elapsed = Time.realtimeSinceStartup - start;
		Debug.Log("[SmokeTests] long run: distance " + maxDistance + " m in " + elapsed.ToString("0") + " s at " + (frames / elapsed).ToString("0") + " fps, top speed " + maxSpeed + " m/s, lives left (half units) " + minLives + ", game over " + (bool)Field(game, "showingGameOver"));
		// Without steering the car usually wrecks on traffic before 1 km (three hearts): both outcomes are
		// valid ends; the systems whose spawn window was passed must have fired.
		bool over = (bool)Field(game, "showingGameOver");
		Assert.IsTrue(maxDistance >= 1000f || over, "reached 1 km or wrecked");
		Assert.Greater(maxDistance, 300f, "a real run");
		Assert.Greater(maxSpeed, 28f, "cruise speed");
		if (over) Assert.AreEqual(0, minLives, "game over comes from losing every half heart");
		int runCollected = (int)drivingType.GetField("s_runCollected", Any).GetValue(null);
		float nextCrystal = (float)drivingType.GetField("s_nextDistance", Any).GetValue(null);
		float nextPuddle = (float)hazardsType.GetField("s_nextPuddle", Any).GetValue(null);
		float nextBall = (float)hazardsType.GetField("s_nextBall", Any).GetValue(null);
		Debug.Log("[SmokeTests] crystals collected " + runCollected + ", next crystal " + nextCrystal + ", next puddle " + nextPuddle + ", next ball " + nextBall);
		if (maxDistance > 650f) Assert.Greater(nextCrystal, 650f, "a road crystal spawned (next distance advanced past the first window)");
		if (maxDistance > 900f) Assert.Greater(nextPuddle, 900f, "a puddle spawned");
		if (maxDistance > 1300f) Assert.Greater(nextBall, 1300f, "a wrecking ball spawned");
		yield return Shot("21-run-end");
		if (!(bool)Field(game, "showingGameOver")) { Call(game, "ShowMainMenu"); yield return Seconds(0.5f); }
		else { Call(game, "ShowMainMenu"); yield return Seconds(0.5f); }
		if (hadTutorial) PlayerPrefs.SetInt(tutorialKey, tutorialValue); else PlayerPrefs.DeleteKey(tutorialKey);
	}
}
