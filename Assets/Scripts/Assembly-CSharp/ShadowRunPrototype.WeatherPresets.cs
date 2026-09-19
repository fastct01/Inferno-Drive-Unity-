using UnityEngine;
using DynamicWeatherSystem;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_InitializeDynamicWeatherSystem, _AddWeatherModule<T>
// (GenericMethods__38.cpp), _UpdateDynamicWeatherSystem, _BuildWeatherPresets, _BuildPlanetsPreset,
// _LoadWeatherPresetAsset, _CloneWeatherPreset, _BuildNightPreset, _BuildDayPreset, _BuildFallenSkyPreset,
// _ApplyDayVisualsToRain, _BuildRainFallbackPreset, _BuildStormFallbackPreset, _ApplyCalmStormProfile,
// _ApplyRoadFogProfiles, _SetRoadFog, _CreateRuntimeWeatherState, _ApplyRuntimeSkyPlacement, _CopyFastSkyProfile,
// _ApplyFastSkyNightProfile, _ApplyFastSkyDayProfile, _ApplyFastSkyRainProfile, _ApplyFastSkyStormProfile.
public sealed partial class ShadowRunPrototype
{
	private void InitializeDynamicWeatherSystem()
	{
		if (dynamicWeatherReady)
		{
			return;
		}
		SetupWeatherSkybox();
		DisableLegacySkyBackdrop();
		Transform root = new GameObject("WeatherSystem").transform;
		AddWeatherModule<SkyModule>(root, "SkyModule");
		AddWeatherModule<LightModule>(root, "LightModule").SetDirectionalLight(sceneKeyLight);
		AddWeatherModule<FogModule>(root, "FogModule");
		AddWeatherModule<RainModule>(root, "RainModule");
		weatherAudioModule = AddWeatherModule<AudioModule>(root, "AudioModule");
		weatherManager = root.gameObject.AddComponent<WeatherManager>();
		BuildWeatherPresets();
		weatherPresetIndex = 0;
		weatherAutoTimer = WeatherPhaseDuration;
		dynamicWeatherReady = weatherManager != null && weatherPresetCycle != null && weatherPresetCycle.Length > 0;
		if (dynamicWeatherReady)
		{
			weatherManager.SetWeather(weatherPresetCycle[weatherPresetIndex], 0f);
		}
		EnsureLightningFlash();
		lightningTimer = LightningInterval;
		UpdateWeatherAudioMute();
	}

	private T AddWeatherModule<T>(Transform root, string objectName) where T : Component
	{
		GameObject moduleObject = new GameObject(objectName);
		moduleObject.transform.SetParent(root, false);
		return moduleObject.AddComponent<T>();
	}

	private void UpdateDynamicWeatherSystem(float dt)
	{
		if (!dynamicWeatherReady || weatherManager == null)
		{
			return;
		}
		EnsureWeatherSkyboxActive();
		UpdateLightningFlash(dt);
		UpdatePulsarNight(dt);
		UpdateRainStormSun(dt);
		UpdateFallenSkyFallingStars();
		if (!running)
		{
			return;
		}
		weatherAutoTimer -= dt;
		if (weatherAutoTimer > 0f)
		{
			return;
		}
		AdvanceWeatherPreset();
	}

	private void BuildWeatherPresets()
	{
		weatherDayPreset = BuildDayPreset();
		weatherNightPreset = BuildNightPreset();
		weatherFallenSkyPreset = BuildFallenSkyPreset();
		weatherRainPreset = CloneWeatherPreset(LoadWeatherPresetAsset("Assets/DynamicWeatherSystem/Presets/WS_Rain.asset"), "Rain", BuildRainFallbackPreset());
		weatherStormPreset = CloneWeatherPreset(LoadWeatherPresetAsset("Assets/DynamicWeatherSystem/Presets/WS_Storm.asset"), "Storm", BuildStormFallbackPreset());
		weatherPlanetsPreset = BuildPlanetsPreset();
		ApplyDayVisualsToRain(weatherRainPreset, weatherDayPreset);
		ApplyCalmStormProfile(weatherStormPreset, weatherRainPreset);
		ApplyRoadFogProfiles();
		ApplyRainAudio();
		ApplyRuntimeSkyPlacement(weatherRainPreset, new Vector3(18f, -32f, 0f));
		ApplyRuntimeSkyPlacement(weatherStormPreset, new Vector3(8f, -32f, 0f));
		weatherPresetCycle = new WeatherStateData[6] { weatherDayPreset, weatherNightPreset, weatherRainPreset, weatherStormPreset, weatherFallenSkyPreset, weatherPlanetsPreset };
	}

