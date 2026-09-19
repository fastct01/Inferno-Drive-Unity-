using UnityEngine;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_MakeFrostSprite, _MakeCloudSprite, _MakeAdvancedCloudSprite,
// _MakeCircleSprite, _MakeRadialGlowSprite, _MakeRoundedRectSprite, _MakeCaseActionButtonSprite, _MakeGemCounterSprite,
// _MakeGradientBarSprite, _MakeCaseRarityStripSprite, _MakeCaseRouletteRailSprite, _MakeHorizontalFadeSprite,
// _MakeCaseScanlineSprite, _MakeVerticalLineSprite, _MakeTriangleSprite, _MakeVerticalFadeSprite,
// _MakeReferenceTabFrameSprite, _MakeReferenceActiveTabSprite, _RoundedBoxDistance.
public sealed partial class ShadowRunPrototype
{
	private Sprite MakeFrostSprite(int width, int height, int seed)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		System.Random random = new System.Random(seed);
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float roll = (float)random.NextDouble();
				float alpha = roll > 0.72f ? 0.32f : (roll > 0.55f ? 0.13f : 0.035f);
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeCloudSprite(int width, int height, int seed)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		System.Random random = new System.Random(seed);
		int layers = 3;
		int samples = 18;
		float[,] bottoms = new float[layers, samples];
		float[,] tops = new float[layers, samples];
		float[] starts = new float[layers];
		float[] ends = new float[layers];
		for (int layer = 0; layer < layers; layer++)
		{
			float centerY = 48f + layer * 48f + random.Next(-16, 17);
			float thickness = 22f + random.Next(0, 24);
			float tilt = -0.05f + (float)random.NextDouble() * 0.1f;
			starts[layer] = 42f + random.Next(0, 80);
			ends[layer] = width - 42f - random.Next(0, 80);
			for (int s = 0; s < samples; s++)
			{
				float t = (float)s / (samples - 1);
				float waveA = Mathf.Sin(t * Mathf.PI * 3.4f + seed * 0.21f + layer) * 9f;
				float waveB = Mathf.Sin(t * Mathf.PI * 17f + seed + layer * 3f) * 6f;
				float waveC = Mathf.Sin(t * Mathf.PI * 31f + seed * 0.47f + layer) * 3f;
				float x = Mathf.Lerp(starts[layer], ends[layer], t);
				float center = centerY + tilt * (x - width * 0.5f) + waveA;
				float bulge = Mathf.Sin(t * Mathf.PI);
				float halfHeight = thickness * (0.15f + bulge * 0.95f);
				bottoms[layer, s] = center - halfHeight + waveB + random.Next(-5, 6);
				tops[layer, s] = center + halfHeight + waveC + random.Next(-5, 6);
			}
		}
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float alpha = 0f;
				for (int layer = 0; layer < layers; layer++)
				{
					float t = Mathf.InverseLerp(starts[layer], ends[layer], x);
					if (t < 0f || t > 1f)
					{
						continue;
					}
					float sample = t * (samples - 1);
					int index = Mathf.Clamp(Mathf.FloorToInt(sample), 0, samples - 2);
					float blend = Mathf.SmoothStep(0f, 1f, sample - index);
					float bottom = Mathf.Lerp(bottoms[layer, index], bottoms[layer, index + 1], blend);
					float top = Mathf.Lerp(tops[layer, index], tops[layer, index + 1], blend);
					float softness = 7f;
					float inside = Mathf.Min(Mathf.SmoothStep(0f, 1f, (y - bottom) / softness), Mathf.SmoothStep(0f, 1f, (top - y) / softness));
					float holeA = Mathf.Sin(t * Mathf.PI * 10f + seed * 0.3f + layer) * 0.5f + 0.5f;
					float holeB = Mathf.Sin(t * Mathf.PI * 23f + seed + layer * 1.9f) * 0.5f + 0.5f;
					if (holeA > 0.86f && y < Mathf.Lerp(bottom, top, 0.42f))
					{
						inside *= 0.28f;
					}
					if (holeB > 0.9f && y > Mathf.Lerp(bottom, top, 0.58f))
					{
						inside *= 0.38f;
					}
					alpha = Mathf.Max(alpha, Mathf.Clamp01(inside));
				}
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeAdvancedCloudSprite(int width, int height, int seed)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Trilinear;
		System.Random random = new System.Random(seed);
		int layers = 5;
		int samples = 28;
		float[,] bottoms = new float[layers, samples];
		float[,] tops = new float[layers, samples];
		float[] starts = new float[layers];
		float[] ends = new float[layers];
		for (int layer = 0; layer < layers; layer++)
		{
			float centerY = 32f + layer * 40f + random.Next(-12, 13);
			float thickness = 24f + random.Next(-4, 20);
			float tilt = -0.04f + (float)random.NextDouble() * 0.08f;
			starts[layer] = 28f + random.Next(0, 60);
			ends[layer] = width - 28f - random.Next(0, 60);
			for (int s = 0; s < samples; s++)
			{
				float t = (float)s / (samples - 1);
				float waveA = Mathf.Sin(t * Mathf.PI * 2.8f + seed * 0.23f + layer) * 7f;
				float waveB = Mathf.Sin(t * Mathf.PI * 19f + seed + layer * 2.3f) * 4f;
				float waveC = Mathf.Sin(t * Mathf.PI * 37f + seed * 0.51f + layer) * 2f;
				float waveD = Mathf.Sin(t * Mathf.PI * 4.2f + seed * 0.31f + layer) * 3.5f;
				float x = Mathf.Lerp(starts[layer], ends[layer], t);
				float center = centerY + tilt * (x - width * 0.5f) + waveA + waveD;
				float bulge = Mathf.Sin(t * Mathf.PI);
				float halfHeight = thickness * (0.12f + bulge * 0.92f);
				bottoms[layer, s] = center - halfHeight + waveB + random.Next(-3, 4);
				tops[layer, s] = center + halfHeight + waveC + random.Next(-3, 4);
			}
		}
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float alpha = 0f;
				for (int layer = 0; layer < layers; layer++)
				{
					float t = Mathf.InverseLerp(starts[layer], ends[layer], x);
					if (t < 0f || t > 1f)
					{
						continue;
					}
					float sample = t * (samples - 1);
					int index = Mathf.Clamp(Mathf.FloorToInt(sample), 0, samples - 2);
					float blend = Mathf.SmoothStep(0f, 1f, sample - index);
					float bottom = Mathf.Lerp(bottoms[layer, index], bottoms[layer, index + 1], blend);
					float top = Mathf.Lerp(tops[layer, index], tops[layer, index + 1], blend);
					float softness = 8.5f;
					float inside = Mathf.Min(Mathf.SmoothStep(0f, 1f, (y - bottom) / softness), Mathf.SmoothStep(0f, 1f, (top - y) / softness));
					float holeA = Mathf.Sin(t * Mathf.PI * 12f + seed * 0.29f + layer) * 0.5f + 0.5f;
					float holeB = Mathf.Sin(t * Mathf.PI * 27f + seed + layer * 1.7f) * 0.5f + 0.5f;
					float density = Mathf.Pow(Mathf.Sin(t * Mathf.PI * 5.4f + seed * 0.17f + layer), 2f);
					if (holeA > 0.88f && y < Mathf.Lerp(bottom, top, 0.38f))
					{
						inside *= 0.32f;
					}
					if (holeB > 0.92f && y > Mathf.Lerp(bottom, top, 0.62f))
					{
						inside *= 0.35f;
					}
					inside *= Mathf.Lerp(0.7f, 1f, density);
					alpha = Mathf.Max(alpha, Mathf.Clamp01(inside));
				}
				float edgeFade = Mathf.Min(Mathf.SmoothStep(0f, 80f, x), Mathf.SmoothStep(width, width - 80f, x));
				alpha *= Mathf.Lerp(0.5f, 1f, edgeFade);
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha * 0.92f));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeCircleSprite(int size)
	{
		Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		float radius = size * 0.48f;
		Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				float distance = Vector2.Distance(new Vector2(x, y), center);
				float alpha = Mathf.Clamp01((radius - distance) / 4f);
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeRadialGlowSprite(int size)
	{
		Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
		float radius = size * 0.5f;
		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / radius;
				float alpha = Mathf.Pow(Mathf.Clamp01(1f - distance), 2.2f);
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeRoundedRectSprite(int width, int height, int radius, float strength)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float dx = Mathf.Max(new float[3] { radius - x, x - (width - 1 - radius), 0f });
				float dy = Mathf.Max(new float[3] { radius - y, y - (height - 1 - radius), 0f });
				float distance = Mathf.Sqrt(dx * dx + dy * dy);
				float alpha = Mathf.Clamp01((radius + 1f - distance) / 3f) * strength;
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.Tight, new Vector4(radius, radius, radius, radius));
	}

	private Sprite MakeCaseActionButtonSprite(int width, int height, float radius, bool primary)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		Vector2 halfSize = new Vector2(width, height) * 0.5f - Vector2.one * 3f;
		Color topColor = primary ? new Color(0.42f, 0.16f, 0.73f, 1f) : new Color(0.095f, 0.052f, 0.145f, 1f);
		Color midColor = primary ? new Color(0.31f, 0.095f, 0.58f, 1f) : new Color(0.06f, 0.033f, 0.105f, 1f);
		Color bottomColor = primary ? new Color(0.2f, 0.055f, 0.42f, 1f) : new Color(0.035f, 0.02f, 0.07f, 1f);
		Color rimColor = primary ? new Color(0.86f, 0.56f, 1f, 1f) : new Color(0.58f, 0.38f, 0.86f, 1f);
		for (int y = 0; y < height; y++)
		{
			float v = y / Mathf.Max(1f, height - 1f);
			for (int x = 0; x < width; x++)
			{
				Vector2 point = new Vector2(x + 0.5f - width * 0.5f, y + 0.5f - height * 0.5f);
				float distance = RoundedBoxDistance(point, halfSize, radius);
				float inside = 1f - Mathf.SmoothStep(-0.85f, 1.2f, distance);
				float outerGlow = Mathf.Exp(-Mathf.Max(distance, 0f) / 18f) * (1f - inside);
				float rim = Mathf.Clamp01(1f - Mathf.SmoothStep(0.2f, 3.4f, Mathf.Abs(distance)));
				float innerGlow = Mathf.Clamp01(1f - Mathf.SmoothStep(5f, 15f, -distance)) * inside;
				float topSheen = Mathf.SmoothStep(0.58f, 1f, v) * inside * (primary ? 0.13f : 0.07f);
				float band = Mathf.Exp(-Mathf.Pow((v - 0.58f) * 3.4f, 2f)) * inside * (primary ? 0.035f : 0.02f);
				float bottomShade = Mathf.SmoothStep(0.5f, 0.02f, v) * inside * (primary ? 0.26f : 0.34f);
				Color pixel = v > 0.48f ? Color.Lerp(midColor, topColor, Mathf.InverseLerp(0.48f, 1f, v)) : Color.Lerp(bottomColor, midColor, Mathf.InverseLerp(0f, 0.48f, v));
				pixel = Color.Lerp(pixel, rimColor, rim * (primary ? 0.7f : 0.52f));
				pixel = Color.Lerp(pixel, new Color(rimColor.r, rimColor.g, rimColor.b, 1f), innerGlow * 0.1f);
				pixel.r += topSheen + band + rimColor.r * outerGlow * 0.24f;
				pixel.g += topSheen + band + rimColor.g * outerGlow * 0.18f;
				pixel.b += topSheen + band + rimColor.b * outerGlow * 0.27f;
				pixel = Color.Lerp(pixel, new Color(0.01f, 0.006f, 0.025f, 1f), bottomShade);
				pixel.a = Mathf.Clamp01(inside * (primary ? 0.985f : 0.92f) + outerGlow * (primary ? 0.28f : 0.14f));
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.Tight, new Vector4(radius, radius, radius, radius));
	}

	private Sprite MakeGemCounterSprite(int width, int height, float radius)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		Vector2 halfSize = new Vector2(width, height) * 0.5f - Vector2.one * 3f;
		Color topColor = new Color(0.12f, 0.085f, 0.21f, 1f);
		Color midColor = new Color(0.075f, 0.052f, 0.145f, 1f);
		Color bottomColor = new Color(0.04f, 0.03f, 0.09f, 1f);
		Color rimColor = new Color(0.58f, 0.45f, 0.94f, 1f);
		Color accentColor = new Color(0.34f, 0.78f, 1f, 1f);
		for (int y = 0; y < height; y++)
		{
			float v = y / Mathf.Max(1f, height - 1f);
			for (int x = 0; x < width; x++)
			{
				Vector2 point = new Vector2(x + 0.5f - width * 0.5f, y + 0.5f - height * 0.5f);
				float distance = RoundedBoxDistance(point, halfSize, radius);
				float inside = 1f - Mathf.SmoothStep(-0.85f, 1.25f, distance);
				float rim = Mathf.Clamp01(1f - Mathf.SmoothStep(0.3f, 3.3f, Mathf.Abs(distance)));
				float innerGlow = Mathf.Clamp01(1f - Mathf.SmoothStep(4f, 14f, -distance)) * inside;
				float outerGlow = Mathf.Exp(-Mathf.Max(distance, 0f) / 18f) * (1f - inside);
				float accent = Mathf.Exp(-Mathf.Pow((x / Mathf.Max(1f, width - 1f) - 0.16f) * 4.4f, 2f)) * inside;
				float topSheen = Mathf.SmoothStep(0.62f, 1f, v) * inside * 0.07f;
				float bottomShade = Mathf.SmoothStep(0.48f, 0f, v) * inside * 0.28f;
				Color pixel = v > 0.46f ? Color.Lerp(midColor, topColor, Mathf.InverseLerp(0.46f, 1f, v)) : Color.Lerp(bottomColor, midColor, Mathf.InverseLerp(0f, 0.46f, v));
				pixel = Color.Lerp(pixel, rimColor, rim * 0.54f);
				pixel = Color.Lerp(pixel, new Color(rimColor.r, rimColor.g, rimColor.b, 1f), innerGlow * 0.085f);
				pixel.r += topSheen + rimColor.r * outerGlow * 0.22f + accentColor.r * accent * 0.045f;
				pixel.g += topSheen + rimColor.g * outerGlow * 0.16f + accentColor.g * accent * 0.06f;
				pixel.b += topSheen + rimColor.b * outerGlow * 0.27f + accentColor.b * accent * 0.085f;
				pixel = Color.Lerp(pixel, new Color(0.01f, 0.007f, 0.03f, 1f), bottomShade);
				pixel.a = Mathf.Clamp01(inside * 0.955f + outerGlow * 0.18f);
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.Tight, new Vector4(radius, radius, radius, radius));
	}

	private Sprite MakeGradientBarSprite(int width, int height, Color left, Color right)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		float radius = height * 0.5f;
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				float dx = Mathf.Max(new float[3] { radius - x, x - (width - 1f - radius), 0f });
				float dy = Mathf.Max(new float[3] { radius - y, y - (height - 1f - radius), 0f });
				float distance = Mathf.Sqrt(dx * dx + dy * dy);
				float alpha = Mathf.Clamp01((radius + 0.5f - distance) / 1.5f);
				Color pixel = Color.Lerp(left, right, x / Mathf.Max(1f, width - 1f));
				pixel.a *= alpha;
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeCaseRarityStripSprite(int width, int height)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int y = 0; y < height; y++)
		{
			float v = 1f - y / Mathf.Max(1f, height - 1f);
			float alpha = v > 0.4f ? Mathf.Lerp(0.18f, 0.38f, Mathf.InverseLerp(0.4f, 1f, v)) : Mathf.Lerp(0f, 0.18f, Mathf.InverseLerp(0f, 0.4f, v));
			for (int x = 0; x < width; x++)
			{
				texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeCaseRouletteRailSprite(int width, int height)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int x = 0; x < width; x++)
		{
			float u = x / Mathf.Max(1f, width - 1f);
			Color pixel;
			if (u < 0.2f)
			{
				pixel = Color.Lerp(Color.clear, new Color(0.659f, 0.333f, 0.969f, 0.6f), u / 0.2f);
			}
			else if (u < 0.5f)
			{
				pixel = Color.Lerp(new Color(0.659f, 0.333f, 0.969f, 0.6f), new Color(0.753f, 0.518f, 0.988f, 0.8f), (u - 0.2f) / 0.3f);
			}
			else if (u < 0.8f)
			{
				pixel = Color.Lerp(new Color(0.753f, 0.518f, 0.988f, 0.8f), new Color(0.659f, 0.333f, 0.969f, 0.6f), (u - 0.5f) / 0.3f);
			}
			else
			{
				pixel = Color.Lerp(new Color(0.659f, 0.333f, 0.969f, 0.6f), Color.clear, (u - 0.8f) / 0.2f);
			}
			for (int y = 0; y < height; y++)
			{
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeHorizontalFadeSprite(int width, int height, bool leftToRight)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int x = 0; x < width; x++)
		{
			float u = x / Mathf.Max(1f, width - 1f);
			float fade = leftToRight ? 1f - u : u;
			Color pixel = new Color(0.024f, 0.016f, 0.063f, 0.97f * Mathf.SmoothStep(0f, 1f, fade));
			for (int y = 0; y < height; y++)
			{
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeCaseScanlineSprite(int width, int height)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int y = 0; y < height; y++)
		{
			float alpha = Mathf.Sin(y / Mathf.Max(1f, height - 1f) * Mathf.PI) * 0.04f;
			for (int x = 0; x < width; x++)
			{
				texture.SetPixel(x, y, new Color(0.659f, 0.333f, 0.969f, alpha));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeVerticalLineSprite(int width, int height, Color top, Color bottom)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int y = 0; y < height; y++)
		{
			Color pixel = Color.Lerp(bottom, top, y / Mathf.Max(1f, height - 1f));
			for (int x = 0; x < width; x++)
			{
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeTriangleSprite(int width, int height, bool down)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		Color color = new Color(1f, 1f, 1f, 1f);
		for (int y = 0; y < height; y++)
		{
			float v = y / Mathf.Max(1f, height - 1f);
			float halfWidth = (down ? 1f - v : v) * 0.5f;
			for (int x = 0; x < width; x++)
			{
				float u = x / Mathf.Max(1f, width - 1f) - 0.5f;
				float alpha = Mathf.InverseLerp(halfWidth + 0.035f, halfWidth - 0.035f, Mathf.Abs(u));
				texture.SetPixel(x, y, new Color(color.r, color.g, color.b, Mathf.Clamp01(alpha)));
			}
		}
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeVerticalFadeSprite(int width, int height)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int y = 0; y < height; y++)
		{
			float v = y / Mathf.Max(1f, height - 1f);
			float alpha = Mathf.Pow(1f - v, 1.65f) * 0.6f;
			for (int x = 0; x < width; x++)
			{
				texture.SetPixel(x, y, new Color(0.051f, 0.043f, 0.102f, alpha));
			}
		}
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeReferenceTabFrameSprite(int width, int height, float radius)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		Vector2 halfSize = new Vector2(width * 0.421f, height * 0.286f);
		Color topColor = new Color(0.108f, 0.078f, 0.18f, 1f);
		Color midColor = new Color(0.061f, 0.047f, 0.105f, 1f);
		Color bottomColor = new Color(0.035f, 0.03f, 0.07f, 1f);
		Color rimColor = new Color(0.69f, 0.38f, 1f, 0.62f);
		for (int y = 0; y < height; y++)
		{
			float v = Mathf.InverseLerp(height * 0.5f - halfSize.y, height * 0.5f + halfSize.y, y);
			for (int x = 0; x < width; x++)
			{
				Vector2 point = new Vector2(x + 0.5f - width * 0.5f, y + 0.5f - height * 0.5f);
				float distance = RoundedBoxDistance(point, halfSize, radius);
				float inside = 1f - Mathf.SmoothStep(-0.85f, 1.25f, distance);
				float rim = Mathf.Clamp01(1f - Mathf.SmoothStep(1f, 3.4f, Mathf.Abs(distance)));
				float innerGlow = Mathf.Clamp01(1f - Mathf.SmoothStep(3.5f, 12f, -distance)) * inside;
				float outerGlow = Mathf.Exp(-Mathf.Max(distance, 0f) / 12f) * (1f - inside);
				float farGlow = Mathf.Exp(-Mathf.Max(distance, 0f) / 34f) * (1f - inside);
				float topSheen = Mathf.SmoothStep(0.58f, 1f, v) * inside * 0.055f;
				float bottomShade = Mathf.SmoothStep(0.52f, 0f, v) * inside * 0.24f;
				Color body = v > 0.45f ? Color.Lerp(midColor, topColor, Mathf.InverseLerp(0.45f, 1f, v)) : Color.Lerp(bottomColor, midColor, Mathf.InverseLerp(0f, 0.45f, v));
				Color pixel = new Color(body.r, body.g, body.b, inside * 0.96f);
				pixel = Color.Lerp(pixel, rimColor, rim * 0.88f);
				pixel = Color.Lerp(pixel, new Color(rimColor.r, rimColor.g, rimColor.b, pixel.a), innerGlow * 0.12f);
				pixel.r += rimColor.r * outerGlow * 0.36f + rimColor.r * farGlow * 0.1f + topSheen;
				pixel.g += rimColor.g * outerGlow * 0.28f + rimColor.g * farGlow * 0.08f + topSheen;
				pixel.b += rimColor.b * outerGlow * 0.4f + rimColor.b * farGlow * 0.12f + topSheen;
				pixel = Color.Lerp(pixel, new Color(0f, 0f, 0f, pixel.a), bottomShade);
				pixel.a = Mathf.Max(pixel.a, Mathf.Max(outerGlow * 0.42f, farGlow * 0.15f));
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Sprite MakeReferenceActiveTabSprite(int width, int height, float radius)
	{
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		Vector2 halfSize = new Vector2(width * 0.34f, height * 0.31f);
		Color rimColor = new Color(0.86f, 0.66f, 1f, 0.76f);
		for (int y = 0; y < height; y++)
		{
			float v = Mathf.InverseLerp(height * 0.5f - halfSize.y, height * 0.5f + halfSize.y, y);
			for (int x = 0; x < width; x++)
			{
				Vector2 point = new Vector2(x + 0.5f - width * 0.5f, y + 0.5f - height * 0.5f);
				float distance = RoundedBoxDistance(point, halfSize, radius);
				float inside = 1f - Mathf.SmoothStep(-0.85f, 1.25f, distance);
				float rim = Mathf.Clamp01(1f - Mathf.SmoothStep(1f, 3.2f, Mathf.Abs(distance)));
				float outerGlow = Mathf.Exp(-Mathf.Max(distance, 0f) / 10f) * (1f - inside);
				float farGlow = Mathf.Exp(-Mathf.Max(distance, 0f) / 28f) * (1f - inside);
				float innerGlow = Mathf.Exp(-Mathf.Max(-distance, 0f) / 26f) * inside;
				float centerWeight = 1f - Mathf.Abs(point.x / halfSize.x);
				Color body = Color.Lerp(new Color(0.07f, 0.045f, 0.12f, 0.94f), new Color(0.108f, 0.078f, 0.18f, 0.9f), v);
				body.r += centerWeight * 0.05f;
				body.b += centerWeight * 0.065f;
				Color pixel = new Color(body.r, body.g, body.b, body.a * inside);
				pixel = Color.Lerp(pixel, rimColor, rim * 0.86f);
				pixel.r += rimColor.r * outerGlow * 0.32f + rimColor.r * farGlow * 0.12f;
				pixel.g += rimColor.g * outerGlow * 0.22f + rimColor.g * farGlow * 0.08f;
				pixel.b += rimColor.b * outerGlow * 0.4f + rimColor.b * farGlow * 0.14f;
				pixel.a = Mathf.Max(pixel.a, Mathf.Max(outerGlow * 0.36f, farGlow * 0.13f));
				pixel = Color.Lerp(pixel, new Color(rimColor.r, rimColor.g, rimColor.b, pixel.a), innerGlow * 0.1f);
				texture.SetPixel(x, y, pixel);
			}
		}
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
	}

	private float RoundedBoxDistance(Vector2 point, Vector2 halfSize, float radius)
	{
		Vector2 q = new Vector2(Mathf.Abs(point.x), Mathf.Abs(point.y)) - (halfSize - Vector2.one * radius);
		Vector2 outside = new Vector2(Mathf.Max(q.x, 0f), Mathf.Max(q.y, 0f));
		return outside.magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - radius;
	}
}
