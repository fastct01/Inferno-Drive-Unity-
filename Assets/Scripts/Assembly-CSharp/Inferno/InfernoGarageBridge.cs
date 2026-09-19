// extern "C" bridge of InfernoGarage.inc (InfernoGarage_IsUnlocked / InfernoGarage_SetUnlocked):
// the profile code in Assembly-CSharp__1.cpp (SupabaseGameManager) lives in another
// translation unit in the shipped build and reached the garage editions through these two
// C functions. Kept as static forwarders on ShadowRunPrototype so the call sites read the same.
public sealed partial class ShadowRunPrototype
{
	// IsUnlocked: -1 = not an InfernoGarage vehicle, else 0/1. SetUnlocked: true = handled.
	public static int InfernoGarageIsUnlocked(string vehicleId) { return InfernoGarage.IsUnlocked(vehicleId); }
	public static bool InfernoGarageSetUnlocked(string vehicleId, bool unlocked) { return InfernoGarage.SetUnlocked(vehicleId, unlocked); }
}
