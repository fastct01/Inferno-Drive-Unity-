using UnityEngine;

namespace DynamicWeatherSystem
{
	public class RainModule : WeatherModule
	{
		private const string ImpactPrefabPath = "Rain Particles/Prefabs/Rain Particles";

		private const string RainMaterialPath = "Rain Particles/Materials/Rain Sprite";

		private const float MaxRainRate = 300f;

		private const float ImpactBurstMultiplier = 1.2f;

		private const float RainDriftSpeed = 1.4f;

		private const float RainFallSpeed = 17f;

		private const float RainSpawnHeight = 14f;

		private static readonly Vector3 RainWorldOffsetFromCamera;

		private static readonly Vector3 RainArea;

		private Transform followTarget;

		private GameObject rainRoot;

		private ParticleSystem rainSystem;

		private ParticleSystem splashSystem;

		private ParticleSystem ringSystem;

		private bool isReady;

		private bool isVisible;

		private void Awake()
		{
		}

		private void LateUpdate()
		{
		}

		public override void Blend(WeatherStateData from, WeatherStateData to, float t)
		{
		}

		private void BuildRain()
		{
		}

		private ParticleSystem CreateRainSystem(Transform parent, Material material)
		{
			return null;
		}

		private void CreateAssetImpactSystems(Transform parent, GameObject impactPrefab)
		{
		}

		private static void PrepareAssetImpactSystem(ParticleSystem particle)
		{
		}

		private static void ScaleBursts(ParticleSystem particle, float multiplier)
		{
		}

		private static ParticleSystem CreateParticleSystem(Transform parent, string objectName)
		{
			return null;
		}

		private void ApplyRain(float intensity)
		{
		}

		private void TryFindFollowTarget()
		{
		}

		private static float GetNightPrecipitationTransitionT(WeatherStateData from, WeatherStateData to, float t, float delay)
		{
			return 0f;
		}
	}
}
