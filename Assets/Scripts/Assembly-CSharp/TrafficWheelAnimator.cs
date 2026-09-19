using System;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   TrafficWheelAnimator__cctor_mDF031600B59D4C68DFA0BF4292EEFF977111A843
//   TrafficWheelAnimator__ctor_m34245A5C722D033F9D3E2BFE70920145DED3584A
//   TrafficWheelAnimator_Bind_m8D87BD8D89855C03DD80CAD8792213D162DBF1CC
//   TrafficWheelAnimator_UpdateSpin_m50AB3205D06169E876779955182B02BDD30EA5D8
//   TrafficWheelAnimator_ResetSpin_m6D161F8175C01C2FEAA5473E34327E9CF4E66BA0
public sealed class TrafficWheelAnimator : MonoBehaviour
{
	private static readonly string[] WheelSuffixes = new string[4] { "Wheel.Bk.L", "Wheel.Bk.R", "Wheel.Ft.L", "Wheel.Ft.R" };

	private readonly Transform[] wheels = new Transform[4];

	private readonly Quaternion[] baseRotations = new Quaternion[4];

	private float spinDegrees;

	private float degreesPerMeter = 120f;

	public void Bind(Transform modelRoot)
	{
		Array.Clear(wheels, 0, wheels.Length);
		Transform[] nodes = modelRoot != null ? modelRoot.GetComponentsInChildren<Transform>(true) : Array.Empty<Transform>();
		for (int i = 0; i < nodes.Length; i++)
		{
			for (int j = 0; j < WheelSuffixes.Length; j++)
			{
				if (nodes[i].name.EndsWith(WheelSuffixes[j], StringComparison.OrdinalIgnoreCase))
				{
					wheels[j] = nodes[i];
				}
			}
		}
		float radiusSum = 0f;
		int radiusCount = 0;
		for (int i = 0; i < wheels.Length; i++)
		{
			baseRotations[i] = wheels[i] != null ? wheels[i].localRotation : Quaternion.identity;
			if (wheels[i] == null)
			{
				continue;
			}
			Renderer renderer = wheels[i].GetComponentInChildren<Renderer>(true);
			if (renderer == null)
			{
				continue;
			}
			float radius = Mathf.Max(renderer.bounds.extents.y, 0.01f);
			radiusSum += radius;
			radiusCount++;
		}
		if (radiusCount > 0)
		{
			degreesPerMeter = 360f / (6.28318548f * (radiusSum / radiusCount));
		}
		ResetSpin();
	}

	public void UpdateSpin(float metersPerSecond, float dt)
	{
		spinDegrees = Mathf.Repeat(spinDegrees + Mathf.Max(0f, metersPerSecond) * dt * degreesPerMeter, 360f);
		Quaternion spin = Quaternion.Euler(spinDegrees, 0f, 0f);
		for (int i = 0; i < wheels.Length; i++)
		{
			if (wheels[i] != null)
			{
				wheels[i].localRotation = baseRotations[i] * spin;
			}
		}
	}

	public void ResetSpin()
	{
		spinDegrees = 0f;
		for (int i = 0; i < wheels.Length; i++)
		{
			if (wheels[i] != null)
			{
				wheels[i].localRotation = baseRotations[i];
			}
		}
	}
}
