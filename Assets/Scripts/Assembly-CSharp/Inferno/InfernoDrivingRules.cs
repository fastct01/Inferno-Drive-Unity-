using UnityEngine;

// Port of Il2CppOutputProject/Source/il2cppOutput/InfernoDrivingRules.h — the
// post-source speed-stage and collision-recovery rules hooked into UpdatePlayer /
// ResolveTrafficContact. Pure functions plus one static recovery state, exactly as
// the C++ header (its `static Recovery state` singleton).
public sealed partial class ShadowRunPrototype
{
	internal static class InfernoDrivingRules
	{
		public const float MaxSpeed = 49f;
		public const float CrystalGapMin = 550f;
		public const float CrystalGapMax = 950f;

		public const float CruiseCap = 42f;   // the game's speed clamp without boost (UpdatePlayer)

		// High beam reach. SendHighBeamReaction flashes the nearest car ahead in the
		// player's lane within this many metres (the game shipped 20 m: under a second
		// at cruise); the headlight spot in ConfigureHighBeamPrefab lights the same
		// distance so the beam looks as long as it acts.
		public const float HighBeamRange = 60f;

		// Distance speed steps. The game's own cruise ramp (29 -> 40 m/s over the first
		// 2600 m) gains a further +3 m/s at every 1000 m mark, up to 52 m/s; boost sits
		// 9 m/s above cruise (never below its original 49, never above 58). Every step
		// is announced on the HUD by InfernoDriving.
		public const float StepDistance = 1000f;
		public const float StepGain = 3f;
		public const float CruiseTopMax = 52f;
		public const float BoostGain = 9f;
		public const float BoostTopMax = 58f;

		public static int Stage(float distance)
		{
			return distance > 0f ? (int)(distance / StepDistance) : 0;
		}

		public static float Cruise(float distance)
		{
			float ramp = 29f + 11f * Mathf.Clamp(distance / 2600f, 0f, 1f);
			return Mathf.Min(ramp + StepGain * Stage(distance), CruiseTopMax);
		}

		public static float CruiseTop(float distance)   // clamp without boost
		{
			return Mathf.Max(CruiseCap, Cruise(distance));
		}

		public static float BoostTop(float distance)
		{
			return Mathf.Clamp(Cruise(distance) + BoostGain, MaxSpeed, BoostTopMax);
		}

		// Collision recovery. A bump sheds speed (Impact); afterwards the car climbs
		// back to the speed it had before the hit on its own, with no input, faster
		// than a normal launch. Once it is back, or the window runs out, the normal
		// acceleration rule resumes.
		public const float RecoverRate = 0.9f;
		public const float RecoverWindow = 6f;

		private static float recoveryTarget;
		private static float recoveryLeft;

		// RecoveryState() of the C++ header, read-only (the EditMode tests inspect it).
		public static float RecoveryTarget => recoveryTarget;
		public static float RecoveryLeft => recoveryLeft;

		public static void ResetRecovery()
		{
			recoveryTarget = 0f;
			recoveryLeft = 0f;
		}

		// Applies an impact penalty: the result can never exceed the speed before the
		// hit, and the speed before the hit becomes the recovery target.
		public static float Impact(float before, float result)
		{
			float after = Mathf.Min(before, result);
			if (after < before)
			{
				recoveryTarget = Mathf.Max(before, recoveryLeft > 0f ? recoveryTarget : 0f);
				recoveryLeft = RecoverWindow;
			}
			return after;
		}

		// Exponential approach: strong initial acceleration easing into cruising speed.
		// Releasing boost or drifting cannot lower the speed already reached.
		// boostTop / cruiseTop are the stage-dependent ceilings (BoostTop / CruiseTop
		// of the current distance); the defaults are the original fixed limits.
		public static float AdvanceSpeed(float speed, float cruise, bool boost, float acceleration,
			float dt, bool airborne, float boostTop = MaxSpeed, float cruiseTop = CruiseCap)
		{
			if (dt <= 0f || airborne) return speed;
			float target = Mathf.Max(speed, boost ? boostTop : cruise);
			float rate = (boost ? 0.42f : 0.28f) * Mathf.Clamp(acceleration, 0.5f, 2f);
			if (recoveryLeft > 0f)
			{
				recoveryLeft -= dt;
				float goal = Mathf.Min(recoveryTarget, boost ? boostTop : cruiseTop);
				if (speed >= goal) recoveryLeft = 0f;
				else
				{
					target = Mathf.Max(target, goal);
					rate = Mathf.Max(rate, RecoverRate);
				}
			}
			// -expm1(-rate*dt) == 1 - exp(-rate*dt)
			return speed + (target - speed) * (1f - Mathf.Exp(-rate * dt));
		}

		public static float CrystalGap(float random01)
		{
			return CrystalGapMin + (CrystalGapMax - CrystalGapMin) * Mathf.Clamp(random01, 0f, 1f);
		}

		// Segment/box intersection in car-relative road coordinates; fast movement or
		// a lane change between frames must not skip a pickup or collect a neighbouring lane.
		public static bool CrossesCrystal(float x0, float z0, float x1, float z1)
		{
			float enter = 0f, leave = 1f;
			bool Axis(float start, float end, float radius)
			{
				float delta = end - start;
				if (Mathf.Abs(delta) < 0.00001f) return Mathf.Abs(start) <= radius;
				float a = (-radius - start) / delta, b = (radius - start) / delta;
				if (a > b) { float t = a; a = b; b = t; }
				enter = Mathf.Max(enter, a);
				leave = Mathf.Min(leave, b);
				return enter <= leave;
			}
			return Axis(x0, x1, 1.05f) && Axis(z0, z1, 2.6f);
		}
	}
}