	private WeatherStateData BuildPlanetsPreset()
	{
		WeatherStateData data = CreateRuntimeWeatherState("Planets");
		data.fogEnabled = true;
		data.fogColor = new Color(0.0015f, 0.0025f, 0.006f, 1f);
		data.fogDensity = 0.0015f;
		data.lightColor = new Color(0.1f, 0.18f, 0.42f);
		data.lightIntensity = 0.055f;
		data.lightEulerAngles = new Vector3(-58f, -32f, 0f);
		data.ambientColor = new Color(0.002f, 0.004f, 0.012f);
		data.skyboxTint = Color.black;
		data.skyboxExposure = 0f;
		data.rainIntensity = 0f;
		data.ambientVolume = 0f;
		data.fastSkyDayColor = Color.black;
		data.fastSkyEveningColor = Color.black;
		data.fastSkyStarColor = Color.black;
		data.fastSkyCloudColor = Color.black;
		data.fastSkySunIntensity = 0f;
		data.fastSkySunSize = 0.04f;
		data.fastSkySaturation = 0f;
		data.fastSkyDayBrightness = 0f;
		data.fastSkyEveningBrightness = 0f;
		data.fastSkyEveningThreshold = 1.5f;
		data.fastSkyWavelengths = new Color(680f, 550f, 440f, 0f);
		data.fastSkyDayScatterStrength = 0f;
		data.fastSkyEveningScatterStrength = 0f;
		data.fastSkyDensityPower = 10f;
		data.fastSkyStarBrightness = 0f;
		data.fastSkyStarSize = weatherNightPreset != null ? weatherNightPreset.fastSkyStarSize : 0.98f;
		data.fastSkyStarScale = weatherNightPreset != null ? weatherNightPreset.fastSkyStarScale : 2f;
		data.fastSkyStarSpeed = 0f;
		data.fastSkyStarJitterSpeed = 0f;
		data.fastSkyStarRotateSpeed = 0f;
		data.fastSkyStarThreshold = 20f;
		data.fastSkyCloudBrightness = 0f;
		data.fastSkyCloudAnisotropy = 0f;
		data.fastSkyCloudDensity = 0f;
		data.fastSkyCloudHeight = 0f;
		data.fastSkyCloudThickness = 0f;
		data.fastSkyCloudSoftness = 1f;
		data.fastSkyCloudSmoothness = 1f;
		data.fastSkyCloudDensitySmoothness = 1f;
		data.fastSkyCloudSpeed = 0f;
		data.fastSkyCloudScale = 1f;
		data.fastSkyCloudTwirl = 0f;
		return data;
	}

	private WeatherStateData LoadWeatherPresetAsset(string path)
	{
		return null;
	}

	private WeatherStateData CloneWeatherPreset(WeatherStateData source, string stateName, WeatherStateData fallback)
	{
		WeatherStateData data = ScriptableObject.CreateInstance<WeatherStateData>();
		data.name = "Runtime_WS_" + stateName;
		data.stateName = stateName;
		WeatherStateData template = source != null ? source : fallback;
		data.fogEnabled = template.fogEnabled;
		data.fogColor = template.fogColor;
		data.fogDensity = template.fogDensity;
		data.fogStartDistance = template.fogStartDistance;
		data.fogEndDistance = template.fogEndDistance;
		data.lightColor = template.lightColor;
		data.lightIntensity = template.lightIntensity;
		data.lightEulerAngles = template.lightEulerAngles;
		data.ambientColor = template.ambientColor;
		data.skyboxTint = template.skyboxTint;
		data.skyboxExposure = template.skyboxExposure;
		data.ambientClip = template.ambientClip;
		data.ambientVolume = template.ambientVolume;
		data.rainIntensity = template.rainIntensity;
		data.rainColor = template.rainColor;
		data.rainSpeed = template.rainSpeed;
		CopyFastSkyProfile(template, data);
		return data;
	}

