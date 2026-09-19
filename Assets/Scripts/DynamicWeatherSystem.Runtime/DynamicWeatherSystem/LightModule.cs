using UnityEngine;

namespace DynamicWeatherSystem
{
	public class LightModule : WeatherModule
	{
		private Light directionalLight;

		private void Awake()
		{
		}

		public void SetDirectionalLight(Light light)
		{
		}

		private void Reset()
		{
		}

		public override void Blend(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private void BlendDirectionalLight(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private void BlendAmbientLight(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private static Light FindDirectionalLight()
		{
			return null;
		}

		private static float GetNightPrecipitationTransitionT(WeatherStateData from, WeatherStateData to, float t, float delay)
		{
			return 0f;
		}
	}
}
