using NUnit.Framework;
using UnityEngine;
using Rules = ShadowRunPrototype.InfernoDrivingRules;

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
}