	private WeatherStateData BuildNightPreset()
	{
		WeatherStateData data = CreateRuntimeWeatherState("Night");
		data.fogEnabled = true;
		data.fogColor = new Color(0.026f, 0.05f, 0.12f);
		data.fogDensity = 0.0012f;
		data.lightColor = new Color(0.3f, 0.44f, 0.82f);
		data.lightIntensity = 0.14f;
		data.lightEulerAngles = new Vector3(-64f, -32f, 0f);
		data.ambientColor = new Color(0.02f, 0.036f, 0.08f);
		data.skyboxTint = new Color(0.22f, 0.38f, 0.82f);
		data.skyboxExposure = 0.3f;
		data.rainIntensity = 0f;
		data.rainColor = new Color(0.7f, 0.8f, 0.9f, 0.35f);
		data.rainSpeed = 8f;
		data.ambientVolume = 0.2f;
		ApplyFastSkyNightProfile(data);
		return data;
	}

	private WeatherStateData BuildDayPreset()
	{
		WeatherStateData data = CreateRuntimeWeatherState("Day");
		data.fogEnabled = true;
		data.fogColor = new Color(1f, 0.09f, 0.055f);
		data.fogDensity = 0.00155f;
		data.lightColor = new Color(1f, 0.42f, 0.3f);
		data.lightIntensity = 0.95f;
		data.lightEulerAngles = new Vector3(18f, -32f, 0f);
		data.ambientColor = new Color(0.18f, 0.08f, 0.09f);
		data.skyboxTint = new Color(0.9f, 0.2f, 0.16f);
		data.skyboxExposure = 0.72f;
		data.rainIntensity = 0f;
		data.rainColor = new Color(0.7f, 0.8f, 0.9f, 0.35f);
		data.rainSpeed = 8f;
		data.ambientVolume = 0f;
		ApplyFastSkyDayProfile(data);
		return data;
	}

	private WeatherStateData BuildFallenSkyPreset()
	{
		WeatherStateData data = CreateRuntimeWeatherState("Fallen Sky");
		data.fogEnabled = true;
		data.fogColor = new Color(0.58f, 0.46f, 0.66f);
		data.fogDensity = 0.0018f;
		data.lightColor = new Color(0.94f, 0.8f, 1f);
		data.lightIntensity = 0.66f;
		data.lightEulerAngles = new Vector3(16f, -32f, 0f);
		data.ambientColor = new Color(0.2f, 0.14f, 0.25f);
		data.skyboxTint = new Color(0.74f, 0.6f, 0.84f);
		data.skyboxExposure = 0.62f;
		data.rainIntensity = 0f;
		data.rainColor = new Color(0.78f, 0.68f, 0.94f, 0.3f);
		data.rainSpeed = 8f;
		data.ambientVolume = 0f;
		ApplyFastSkyDayProfile(data);
		data.fastSkyDayColor = new Color(0.76f, 0.58f, 0.84f);
		data.fastSkyEveningColor = new Color(0.34f, 0.22f, 0.46f);
		data.fastSkyStarBrightness = 0f;
		data.fastSkyCloudDensity = 0f;
		data.fastSkyCloudBrightness = 0f;
		data.fastSkySunIntensity = 5f;
		data.fastSkySunSize = 0.05f;
		return data;
	}

