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
	// The editor cannot serve the built-in extra sprite the first-launch screen asks for
	// (Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd") is an editor-only asset database lookup
	// in 6000.x); the shipped code does exactly this, so the assert is expected. Everything else that is an
	// error or exception still fails the test.
	private static readonly System.Collections.Generic.List<string> unexpected = new System.Collections.Generic.List<string>();
	private static void OnLog(string condition, string stackTrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
		if (condition.Contains("UISprite.psd")) return;
		unexpected.Add(type + ": " + condition);
	}

	// PlayerPrefs the tests touch are snapshotted and restored (also when a test fails half-way), so one
	// run never changes what the next one sees.
	private static readonly string[] PrefKeys = BuildPrefKeys();
	private static string[] BuildPrefKeys()
	{
		var keys = new System.Collections.Generic.List<string> { "InfernoDrive.MapUnlocked.2", "InfernoDrive.MapUnlocked.3", "InfernoDrive.SelectedMap", "shadowRun.daily.claimedMask", "shadowRun.daily.date", "infernoDriveFirstRunTutorialV7", "InfernoDrive.MasterVolume", "shadowRunCrystals", "shadowRunPendingCrystalDelta", "shadowRunCloudDirty" };
		foreach (var cat in new[] { "Tires", "Flags", "Trails" })
		{
			keys.Add("InfernoDrive.Style." + cat + ".Selected");
			for (int i = 1; i < 8; i++) keys.Add("InfernoDrive.Style." + cat + "." + i + ".Owned");
		}
		return keys.ToArray();
	}
	private static readonly System.Collections.Generic.Dictionary<string, int?> prefSnapshot = new System.Collections.Generic.Dictionary<string, int?>();

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
	private static readonly string ShotDir = Environment.GetEnvironmentVariable("INFERNO_SHOTS") ?? "/tmp/inferno-shots";

	private static Component Game()
	{
		return (Component)UnityEngine.Object.FindFirstObjectByType(GameType);
	}

	// The app boots into SampleScene (Main Camera + AudioListener, Directional Light, Global Volume) and
	// AutoStart adds the game there; the test runner starts in an empty scene where AutoStart already
	// created the game. The scene camera's AudioListener is deliberately NOT recreated: on this build
	// machine (no audio output device) the editor wedges inside the case-opening audio as soon as a
	// listener exists, and reloading SampleScene mid-test wedges it the same way — the "no audio
	// listeners" warning is the price. The first-launch authorization screen is closed the way a
	// completed sign-in would (its private Close) — no guest account is created on the backend.
	private static IEnumerator Boot()
	{
		LogAssert.ignoreFailingMessages = true;
#if UNITY_EDITOR
		SetPortraitGameView();
		yield return null;
#endif
		yield return null;
		if (Game() == null) new GameObject("Shadow Run Prototype").AddComponent(GameType);
		yield return Seconds(2f);
		Type authType = Type.GetType("ShadowRun.Services.FirstLaunchAuthScreen, Assembly-CSharp");
		var auth = (Component)UnityEngine.Object.FindFirstObjectByType(authType);
		if (auth != null)
		{
			yield return Shot("00-first-launch-auth");
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
				c.planeDistance = cam.nearClipPlane + 0.02f;   // nothing in the world can sit in front of the UI plane
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
		yield return Boot();
		Component game = Game();
		Assert.IsNotNull(game, "ShadowRunPrototype instance");
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
		// The rewarded ad cannot load in the editor: start the opening directly (what the ad reward does):
		// lid animation, 9 s roulette, prize panel.
		int crystalsBefore = (int)Type.GetType("ShadowRun.Services.SupabaseGameManager, Assembly-CSharp").GetProperty("Crystals").GetValue(null);
		// applying the reward without an authenticated profile (QueueCaseReward) may re-open the auth
		// screen, whose rounded frames log the editor-only UISprite assert (filtered by OnLog)
		Call(game, "StartCaseOpening");
		yield return Seconds(2.5f);
		yield return Shot("04b-case-lid");
		yield return Seconds(4f);
		yield return Shot("04c-case-roulette");
		float caseWait = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - caseWait < 20f && (bool)Field(game, "caseSpinActive")) yield return null;
		Assert.IsFalse((bool)Field(game, "caseSpinActive"), "the roulette stops");
		yield return Seconds(2f);
		yield return Shot("04d-case-result");
		Debug.Log("[SmokeTests] case prize applied: " + Field(game, "caseRewardApplied") + ", crystals " + crystalsBefore + " -> " + Type.GetType("ShadowRun.Services.SupabaseGameManager, Assembly-CSharp").GetProperty("Crystals").GetValue(null));
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
		yield return Boot();
		Component game = Game();

		const string tutorialKey = "infernoDriveFirstRunTutorialV7";
		bool hadTutorial = PlayerPrefs.HasKey(tutorialKey);
		int tutorialValue = PlayerPrefs.GetInt(tutorialKey, 0);
		PlayerPrefs.SetInt(tutorialKey, 1);
		Call(game, "StartRun");
		float start = Time.realtimeSinceStartup;
		float maxDistance = 0f, maxSpeed = 0f; int shots = 0; int minLives = 6;
		Type drivingType = GameType.GetNestedType("InfernoDriving", Any);
		Type hazardsType = GameType.GetNestedType("InfernoHazards", Any);
		int frames = 0, airborneFrames = 0;
		// the editor may run well under 60 fps and the gameplay dt is capped at 1/30 s, so the budget is
		// in distance (1.5 km covers every distance-driven system) with a generous time limit
		// A simple driver: steer toward the lane whose nearest car ahead is farthest (traffic sits in
		// `pickups`, ahead = smaller z than the player at z 5) using the same touch pointer sets a finger
		// would fill; boost is held all the way.
		var lanes = (float[])Field(game, "lanes");
		var leftSet = (System.Collections.Generic.HashSet<int>)Field(game, "touchSteerLeftPointers");
		var rightSet = (System.Collections.Generic.HashSet<int>)Field(game, "touchSteerRightPointers");
		var pickupsList = (System.Collections.IList)Field(game, "pickups");
		Type pickupType = GameType.GetNestedType("Pickup", Any);
		var transformField = pickupType.GetField("Transform", Any);
		SetField(game, "touchBoostHeld", true);
		while (Time.realtimeSinceStartup - start < 240f && maxDistance < 1500f && !(bool)Field(game, "showingGameOver"))
		{
			yield return null;
			frames++;
			if ((bool)Field(game, "playerAirborne")) airborneFrames++;
			{
				float px = (float)Field(game, "playerX");
				float best = -1f; int bestLane = 0; float current = 0f;
				for (int lane = 0; lane < lanes.Length; lane++)
				{
					float gap = 400f;
					foreach (object car in pickupsList)
					{
						var t = (Transform)transformField.GetValue(car);
						if (t == null || !t.gameObject.activeInHierarchy) continue;
						Vector3 cp = t.position;
						if (Mathf.Abs(cp.x - lanes[lane]) > 1.6f) continue;
						float ahead = 5f - cp.z;   // metres in front of the car
						if (ahead > -6f && ahead < gap) gap = ahead;
					}
					if (Mathf.Abs(px - lanes[lane]) < 1.2f) current = gap;
					if (gap > best) { best = gap; bestLane = lane; }
				}
				float targetX = current > 45f ? px : lanes[bestLane];
				leftSet.Clear(); rightSet.Clear();
				if (targetX < px - 0.3f) leftSet.Add(1); else if (targetX > px + 0.3f) rightSet.Add(2);
			}
			float distance = (float)Field(game, "distance");
			maxDistance = Mathf.Max(maxDistance, distance);
			maxSpeed = Mathf.Max(maxSpeed, (float)Field(game, "speed"));
			minLives = Mathf.Min(minLives, (int)Field(game, "lifeHalfUnits"));
			if (distance > 300f * (shots + 1) && shots < 6) { shots++; yield return Shot("20-run-" + Mathf.RoundToInt(distance) + "m"); }
		}
		leftSet.Clear(); rightSet.Clear(); SetField(game, "touchBoostHeld", false);
		float elapsed = Time.realtimeSinceStartup - start;
		Debug.Log("[SmokeTests] long run: distance " + maxDistance + " m in " + elapsed.ToString("0") + " s at " + (frames / elapsed).ToString("0") + " fps, top speed " + maxSpeed + " m/s, lives left (half units) " + minLives + ", game over " + (bool)Field(game, "showingGameOver") + ", airborne frames (ramps) " + airborneFrames);
		// Without steering the car usually wrecks on traffic before 1 km (three hearts): both outcomes are
		// valid ends; the systems whose spawn window was passed must have fired.
		bool over = (bool)Field(game, "showingGameOver");
		Assert.IsTrue(maxDistance >= 1000f || over, "reached 1 km or wrecked");
		int stage = (int)drivingType.GetField("s_stage", Any).GetValue(null);
		Debug.Log("[SmokeTests] speed stage reached " + stage);
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

	// ---- driving the real UI ------------------------------------------------------------------------
	private static UnityEngine.UI.Button FindButton(string objectName)
	{
		foreach (var b in UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			if (b.gameObject.name == objectName && b.gameObject.activeInHierarchy) return b;
		return null;
	}

	private static bool Click(string objectName)
	{
		var b = FindButton(objectName);
		if (b == null) { Debug.Log("[SmokeTests] no active button " + objectName); return false; }
		b.onClick.Invoke();
		return true;
	}

	// Buttons built on EventTrigger (pause, BEAM): fire their PointerDown / PointerUp entries.
	private static bool Press(string objectName)
	{
		foreach (var t in UnityEngine.Object.FindObjectsByType<UnityEngine.EventSystems.EventTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (t.gameObject.name != objectName || !t.gameObject.activeInHierarchy) continue;
			var data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
			foreach (var e in t.triggers) if (e.eventID == UnityEngine.EventSystems.EventTriggerType.PointerDown) e.callback.Invoke(data);
			foreach (var e in t.triggers) if (e.eventID == UnityEngine.EventSystems.EventTriggerType.PointerUp) e.callback.Invoke(data);
			return true;
		}
		Debug.Log("[SmokeTests] no event trigger " + objectName);
		return false;
	}

	private static string Names(string prefix)
	{
		var list = new System.Collections.Generic.List<string>();
		foreach (var b in UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			if (b.gameObject.activeInHierarchy && b.gameObject.name.StartsWith(prefix)) list.Add(b.gameObject.name);
		return string.Join(", ", list);
	}

	// Lobby, garage, style sheet, settings and HUD controls through their own buttons, plus boost /
	// steering / high beam through the touch state the game reads (touchBoostHeld, touchSteer pointers).
	[UnityTest]
	public IEnumerator ControlsAndScreensThroughTheirButtons()
	{
		yield return Boot();
		Component game = Game();
		Type lobby = GameType.GetNestedType("InfernoLobby", Any);

		// map selector: › steps to RAIN RUN (locked, price 150 > 25 crystals) -> PLAY reads U N L O C K 150,
		// tapping it shows NEED n MORE CRYSTALS; ‹ twice back to NEON NIGHT
		int mapBefore = (int)lobby.GetMethod("SelectedMap", Any).Invoke(null, null);
		Assert.IsTrue(Click("NextMap"), "NextMap button");
		yield return Seconds(0.3f);
		Assert.AreEqual((mapBefore + 1) % 6, (int)lobby.GetMethod("SelectedMap", Any).Invoke(null, null));
		var primary = (UnityEngine.UI.Text)Field(game, "primaryButtonLabel");
		Debug.Log("[SmokeTests] map " + lobby.GetMethod("SelectedMap", Any).Invoke(null, null) + " primary label '" + primary.text + "'");
		if (primary.text.StartsWith("U N L O C K"))
		{
			((UnityEngine.UI.Button)Field(game, "primaryOverlayButton")).onClick.Invoke();
			yield return Seconds(0.3f);
			yield return Shot("30-map-locked");
			Debug.Log("[SmokeTests] locked map note '" + primary.text + "'");
		}
		Assert.IsTrue(Click("PreviousMap")); yield return Seconds(0.2f);
		while ((int)lobby.GetMethod("SelectedMap", Any).Invoke(null, null) != mapBefore) { Click("PreviousMap"); yield return Seconds(0.2f); }

		// daily goal claim buttons exist (three goals) and the gem counter opens the shop
		Debug.Log("[SmokeTests] lobby buttons: " + Names(""));
		Assert.IsTrue(Click("GemCounter")); yield return Seconds(0.5f);
		Call(game, "HideGemShop"); yield return Seconds(0.3f);

		// garage through the dock: select the next card, try the purchase (25 < 100 crystals -> "Need 75 more"),
		// open the STYLE sheet, page each row, pick a free item, close
		Assert.IsTrue(Click("Garage")); yield return Seconds(1f);
		Call(game, "SelectGarageVehicleCard", 1); yield return Seconds(0.8f);
		var purchaseText = (UnityEngine.UI.Text)Field(game, "garagePurchaseButtonText");
		Debug.Log("[SmokeTests] garage card 1 purchase label '" + (purchaseText != null ? purchaseText.text : "-") + "'");
		if (FindButton("Unlock Selected Vehicle Button") != null)
		{
			Click("Unlock Selected Vehicle Button"); yield return Seconds(1.5f);
			Debug.Log("[SmokeTests] after purchase attempt '" + purchaseText.text + "'");
		}
		yield return Shot("31-garage-card-1");
		Call(game, "SelectGarageVehicleCard", 0); yield return Seconds(0.5f);
		Assert.IsTrue(Click("StyleButton"), "STYLE button"); yield return Seconds(0.5f);
		yield return Shot("32-style-sheet");
		Assert.IsTrue(Click("Next")); yield return Seconds(0.3f);   // first row's › (TIRES page 2)
		Click("Prev"); yield return Seconds(0.3f);
		Click("Chip"); yield return Seconds(0.5f);                   // STOCK (free) — already selected, no-op
		yield return Shot("33-style-paged");
		Assert.IsTrue(Click("Close")); yield return Seconds(0.3f);

		// profile: vibration switch toggles the pref, sound toggle mutes the listener
		Assert.IsTrue(Click("Profile")); yield return Seconds(0.8f);
		bool vibrationBefore = (bool)Field(game, "vibrationOn");
		Assert.IsTrue(Click("VibrationSwitch")); yield return Seconds(0.3f);
		Assert.AreNotEqual(vibrationBefore, (bool)Field(game, "vibrationOn"), "vibration toggles");
		Click("VibrationSwitch"); yield return Seconds(0.3f);
		yield return Shot("34-profile-after-toggle");
		Assert.IsTrue(Click("Home")); yield return Seconds(0.5f);

		// a run with boost held, steering pointers and the BEAM button
		const string tutorialKey = "infernoDriveFirstRunTutorialV7";
		bool hadTutorial = PlayerPrefs.HasKey(tutorialKey); int tutorialValue = PlayerPrefs.GetInt(tutorialKey, 0);
		PlayerPrefs.SetInt(tutorialKey, 1);
		((UnityEngine.UI.Button)Field(game, "primaryOverlayButton")).onClick.Invoke();   // PLAY
		yield return Seconds(1f);
		Assert.IsTrue((bool)Field(game, "running"), "PLAY starts the run");
		SetField(game, "touchBoostHeld", true);
		var left = (System.Collections.Generic.HashSet<int>)Field(game, "touchSteerLeftPointers");
		left.Add(1);
		yield return Seconds(3f);
		float x = (float)Field(game, "playerX");
		Debug.Log("[SmokeTests] boost+left for 3 s: speed " + Field(game, "speed") + " playerX " + x);
		Assert.Less(x, -1f, "steering pointer moves the car left");
		left.Clear();
		var right = (System.Collections.Generic.HashSet<int>)Field(game, "touchSteerRightPointers");
		right.Add(2);
		yield return Seconds(2f);
		right.Clear();
		Assert.IsTrue(Press("BEAM Button"), "BEAM button " + Names("BEAM"));
		yield return Seconds(0.5f);
		Debug.Log("[SmokeTests] high beam: window " + Field(game, "highBeamActiveWindowRemaining") + " cooldown " + Field(game, "highBeamCooldownRemaining"));
		Assert.Greater((float)Field(game, "highBeamActiveWindowRemaining"), 0f, "the BEAM press opens the 5 s high-beam window (the 10 s cooldown follows it)");
		yield return Shot("35-run-boost-beam");
		SetField(game, "touchBoostHeld", false);
		Assert.IsTrue(Click("Pause HUD Button") || Press("Pause HUD Button"), "pause button " + Names("Pause"));
		yield return Seconds(0.5f);
		Assert.IsTrue((bool)Field(game, "paused"), "pause button pauses");
		Assert.IsTrue(Click("Pause Sound Toggle"), "pause sound toggle"); yield return Seconds(0.2f); Click("Pause Sound Toggle");
		Assert.IsTrue(Click("Pause Continue"), "CONTINUE DRIVING"); yield return Seconds(0.5f);
		Assert.IsFalse((bool)Field(game, "paused"), "continue resumes");
		Call(game, "ShowMainMenu"); yield return Seconds(0.5f);
		if (hadTutorial) PlayerPrefs.SetInt(tutorialKey, tutorialValue); else PlayerPrefs.DeleteKey(tutorialKey);
	}

	private static T Find<T>(string objectName) where T : Component
	{
		foreach (var c in UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			if (c.gameObject.name == objectName && c.gameObject.activeInHierarchy) return c;
		return null;
	}

	// Economy and menus with a local balance: map unlock, daily-goal claims, the settings sliders and
	// sound toggle, the game-over actions, the tutorial stage machine. Purchases stay behind
	// RequireOnlineEconomyAsync (an authenticated account), so the garage purchase is only driven up to
	// its "account required" status — nothing here signs in to the real backend.
	[UnityTest]
	public IEnumerator EconomyMenusAndTutorial()
	{
		yield return Boot();
		Component game = Game();
		Type lobby = GameType.GetNestedType("InfernoLobby", Any);
		Type manager = Type.GetType("ShadowRun.Services.SupabaseGameManager, Assembly-CSharp");
		Func<int> crystals = () => (int)manager.GetProperty("Crystals").GetValue(null);
		Action<int> setBalance = v => lobby.GetMethod("SetCrystalBalance", Any).Invoke(null, new object[] { game, v });
		int startCrystals = crystals();
		const string tutorialKey = "infernoDriveFirstRunTutorialV7";
		bool hadTutorial = PlayerPrefs.HasKey(tutorialKey); int tutorialValue = PlayerPrefs.GetInt(tutorialKey, 0);
		PlayerPrefs.SetInt(tutorialKey, 1);

		// --- map unlock: 500 crystals, › to RAIN RUN (150), PLAY unlocks, PLAY again starts the run on map 2
		PlayerPrefs.DeleteKey("InfernoDrive.MapUnlocked.2"); PlayerPrefs.DeleteKey("shadowRun.daily.claimedMask");
		SetField(game, "dailyGoalClaimedMask", 0);
		setBalance(500); yield return Seconds(0.3f);
		lobby.GetMethod("RefreshMap", Any).Invoke(null, new object[] { game });
		int mapBefore = (int)lobby.GetMethod("SelectedMap", Any).Invoke(null, null);
		while ((int)lobby.GetMethod("SelectedMap", Any).Invoke(null, null) != 2) { Click("NextMap"); yield return Seconds(0.2f); }
		var play = (UnityEngine.UI.Button)Field(game, "primaryOverlayButton");
		var primary = (UnityEngine.UI.Text)Field(game, "primaryButtonLabel");
		Assert.AreEqual("U N L O C K   150", primary.text);
		play.onClick.Invoke(); yield return Seconds(0.4f);
		Assert.IsTrue((bool)lobby.GetMethod("MapUnlocked", Any).Invoke(null, new object[] { 2 }), "RAIN RUN unlocked");
		Assert.AreEqual(350, crystals(), "150 crystals spent");
		Assert.AreEqual("P  L  A  Y", primary.text);
		var descriptionNode = (Transform)lobby.GetMethod("Find", Any).Invoke(null, new object[] { Field(game, "homeHeroRoot"), "MapDescription" });
		var description = descriptionNode != null ? descriptionNode.GetComponent<UnityEngine.UI.Text>() : null;
		Debug.Log("[SmokeTests] map 2 after unlock: '" + (description != null ? description.text : "-") + "'");
		yield return Shot("40-map-unlocked");
		play.onClick.Invoke(); yield return Seconds(1.5f);
		Assert.IsTrue((bool)Field(game, "running"), "PLAY starts the run on the unlocked map");
		Assert.AreEqual(2, (int)lobby.GetMethod("SelectedMap", Any).Invoke(null, null));
		yield return Shot("41-run-rain-map");
		Call(game, "ShowMainMenu"); yield return Seconds(0.5f);
		PlayerPrefs.DeleteKey("InfernoDrive.MapUnlocked.2");
		PlayerPrefs.SetInt("InfernoDrive.SelectedMap", mapBefore); PlayerPrefs.Save();
		lobby.GetMethod("RefreshMap", Any).Invoke(null, new object[] { game });

		// --- daily goals: complete the three visible goals, claim each through its CollectGoal button
		var goals = (Array)Call(game, "PickDailyGoals", 3);
		Type goalType = GameType.GetNestedType("HomeGoalData", Any);
		var kinds = new System.Collections.Generic.List<int>();
		foreach (object g in goals) kinds.Add((int)goalType.GetField("Kind", Any).GetValue(g));
		SetField(game, "dailyGoalClaimedMask", 0);
		PlayerPrefs.DeleteKey("shadowRun.daily.claimedMask");
		foreach (int kind in kinds)
		{
			switch (kind)
			{
				case 0: SetField(game, "dailyGoalDistanceMeters", 10000f); break;
				case 1: SetField(game, "dailyGoalRuns", 3); break;
				case 2: SetField(game, "dailyGoalBoosts", 10); break;
				case 3: SetField(game, "dailyGoalDriftSeconds", 30f); break;
				case 4: SetField(game, "dailyGoalCarsPassed", 25); break;
				case 5: SetField(game, "dailyGoalCasesOpened", 1); break;
			}
		}
		Call(game, "UpdateDailyGoalsTimer", true); yield return Seconds(0.5f);
		yield return Shot("42-goals-complete");
		int before = crystals(); int expected = 0;
		foreach (int kind in kinds) expected += (int)lobby.GetMethod("GoalReward", Any).Invoke(null, new object[] { kind });
		int claimed = 0;
		for (int i = 0; i < 3 && Click("CollectGoal"); i++) { claimed++; yield return Seconds(0.3f); }
		Debug.Log("[SmokeTests] goals " + string.Join(",", kinds) + " claimed " + claimed + ": crystals " + before + " -> " + crystals() + " (expected +" + expected + ")");
		Assert.AreEqual(3, claimed, "three CLAIM buttons");
		Assert.AreEqual(before + expected, crystals(), "goal rewards 15/10/10/15/10/20 per kind");
		yield return Shot("43-goals-claimed");
		SetField(game, "dailyGoalClaimedMask", 0);
		SetField(game, "dailyGoalDistanceMeters", 0f); SetField(game, "dailyGoalRuns", 0); SetField(game, "dailyGoalBoosts", 0);
		SetField(game, "dailyGoalDriftSeconds", 0f); SetField(game, "dailyGoalCarsPassed", 0); SetField(game, "dailyGoalCasesOpened", 0);
		PlayerPrefs.DeleteKey("shadowRun.daily.claimedMask");
		Call(game, "UpdateDailyGoalsTimer", true);

		// --- garage purchase with 100+ crystals stops at the account requirement (no sign-in from a test)
		Click("Garage"); yield return Seconds(0.8f);
		Call(game, "SelectGarageVehicleCard", 1); yield return Seconds(0.6f);
		Click("Unlock Selected Vehicle Button"); yield return Seconds(2.5f);
		var status = (UnityEngine.UI.Text)Field(game, "garagePurchaseStatusText");
		Debug.Log("[SmokeTests] purchase with " + crystals() + " crystals: button '" + ((UnityEngine.UI.Text)Field(game, "garagePurchaseButtonText")).text + "' status '" + (status != null ? status.text : "-") + "'");
		Assert.IsFalse((bool)manager.GetMethod("IsVehicleUnlocked").Invoke(null, new object[] { "flying_car" }), "no purchase without an account");
		Call(game, "SelectGarageVehicleCard", 0); yield return Seconds(0.3f);

		// --- settings: sliders save after 0.6 s, listener volume follows master and the sound toggle
		Click("Profile"); yield return Seconds(0.8f);
		var master = Find<UnityEngine.UI.Slider>("MasterSlider"); var music = Find<UnityEngine.UI.Slider>("MusicSlider"); var effects = Find<UnityEngine.UI.Slider>("EffectsSlider");
		Assert.IsNotNull(master); Assert.IsNotNull(music); Assert.IsNotNull(effects);
		float m0 = master.value, mu0 = music.value, ef0 = effects.value;
		master.value = 40f; music.value = 20f; effects.value = 90f;
		yield return Seconds(1.2f);
		Assert.AreEqual(40, PlayerPrefs.GetInt("InfernoDrive.MasterVolume", -1), "master saved");
		Assert.AreEqual(20, (int)Field(game, "musicVolume")); Assert.AreEqual(90, (int)Field(game, "sfxVolume"));
		Assert.AreEqual(0.4f, AudioListener.volume, 0.01f, "listener = master/100");
		yield return Shot("44-settings-changed");
		// the profile page's own sound button is hidden by the settings patch (MASTER replaces it); the
		// pause menu keeps its toggle
		Assert.IsNull(FindButton("Profile Sound Toggle"), "profile sound toggle hidden, as shipped");
		master.value = m0; music.value = mu0; effects.value = ef0; yield return Seconds(1f);
		Click("Home"); yield return Seconds(0.5f);

		// --- pause-menu sound toggle mutes the listener; game over actions: TRY AGAIN restarts, OPEN CASE
		// opens the case sheet, BACK TO MENU
		play.onClick.Invoke(); yield return Seconds(1f);
		Debug.Log("[SmokeTests] after PLAY: running " + Field(game, "running") + " paused " + Field(game, "paused") + " gameOver " + Field(game, "showingGameOver") + " map " + lobby.GetMethod("SelectedMap", Any).Invoke(null, null) + " unlocked " + lobby.GetMethod("MapUnlocked", Any).Invoke(null, new object[] { lobby.GetMethod("SelectedMap", Any).Invoke(null, null) }) + " purchaseBusy " + Field(game, "garagePurchaseBusy") + " label '" + primary.text + "' listeners " + play.onClick.GetPersistentEventCount());
		Assert.IsTrue((bool)Field(game, "running"), "PLAY starts the run (menu panel open: " + Call(game, "MenuPanelOpen") + ")");
		Call(game, "PauseRun"); yield return Seconds(0.3f);
		Assert.IsTrue((bool)Field(game, "paused"), "paused");
		Debug.Log("[SmokeTests] pause buttons: " + Names("Pause"));
		Assert.IsTrue(Click("Pause Sound Toggle"), "pause sound toggle"); yield return Seconds(0.3f);
		Assert.IsFalse((bool)Field(game, "gameSoundEnabled")); Assert.AreEqual(0f, AudioListener.volume, 0.001f, "muted");
		Click("Pause Sound Toggle"); yield return Seconds(0.3f);
		Assert.IsTrue((bool)Field(game, "gameSoundEnabled"));
		Click("Pause Continue"); yield return Seconds(0.5f);
		SetField(game, "lifeHalfUnits", 0); yield return Seconds(2f);
		Assert.IsTrue((bool)Field(game, "showingGameOver"));
		Assert.IsTrue(Click("TRY AGAIN Profile Button"), "TRY AGAIN"); yield return Seconds(1f);
		Assert.IsTrue((bool)Field(game, "running") && !(bool)Field(game, "showingGameOver"), "TRY AGAIN restarts the run");
		SetField(game, "lifeHalfUnits", 0); yield return Seconds(2f);
		Assert.IsTrue(Click("OPEN CASE Profile Button"), "OPEN CASE"); yield return Seconds(0.8f);
		yield return Shot("45-game-over-open-case");
		Call(game, "HideCaseOverlay"); yield return Seconds(0.3f);
		if (FindButton("BACK TO MENU Profile Button") != null) Click("BACK TO MENU Profile Button"); else Call(game, "ShowMainMenu");
		yield return Seconds(0.8f);
		Assert.IsFalse((bool)Field(game, "running"));

		// --- tutorial stage machine: stage 3 (brake stage, auto-passes) -> 4 (steer left) -> 5 (steer right) -> 6 -> done
		PlayerPrefs.DeleteKey(tutorialKey);
		play.onClick.Invoke(); yield return Seconds(1f);
		Assert.IsTrue((bool)Field(game, "firstRunTutorialActive"), "tutorial on a first run");
		Call(game, "SetFirstRunTutorialStage", 3); yield return Seconds(1f);
		Assert.AreEqual(4, (int)Field(game, "firstRunTutorialStage"), "the brake stage passes by itself (brake removed)");
		var left = (System.Collections.Generic.HashSet<int>)Field(game, "touchSteerLeftPointers");
		var right = (System.Collections.Generic.HashSet<int>)Field(game, "touchSteerRightPointers");
		left.Add(1); yield return Seconds(1f); left.Clear();
		Assert.AreEqual(5, (int)Field(game, "firstRunTutorialStage"), "steer left completes stage 4");
		yield return Shot("46-tutorial-stage-5");
		right.Add(2); yield return Seconds(1f); right.Clear();
		Assert.GreaterOrEqual((int)Field(game, "firstRunTutorialStage"), 6, "steer right completes stage 5");
		yield return Seconds(2f);
		Assert.IsFalse((bool)Field(game, "firstRunTutorialActive"), "tutorial completes");
		Assert.AreEqual(1, PlayerPrefs.GetInt(tutorialKey, 0), "completion is remembered");
		Call(game, "ShowMainMenu"); yield return Seconds(0.5f);

		setBalance(startCrystals);
		if (hadTutorial) PlayerPrefs.SetInt(tutorialKey, tutorialValue); else PlayerPrefs.DeleteKey(tutorialKey);
	}
}
