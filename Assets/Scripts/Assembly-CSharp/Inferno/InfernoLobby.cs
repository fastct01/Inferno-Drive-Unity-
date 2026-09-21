using System;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Port of Il2CppOutput/InfernoLobby.inc (+ InfernoLobbyBindings.inc): the hand-written lobby of the
// shipped iOS export. Statement-by-statement; the C++ helpers (Str/Hex/Keep/Place/Block/Card/Label/...)
// keep their names. Native PNG decoding (InfernoLobby_LoadPNG on Data/Raw/InfernoLobby/*.png) is replaced
// by Texture2D.LoadImage on the same files stored as Assets/Resources/InfernoLobby/*.bytes.
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoLobby
	{
		// Original game theme: black with violet neon. Values are the ones the untouched
		// garage / profile / case screens of this export use.
		public const uint Black = 0x000000, Ink = 0x050309, Night = 0x080513, Slate = 0x120C1F;
		public const uint Plum = 0x1A0F33, Grape = 0x420E7A, Mulberry = 0x3B1466, Indigo = 0x7A22D6;
		public const uint Electric = 0x9433FF, Violet = 0xB840FF, Magenta = 0xBF66FF, Orchid = 0xB26BFF;
		public const uint Lavender = 0x948CD1, Lilac = 0xAD8ADB, Blossom = 0xD199F5, Pearl = 0xE0D6F0;
		public const float ContentWidth = 960f, ContentHeight = 1550f;   /* [user request 2026-09-21 restyle] taller: gem row + picture + cards; was 1440 */
		public const int MapCount = 6;
		public static readonly string[] MapNames = { "DAYLIGHT CITY", "NEON NIGHT", "RAIN RUN", "STORM CHASE", "FALLEN SKY", "PLANET HORIZON" };
		public static readonly string[] MapDescriptions = { "Golden streets. Open throttle.", "Electric skyline. Endless asphalt.", "Rain-lit roads. Stay sharp.", "Lightning overhead. Keep your line.", "A sky full of falling stars.", "A different world above the road." };
		private static readonly uint[] MapColors = { Blossom, Violet, Lavender, Electric, Magenta, Orchid };
		// Screenshots shipped in Data/Raw/InfernoLobby (bundle-relative). null keeps the hero art.
		// Every map is the same city (the presets only change the weather/lighting), so DAYLIGHT
		// CITY shows the NEON NIGHT shot re-lit as day (tools/relight_map_card.py) — same view.
		private static readonly string[] MapImages = { "Data/Raw/InfernoLobby/daylight_city.png", "Data/Raw/InfernoLobby/neon_night.png", null, null, null, null };
		/* [user request 2026-09-20] maps without their own picture show the closest existing city art,
		   tinted with the map's accent colour, so a locked map still shows a picture of the map. */
		private static readonly string[] MapFallbackImages = { null, null, "Data/Raw/InfernoLobby/neon_night.png", "Data/Raw/InfernoLobby/neon_night.png", "Data/Raw/InfernoLobby/daylight_city.png", "Data/Raw/InfernoLobby/neon_night.png" };
		/* [user request 2026-09-21 restyle] picture 936x440 centred at y -232 inside a 464-tall frame; was 820x376 at -246 */
		public const float ArtWidth = 936f, ArtHeight = 476f, ArtY = -250f;
		private static RectTransform s_mapLock;   // the LOCKED overlay inside the art frame (Build / RefreshMap)

		public static Color Hex(uint value, float alpha = 1f)
		{
			return new Color(((value >> 16) & 255) / 255f, ((value >> 8) & 255) / 255f, (value & 255) / 255f, alpha);
		}

		public static void Paint(Image image, uint color)
		{
			image.color = Hex(color);
		}

		public static void Caption(Text text, string value)
		{
			text.text = value;
		}

		public static void Place(RectTransform rect, float x, float y, float width, float height)
		{
			Place(rect, x, y, width, height, new Vector2(0.5f, 1f));
		}

		public static void Place(RectTransform rect, float x, float y, float width, float height, Vector2 anchor)
		{
			rect.anchorMin = anchor;
			rect.anchorMax = anchor;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = new Vector2(width, height);
			rect.anchoredPosition = new Vector2(x, y);
			rect.localScale = Vector3.one;
			rect.localRotation = Quaternion.identity;
		}

		public static RectTransform Root(Transform parent, string name)
		{
			RectTransform rect = new GameObject(name).AddComponent<RectTransform>();
			rect.SetParent(parent, false);
			return rect;
		}

		public static Image Block(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height, uint color, bool rounded = false)
		{
			Image image = game.MakeUiBlock(parent, name, new Vector2(x, y), new Vector2(width, height), TextAnchor.UpperCenter, Hex(color));
			Place(image.rectTransform, x, y, width, height);
			if (rounded)
			{
				image.sprite = game.roundedGlassSprite;
				image.type = Image.Type.Sliced;
			}
			image.raycastTarget = false;
			return image;
		}

		public static Image Card(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height, uint fill, uint rim)
		{
			Block(game, parent, "CardShadow", x, y - 10f, width + 4f, height + 4f, Black, true);
			Block(game, parent, "CardRim", x, y, width + 4f, height + 4f, rim, true);
			return Block(game, parent, name, x, y, width, height, fill, true);
		}

		// The game's neon button frame (the garage "кнопка в гараж" art: chamfered dark
		// glass with a violet contour), re-cut as a 9-slice so it fits any size.
		private static Sprite s_neonFrame;
		// Procedural button art: an octagon (chamfered corners) drawn into a small
		// texture and 9-sliced. The textures are symmetric so nothing gets stretched
		// but the flat middle.
		private const int FrameTex = 128, FrameGlow = 10, FrameChamfer = 26, FrameEdge = 4;
		private const int FrameBorder = FrameGlow + FrameChamfer + FrameEdge;   // 40 px per side

		private static void Set(ref Color32 px, int r, int g, int b, int a)
		{
			px.r = (byte)Math.Max(0, Math.Min(255, r));
			px.g = (byte)Math.Max(0, Math.Min(255, g));
			px.b = (byte)Math.Max(0, Math.Min(255, b));
			px.a = (byte)Math.Max(0, Math.Min(255, a));
		}

		// Signed distance (px) to the octagon inset by `inset` from the texture edge; positive inside.
		private static float Octagon(int x, int y, int inset, int chamfer)
		{
			const int n = FrameTex - 1;
			float d = Math.Min(Math.Min((float)(x - inset), (float)(n - inset - x)), Math.Min((float)(y - inset), (float)(n - inset - y)));
			d = Math.Min(d, ((x - inset) + (y - inset) - chamfer) / 1.41421f);
			d = Math.Min(d, ((n - inset - x) + (y - inset) - chamfer) / 1.41421f);
			d = Math.Min(d, ((x - inset) + (n - inset - y) - chamfer) / 1.41421f);
			d = Math.Min(d, ((n - inset - x) + (n - inset - y) - chamfer) / 1.41421f);
			return d;
		}

		// Frame: dark glass with a violet contour and a soft outer glow.
		private static void FramePixel(ref Color32 px, int x, int y)
		{
			float d = Octagon(x, y, FrameGlow, FrameChamfer);
			if (d < 0f)
			{
				float t = Math.Max(0f, 1f + d / FrameGlow);
				Set(ref px, 184, 64, 255, (int)(120f * t * t));
			}
			else if (d < FrameEdge)
			{
				Set(ref px, 200, 96, 255, 255);
			}
			else if (d < FrameEdge + 2)
			{
				Set(ref px, 120, 40, 200, 235);
			}
			else
			{
				Set(ref px, 9, 5, 16, 236);
			}
		}

		// Plate (PLAY): the same octagon filled edge to edge with electric violet, a
		// little lighter at the top so it reads as a lit glass slab.
		private static void PlatePixel(ref Color32 px, int x, int y)
		{
			float d = Octagon(x, y, 0, FrameChamfer);
			if (d < -0.5f)
			{
				Set(ref px, 0, 0, 0, 0);
				return;
			}
			float t = y / (float)(FrameTex - 1);
			Set(ref px, (int)(136 + (186 - 136) * t), (int)(40 + (108 - 40) * t), (int)(232 + (255 - 232) * t), (int)(255f * Math.Min(1f, d + 0.5f)));
		}

		private delegate void PixelShader(ref Color32 px, int x, int y);

		private static Sprite SlicedSprite(PixelShader pixel, int border = FrameBorder)
		{
			Texture2D texture = new Texture2D(FrameTex, FrameTex, TextureFormat.RGBA32, false);
			Color32[] colors = new Color32[FrameTex * FrameTex];
			for (int y = 0; y < FrameTex; y++)
			{
				for (int x = 0; x < FrameTex; x++)
				{
					pixel(ref colors[y * FrameTex + x], x, y);
				}
			}
			texture.SetPixels32(colors);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			Rect rect = new Rect(0f, 0f, FrameTex, FrameTex);
			Vector4 borders = new Vector4(border, border, border, border);
			return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.Tight, borders);
		}

		// Glass bevel (dock tabs): a wide translucent lavender rim, lit from the top so
		// it reads as a thick glass edge, a thin dark seam, and dark glass inside.
		private const int GlassInset = 6, GlassRim = 14, GlassChamfer = 26;
		private const int GlassBorder = GlassInset + GlassChamfer + GlassRim + 2;   // 48 px per side

		private static void GlassPixel(ref Color32 px, int x, int y)
		{
			float d = Octagon(x, y, GlassInset, GlassChamfer);
			float t = y / (float)(FrameTex - 1);
			if (d < 0f)
			{
				float k = Math.Max(0f, 1f + d / GlassInset);
				Set(ref px, 170, 90, 255, (int)(70f * k * k));
			}
			else if (d < 1.5f)
			{
				Set(ref px, 236, 226, 255, 255);
			}
			else if (d < GlassRim)
			{
				float depth = (d - 1.5f) / (GlassRim - 1.5f);
				int baseValue = (int)(150 + 70 * t - 25 * depth);
				Set(ref px, baseValue, baseValue - 12, Math.Min(255, baseValue + 30), 232);
			}
			else if (d < GlassRim + 2)
			{
				Set(ref px, 40, 22, 66, 255);
			}
			else
			{
				Set(ref px, 12, 7, 22, 238);
			}
		}

		private static Sprite s_glassFrame;

		private static Sprite GlassFrameSprite(ShadowRunPrototype game)
		{
			if (s_glassFrame == null) s_glassFrame = SlicedSprite(GlassPixel, GlassBorder);
			return s_glassFrame;
		}

		public static Sprite NeonFrameSprite(ShadowRunPrototype game)
		{
			if (s_neonFrame == null) s_neonFrame = SlicedSprite(FramePixel);
			return s_neonFrame;
		}

		private static Sprite s_neonPlate;

		private static Sprite NeonPlateSprite(ShadowRunPrototype game)
		{
			if (s_neonPlate == null) s_neonPlate = SlicedSprite(PlatePixel);
			return s_neonPlate;
		}

		// A faceted gem icon in the classic cut: flat top, a row of triangular crown
		// facets, the girdle, and pavilion facets fanning down to the tip.
		private const int GemTexW = 128, GemTexH = 96;
		private const float GemTop = 0.62f, GemGirdle = 0.12f;

		private struct Facet
		{
			public int count;
			public float[] x;
			public float[] y;
			public float shade;
			public bool pavilion;

			public Facet(int count, float[] x, float[] y, float shade, bool pavilion)
			{
				this.count = count; this.x = x; this.y = y; this.shade = shade; this.pavilion = pavilion;
			}
		}

		private static readonly Facet[] GemFacets =
		{
			new Facet(4, new[] { -0.36f, 0.36f, 0.30f, -0.30f }, new[] { GemTop, GemTop, GemGirdle, GemGirdle }, 1.30f, false),   // table
			new Facet(3, new[] { -0.78f, -0.36f, -0.60f }, new[] { GemTop, GemTop, GemGirdle, 0f }, 1.55f, false),                 // crown left, lit facet
			new Facet(3, new[] { -1.0f, -0.60f, -0.78f }, new[] { GemGirdle, GemGirdle, GemTop, 0f }, 0.95f, false),               // crown left outer
			new Facet(3, new[] { -0.36f, -0.30f, -0.60f }, new[] { GemTop, GemGirdle, GemGirdle, 0f }, 1.10f, false),              // crown left inner
			new Facet(3, new[] { 0.36f, 0.78f, 0.60f }, new[] { GemTop, GemTop, GemGirdle, 0f }, 1.05f, false),                    // crown right
			new Facet(3, new[] { 0.60f, 1.0f, 0.78f }, new[] { GemGirdle, GemGirdle, GemTop, 0f }, 0.60f, false),                  // crown right outer
			new Facet(3, new[] { 0.30f, 0.36f, 0.60f }, new[] { GemGirdle, GemTop, GemGirdle, 0f }, 0.80f, false),                 // crown right inner
			new Facet(3, new[] { -1.0f, -0.60f, 0.0f }, new[] { GemGirdle, GemGirdle, -1.0f, 0f }, 0.70f, true),                   // pavilion, left to right
			new Facet(3, new[] { -0.60f, -0.30f, 0.0f }, new[] { GemGirdle, GemGirdle, -1.0f, 0f }, 1.00f, true),
			new Facet(3, new[] { -0.30f, 0.30f, 0.0f }, new[] { GemGirdle, GemGirdle, -1.0f, 0f }, 1.22f, true),
			new Facet(3, new[] { 0.30f, 0.60f, 0.0f }, new[] { GemGirdle, GemGirdle, -1.0f, 0f }, 0.72f, true),
			new Facet(3, new[] { 0.60f, 1.0f, 0.0f }, new[] { GemGirdle, GemGirdle, -1.0f, 0f }, 0.48f, true),
		};

		private static int GemFacetAt(float u, float v)
		{
			for (int f = 0; f < GemFacets.Length; f++)
			{
				Facet facet = GemFacets[f];
				bool positive = false, negative = false;
				for (int i = 0; i < facet.count; i++)
				{
					int j = (i + 1) % facet.count;
					float cross = (facet.x[j] - facet.x[i]) * (v - facet.y[i]) - (facet.y[j] - facet.y[i]) * (u - facet.x[i]);
					if (cross > 0f) positive = true; else if (cross < 0f) negative = true;
				}
				if (!(positive && negative)) return f;
			}
			return -1;
		}

		private static float Unit(float value)
		{
			return Math.Max(0f, Math.Min(1f, value));
		}

		private static void GemPixel(ref Color32 px, int x, int y)
		{
			const int samples = 4;
			int inside = 0;
			int[] hits = new int[GemFacets.Length];
			float rr = 0f, gg = 0f, bb = 0f;
			for (int sy = 0; sy < samples; sy++)
			{
				for (int sx = 0; sx < samples; sx++)
				{
					float u = ((x + (sx + 0.5f) / samples) / GemTexW * 2f - 1f) / 0.94f;
					float v = ((y + (sy + 0.5f) / samples) / GemTexH * 2f - 1f) / 0.94f * 0.95f - 0.19f;   // stone centred in the texture
					int f = GemFacetAt(u, v);
					if (f < 0) continue;
					inside++; hits[f]++;
					float shade = GemFacets[f].shade;
					if (GemFacets[f].pavilion) shade *= 0.70f + 0.30f * Unit((v + 1f) / 1.12f);   // darker toward the tip
					shade *= 0.90f + 0.14f * Unit((v + 1f) / 1.62f);                                 // lighter toward the top
					float r = 0.62f, g = 0.30f, b = 0.95f;                                            // purple body
					if (shade < 1f) { r *= shade; g *= shade; b *= shade; }
					else { r += (1f - r) * (shade - 1f); g += (1f - g) * (shade - 1f); b += (1f - b) * (shade - 1f); }
					rr += r; gg += g; bb += b;
				}
			}
			if (inside == 0) { Set(ref px, 0, 0, 0, 0); return; }
			rr /= inside; gg /= inside; bb /= inside;
			int majority = 0;
			for (int f = 1; f < GemFacets.Length; f++) if (hits[f] > hits[majority]) majority = f;
			float edge = (inside - hits[majority]) / (float)inside;                        // facet boundary: white cut line
			float rim = inside < samples * samples ? 0.75f : 0f;                          // outline: white rim
			float light = Math.Min(1f, edge * 2.2f + rim);
			rr += (1f - rr) * light; gg += (1f - gg) * light; bb += (1f - bb) * light;
			Set(ref px, (int)(rr * 255f), (int)(gg * 255f), (int)(bb * 255f), 255 * inside / (samples * samples));
		}

		private static Sprite s_gem;
		public static readonly int[] s_lastLoaded = new int[2];   // width/height of the last LoadTexture (read by InfernoGarage.Picture)

		// ShopArtScope: RAII counter in InfernoLobby.inc (s_shopArtDepth); C# port exposes explicit Begin/End.
		public static void BeginShopArtScope() { shopArtDepth++; }
		public static void EndShopArtScope() { shopArtDepth--; }
		private static int shopArtDepth;

		// ---- Hearts (HUD lives) ----------------------------------------------------
		// The game's life icons load the flat "сердце" resource; the patcher routes it
		// here. A glossy neon heart in the lobby palette. 128 px, drawn once.
		private const int HeartTex = 128;

		// Approximate signed distance (px, positive inside) to a heart centred in the texture.
		private static float HeartDist(float px, float py)
		{
			float x = (px - HeartTex * 0.5f) / (HeartTex * 0.40f);
			float y = -(py - HeartTex * 0.53f) / (HeartTex * 0.40f) + 0.15f;
			float a = x * x + y * y - 1.0f;
			float f = a * a * a - x * x * y * y * y;          // classic heart curve, < 0 inside
			float gx = 6f * a * a * x - 2f * x * y * y * y, gy = 6f * a * a * y - 3f * x * x * y * y;
			float g = (float)Math.Sqrt(gx * gx + gy * gy) + 1e-4f;
			return -f / g * (HeartTex * 0.40f);
		}

		private static void HeartPixel(ref Color32 px, int x, int y)
		{
			float d = HeartDist(x + 0.5f, (HeartTex - 1 - y) + 0.5f);   // row 0 is the bottom
			float t = y / (float)(HeartTex - 1);                         // 0 bottom .. 1 top
			if (d < -12f) { Set(ref px, 0, 0, 0, 0); return; }
			if (d < 0f) { float k = 1.0f + d / 12.0f; Set(ref px, 255, 70, 200, (int)(110f * k * k)); return; }   // outer glow
			if (d < 3.5f) { Set(ref px, 255, 214, 250, 255); return; }                // bright rim
			if (d < 5.5f) { Set(ref px, 150, 40, 200, 255); return; }                 // dark seam
			int r = (int)(120 + (245 - 120) * t), g = (int)(20 + (70 - 20) * t), b = (int)(150 + (225 - 150) * t);   // glass body
			float hx = (x - HeartTex * 0.36f) / 16.0f, hy = (y - HeartTex * 0.66f) / 11.0f;                        // specular
			float h = Math.Max(0f, 1.0f - (hx * hx + hy * hy));
			r += (int)((255 - r) * h * 0.9f); g += (int)((255 - g) * h * 0.9f); b += (int)((255 - b) * h * 0.9f);
			Set(ref px, r, g, b, 255);
		}

		private static Sprite s_heart;

		public static Sprite HeartSprite()
		{
			if (s_heart != null) return s_heart;
			Texture2D texture = new Texture2D(HeartTex, HeartTex, TextureFormat.RGBA32, false);
			Color32[] colors = new Color32[HeartTex * HeartTex];
			for (int y = 0; y < HeartTex; y++)
				for (int x = 0; x < HeartTex; x++)
					HeartPixel(ref colors[y * HeartTex + x], x, y);
			texture.SetPixels32(colors);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			Rect rect = new Rect(0f, 0f, HeartTex, HeartTex);
			s_heart = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, Vector4.zero);
			return s_heart;
		}

		public static bool IsHeartResource(string name)
		{
			return name != null && name == "сердце";
		}

		public static bool IsDiamondResource(string name)
		{
			if (name == null || shopArtDepth > 0) return false;
			string[] names = { "diamons_final", "алмаз 1", "алмаз 3" };
			foreach (string n in names) if (name == n) return true;
			return false;
		}

		private static bool s_gemLoading;

		public static Sprite GemSprite(ShadowRunPrototype game)
		{
			if (s_gem != null) return s_gem;
			Texture2D png = LoadTexture("Data/Raw/InfernoLobby/crystal.png", true);
			if (png != null)
			{
				Rect full = new Rect(0f, 0f, s_lastLoaded[0], s_lastLoaded[1]);
				s_gem = Sprite.Create(png, full, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, Vector4.zero);
				return s_gem;
			}
			// The loaders are patched to route diamond resources back here; the guard
			// makes that call fall through to the game's original loader instead.
			if (game != null && !s_gemLoading)
			{
				s_gemLoading = true;
				Sprite art = game.LoadTightMenuArtSprite("diamons_final");
				s_gemLoading = false;
				if (art != null) { s_gem = art; return s_gem; }
			}
			if (s_gemLoading) return null;
			Texture2D texture = new Texture2D(GemTexW, GemTexH, TextureFormat.RGBA32, false);
			Color32[] colors = new Color32[GemTexW * GemTexH];
			for (int y = 0; y < GemTexH; y++)
				for (int x = 0; x < GemTexW; x++)
					GemPixel(ref colors[y * GemTexW + x], x, y);
			texture.SetPixels32(colors);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			Rect rect = new Rect(0f, 0f, GemTexW, GemTexH);
			s_gem = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, Vector4.zero);
			return s_gem;
		}

		public const uint FrameText = 0xFAE6FF;   // the pink-white the game uses on these frames

		public static Image NeonFrame(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height, uint tint = 0xFFFFFF)
		{
			Sprite sprite = NeonFrameSprite(game);
			Image image = Block(game, parent, name, x, y, width, height, sprite != null ? tint : Ink, sprite == null);
			if (sprite != null)
			{
				image.sprite = sprite;
				image.type = Image.Type.Sliced;
			}
			return image;
		}

		// The filled plate for the PLAY button: sized to the frame's inside so the
		// colour runs edge to edge up to the neon contour.
		public const float PlateInset = FrameGlow + FrameEdge + 1;

		public static Image NeonPlate(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height)
		{
			Sprite sprite = NeonPlateSprite(game);
			Image image = Block(game, parent, name, x, y, width, height, sprite != null ? 0xFFFFFFu : Electric, sprite == null);
			if (sprite != null)
			{
				image.sprite = sprite;
				image.type = Image.Type.Sliced;
			}
			return image;
		}

		public static Text Label(ShadowRunPrototype game, Transform parent, string name, string value, float x, float y, float width, float height, int size, uint color = Pearl, int alignment = 4)
		{
			Text text = game.MakeText(parent, value, size, TextAnchor.UpperCenter, new Vector2(x, y));
			text.gameObject.name = name;
			Place(text.rectTransform, x, y, width, height);
			text.alignment = (TextAnchor)alignment;
			text.fontStyle = FontStyle.Bold;
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = Math.Max(16, size - 6);
			text.resizeTextMaxSize = size;
			text.color = Hex(color);
			text.raycastTarget = false;
			return text;
		}

		public static Transform Find(Transform root, string name)
		{
			return root != null ? root.Find(name) : null;
		}

		private static readonly RawImage[] s_mapArt = new RawImage[MapCount];
		private static readonly int[,] s_mapArtSize = new int[MapCount, 2];
		private static readonly bool[] s_mapArtTried = new bool[MapCount];

		// Bundle path "Data/Raw/InfernoLobby/<name>.png" -> Resources "InfernoLobby/<name>" (.bytes).
		public static Texture2D LoadTexture(string file, bool mips = false)
		{
			const string prefix = "Data/Raw/InfernoLobby/";
			string resource = file.StartsWith(prefix, StringComparison.Ordinal) ? file.Substring(prefix.Length) : file;
			if (resource.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) resource = resource.Substring(0, resource.Length - 4);
			TextAsset bytes = Resources.Load<TextAsset>("InfernoLobby/" + resource);
			if (bytes == null) return null;
			Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, mips);
			if (!texture.LoadImage(bytes.bytes))
			{
				UnityEngine.Object.Destroy(texture);
				return null;
			}
			texture.wrapMode = TextureWrapMode.Clamp;
			s_lastLoaded[0] = texture.width; s_lastLoaded[1] = texture.height;
			return texture;
		}

		// Centre-crops the texture to the art frame (like CSS background-size: cover).
		private static void CoverCrop(RawImage image, int w, int h)
		{
			float frame = ArtWidth / ArtHeight, tex = w / (float)h;
			Rect uv = new Rect(0f, 0f, 1f, 1f);
			if (tex > frame) { uv.width = frame / tex; uv.xMin = (1f - uv.width) / 2f; uv.width = frame / tex; }
			else { uv.height = tex / frame; uv.yMin = (1f - uv.height) / 2f; uv.height = tex / frame; }
			image.uvRect = uv;
		}

		private static bool ShowMapArt(ShadowRunPrototype game, int selected)
		{
			bool shown = false;
			for (int i = 0; i < MapCount; i++)
			{
				RawImage art = s_mapArt[i];
				if (art == null) continue;
				bool active = false;
				string file = MapImages[i] ?? MapFallbackImages[i];   /* [user request 2026-09-20] */
				if (i == selected && file != null)
				{
					if (!s_mapArtTried[i])
					{
						s_mapArtTried[i] = true;
						Texture2D texture = LoadTexture(file);
						if (texture != null)
						{
							art.texture = texture;
							// a borrowed picture is tinted towards the map's accent so each map reads differently
							art.color = MapImages[i] != null ? Color.white : Color.Lerp(Color.white, Hex(MapColors[i]), 0.45f);
							s_mapArtSize[i, 0] = s_lastLoaded[0]; s_mapArtSize[i, 1] = s_lastLoaded[1];
							CoverCrop(art, s_mapArtSize[i, 0], s_mapArtSize[i, 1]);
						}
					}
					active = s_mapArtSize[i, 0] > 0;
				}
				art.gameObject.SetActive(active);
				shown = shown || active;
			}
			if (game.homeHeroArtRect != null)
				game.homeHeroArtRect.gameObject.SetActive(!shown);
			return shown;
		}

		// ---- Crystal-locked maps ---------------------------------------------------
		// Maps 0 and 1 are free; the rest cost crystals. Unlocks persist locally in
		// PlayerPrefs (the cloud profile has no map fields).
		public static readonly int[] MapPrices = { 0, 0, 150, 250, 400, 600 };
		private static string MapUnlockKey(int i) { return "InfernoDrive.MapUnlocked." + i; }
		public static bool MapUnlocked(int i) { return MapPrices[i] <= 0 || PlayerPrefs.GetInt(MapUnlockKey(i), 0) == 1; }
		private static void SetMapUnlocked(int i) { PlayerPrefs.SetInt(MapUnlockKey(i), 1); PlayerPrefs.Save(); }
		private static int Crystals() { return SupabaseGameManager.Crystals; }

		private static void SetCrystalBalance(ShadowRunPrototype game, int amount)
		{
			game.menuGemCount = amount;
			SupabaseGameManager.SetCrystals(amount);
			_ = SupabaseGameManager.SyncProfileAfterEconomyChangeAsync();
			if (game.gemAmountText != null) Caption(game.gemAmountText, amount.ToString());
		}

		public static int SelectedMap() { return Math.Max(0, Math.Min(MapCount - 1, PlayerPrefs.GetInt("InfernoDrive.SelectedMap", 1))); }

		private static void RefreshMap(ShadowRunPrototype game)
		{
			if (game.homeHeroRoot == null) return;
			int selected = SelectedMap();
			Transform title = Find(game.homeHeroRoot, "MapName");
			Transform subtitle = Find(game.homeHeroRoot, "MapDescription");
			if (title != null) Caption(title.GetComponent<Text>(), MapNames[selected]);
			if (subtitle != null) Caption(subtitle.GetComponent<Text>(), "");   /* [user request 2026-09-21 restyle] status line only; was MapDescriptions[selected] */
			string counter = "MAP " + (selected + 1) + " / " + MapCount;
			Transform count = Find(game.homeHeroRoot, "MapCounter");
			if (count != null) Caption(count.GetComponent<Text>(), counter);
			Transform accent = Find(game.homeHeroRoot, "MapAccent");
			if (accent != null) Paint(accent.GetComponent<Image>(), MapColors[selected]);
			ShowMapArt(game, selected);
			bool locked = !MapUnlocked(selected);
			Transform lockRoot = s_mapLock;   /* [user request 2026-09-20] lives inside the (renamed) art frame; kept by reference */
			if (lockRoot != null) lockRoot.gameObject.SetActive(locked);
			if (lockRoot != null && locked)
			{
				Transform p = Find(lockRoot, "LockPrice");
				if (p != null) Caption(p.GetComponent<Text>(), MapPrices[selected].ToString());
			}
			if (game.primaryButtonLabel != null && !game.running)
			{
				Caption(game.primaryButtonLabel, locked ? "U N L O C K   " + MapPrices[selected] : "P  L  A  Y");
			}
		}

		// Called at the start of StartRun. Returns true to block the run: on a locked
		// map the PLAY tap either buys the map (enough crystals) or shows what is
		// missing; the next tap on the now-unlocked map starts the run.
		public static bool HandleLockedMap(ShadowRunPrototype game)
		{
			int selected = SelectedMap();
			if (MapUnlocked(selected)) return false;
			if (game.running) return false;
			int price = MapPrices[selected], have = Crystals();
			Transform subtitle = Find(game.homeHeroRoot, "MapDescription");
			if (have >= price)
			{
				SetCrystalBalance(game, have - price);
				SetMapUnlocked(selected);
				if (subtitle != null) Caption(subtitle.GetComponent<Text>(), "MAP UNLOCKED. TAP PLAY.");
				RefreshMap(game);
				if (subtitle != null) Caption(subtitle.GetComponent<Text>(), "MAP UNLOCKED. TAP PLAY.");
			}
			else
			{
				if (subtitle != null) Caption(subtitle.GetComponent<Text>(), "NEED " + (price - have) + " MORE CRYSTALS");
			}
			return true;
		}

		private static void ApplySelectedMap(ShadowRunPrototype game)
		{
			if (game.dynamicWeatherReady)
				game.SetWeatherPreset(SelectedMap(), 0f);
		}

		private static RawImage s_caseArt;
		private static bool s_caseArtBound;

		// Keeps the case preview camera rendering while the home page is visible so the
		// Cases card shows the real crate; the case screen owns the camera while open.
		private static void HomePreview(ShadowRunPrototype game, bool on)
		{
			if (!on)
			{
				if (game.casePreviewReady) game.SetCasePreviewVisible(false);
				return;
			}
			if (game.caseOverlayRoot != null && game.caseOverlayRoot.activeInHierarchy) return;
			if (!game.EnsureCasePreviewWorld()) return;
			if (s_caseArt != null && !s_caseArtBound && game.casePreviewTexture != null)
			{
				s_caseArt.texture = game.casePreviewTexture;
				s_caseArtBound = true;
			}
			game.SetCasePreviewVisible(true);
		}

		public static void BeginRun(ShadowRunPrototype game)
		{
			ApplySelectedMap(game);
			HomePreview(game, false);
		}

		private static bool CanSelectMap(ShadowRunPrototype game)
		{
			return !game.running && !game.paused && !game.showingGameOver && game.menuTargetTab == 1
				&& game.overlay != null && game.overlay.activeInHierarchy
				&& !(game.caseOverlayRoot != null && game.caseOverlayRoot.activeInHierarchy)
				&& !(game.gemShopRoot != null && game.gemShopRoot.activeInHierarchy);
		}

		private static void StepMap(ShadowRunPrototype game, int direction)
		{
			if (!CanSelectMap(game)) return;
			int selected = (SelectedMap() + direction + MapCount) % MapCount;
			PlayerPrefs.SetInt("InfernoDrive.SelectedMap", selected);
			PlayerPrefs.Save();
			RefreshMap(game);
			ApplySelectedMap(game);
		}

		// Click<Index> of the C++: the closed instance delegates.
		private static void Click(ShadowRunPrototype game, int index)
		{
			if (index == 0) StepMap(game, -1);
			else if (index == 1) StepMap(game, 1);
			else if (index == 2) game.SelectMenuTab(0, true);
			else if (index == 3) game.SelectMenuTab(1, true);
			else if (index == 4) game.SelectMenuTab(2, true);
			else if (index == 5) game.ShowCaseOverlay();
			else if (index == 6) game.ShowGemShop();
			else if (index >= 7 && index <= 9)
			{
				Image image = game.dailyGoalCollectImages[index - 7];
				game.ClaimVisibleDailyGoal(index - 7, image.gameObject);
			}
		}

		private static void Connect(ShadowRunPrototype game, Button button, int index)
		{
			button.onClick.AddListener(() => Click(game, index));
		}

		public static Button ButtonOn(Image image)
		{
			image.raycastTarget = true;
			Button button = image.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			// No automatic directional navigation: changing maps requires an explicit click.
			Navigation navigation = Navigation.defaultNavigation;
			navigation.mode = Navigation.Mode.None;
			button.navigation = navigation;
			return button;
		}

		private static Button ActionButton(ShadowRunPrototype game, Transform parent, string name, string title, float x, float y, float width, float height, uint fill, uint rim, int index, int size = 32)
		{
			// one style everywhere: the game's neon frame
			Image image = NeonFrame(game, parent, name, x, y, width, height);
			Button button = ButtonOn(image);
			Label(game, image.transform, "Label", title, 0f, -height / 2f, width - 16f, height - 8f, size, FrameText);
			Connect(game, button, index);
			return button;
		}

		private static Button NeonTab(ShadowRunPrototype game, Transform parent, string name, string title, float x, float y, float width, float height, int size, int index)
		{
			Image image = Block(game, parent, name, x, y, width, height, 0xFFFFFF);
			image.sprite = GlassFrameSprite(game);
			image.type = Image.Type.Sliced;   // sliced glass bevel
			Button button = ButtonOn(image);
			Label(game, image.transform, "Label", title, 0f, -height / 2f, width - 40f, height - 24f, size, FrameText);
			Connect(game, button, index);
			return button;
		}

		public static void Backdrop(ShadowRunPrototype game, Transform parent)
		{
			RectTransform root = Root(parent, "RacingLobbyBackdrop");
			game.menuBackgroundRect = root;
			Place(root, 0f, 0f, 1080f, 1920f, new Vector2(0.5f, 0.5f));
			// Plain black, like the original game screens.
			Block(game, root, "Base", 0f, -960f, 1080f, 1920f, Black);
		}

		// Crystals paid for a claimed daily goal, by goal kind (0 distance, 1 runs,
		// 2 boosts, 3 drift, 4 cars passed, 5 cases opened; see GetDailyGoalValue).
		// ClaimDailyGoal adds this to the profile's crystal count (patched in by
		// tools/apply_lobby_patches.py; the original paid a single crystal).
		private static readonly int[] GoalRewards = { 15, 10, 10, 15, 10, 20 };
		public static int GoalReward(int kind) { return kind >= 0 && kind < 6 ? GoalRewards[kind] : 10; }

		// The three reward chips; UpdateDailyGoalsTimer relabels the rows in place when
		// the goals re-pick at midnight, so the chips are refreshed the same way.
		private static readonly Text[] s_goalReward = new Text[3];

		public static void RefreshGoalRewards(ShadowRunPrototype game)
		{
			RefreshGoalRows(game);   /* [user request 2026-09-21 restyle] progress bars + GO buttons */
			if (s_goalReward[0] == null || game.homeGoalsArtRect == null) return;
			HomeGoalData[] goals = game.PickDailyGoals(3);
			if (goals == null) return;
			for (int i = 0; i < 3 && i < goals.Length; i++)
			{
				HomeGoalData data = goals[i];
				if (data == null || s_goalReward[i] == null) continue;
				Caption(s_goalReward[i], "+" + GoalReward((int)data.Kind));   // Text.text ignores an unchanged string
			}
		}

		public static void Build(ShadowRunPrototype game, Transform parent)
		{
			/* [user request 2026-09-21 restyle] layout to the supplied mockup; drawing helpers in InfernoLobbyStyle.cs */
			RectTransform root = Root(parent, "RacingLobby");
			game.homeContentRoot = root;
			Place(root, 0f, -ContentHeight / 2f, ContentWidth, ContentHeight);

			RectTransform arena = Root(root, "Arena");
			game.homeHeroRoot = arena;
			// The arena starts 112 units down: the gem counter (top-right, 100 tall) sits above it.
			Place(arena, 0f, -445f, 960f, 666f);
			// Map selector: a dark strip with square arrow blocks at both ends; "MAP 4 / 6"
			// small above the name, both left-aligned after the left block.
			Panel(game, arena, "MapSelector", 0f, -75f, 960f, 150f);
			Label(game, arena, "MapCounter", "", -50f, -46f, 560f, 26f, 19, Steel, 3);
			Text mapName = Label(game, arena, "MapName", "", -50f, -92f, 580f, 60f, 46, 0xFFFFFF, 3);
			mapName.fontStyle = FontStyle.BoldAndItalic;
			Block(game, arena, "MapAccent", 0f, -148f, 900f, 3f, Lilac);   // RefreshMap tints it with the map colour
			IconButton(game, arena, "PreviousMap", "chevron-left", -420f, -75f, 104f, 120f, 0);
			IconButton(game, arena, "NextMap", "chevron-right", 420f, -75f, 104f, 120f, 1);
			// Picture frame
			Image preview = Panel(game, arena, "ArenaCard", 0f, -416f, 960f, 500f);
			RectTransform previewRoot = preview.rectTransform;
			game.MakeHome101Artwork(previewRoot);
			Place(game.homeHeroArtRect, 0f, ArtY, ArtWidth, ArtHeight);
			for (int i = 0; i < MapCount; i++)
			{
				RectTransform artRect = Root(previewRoot, "MapArt" + i);
				s_mapArt[i] = artRect.gameObject.AddComponent<RawImage>();
				s_mapArtTried[i] = false;
				s_mapArtSize[i, 0] = s_mapArtSize[i, 1] = 0;
				Place(artRect, 0f, ArtY, ArtWidth, ArtHeight);
				s_mapArt[i].raycastTarget = false;
				artRect.gameObject.SetActive(false);
			}
			Image heroImage = game.homeHeroArtRect.GetComponent<Image>();
			heroImage.preserveAspect = true;
			// Status line over the bottom of the picture ("NEED N MORE CRYSTALS", "MAP UNLOCKED. TAP PLAY."); empty otherwise.
			Label(game, arena, "MapDescription", "", 0f, -640f, 880f, 36f, 24, Pearl);
			{   // lock overlay: the map picture stays visible under a light shade; a plate in the
				// middle carries LOCKED and the crystal price (icon and number in separate slots).
				RectTransform lockRoot = Root(previewRoot, "MapLock");
				s_mapLock = lockRoot;
				Place(lockRoot, 0f, ArtY, ArtWidth, ArtHeight);
				lockRoot.SetAsLastSibling();
				Image shade = Block(game, lockRoot, "LockShade", 0f, -ArtHeight / 2f, ArtWidth, ArtHeight, Black, true);
				shade.color = Hex(Black, 0.42f);
				Image plate = Panel(game, lockRoot, "LockPlate", 0f, -ArtHeight / 2f, 400f, 176f);
				plate.color = new Color(1f, 1f, 1f, 0.94f);
				IconImage(game, lockRoot, "LockIcon", "lock", 0f, -ArtHeight / 2f + 52f, 30f, Steel);
				Text lockTitle = Label(game, lockRoot, "LockTitle", "LOCKED", 0f, -ArtHeight / 2f + 6f, 360f, 56f, 40, Pearl);
				lockTitle.fontStyle = FontStyle.BoldAndItalic;
				// crystal 44 px centred at -50 (spans -72..-28); price text starts at -6 (x 70, 152 wide, left-aligned)
				Image gem = Block(game, lockRoot, "LockGem", -50f, -ArtHeight / 2f - 48f, 44f, 44f, 0xFFFFFF);
				gem.sprite = GemSprite(game);
				gem.preserveAspect = true;
				Label(game, lockRoot, "LockPrice", "", 70f, -ArtHeight / 2f - 48f, 152f, 48f, 34, Lilac, 3);
				lockRoot.gameObject.SetActive(false);
			}

			// PLAY: the slanted magenta plate; MakeHomeDriveShine adds the light sweep that
			// UpdateHomeDriveShine animates. ShowMainMenu relabels it ("P  L  A  Y") and wires
			// StartRun through primaryOverlayButton; PauseRun relabels it CONTINUE.
			Image halo = Block(game, root, "DriveHalo", 0f, -905f, 944f, 250f, 0xFFFFFF);
			s_playHalo = halo;
			halo.sprite = PlayPlateSprite();
			halo.type = Image.Type.Simple;
			halo.color = Hex(Neon, 0.16f);
			Image playFrame = Block(game, root, "Drive", 0f, -905f, 900f, 210f, 0xFFFFFF);
			playFrame.sprite = PlayPlateSprite();
			playFrame.type = Image.Type.Simple;
			RectTransform playRect = playFrame.rectTransform;
			Button driveButton = ButtonOn(playFrame);
			game.primaryOverlayButton = driveButton;
			game.homeDriveArtRect = playRect;
			game.homeDriveHitRect = playRect;
			game.MakeHomeDriveShine(playFrame);
			Transform sweepMask = playRect.Find("DRIVE Gloss Mask");
			if (sweepMask != null)
			{   // clip the sweep to the parallelogram, not the game's rounded rectangle
				Image mask = sweepMask.GetComponent<Image>();
				mask.sprite = PlayPlateSprite();
				mask.type = Image.Type.Simple;
				mask.rectTransform.offsetMax = Vector2.zero;
			}
			game.primaryButtonLabel = Label(game, playRect, "Label", "PLAY", 0f, -105f, 760f, 146f, 100, 0xFFFFFF);
			game.primaryButtonLabel.fontStyle = FontStyle.BoldAndItalic;
			Glow(game.primaryButtonLabel, NeonSoft, 0.75f, 2.5f);

			CaseCard(game, root);
			GoalCard(game, root);
			RefreshMap(game);
		}

		public static void Gems(ShadowRunPrototype game, Transform parent)
		{
			// Share the reference's faceted crystal artwork with daily-goal rewards.
			// Tapping the whole counter opens the gem shop.
			Button button = ActionButton(game, parent, "GemCounter", "", 0f, 0f, 252f, 100f, Ink, Grape, 6, 33);
			RectTransform rect = button.GetComponent<RectTransform>();
			game.gemCounterRect = rect;
			// Stone hard left (56 px at x -88: spans -116..-60, inside the -122 inner edge),
			// number in its own slot on the right (x 40, 130 wide: -25..105).
			Image diamond = Block(game, rect, "Gem", -88f, -50f, 56f, 56f, 0xFFFFFF);
			diamond.sprite = GemSprite(game);
			diamond.preserveAspect = true;
			game.gemAmountText = Label(game, rect, "Amount", game.menuGemCount.ToString(), 40f, -50f, 130f, 66f, 34, Pearl, 4);   // centred in its own slot, right of the stone
		}

		public static void Tabs(ShadowRunPrototype game, Transform parent)
		{
			/* [user request 2026-09-21 restyle] icon over label; the active tab on a lifted plate with a neon underline */
			RectTransform root = Root(parent, "LobbyTabs");
			game.menuTabBarRoot = root;
			Place(root, 0f, 112f, 1000f, 184f, new Vector2(0.5f, 0f));
			Image dock = Block(game, root, "Dock", 0f, -92f, 1400f, 184f, PanelFill);
			dock.color = Hex(PanelFill, 0.98f);
			Block(game, root, "DockRail", 0f, -1f, 1400f, 2f, PanelRim);
			DockTab(game, root, "Garage", "GARAGE", "garage", -320f, 2);
			DockTab(game, root, "Home", "DRIVE", "drive", 0f, 3);
			DockTab(game, root, "Profile", "PROFILE", "profile", 320f, 4);
			DockVisual(game, 1);
		}

		private static void TabVisual(ShadowRunPrototype game, float position)
		{
			DockVisual(game, (int)(position + 0.5f));   /* [user request 2026-09-21 restyle] */
		}

		public static void PositionPages(ShadowRunPrototype game)
		{
			// Pages share one stationary viewport and only the selected page receives input.
			// This also prevents offscreen garage/profile content overlapping map arrows.
			if (game.menuTabsRoot == null || game.overlayRect == null) return;
			game.menuTabsRoot.anchoredPosition = Vector2.zero;
			GameObject[] pages = { game.garageRoot, game.menuMainTabRoot != null ? game.menuMainTabRoot.gameObject : null, game.settingsRoot };
			float width = game.GetMenuViewportWidth();
			for (int i = 0; i < 3; i++)
			{
				if (pages[i] == null) continue;
				game.SetMenuPagePosition(pages[i], 0f, width);
				pages[i].SetActive(i == game.menuTargetTab);
			}
			game.menuActiveTab = game.menuTargetTab;
			game.menuTabStartX = game.menuTabEndX = 0f;
			game.menuTabTweenElapsed = 0.13f;
			TabVisual(game, game.menuActiveTab);
			// The original PositionMenuTabs also re-laid the page headers here; the
			// patcher's centring (CenterHeader) lives in that function, and at header
			// creation time the header being created is not stored yet.
			game.LayoutMenuPageHeaders();
		}

		public static void Layout(ShadowRunPrototype game)
		{
			if (game.homeContentRoot == null || game.overlayRect == null) return;
			Rect bounds = game.overlayRect.rect;
			float width = bounds.width, height = bounds.height;
			if (width <= 0f || height <= 0f) return;
			Rect safe = Screen.safeArea;
			float screenHeight = Screen.height;
			float screenWidth = Screen.width;
			if (screenWidth <= 0f || screenHeight <= 0f) return;
			float top = Math.Max(0f, screenHeight - safe.yMin - safe.height) / screenHeight * height;
			float bottom = Math.Max(0f, safe.yMin) / screenHeight * height;
			float safeWidth = safe.width / screenWidth * width;
			float centerX = (safe.xMin + safe.width / 2f) / screenWidth * width - width / 2f;
			float dockScale = Math.Min(1f, (safeWidth - 32f) / 1000f);
			float available = height - top - bottom - 224f * dockScale - 28f;
			float scale = Math.Min((safeWidth - 48f) / ContentWidth, available / ContentHeight);
			scale = Math.Max(0.1f, scale);
			Place(game.homeContentRoot, centerX, -top - 16f - ContentHeight * scale / 2f, ContentWidth, ContentHeight);
			game.homeContentRoot.localScale = new Vector3(scale, scale, 1f);
			if (game.menuTabBarRoot != null)
			{
				Place(game.menuTabBarRoot, centerX, bottom + 110f * dockScale, 1000f, 184f, new Vector2(0.5f, 0f));
				game.menuTabBarRoot.localScale = new Vector3(dockScale, dockScale, 1f);
			}
			if (game.gemCounterRect != null)
			{
				Place(game.gemCounterRect, centerX + 326f * scale, -top - 16f - 50f * scale, 300f, 100f);   /* [user request 2026-09-21 restyle] 300 wide incl. the + segment; was 252 at +350 */
				game.gemCounterRect.localScale = new Vector3(scale, scale, 1f);
			}
			if (game.menuTitleRoot != null) game.menuTitleRoot.gameObject.SetActive(false);
			HomePreview(game, game.menuTargetTab == 1);
			if (s_playHalo != null) s_playHalo.color = Hex(Neon, 0.12f + 0.10f * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 2.4f)));   // slow breathing glow behind PLAY
		}

		public static void Show(ShadowRunPrototype game)
		{
			StyleGemCounter(game);   /* [user request 2026-09-21 restyle] */
			game.impactShake = 0f;
			RefreshMap(game);
			RefreshGoalRewards(game);
			ApplySelectedMap(game);
			HomePreview(game, true);
			Layout(game);
		}

		// ---- Game over screen (replaces MakeGameOverScreen) -------------------------
		// Same language as the lobby: black backdrop, ink stats card with a lavender
		// rim, and the game's neon-contour buttons. Keeps the fields and click wiring
		// GameOver()/UpdateHud rely on (gameOverRoot, the two value texts, the two
		// buttons -> StartRun / ShowMainMenu).
		private static void Bind(ShadowRunPrototype game, Button button, UnityAction method)
		{
			button.onClick.AddListener(method);
		}

		private static Text Centered(ShadowRunPrototype game, Transform parent, string name, string value, float x, float y, float width, float height, int size, uint color = Pearl, int alignment = 4)
		{
			Text text = Label(game, parent, name, value, x, y, width, height, size, color, alignment);
			Place(text.rectTransform, x, y, width, height, new Vector2(0.5f, 0.5f));
			return text;
		}

		private static Text StatRow(ShadowRunPrototype game, Transform card, string name, string title, float y)
		{
			Label(game, card, name, title, -180f, y, 330f, 40f, 26, Lilac, 3);
			// The game's own value style (33 px, violet, right-aligned), re-placed into the card.
			Text value = game.MakeGameOverValue(card, Vector2.zero);
			Place(value.rectTransform, 210f, y, 300f, 64f);
			return value;
		}

		public static void CaseVisibility(ShadowRunPrototype game, bool visible)
		{
			// The case lives inside menuSafeRoot, below the loss screen in the canvas.
			// Hide the loss screen while browsing a case, then restore its saved result.
			if (game.showingGameOver && game.gameOverRoot != null)
				game.gameOverRoot.gameObject.SetActive(!visible);
		}

		public static void BuildGameOver(ShadowRunPrototype game, Transform parent)
		{
			RectTransform root = Root(parent, "Game Over Screen");
			game.gameOverRoot = root;
			Place(root, 0f, 0f, 2400f, 2400f, new Vector2(0.5f, 0.5f));
			Image shade = Block(game, root, "Shade", 0f, 0f, 2400f, 2400f, Black);
			Place(shade.rectTransform, 0f, 0f, 2400f, 2400f, new Vector2(0.5f, 0.5f));
			shade.raycastTarget = true;   // swallow taps meant for the HUD below

			Centered(game, root, "Title", "YOU LOST", 0f, 560f, 960f, 130f, 96);
			Centered(game, root, "Subtitle", "BETTER LUCK NEXT TIME", 0f, 460f, 900f, 40f, 26, Lilac);
			Image rule = Block(game, root, "TitleRule", 0f, 0f, 520f, 3f, Mulberry);
			Place(rule.rectTransform, 0f, 415f, 520f, 3f, new Vector2(0.5f, 0.5f));

			RectTransform statsRoot = Root(root, "Stats");
			Place(statsRoot, 0f, 230f, 780f, 260f, new Vector2(0.5f, 0.5f));
			Image card = NeonFrame(game, statsRoot, "StatsCard", 0f, -130f, 780f, 260f);
			Transform cardRoot = card.transform;
			game.gameOverDistanceText = StatRow(game, cardRoot, "Distance", "DISTANCE", -70f);
			Block(game, cardRoot, "StatsDivider", 0f, -130f, 720f, 2f, Mulberry);
			game.gameOverBestText = StatRow(game, cardRoot, "Best", "BEST RECORD", -190f);

			Button retry = game.MakeProfileActionButton(root, "TRY AGAIN", new Vector2(0f, -380f), new Vector2(720f, 150f), true, false, out _, true, true);
			game.gameOverRetryButton = retry;
			Bind(game, retry, game.StartRun);
			Button openCase = game.MakeProfileActionButton(root, "OPEN CASE", new Vector2(0f, -190f), new Vector2(720f, 110f), false, false, out _, true, false);
			// Keep the standard case flow and return to this result when it closes.
			Connect(game, openCase, 5);
			Button menu = game.MakeProfileActionButton(root, "BACK TO MENU", new Vector2(0f, -560f), new Vector2(720f, 110f), false, false, out _, true, false);
			game.gameOverMenuButton = menu;
			Bind(game, menu, game.ShowMainMenu);

			root.gameObject.SetActive(false);
		}
	}
}
