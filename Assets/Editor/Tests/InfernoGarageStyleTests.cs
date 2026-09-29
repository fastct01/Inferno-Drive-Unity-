using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Cars = ShadowRunPrototype.InfernoCars;

// Catalogue rules of the garage: the ten InfernoCars models (four rarities, price order, PlayerPrefs
// ownership of the six non-cloud ids, rarity-weighted case prizes) and the InfernoGarage forwarders
// the generated code and the profile bridge still call.
// [user request 2026-09-21: cars] Replaces the 25-edition checks; the style catalogue tests that lived
// here moved to InfernoStyleTests.cs.
public class InfernoGarageStyleTests
{
	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	// InfernoGarage stays a private nested class, so its forwarders are driven through reflection.
	private static readonly Type GarageType = typeof(ShadowRunPrototype).GetNestedType("InfernoGarage", Any);
	private static object Call(string method, params object[] args) { return GarageType.GetMethod(method, Any).Invoke(null, args); }
	private static bool SameName(string s, string ascii) { return (bool)Call("SameName", s, ascii); }

	// GarageVehicleCardData is a nested class of the game: read through reflection like the smoke tests do.
	private static readonly Type CardType = typeof(ShadowRunPrototype).GetNestedType("GarageVehicleCardData", Any);
	private static object Field(object card, string name) { return CardType.GetField(name, Any).GetValue(card); }

	private static readonly string[] ExpectedIds =
	{
		"sportcar", "amg_w222", "car3", "sportcar_rs", "infal_model", "amg_armour", "car3_clubsport", "flying_car", "infal_spyder", "flying_x",
	};
	private static readonly string[] NonCloudIds = { "amg_w222", "sportcar_rs", "amg_armour", "car3_clubsport", "infal_spyder", "flying_x" };

	private readonly Dictionary<string, int?> snapshot = new Dictionary<string, int?>();