	private void ApplyDayVisualsToRain(WeatherStateData rain, WeatherStateData day)
	{
		if (rain == null || day == null)
		{
			return;
		}
		rain.fogEnabled = day.fogEnabled;
		rain.fogColor = day.fogColor;
		rain.fogDensity = day.fogDensity;
		rain.lightColor = day.lightColor;
		rain.lightIntensity = day.lightIntensity;
		rain.lightEulerAngles = day.lightEulerAngles;
		rain.ambientColor = day.ambientColor;
		rain.skyboxTint = day.skyboxTint;
		rain.skyboxExposure = day.skyboxExposure;
		ApplyFastSkyDayProfile(rain);
	}

	private WeatherStateData BuildRainFallbackPreset()
	{
		WeatherStateData data = CreateRuntimeWeatherState("Rain");
		data.fogEnabled = true;
		data.fogColor = new Color(0.55f, 0.58f, 0.63f);
		data.fogDensity = 0.015f;
		data.lightColor = new Color(0.72f, 0.78f, 0.88f);
		data.lightIntensity = 0.6f;
		data.lightEulerAngles = new Vector3(24f, -32f, 0f);
		data.ambientColor = new Color(0.14f, 0.17f, 0.22f);
		data.skyboxTint = new Color(0.82f, 0.66f, 0.82f);
		data.skyboxExposure = 0.62f;
		data.rainIntensity = 0.6f;
		data.rainColor = new Color(0.7f, 0.8f, 0.9f, 0.35f);
		data.rainSpeed = 9f;
		data.ambientVolume = 0.5f;
		ApplyFastSkyRainProfile(data);
		return data;
	}

	private WeatherStateData BuildStormFallbackPreset()
	{
		WeatherStateData data = CreateRuntimeWeatherState("Storm");
		data.fogEnabled = true;
		data.fogColor = new Color(0.42f, 0.3f, 0.5f);
		data.fogDensity = 0.0065f;
		data.lightColor = new Color(0.88f, 0.72f, 0.94f);
		data.lightIntensity = 0.52f;
		data.lightEulerAngles = new Vector3(18f, -32f, 0f);
		data.ambientColor = new Color(0.18f, 0.11f, 0.24f);
		data.skyboxTint = new Color(0.82f, 0.56f, 0.92f);
		data.skyboxExposure = 0.62f;
		data.rainIntensity = 0.62f;
		data.rainColor = new Color(0.84f, 0.72f, 0.96f, 0.34f);
		data.rainSpeed = 9f;
		data.ambientVolume = 0.42f;
		ApplyFastSkyStormProfile(data);
		return data;
	}

	private void ApplyCalmStormProfile(WeatherStateData data, WeatherStateData rainTemplate)
	{
		if (data == null)
		{
			return;
		}
		data.fogEnabled = true;
		data.fogColor = new Color(0.42f, 0.3f, 0.5f);
		data.fogDensity = 0.0065f;
		data.lightColor = new Color(0.88f, 0.72f, 0.94f);
		data.lightIntensity = 0.52f;
		data.lightEulerAngles = new Vector3(18f, -32f, 0f);
		data.ambientColor = new Color(0.18f, 0.11f, 0.24f);
		data.skyboxTint = new Color(0.82f, 0.56f, 0.92f);
		data.skyboxExposure = 0.62f;
		data.rainIntensity = 0.62f;
		data.rainColor = new Color(0.84f, 0.72f, 0.96f, 0.34f);
		data.rainSpeed = 9f;
		data.ambientClip = rainTemplate != null ? rainTemplate.ambientClip : data.ambientClip;
		data.ambientVolume = 0.42f;
		ApplyFastSkyStormProfile(data);
	}

