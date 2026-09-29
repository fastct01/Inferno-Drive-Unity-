using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// [user request 2026-09-21: maps] Captures the six lobby map cards from the maps themselves:
// with the game running (Inferno Drive > Play (portrait)), every map is selected in turn through
// the lobby arrows (so InfernoMaps.Apply runs exactly as in the game), the world camera is rendered
// into a 1170x595 texture (the card's 936x476 aspect) and written to
// Assets/Resources/InfernoLobby/<slug>.bytes — the file InfernoLobby.ShowMapArt loads.
// Menu: Inferno Drive > Capture Map Thumbnails, or InfernoMapThumbnails.CaptureAll() from a command.
// The unfocused Editor does not advance the player loop on its own, so the tool steps it.
public static class InfernoMapThumbnails
{
	private static readonly string[] Slugs = { "daylight_city", "neon_night", "rain_run", "storm_chase", "fallen_sky", "planet_horizon" };
	private const int Width = 1170, Height = 595;

	private static int map, step, wait, savedMap;
	private static bool armed;
	private static readonly bool[] unlocked = new bool[6];

	[MenuItem("Inferno Drive/Capture Map Thumbnails")]
	public static void CaptureAll()
	{
		if (!EditorApplication.isPlaying) { Debug.LogWarning("[Thumbs] enter play mode first (Inferno Drive > Play (portrait))"); return; }
		if (armed) return;
		Application.runInBackground = true;
		savedMap = PlayerPrefs.GetInt("InfernoDrive.SelectedMap", 1);
		for (int i = 0; i < Slugs.Length; i++)
		{
			unlocked[i] = PlayerPrefs.GetInt("InfernoDrive.MapUnlocked." + i, 0) == 1;
			PlayerPrefs.SetInt("InfernoDrive.MapUnlocked." + i, 1);
		}
		PlayerPrefs.Save();
		Click("Home");
		map = 0; step = 0; wait = 15; armed = true;
		EditorApplication.update += Tick;
		Debug.Log("[Thumbs] started");
	}

	private static Button Find(string name)
	{
		foreach (Button b in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
			if (b.gameObject.name == name && b.gameObject.activeInHierarchy) return b;
		return null;
	}

	private static void Click(string name)
	{
		Button b = Find(name);
		if (b != null) b.onClick.Invoke(); else Debug.LogWarning("[Thumbs] no button " + name);
	}

	private static void Tick()
	{
		if (!armed) return;
		if (!EditorApplication.isPlaying) { Finish(); return; }
		EditorApplication.Step();
		if (--wait > 0) return;
		try
		{
			if (step == 0)
			{
				PlayerPrefs.SetInt("InfernoDrive.SelectedMap", map); PlayerPrefs.Save();
				Click("PreviousMap"); Click("NextMap");   // net zero, but RefreshMap + ApplySelectedMap run
				step = 1; wait = 30;
			}
			else
			{
				Render(Path.Combine(Application.dataPath, "Resources/InfernoLobby/" + Slugs[map] + ".bytes"));
				map++; step = 0; wait = 5;
				if (map >= Slugs.Length) Finish();
			}
		}
		catch (System.Exception e) { Debug.LogError("[Thumbs] failed: " + e); Finish(); }
	}

	private static void Render(string path)
	{
		Camera cam = null;
		foreach (Camera c in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			if (c.gameObject.name == "Main Camera") cam = c;
		if (cam == null) { Debug.LogWarning("[Thumbs] no Main Camera"); return; }
		var rt = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32);
		RenderTexture previous = cam.targetTexture;
		cam.targetTexture = rt; cam.Render(); cam.targetTexture = previous;   // disabled cameras still render on demand
		var tex = new Texture2D(Width, Height, TextureFormat.RGB24, false);
		RenderTexture active = RenderTexture.active; RenderTexture.active = rt;
		tex.ReadPixels(new Rect(0, 0, Width, Height), 0, 0); tex.Apply(); RenderTexture.active = active;
		File.WriteAllBytes(path, tex.EncodeToPNG());
		Object.Destroy(tex); rt.Release(); Object.Destroy(rt);
		Debug.Log("[Thumbs] wrote " + path);
	}

	private static void Finish()
	{
		armed = false;
		EditorApplication.update -= Tick;
		for (int i = 0; i < Slugs.Length; i++)
		{
			if (unlocked[i]) PlayerPrefs.SetInt("InfernoDrive.MapUnlocked." + i, 1); else PlayerPrefs.DeleteKey("InfernoDrive.MapUnlocked." + i);
		}
		PlayerPrefs.SetInt("InfernoDrive.SelectedMap", savedMap); PlayerPrefs.Save();
		if (EditorApplication.isPlaying) { Click("PreviousMap"); Click("NextMap"); }
		EditorApplication.isPaused = false;
		AssetDatabase.Refresh();
		Debug.Log("[Thumbs] done");
	}
}
