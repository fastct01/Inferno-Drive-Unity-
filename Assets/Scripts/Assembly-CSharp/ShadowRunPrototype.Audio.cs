using System;
using UnityEngine;
using DynamicWeatherSystem;
using ShadowRun.Services;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_InitializeVehicleEngineAudio, _CreateVehicleEngineSource,
// _StartVehicleEngineAudio, _UpdateVehicleEngineAudio, _TriggerVehicleEngineImpact, _SetVehicleEngineLoopTargets,
// _SilenceVehicleEngineAudio, _InitializeBackgroundMusic, _UpdateBackgroundMusic, _InitializeGameAudioIntegration,
// _ConfigureNativeGameAudioSession, _RefreshExternalAudioState, _ToggleGameSound, _ApplyGameSoundState,
// _UpdateWeatherAudioMute, _ApplyRainAudio.
public sealed partial class ShadowRunPrototype
{
	private void InitializeVehicleEngineAudio()
	{
		AudioClip startup = Resources.Load<AudioClip>(SportCarEngineAudioResource + "startup");
		AudioClip idle = Resources.Load<AudioClip>(SportCarEngineAudioResource + "idle");
		AudioClip highOn = Resources.Load<AudioClip>(SportCarEngineAudioResource + "high_on");
		AudioClip highOff = Resources.Load<AudioClip>(SportCarEngineAudioResource + "high_off");
		if (startup == null || idle == null || highOn == null || highOff == null)
		{
			Debug.LogWarning("[VEHICLE AUDIO] SportCar engine sound set is incomplete.", this);
			return;
		}
		Transform root = new GameObject("SportCar Engine Audio").transform;
		root.SetParent(transform, false);
		vehicleEngineStartupSource = CreateVehicleEngineSource(root, "Startup", startup, false);
		vehicleEngineIdleSource = CreateVehicleEngineSource(root, "Idle Loop", idle, true);
		vehicleEngineOnSource = CreateVehicleEngineSource(root, "Engine Load Loop", highOn, true);
		vehicleEngineOffSource = CreateVehicleEngineSource(root, "Engine Coast Loop", highOff, true);
		vehicleEngineLastSpeed = speed;
		vehicleEngineLoopsStarted = false;
	}

	private AudioSource CreateVehicleEngineSource(Transform parent, string sourceName, AudioClip clip, bool loop)
	{
		GameObject sourceObject = new GameObject(sourceName);
		sourceObject.transform.SetParent(parent, false);
		AudioSource source = sourceObject.AddComponent<AudioSource>();
		source.clip = clip;
		source.loop = loop;
		source.playOnAwake = false;
		source.spatialBlend = 0f;
		source.dopplerLevel = 0f;
		source.priority = 96;
		source.volume = 0f;
		return source;
	}

