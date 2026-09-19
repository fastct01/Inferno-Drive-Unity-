using UnityEngine;

namespace DynamicWeatherSystem
{
	public class WeatherStateData : ScriptableObject
	{
		public string stateName;

		public bool fogEnabled;

		public Color fogColor;

		public float fogDensity;

		public float fogStartDistance;

		public float fogEndDistance;

		public Color lightColor;

		public float lightIntensity;

		public Vector3 lightEulerAngles;

		public Color ambientColor;

		public Color skyboxTint;

		public float skyboxExposure;

		public Color fastSkyDayColor;

		public Color fastSkyEveningColor;

		public Color fastSkyStarColor;

		public Color fastSkyCloudColor;

		public float fastSkySunIntensity;

		public float fastSkySunSize;

		public float fastSkySaturation;

		public float fastSkyDayBrightness;

		public float fastSkyEveningBrightness;

		public float fastSkyEveningThreshold;

		public Color fastSkyWavelengths;

		public float fastSkyDayScatterStrength;

		public float fastSkyEveningScatterStrength;

		public float fastSkyDensityPower;

		public float fastSkyStarBrightness;

		public float fastSkyStarSize;

		public float fastSkyStarScale;

		public float fastSkyStarSpeed;

		public float fastSkyStarJitterSpeed;

		public float fastSkyStarRotateSpeed;

		public float fastSkyStarThreshold;

		public float fastSkyCloudBrightness;

		public float fastSkyCloudAnisotropy;

		public float fastSkyCloudDensity;

		public float fastSkyCloudHeight;

		public float fastSkyCloudThickness;

		public float fastSkyCloudSoftness;

		public float fastSkyCloudSmoothness;

		public float fastSkyCloudDensitySmoothness;

		public float fastSkyCloudSpeed;

		public float fastSkyCloudScale;

		public float fastSkyCloudTwirl;

		public AudioClip ambientClip;

		public float ambientVolume;

		public float rainIntensity;

		public Color rainColor;

		public float rainSpeed;
	}
}
