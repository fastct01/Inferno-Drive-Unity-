using UnityEngine;

namespace DynamicWeatherSystem
{
	public class SkyModule : WeatherModule
	{
		private static readonly int TintId;

		private static readonly int SkyTintId;

		private static readonly int ExposureId;

		private Material _originalSkybox;

		private Material _skyboxInstance;

		private Shader _shadowRunFastSkyShader;

		private Shader _nativeFastSkyShader;

		private int _activeTintPropertyId;

		private int _fastSkyDayColorId;

		private int _fastSkyEveningColorId;

		private int _fastSkyStarColorId;

		private int _fastSkyCloudColorId;

		private int _fastSkySunIntensityId;

		private int _fastSkySunSizeId;

		private int _fastSkySaturationId;

		private int _fastSkyDayBrightnessId;

		private int _fastSkyEveningBrightnessId;

		private int _fastSkyEveningThresholdId;

		private int _fastSkyWavelengthsId;

		private int _fastSkyDayScatterStrengthId;

		private int _fastSkyEveningScatterStrengthId;

		private int _fastSkyDensityPowerId;

		private int _fastSkyStarBrightnessId;

		private int _fastSkyStarSizeId;

		private int _fastSkyStarScaleId;

		private int _fastSkyStarSpeedId;

		private int _fastSkyStarJitterSpeedId;

		private int _fastSkyStarRotateSpeedId;

		private int _fastSkyStarThresholdId;

		private int _fastSkyCloudBrightnessId;

		private int _fastSkyCloudAnisotropyId;

		private int _fastSkyCloudDensityId;

		private int _fastSkyCloudHeightId;

		private int _fastSkyCloudThicknessId;

		private int _fastSkyCloudSoftnessId;

		private int _fastSkyCloudSmoothnessId;

		private int _fastSkyCloudDensitySmoothnessId;

		private int _fastSkyCloudSpeedId;

		private int _fastSkyCloudScaleId;

		private int _fastSkyCloudTwirlId;

		private int _fastSkySunDirectionId;

		private bool _hasTint;

		private bool _hasExposure;

		private bool _hasFastSky;

		private bool _isReady;

		private const float DayToNightShaderSwitch = 0.76f;

		private const float StormToNightShaderSwitch = 0.86f;

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		public override void Blend(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private void SelectWeatherShader(WeatherStateData from, WeatherStateData target, float t)
		{
		}

		private void ApplyUnitySky(Color tint, float exposure)
		{
		}

		private void ApplyFastSky(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private static float GetNightPrecipitationTransitionT(WeatherStateData from, WeatherStateData to, float t, float delay)
		{
			return 0f;
		}

		private static bool IsNightPrecipitationTransition(WeatherStateData from, WeatherStateData to)
		{
			return false;
		}

		private static bool IsStormToNightTransition(WeatherStateData from, WeatherStateData to)
		{
			return false;
		}

		private void SetColor(int propertyId, Color value)
		{
		}

		private void SetFloat(int propertyId, float value)
		{
		}

		private void SetVector(int propertyId, Vector4 value)
		{
		}

		private void DetectProperties()
		{
		}

		private int FindFastSkyPropertyId(string wanted, bool warn = true)
		{
			return 0;
		}

		private static string NormalizePropertyName(string value)
		{
			return null;
		}
	}
}