	[SetUp]
	public void SnapshotOwnership()
	{
		snapshot.Clear();
		foreach (string id in NonCloudIds)
		{
			string key = Cars.PrefKey(id);
			snapshot[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
			PlayerPrefs.DeleteKey(key);
		}
	}

	[TearDown]
	public void RestoreOwnership()
	{
		foreach (var kv in snapshot) { if (kv.Value.HasValue) PlayerPrefs.SetInt(kv.Key, kv.Value.Value); else PlayerPrefs.DeleteKey(kv.Key); }
		PlayerPrefs.Save();
	}

	[Test]
	public void CatalogueIsTheTenModelsInPriceOrder()
	{
		Assert.AreEqual(10, Cars.Count);
		CollectionAssert.AreEqual(ExpectedIds, Cars.Ids);
		Assert.AreEqual(ExpectedIds.Length, new HashSet<string>(Cars.Ids, StringComparer.OrdinalIgnoreCase).Count, "ids are unique");
		int previousPrice = -1;
		Cars.Rarity previousRarity = Cars.Rarity.Common;
		foreach (string id in Cars.Ids)
		{
			Assert.IsTrue(Cars.Contains(id));
			Assert.IsTrue(Cars.Contains(id.ToUpperInvariant()), "case-insensitive " + id);
			Assert.GreaterOrEqual(Cars.Price(id), previousPrice, "price order " + id);
			Assert.GreaterOrEqual((int)Cars.RarityOf(id), (int)previousRarity, "rarity order " + id);
			previousPrice = Cars.Price(id);
			previousRarity = Cars.RarityOf(id);
			Assert.IsFalse(string.IsNullOrEmpty(Cars.Name(id)));
			Assert.IsFalse(string.IsNullOrEmpty(Cars.Subtitle(id)));
			Assert.AreEqual(1, Cars.PaintMaterials(id).Length, "one paint material " + id);
		}
		Assert.AreEqual(0, Cars.Price("sportcar"));
		Assert.IsTrue(Cars.IsStarter("sportcar"));
		Assert.IsFalse(Cars.IsStarter("amg_w222"));
		Assert.AreEqual(120, Cars.Price("amg_w222"));
		Assert.AreEqual(300, Cars.Price("car3"));
		Assert.AreEqual(340, Cars.Price("sportcar_rs"));
		Assert.AreEqual(650, Cars.Price("infal_model"));
		Assert.AreEqual(700, Cars.Price("amg_armour"));
		Assert.AreEqual(750, Cars.Price("car3_clubsport"));
		Assert.AreEqual(1200, Cars.Price("flying_car"));
		Assert.AreEqual(1300, Cars.Price("infal_spyder"));
		Assert.AreEqual(1500, Cars.Price("flying_x"));
		Assert.AreEqual(-1, Cars.Price("car3_noir"));   // a retired edition
		Assert.IsFalse(Cars.Contains(null));
		Assert.AreEqual(Cars.Rarity.Common, Cars.RarityOf("amg_w222"));
		Assert.AreEqual(Cars.Rarity.Rare, Cars.RarityOf("sportcar_rs"));
		Assert.AreEqual(Cars.Rarity.Epic, Cars.RarityOf("car3_clubsport"));
		Assert.AreEqual(Cars.Rarity.Legendary, Cars.RarityOf("flying_x"));
		Assert.AreEqual("AMG SOVEREIGN", Cars.Name("amg_w222"));
		Assert.AreEqual("PHANTOM RS", Cars.Name("sportcar_rs"));
		Assert.AreEqual("AERO WRAITH X", Cars.Name("flying_x"));
		CollectionAssert.AreEqual(new[] { "COMMON", "RARE", "EPIC", "LEGENDARY" }, new[] { Cars.RarityName(Cars.Rarity.Common), Cars.RarityName(Cars.Rarity.Rare), Cars.RarityName(Cars.Rarity.Epic), Cars.RarityName(Cars.Rarity.Legendary) });
		Assert.AreEqual(new Color(0.62f, 0.68f, 0.78f, 1f), Cars.RarityColor(Cars.Rarity.Common));
		Assert.AreEqual(new Color(0.24f, 0.70f, 1f, 1f), Cars.RarityColor(Cars.Rarity.Rare));
		Assert.AreEqual(new Color(0.76f, 0.30f, 1f, 1f), Cars.RarityColor(Cars.Rarity.Epic));
		Assert.AreEqual(new Color(1f, 0.68f, 0.16f, 1f), Cars.RarityColor(Cars.Rarity.Legendary));
		/* [user request 2026-09-24: plain cars] */ // the editions are plain models with their own paint (no body kits): IsVariant, formerly HasKit
		Assert.IsTrue(Cars.IsVariant("sportcar_rs") && Cars.IsVariant("amg_armour") && Cars.IsVariant("car3_clubsport") && Cars.IsVariant("infal_spyder") && Cars.IsVariant("flying_x"));
		Assert.IsFalse(Cars.IsVariant("sportcar") || Cars.IsVariant("amg_w222") || Cars.IsVariant("car3") || Cars.IsVariant("infal_model") || Cars.IsVariant("flying_car"));
	}

	[Test]
	public void CardsFollowTheCatalogue()
	{
		Array cards = Cars.Cards(null);
		Assert.AreEqual(10, cards.Length);
		for (int i = 0; i < ExpectedIds.Length; i++)
		{
			object card = cards.GetValue(i);
			string id = ExpectedIds[i];
			Assert.AreEqual(id, Field(card, "VehicleId"));
			Assert.AreEqual(Cars.Name(id), Field(card, "Name"));
			Assert.AreEqual(Cars.Subtitle(id), Field(card, "Subtitle"));
			Assert.AreEqual(Cars.RarityName(Cars.RarityOf(id)), Field(card, "Rarity"));
			Assert.AreEqual(Cars.RarityColor(Cars.RarityOf(id)), (Color)Field(card, "Color"));
			Assert.AreEqual(Cars.CasePicture(id), Field(card, "IconResource"));
			float power = (float)Field(card, "Power");
			Assert.IsTrue(power > 0f && power <= 1f, "power bar " + id);
			Assert.IsTrue(((string)Field(card, "PowerValue")).EndsWith(" HP"));
		}
		Assert.AreEqual("sportcar photo", Field(cards.GetValue(0), "IconResource"));
		Assert.AreEqual("Data/Raw/InfernoLobby/cars/amg_w222.png", Field(cards.GetValue(1), "IconResource"));
		Assert.AreEqual("porsche photo", Field(cards.GetValue(2), "IconResource"));
		Assert.AreEqual("impala photo", Field(cards.GetValue(4), "IconResource"));
		Assert.AreEqual("flyingcar photo", Field(cards.GetValue(7), "IconResource"));
		Assert.AreEqual("Data/Raw/InfernoCars/sportcar_rs.png", Field(cards.GetValue(3), "IconResource"));
		Assert.AreEqual("sportcar_rs", Cars.IdFromPicture("Data/Raw/InfernoCars/sportcar_rs.png"));
		Assert.IsNull(Cars.IdFromPicture("Data/Raw/InfernoLobby/cars/amg_w222.png"));
		Assert.AreEqual("sportcar photo", Cars.FallbackPicture("sportcar_rs"));
		Assert.AreEqual("Data/Raw/InfernoLobby/cars/amg_w222.png", Cars.FallbackPicture("amg_armour"));
		Assert.IsFalse((bool)Field(cards.GetValue(0), "VisualLocked"), "the starter is owned");
		Assert.IsTrue((bool)Field(cards.GetValue(3), "VisualLocked"), "a variant without its pref is locked");
		// the old AppendCards hook passes the catalogue's cards through untouched
		Array appended = (Array)Call("AppendCards", null, cards);
		Assert.AreSame(cards, appended);
		Assert.AreEqual(0, ((Array)Call("AppendCards", null, null)).Length);
	}

	[Test]
	public void OwnershipLivesInPlayerPrefs()
	{
		string key = Cars.PrefKey("sportcar_rs");
		Assert.AreEqual("InfernoDrive.Vehicle.sportcar_rs.Unlocked", key);
		Assert.AreEqual(-1, Cars.IsUnlocked("sportcar"));          // the cloud ids are answered by SupabaseGameManager
		Assert.AreEqual(-1, Cars.IsUnlocked("flying_car"));
		Assert.AreEqual(-1, Cars.IsUnlocked("car3_noir"));         // retired edition
		Assert.AreEqual(-1, Cars.IsUnlocked(null));
		Assert.AreEqual(0, Cars.IsUnlocked("sportcar_rs"));
		Assert.AreEqual(0, Cars.IsUnlocked("SPORTCAR_RS"));        // case-insensitive match
		Assert.IsFalse(Cars.SetUnlocked("sportcar", true));
		Assert.IsFalse(Cars.SetUnlocked("car3_noir", true));
		Assert.IsTrue(Cars.SetUnlocked("sportcar_rs", true));
		Assert.AreEqual(1, PlayerPrefs.GetInt(key, 0));
		Assert.AreEqual(1, Cars.IsUnlocked("sportcar_rs"));
		Assert.AreEqual(1, (int)Call("IsUnlocked", "sportcar_rs"));   // the InfernoGarage forwarder the profile bridge uses
		Assert.IsFalse(Cars.LockedIds().Contains("sportcar_rs"));
		Assert.IsFalse(Cars.LockedIds().Contains("sportcar"));
		Assert.IsTrue(Cars.LockedIds().Contains("flying_x"));
		var locked = new List<string>();
		Call("AppendLockedCaseVehicles", null, locked);
		Assert.IsFalse(locked.Contains("sportcar_rs"));
		Assert.AreEqual(5, locked.Count, "the five other non-cloud cars");
		Assert.IsFalse(locked.Contains("flying_car"), "cloud ids are the caller's");
		Assert.IsTrue((bool)Call("SetUnlocked", "sportcar_rs", false));
		Assert.AreEqual(0, Cars.IsUnlocked("sportcar_rs"));
		// LockedIds keeps the price order and lists every locked catalogue car
		List<string> all = Cars.LockedIds();
		int previous = -1;
		foreach (string id in all) { int index = Array.IndexOf(ExpectedIds, id); Assert.Greater(index, previous, "price order " + id); previous = index; }
		foreach (string id in NonCloudIds) Assert.IsTrue(all.Contains(id), id + " locked");
	}

	[Test]
	public void CasePrizeIsRarityWeightedOverTheLockedCars()
	{
		List<string> locked = Cars.LockedIds();
		Assert.GreaterOrEqual(locked.Count, 6);
		int[] perTier = new int[4];
		foreach (string id in locked) perTier[(int)Cars.RarityOf(id)]++;
		float total = 0f;
		for (int t = 0; t < 4; t++) if (perTier[t] > 0) total += Cars.CaseWeights[t];
		var random = new System.Random(7);
		const int rolls = 40000;
		int[] hits = new int[4];
		var perCar = new Dictionary<string, int>();
		for (int i = 0; i < rolls; i++)
		{
			string id = Cars.PickCasePrize((float)random.NextDouble());
			Assert.IsTrue(locked.Contains(id), "prize " + id + " is locked");
			hits[(int)Cars.RarityOf(id)]++;
			int n; perCar.TryGetValue(id, out n); perCar[id] = n + 1;
		}
		for (int t = 0; t < 4; t++)
		{
			float expected = perTier[t] > 0 ? Cars.CaseWeights[t] / total : 0f;
			Assert.AreEqual(expected, hits[t] / (float)rolls, 0.02f, "tier " + t + " share");
			// uniform inside the tier
			foreach (string id in locked)
				if ((int)Cars.RarityOf(id) == t) Assert.AreEqual(expected / perTier[t], perCar[id] / (float)rolls, 0.02f, id + " share");
		}
		Assert.AreEqual(locked[0], Cars.PickCasePrize(0f));                    // the cheapest locked car opens the roll
		Assert.AreEqual(Cars.Rarity.Legendary, Cars.RarityOf(Cars.PickCasePrize(1f)));   // the roll's end is the top tier
		Assert.AreEqual(Cars.Rarity.Legendary, Cars.RarityOf(Cars.PickCasePrize(2f)));   // clamped
		CollectionAssert.AreEqual(new[] { 40, 30, 20, 10 }, Cars.CaseWeights);
	}

	[Test]
	public void CaseNamesAndPicturesComeFromTheCatalogue()
	{
		Assert.AreEqual("P900 SPYDER", Cars.CaseName("infal_spyder"));
		Assert.AreEqual("PHANTOM GT", Cars.CaseName("sportcar"));
		Assert.AreEqual("Data/Raw/InfernoCars/infal_spyder.png", Cars.CasePicture("infal_spyder"));
		Assert.AreEqual("flyingcar photo", Cars.CasePicture("flying_car"));
		Assert.IsNull(Cars.CaseName("infal_onyx"));
		Assert.IsNull(Cars.CasePicture(null));
		Assert.AreEqual("SOVEREIGN ARMOUR", (string)Call("CaseName", "amg_armour"));       // InfernoGarage forwards
		Assert.AreEqual("Data/Raw/InfernoCars/amg_armour.png", (string)Call("CasePicture", "amg_armour"));
		Assert.IsNull(Call("CaseName", "car3_noir"));
	}

	[Test]
	public void MaterialNameMatchingIgnoresCaseAndInstanceSuffix()
	{
		Assert.IsTrue(SameName("W222 - Car Color (Instance)", "w222 - car color"));
		Assert.IsTrue(SameName("Paint", "PAINT"));
		Assert.IsFalse(SameName("Paint2", "Paint"));
		Assert.IsFalse(SameName(null, "Paint"));
	}
}
