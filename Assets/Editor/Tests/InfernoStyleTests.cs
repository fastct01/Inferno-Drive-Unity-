using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

// Catalogue and persistence rules of InfernoStyle (Inferno/InfernoStyle.cs): the four
// categories' items and prices, the free first item of every category, and the paint
// choice that is owned once but selected per car (InfernoDrive.Paint.<carId>.Selected).
/* [user request 2026-09-21: style] */
public class InfernoStyleTests
{
	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	// InfernoStyle is an internal nested class of the game: driven through reflection like the garage tests.
	private static readonly Type StyleType = typeof(ShadowRunPrototype).GetNestedType("InfernoStyle", Any);
	private static object Call(string method, params object[] args) { return StyleType.GetMethod(method, Any).Invoke(null, args); }
	private static object Field(object o, string name) { return o.GetType().GetField(name, Any).GetValue(o); }
	private static int Const(string name) { return (int)StyleType.GetField(name, Any).GetValue(null); }
	private static bool Owned(int category, int index) { return (bool)Call("Owned", category, index); }
	private static object SelectedPaint(string carId) { return Call("SelectedPaint", new object[] { carId }); }
	private static bool IsSet(object choice) { return (bool)Field(choice, "set"); }

	private static (string name, int price)[] Catalogue(string field)
	{
		Array items = (Array)StyleType.GetField(field, Any).GetValue(null);
		var result = new (string, int)[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			object item = items.GetValue(i);
			result[i] = ((string)Field(item, "name"), (int)Field(item, "price"));
		}
		return result;
	}

	private static float[] Numbers(string field, int index, string member)
	{
		Array items = (Array)StyleType.GetField(field, Any).GetValue(null);
		return (float[])Field(items.GetValue(index), member);
	}

	[Test]
	public void CategoryConstants()
	{
		Assert.AreEqual(0, Const("Tires"));
		Assert.AreEqual(1, Const("Flags"));
		Assert.AreEqual(2, Const("Trails"));
		Assert.AreEqual(3, Const("Paint"));
		Assert.AreEqual(4, Const("CategoryCount"));
		string[] keys = (string[])StyleType.GetField("CategoryKeys", Any).GetValue(null);
		CollectionAssert.AreEqual(new[] { "Tires", "Flags", "Trails", "Paint" }, keys);
	}

	[Test]
	public void TireFlagTrailCataloguePrices()
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

	[Test]
	public void PaintCatalogue()
	{
		CollectionAssert.AreEqual(new[]
		{
			("STOCK", 0), ("MIDNIGHT", 60), ("ARCTIC", 60), ("GUNMETAL", 70), ("CRIMSON", 80), ("INFERNO", 80),
			("ELECTRIC BLUE", 90), ("VIOLET", 90), ("ACID", 90), ("MAGENTA", 100), ("GOLD", 120), ("PEARL", 120), ("CHROME", 150),
		}, Catalogue("PaintItems"));
		// b = { metallic, smoothness }: the two metallic finishes, every value a valid PBR input
		Assert.AreEqual(1.0f, Numbers("PaintItems", 12, "b")[0], 1e-6f, "CHROME is fully metallic");
		Assert.GreaterOrEqual(Numbers("PaintItems", 10, "b")[0], 0.85f, "GOLD is metallic");
		for (int i = 1; i < 13; i++)
		{
			float[] pbr = Numbers("PaintItems", i, "b");
			Assert.That(pbr[0], Is.InRange(0f, 1f), "metallic " + i);
			Assert.That(pbr[1], Is.InRange(0f, 1f), "smoothness " + i);
			float[] colour = Numbers("PaintItems", i, "a");
			for (int k = 0; k < 3; k++) Assert.That(colour[k], Is.InRange(0f, 1f), "colour " + i);
		}
	}

	[Test]
	public void FirstItemOfEveryCategoryIsFreeAndOwned()
	{
		string[] fields = { "TireItems", "FlagItems", "TrailItems", "PaintItems" };
		for (int category = 0; category < 4; category++)
		{
			Assert.AreEqual(0, Catalogue(fields[category])[0].price, fields[category] + "[0] is free");
			Assert.IsTrue(Owned(category, 0), fields[category] + "[0] is owned without a purchase");
		}
		Assert.AreEqual(13, (int)Call("Count", 3));
		Assert.AreEqual(2, (int)Call("Pages", 3), "PAINT pages at 8 per page");
		Assert.AreEqual(1, (int)Call("Pages", 0), "TIRES fit on one page");
	}

	[Test]
	public void OwnershipAndSelectionKeys()
	{
		Assert.AreEqual("InfernoDrive.Style.Paint.5.Owned", (string)Call("Key", 3, "Owned", 5));
		Assert.AreEqual("InfernoDrive.Style.Tires.Selected", (string)Call("Key", 0, "Selected", -1));
		Assert.AreEqual("InfernoDrive.Paint.sportcar.Selected", (string)Call("PaintKey", "sportcar"));
		Assert.AreEqual("InfernoDrive.Paint.amg_w222.Selected", (string)Call("PaintKey", "AMG_W222"), "ids compare case-insensitively in the game");
	}

	[Test]
	public void PaintSelectionIsPerCar()
	{
		const string ownedKey = "InfernoDrive.Style.Paint.4.Owned";
		const string keyA = "InfernoDrive.Paint.stylecar_a.Selected", keyB = "InfernoDrive.Paint.stylecar_b.Selected";
		PlayerPrefs.DeleteKey(ownedKey); PlayerPrefs.DeleteKey(keyA); PlayerPrefs.DeleteKey(keyB);
		try
		{
			// nothing chosen: the model's own paint
			Assert.IsFalse(IsSet(SelectedPaint("stylecar_a")));
			Assert.IsFalse(IsSet(SelectedPaint(null)));
			Assert.IsFalse(IsSet(SelectedPaint("")));
			// a choice that was never bought does not apply
			PlayerPrefs.SetInt(keyA, 4);   // CRIMSON
			Assert.IsFalse(IsSet(SelectedPaint("stylecar_a")));
			PlayerPrefs.SetInt(ownedKey, 1);
			object choice = SelectedPaint("stylecar_a");
			Assert.IsTrue(IsSet(choice));
			Color colour = (Color)Field(choice, "color");
			Assert.AreEqual(0.78f, colour.r, 1e-6f); Assert.AreEqual(0.05f, colour.g, 1e-6f); Assert.AreEqual(0.10f, colour.b, 1e-6f);
			Assert.AreEqual(0.30f, (float)Field(choice, "metallic"), 1e-6f);
			Assert.AreEqual(0.88f, (float)Field(choice, "smoothness"), 1e-6f);
			// the same choice through the id spelt differently, and none for another car
			Assert.IsTrue(IsSet(SelectedPaint("STYLECAR_A")));
			Assert.IsFalse(IsSet(SelectedPaint("stylecar_b")), "paint is chosen per car");
			Assert.IsFalse(PlayerPrefs.HasKey(keyB));
			// STOCK and out-of-range choices keep the model's paint
			PlayerPrefs.SetInt(keyA, 0);
			Assert.IsFalse(IsSet(SelectedPaint("stylecar_a")));
			PlayerPrefs.SetInt(keyA, 99);
			Assert.IsFalse(IsSet(SelectedPaint("stylecar_a")));
		}
		finally
		{
			PlayerPrefs.DeleteKey(ownedKey); PlayerPrefs.DeleteKey(keyA); PlayerPrefs.DeleteKey(keyB);
			PlayerPrefs.Save();
		}
	}
}
