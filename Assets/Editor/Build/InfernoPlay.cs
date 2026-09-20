using System;
using UnityEditor;
using UnityEngine;

// Opens the Game view in portrait (1080x1920) and enters play mode:
//   Unity -projectPath . -executeMethod InfernoPlay.Play        (with the editor window)
// Also available from the menu: Inferno Drive > Play (portrait).
public static class InfernoPlay
{
	[MenuItem("Inferno Drive/Play (portrait)")]
	public static void Play()
	{
		EditorApplication.delayCall += () =>
		{
			SetPortraitGameView();
			EditorApplication.delayCall += () => { EditorApplication.isPlaying = true; };
		};
	}

	private static void SetPortraitGameView()
	{
		try
		{
			var editor = typeof(Editor).Assembly;
			Type gameViewType = editor.GetType("UnityEditor.GameView");
			Type sizesType = editor.GetType("UnityEditor.GameViewSizes");
			Type sizeType = editor.GetType("UnityEditor.GameViewSize");
			Type sizeKind = editor.GetType("UnityEditor.GameViewSizeType");
			object instance = typeof(ScriptableSingleton<>).MakeGenericType(sizesType).GetProperty("instance").GetValue(null);
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
			var window = EditorWindow.GetWindow(gameViewType);
			window.Show();
			window.Focus();
			gameViewType.GetMethod("SizeSelectionCallback", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public).Invoke(window, new object[] { index, null });
		}
		catch (Exception e)
		{
			Debug.Log("[InfernoPlay] game view size not set: " + e.Message);
		}
	}
}
