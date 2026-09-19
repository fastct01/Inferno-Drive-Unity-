using UnityEngine;

public sealed class TrafficWheelAnimator : MonoBehaviour
{
	private static readonly string[] WheelSuffixes;

	private readonly Transform[] wheels;

	private readonly Quaternion[] baseRotations;

	private float spinDegrees;

	private float degreesPerMeter;

	public void Bind(Transform modelRoot)
	{
	}

	public void UpdateSpin(float metersPerSecond, float dt)
	{
	}

	public void ResetSpin()
	{
	}
}
