using UnityEngine;

namespace DynamicWeatherSystem
{
	public class AudioModule : WeatherModule
	{
		private float masterVolume;

		private AudioSource _sourceA;

		private AudioSource _sourceB;

		private AudioClip _trackedFrom;

		private AudioClip _trackedTo;

		public void SetMuted(bool muted)
		{
		}

		private void Awake()
		{
		}

		public override void Blend(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private void InitializeCrossfade(AudioClip fromClip, float fromVolume, AudioClip toClip)
		{
		}

		private static void SetupSource(AudioSource source, AudioClip clip, float volume)
		{
		}

		private AudioSource CreateAudioSource(string sourceName)
		{
			return null;
		}
	}
}