	private void ApplyRoadFogProfiles()
	{
		SetRoadFog(weatherDayPreset, new Color(0.46f, 0.035f, 0.045f, 1f), 95f, 360f);
		SetRoadFog(weatherNightPreset, new Color(0.008f, 0.015f, 0.038f, 1f), 90f, 330f);
		SetRoadFog(weatherRainPreset, new Color(0.17f, 0.2f, 0.25f, 1f), 70f, 270f);
		SetRoadFog(weatherStormPreset, new Color(0.105f, 0.12f, 0.14f, 1f), 55f, 230f);
		SetRoadFog(weatherFallenSkyPreset, new Color(0.24f, 0.15f, 0.29f, 1f), 85f, 320f);
		SetRoadFog(weatherPlanetsPreset, new Color(0.0015f, 0.003f, 0.01f, 1f), 75f, 290f);
	}

	private void SetRoadFog(WeatherStateData data, Color color, float startDistance, float endDistance)
	{
		if (data == null)
		{
			return;
		}
		data.fogEnabled = true;
		data.fogColor = color;
		data.fogStartDistance = startDistance;
		data.fogEndDistance = Mathf.Max(startDistance + 1f, endDistance);
	}

	private WeatherStateData CreateRuntimeWeatherState(string stateName)
	{
		WeatherStateData data = ScriptableObject.CreateInstance<WeatherStateData>();
		data.name = "Runtime_WS_" + stateName;
		data.stateName = stateName;
		return data;
	}

	private void ApplyRuntimeSkyPlacement(WeatherStateData data, Vector3 lightEulerAngles)
	{
		if (data == null)
		{
			return;
		}
		data.lightEulerAngles = lightEulerAngles;
	}

	private void CopyFastSkyProfile(WeatherStateData source, WeatherStateData target)
	{
		target.fastSkyDayColor = source.fastSkyDayColor;
		target.fastSkyEveningColor = source.fastSkyEveningColor;
		target.fastSkyStarColor = source.fastSkyStarColor;
		target.fastSkyCloudColor = source.fastSkyCloudColor;
		target.fastSkySunIntensity = source.fastSkySunIntensity;
		target.fastSkySunSize = source.fastSkySunSize;
		target.fastSkySaturation = source.fastSkySaturation;
		target.fastSkyDayBrightness = source.fastSkyDayBrightness;
		target.fastSkyEveningBrightness = source.fastSkyEveningBrightness;
		target.fastSkyEveningThreshold = source.fastSkyEveningThreshold;
		target.fastSkyWavelengths = source.fastSkyWavelengths;
		target.fastSkyDayScatterStrength = source.fastSkyDayScatterStrength;
		target.fastSkyEveningScatterStrength = source.fastSkyEveningScatterStrength;
		target.fastSkyDensityPower = source.fastSkyDensityPower;
		target.fastSkyStarBrightness = source.fastSkyStarBrightness;
		target.fastSkyStarSize = source.fastSkyStarSize;
		target.fastSkyStarScale = source.fastSkyStarScale;
		target.fastSkyStarSpeed = source.fastSkyStarSpeed;
		target.fastSkyStarJitterSpeed = source.fastSkyStarJitterSpeed;
		target.fastSkyStarRotateSpeed = source.fastSkyStarRotateSpeed;
		target.fastSkyStarThreshold = source.fastSkyStarThreshold;
		target.fastSkyCloudBrightness = source.fastSkyCloudBrightness;
		target.fastSkyCloudAnisotropy = source.fastSkyCloudAnisotropy;
		target.fastSkyCloudDensity = source.fastSkyCloudDensity;
		target.fastSkyCloudHeight = source.fastSkyCloudHeight;
		target.fastSkyCloudThickness = source.fastSkyCloudThickness;
		target.fastSkyCloudSoftness = source.fastSkyCloudSoftness;
		target.fastSkyCloudSmoothness = source.fastSkyCloudSmoothness;
		target.fastSkyCloudDensitySmoothness = source.fastSkyCloudDensitySmoothness;
		target.fastSkyCloudSpeed = source.fastSkyCloudSpeed;
		target.fastSkyCloudScale = source.fastSkyCloudScale;
		target.fastSkyCloudTwirl = source.fastSkyCloudTwirl;
	}

