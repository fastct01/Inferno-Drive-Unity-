using NUnit.Framework;
using UnityEngine;
using Rules = ShadowRunPrototype.InfernoDrivingRules;
using Lanes = ShadowRunPrototype.InfernoLanes;

// Port of car точно финал/tools/check_driving_rules.cpp: the expected values of the
// post-source speed / recovery / crystal rules (InfernoDrivingRules.h). Every assert of the
// C++ checker is one assertion here, in the same order.
public class InfernoDrivingRulesTests
{
	// Speed never falls, and never exceeds cruise / boost max / a pending collision-recovery target.
	private static float Simulate(int fps, bool boost, float seconds, float initial = 0f)
	{
		float speed = initial;
		float ceiling = Mathf.Max(boost ? 49.0f : Mathf.Max(29.0f, initial),
			Rules.RecoveryLeft > 0f ? Mathf.Min(Rules.RecoveryTarget, boost ? Rules.MaxSpeed : Rules.CruiseCap) : 0.0f);
		for (int i = 0; i < (int)(seconds * fps); ++i)
		{
			float next = Rules.AdvanceSpeed(speed, 29f, boost, 1f, 1.0f / fps, false);
			Assert.GreaterOrEqual(next, speed);
			Assert.LessOrEqual(next, ceiling + 0.001f);
			speed = next;
		}
		return speed;
	}

	[SetUp]
	public void Reset() { Rules.ResetRecovery(); }

	[Test]
	public void LaunchBuildsSpeedInsteadOfJumpingTo15()
	{
		float first = Rules.AdvanceSpeed(0f, 29f, false, 1f, 1.0f / 60, false);
		Assert.IsTrue(first > 0f && first < 0.2f);
		Assert.IsTrue(Simulate(60, false, 1) > 5f && Simulate(60, false, 1) < 10f);
		Assert.IsTrue(Simulate(60, false, 10) > 26f && Simulate(60, false, 10) < 29f);
	}

	[Test]
	public void FrameRateDoesNotChangeAcceleration()
	{
		Assert.Less(Mathf.Abs(Simulate(30, false, 8) - Simulate(120, false, 8)), 0.001f);
		Assert.Less(Mathf.Abs(Simulate(30, true, 8) - Simulate(120, true, 8)), 0.001f);
	}

	[Test]
	public void BoostAndTargets()
	{
		float boosted = Simulate(60, true, 10, 29f);
		Assert.Greater(boosted, 48f);
		Assert.AreEqual(boosted, Simulate(60, false, 5, boosted));                    // releasing boost cannot brake
		Assert.AreEqual(35f, Rules.AdvanceSpeed(35f, 21f, false, 1f, 1f, false));     // lower cruise/drift target
		Assert.AreEqual(35f, Rules.AdvanceSpeed(35f, 40f, true, 1f, 0f, false));      // paused
		Assert.AreEqual(35f, Rules.AdvanceSpeed(35f, 40f, true, 1f, 1f, true));       // airborne momentum
		Assert.Greater(Rules.AdvanceSpeed(10f, 29f, false, 1.4f, 1f, false), Rules.AdvanceSpeed(10f, 29f, false, 0.7f, 1f, false));
		// Recover progressively after a collision rather than restoring speed instantly.
		Assert.Less(Rules.AdvanceSpeed(22f, 29f, false, 1f, 1.0f / 60, false), 22.1f);
	}

