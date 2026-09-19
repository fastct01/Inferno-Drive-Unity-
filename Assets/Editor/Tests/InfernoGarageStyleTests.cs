using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

// Catalogue rules of InfernoGarage.inc (25 paint editions, PlayerPrefs ownership) and
// InfernoStyle.inc (tires / flags / trails prices).
public class InfernoGarageStyleTests
{
	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	// InfernoGarage stays a private nested class (its AppendCards signature names the game's private
	// GarageVehicleCardData), so it is driven through reflection.
	private static readonly Type GarageType = typeof(ShadowRunPrototype).GetNestedType("InfernoGarage", Any);
	private static object Call(string method, params object[] args) { return GarageType.GetMethod(method, Any).Invoke(null, args); }
	private static int IsUnlocked(string id) { return (int)Call("IsUnlocked", id); }
	private static bool SetUnlocked(string id, bool on) { return (bool)Call("SetUnlocked", id, on); }
	private static string CaseName(string id) { return (string)Call("CaseName", id); }
	private static string CasePicture(string id) { return (string)Call("CasePicture", id); }
	private static bool SameName(string s, string ascii) { return (bool)Call("SameName", s, ascii); }

	// GarageVehicleCardData is a private nested class of the game: read through reflection.
	private static readonly Type CardType = typeof(ShadowRunPrototype).GetNestedType("GarageVehicleCardData", Any);
	private static string Field(object card, string name) { return (string)CardType.GetField(name, Any).GetValue(card); }
	private static Array AppendCards(Array cards)
	{
		return (Array)Call("AppendCards", null, cards);
	}

	[Test]
	public void EditionsAreTheTwentyFiveShippedOnes()
	{
		string[] expected =
		{
			"amg_w222", "car3_gt4rs", "infal_aurum", "sportcar_stealth", "flying_neon",
			"sportcar_inferno", "sportcar_arctic", "sportcar_venom", "sportcar_royal",
			"car3_glacier", "car3_noir", "car3_sunset", "car3_racing",
			"infal_crimson", "infal_cobalt", "infal_onyx", "infal_titan",
			"flying_cyan", "flying_ember", "flying_ghost", "flying_void",
			"amg_obsidian", "amg_bordeaux", "amg_emerald", "amg_pearl",
		};
		Array cards = AppendCards(null);
		Assert.AreEqual(25, cards.Length);
		for (int i = 0; i < expected.Length; i++) Assert.AreEqual(expected[i], Field(cards.GetValue(i), "VehicleId"));
		object first = cards.GetValue(0);
		Assert.AreEqual("AMG SOVEREIGN", Field(first, "Name"));
		Assert.AreEqual("Executive Saloon / Class A", Field(first, "Subtitle"));
		Assert.AreEqual("EPIC", Field(first, "Rarity"));
		Assert.AreEqual("Data/Raw/InfernoLobby/cars/amg_w222.png", Field(first, "IconResource"));
		Assert.AreEqual("800 HP", Field(first, "PowerValue"));
		Assert.AreEqual("290 KM/H", Field(first, "SpeedValue"));
		Assert.AreEqual("70%", Field(first, "HandlingValue"));
		Assert.AreEqual("P900 ONYX", CaseName("infal_onyx"));
		Assert.AreEqual("Data/Raw/InfernoLobby/cars/infal_onyx.png", CasePicture("infal_onyx"));
		Assert.IsNull(CaseName("sportcar"));
		// the original four cards are kept in front of the editions
		object card = Activator.CreateInstance(CardType, true);
		CardType.GetField("VehicleId", Any).SetValue(card, "sportcar");
		Array original = Array.CreateInstance(CardType, 1);
		original.SetValue(card, 0);
		Array appended = AppendCards(original);
		Assert.AreEqual(26, appended.Length);
		Assert.AreSame(card, appended.GetValue(0));
	}

	[Test]
	public void OwnershipLivesInPlayerPrefs()
	{
		const string key = "InfernoDrive.Vehicle.car3_noir.Unlocked";
		PlayerPrefs.DeleteKey(key);
		Assert.AreEqual(-1, IsUnlocked("sportcar"));          // not one of ours
		Assert.AreEqual(-1, IsUnlocked(null));
		Assert.AreEqual(0, IsUnlocked("car3_noir"));
		Assert.AreEqual(0, IsUnlocked("CAR3_NOIR"));           // case-insensitive match
		Assert.IsFalse(SetUnlocked("sportcar", true));
		Assert.IsTrue(SetUnlocked("car3_noir", true));
		Assert.AreEqual(1, PlayerPrefs.GetInt(key, 0));
		Assert.AreEqual(1, IsUnlocked("car3_noir"));
		var locked = new List<string>();
		Call("AppendLockedCaseVehicles", null, locked);
		Assert.IsFalse(locked.Contains("car3_noir"));
		Assert.AreEqual(24, locked.Count);
		Assert.IsTrue(SetUnlocked("car3_noir", false));
		Assert.AreEqual(0, IsUnlocked("car3_noir"));
		PlayerPrefs.DeleteKey(key);
	}

	[Test]
	public void MaterialNameMatchingIgnoresCaseAndInstanceSuffix()
	{
		Assert.IsTrue(SameName("W222 - Car Color (Instance)", "w222 - car color"));
		Assert.IsTrue(SameName("Paint", "PAINT"));
		Assert.IsFalse(SameName("Paint2", "Paint"));
		Assert.IsFalse(SameName(null, "Paint"));
	}

	private static (string name, int price)[] Catalogue(string field)
	{
		var t = typeof(ShadowRunPrototype).GetNestedType("InfernoStyle", Any);
		Array items = (Array)t.GetField(field, Any).GetValue(null);
		var result = new (string, int)[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			object item = items.GetValue(i);
			Type it = item.GetType();
			result[i] = ((string)it.GetField("name", Any).GetValue(item), (int)it.GetField("price", Any).GetValue(item));
		}
		return result;
	}

	[Test]
	public void StyleCataloguePrices()
	{
		CollectionAssert.AreEqual(new[]
		{
			("STOCK", 0), ("CHROME", 40), ("GOLD", 60), ("BLACKOUT", 60),
			("NEON VIOLET", 90), ("NEON CYAN", 90), ("CRIMSON", 90), ("WHITE WALL", 70),
		}, Catalogue("TireItems"));
		CollectionAssert.AreEqual(new[]
		{
			("NONE", 0), ("CHECKERED", 50), ("STRIPES", 50), ("NEON", 80), ("PIRATE", 90), ("SUNBURST", 70), ("BOLT", 70),
		}, Catalogue("FlagItems"));
		CollectionAssert.AreEqual(new[]
		{
			("NONE", 0), ("VIOLET NEON", 50), ("CYAN NEON", 50), ("FIRE", 70), ("EMERALD", 70), ("MAGENTA", 70), ("GOLD", 90),
		}, Catalogue("TrailItems"));
	}
}
