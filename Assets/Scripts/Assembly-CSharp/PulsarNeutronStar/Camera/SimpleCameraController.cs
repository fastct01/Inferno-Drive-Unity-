using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__2.cpp:
//   SimpleCameraController__ctor_m0FD259DC281767714BB4B2E3FC566969EAE2D581
//   SimpleCameraController_OnEnable_mE719AB6911DEB3AAB7F88952A9D8ED8AFBE82A6C
//   SimpleCameraController_GetInputTranslationDirection_m43B88CE9B3739BF57FE313DA59AFF4B6FF74B187
//   SimpleCameraController_Update_mCAEBE69295DBAE3025909B70F2CC0DE06E902E51 (empty)
//   CameraState_SetFromTransform_m381B4C58AE68712321BA3B2888E7AD52A551C993
//   CameraState_Translate_mD9BBFEFCA940B762D63F2D6481CB32BBEE81D8E9
//   CameraState_LerpTowards_m0859D92472426EEF86DBF5096CB210809F299166
//   CameraState_UpdateTransform_m0A6955CCF6E0213E3DE06D5C61C9266824E32328
namespace PulsarNeutronStar.Camera
{
	public class SimpleCameraController : MonoBehaviour
	{
		private class CameraState
		{
			public float yaw;

			public float pitch;

			public float roll;

			public float x;

			public float y;

			public float z;

			public void SetFromTransform(Transform t)
			{
				pitch = t.eulerAngles.x;
				yaw = t.eulerAngles.y;
				roll = t.eulerAngles.z;
				x = t.position.x;
				y = t.position.y;
				z = t.position.z;
			}

			public void Translate(Vector3 translation)
			{
				Vector3 rotated = Quaternion.Euler(pitch, yaw, roll) * translation;
				x += rotated.x;
				y += rotated.y;
				z += rotated.z;
			}

			public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
			{
				yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
				pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
				roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
				x = Mathf.Lerp(x, target.x, positionLerpPct);
				y = Mathf.Lerp(y, target.y, positionLerpPct);
				z = Mathf.Lerp(z, target.z, positionLerpPct);
			}

			public void UpdateTransform(Transform t)
			{
				t.eulerAngles = new Vector3(pitch, yaw, roll);
				t.position = new Vector3(x, y, z);
			}
		}

		private CameraState m_TargetCameraState = new CameraState();

		private CameraState m_InterpolatingCameraState = new CameraState();

		public float movementSpeed = 10f;

		public float boost = 0.2f;

		public float positionLerpTime = 0.2f;

		public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

		public float rotationLerpTime = 0.01f;

		public bool invertY;

		public float scrollSensitivity = 0.1f;

		public float mouseSensitivityAccelerationTime = 1f;

		private float currentMouseSensitivity = 0.1f;

		private float targetMouseSensitivity = 0.5f;

		private float maxMouseSensitivity;

		private float currentSpeedMultiplier = 1f;

		private float targetSpeedMultiplier = 1f;

		private float speedAdjustmentRate = 1f;

		private void OnEnable()
		{
			m_TargetCameraState.SetFromTransform(transform);
			m_InterpolatingCameraState.SetFromTransform(transform);
			maxMouseSensitivity = mouseSensitivityCurve.Evaluate(1f);
		}

		private Vector3 GetInputTranslationDirection()
		{
			Vector3 direction = default(Vector3);
			if (Input.GetKey(KeyCode.W))
			{
				direction += Vector3.forward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				direction += Vector3.back;
			}
			if (Input.GetKey(KeyCode.A))
			{
				direction += Vector3.left;
			}
			if (Input.GetKey(KeyCode.D))
			{
				direction += Vector3.right;
			}
			if (Input.GetKey(KeyCode.Q))
			{
				direction += Vector3.down;
			}
			if (Input.GetKey(KeyCode.E))
			{
				direction += Vector3.up;
			}
			return direction;
		}

		private void Update()
		{
		}
	}
}