	[Test]
	public void CollisionRecovery()
	{
		// a bump sheds speed, then the car returns to its pre-hit speed by itself, progressively, and stops there.
		Rules.ResetRecovery();
		Assert.AreEqual(30f, Rules.Impact(40f, 30f));
		Assert.AreEqual(40f, Rules.Impact(40f, 45f));   // an impact can never add speed
		Assert.Less(Rules.AdvanceSpeed(30f, 29f, false, 1f, 1.0f / 60, false), 30.3f);   // not instant
		float recovered = Simulate(60, false, 3, 30f);
		Assert.IsTrue(recovered > 37f && recovered < 40f);                             // well above cruise (29), not yet back
		Rules.ResetRecovery(); Rules.Impact(40f, 30f);
		float back = Simulate(60, false, 8, 30f);
		Assert.IsTrue(back > 39.5f && back <= 40f);                                    // back to 40, not beyond
		Assert.AreEqual(back, Simulate(60, false, 3, back));                           // and it stays there
		Rules.ResetRecovery(); Rules.Impact(40f, 30f);
		float slow = Simulate(30, false, 3, 30f);
		Rules.ResetRecovery(); Rules.Impact(40f, 30f);
		float fast = Simulate(120, false, 3, 30f);
		Assert.Less(Mathf.Abs(slow - fast), 0.05f);                                    // frame-rate independent
		Rules.ResetRecovery();
		Rules.Impact(49f, 35f);                                                        // hit while boosting...
		Assert.LessOrEqual(Simulate(60, false, 8, 35f), Rules.CruiseCap + 0.001f);     // ...boost released: caps at the no-boost limit
		Rules.ResetRecovery();
		Rules.Impact(49f, 35f);
		Assert.Greater(Simulate(60, true, 8, 35f), 48f);                               // boost held: all the way back
		Rules.ResetRecovery();
		Assert.AreEqual(30f, Simulate(60, false, 3, 30f));                             // no impact recorded: nothing to recover
		Rules.ResetRecovery();
		Rules.Impact(40f, 30f);
		float stalled = 30f;
		for (int i = 0; i < 60 * 7; ++i) stalled = Rules.AdvanceSpeed(stalled, 29f, false, 1f, 1.0f / 60, true);
		Assert.IsTrue(stalled == 30f && Rules.RecoveryLeft > 0f);                      // airborne frames neither advance nor consume the window
		Rules.ResetRecovery();
	}

	[Test]
	public void DistanceSpeedStages()
	{
		Assert.IsTrue(Rules.Stage(0f) == 0 && Rules.Stage(999f) == 0 && Rules.Stage(1000f) == 1 && Rules.Stage(4500f) == 4 && Rules.Stage(-5f) == 0);
		Assert.AreEqual(29.0f, Rules.Cruise(0f));
		Assert.Less(Mathf.Abs(Rules.Cruise(999f) - (29f + 11f * 999f / 2600.0f)), 0.001f);   // the game's ramp, untouched
		Assert.Greater(Rules.Cruise(1000f), Rules.Cruise(999f) + 2.9f);                     // the first step
		Assert.AreEqual(40f + 2f * Rules.StepGain, Rules.Cruise(2600f));
		Assert.IsTrue(Rules.Cruise(4000f) == 52.0f && Rules.Cruise(9000f) == 52.0f);        // capped
		Assert.IsTrue(Rules.CruiseTop(0f) == Rules.CruiseCap && Rules.CruiseTop(4000f) == 52.0f);
		Assert.IsTrue(Rules.BoostTop(0f) == Rules.MaxSpeed && Rules.BoostTop(1000f) == Rules.MaxSpeed && Rules.BoostTop(2000f) > Rules.MaxSpeed && Rules.BoostTop(4000f) == 58.0f && Rules.BoostTop(9000f) == 58.0f);
		for (float d = 0f; d < 9000f; d += 50f) Assert.IsTrue(Rules.Cruise(d + 50f) >= Rules.Cruise(d) && Rules.BoostTop(d) > Rules.Cruise(d));
		Rules.ResetRecovery();
		float staged = 40f;
		for (int i = 0; i < 60 * 20; ++i) staged = Rules.AdvanceSpeed(staged, Rules.Cruise(4000f), false, 1f, 1.0f / 60, false, Rules.BoostTop(4000f), Rules.CruiseTop(4000f));
		Assert.IsTrue(staged > 51.5f && staged <= 52.0f);                                  // no boost: reaches the stage cruise
		staged = 40f;
		for (int i = 0; i < 60 * 20; ++i) staged = Rules.AdvanceSpeed(staged, Rules.Cruise(4000f), true, 1f, 1.0f / 60, false, Rules.BoostTop(4000f), Rules.CruiseTop(4000f));
		Assert.IsTrue(staged > 57.5f && staged <= 58.0f);                                  // boost: up to the stage boost top
		Rules.ResetRecovery(); Rules.Impact(58f, 40f);
		float regained = 40f;
		for (int i = 0; i < 60 * 10; ++i) regained = Rules.AdvanceSpeed(regained, Rules.Cruise(4000f), false, 1f, 1.0f / 60, false, Rules.BoostTop(4000f), Rules.CruiseTop(4000f));
		Assert.LessOrEqual(regained, 52.0f + 0.001f);                                       // recovery without boost caps at the stage cruise
		Rules.ResetRecovery();
	}

