using UnityEngine;

namespace CartoonFX
{
	public class CFXR_ParticleText : MonoBehaviour
	{
		public bool isDynamic;

		private string text;

		private float size;

		private float letterSpacing;

		private Color backgroundColor;

		private Color color1;

		private Color color2;

		private float delay;

		private bool cumulativeDelay;

		private float compensateLifetime;

		private float lifetimeMultiplier;

		private float rotation;

		private float sortingFudgeOffset;

		private CFXR_ParticleTextFontAsset font;

		private float baseLifetime;

		private float baseScaleX;

		private float baseScaleY;

		private float baseScaleZ;

		private Vector3 basePivot;

		private void Awake()
		{
		}

		private void InitializeFirstParticle()
		{
		}

		public void UpdateText(string newText = null, float? newSize = null, Color? newColor1 = null, Color? newColor2 = null, Color? newBackgroundColor = null, float? newLifetimeMultiplier = null)
		{
		}
	}
}
