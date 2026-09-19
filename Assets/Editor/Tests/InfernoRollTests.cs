using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

// Random-driven rules: the traffic paint cycle (InfernoDriving.NextPaintStyle) and the case
// prize roll (ShadowRunPrototype.RollCasePrize). Both are replayed against the same seeded
// UnityEngine.Random stream so the thresholds themselves are what is checked.
public class InfernoRollTests
{
	[Test]
	public void PaintCycleUsesOnlyTheThreeDarkPaintsAndNeverRepeats()
	{
		// TRAFFIC_PAINTS: navy (0), dark red (4), dark green (3); style 1/2 never chosen.
		var allowed = new HashSet<int> { 0, 4, 3 };
		var seen = new HashSet<int>();
		UnityEngine.Random.InitState(1234);
		int previous = ShadowRunPrototype.InfernoDriving.NextPaintStyle();
		Assert.IsTrue(allowed.Contains(previous));
		seen.Add(previous);
		for (int i = 0; i < 1000; i++)
		{
			int next = ShadowRunPrototype.InfernoDriving.NextPaintStyle();
			Assert.IsTrue(allowed.Contains(next), "style " + next);
			Assert.AreNotEqual(previous, next, "consecutive repeat at draw " + i);
			seen.Add(next);
			previous = next;
		}
		Assert.AreEqual(3, seen.Count);
	}

	[Test]
	public void PaintCycleIsUniformOverTheOtherTwoPaints()
	{
		UnityEngine.Random.InitState(99);
		var counts = new Dictionary<int, int> { { 0, 0 }, { 4, 0 }, { 3, 0 } };
		for (int i = 0; i < 30000; i++) counts[ShadowRunPrototype.InfernoDriving.NextPaintStyle()]++;
		foreach (var kv in counts) Assert.IsTrue(kv.Value > 9000 && kv.Value < 11000, "paint " + kv.Key + " drawn " + kv.Value);
	}

	// ---- case prize roll ---------------------------------------------------------------------
	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	private struct Prize { public int Kind; public int Diamonds; public string VehicleId; }

	private static Prize Roll(ShadowRunPrototype game)
	{
		object prize = typeof(ShadowRunPrototype).GetMethod("RollCasePrize", Any).Invoke(game, null);
		Type t = prize.GetType();
		return new Prize
		{
			Kind = (int)t.GetField("Kind", Any).GetValue(prize),
			Diamonds = (int)t.GetField("Diamonds", Any).GetValue(prize),
			VehicleId = (string)t.GetField("VehicleId", Any).GetValue(prize),
		};
	}

	private static List<string> LockedVehicles(ShadowRunPrototype game)
	{
		return (List<string>)typeof(ShadowRunPrototype).GetMethod("GetLockedCaseVehicles", Any).Invoke(game, null);
	}

	// The shipped roll: 6 % vehicle (when any is locked), else 42 % +2, 30 % +3, 19 % +5, 9 % +10.
	private static Prize Expected(int seed, int lockedCount)
	{
		UnityEngine.Random.InitState(seed);
		if (lockedCount > 0 && UnityEngine.Random.value < 0.06f)
			return new Prize { Kind = 1, VehicleId = "#" + UnityEngine.Random.Range(0, lockedCount) };
		float roll = UnityEngine.Random.value;
		int amount = roll < 0.42f ? 2 : roll < 0.72f ? 3 : roll < 0.91f ? 5 : 10;
		return new Prize { Kind = 0, Diamonds = amount };
	}

	[Test]
	public void CasePrizeRollMatchesTheShippedThresholds()
	{
		Assert.AreEqual(0.06f, (float)typeof(ShadowRunPrototype).GetField("CaseVehiclePrizeChance", Any).GetRawConstantValue());
		var host = new GameObject("RollCasePrize test");
		try
		{
			var game = host.AddComponent<ShadowRunPrototype>();   // edit mode: no Awake, no UI
			List<string> locked = LockedVehicles(game);
			int vehicles = 0, d2 = 0, d3 = 0, d5 = 0, d10 = 0;
			for (int seed = 1; seed <= 4000; seed++)
			{
				Prize expected = Expected(seed, locked.Count);
				UnityEngine.Random.InitState(seed);
				Prize actual = Roll(game);
				Assert.AreEqual(expected.Kind, actual.Kind, "seed " + seed);
				if (expected.Kind == 1)
				{
					int index = int.Parse(expected.VehicleId.Substring(1));
					Assert.AreEqual(locked[index], actual.VehicleId, "seed " + seed);
					vehicles++;
				}
				else
				{
					Assert.AreEqual(expected.Diamonds, actual.Diamonds, "seed " + seed);
					if (actual.Diamonds == 2) d2++; else if (actual.Diamonds == 3) d3++; else if (actual.Diamonds == 5) d5++; else d10++;
				}
			}
			if (locked.Count > 0) Assert.IsTrue(vehicles > 4000 * 0.04f && vehicles < 4000 * 0.08f, "vehicle prizes " + vehicles);
			Assert.IsTrue(d2 > d3 && d3 > d5 && d5 > d10, d2 + "/" + d3 + "/" + d5 + "/" + d10);
		}
		finally
		{
			UnityEngine.Object.DestroyImmediate(host);
		}
	}
}