	[Test]
	public void CrystalGapsAndSweptPickups()
	{
		Assert.IsTrue(Rules.CrystalGap(0f) == 550f && Rules.CrystalGap(1f) == 950f);
		for (int i = 0; i <= 1000; ++i)
		{
			float gap = Rules.CrystalGap(i / 1000.0f);
			Assert.IsTrue(gap >= 550f && gap <= 950f);
		}
		Assert.IsTrue(Rules.CrossesCrystal(0f, -12f, 0f, 12f));     // swept forward motion
		Assert.IsTrue(Rules.CrossesCrystal(-3f, -8f, 3f, 8f));      // crossing a lane while passing
		Assert.IsTrue(Rules.CrossesCrystal(0f, 0f, 0f, 0f));        // overlaps at low speed
		Assert.IsFalse(Rules.CrossesCrystal(3f, -12f, 3f, 12f));    // neighbouring lane
		Assert.IsFalse(Rules.CrossesCrystal(0f, 8f, 0f, 15f));      // already missed
		Assert.IsFalse(Rules.CrossesCrystal(-6f, -8f, -2f, 8f));    // stays outside the hitbox
		Assert.IsFalse(Rules.CrossesCrystal(-5f, -8f, 1f, -4f));    // not yet alongside the car
	}

	// [user request 2026-09-21: cars] The upgrade overloads: ENGINE's top-speed bonus lifts the cruise
	// target and its clamp, NITRO's cap bonus lifts the boost clamp and its rate multiplier speeds up
	// the boost approach only; every parameterless rule is the overload with 0 / 1.
	[Test]
	public void UpgradeBonusOverloads()
	{
		Assert.AreEqual(Rules.Cruise(0f), Rules.Cruise(0f, 0f));
		Assert.AreEqual(Rules.Cruise(1234f) + 6f, Rules.Cruise(1234f, 6f), 0.0001f);
		Assert.AreEqual(Rules.Cruise(1234f), Rules.Cruise(1234f, -3f));                  // a bonus never slows the car
		Assert.AreEqual(Rules.CruiseCap, Rules.CruiseTop(0f, 6f));                       // 29 + 6 stays under the 42 clamp
		Assert.AreEqual(Rules.CruiseTopMax + 6f, Rules.CruiseTop(9000f, 6f), 0.0001f);   // 52 + 6
		Assert.AreEqual(Rules.CruiseTop(9000f), Rules.CruiseTop(9000f, 0f));
		Assert.AreEqual(Rules.BoostTop(0f) + 5f, Rules.BoostTop(0f, 5f), 0.0001f);
		Assert.AreEqual(Rules.BoostTop(9000f), Rules.BoostTop(9000f, 0f));
		Assert.AreEqual(Rules.BoostTopMax + 5f, Rules.BoostTop(9000f, 5f), 0.0001f);     // 58 + 5
		for (float d = 0f; d < 9000f; d += 250f) Assert.Greater(Rules.BoostTop(d, 5f), Rules.Cruise(d, 6f));   // boost stays above cruise with the level-5 bonuses
		Rules.ResetRecovery();
		float plain = Rules.AdvanceSpeed(30f, 29f, true, 1f, 0.5f, false, 49f, 42f, 1f);
		Assert.AreEqual(Rules.AdvanceSpeed(30f, 29f, true, 1f, 0.5f, false, 49f, 42f), plain);   // the old signature is the overload with 1
		float quick = Rules.AdvanceSpeed(30f, 29f, true, 1f, 0.5f, false, 49f, 42f, 1.35f);
		Assert.Greater(quick, plain);
		Assert.LessOrEqual(quick, 49f);
		Assert.AreEqual(Rules.AdvanceSpeed(20f, 29f, false, 1f, 0.5f, false, 49f, 42f, 1f), Rules.AdvanceSpeed(20f, 29f, false, 1f, 0.5f, false, 49f, 42f, 1.35f));   // no boost: the multiplier is idle
		Assert.AreEqual(35f, Rules.AdvanceSpeed(35f, 40f, true, 1f, 0f, false, 49f, 42f, 1.35f));    // paused
		Assert.AreEqual(35f, Rules.AdvanceSpeed(35f, 40f, true, 1f, 1f, true, 49f, 42f, 1.35f));     // airborne momentum
		// with the level-5 bonuses the car settles exactly the bonus above the stage limits
		Rules.ResetRecovery();
		float speed = 40f;
		for (int i = 0; i < 60 * 30; ++i) speed = Rules.AdvanceSpeed(speed, Rules.Cruise(4000f, 6f), false, 1f, 1.0f / 60, false, Rules.BoostTop(4000f, 5f), Rules.CruiseTop(4000f, 6f), 1f);
		Assert.IsTrue(speed > 57.5f && speed <= 58f, "cruise + 6: " + speed);
		speed = 40f;
		for (int i = 0; i < 60 * 30; ++i) speed = Rules.AdvanceSpeed(speed, Rules.Cruise(4000f, 6f), true, 1f, 1.0f / 60, false, Rules.BoostTop(4000f, 5f), Rules.CruiseTop(4000f, 6f), 1.35f);
		Assert.IsTrue(speed > 62.5f && speed <= 63f, "boost + 5: " + speed);
		Rules.ResetRecovery();
	}

