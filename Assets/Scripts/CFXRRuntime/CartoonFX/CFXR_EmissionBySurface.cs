using UnityEngine;

namespace CartoonFX
{
	public class CFXR_EmissionBySurface : MonoBehaviour
	{
		public bool active;

		public float particlesPerUnit;

		public float maxEmissionRate;

		public float density;

		private bool attachedToEditor;

		private ParticleSystem ps;
	}
}
