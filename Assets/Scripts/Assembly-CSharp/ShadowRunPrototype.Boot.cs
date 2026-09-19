using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using ShadowRun.Services;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_AutoStart, _Awake, _LockPortraitOrientation,
// _ConfigurePerformance, _UpdateTargetFrameRateForState, _Update, _LateUpdate, _KeyDown, _KeyHeld,
// _OnDestroy, _OnApplicationQuit, _OnApplicationFocus, _OnApplicationPause, _MakeEvents,
// _DisableNonGameplayCameras, _DisableImportedModelCamerasAndLights, _SyncSelectiveOverlayCamera,
// _DisableRuntimePostProcessing, _MakeCamera, _MakeLights, _ResetRun, _StartRun, _ResumeRun, _PauseRun,
// _ShowMainMenu, _MenuPanelOpen, _GameOver, _FormatGameOverDistance, _SetGameplayCameraRendering,
// _ShowGarageNote, _ShowStatsNote, _ShowSettingsNote, _Reward.
public sealed partial class ShadowRunPrototype
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void AutoStart()
	{
		if (UnityEngine.Object.FindFirstObjectByType<ShadowRunPrototype>() != null)
		{
			return;
		}
		new GameObject("Shadow Run Prototype").AddComponent<ShadowRunPrototype>();
	}

	private void Awake()
	{
		LockPortraitOrientation();
		cachedKeyboard = Keyboard.current;
		ConfigurePerformance();
		MakeMaterials();
		MakeCamera();
		MakeLights();
		DisableRuntimePostProcessing();
		InitializeDynamicWeatherSystem();
		InitializeCloudProfile();
		MakeRoad();
		MakeCity();
		RefreshRoadReflectionProbe();
		MakeCar();
		MakeEvents();
		MakeUI();
		best = PlayerPrefs.GetInt("shadowRunBest", 0);
		LoadSettings();
		InitializeGameAudioIntegration();
		InitializeVehicleEngineAudio();
		InitializeBackgroundMusic();
		ApplySettings();
		ResetRun();
		DisableNonGameplayCameras();
		ShowMainMenu();
	}

	private void LockPortraitOrientation()
	{
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.orientation = ScreenOrientation.Portrait;
#if UNITY_IOS
		Device.deferSystemGesturesMode = SystemGestureDeferMode.LeftEdge | SystemGestureDeferMode.RightEdge;   // UnityEngine.iOS only exists for the iOS player (the shipped build was iOS-only)
#endif
	}

	private void Update()
	{
		/* [InfernoLobby patch] */ InfernoStyle.Frame(this);
		/* [InfernoLobby patch] */ InfernoSettings.Tick(this);
		UpdateTargetFrameRateForState();
		float dt = Mathf.Min(Time.deltaTime, MaxGameplayDeltaTime);
		UpdateDynamicWeatherSystem(dt);
		UpdateWeatherAudioMute();
		UpdateVehicleEngineAudio(dt);
		UpdateBackgroundMusic(Time.unscaledDeltaTime);
		if (!running)
		{
			if (paused)
			{
				if (KeyDown(32, "space") || KeyDown(13, "enter"))
				{
					ResumeRun();
				}
			}
			else if (KeyDown(27, "escape") && MenuPanelOpen())
			{
				if (garageRoot != null && garageRoot.activeSelf)
				{
					HideGarageScreen();
				}
				else if (settingsRoot != null && settingsRoot.activeSelf)
				{
					HideSettingsScreen();
				}
			}
			else if (KeyDown(32, "space") || KeyDown(13, "enter"))
			{
				if (!MenuPanelOpen())
				{
					StartRun();
				}
			}
		}
		else
		{
			if (KeyDown(27, "escape"))
			{
				PauseRun();
				UpdateEvents(dt);
				UpdateHud();
				return;
			}
			UpdateFirstRunTutorial(Time.unscaledDeltaTime);
			float gameplayDt = dt * FirstRunTutorialGameplayScale;
			UpdatePlayer(gameplayDt);
			UpdateWorld(gameplayDt);
			UpdateRamps(gameplayDt);
			UpdateSpawns(gameplayDt);
			UpdateHighBeams(gameplayDt);
			UpdateCamera(gameplayDt);
			UpdateSkyClouds(gameplayDt);
			if (lifeHalfUnits <= 0)
			{
				GameOver();
			}
		}
		UpdateEvents(dt);
		UpdateMenuAnimation(dt);
		UpdateGaragePreview(dt);
		UpdateGarageVehicleCarousel(dt);
		UpdateHud();
	}

	private void LateUpdate()
	{
		if (cachedKeyboard == null)
		{
			cachedKeyboard = Keyboard.current;
		}
		SyncSelectiveOverlayCamera();
	}

	private void UpdateTargetFrameRateForState()
	{
		int target = (!running || paused) ? MenuTargetFrameRate : TargetFrameRate;
		if (appliedTargetFrameRate == target)
		{
			return;
		}
		Application.targetFrameRate = target;
		appliedTargetFrameRate = target;
	}

	private void ConfigurePerformance()
	{
		int quality = Mathf.Clamp(graphicsIndex, 0, 2);
		Application.targetFrameRate = TargetFrameRate;
		appliedTargetFrameRate = TargetFrameRate;
		QualitySettings.vSyncCount = 0;
		QualitySettings.pixelLightCount = quality >= 2 ? 2 : 1;
		QualitySettings.realtimeReflectionProbes = false;
		QualitySettings.softParticles = false;
		QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
		QualitySettings.anisotropicFiltering = quality >= 2 ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Enable;
		QualitySettings.globalTextureMipmapLimit = 0;
		QualitySettings.lodBias = quality == 0 ? 0.85f : (quality == 1 ? 1.25f : 1.75f);
		Time.fixedDeltaTime = 1f / 60f;
		Time.maximumDeltaTime = 1f / 30f;
		Physics.autoSimulation = false;
		Physics.defaultSolverIterations = 3;
		Physics.defaultSolverVelocityIterations = 1;
	}

	private bool KeyHeld(int keyCode, string key)
	{
		if (cachedKeyboard == null)
		{
			cachedKeyboard = Keyboard.current;
		}
		Keyboard keyboard = cachedKeyboard;
		if (keyboard == null)
		{
			return false;
		}
		if (key == "left") return keyboard.leftArrowKey.isPressed;
		if (key == "right") return keyboard.rightArrowKey.isPressed;
		if (key == "up") return keyboard.upArrowKey.isPressed;
		if (key == "down") return keyboard.downArrowKey.isPressed;
		if (key == "space") return keyboard.spaceKey.isPressed;
		if (key == "shift") return keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
		if (key == "a") return keyboard.aKey.isPressed;
		if (key == "s") return keyboard.sKey.isPressed;
		if (key == "d") return keyboard.dKey.isPressed;
		if (key == "f") return keyboard.fKey.isPressed;
		if (key == "enter") return keyboard.enterKey.isPressed;
		return false;
	}

	private bool KeyDown(int keyCode, string key)
	{
		if (cachedKeyboard == null)
		{
			cachedKeyboard = Keyboard.current;
		}
		Keyboard keyboard = cachedKeyboard;
		if (keyboard == null)
		{
			return false;
		}
		if (key == "space") return keyboard.spaceKey.wasPressedThisFrame;
		if (key == "f") return keyboard.fKey.wasPressedThisFrame;
		if (key == "enter") return keyboard.enterKey.wasPressedThisFrame;
		if (key == "escape") return keyboard.escapeKey.wasPressedThisFrame;
		return false;
	}

	private void OnDestroy()
	{
		/* [InfernoLobby patch] */ InfernoStyle.Dispose(this);
		/* [InfernoLobby patch] */ InfernoHazards.Dispose(this);
		/* [InfernoLobby patch] */ InfernoDriving.Dispose(this);
		ClearP900BrakeLightMaterial();
		SupabaseGameManager.ProfileChanged -= ApplyCloudProfile;
		SupabaseGameManager.AuthenticationStatusChanged -= SetProfileAuthenticationStatus;
		SupabaseGameManager.AuthenticationCompleted -= RefreshProfileUi;
	}

	private void OnApplicationQuit()
	{
		SaveDailyGoalProgress();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus)
		{
			SaveDailyGoalProgress();
			ResetGameplayTouches();
			return;
		}
		LockPortraitOrientation();
		ConfigureNativeGameAudioSession();
		RefreshExternalAudioState(true);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			return;
		}
		SaveDailyGoalProgress();
		ResetGameplayTouches();
	}

	private void MakeEvents()
	{
		santa = new GameObject("Santa").transform;
		santa.gameObject.SetActive(false);
	}

	private void DisableNonGameplayCameras()
	{
		Camera[] cameras = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
		for (int i = 0; i < cameras.Length; i++)
		{
			Camera camera = cameras[i];
			if (camera != cam && camera != pulsarNightCamera)
			{
				camera.enabled = false;
			}
		}
	}

	private void DisableImportedModelCamerasAndLights(GameObject root)
	{
		Camera[] cameras = root.GetComponentsInChildren<Camera>(true);
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].enabled = false;
		}
		Light[] lights = root.GetComponentsInChildren<Light>(true);
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].enabled = false;
		}
	}

	private void SyncSelectiveOverlayCamera()
	{
		if (cam == null)
		{
			return;
		}
		if (pulsarNightCamera != null)
		{
			pulsarNightCamera.fieldOfView = cam.fieldOfView;
			pulsarNightCamera.aspect = cam.aspect;
			pulsarNightCamera.farClipPlane = cam.farClipPlane;
		}
	}

	private void DisableRuntimePostProcessing()
	{
		ApplyVisualQuality();
	}

	private void MakeCamera()
	{
		cam = Camera.main;
		if (cam == null)
		{
			cam = new GameObject("Main Camera").AddComponent<Camera>();
		}
		cam.tag = "MainCamera";
		cam.fieldOfView = 65f;
		cam.nearClipPlane = 0.1f;
		cam.farClipPlane = 900f;
		cam.allowHDR = true;
		cam.clearFlags = CameraClearFlags.Color;
		cam.backgroundColor = new Color(0.1f, 0.01f, 0.015f);
		UniversalAdditionalCameraData cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
		if (cameraData == null)
		{
			cameraData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
		}
		cameraData.renderPostProcessing = true;
		cam.transform.position = new Vector3(0f, 7.5f, 15f);
		cam.transform.LookAt(new Vector3(0f, 0.5f, -28f));
		ResetCameraImmediate();
	}

	private void MakeLights()
	{
		Light[] lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].enabled = false;
		}
		RenderSettings.fog = false;
		RenderSettings.ambientLight = new Color(0.07f, 0.026f, 0.036f);
		sceneKeyLight = new GameObject("Red Rim Light").AddComponent<Light>();
		sceneKeyLight.type = LightType.Directional;
		sceneKeyLight.color = new Color(1f, 0.28f, 0.24f);
		sceneKeyLight.intensity = 1.42f;
		sceneKeyLight.transform.rotation = Quaternion.Euler(25f, -165f, 0f);
	}

	private void ResetRun()
	{
		/* [InfernoLobby patch] */ InfernoDrivingRules.ResetRecovery();
		ResetDailyGoalRunState();
		playerX = 0f;
		velocityX = 0f;
		speed = 0f; /* [InfernoLobby patch] original: 15f */
		steerInput = 0f;
		brakePressure = 0f;
		brakeRecoveryActive = false;
		trafficIncidentCooldown = 0f;
		playerStoppedTrafficTimer = 0f;
		nextTrafficWowTime = Time.unscaledTime + UnityEngine.Random.Range(30f, 45f);
		playerAirborne = false;
		playerLandedThisFrame = false;
		playerLandingDownSpeed = 0f;
		playerJumpHeight = 0f;
		playerJumpVelocity = 0f;
		playerJumpPitch = 0f;
		playerJumpRoll = 0f;
		playerJumpRollVelocity = 0f;
		playerJumpYaw = 0f;
		playerRampRiding = false;
		playerRampRideElapsed = 0f;
		playerRampRideDuration = 0f;
		playerRampRiseHeight = 0f;
		playerRampTakeoffVelocity = 0f;
		playerJumpPitchVelocity = 0f;
		rampSpawnCount = 0;
		nextRampDistance = UnityEngine.Random.Range(1000f, 1200f);
		brakeButtonHeld = false;
		ResetGameplayTouches();
		DisableHighBeam();
		ResetHighBeamAbility();
		highBeamCourtesyNext = true;
		drift = 0f;
		driftChainTimer = 0f;
		yawVisual = 0f;
		rollVisual = 0f;
		impactYaw = 0f;
		impactRoll = 0f;
		impactShake = 0f;
		tractionLoss = 0f;
		lifeHalfUnits = 6;
		invuln = 0f;
		rush = 42f;
		lastSpeedometerKmh = -1;
		lastSpeedFillAmount = -1f;
		lastRushFillAmount = -1f;
		lastSpeedometerNeedleAngle = 999f;
		lastHudScore = -1;
		lastHudBest = -1;
		lastHudLifeHalfUnits = -1;
		lastOverlayScore = -1;
		lastOverlayBest = -1;
		lastOverlayCombo = -1;
		distance = 0f;
		score = 0f;
		combo = 0;
		comboTimer = 0f;
		spawnTimer = 0f;
		lastTrafficPattern = -1;
		ResetTrafficLights();
		EnsureVehicleSmoke();
		foreach (Pickup pickup in pickups)
		{
			if (pickup.Transform != null)
			{
				ReleaseTrafficCar(pickup.Transform.gameObject);
			}
		}
		pickups.Clear();
		PrewarmTrafficPool();
		PrewarmRampPool();
		ReleaseAllRamps();
		ResetVehicleSmoke();
		smokeLastSpeed = speed;
		player.position = new Vector3(0f, 0f, 5f);
		carVisual.localPosition = new Vector3(0f, CurrentPlayerVehicleRoadYOffset(), 0f);
		carVisual.localRotation = PlayerVehicleModelBaseRotation();
		SetPlayerHoverSuppressedForJump(false);
		playerWheelSpin = 0f;
		SetPlayerWheelSpin(playerWheelSpin);
		SetPlayerBrakeLights(false);
		carVisual.gameObject.SetActive(true);
		ResetCameraImmediate();
		DisableNonGameplayCameras();
		SeedInitialTraffic();
		/* [InfernoLobby patch] */ InfernoDriving.Reset(this);
		/* [InfernoLobby patch] */ InfernoHazards.Reset(this);
		/* [InfernoLobby patch] */ InfernoStyle.ResetTrails(this);
	}

	private void StartRun()
	{
		/* [InfernoLobby patch] */ if (InfernoLobby.HandleLockedMap(this)) return;
		/* [InfernoLobby patch] */ InfernoLobby.BeginRun(this);
		ResetRun();
		SetGameplayCameraRendering(true);
		running = true;
		paused = false;
		showingGameOver = false;
		if (gameOverRoot != null)
		{
			gameOverRoot.gameObject.SetActive(false);
		}
		if (pauseActionsRoot != null)
		{
			pauseActionsRoot.SetActive(false);
		}
		SetGaragePreviewVisible(false);
		overlay.SetActive(false);
		hudRoot.SetActive(true);
		StartVehicleEngineAudio();
		BeginFirstRunTutorialIfNeeded();
	}

	private void ResumeRun()
	{
		SetGameplayCameraRendering(true);
		running = true;
		paused = false;
		if (gameOverRoot != null)
		{
			gameOverRoot.gameObject.SetActive(false);
		}
		if (pauseActionsRoot != null)
		{
			pauseActionsRoot.SetActive(false);
		}
		SetGaragePreviewVisible(false);
		overlay.SetActive(false);
		hudRoot.SetActive(true);
		RestoreFirstRunTutorialAfterPause();
	}

	private void PauseRun()
	{
		if (!running)
		{
			return;
		}
		SaveDailyGoalProgress();
		running = false;
		paused = true;
		brakeButtonHeld = false;
		ResetGameplayTouches();
		DisableHighBeam();
		HideFirstRunTutorialForPause();
		showingGameOver = false;
		if (gameOverRoot != null)
		{
			gameOverRoot.gameObject.SetActive(false);
		}
		hudRoot.SetActive(false);
		if (menuTitleRoot != null)
		{
			menuTitleRoot.gameObject.SetActive(false);
		}
		if (menuTabsRoot != null)
		{
			menuTabsRoot.gameObject.SetActive(false);
		}
		if (menuTabBarRoot != null)
		{
			menuTabBarRoot.gameObject.SetActive(false);
		}
		if (menuSceneRoot != null)
		{
			menuSceneRoot.gameObject.SetActive(false);
		}
		SetGaragePreviewVisible(false);
		overlayTitle.text = "P  A  U  S  E  D";
		overlayTitle.fontSize = 76;
		overlayTitle.fontStyle = FontStyle.Bold;
		overlayTag.text = "THE CITY IS STILL MOVING";
		overlayTag.color = Color.white;
		int scoreMeters = Mathf.FloorToInt(score);
		overlayStats.text = scoreMeters.ToString() + "m     BEST " + best.ToString() + "m";
		primaryButtonLabel.text = "C  O  N  T  I  N  U  E";
		primaryOverlayButton.onClick.RemoveAllListeners();
		primaryOverlayButton.onClick.AddListener(ResumeRun);
		if (pauseActionsRoot != null)
		{
			pauseActionsRoot.SetActive(true);
			pauseActionsRoot.transform.SetAsLastSibling();
		}
		if (overlayTitle != null)
		{
			overlayTitle.transform.SetAsLastSibling();
		}
		if (overlayTag != null)
		{
			overlayTag.transform.SetAsLastSibling();
		}
		overlay.SetActive(true);
	}

	private void ShowMainMenu()
	{
		/* [InfernoLobby patch] */ InfernoStyle.HideTrails(this);
		/* [InfernoLobby patch] */ InfernoHazards.Hide(this);
		/* [InfernoLobby patch] */ InfernoDriving.Hide(this);
		running = false;
		paused = false;
		showingGameOver = false;
		if (gameOverRoot != null)
		{
			gameOverRoot.gameObject.SetActive(false);
		}
		if (pauseActionsRoot != null)
		{
			pauseActionsRoot.SetActive(false);
		}
		brakeButtonHeld = false;
		ResetGameplayTouches();
		DisableHighBeam();
		CancelFirstRunTutorial();
		hudRoot.SetActive(false);
		if (menuTabsRoot != null)
		{
			menuTabsRoot.gameObject.SetActive(true);
		}
		if (menuTabBarRoot != null)
		{
			menuTabBarRoot.gameObject.SetActive(true);
		}
		if (menuSceneRoot != null)
		{
			menuSceneRoot.gameObject.SetActive(true);
		}
		if (menuTitleRoot != null)
		{
			menuTitleRoot.gameObject.SetActive(false);
		}
		if (menuButtonRoot != null)
		{
			menuButtonRoot.gameObject.SetActive(true);
		}
		if (menuDetails != null)
		{
			menuDetails.SetActive(false);
		}
		overlayTitle.text = "";
		overlayTag.text = "";
		overlayStats.text = "";
		primaryButtonLabel.text = "P  L  A  Y";
		primaryOverlayButton.onClick.RemoveAllListeners();
		primaryOverlayButton.onClick.AddListener(StartRun);
		overlay.SetActive(true);
		SelectMenuTab(1, true);
		SetGameplayCameraRendering(false);
	}

	private void SetGameplayCameraRendering(bool visible)
	{
		if (cam != null)
		{
			cam.enabled = visible;
		}
		if (!visible)
		{
			if (pulsarNightCamera != null)
			{
				SetPulsarNightCameraActive(false);
			}
			return;
		}
		bool pulsarVisible = pulsarNightRoot != null && pulsarNightRoot.gameObject.activeSelf && pulsarNightVisibility > 0.001f;
		if (pulsarVisible)
		{
			SetPulsarNightCameraActive(true);
		}
	}

	private bool MenuPanelOpen()
	{
		/* [InfernoLobby patch] */ if (InfernoStyle.IsOpen(this)) return true;
		/* [InfernoLobby patch] */ if (caseOverlayRoot != null && caseOverlayRoot.activeInHierarchy) return true;
		if (overlay != null && overlay.activeSelf)
		{
			return menuTargetTab != 1;
		}
		return false;
	}

	private void ShowGarageNote()
	{
		overlayStats.text = "GARAGE     COMING SOON";
	}

	private void ShowStatsNote()
	{
		overlayStats.text = "BEST " + best.ToString() + "m     SCORE " + Mathf.FloorToInt(score).ToString() + "m";
	}

	private void ShowSettingsNote()
	{
		overlayStats.text = "SETTINGS     SOUND / CONTROLS";
	}

	private void GameOver()
	{
		/* [InfernoLobby patch] */ InfernoStyle.HideTrails(this);
		/* [InfernoLobby patch] */ InfernoHazards.Hide(this);
		/* [InfernoLobby patch] */ InfernoDriving.Hide(this);
		RegisterDailyRunCompleted();
		running = false;
		paused = false;
		showingGameOver = true;
		if (pauseActionsRoot != null)
		{
			pauseActionsRoot.SetActive(false);
		}
		brakeButtonHeld = false;
		ResetGameplayTouches();
		DisableHighBeam();
		CancelFirstRunTutorial();
		hudRoot.SetActive(false);
		if (menuTitleRoot != null)
		{
			menuTitleRoot.gameObject.SetActive(false);
		}
		if (menuTabsRoot != null)
		{
			menuTabsRoot.gameObject.SetActive(false);
		}
		if (menuTabBarRoot != null)
		{
			menuTabBarRoot.gameObject.SetActive(false);
		}
		SetGaragePreviewVisible(false);
		best = Mathf.Max(best, Mathf.FloorToInt(score));
		PlayerPrefs.SetInt("shadowRunBest", best);
		overlayTitle.text = "";
		overlayTag.text = "";
		overlayStats.text = "";
		if (gameOverDistanceText != null)
		{
			gameOverDistanceText.text = FormatGameOverDistance(distance);
		}
		if (gameOverBestText != null)
		{
			gameOverBestText.text = FormatGameOverDistance(best);
		}
		if (menuSceneRoot != null)
		{
			menuSceneRoot.gameObject.SetActive(false);
		}
		if (gameOverRoot != null)
		{
			gameOverRoot.gameObject.SetActive(true);
			gameOverRoot.SetAsLastSibling();
		}
		overlay.SetActive(true);
	}

	private string FormatGameOverDistance(float meters)
	{
		float km = Mathf.Max(0f, meters) / 1000f;
		return km.ToString("0.00", CultureInfo.InvariantCulture) + " KM";
	}

	private void Reward(float amount)
	{
		rush = Mathf.Min(100f, rush + amount);
	}
}