	[Test]
	public void ShippedConstants()
	{
		Assert.AreEqual(49f, Rules.MaxSpeed);
		Assert.AreEqual(42f, Rules.CruiseCap);
		Assert.AreEqual(60f, Rules.HighBeamRange);
		Assert.AreEqual(1000f, Rules.StepDistance);
		Assert.AreEqual(3f, Rules.StepGain);
		Assert.AreEqual(52f, Rules.CruiseTopMax);
		Assert.AreEqual(9f, Rules.BoostGain);
		Assert.AreEqual(58f, Rules.BoostTopMax);
		Assert.AreEqual(0.9f, Rules.RecoverRate);
		Assert.AreEqual(6f, Rules.RecoverWindow);
		Assert.AreEqual(550f, Rules.CrystalGapMin);
		Assert.AreEqual(950f, Rules.CrystalGapMax);
	}

	// [user request 2026-09-22: five lanes] The traffic rules for a wider road: on the shipped three lanes every
	// helper is the identity; on five the pool grows 12 -> 20, the spawn intervals shrink by 3 / 5 (so the per-lane
	// density is unchanged) and each three-lane formation lands whole on one of the windows 0..2, 1..3 or 2..4.
	[Test]
	public void FiveLaneTrafficRules()
	{
		Assert.AreEqual(3, Rules.ShippedLaneCount);
		Assert.AreEqual(12, Rules.TrafficPoolFor(12, 3));
		Assert.AreEqual(20, Rules.TrafficPoolFor(12, 5));
		Assert.AreEqual(12, Rules.TrafficPoolFor(12, 2));                                    // never below the shipped pool
		Assert.AreEqual(20, Rules.TrafficPoolFor(12, Lanes.Count));
		Assert.AreEqual(1f, Rules.SpawnIntervalScale(3));
		Assert.AreEqual(0.6f, Rules.SpawnIntervalScale(5), 0.0001f);
		Assert.AreEqual(1f, Rules.SpawnIntervalScale(2));                                    // never longer than shipped
		Assert.AreEqual(Rules.SpawnIntervalScale(5), Rules.SpawnIntervalScale(Lanes.Count));
		// per-lane density: cars per second per lane, and pooled cars per lane, are what they were on three lanes
		Assert.AreEqual(Rules.SpawnIntervalScale(3) * 3f, Rules.SpawnIntervalScale(5) * 5f, 0.0001f);
		Assert.AreEqual(Rules.TrafficPoolFor(12, 3) / 3, Rules.TrafficPoolFor(12, 5) / 5);
		// the formation window
		for (float r = 0f; r <= 1f; r += 0.05f) Assert.AreEqual(0, Rules.FormationOffset(3, r));   // three lanes: one window
		Assert.AreEqual(0, Rules.FormationOffset(5, 0f));
		Assert.AreEqual(0, Rules.FormationOffset(5, 0.33f));
		Assert.AreEqual(1, Rules.FormationOffset(5, 0.34f));
		Assert.AreEqual(1, Rules.FormationOffset(5, 0.66f));
		Assert.AreEqual(2, Rules.FormationOffset(5, 0.67f));
		Assert.AreEqual(2, Rules.FormationOffset(5, 0.999f));
		Assert.AreEqual(2, Rules.FormationOffset(5, 1f));                                    // Random.value can return 1
		Assert.AreEqual(0, Rules.FormationOffset(5, -1f));
		Assert.AreEqual(0, Rules.FormationOffset(2, 0.9f));                                  // fewer lanes than a formation: still lane 0
		int[] seen = new int[3];
		for (int i = 0; i < 300; ++i) seen[Rules.FormationOffset(5, i / 300f)]++;
		for (int k = 0; k < 3; ++k) Assert.IsTrue(seen[k] >= 98 && seen[k] <= 102, "window " + k + " picked " + seen[k] + " of 300");   // uniform
		Assert.AreEqual(0, Rules.FormationLane(0, 0, 5));
		Assert.AreEqual(2, Rules.FormationLane(2, 0, 5));
		Assert.AreEqual(2, Rules.FormationLane(1, 1, 5));                                    // the formation's middle on the road's middle
		Assert.AreEqual(4, Rules.FormationLane(2, 2, 5));                                    // the last window ends on the outer lane
		Assert.AreEqual(4, Rules.FormationLane(2, 3, 5));                                    // never past the road
		Assert.AreEqual(2, Rules.FormationLane(2, 2, 3));                                    // three lanes: clamped to the road
		Assert.AreEqual(1, Rules.FormationLane(1, 0, 3));
		Assert.AreEqual(0, Rules.FormationLane(-1, 0, 5));
		for (int offset = 0; offset < 3; ++offset)
		{
			for (int lane = 0; lane < 3; ++lane)
			{
				int road = Rules.FormationLane(lane, offset, Lanes.Count);
				Assert.IsTrue(road >= 0 && road < Lanes.Count);                              // every formation lane of every window is a road lane
				Assert.AreEqual(lane + offset, road);                                        // and the formation keeps its shape
			}
			// the mirrored formations (side / 2 - side) stay mirrored about the window's middle
			Assert.AreEqual(2 * Rules.FormationLane(1, offset, 5), Rules.FormationLane(0, offset, 5) + Rules.FormationLane(2, offset, 5));
		}
	}

