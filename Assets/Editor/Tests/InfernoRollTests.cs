using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

// Random-driven rules: the traffic paint cycle (InfernoDriving.NextPaintStyle) and the case
// prize roll (ShadowRunPrototype.RollCasePrize). Both are replayed against the same seeded
// UnityEngine.Random stream so the thresholds themselves are what is checked.
// [user request 2026-09-21: cars] The vehicle branch now draws one more Random.value and hands it
// to InfernoCars.PickCasePrize (rarity-weighted over every locked catalogue car); the 6 % chance
// (CaseVehiclePrizeChance) and the diamond thresholds are the shipped ones.
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

	// The six non-cloud catalogue cars are locked for the roll (their PlayerPrefs flags are put back afterwards).
	private static readonly string[] NonCloudIds = { "amg_w222", "sportcar_rs", "amg_armour", "car3_clubsport", "infal_spyder", "flying_x" };
	private readonly Dictionary<string, int?> ownership = new Dictionary<string, int?>();

	[SetUp]
	public void LockTheCatalogue()
	{
		ownership.Clear();
		foreach (string id in NonCloudIds)
		{
			string key = ShadowRunPrototype.InfernoCars.PrefKey(id);
			ownership[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
			PlayerPrefs.DeleteKey(key);
		}
	}

	[TearDown]
	public void RestoreOwnership()
	{
		foreach (var kv in ownership) { if (kv.Value.HasValue) PlayerPrefs.SetInt(kv.Key, kv.Value.Value); else PlayerPrefs.DeleteKey(kv.Key); }
		PlayerPrefs.Save();
	}

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
	// original: the vehicle was locked[UnityEngine.Random.Range(0, lockedCount)] — uniform over the locked cars
	private static Prize Expected(int seed, int lockedCount)
	{
		UnityEngine.Random.InitState(seed);
		if (lockedCount > 0 && UnityEngine.Random.value < 0.06f)
			return new Prize { Kind = 1, VehicleId = ShadowRunPrototype.InfernoCars.PickCasePrize(UnityEngine.Random.value) };
		float roll = UnityEngine.Random.value;
		int amount = roll < 0.42f ? 2 : roll < 0.72f ? 3 : roll < 0.91f ? 5 : 10;
		return new Prize { Kind = 0, Diamonds = amount };
	}

	[Test]
	public void CasePrizeRollMatchesTheShippedThresholds()
	{
		Assert.AreEqual(0.06f, (float)typeof(ShadowRunPrototype).GetField("CaseVehiclePrizeChance", Any).GetRawConstantValue());
		// edit mode: no Awake, no UI — unless the Editor is in play mode, where the running game is used instead
		ShadowRunPrototype game = UnityEngine.Object.FindAnyObjectByType<ShadowRunPrototype>();
		GameObject host = null;
		if (game == null)
		{
			host = new GameObject("RollCasePrize test");
			game = host.AddComponent<ShadowRunPrototype>();
		}
		try
		{
			List<string> locked = LockedVehicles(game);
			CollectionAssert.AreEqual(ShadowRunPrototype.InfernoCars.LockedIds(), locked, "the case draws from every locked catalogue car");
			Assert.IsFalse(locked.Contains("sportcar"), "the starter is never a prize");
			int vehicles = 0, d2 = 0, d3 = 0, d5 = 0, d10 = 0;
			var tiers = new Dictionary<int, int>();
			for (int seed = 1; seed <= 4000; seed++)
			{
				Prize expected = Expected(seed, locked.Count);
				UnityEngine.Random.InitState(seed);
				Prize actual = Roll(game);
				Assert.AreEqual(expected.Kind, actual.Kind, "seed " + seed);
				if (expected.Kind == 1)
				{
					Assert.AreEqual(expected.VehicleId, actual.VehicleId, "seed " + seed);
					Assert.IsTrue(locked.Contains(actual.VehicleId), "seed " + seed + " prize " + actual.VehicleId + " is locked");
					int tier = (int)ShadowRunPrototype.InfernoCars.RarityOf(actual.VehicleId);
					int n; tiers.TryGetValue(tier, out n); tiers[tier] = n + 1;
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
			// rarity-weighted (40 / 30 / 20 / 10 over the tiers that have a locked car): the lowest present tier
			// is drawn clearly more often than the highest one (240-odd vehicle draws leave the middle tiers noisy)
			int lowest = -1, highest = -1;
			for (int tier = 0; tier < 4; tier++) if (tiers.ContainsKey(tier)) { if (lowest < 0) lowest = tier; highest = tier; }
			if (lowest >= 0 && highest > lowest) Assert.Greater(tiers[lowest], tiers[highest], "tier " + lowest + " vs " + highest);
		}
		finally
		{
			if (host != null) UnityEngine.Object.DestroyImmediate(host);
		}
	}
}
