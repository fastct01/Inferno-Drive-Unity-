using UnityEngine;

namespace DynamicWeatherSystem
{
	public abstract class WeatherModule : MonoBehaviour
	{
		public void Apply(WeatherStateData state)
		{
		}

		public abstract void Blend(WeatherStateData from, WeatherStateData to, float t);
	}
}