	private void ApplyFastSkyNightProfile(WeatherStateData data)
	{
		data.fastSkyDayColor = new Color(0.025f, 0.06f, 0.18f);
		data.fastSkyEveningColor = new Color(0.006f, 0.016f, 0.06f);
		data.fastSkyStarColor = new Color(1.35f, 1.55f, 2.15f);
		data.fastSkyCloudColor = new Color(0.18f, 0.22f, 0.34f);
		data.fastSkySunIntensity = 0.08f;
		data.fastSkySunSize = 0.1f;
		data.fastSkySaturation = 1.85f;
		data.fastSkyDayBrightness = 0.24f;
		data.fastSkyEveningBrightness = 0.105f;
		data.fastSkyEveningThreshold = 1.62f;
		data.fastSkyWavelengths = new Color(520f, 575f, 760f, 0f);
		data.fastSkyDayScatterStrength = 1.28f;
		data.fastSkyEveningScatterStrength = 22f;
		data.fastSkyDensityPower = 6.2f;
		data.fastSkyStarBrightness = 500f;
		data.fastSkyStarSize = 0.98f;
		data.fastSkyStarScale = 2f;
		data.fastSkyStarSpeed = 0.02f;
		data.fastSkyStarJitterSpeed = 0.1f;
		data.fastSkyStarRotateSpeed = 5f;
		data.fastSkyStarThreshold = 20f;
		data.fastSkyCloudBrightness = 0f;
		data.fastSkyCloudAnisotropy = 0.55f;
		data.fastSkyCloudDensity = 0f;
		data.fastSkyCloudHeight = 0.22f;
		data.fastSkyCloudThickness = 0.34f;
		data.fastSkyCloudSoftness = 0.82f;
		data.fastSkyCloudSmoothness = 0.62f;
		data.fastSkyCloudDensitySmoothness = 0.76f;
		data.fastSkyCloudSpeed = 0.06f;
		data.fastSkyCloudScale = 0.9f;
		data.fastSkyCloudTwirl = 0.06f;
	}

	private void ApplyFastSkyDayProfile(WeatherStateData data)
	{
		data.fastSkyDayColor = new Color(0.5f, 0.025f, 0.045f);
		data.fastSkyEveningColor = new Color(1.1f, 0.1f, 0.045f);
		data.fastSkyStarColor = new Color(1f, 0.28f, 0.22f);
		data.fastSkyCloudColor = new Color(0.3f, 0.018f, 0.025f);
		data.fastSkySunIntensity = 12f;
		data.fastSkySunSize = 0.055f;
		data.fastSkySaturation = 1.65f;
		data.fastSkyDayBrightness = 0.38f;
		data.fastSkyEveningBrightness = 0.16f;
		data.fastSkyEveningThreshold = 1.34f;
		data.fastSkyWavelengths = new Color(760f, 470f, 390f, 0f);
		data.fastSkyDayScatterStrength = 1.12f;
		data.fastSkyEveningScatterStrength = 19f;
		data.fastSkyDensityPower = 7.2f;
		data.fastSkyStarBrightness = 0f;
		data.fastSkyStarSize = 0.99f;
		data.fastSkyStarScale = 2f;
		data.fastSkyStarSpeed = 0.1f;
		data.fastSkyStarJitterSpeed = 0.1f;
		data.fastSkyStarRotateSpeed = 5f;
		data.fastSkyStarThreshold = 20f;
		data.fastSkyCloudColor = Color.white;
		data.fastSkyCloudBrightness = 10f;
		data.fastSkyCloudAnisotropy = 0f;
		data.fastSkyCloudDensity = 2f;
		data.fastSkyCloudHeight = 0.25f;
		data.fastSkyCloudThickness = 0.6f;
		data.fastSkyCloudSoftness = 0.05f;
		data.fastSkyCloudSmoothness = 0f;
		data.fastSkyCloudDensitySmoothness = 1f;
		data.fastSkyCloudSpeed = 5f;
		data.fastSkyCloudScale = 0.5f;
		data.fastSkyCloudTwirl = 1f;
	}

