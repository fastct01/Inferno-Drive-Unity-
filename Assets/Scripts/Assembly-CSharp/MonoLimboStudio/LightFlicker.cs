using UnityEngine;

namespace MonoLimboStudio
{
	public class LightFlicker : MonoBehaviour
	{
		public Light targetLight;

		public float minIntensity;

		public float maxIntensity;

		public float flickerSpeed;

		public Renderer targetRenderer;

		public string opacityProperty;

		public float minOpacity;

		public float maxOpacity;

		private Material materialInstance;

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
