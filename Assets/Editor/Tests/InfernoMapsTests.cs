using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Lobby = ShadowRunPrototype.InfernoLobby;

// [user request 2026-09-21: maps] The six lobby maps are data-driven environment profiles
// (Inferno/InfernoMaps.cs). InfernoMaps is a private nested class, so it is driven by reflection.
public class InfernoMapsTests
{
	private static readonly BindingFlags Any = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
	private static readonly Type MapsType = typeof(ShadowRunPrototype).GetNestedType("InfernoMaps", Any);

	private static object Call(string method, params object[] args)
	{
		MethodInfo m = MapsType.GetMethod(method, Any);
		Assert.IsNotNull(m, "InfernoMaps." + method);
		return m.Invoke(null, args);
	}

	private static int Count => (int)MapsType.GetField("Count", Any).GetValue(null);

	[Test]
	public void OneProfilePerLobbyMap()
	{
		Assert.IsNotNull(MapsType, "ShadowRunPrototype.InfernoMaps");
		Assert.AreEqual(Lobby.MapCount, Count);
		Assert.AreEqual(Lobby.MapCount, Lobby.MapSlugs.Length);
		for (int i = 0; i < Count; i++)
		{
			Assert.AreEqual(Lobby.MapNames[i], (string)Call("Name", i), "map " + i);
			Assert.AreEqual(Lobby.MapSlugs[i], (string)Call("Slug", i), "slug " + i);
		}
	}

	[Test]
	public void EveryMapLooksDifferent()
	{
		// road surface, sky and roadside scenery: no two maps share the same triple, and the six
		// maps use at least four different road surfaces and five different skies between them.
		var triples = new HashSet<string>();
		var roads = new HashSet<string>();
		var skies = new HashSet<string>();
		for (int i = 0; i < Count; i++)
		{
			string road = (string)Call("RoadKind", i), sky = (string)Call("SkyKind", i), scenery = (string)Call("SceneryKind", i);
			Assert.IsTrue(triples.Add(road + "/" + sky + "/" + scenery), "map " + i + " repeats " + road + "/" + sky + "/" + scenery);
			roads.Add(road);
			skies.Add(sky);
		}
		Assert.GreaterOrEqual(roads.Count, 4, "road kinds");
		Assert.GreaterOrEqual(skies.Count, 5, "sky kinds");
	}

	[Test]
	public void MapPricesNeverDecreaseAlongTheCarousel()
	{
		for (int i = 1; i < Lobby.MapCount; i++) Assert.GreaterOrEqual(Lobby.MapPrices[i], Lobby.MapPrices[i - 1], "map " + i);
	}

	[Test]
	public void OnlyNeonNightKeepsTheShippedRedPulse()
	{
		for (int i = 0; i < Count; i++)
			Assert.AreEqual(i == 1, (bool)Call("KeepsShippedPulse", i), "map " + i);
		Assert.IsTrue((bool)Call("HasLightning", 3), "STORM CHASE has lightning");
	}

	// [user request 2026-09-24: neon night cycle] Only NEON NIGHT carries a Day profile. A run starts at night; the first
	// dawn begins PhaseDuration (35 s, the shipped WeatherPhaseDuration) in and takes ToDayDuration (4 s, the shipped
	// NightToDayTransitionDuration); dusk comes another 35 s later over ToNightDuration (4.25 s, WeatherTransitionDuration).
	[Test]
	public void NeonNightCyclesBetweenNightAndDay()
	{
		for (int i = 0; i < Count; i++) Assert.AreEqual(i == 1, (bool)Call("HasDayNightCycle", i), "map " + i);
		Assert.AreEqual(35f, (float)MapsType.GetField("PhaseDuration", Any).GetValue(null));
		Assert.AreEqual(4f, (float)MapsType.GetField("ToDayDuration", Any).GetValue(null));
		Assert.AreEqual(4.25f, (float)MapsType.GetField("ToNightDuration", Any).GetValue(null));
		object cycle = Call("ResetCycle");
		Assert.AreEqual(0f, Blend(cycle), "a run starts at night");
		cycle = Advance(cycle, 34.9f);
		Assert.AreEqual(0f, Blend(cycle), "still night just before the phase ends");
		cycle = Advance(cycle, 0.2f);   // the phase timer runs out: the dawn starts
		cycle = Advance(cycle, 2f);
		float mid = Blend(cycle);
		Assert.IsTrue(mid > 0.2f && mid < 0.8f, "half-way through the dawn: " + mid);
		cycle = Advance(cycle, 2.5f);
		Assert.AreEqual(1f, Blend(cycle), 0.0001f, "day");
		cycle = Advance(cycle, 34.9f);
		Assert.AreEqual(1f, Blend(cycle), 0.0001f, "still day");
		cycle = Advance(cycle, 0.2f);   // dusk starts
		cycle = Advance(cycle, 2f);
		mid = Blend(cycle);
		Assert.IsTrue(mid > 0.2f && mid < 0.8f, "half-way through the dusk: " + mid);
		cycle = Advance(cycle, 2.5f);
		Assert.AreEqual(0f, Blend(cycle), 0.0001f, "night again");
		cycle = Advance(cycle, 0f);     // a zero step changes nothing
		Assert.AreEqual(0f, Blend(cycle));
	}

	private static float Blend(object cycle) { return (float)cycle.GetType().GetField("Blend").GetValue(cycle); }

	private static object Advance(object cycle, float dt)
	{
		object[] args = { cycle, dt };
		MethodInfo m = MapsType.GetMethod("Advance", Any);
		Assert.IsNotNull(m, "InfernoMaps.Advance");
		m.Invoke(null, args);
		return args[0];
	}
}
