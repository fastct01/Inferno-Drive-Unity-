using NUnit.Framework;
using UnityEngine;
using Lobby = ShadowRunPrototype.InfernoLobby;

// Pure rules of InfernoLobby.inc: daily-goal rewards, map prices / unlock state, map names.
public class InfernoLobbyRulesTests
{
	private const string SelectedMapKey = "InfernoDrive.SelectedMap";

	[Test]
	public void GoalRewardsPerKind()
	{
		// DailyGoalKind order: Distance, Runs, Boosts, Drift, CarsPassed, CasesOpened
		int[] expected = { 15, 10, 10, 15, 10, 20 };
		for (int kind = 0; kind < expected.Length; kind++) Assert.AreEqual(expected[kind], Lobby.GoalReward(kind), "kind " + kind);
		Assert.AreEqual(10, Lobby.GoalReward(-1));
		Assert.AreEqual(10, Lobby.GoalReward(6));
	}

	[Test]
	public void MapPrices()
	{
		CollectionAssert.AreEqual(new[] { 0, 0, 150, 250, 400, 600 }, Lobby.MapPrices);
		Assert.AreEqual(6, Lobby.MapCount);
		Assert.AreEqual(Lobby.MapCount, Lobby.MapPrices.Length);
	}

	[Test]
	public void FreeMapsAreAlwaysUnlockedAndPaidOnesReadPrefs()
	{
		for (int i = 2; i < Lobby.MapCount; i++) PlayerPrefs.DeleteKey("InfernoDrive.MapUnlocked." + i);
		Assert.IsTrue(Lobby.MapUnlocked(0));
		Assert.IsTrue(Lobby.MapUnlocked(1));
		for (int i = 2; i < Lobby.MapCount; i++) Assert.IsFalse(Lobby.MapUnlocked(i), "map " + i);
		PlayerPrefs.SetInt("InfernoDrive.MapUnlocked.3", 1);
		Assert.IsTrue(Lobby.MapUnlocked(3));
		PlayerPrefs.DeleteKey("InfernoDrive.MapUnlocked.3");
	}

	[Test]
	public void SelectedMapDefaultsToNeonNightAndClamps()
	{
		bool had = PlayerPrefs.HasKey(SelectedMapKey);
		int previous = PlayerPrefs.GetInt(SelectedMapKey, 1);
		PlayerPrefs.DeleteKey(SelectedMapKey);
		Assert.AreEqual(1, Lobby.SelectedMap());
		PlayerPrefs.SetInt(SelectedMapKey, 99);
		Assert.AreEqual(Lobby.MapCount - 1, Lobby.SelectedMap());
		PlayerPrefs.SetInt(SelectedMapKey, -4);
		Assert.AreEqual(0, Lobby.SelectedMap());
		if (had) PlayerPrefs.SetInt(SelectedMapKey, previous); else PlayerPrefs.DeleteKey(SelectedMapKey);
	}

	[Test]
	public void MapCatalogue()
	{
		CollectionAssert.AreEqual(new[] { "DAYLIGHT CITY", "NEON NIGHT", "RAIN RUN", "STORM CHASE", "FALLEN SKY", "PLANET HORIZON" }, Lobby.MapNames);
		CollectionAssert.AreEqual(new[]
		{
			"Golden streets. Open throttle.",
			"Electric skyline. Endless asphalt.",
			"Rain-lit roads. Stay sharp.",
			"Lightning overhead. Keep your line.",
			"A sky full of falling stars.",
			"A different world above the road.",
		}, Lobby.MapDescriptions);
	}

	[Test]
	public void PaletteIsTheShippedOne()
	{
		Assert.AreEqual(0x000000u, Lobby.Black);
		Assert.AreEqual(0x050309u, Lobby.Ink);
		Assert.AreEqual(0x080513u, Lobby.Night);
		Assert.AreEqual(0x120C1Fu, Lobby.Slate);
		Assert.AreEqual(0x420E7Au, Lobby.Grape);
		Assert.AreEqual(0x9433FFu, Lobby.Electric);
		Assert.AreEqual(0xB840FFu, Lobby.Violet);
		Assert.AreEqual(0xBF66FFu, Lobby.Magenta);
		Assert.AreEqual(0xB26BFFu, Lobby.Orchid);
		Assert.AreEqual(0x948CD1u, Lobby.Lavender);
		Assert.AreEqual(0xAD8ADBu, Lobby.Lilac);
		Assert.AreEqual(0xD199F5u, Lobby.Blossom);
		Assert.AreEqual(0xE0D6F0u, Lobby.Pearl);
		Assert.AreEqual(0xFAE6FFu, Lobby.FrameText);
		Color c = Lobby.Hex(0x9433FF, 0.5f);
		Assert.AreEqual(0x94 / 255f, c.r, 1e-6f);
		Assert.AreEqual(0x33 / 255f, c.g, 1e-6f);
		Assert.AreEqual(0xFF / 255f, c.b, 1e-6f);
		Assert.AreEqual(0.5f, c.a);
	}
}
