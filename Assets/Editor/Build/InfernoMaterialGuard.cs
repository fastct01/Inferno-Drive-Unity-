using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// The game mutates *shared* materials at runtime (StyleVehicleForGarage / StyleGarageScene lerp the
// paint toward black, force _EMISSION, renderQueue 2000 and an opaque surface every time the garage
// preview is rebuilt).  A player build reloads the assets fresh on every launch, but in the editor
// those writes land on the .mat assets and get saved to disk, so repeated play sessions turned the
// car paint and glass black (tools/remake/restore_material_colours.py undid that).
//
// This guard gives the editor the player's semantics: every material asset is snapshotted when play
// mode starts, restored when it ends, and never saved while play mode is running.
[InitializeOnLoad]
public sealed class InfernoMaterialGuard : AssetModificationProcessor
{
	private static readonly Dictionary<string, string> Snapshot = new Dictionary<string, string>(StringComparer.Ordinal);
	private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>(StringComparer.Ordinal);

	static InfernoMaterialGuard()
	{
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		EditorApplication.quitting += Restore;
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange change)
	{
		if (change == PlayModeStateChange.ExitingEditMode) Take();
		else if (change == PlayModeStateChange.EnteredEditMode) Restore();
	}

	private static void Take()
	{
		Snapshot.Clear();
		Materials.Clear();
		string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
		for (int i = 0; i < guids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
			if (material == null) continue;
			Snapshot[path] = EditorJsonUtility.ToJson(material);
			Materials[path] = material;
		}
	}

	private static void Restore()
	{
		if (Snapshot.Count == 0) return;
		int restored = 0;
		foreach (KeyValuePair<string, string> entry in Snapshot)
		{
			Material material;
			if (!Materials.TryGetValue(entry.Key, out material) || material == null) continue;
			if (EditorJsonUtility.ToJson(material) == entry.Value)
			{
				continue;
			}
			EditorJsonUtility.FromJsonOverwrite(entry.Value, material);
			EditorUtility.ClearDirty(material);
			restored++;
		}
		Snapshot.Clear();
		Materials.Clear();
		if (restored > 0) Debug.Log("[InfernoMaterialGuard] restored " + restored + " material asset(s) mutated during play mode");
	}

	// Never write a snapshotted material to disk while play mode is running (the code coverage /
	// test runner packages and the editor's own quit path call SaveAssets).
	private static string[] OnWillSaveAssets(string[] paths)
	{
		if (Snapshot.Count == 0 || !EditorApplication.isPlayingOrWillChangePlaymode) return paths;
		var kept = new List<string>(paths.Length);
		for (int i = 0; i < paths.Length; i++)
		{
			if (Snapshot.ContainsKey(paths[i])) Debug.Log("[InfernoMaterialGuard] blocked saving " + paths[i] + " during play mode");
			else kept.Add(paths[i]);
		}
		return kept.ToArray();
	}
}
