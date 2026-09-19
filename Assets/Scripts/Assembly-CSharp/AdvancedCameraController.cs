using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   AdvancedCameraController__ctor_m9167CBFC12CAC0A450D48CEBB3C9F6A6106DA4EA
//   AdvancedCameraController_Start_mBFF474251E4DCECD02D9805B13F9FAF57E8FE6EF
//   AdvancedCameraController_Update_m32B9E6C61F82DFED15DF268C517253740C1320E4
//   AdvancedCameraController_ApplyOptimalSettings_m6C7292C53C14CA2D168E1B8306EE99DDDE3A4F18
//   AdvancedCameraController_ReduceQuality_mF4BEBDA8033D2D53D1C6899B555EEFF7B83167B6
//   AdvancedCameraController_IncreaseQuality_mF1DA22B84B088CF95557E3D801E25B97C7B3A59C
//   AdvancedCameraController_SetTargetResolution_mBAC041DC74B57534BCE8776DFE55702598DBD162
//   AdvancedCameraController_GetCurrentFPS_m4FC01F52D8BAB7ABB721D69FBA7E84338374EF56
//   AdvancedCameraController_SetAdaptiveQuality_mF4F1E283830BADBDA5CA0B7F9655CE1A265380F0
public class AdvancedCameraController : MonoBehaviour
{
	private float minFPS = 30f;

	private float targetFPS = 60f;

	private bool adaptiveQuality = true;

	private bool enableMotionBlur;

	private Camera mainCamera;

	private int targetResolutionWidth = 1920;

	private int targetResolutionHeight = 1080;

	private bool lastAdaptiveState = true;

	private void Start()
	{
		mainCamera = GetComponent<Camera>();
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
		if (mainCamera != null)
		{
			mainCamera.stereoTargetEye = StereoTargetEyeMask.Both;
			mainCamera.depthTextureMode = DepthTextureMode.None;
			mainCamera.allowHDR = false;
			mainCamera.allowMSAA = false;
		}
		targetResolutionWidth = Screen.width;
		targetResolutionHeight = Screen.height;
		ApplyOptimalSettings();
	}

	private void Update()
	{
		// original: bool adaptive = adaptiveQuality;
		bool adaptive = false; /* [InfernoLobby patch] adaptive resolution toggling disabled */
		if (!adaptive)
		{
			return;
		}
		float fps = 1f / Time.deltaTime;
		if (fps < minFPS && lastAdaptiveState)
		{
			ReduceQuality();
			lastAdaptiveState = false;
			return;
		}
		if (fps > targetFPS && !lastAdaptiveState)
		{
			IncreaseQuality();
			lastAdaptiveState = true;
		}
	}

	public void ApplyOptimalSettings()
	{
		Application.targetFrameRate = (int)targetFPS;
		QualitySettings.vSyncCount = 0;
		QualitySettings.maxQueuedFrames = 2;
		Time.fixedDeltaTime = 1f / targetFPS;
		Time.maximumDeltaTime = 1f / minFPS;
		QualitySettings.softVegetation = false;
		QualitySettings.realtimeReflectionProbes = false;
	}

	private void ReduceQuality()
	{
		Debug.Log("Снижение качества графики для улучшения FPS...");
		QualitySettings.pixelLightCount = Mathf.Max(1, QualitySettings.pixelLightCount - 1);
		if (targetResolutionWidth > 1280)
		{
			targetResolutionWidth = (int)(targetResolutionWidth * 0.8f);
			targetResolutionHeight = (int)(targetResolutionHeight * 0.8f);
			Screen.SetResolution(targetResolutionWidth, targetResolutionHeight, Screen.fullScreen);
		}
	}

	private void IncreaseQuality()
	{
		Debug.Log("Повышение качества графики...");
		if (QualitySettings.pixelLightCount < 4)
		{
			QualitySettings.pixelLightCount++;
		}
	}

	public void SetTargetResolution(int width, int height)
	{
		targetResolutionWidth = width;
		targetResolutionHeight = height;
		Screen.SetResolution(width, height, Screen.fullScreen);
	}

	public float GetCurrentFPS()
	{
		return 1f / Time.deltaTime;
	}

	public void SetAdaptiveQuality(bool enabled)
	{
		adaptiveQuality = enabled;
	}
}