	private void ApplyFastSkyRainProfile(WeatherStateData data)
	{
		data.fastSkyDayColor = new Color(0.62f, 0.5f, 0.68f);
		data.fastSkyEveningColor = new Color(0.24f, 0.16f, 0.28f);
		data.fastSkyStarColor = new Color(0.72f, 0.62f, 0.78f);
		data.fastSkyCloudColor = new Color(0.64f, 0.54f, 0.66f);
		data.fastSkySunIntensity = 5f;
		data.fastSkySunSize = 0.045f;
		data.fastSkySaturation = 0.82f;
		data.fastSkyDayBrightness = 0.34f;
		data.fastSkyEveningBrightness = 0.105f;
		data.fastSkyEveningThreshold = 1.28f;
		data.fastSkyWavelengths = new Color(700f, 530f, 590f, 0f);
		data.fastSkyDayScatterStrength = 0.92f;
		data.fastSkyEveningScatterStrength = 11.5f;
		data.fastSkyDensityPower = 8.6f;
		data.fastSkyStarBrightness = 0f;
		data.fastSkyStarSize = 0.99f;
		data.fastSkyStarScale = 2f;
		data.fastSkyStarSpeed = 0.1f;
		data.fastSkyStarJitterSpeed = 0.1f;
		data.fastSkyStarRotateSpeed = 5f;
		data.fastSkyStarThreshold = 24f;
		data.fastSkyCloudBrightness = 22f;
		data.fastSkyCloudAnisotropy = 0.66f;
		data.fastSkyCloudDensity = 3.8f;
		data.fastSkyCloudHeight = 0.17f;
		data.fastSkyCloudThickness = 0.8f;
		data.fastSkyCloudSoftness = 0.82f;
		data.fastSkyCloudSmoothness = 0.62f;
		data.fastSkyCloudDensitySmoothness = 0.79f;
		data.fastSkyCloudSpeed = 0.32f;
		data.fastSkyCloudScale = 0.7f;
		data.fastSkyCloudTwirl = 0.11f;
	}

	private void ApplyFastSkyStormProfile(WeatherStateData data)
	{
		data.fastSkyDayColor = new Color(0.84f, 0.46f, 0.7f);
		data.fastSkyEveningColor = new Color(0.4f, 0.22f, 0.52f);
		data.fastSkyStarColor = new Color(0.92f, 0.78f, 1.1f);
		data.fastSkyCloudColor = new Color(0.72f, 0.58f, 0.8f);
		data.fastSkySunIntensity = 0f;
		data.fastSkySunSize = 0.045f;
		data.fastSkySaturation = 0.96f;
		data.fastSkyDayBrightness = 0.42f;
		data.fastSkyEveningBrightness = 0.22f;
		data.fastSkyEveningThreshold = 1.32f;
		data.fastSkyWavelengths = new Color(680f, 565f, 720f, 0f);
		data.fastSkyDayScatterStrength = 0.96f;
		data.fastSkyEveningScatterStrength = 13f;
		data.fastSkyDensityPower = 8.2f;
		data.fastSkyStarBrightness = 0f;
		data.fastSkyStarSize = 0.9f;
		data.fastSkyStarScale = 2f;
		data.fastSkyStarSpeed = 0f;
		data.fastSkyStarJitterSpeed = 0f;
		data.fastSkyStarRotateSpeed = 0f;
		data.fastSkyStarThreshold = 14f;
		data.fastSkyCloudBrightness = 16f;
		data.fastSkyCloudAnisotropy = 0.54f;
		data.fastSkyCloudDensity = 2.35f;
		data.fastSkyCloudHeight = 0.2f;
		data.fastSkyCloudThickness = 0.58f;
		data.fastSkyCloudSoftness = 0.88f;
		data.fastSkyCloudSmoothness = 0.64f;
		data.fastSkyCloudDensitySmoothness = 0.72f;
		data.fastSkyCloudSpeed = 0.22f;
		data.fastSkyCloudScale = 0.82f;
		data.fastSkyCloudTwirl = 0.08f;
	}
}