	private void StartVehicleEngineAudio()
	{
		SilenceVehicleEngineAudio(true);
		vehicleEngineLastSpeed = speed;
		vehicleEngineImpactOffRemaining = 0f;
		if (!gameSoundEnabled || vehicleEngineStartupSource == null || string.Equals(ActiveVehicle.Id, "flying_car", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		vehicleEngineStartupSource.volume = Mathf.Clamp01(sfxVolume / 100f) * VehicleEngineVolumeScale * 0.72f;
		vehicleEngineStartupSource.Play();
	}

	private void UpdateVehicleEngineAudio(float dt)
	{
		if (vehicleEngineIdleSource == null || vehicleEngineOnSource == null || vehicleEngineOffSource == null)
		{
			return;
		}
		float sfx = Mathf.Clamp01(sfxVolume / 100f);
		float engineVolume = sfx * VehicleEngineVolumeScale;
		bool engineAudible = gameSoundEnabled && !string.Equals(ActiveVehicle.Id, "flying_car", StringComparison.OrdinalIgnoreCase);
		if (!running || paused || !engineAudible)
		{
			SilenceVehicleEngineAudio(false);
			vehicleEngineLastSpeed = speed;
			vehicleEngineImpactOffRemaining = 0f;
			return;
		}
		if (vehicleEngineStartupSource != null && vehicleEngineStartupSource.isPlaying)
		{
			vehicleEngineStartupSource.volume = engineVolume * 0.72f;
			SetVehicleEngineLoopTargets(0f, 0f, 0f, dt);
			vehicleEngineLastSpeed = speed;
			return;
		}
		if (!vehicleEngineLoopsStarted)
		{
			vehicleEngineIdleSource.Play();
			vehicleEngineOnSource.Play();
			vehicleEngineOffSource.Play();
			vehicleEngineLoopsStarted = true;
		}
		float safeDt = Mathf.Max(0.001f, dt);
		float acceleration = (speed - vehicleEngineLastSpeed) / safeDt;
		vehicleEngineLastSpeed = speed;
		vehicleEngineImpactOffRemaining = Mathf.Max(0f, vehicleEngineImpactOffRemaining - dt);
		float speedT = Mathf.InverseLerp(4f, 49f, speed);
		bool boostHeld = touchBoostHeld || KeyHeld(32, "space") || KeyHeld(273, "up");
		float load = 0.58f;
		if (touchBoostHeld)
		{
			load = 1f;
		}
		if (acceleration > 0.35f)
		{
			load = Mathf.Max(load, Mathf.InverseLerp(0.35f, 8f, acceleration));
		}
		if (brakePressure > 0.05f || acceleration < -0.35f)
		{
			load = Mathf.Min(load, 1f - Mathf.Max(brakePressure, Mathf.InverseLerp(-0.35f, -10f, acceleration)));
		}
		if (vehicleEngineImpactOffRemaining > 0f)
		{
			load = 0f;
		}
		load = Mathf.Clamp01(load);
		float idleWeight = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(3f, 17f, speed));
		float speedGain = Mathf.Lerp(0.34f, 1f, speedT);
		float idleVolume = sfx * 0.42f * idleWeight;
		float boostDuck = boostHeld ? VehicleEngineAccelerationVolumeScale : 1f;
		float onVolume = engineVolume * 0.408f * speedGain * load * boostDuck;
		float offVolume = engineVolume * 0.336f * speedGain * (1f - load);
		vehicleEngineIdleSource.pitch = Mathf.Lerp(0.92f, 1.08f, speedT);
		vehicleEngineOnSource.pitch = Mathf.Lerp(0.62f, 1.38f, speedT);
		vehicleEngineOffSource.pitch = Mathf.Lerp(0.66f, 1.26f, speedT);
		SetVehicleEngineLoopTargets(idleVolume, onVolume, offVolume, dt);
	}

	private void TriggerVehicleEngineImpact(bool sideImpact)
	{
		vehicleEngineImpactOffRemaining = Mathf.Max(vehicleEngineImpactOffRemaining, sideImpact ? 0.48f : 0.9f);
	}

	private void SetVehicleEngineLoopTargets(float idle, float onLoad, float offLoad, float dt)
	{
		float blend = 1f - Mathf.Exp(-Mathf.Max(0f, dt) * 6.5f);
		vehicleEngineIdleSource.volume = Mathf.Lerp(vehicleEngineIdleSource.volume, idle, blend);
		vehicleEngineOnSource.volume = Mathf.Lerp(vehicleEngineOnSource.volume, onLoad, blend);
		vehicleEngineOffSource.volume = Mathf.Lerp(vehicleEngineOffSource.volume, offLoad, blend);
	}

	private void SilenceVehicleEngineAudio(bool stopStartup)
	{
		if (vehicleEngineStartupSource != null)
		{
			vehicleEngineStartupSource.volume = 0f;
			if (stopStartup)
			{
				vehicleEngineStartupSource.Stop();
			}
		}
		AudioSource[] loops = new AudioSource[3] { vehicleEngineIdleSource, vehicleEngineOnSource, vehicleEngineOffSource };
		for (int i = 0; i < loops.Length; i++)
		{
			if (loops[i] != null)
			{
				loops[i].volume = 0f;
			}
		}
	}

	private void InitializeBackgroundMusic()
	{
		backgroundMusicClips = new AudioClip[2] { Resources.Load<AudioClip>("gamebeat1"), Resources.Load<AudioClip>("gamebeat2") };
		GameObject musicObject = new GameObject("Background Music");
		musicObject.transform.SetParent(transform, false);
		backgroundMusicSource = musicObject.AddComponent<AudioSource>();
		backgroundMusicSource.playOnAwake = false;
		backgroundMusicSource.loop = false;
		backgroundMusicSource.spatialBlend = 0f;
		backgroundMusicSource.dopplerLevel = 0f;
		backgroundMusicSource.priority = 0;
		backgroundMusicSource.ignoreListenerPause = true;
		backgroundMusicSource.volume = Mathf.Clamp01(musicVolume / 100f * 1.1f);
		backgroundMusicTrackIndex = 0;
		backgroundMusicGapRemaining = 0f;
		backgroundMusicTrackActive = false;
		backgroundMusicPaused = false;
	}

	private void UpdateBackgroundMusic(float unscaledDt)
	{
		if (backgroundMusicSource == null || backgroundMusicClips == null)
		{
			return;
		}
		RefreshExternalAudioState(false);
		backgroundMusicSource.volume = Mathf.Clamp01(musicVolume / 100f * 1.1f);
		bool authFormOpen = menuTargetTab == 2 && profileAuthFormRoot != null && profileAuthFormRoot.activeInHierarchy;
		bool suppressed = paused || !gameSoundEnabled || externalAudioPlaying || caseMusicSuppressed;
		bool shouldPause = suppressed || authFormOpen || FirstLaunchAuthScreen.IsVisible;
		if (shouldPause)
		{
			if (!backgroundMusicPaused && backgroundMusicTrackActive && backgroundMusicSource.isPlaying)
			{
				backgroundMusicSource.Pause();
				backgroundMusicPaused = true;
			}
			return;
		}
		if (backgroundMusicPaused)
		{
			backgroundMusicSource.UnPause();
			backgroundMusicPaused = false;
			return;
		}
		if (backgroundMusicTrackActive)
		{
			if (backgroundMusicSource.isPlaying)
			{
				return;
			}
			backgroundMusicTrackActive = false;
			backgroundMusicGapRemaining = BackgroundMusicGapSeconds;
			backgroundMusicTrackIndex = (backgroundMusicTrackIndex + 1) % backgroundMusicClips.Length;
		}
		if (backgroundMusicGapRemaining > 0f)
		{
			backgroundMusicGapRemaining = Mathf.Max(0f, backgroundMusicGapRemaining - Mathf.Max(0f, unscaledDt));
			if (backgroundMusicGapRemaining > 0f)
			{
				return;
			}
		}
		AudioClip clip = backgroundMusicClips[backgroundMusicTrackIndex];
		if (clip == null)
		{
			backgroundMusicTrackIndex = (backgroundMusicTrackIndex + 1) % backgroundMusicClips.Length;
			backgroundMusicGapRemaining = BackgroundMusicGapSeconds;
			return;
		}
		backgroundMusicSource.clip = clip;
		backgroundMusicSource.Play();
		backgroundMusicTrackActive = true;
	}

	private void InitializeGameAudioIntegration()
	{
		ConfigureNativeGameAudioSession();
		RefreshExternalAudioState(true);
		ApplyGameSoundState(false);
	}

	private void ConfigureNativeGameAudioSession()
	{
		ShadowRun_ConfigureGameAudioSession();
	}

	private void RefreshExternalAudioState(bool force = false)
	{
		if (!force && running && !paused)
		{
			return;
		}
		if (!force && Time.unscaledTime < nextExternalAudioCheck)
		{
			return;
		}
		nextExternalAudioCheck = Time.unscaledTime + 1f;
		externalAudioPlaying = ShadowRun_IsOtherAudioPlaying() > 0;
	}

	private void ToggleGameSound()
	{
		gameSoundEnabled = !gameSoundEnabled;
		ApplyGameSoundState(true);
	}

	private void ApplyGameSoundState(bool save)
	{
		// original: AudioListener.volume = gameSoundEnabled ? 1f : 0f
		AudioListener.volume = InfernoSettings.ListenerVolume(this); /* [InfernoLobby patch] */
		if (!gameSoundEnabled)
		{
			SilenceVehicleEngineAudio(false);
		}
		Sprite icon = LoadResourceSprite(gameSoundEnabled ? "звук есть" : "звука нет");
		if (profileSoundIcon != null)
		{
			profileSoundIcon.sprite = icon;
		}
		if (pauseSoundIcon != null)
		{
			pauseSoundIcon.sprite = icon;
		}
		if (!save)
		{
			return;
		}
		PlayerPrefs.SetInt("shadowRunSoundEnabled", gameSoundEnabled ? 1 : 0);
		PlayerPrefs.Save();
	}

	private void UpdateWeatherAudioMute()
	{
		if (weatherAudioModule == null)
		{
			return;
		}
		bool muted = !running || paused;
		if (weatherAudioMuteInitialized && weatherAudioMuted == muted)
		{
			return;
		}
		weatherAudioModule.SetMuted(muted);
		weatherAudioMuted = muted;
		weatherAudioMuteInitialized = true;
	}

	private void ApplyRainAudio()
	{
		AudioClip clip = Resources.Load<AudioClip>(RainAudioResource);
		if (clip == null)
		{
			Debug.LogWarning("[WEATHER] Rain audio loop not found: WeatherAudio/low_rain_loop", this);
			return;
		}
		weatherRainPreset.ambientClip = clip;
		weatherStormPreset.ambientClip = clip;
	}
}
