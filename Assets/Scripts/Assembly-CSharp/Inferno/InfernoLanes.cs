using UnityEngine;

// [user request 2026-09-22: five lanes] The road has five lanes on every map instead of the shipped
// three. The shipped constants in ShadowRunPrototype.cs are left untouched (the brief forbids editing
// them); everything that depended on the three-lane geometry reads these values instead, and every
// roadside element keeps its shipped distance from the road EDGE (the edge moved out by Shift).
//
//   shipped: lanes {-3.1, 0, 3.1}, RoadWidth 10.2 (edge lines at ±5.1), PlayerMinX/MaxX ±3.85,
//            NeonFenceX 6.08, StreetLampX 5.72, verges at ±12.09 (14.02 wide), buildings at ±12.5
public sealed partial class ShadowRunPrototype
{
	internal static class InfernoLanes
	{
		public const int Count = 5;
		public const float Spacing = 3.1f;                                   // the shipped lane pitch
		public static readonly float[] X = { -6.2f, -3.1f, 0f, 3.1f, 6.2f };  // lane centres, left to right
		public const float ShippedHalfRoad = 5.1f;                           // RoadWidth 10.2 / 2
		public const float HalfRoad = 8.5f;                                  // 5 lanes: 6.2 + 3.1 / 2 + 0.75 shoulder
		public const float RoadWidth = HalfRoad * 2f;                        // 17.0 (original RoadWidth 10.2)
		public const float Shift = HalfRoad - ShippedHalfRoad;               // 3.4: how far every roadside thing moves out
		public const float PlayerMinX = -6.95f, PlayerMaxX = 6.95f;          // outer lane 6.2 + 0.75, as ±3.85 was 3.1 + 0.75
		public const float EdgeX = HalfRoad;                                 // edge lines (original 5.1)
		public const float FenceX = 6.08f + Shift;                           // 9.48 (original NeonFenceX 6.08)
		public const float LampX = 5.72f + Shift;                            // 9.12 (original StreetLampX 5.72)
		public const float VergeX = 12.09f + Shift;                          // 15.49 (original 12.09), same 14.02 width
		public const float BuildingX = 12.5f + Shift;                        // 15.9 (original 12.5 + inset)
		public const float TextureTilingX = 1.35f * RoadWidth / 10.2f;       // 2.25: the asphalt keeps its shipped metres-per-tile
		public const float TrafficLightX = 6.7f + Shift;                     // 10.1 (original 6.7: the traffic-light signals, 1.6 m outside the edge)

		// Lane-divider x positions (four dashed lines between five lanes).
		public static readonly float[] DividerX = { -4.65f, -1.55f, 1.55f, 4.65f };

		// The shipped RoadTextureTiling (1.35 x 170) with the x repeat widened to the five-lane road, so the
		// asphalt keeps its metres per tile instead of stretching; the y repeat (and the scroll maths) is untouched.
		public static Vector2 TextureTiling { get { return new Vector2(TextureTilingX, RoadTextureTiling.y); } }

		// A shipped width centred on the road (a probe box, a particle box) with both sides moved out with the edge.
		public static float Widen(float shippedWidth)
		{
			return shippedWidth + 2f * Shift;
		}

		public static float Clamp(float x) { return Mathf.Clamp(x, PlayerMinX, PlayerMaxX); }

		public static int NearestLane(float x)
		{
			int best = 0; float bestDistance = Mathf.Abs(x - X[0]);
			for (int i = 1; i < X.Length; i++)
			{
				float d = Mathf.Abs(x - X[i]);
				if (d < bestDistance) { bestDistance = d; best = i; }
			}
			return best;
		}

		// A shipped roadside x (measured from the road centre) moved out with the edge, sign kept.
		public static float Outward(float shippedX)
		{
			return shippedX + Mathf.Sign(shippedX) * Shift;
		}
	}
}
