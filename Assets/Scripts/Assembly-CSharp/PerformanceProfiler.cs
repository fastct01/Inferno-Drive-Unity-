using System;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   PerformanceProfiler__ctor_mD9FDC09E94E2904C16F2D44D026329397B51DED6
//   PerformanceProfiler_Start_m484A638FC1026DCBD441A0CC94EE33C603BFF268
//   PerformanceProfiler_Update_m51446EBFF1419C9214D74F9B80E7E3CD9CBF6E68
//   PerformanceProfiler_OnGUI_m15BE0B25D0A618040EF26C64BD29131130FE5954
public class PerformanceProfiler : MonoBehaviour
{
	private float fps;

	private float frameTime;

	private int frameCount;

	private float lastTime;

	private GUIStyle guiStyle;

	private bool showStats = true;

	private float minFps = float.MaxValue;

	private float maxFps;

	private float avgFps;

	private void Start()
	{
		guiStyle = new GUIStyle(GUI.skin.label)
		{
			fontSize = 16,
			fontStyle = FontStyle.Bold,
			alignment = TextAnchor.UpperLeft
		};
		guiStyle.normal.textColor = Color.green;
		lastTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		frameCount++;
		float now = Time.realtimeSinceStartup;
		float elapsed = now - lastTime;
		if (elapsed >= 0.1f)
		{
			fps = frameCount / elapsed;
			frameTime = elapsed / frameCount * 1000f;
			if (fps < minFps)
			{
				minFps = fps;
			}
			if (fps > maxFps)
			{
				maxFps = fps;
			}
			avgFps = (avgFps + fps) / 2f;
			frameCount = 0;
			lastTime = now;
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			showStats = !showStats;
		}
	}

	private void OnGUI()
	{
		if (!showStats)
		{
			return;
		}
		GUI.backgroundColor = new Color(0f, 0f, 0f, 0.5f);
		string stats = string.Format("FPS: {0:F1}\nFrame Time: {1:F2}ms\nMin FPS: {2:F1}\nMax FPS: {3:F1}\nAvg FPS: {4:F1}\nUsed Memory: {5:F1}MB\nTotal Memory: {6:F1}MB\n\nPress 'P' to toggle stats", fps, frameTime, minFps, maxFps, avgFps, GC.GetTotalMemory(false) / 1024f / 1024f, SystemInfo.systemMemorySize);
		GUI.Box(new Rect(10f, 10f, 250f, 160f), stats, guiStyle);
		Color barColor = Color.green;
		if (fps < 30f)
		{
			barColor = Color.red;
		}
		else if (fps < 45f)
		{
			barColor = Color.yellow;
		}
		GUI.color = barColor;
		GUI.Box(new Rect(10f, 175f, 250f, 20f), "");
		GUI.color = Color.white;
	}
}
