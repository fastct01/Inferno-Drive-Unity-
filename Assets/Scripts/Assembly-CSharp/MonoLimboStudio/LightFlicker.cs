using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__2.cpp:
//   LightFlicker__ctor_m9163E2AF144EA7F98391488A82B08E88566DC345
//   LightFlicker_Start_m12306DF9F96799412507FB6300EF8B0EC1C5FC7C
//   LightFlicker_Update_m37A1B6DB9291817603D323FC7DD39F7906077CB9
namespace MonoLimboStudio
{
	public class LightFlicker : MonoBehaviour
	{
		public Light targetLight;

		public float minIntensity = 0.5f;

		public float maxIntensity = 1.5f;

		public float flickerSpeed = 5f;

		public Renderer targetRenderer;

		public string opacityProperty = "_Opacity";

		public float minOpacity = 0.2f;

		public float maxOpacity = 1f;

		private Material materialInstance;

		private void Start()
		{
			if (targetRenderer != null)
			{
				materialInstance = targetRenderer.material;
				if (!materialInstance.HasProperty(opacityProperty))
				{
					Debug.LogWarning("Material does not have a float property named '" + opacityProperty + "'.");
				}
			}
		}

		private void Update()
		{
			float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
			if (targetLight != null)
			{
				float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
				targetLight.intensity = intensity;
			}
			if (materialInstance != null && materialInstance.HasProperty(opacityProperty))
			{
				float opacity = Mathf.Lerp(minOpacity, maxOpacity, noise);
				materialInstance.SetFloat(opacityProperty, opacity);
			}
		}
	}
}
