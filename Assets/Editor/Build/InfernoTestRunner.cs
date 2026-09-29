using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

// Runs the NUnit tests inside the open Editor (no batch mode, no second Editor) and writes a
// plain-text summary to Logs/inferno-tests-<mode>.txt so a shell/MCP caller can wait for it:
//   InfernoTestRunner.Run(TestMode.EditMode, "InfernoCarsTests")   from Unity_RunCommand
//   menu: Inferno Drive > Run EditMode Tests / Run PlayMode Tests
// The callbacks are registered with [InitializeOnLoad] so they survive the domain reload that
// PlayMode tests trigger; the summary file is finished in RunFinished.
[InitializeOnLoad]
public static class InfernoTestRunner
{
	private const string Marker = "[InfernoTests]";

	static InfernoTestRunner()
	{
		ScriptableObject.CreateInstance<TestRunnerApi>().RegisterCallbacks(new Callbacks());
	}

	[MenuItem("Inferno Drive/Run EditMode Tests")]
	public static void RunEditMode() { Run(TestMode.EditMode, null); }

	[MenuItem("Inferno Drive/Run PlayMode Tests")]
	public static void RunPlayMode() { Run(TestMode.PlayMode, null); }

	// The layout audit only opens screens (no purchases, claims or case openings), so it may run signed in.
	[MenuItem("Inferno Drive/Run Layout Audit Test")]
	public static void RunLayoutAudit() { Run(TestMode.PlayMode, "LayoutAuditTests", true); }

	// The play-mode smoke tests drive the real economy (goal claims, a case opening, a garage purchase).
	// tools/remake/test.sh runs them in a fresh batch process with the Supabase session file moved aside,
	// so they only ever touch a guest profile. Inside the open Editor the Supabase client can still be
	// signed in from the user's own play session, and the tests would then spend the user's cloud
	// crystals (it happened once on 2026-09-22). So: no signed-in session, no play-mode run.
	public static bool PlayModeRunIsSafe(out string reason)
	{
		reason = null;
		if (ShadowRun.Services.SupabaseGameManager.IsAuthenticated)
		{
			reason = "the Supabase client is signed in (user " + ShadowRun.Services.SupabaseGameManager.CurrentUserId + ") — the smoke tests would change that cloud profile; sign out first or use tools/remake/test.sh PlayMode";
			return false;
		}
		string sessionFile = Path.Combine(Application.persistentDataPath, "supabase-session.json");
		if (File.Exists(sessionFile))
		{
			reason = "a persisted Supabase session exists (" + sessionFile + ") and the tests would restore it; move it aside first (tools/remake/test.sh does)";
			return false;
		}
		return true;
	}

	public static string SummaryPath(TestMode mode)
	{
		return Path.Combine(Path.GetDirectoryName(Application.dataPath), "Logs", "inferno-tests-" + mode.ToString().ToLowerInvariant() + ".txt");
	}

	// filter: null = every test of the mode; otherwise a NUnit full-name prefix / class name
	// (matches the -testFilter semantics loosely: any test whose full name contains it).
	public static void Run(TestMode mode, string filter) { Run(mode, filter, false); }

	public static void Run(TestMode mode, string filter, bool allowSignedIn)
	{
		string path = SummaryPath(mode);
		string unsafeReason;
		if (mode == TestMode.PlayMode && !allowSignedIn && !PlayModeRunIsSafe(out unsafeReason))
		{
			Debug.LogWarning(Marker + " refused PlayMode run: " + unsafeReason);
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, Marker + " refused PlayMode run: " + unsafeReason + "\n");
			return;
		}
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		if (File.Exists(path)) File.Delete(path);
		File.WriteAllText(path + ".running", DateTime.Now.ToString("s") + " " + mode + " " + (filter ?? "*") + "\n");
		var api = ScriptableObject.CreateInstance<TestRunnerApi>();
		var f = new Filter { testMode = mode };
		if (!string.IsNullOrEmpty(filter)) f.groupNames = new[] { filter };
		api.Execute(new ExecutionSettings(f));
		Debug.Log(Marker + " started " + mode + " filter=" + (filter ?? "*"));
	}

	private sealed class Callbacks : ICallbacks
	{
		private readonly StringBuilder lines = new StringBuilder();
		private int passed, failed, skipped;

		public void RunStarted(ITestAdaptor testsToRun)
		{
			lines.Length = 0; passed = failed = skipped = 0;
		}

		public void RunFinished(ITestResultAdaptor result)
		{
			TestMode mode = result.Test.TestMode;
			string summary = string.Format("{0} finished {1}: {2} passed, {3} failed, {4} skipped ({5})", Marker, mode, passed, failed, skipped, result.ResultState);
			lines.Insert(0, summary + "\n");
			string path = SummaryPath(mode);
			try
			{
				File.WriteAllText(path, lines.ToString());
				if (File.Exists(path + ".running")) File.Delete(path + ".running");
			}
			catch (Exception e) { Debug.LogWarning(Marker + " could not write " + path + ": " + e.Message); }
			if (failed > 0) Debug.LogWarning(summary); else Debug.Log(summary);
		}

		public void TestStarted(ITestAdaptor test) { }

		public void TestFinished(ITestResultAdaptor result)
		{
			if (result.Test.IsSuite) return;
			switch (result.TestStatus)
			{
				case TestStatus.Passed: passed++; break;
				case TestStatus.Failed: failed++; break;
				default: skipped++; break;
			}
			lines.Append(result.TestStatus).Append(' ').Append(result.Test.FullName).Append(' ').Append(result.Duration.ToString("0.00")).Append("s\n");
			if (result.TestStatus == TestStatus.Failed)
			{
				lines.Append("    ").Append((result.Message ?? "").Replace("\n", "\n    ")).Append('\n');
				string stack = result.StackTrace ?? "";
				if (stack.Length > 1200) stack = stack.Substring(0, 1200) + "…";
				lines.Append("    ").Append(stack.Replace("\n", "\n    ")).Append('\n');
			}
		}
	}
}
