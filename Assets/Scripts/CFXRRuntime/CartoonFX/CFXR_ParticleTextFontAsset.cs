using System;
using UnityEngine;

namespace CartoonFX
{
	public class CFXR_ParticleTextFontAsset : ScriptableObject
	{
		public enum LetterCase
		{
			Both = 0,
			Upper = 1,
			Lower = 2
		}

		[Serializable]
		public class Kerning
		{
			public string name;

			public float pre;

			public float post;
		}

		public string CharSequence;

		public LetterCase letterCase;

		public Sprite[] CharSprites;

		public Kerning[] CharKerningOffsets;

		private void OnValidate()
		{
		}

		public bool IsValid()
		{
			return false;
		}
	}
}
