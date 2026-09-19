using UnityEngine;

public sealed class HoverAnimation : MonoBehaviour
{
	public float hoverSpeed;

	public float hoverHeight;

	public float swaySpeed;

	public float swayAngle;

	public float rotationSpeed;

	public bool useUnscaledTime;

	private float baseLocalY;

	private Quaternion baseLocalRotation;

	private Quaternion lastAppliedRotation;

	private bool hasAppliedRotation;

	public void CaptureBaseTransform()
	{
	}

	private void Awake()
	{
	}

	private void LateUpdate()
	{
	}
}
