using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   HoverAnimation__ctor_mA869DBF93177DC6F83ADA529B52FFAE00BE7D2B0
//   HoverAnimation_CaptureBaseTransform_m441525B78445489A207DB94BDAE38C7E473AB736
//   HoverAnimation_Awake_m083CE55F631C58DB8262CD264EA70B330475B170
//   HoverAnimation_LateUpdate_m86B629CD61779B4721E076CCB03497F8DE9A2BBD
public sealed class HoverAnimation : MonoBehaviour
{
	public float hoverSpeed = 2f;

	public float hoverHeight = 0.15f;

	public float swaySpeed = 1.35f;

	public float swayAngle = 1.2f;

	public float rotationSpeed;

	public bool useUnscaledTime;

	private float baseLocalY;

	private Quaternion baseLocalRotation;

	private Quaternion lastAppliedRotation;

	private bool hasAppliedRotation;

	public void CaptureBaseTransform()
	{
		baseLocalY = transform.localPosition.y;
		baseLocalRotation = transform.localRotation;
		hasAppliedRotation = false;
	}

	private void Awake()
	{
		CaptureBaseTransform();
	}

	private void LateUpdate()
	{
		float time = useUnscaledTime ? Time.unscaledTime : Time.time;
		Vector3 position = transform.localPosition;
		position.y = baseLocalY + Mathf.Sin(time * hoverSpeed) * hoverHeight;
		transform.localPosition = position;
		if (!hasAppliedRotation || Quaternion.Angle(transform.localRotation, lastAppliedRotation) > 0.01f)
		{
			baseLocalRotation = transform.localRotation;
		}
		Quaternion sway = Quaternion.Euler(0f, 0f, Mathf.Sin(time * swaySpeed) * swayAngle);
		if (rotationSpeed > 0f)
		{
			transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, time * rotationSpeed, 0f) * sway;
		}
		else
		{
			transform.localRotation = baseLocalRotation * sway;
		}
		lastAppliedRotation = transform.localRotation;
		hasAppliedRotation = true;
	}
}