	// [user request 2026-09-22: five lanes] The shared road geometry every lane-indexed system reads (InfernoLanes):
	// five lane centres on the shipped 3.1 m pitch, the player limits 0.75 m outside the outer lanes as ±3.85 was
	// outside ±3.1, and the road edge and every roadside prop moved out by Shift.
	[Test]
	public void FiveLaneGeometry()
	{
		Assert.AreEqual(5, Lanes.Count);
		Assert.AreEqual(Lanes.Count, Lanes.X.Length);
		float[] expected = { -6.2f, -3.1f, 0f, 3.1f, 6.2f };
		for (int i = 0; i < expected.Length; ++i) Assert.AreEqual(expected[i], Lanes.X[i], 0.0001f);
		for (int i = 1; i < Lanes.X.Length; ++i) Assert.AreEqual(Lanes.Spacing, Lanes.X[i] - Lanes.X[i - 1], 0.0001f);
		Assert.AreEqual(0f, Lanes.X[Lanes.Count / 2]);                                       // the middle lane is index 2, not 1
		Assert.AreEqual(3.1f, Lanes.Spacing);
		Assert.AreEqual(17f, Lanes.RoadWidth, 0.0001f);
		Assert.AreEqual(8.5f, Lanes.HalfRoad);
		Assert.AreEqual(Lanes.HalfRoad, Lanes.EdgeX);
		Assert.AreEqual(5.1f, Lanes.ShippedHalfRoad);
		Assert.AreEqual(3.4f, Lanes.Shift, 0.0001f);
		Assert.AreEqual(-6.95f, Lanes.PlayerMinX);
		Assert.AreEqual(6.95f, Lanes.PlayerMaxX);
		Assert.AreEqual(Lanes.X[Lanes.Count - 1] + 0.75f, Lanes.PlayerMaxX, 0.0001f);         // 3.85 was 3.1 + 0.75
		Assert.AreEqual(-Lanes.PlayerMaxX, Lanes.PlayerMinX);
		Assert.Less(Lanes.PlayerMaxX, Lanes.HalfRoad);                                        // the car stays on the asphalt
		Assert.AreEqual(Lanes.Count - 1, Lanes.DividerX.Length);
		for (int i = 0; i < Lanes.DividerX.Length; ++i) Assert.AreEqual((Lanes.X[i] + Lanes.X[i + 1]) * 0.5f, Lanes.DividerX[i], 0.0001f);   // dividers halfway between lanes
		// roadside props keep their shipped distance from the road edge
		Assert.AreEqual(6.08f + Lanes.Shift, Lanes.FenceX, 0.0001f);
		Assert.AreEqual(5.72f + Lanes.Shift, Lanes.LampX, 0.0001f);
		Assert.AreEqual(12.09f + Lanes.Shift, Lanes.VergeX, 0.0001f);
		Assert.AreEqual(12.5f + Lanes.Shift, Lanes.BuildingX, 0.0001f);
		Assert.AreEqual(13f, Lanes.Outward(9.6f), 0.0001f);                                   // the wrecking-ball posts
		Assert.AreEqual(-9.12f, Lanes.Outward(-5.72f), 0.0001f);                              // sign kept
		Assert.AreEqual(1.35f * 17f / 10.2f, Lanes.TextureTilingX, 0.0001f);                  // the asphalt keeps its metres per tile
		// Clamp and NearestLane
		Assert.AreEqual(Lanes.PlayerMinX, Lanes.Clamp(-40f));
		Assert.AreEqual(Lanes.PlayerMaxX, Lanes.Clamp(40f));
		Assert.AreEqual(1.2f, Lanes.Clamp(1.2f));
		for (int i = 0; i < Lanes.Count; ++i) Assert.AreEqual(i, Lanes.NearestLane(Lanes.X[i]));
		Assert.AreEqual(0, Lanes.NearestLane(-40f));
		Assert.AreEqual(4, Lanes.NearestLane(40f));
		Assert.AreEqual(1, Lanes.NearestLane(-1.6f));
		Assert.AreEqual(2, Lanes.NearestLane(-1.5f));
		Assert.AreEqual(3, Lanes.NearestLane(4.6f));
		Assert.AreEqual(4, Lanes.NearestLane(4.7f));
		Assert.AreEqual(4, Lanes.NearestLane(Lanes.PlayerMaxX));
		Assert.AreEqual(0, Lanes.NearestLane(Lanes.PlayerMinX));
		for (int i = 0; i < Lanes.DividerX.Length; ++i) Assert.IsTrue(Lanes.NearestLane(Lanes.DividerX[i] - 0.01f) == i && Lanes.NearestLane(Lanes.DividerX[i] + 0.01f) == i + 1);
	}

