// extern "C" bridge of InfernoGarage.inc (InfernoGarage_IsUnlocked / InfernoGarage_SetUnlocked):
// the profile code in Assembly-CSharp__1.cpp (SupabaseGameManager) lives in another
// translation unit in the shipped build and reached the garage editions through these two
// C functions. Kept as static forwarders on ShadowRunPrototype so the call sites read the same.
// /* [user request 2026-09-21: cars] */ The editions are retired; InfernoGarage forwards to the
// InfernoCars catalogue, whose six non-cloud ids own a PlayerPrefs flag
// (InfernoDrive.Vehicle.<id>.Unlocked). The four shipped ids stay on the cloud columns.
public sealed partial class ShadowRunPrototype
{
	// IsUnlocked: -1 = not an InfernoGarage vehicle, else 0/1. SetUnlocked: true = handled.
	public static int InfernoGarageIsUnlocked(string vehicleId) { return InfernoGarage.IsUnlocked(vehicleId); }
	public static bool InfernoGarageSetUnlocked(string vehicleId, bool unlocked) { return InfernoGarage.SetUnlocked(vehicleId, unlocked); }
}
