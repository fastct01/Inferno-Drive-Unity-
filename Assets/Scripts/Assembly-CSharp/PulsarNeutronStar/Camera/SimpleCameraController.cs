using UnityEngine;

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
			}

			public void Translate(Vector3 translation)
			{
			}

			public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
			{
			}

			public void UpdateTransform(Transform t)
			{
			}
		}

		private CameraState m_TargetCameraState;

		private CameraState m_InterpolatingCameraState;

		public float movementSpeed;

		public float boost;

		public float positionLerpTime;

		public AnimationCurve mouseSensitivityCurve;

		public float rotationLerpTime;

		public bool invertY;

		public float scrollSensitivity;

		public float mouseSensitivityAccelerationTime;

		private float currentMouseSensitivity;

		private float targetMouseSensitivity;

		private float maxMouseSensitivity;

		private float currentSpeedMultiplier;

		private float targetSpeedMultiplier;

		private float speedAdjustmentRate;

		private void OnEnable()
		{
		}

		private Vector3 GetInputTranslationDirection()
		{
			return default;
		}

		private void Update()
		{
		}
	}
}