	// [user request 2026-09-22: five lanes] The wrecking ball must be a hazard in every lane: at car height its reach
	// covers the outer lane centres and the shoulder limit, it still dips to the road at the bottom of the swing, it
	// turns short of the gantry posts at the swing extreme, and the posts stand outside the road edge and the fence.
	// (InfernoHazards keeps its geometry private; the consts are read back by reflection.)
	[Test]
	public void WreckingBallReachesEveryLane()
	{
		const System.Reflection.BindingFlags any = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
		System.Type hazards = typeof(ShadowRunPrototype).GetNestedType("InfernoHazards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
		Assert.IsNotNull(hazards);
		float Const(string name) { return (float)hazards.GetField(name, any).GetRawConstantValue(); }
		float gantryHeight = Const("GantryHeight"), halfSpan = Const("GantryHalfSpan"), rope = Const("RopeLength");
		float radius = Const("BallRadius"), amplitude = Const("SwingAmplitude"), carHeight = Const("CarHeight");
		float hook = gantryHeight - 0.5f;                                                     // PlaceBall: the ball hangs 0.5 below the beam
		float BallCentreY(float x) { return hook - Mathf.Sqrt(rope * rope - x * x); }
		Assert.Less(hook - rope - radius, 0.05f);                                             // dips to the road at the bottom of the swing...
		Assert.Greater(hook - rope - radius, -0.5f);                                          // ...without sinking into it
		foreach (float x in Lanes.X) Assert.LessOrEqual(BallCentreY(x) - radius, carHeight, "lane at x " + x);   // hits a car in every lane
		Assert.LessOrEqual(BallCentreY(Lanes.PlayerMaxX) - radius, carHeight);                // and one hugging the shoulder
		float reach = rope * Mathf.Sin(amplitude);
		Assert.Greater(reach, Lanes.HalfRoad);                                                // swings out past the road edge
		Assert.Less(reach + radius, halfSpan - 0.3f);                                         // and turns short of the posts (0.6 wide)
		Assert.Greater(BallCentreY(reach) - radius, carHeight + 1f);                          // the road is open while the ball is up at a side
		Assert.Greater(halfSpan, Lanes.FenceX);                                               // posts outside the road edge and the fence...
		Assert.Less(halfSpan, Lanes.VergeX);                                                  // ...and inside the verge
	}
}
