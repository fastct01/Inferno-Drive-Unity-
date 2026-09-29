using System;
using System.Collections.Generic;
using NUnit.Framework;
using ShadowRun.Services;
using UnityEngine;
using Cars = ShadowRunPrototype.InfernoCars;
using Upgrades = ShadowRunPrototype.InfernoUpgrades;

// [user request 2026-09-21: cars] The per-car upgrade table: PlayerPrefs levels, the rarity price ladder,
// the arcade multipliers and speed bonuses UpdatePlayer reads, and Buy's crystal bookkeeping.
public class InfernoUpgradesTests
{
	private static readonly string[] EconomyKeys = { "shadowRunCrystals", "shadowRunPendingCrystalDelta", "shadowRunCloudDirty", "shadowRunCar1Unlocked", "shadowRunCar2Unlocked", "shadowRunCar3Unlocked", "shadowRunCar4Unlocked" };
	private readonly Dictionary<string, int?> snapshot = new Dictionary<string, int?>();
	private string profileId;

	private static IEnumerable<string> UpgradeKeys()
	{
		foreach (string id in Cars.Ids)
			for (int s = 0; s < Upgrades.StatCount; s++) yield return Upgrades.Key(id, (Upgrades.Stat)s);
	}

	[SetUp]
	public void Snapshot()
	{
		snapshot.Clear();
		foreach (string key in UpgradeKeys()) { snapshot[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null; PlayerPrefs.DeleteKey(key); }
		foreach (string key in EconomyKeys) snapshot[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
		profileId = PlayerPrefs.GetString("shadowRunProfileId", null);
	}

	[TearDown]
	public void Restore()
	{
		foreach (var kv in snapshot) { if (kv.Value.HasValue) PlayerPrefs.SetInt(kv.Key, kv.Value.Value); else PlayerPrefs.DeleteKey(kv.Key); }
		if (profileId != null) PlayerPrefs.SetString("shadowRunProfileId", profileId); else PlayerPrefs.DeleteKey("shadowRunProfileId");
		PlayerPrefs.Save();
	}

	[Test]
	public void PriceLadderFollowsTheRarity()
	{
		CollectionAssert.AreEqual(new[] { 40, 60, 80, 100, 120 }, Ladder(Cars.Rarity.Common));
		CollectionAssert.AreEqual(new[] { 60, 90, 120, 150, 180 }, Ladder(Cars.Rarity.Rare));
		CollectionAssert.AreEqual(new[] { 90, 135, 180, 225, 270 }, Ladder(Cars.Rarity.Epic));
		CollectionAssert.AreEqual(new[] { 130, 195, 260, 325, 390 }, Ladder(Cars.Rarity.Legendary));
		Assert.AreEqual(-1, Upgrades.PriceAt(Cars.Rarity.Common, Upgrades.MaxLevel));
		Assert.AreEqual(-1, Upgrades.PriceAt(Cars.Rarity.Legendary, -1));
		Assert.AreEqual(40, Upgrades.BasePrice(Cars.Rarity.Common));
		Assert.AreEqual(130, Upgrades.BasePrice(Cars.Rarity.Legendary));
		Assert.AreEqual(5, Upgrades.MaxLevel);
		Assert.AreEqual(3, Upgrades.StatCount);
		Assert.AreEqual(60, Upgrades.Price("sportcar_rs", Upgrades.Stat.Engine));    // Rare, level 0
		Assert.AreEqual(130, Upgrades.Price("flying_x", Upgrades.Stat.Nitro));      // Legendary, level 0
		Assert.AreEqual(-1, Upgrades.Price("car3_noir", Upgrades.Stat.Engine));     // not in the catalogue
		PlayerPrefs.SetInt(Upgrades.Key("infal_model", Upgrades.Stat.Handling), 2);
		Assert.AreEqual(180, Upgrades.Price("infal_model", Upgrades.Stat.Handling)); // Epic, level 2
		PlayerPrefs.SetInt(Upgrades.Key("infal_model", Upgrades.Stat.Handling), 5);
		Assert.AreEqual(-1, Upgrades.Price("infal_model", Upgrades.Stat.Handling));  // maxed
	}

	private static int[] Ladder(Cars.Rarity rarity)
	{
		int[] prices = new int[Upgrades.MaxLevel];
		for (int level = 0; level < Upgrades.MaxLevel; level++) prices[level] = Upgrades.PriceAt(rarity, level);
		return prices;
	}

	[Test]
	public void LevelsLiveInPlayerPrefs()
	{
		Assert.AreEqual("InfernoDrive.Upgrade.amg_armour.Nitro", Upgrades.Key("amg_armour", Upgrades.Stat.Nitro));
		Assert.AreEqual("InfernoDrive.Upgrade.sportcar.Engine", Upgrades.Key("sportcar", Upgrades.Stat.Engine));
		CollectionAssert.AreEqual(new[] { "ENGINE", "HANDLING", "NITRO" }, new[] { Upgrades.StatName(Upgrades.Stat.Engine), Upgrades.StatName(Upgrades.Stat.Handling), Upgrades.StatName(Upgrades.Stat.Nitro) });
		Assert.AreEqual(0, Upgrades.Level("sportcar", Upgrades.Stat.Engine));
		Assert.AreEqual(0, Upgrades.Level(null, Upgrades.Stat.Engine));
		PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Engine), 3);
		PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Handling), 9);   // clamped to the max
		PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Nitro), -2);    // and to zero
		Assert.AreEqual(3, Upgrades.Level("sportcar", Upgrades.Stat.Engine));
		Assert.AreEqual(5, Upgrades.Level("sportcar", Upgrades.Stat.Handling));
		Assert.AreEqual(0, Upgrades.Level("sportcar", Upgrades.Stat.Nitro));
		Assert.AreEqual(8, Upgrades.TotalLevels("sportcar"));
		Assert.AreEqual(0, Upgrades.TotalLevels("flying_x"));
	}

	[Test]
	public void MultipliersAndBonusesPerLevel()
	{
		Assert.AreEqual(1f, Upgrades.AccelMultiplier("sportcar"), 0.0001f);
		Assert.AreEqual(1f, Upgrades.SteerMultiplier("sportcar"), 0.0001f);
		Assert.AreEqual(1f, Upgrades.GripMultiplier("sportcar"), 0.0001f);
		Assert.AreEqual(1f, Upgrades.BoostRateMultiplier("sportcar"), 0.0001f);
		Assert.AreEqual(0f, Upgrades.TopSpeedBonus("sportcar"), 0.0001f);
		Assert.AreEqual(0f, Upgrades.BoostCapBonus("sportcar"), 0.0001f);
		PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Engine), 3);
		PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Handling), 2);
		PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Nitro), 5);
		Assert.AreEqual(1.24f, Upgrades.AccelMultiplier("sportcar"), 0.0001f);     // 1 + 0.08 x 3
		Assert.AreEqual(3.6f, Upgrades.TopSpeedBonus("sportcar"), 0.0001f);        // 1.2 m/s x 3
		Assert.AreEqual(1.12f, Upgrades.SteerMultiplier("sportcar"), 0.0001f);     // 1 + 0.06 x 2
		Assert.AreEqual(1.10f, Upgrades.GripMultiplier("sportcar"), 0.0001f);      // 1 + 0.05 x 2
		Assert.AreEqual(1.35f, Upgrades.BoostRateMultiplier("sportcar"), 0.0001f); // 1 + 0.07 x 5
		Assert.AreEqual(5f, Upgrades.BoostCapBonus("sportcar"), 0.0001f);          // 1.0 m/s x 5
		Assert.AreEqual(1f, Upgrades.AccelMultiplier("amg_w222"), 0.0001f);        // per car
		Assert.AreEqual(1f, Upgrades.AccelMultiplier(null), 0.0001f);
	}

	[Test]
	public void BuySpendsCrystalsAndRaisesTheLevel()
	{
		int before = SupabaseGameManager.Crystals;
		Assert.AreNotEqual(int.MaxValue, before, "a local profile answers the balance");
		try
		{
			Assert.IsFalse(Upgrades.Buy(null, "car3_noir", Upgrades.Stat.Engine), "unknown car");
			PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Engine), Upgrades.MaxLevel);
			Assert.IsFalse(Upgrades.Buy(null, "sportcar", Upgrades.Stat.Engine), "maxed");
			PlayerPrefs.DeleteKey(Upgrades.Key("sportcar", Upgrades.Stat.Engine));
			SupabaseGameManager.SetCrystals(30);
			Assert.IsFalse(Upgrades.Buy(null, "sportcar", Upgrades.Stat.Engine), "40 crystals needed, 30 held");
			Assert.AreEqual(30, SupabaseGameManager.Crystals);
			Assert.AreEqual(0, Upgrades.Level("sportcar", Upgrades.Stat.Engine));
			SupabaseGameManager.SetCrystals(150);
			Assert.IsTrue(Upgrades.Buy(null, "sportcar", Upgrades.Stat.Engine));    // Common level 0: 40
			Assert.AreEqual(110, SupabaseGameManager.Crystals);
			Assert.AreEqual(1, Upgrades.Level("sportcar", Upgrades.Stat.Engine));
			Assert.AreEqual(1, PlayerPrefs.GetInt(Upgrades.Key("sportcar", Upgrades.Stat.Engine), 0));
			Assert.IsTrue(Upgrades.Buy(null, "sportcar", Upgrades.Stat.Engine));    // level 1: 60
			Assert.AreEqual(50, SupabaseGameManager.Crystals);
			Assert.AreEqual(2, Upgrades.Level("sportcar", Upgrades.Stat.Engine));
			Assert.AreEqual(80, Upgrades.Price("sportcar", Upgrades.Stat.Engine));
			Assert.IsFalse(Upgrades.Buy(null, "sportcar", Upgrades.Stat.Engine), "level 2 costs 80, 50 held");
			Assert.AreEqual(0, Upgrades.Level("sportcar", Upgrades.Stat.Handling), "other stats untouched");
		}
		finally
		{
			SupabaseGameManager.SetCrystals(before);
		}
	}
}
