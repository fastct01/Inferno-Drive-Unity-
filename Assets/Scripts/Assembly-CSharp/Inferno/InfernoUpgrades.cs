using ShadowRun.Services;
using UnityEngine;

// [user request 2026-09-21: cars] Per-car upgrades, bought with crystals from the garage's
// tuning card: ENGINE (acceleration, top speed), HANDLING (steering, grip) and NITRO (boost
// rate and boost cap), five levels each. Levels are PlayerPrefs like the style items
// (InfernoDrive.Upgrade.<carId>.<Stat>); the price climbs with the level and the car's rarity
// (round(base * (1 + 0.5 * level)), base Common 40 / Rare 60 / Epic 90 / Legendary 130).
// UpdatePlayer multiplies the active definition's base arcade values by the multipliers every
// frame and hands the speed bonuses to the InfernoDrivingRules overloads (a bought upgrade also
// re-creates the active definition). Pure PlayerPrefs / arithmetic apart from Buy, so the
// EditMode tests cover the table.
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoUpgrades
	{
		public enum Stat { Engine, Handling, Nitro }

		public const int MaxLevel = 5;
		public const int StatCount = 3;

		// per level: ENGINE accel x(1 + 0.08 L), top speed +1.2 m/s L; HANDLING steer x(1 + 0.06 L),
		// grip x(1 + 0.05 L); NITRO boost rate x(1 + 0.07 L), boost cap +1.0 m/s L
		public const float EngineAccelPerLevel = 0.08f;
		public const float EngineTopSpeedPerLevel = 1.2f;
		public const float HandlingSteerPerLevel = 0.06f;
		public const float HandlingGripPerLevel = 0.05f;
		public const float NitroRatePerLevel = 0.07f;
		public const float NitroCapPerLevel = 1.0f;

		private static readonly string[] StatKeys = { "Engine", "Handling", "Nitro" };
		private static readonly string[] StatNames = { "ENGINE", "HANDLING", "NITRO" };
		private static readonly int[] BasePrices = { 40, 60, 90, 130 };   // indexed by InfernoCars.Rarity

		public static string StatName(Stat stat) { return StatNames[(int)stat]; }

		public static string Key(string carId, Stat stat)
		{
			return "InfernoDrive.Upgrade." + carId + "." + StatKeys[(int)stat];
		}

		public static int Level(string carId, Stat stat)
		{
			if (string.IsNullOrEmpty(carId)) return 0;
			return Mathf.Clamp(PlayerPrefs.GetInt(Key(carId, stat), 0), 0, MaxLevel);
		}

		public static int TotalLevels(string carId)
		{
			return Level(carId, Stat.Engine) + Level(carId, Stat.Handling) + Level(carId, Stat.Nitro);
		}

		public static int BasePrice(InfernoCars.Rarity rarity) { return BasePrices[(int)rarity]; }

		// The price of going from `level` to `level + 1`; -1 once the stat is maxed.
		public static int PriceAt(InfernoCars.Rarity rarity, int level)
		{
			if (level < 0 || level >= MaxLevel) return -1;
			return Mathf.RoundToInt(BasePrices[(int)rarity] * (1f + 0.5f * level));
		}

		public static int Price(string carId, Stat stat)   // -1 when maxed (or the car is unknown)
		{
			if (!InfernoCars.Contains(carId)) return -1;
			return PriceAt(InfernoCars.RarityOf(carId), Level(carId, stat));
		}

		// Spends the crystals (SupabaseGameManager.Crystals / SetCrystals, the offline pattern of
		// InfernoStyle.Pick), saves the level, and when the car is the active one re-creates its
		// definition so the new multipliers drive at once. Returns false when maxed, unknown or
		// unaffordable (the caller decides whether to open the gem shop).
		public static bool Buy(ShadowRunPrototype game, string carId, Stat stat)
		{
			if (!InfernoCars.Contains(carId)) return false;
			int level = Level(carId, stat);
			if (level >= MaxLevel) return false;
			int price = PriceAt(InfernoCars.RarityOf(carId), level);
			int balance = SupabaseGameManager.Crystals;
			if (balance == int.MaxValue || balance < price) return false;
			SupabaseGameManager.SetCrystals(balance - price);
			PlayerPrefs.SetInt(Key(carId, stat), level + 1);
			PlayerPrefs.Save();
			if (game != null)
			{
				if (game.activeVehicleDefinition != null && InfernoCars.SameId(game.activeVehicleDefinition.Id, carId))
				{
					game.SetActiveVehicleDefinition(game.CreateVehicleDefinition(carId));
				}
				else if (game.garagePreviewVehicleDefinition != null && InfernoCars.SameId(game.garagePreviewVehicleDefinition.Id, carId))
				{
					game.garagePreviewVehicleDefinition = game.CreateVehicleDefinition(carId);   // stats only; the model needs no rebuild
				}
				game.UpdateGarageUi();
			}
			return true;
		}

		public static float AccelMultiplier(string carId) { return 1f + EngineAccelPerLevel * Level(carId, Stat.Engine); }
		public static float SteerMultiplier(string carId) { return 1f + HandlingSteerPerLevel * Level(carId, Stat.Handling); }
		public static float GripMultiplier(string carId) { return 1f + HandlingGripPerLevel * Level(carId, Stat.Handling); }
		public static float BoostRateMultiplier(string carId) { return 1f + NitroRatePerLevel * Level(carId, Stat.Nitro); }
		public static float TopSpeedBonus(string carId) { return EngineTopSpeedPerLevel * Level(carId, Stat.Engine); }   // m/s added to the cruise caps
		public static float BoostCapBonus(string carId) { return NitroCapPerLevel * Level(carId, Stat.Nitro); }          // m/s added to the boost cap
	}
}
