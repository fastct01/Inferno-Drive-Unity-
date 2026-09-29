using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Cars = ShadowRunPrototype.InfernoCars;
using Upgrades = ShadowRunPrototype.InfernoUpgrades;

// [user request 2026-09-21: cars] The catalogue's definitions (base car + W222 prefab + ride height +
// base arcade stats), paint data, card stat bars and picture routing. Definitions need a game
// instance for the shipped Create*Definition builders: an AddComponent in edit mode runs no Awake.
public class InfernoCarsTests
{
	private static readonly string[] UpgradeKeys = BuildUpgradeKeys();
	private static string[] BuildUpgradeKeys()
	{
		var keys = new List<string>();
		foreach (string id in Cars.Ids)
			for (int s = 0; s < Upgrades.StatCount; s++) keys.Add(Upgrades.Key(id, (Upgrades.Stat)s));
		return keys.ToArray();
	}
	private readonly Dictionary<string, int?> snapshot = new Dictionary<string, int?>();

	[SetUp]
	public void ClearUpgrades()
	{
		snapshot.Clear();
		foreach (string key in UpgradeKeys)
		{
			snapshot[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
			PlayerPrefs.DeleteKey(key);
		}
	}

	[TearDown]
	public void RestoreUpgrades()
	{
		foreach (var kv in snapshot) { if (kv.Value.HasValue) PlayerPrefs.SetInt(kv.Key, kv.Value.Value); else PlayerPrefs.DeleteKey(kv.Key); }
		PlayerPrefs.Save();
	}

	private static VehicleDefinition Definition(ShadowRunPrototype game, string id) { return Cars.MakeDefinition(game, id); }

	[Test]
	public void DefinitionsStartFromTheBaseCars()
	{
		// a running game (the Editor may be in play mode while these run) serves as well as a bare component
		ShadowRunPrototype game = UnityEngine.Object.FindAnyObjectByType<ShadowRunPrototype>();
		GameObject host = null;
		if (game == null)
		{
			host = new GameObject("InfernoCars test");
			game = host.AddComponent<ShadowRunPrototype>();
		}
		try
		{
			Assert.IsNull(Definition(game, "car3_noir"));
			Assert.IsNull(Definition(game, null));
			Assert.IsNull(Cars.MakeDefinition(null, "sportcar"));

			VehicleDefinition starter = Definition(game, "sportcar");
			Assert.AreEqual("sportcar", starter.Id);
			Assert.AreEqual("sportcar", starter.ModelResource);
			Assert.AreEqual("Player Sportcar Model", starter.PlayerModelName);   // the shipped names of the four cloud cars are kept
			Assert.AreEqual(0.14f, starter.RoadYOffset, 0.0001f);
			Assert.AreEqual(1.04f, starter.ArcadeAcceleration, 0.0001f);
			Assert.AreEqual(1.02f, starter.ArcadeGrip, 0.0001f);

			VehicleDefinition amg = Definition(game, "amg_w222");
			Assert.AreEqual("amg_w222", amg.Id);
			Assert.AreEqual("uploads_files_4881612_uploads_files_3750868_MB.W222.AMG", amg.ModelResource);
			Assert.AreEqual("Player AMG SOVEREIGN Model", amg.PlayerModelName);
			Assert.AreEqual("Garage Preview AMG SOVEREIGN", amg.GarageModelName);
			CollectionAssert.AreEqual(new[] { "W222.Wheel.Ft.L", "W222.Wheel.Ft.R", "W222.Wheel.Bk.L", "W222.Wheel.Bk.R" }, amg.WheelMeshNames);
			CollectionAssert.AreEqual(new[] { "W222.Brake.Ft.L", "W222.Brake.Ft.R", "W222.Brake.Bk.L", "W222.Brake.Bk.R" }, amg.BrakeDiscNames);
			CollectionAssert.AreEqual(new[] { "W222" }, amg.BoundsRendererNameKeywords);
			Assert.AreEqual(0, amg.BrakeLightNames.Length);
			Assert.IsTrue(amg.HideOversizedPlaneRenderers);
			Assert.AreEqual(0.05f, amg.RoadYOffset, 0.0001f);
			Assert.AreEqual(2.6f, amg.GarageTargetFootprint, 0.0001f);
			Assert.AreEqual(0.92f, amg.ArcadeSteering, 0.0001f);
			Assert.AreEqual(0.90f, amg.ArcadeDrift, 0.0001f);

			VehicleDefinition armour = Definition(game, "amg_armour");
			Assert.AreEqual("amg_armour", armour.Id);
			Assert.AreEqual(amg.ModelResource, armour.ModelResource);
			Assert.AreEqual(0.05f + 0.06f, armour.RoadYOffset, 0.0001f);           // the lift
			Assert.AreEqual(amg.GarageRoadYOffset + 0.06f, armour.GarageRoadYOffset, 0.0001f);
			Assert.AreEqual(1.02f, armour.ArcadeAcceleration, 0.0001f);
			Assert.AreEqual(1.10f, armour.ArcadeGrip, 0.0001f);

			VehicleDefinition rs = Definition(game, "sportcar_rs");
			Assert.AreEqual("sportcar", rs.ModelResource);
			Assert.AreEqual("Player PHANTOM RS Model", rs.PlayerModelName);
			Assert.AreEqual(0.14f - 0.04f, rs.RoadYOffset, 0.0001f);               // lowered
			Assert.AreEqual(1.10f, rs.ArcadeAcceleration, 0.0001f);

			VehicleDefinition club = Definition(game, "car3_clubsport");
			Assert.AreEqual("car3", club.ModelResource);
			Assert.AreEqual(-0.05f, club.RoadYOffset, 0.0001f);
			Assert.AreEqual(1.18f, club.ArcadeGrip, 0.0001f);

			VehicleDefinition p900 = Definition(game, "infal_model");
			Assert.AreEqual("IFNAL_MODEL_P900", p900.ModelResource);
			CollectionAssert.AreEqual(new[] { "P900 Wheel FL", "P900 Wheel FR", "P900 Wheel BL", "P900 Wheel BR" }, p900.WheelMeshNames);   // the pivots Decorate builds
			Assert.AreEqual(0, p900.BrakeDiscNames.Length);
			VehicleDefinition spyder = Definition(game, "infal_spyder");
			Assert.AreEqual(p900.ModelResource, spyder.ModelResource);
			CollectionAssert.AreEqual(p900.WheelMeshNames, spyder.WheelMeshNames);
			Assert.AreEqual(1.16f, spyder.ArcadeAcceleration, 0.0001f);

			VehicleDefinition x = Definition(game, "flying_x");
			Assert.AreEqual("flying_car", x.ModelResource);
			Assert.IsTrue(x.HoverEnabled);
			Assert.AreEqual(0.80f, x.ArcadeDrift, 0.0001f);

			// the definition carries the base stats only: UpdatePlayer applies the upgrade multipliers
			PlayerPrefs.SetInt(Upgrades.Key("sportcar_rs", Upgrades.Stat.Engine), 3);
			Assert.AreEqual(1.10f, Definition(game, "sportcar_rs").ArcadeAcceleration, 0.0001f);
			Assert.AreEqual(1.24f, Upgrades.AccelMultiplier("sportcar_rs"), 0.0001f);
		}
		finally
		{
			if (host != null) UnityEngine.Object.DestroyImmediate(host);
		}
	}

	[Test]
	public void PaintMaterialsAndDefaults()
	{
		CollectionAssert.AreEqual(new[] { "main" }, Cars.PaintMaterials("sportcar"));
		CollectionAssert.AreEqual(new[] { "main" }, Cars.PaintMaterials("sportcar_rs"));
		CollectionAssert.AreEqual(new[] { "W222 - Car Color" }, Cars.PaintMaterials("amg_armour"));
		CollectionAssert.AreEqual(new[] { "Paint" }, Cars.PaintMaterials("car3_clubsport"));
		CollectionAssert.AreEqual(new[] { "dtP900_car_DeTomaso_P900_carpaint1" }, Cars.PaintMaterials("infal_spyder"));
		CollectionAssert.AreEqual(new[] { "car base" }, Cars.PaintMaterials("flying_x"));
		Assert.AreEqual(0, Cars.PaintMaterials("nope").Length);
		Assert.AreEqual(new Color(0.60f, 0.62f, 0.68f, 1f), Cars.DefaultPaint("amg_w222"));
		Assert.AreEqual(Color.white, Cars.DefaultPaint("nope"));
		var defaults = new HashSet<Color>();
		foreach (string id in Cars.Ids) defaults.Add(Cars.DefaultPaint(id));
		Assert.AreEqual(Cars.Count, defaults.Count, "every model has its own default paint");
	}

	[Test]
	public void StatBarsGrowWithUpgrades()
	{
		Assert.AreEqual(0.6f, Cars.BaseStat("sportcar", Upgrades.Stat.Engine), 0.0001f);      // (1.04 - 0.8) / 0.4
		Assert.AreEqual(0.525f, Cars.BaseStat("sportcar", Upgrades.Stat.Handling), 0.0001f);  // ((1 + 1.02) / 2 - 0.8) / 0.4
		Assert.AreEqual(0.8f, Cars.BaseStat("car3", Upgrades.Stat.Handling), 0.0001f);        // the Cayman corners best
		Assert.AreEqual(0f, Cars.BaseStat("nope", Upgrades.Stat.Engine));
		foreach (string id in Cars.Ids)
			for (int s = 0; s < Upgrades.StatCount; s++)
			{
				float bar = Cars.BaseStat(id, (Upgrades.Stat)s);
				Assert.IsTrue(bar > 0.1f && bar < 1f, id + " " + (Upgrades.Stat)s + " = " + bar);
			}
		Assert.AreEqual(Cars.BaseStat("sportcar", Upgrades.Stat.Engine), Cars.Stat("sportcar", Upgrades.Stat.Engine), 0.0001f);
		PlayerPrefs.SetInt(Upgrades.Key("sportcar", Upgrades.Stat.Engine), 3);
		Assert.AreEqual(0.6f + 3 * Cars.StatPerUpgradeLevel, Cars.Stat("sportcar", Upgrades.Stat.Engine), 0.0001f);
		PlayerPrefs.SetInt(Upgrades.Key("flying_x", Upgrades.Stat.Nitro), 5);
		Assert.LessOrEqual(Cars.Stat("flying_x", Upgrades.Stat.Nitro), 1f);
	}

	[Test]
	public void PicturesRouteThroughTheIconPrefix()
	{
		Assert.AreEqual("Data/Raw/InfernoCars/", Cars.IconPrefix);
		foreach (string id in Cars.Ids)
		{
			string picture = Cars.CasePicture(id);
			if (Cars.IsVariant(id))   /* [user request 2026-09-24: plain cars] */
			{
				Assert.AreEqual(Cars.IconPrefix + id + ".png", picture, id);
				Assert.AreEqual(id, Cars.IdFromPicture(picture));
			}
			else
			{
				Assert.IsNull(Cars.IdFromPicture(picture), id + " keeps its photo");
			}
			Sprite icon = Cars.Icon(null, id);
			Assert.IsTrue(icon == null || icon.name == "Icon " + id, "no game: only an already rendered icon can come back");
		}
		Assert.IsNull(Cars.IdFromPicture("Data/Raw/InfernoCars/"));
		Assert.IsNull(Cars.IdFromPicture(null));
		Assert.AreEqual("porsche photo", Cars.FallbackPicture("car3_clubsport"));
		Assert.AreEqual("impala photo", Cars.FallbackPicture("infal_spyder"));
		Assert.AreEqual("flyingcar photo", Cars.FallbackPicture("flying_x"));
		Assert.AreEqual("sportcar photo", Cars.FallbackPicture("nope"));
		Assert.AreEqual("P900 Wheel BR", Cars.P900WheelName(3));
	}
}
