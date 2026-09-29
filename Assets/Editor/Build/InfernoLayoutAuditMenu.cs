using UnityEditor;
using UnityEngine;

// [user request 2026-09-21: overlaps] Inferno Drive > Audit Layout: in play mode, reports every
// sibling overlap and every text that leaves its parent on the screens currently shown
// (ShadowRunPrototype.InfernoLayoutAudit), one line per finding with a [LayoutAudit] prefix.
public static class InfernoLayoutAuditMenu
{
	[MenuItem("Inferno Drive/Audit Layout")]
	public static void Audit()
	{
		if (!EditorApplication.isPlaying)
		{
			Debug.LogWarning("[LayoutAudit] enter play mode first (Inferno Drive > Play (portrait)); the audit measures the built UI.");
			return;
		}
		ShadowRunPrototype game = Object.FindFirstObjectByType<ShadowRunPrototype>();
		if (game == null)
		{
			Debug.LogWarning("[LayoutAudit] no ShadowRunPrototype in the scene.");
			return;
		}
		string report = ShadowRunPrototype.InfernoLayoutAudit.ReportAll(game);
		if (string.IsNullOrEmpty(report))
		{
			Debug.Log("[LayoutAudit] clean: no overlaps, no text outside its parent.");
			return;
		}
		string[] lines = report.Split('\n');
		Debug.LogWarning("[LayoutAudit] " + lines.Length + " finding(s):\n" + report);
		for (int i = 0; i < lines.Length; i++) Debug.Log("[LayoutAudit] " + lines[i]);
	}
}
