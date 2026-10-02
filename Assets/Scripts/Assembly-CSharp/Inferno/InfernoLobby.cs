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
		/* [user request 2026-09-22: lobby mockup] the page is 1032x1668 so that Layout's scale (min((safeWidth - 48) /
		   ContentWidth, (height - dock - 28) / ContentHeight)) is exactly 1 on the 1080x1920 canvas without insets:
		   page units are canvas px, the page's top-left corner sits at canvas (24, 16), and a widget centred at
		   canvas (cx, cy) is placed at (cx - 540, 16 - cy). On other aspect ratios / safe areas the same scaling
		   shrinks and offsets the whole page as before. original: 960f, 1550f */
		public const float ContentWidth = 1032f, ContentHeight = 1668f;   /* [user request 2026-09-21 restyle] taller: gem row + picture + cards; was 1440 */
		public const int MapCount = 6;
		public static readonly string[] MapNames = { "DAYLIGHT CITY", "NEON NIGHT", "RAIN RUN", "STORM CHASE", "FALLEN SKY", "PLANET HORIZON" };
		public static readonly string[] MapDescriptions = { "Golden streets. Open throttle.", "Electric skyline. Endless asphalt.", "Rain-lit roads. Stay sharp.", "Lightning overhead. Keep your line.", "A sky full of falling stars.", "A different world above the road." };
		private static readonly uint[] MapColors = { Blossom, Violet, Lavender, Electric, Magenta, Orchid };
		// Screenshots shipped in Data/Raw/InfernoLobby (bundle-relative). null keeps the hero art.
		// Every map is the same city (the presets only change the weather/lighting), so DAYLIGHT
		// CITY shows the NEON NIGHT shot re-lit as day (tools/relight_map_card.py) — same view.
		/* [user request 2026-09-21: maps] every map has its own picture slot, Resources/InfernoLobby/<slug>.bytes
		   (captured from the map itself by Inferno Drive > Capture Map Thumbnails); a missing file falls back
		   to the tinted borrowed shot below. InfernoMaps keys its sky/road/scenery caches on the same slugs. */
		public static readonly string[] MapSlugs = { "daylight_city", "neon_night", "rain_run", "storm_chase", "fallen_sky", "planet_horizon" };
		private static readonly string[] MapImages = { "Data/Raw/InfernoLobby/daylight_city.png", "Data/Raw/InfernoLobby/neon_night.png", "Data/Raw/InfernoLobby/rain_run.png", "Data/Raw/InfernoLobby/storm_chase.png", "Data/Raw/InfernoLobby/fallen_sky.png", "Data/Raw/InfernoLobby/planet_horizon.png" };
		// original: MapImages = { "Data/Raw/InfernoLobby/daylight_city.png", "Data/Raw/InfernoLobby/neon_night.png", null, null, null, null };
		/* [user request 2026-09-20] maps without their own picture show the closest existing city art,
		   tinted with the map's accent colour, so a locked map still shows a picture of the map. */
		private static readonly string[] MapFallbackImages = { null, null, "Data/Raw/InfernoLobby/neon_night.png", "Data/Raw/InfernoLobby/neon_night.png", "Data/Raw/InfernoLobby/daylight_city.png", "Data/Raw/InfernoLobby/neon_night.png" };
		/* [user request 2026-09-21 restyle] picture 936x440 centred at y -232 inside a 464-tall frame; was 820x376 at -246 */
		/* [user request 2026-09-22: lobby mockup] the picture is the whole 1000x476 rectangle (canvas x 40..1040, y 316..792),
		   straight corners, no frame; ArtY centres it in its 476-tall root. original: 936f, 476f, -250f */
		public const float ArtWidth = 1000f, ArtHeight = 476f, ArtY = -238f;
		private static RectTransform s_mapLock;   // the LOCKED overlay inside the art frame (Build / RefreshMap)

		public static Color Hex(uint value, float alpha = 1f)
		{
			return new Color(((value >> 16) & 255) / 255f, ((value >> 8) & 255) / 255f, (value & 255) / 255f, alpha);
		}

		public static void Paint(Image image, uint color)
		{
			image.color = Hex(color);
		}

		/* [user request 2026-09-26: crystals] */ // The gem counter's slot is 130 px wide: seven digits do not fit and ran under the "+" key, so big
		// balances read as thousands / millions ("500K", "5M", "5.2M"). Up to 99 999 stays plain.
		public static string GemText(int amount)
		{
			if (amount >= 1000000) { float m = amount / 1000000f; return (m >= 10f ? Mathf.Floor(m).ToString("0") : (Mathf.Floor(m * 10f) / 10f).ToString("0.#")) + "M"; }
			if (amount >= 100000) return (amount / 1000).ToString("0") + "K";
			return amount.ToString();
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
			InfernoUi.Remember(game);   /* [user request 2026-09-21: 3D UI] ButtonOn (no game parameter) elevates through the kit */
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

		internal static void Set(ref Color32 px, int r, int g, int b, int a)   /* [user request 2026-09-21: 3D UI] shared with InfernoUi; original: private */
		{
			px.r = (byte)Math.Max(0, Math.Min(255, r));
			px.g = (byte)Math.Max(0, Math.Min(255, g));
			px.b = (byte)Math.Max(0, Math.Min(255, b));
			px.a = (byte)Math.Max(0, Math.Min(255, a));
		}

		// Signed distance (px) to the octagon inset by `inset` from the texture edge; positive inside.
		internal static float Octagon(int x, int y, int inset, int chamfer)   /* [user request 2026-09-21: 3D UI] original: private */
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

		/* [user request 2026-09-21: 3D UI] The three lobby sprites now come from the widget kit
		   (InfernoUi): the frame is the mockup's cut-corner panel (large chamfer top-left /
		   bottom-right, small top-right / bottom-left, violet contour, glow, lit top edge), so every
		   NeonFrame / NeonPlate / GlassFrame user switches at once. The old octagon shaders above
		   stay for reference. original: s_glassFrame = SlicedSprite(GlassPixel, GlassBorder) */
		private static Sprite GlassFrameSprite(ShadowRunPrototype game)
		{
			InfernoUi.Remember(game);
			if (s_glassFrame == null) s_glassFrame = InfernoUi.BevelGlassSprite();
			return s_glassFrame;
		}

		// original: if (s_neonFrame == null) s_neonFrame = SlicedSprite(FramePixel);
		public static Sprite NeonFrameSprite(ShadowRunPrototype game)
		{
			InfernoUi.Remember(game);
			if (s_neonFrame == null) s_neonFrame = InfernoUi.ChamferFrameSprite(InfernoUi.Chamfer.Large);
			return s_neonFrame;
		}

		// The frame cut to suit a rect: buttons and small widgets get the medium / small chamfers.
		public static Sprite NeonFrameSprite(ShadowRunPrototype game, float width, float height)
		{
			InfernoUi.Remember(game);
			return InfernoUi.ChamferFrameSprite(InfernoUi.ChamferFor(width, height));
		}

		private static Sprite s_neonPlate;

		// original: if (s_neonPlate == null) s_neonPlate = SlicedSprite(PlatePixel);
		private static Sprite NeonPlateSprite(ShadowRunPrototype game)
		{
			InfernoUi.Remember(game);
			if (s_neonPlate == null) s_neonPlate = InfernoUi.BevelPlateSprite();
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
			Sprite sprite = NeonFrameSprite(game, width, height);   /* [user request 2026-09-21: 3D UI] chamfer picked from the size; original: NeonFrameSprite(game) */
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
						/* [user request 2026-09-21: maps] the map's own capture first, the tinted borrowed shot when it is missing */
						bool borrowed = false;
						Texture2D texture = LoadTexture(file);
						if (texture == null && MapFallbackImages[i] != null) { texture = LoadTexture(MapFallbackImages[i]); borrowed = true; }
						if (texture != null)
						{
							art.texture = texture;
							// a borrowed picture is tinted towards the map's accent so each map reads differently
							art.color = !borrowed ? Color.white : Color.Lerp(Color.white, Hex(MapColors[i]), 0.45f);
							// original: art.color = MapImages[i] != null ? Color.white : Color.Lerp(Color.white, Hex(MapColors[i]), 0.45f);
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
			if (game.gemAmountText != null) Caption(game.gemAmountText, GemText(amount));   /* [user request 2026-09-26: crystals] */
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
			if (count != null) Caption(count.GetComponent<Text>(), InfernoUi.Spaced(counter));   /* [user request 2026-09-22: lobby mockup] letter-spaced "M A P   4   /   6"; original: Caption(..., counter) */
			/* [user request 2026-09-22: lobby mockup] no accent bar under the map name any more.
			   original: Transform accent = Find(game.homeHeroRoot, "MapAccent");
			   original: if (accent != null) Paint(accent.GetComponent<Image>(), MapColors[selected]); */
			ShowMapArt(game, selected);
			bool locked = !MapUnlocked(selected);
			Transform lockRoot = s_mapLock;   /* [user request 2026-09-20] lives inside the (renamed) art frame; kept by reference */
			if (lockRoot != null) lockRoot.gameObject.SetActive(locked);
			if (lockRoot != null && locked)
			{
				Transform p = Find(lockRoot, "LockPrice");
				if (p != null)
				{
					Text priceText = p.GetComponent<Text>();
					Caption(priceText, MapPrices[selected].ToString());
					/* [user request 2026-09-22: lobby mockup] the crystal (44 px) + 22 px gap + price run is centred on the
					   plate as one group, whatever the price's width (a fixed layout left it ~10 px to the left). */
					Transform g = Find(lockRoot, "LockGem");
					RectTransform gemRect = g as RectTransform, priceRect = p as RectTransform;
					if (gemRect != null && priceRect != null)
					{
						float textWidth = Mathf.Max(30f, priceText.preferredWidth);
						float left = -(44f + 22f + textWidth) / 2f;
						gemRect.anchoredPosition = new Vector2(left + 22f, gemRect.anchoredPosition.y);
						priceRect.anchoredPosition = new Vector2(left + 66f + priceRect.sizeDelta.x / 2f, priceRect.anchoredPosition.y);   // left-aligned label: its centre is text start + half width
					}
				}
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
			/* [user request 2026-09-21: maps] the map's own environment (sky, road, scenery, particles) */
			InfernoMaps.Apply(game, SelectedMap());
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

		public static Button ButtonOn(Image image, bool elevate = true)
		{
			image.raycastTarget = true;
			Button button = image.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			// No automatic directional navigation: changing maps requires an explicit click.
			Navigation navigation = Navigation.defaultNavigation;
			navigation.mode = Navigation.Mode.None;
			button.navigation = navigation;
			/* [user request 2026-09-21: 3D UI] every lobby button gets the kit's drop shadow and
			   press-down feedback (flat art is re-skinned with the chamfered frame; kit sprites,
			   the PLAY plate and invisible hit areas are kept as they are). */
			if (elevate) InfernoUi.Elevate(null, image, InfernoUi.Style.Frame);
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

			/* [user request 2026-09-22: lobby mockup] every position below is the LOBBY-SPEC canvas position mapped into
			   page units (centre (cx - 540, 16 - cy), see ContentWidth). The arena holds the selector bar, the picture
			   and the status line (canvas y 143..843); inside it a child centred at canvas (cx, cy) sits at (cx - 540, 143 - cy). */
			RectTransform arena = Root(root, "Arena");
			game.homeHeroRoot = arena;
			// The arena starts 127 units down: the gem counter (top-right, canvas y 25..109) sits above it.
			Place(arena, 0f, -477f, 1000f, 700f);   // original: 0f, -445f, 960f, 666f
			// Map selector: a dark steel strip (canvas x 40..1040, y 143..304; cuts TL/BR 40, TR/BL 14) with square
			// arrow blocks at both ends; "MAP 4 / 6" small above the name, both left-aligned at x 224.
			SteelBox(game, arena, "MapSelector", 0f, -80.5f, 1000f, 161f, 40, 14, 40, 14);   // original: Panel(game, arena, "MapSelector", 0f, -75f, 960f, 150f)
			SpacedLabel(game, arena, "MapCounter", "", -66f, -50f, 500f, 26f, 17, MockRimLit, 3);   // x 224.., centre y 193; original: Label(..., -50f, -46f, 560f, 26f, 19, Steel, 3)
			Text mapName = Label(game, arena, "MapName", "", 17f, -107f, 666f, 60f, 48, 0xFFFFFF, 3);   // x 224..890, centre y 250; original: -50f, -92f, 580f, 60f, 46
			mapName.fontStyle = FontStyle.BoldAndItalic;
			mapName.resizeTextMinSize = 34;   // PLANET HORIZON fits before the right arrow
			// original: Block(game, arena, "MapAccent", 0f, -148f, 900f, 3f, Lilac);   // RefreshMap tints it with the map colour — no accent bar in the mockup
			IconButton(game, arena, "PreviousMap", "chevron-left", -425f, -80f, 110f, 110f, 0, 22, 0, 12, 0);   // x 60..170, y 168..278, cuts TL 22 / BR 12; original: -420f, -75f, 104f, 120f, 0
			/* [user request 2026-09-27: lobby corners] the right arrow's cuts are a 180-degree ROTATION of the left arrow's (TL 12 /
			   BR 22), not a mirror: the bar's cuts are TL/BR 40, TR/BL 14, so the mirrored version (TR 22 / BL 12) left the
			   arrow's square BR corner 4.2 px from the bar's BR diagonal (2 px of fill between two rims, which merged on a
			   phone). Now the arrow's BR 22 diagonal runs parallel to the bar's BR 40 cut, 19.8 px away (17.8 px of fill)
			   by geometry vs the left arrow's 19.1 px; as rasterised (rim to rim, capture claude-131) the right side reads
			   19.0-19.1 px and the left 17.0 px, because the bar's TL rim lands ~2 px inside its ideal edge and its bottom rim
			   is 1 px thick vs 2 px on top (top gap 23 / bottom 25 px on a centred arrow). The left arrow's TL chamfer is now
			   the selector's tightest point; that is not a regression, it was 17.0 px before too. round 1: 1, 0, 22, 0, 12 */
			IconButton(game, arena, "NextMap", "chevron-right", 425f, -80f, 110f, 110f, 1, 12, 0, 22, 0);      // x 910..1020, cuts TL 12 / BR 22 (BR follows the bar's BR 40 cut, 19.8 px parallel); original: 420f, -75f, 104f, 120f, 1
			// Picture: a plain straight-cornered rectangle (canvas x 40..1040, y 316..792), no frame, rim or accent bar
			Image preview = Block(game, arena, "ArenaCard", 0f, -411f, ArtWidth, ArtHeight, MockFill);   // original: Panel(game, arena, "ArenaCard", 0f, -416f, 960f, 500f)
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
			// Status line between the picture and PLAY ("NEED N MORE CRYSTALS", "MAP UNLOCKED. TAP PLAY."); empty otherwise.
			/* [user request 2026-09-22: lobby mockup] small grey-lilac line centred at canvas y 826 (rect 811..841: 19 px
			   under the picture, 20 px above PLAY). original: Label(..., 0f, -640f, 880f, 36f, 24, Pearl) */
			Text status = Label(game, arena, "MapDescription", "", 0f, -683f, 880f, 30f, 20, MockGreyLilac);
			status.resizeTextMinSize = 18;
			{   // lock overlay: the map picture stays visible under a light shade; a plate in the
				// middle carries LOCKED and the crystal price (icon and number in separate slots).
				RectTransform lockRoot = Root(previewRoot, "MapLock");
				s_mapLock = lockRoot;
				Place(lockRoot, 0f, ArtY, ArtWidth, ArtHeight);
				lockRoot.SetAsLastSibling();
				/* [user request 2026-09-22: lobby mockup] straight-cornered shade, a steel plate (cuts TL/BR 18, TR/BL 6) and
				   grey-lilac / white text. original: Block(..., Black, true) (rounded); Panel(game, lockRoot, "LockPlate", ...);
				   LockIcon Steel; LockTitle Pearl; LockPrice Lilac */
				Image shade = Block(game, lockRoot, "LockShade", 0f, -ArtHeight / 2f, ArtWidth, ArtHeight, Black);
				shade.color = Hex(Black, 0.42f);
				// opaque plate (nothing of the scenery bleeds through); the 34 px padlock (~32x27 visible) centred 56 above
				// the picture's middle (canvas y ~498, 22 px above the LOCKED text). original: plate.color = new
				// Color(1f, 1f, 1f, 0.94f); IconImage(..., "lock", 0f, -ArtHeight / 2f + 52f, 30f, MockGreyLilac)
				SteelBox(game, lockRoot, "LockPlate", 0f, -ArtHeight / 2f, 400f, 176f, 18, 0, 18, 0);   /* [user request 2026-09-27: lobby corners] TR/BL squared like the two cards (a 6 px nick on the 2 px rim reads as a rounded corner); round 1: 18, 6, 18, 6 */
				IconImage(game, lockRoot, "LockIcon", "lock", 0f, -ArtHeight / 2f + 56f, 34f, MockGreyLilac);
				Text lockTitle = Label(game, lockRoot, "LockTitle", "LOCKED", 0f, -ArtHeight / 2f + 6f, 360f, 56f, 40, 0xFFFFFF);
				lockTitle.fontStyle = FontStyle.BoldAndItalic;
				// crystal 44 px centred at -50 (spans -72..-28); price text starts at -6 (x 70, 152 wide, left-aligned)
				Image gem = Block(game, lockRoot, "LockGem", -50f, -ArtHeight / 2f - 48f, 44f, 44f, 0xFFFFFF);
				gem.sprite = GemSprite(game);
				gem.preserveAspect = true;
				Label(game, lockRoot, "LockPrice", "", 70f, -ArtHeight / 2f - 48f, 152f, 48f, 34, MockGreyLilac, 3);
				lockRoot.gameObject.SetActive(false);
			}

			// PLAY: the magenta plate; MakeHomeDriveShine adds the light sweep that
			// UpdateHomeDriveShine animates. ShowMainMenu relabels it ("P  L  A  Y") and wires
			// StartRun through primaryOverlayButton; PauseRun relabels it CONTINUE.
			/* [user request 2026-09-22: lobby mockup] the 960x212 octagon (cuts TL 92x103, TR 48x46, BR 60x63, BL 32x46)
			   at canvas x 63..1023, y 861..1073, with its soft glow sprite (28 px larger all round) behind it.
			   original: Block(game, root, "DriveHalo", 0f, -905f, 944f, 250f, 0xFFFFFF) with PlayPlateSprite() tinted
			   Hex(Neon, 0.16f); Block(game, root, "Drive", 0f, -905f, 900f, 210f, 0xFFFFFF) */
			Image halo = Block(game, root, "DriveHalo", 2f, -951f, PlayW + 2 * PlayGlowPad, PlayH + 2 * PlayGlowPad, 0xFFFFFF);
			s_playHalo = halo;
			halo.sprite = PlayGlowSprite();
			halo.type = Image.Type.Simple;
			halo.color = Color.white;
			Image playFrame = Block(game, root, "Drive", 2f, -951f, PlayW, PlayH, 0xFFFFFF);
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
			/* [user request 2026-09-22: lobby mockup] ~74 px bold italic, centred on the plate; "U N L O C K   150" fits the
			   760 px slot at 74 px, the best-fit floor (56) only guards longer relabels. original: 0f, -105f, 760f, 146f, 100 */
			game.primaryButtonLabel = Label(game, playRect, "Label", "PLAY", 0f, -PlayH / 2f, 760f, 146f, 74, 0xFFFFFF);
			game.primaryButtonLabel.resizeTextMinSize = 56;
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
			/* [user request 2026-09-22: lobby mockup] built in its final look: a 283x84 steel box (cuts TL 10 / TR 14 / BR 14 /
			   BL 10; Layout puts it at canvas x 767..1050, y 25..109) holding the 44 px crystal centred at x 810, the count
			   (bold italic 40 px, centred around x 906) and a separate 66x66 darker "+" key (cuts TL 6 / BR 14) at x 981..1047
			   that also opens the shop. StyleGemCounter used to re-skin this later.
			   original: Button button = ActionButton(game, parent, "GemCounter", "", 0f, 0f, 252f, 100f, Ink, Grape, 6, 33);
			   original: Image diamond = Block(game, rect, "Gem", -76f, -50f, 48f, 48f, 0xFFFFFF);   // [user request 2026-09-20: pause/UI fixes]
			   original: game.gemAmountText = Label(game, rect, "Amount", game.menuGemCount.ToString(), 40f, -50f, 130f, 66f, 34, Pearl, 4); */
			Image box = SteelBox(game, parent, "GemCounter", 0f, 0f, 283f, 84f, 10, 14, 14, 10);
			Button button = ButtonOn(box);
			Connect(game, button, 6);   // ShowGemShop
			RectTransform rect = box.rectTransform;
			game.gemCounterRect = rect;
			Image diamond = Block(game, rect, "Gem", -98.5f, -42f, 44f, 44f, 0xFFFFFF);
			diamond.sprite = GemSprite(game);
			diamond.preserveAspect = true;
			game.gemAmountText = Label(game, rect, "Amount", GemText(game.menuGemCount), -2f, -42f, 130f, 50f, 40, 0xFFFFFF, 4);   /* [user request 2026-09-26: crystals] */   // centred in its own slot, right of the stone
			game.gemAmountText.fontStyle = FontStyle.BoldAndItalic;
			/* [user request 2026-09-26: no box around the "+"] the key's own steel plate is gone; its 66x66 rect
			   stays as an invisible hit area so tapping the "+" still opens the shop (ButtonOn without the kit's
			   elevation, which would re-skin flat art with a chamfered frame).
			   original: Image plus = SteelBox(game, rect, "Plus", 105.5f, -42f, 66f, 66f, 6, 0, 14, 0, Look.Key); */
			Image plus = Block(game, rect, "Plus", 105.5f, -42f, 66f, 66f, 0xFFFFFF);
			plus.color = new Color(1f, 1f, 1f, 0f);
			Button key = ButtonOn(plus, false);
			Connect(game, key, 6);   // ShowGemShop
			IconImage(game, plus.transform, "Icon", "plus", 0f, -33f, 34f, 0xFFFFFF).raycastTarget = false;
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
			/* [user request 2026-09-21: 3D UI] rhomboid tabs (slanted parallelograms side by side, 330
			   wide, 52 px slant, 10 px gap) — spacing 288 keeps the slanted sides parallel with a
			   constant gap; original: 320 apart */
			DockTab(game, root, "Garage", "GARAGE", "garage", -288f, 2);
			DockTab(game, root, "Home", "DRIVE", "drive", 0f, 3);
			DockTab(game, root, "Profile", "PROFILE", "profile", 288f, 4);
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
				/* [user request 2026-09-22: lobby mockup] 283x84 box centred at canvas (908.5, 67) = x 767..1050, y 25..109, scaled
				   with the page. original: Place(..., centerX + 326f * scale, -top - 16f - 50f * scale, 300f, 100f) — [user request
				   2026-09-21 restyle] 300 wide incl. the + segment; was 252 at +350 */
				Place(game.gemCounterRect, centerX + 368.5f * scale, -top - 67f * scale, 283f, 84f);
				game.gemCounterRect.localScale = new Vector3(scale, scale, 1f);
			}
			if (game.menuTitleRoot != null) game.menuTitleRoot.gameObject.SetActive(false);
			HomePreview(game, game.menuTargetTab == 1);
			// slow breathing of the soft glow behind PLAY (the glow sprite carries its own magenta and 0.35 peak alpha)
			if (s_playHalo != null) s_playHalo.color = new Color(1f, 1f, 1f, 0.8f + 0.2f * (0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 2.4f)));   /* [user request 2026-09-22: lobby mockup] original: Hex(Neon, 0.12f + 0.10f * (...)) */
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
			/* [user request 2026-09-29: loss case box] "add the case box for the user to open when the user losts":
			   the flat OPEN CASE plate is now a real case box - the lobby "Cases" card at 280x380, built in
			   Inferno/InfernoGameOverCase.cs - dropped into the band the plate leaves free (the stats card's
			   bottom edge at y 100 down to the TRY AGAIN glow at y -294; the box is y -287..93, 7 px of air at
			   each end), so nothing else on this screen moves. It keeps the plate's GameObject name and its
			   Connect index 5, so everything that drives this screen by name still works.
			   original: Button openCase = game.MakeProfileActionButton(root, "OPEN CASE", new Vector2(0f, -190f), new Vector2(720f, 110f), false, false, out _, true, false);
			   original: // Keep the standard case flow and return to this result when it closes.
			   original: Connect(game, openCase, 5); */
			BuildCaseBox(game, root);
			Button menu = game.MakeProfileActionButton(root, "BACK TO MENU", new Vector2(0f, -560f), new Vector2(720f, 110f), false, false, out _, true, false);
			game.gameOverMenuButton = menu;
			Bind(game, menu, game.ShowMainMenu);

			/* [user request 2026-09-21: 3D UI] the gem counter (menuSafeRoot) is hidden while the loss
			   screen is up and comes back with the menu; GameOver() has no lobby hook, so the root's
			   own show / hide does it. */
			UiScreenLink link = root.gameObject.AddComponent<UiScreenLink>();
			link.onShown = () => HideGemCounter(game, ref s_gameOverGems);
			link.onHidden = () => RestoreGemCounter(game, ref s_gameOverGems);

			root.gameObject.SetActive(false);
		}

		// ---- gem counter visibility under the pause / loss screens ---------------------------
		// Each screen remembers whether the counter was showing when it hid it (a nullable so a
		// screen that never hid anything restores nothing).
		private static bool? s_gameOverGems, s_pauseGems, s_pauseDetails;

		private static void HideGemCounter(ShadowRunPrototype game, ref bool? saved)
		{
			if (saved.HasValue || game == null || game.gemCounterRect == null) return;
			saved = game.gemCounterRect.gameObject.activeSelf;
			game.gemCounterRect.gameObject.SetActive(false);
		}

		private static void RestoreGemCounter(ShadowRunPrototype game, ref bool? saved)
		{
			if (!saved.HasValue) return;
			bool wasActive = saved.Value;
			saved = null;
			if (game != null && game.gemCounterRect != null) game.gemCounterRect.gameObject.SetActive(wasActive);
		}

		// ---- Pause screen (replaces MakePauseActions) -------------------------------
		/* [user request 2026-09-20: pause/UI fixes] */
		// The loss screen's language: black backdrop, PAUSED / subtitle / rule, a neon
		// stats card with the run so far, the two profile-style action buttons, and the
		// sound toggle re-cut as a small neon frame around the game's speaker icon.
		// Keeps every field and wiring the rest of the game relies on: pauseActionsRoot
		// (shown/hidden by PauseRun / ResumeRun / ShowMainMenu / StartRun / GameOver),
		// pauseContinueButton -> ResumeRun, pauseMenuButton -> ShowMainMenu,
		// pauseSoundButton -> ToggleGameSound, pauseSoundIcon (ApplyGameSoundState swaps
		// its sprite).
		private static Text s_pauseDistance, s_pauseBest;

		public static void BuildPause(ShadowRunPrototype game, Transform parent)
		{
			RectTransform root = Root(parent, "Pause Actions");
			game.pauseActionsRoot = root.gameObject;
			// Stretched over the safe root like the original so the toggle can sit in the corner.
			root.anchorMin = Vector2.zero;
			root.anchorMax = Vector2.one;
			root.offsetMin = Vector2.zero;
			root.offsetMax = Vector2.zero;
			Image shade = Block(game, root, "Shade", 0f, 0f, 2400f, 2400f, Black);
			Place(shade.rectTransform, 0f, 0f, 2400f, 2400f, new Vector2(0.5f, 0.5f));
			shade.raycastTarget = true;   // swallow taps meant for the gem counter / HUD below
			/* [user request 2026-09-21: transparent pause] the frozen city stays visible through a
			   dark translucent shade; a gradient band at the top keeps the title legible.
			   original: opaque Black */
			shade.color = new Color(0.03f, 0.02f, 0.07f, 0.62f);
			Image band = Block(game, root, "Title Band", 0f, -210f, 1080f, 420f, 0xFFFFFF);
			Place(band.rectTransform, 0f, -210f, 1080f, 420f, new Vector2(0.5f, 1f));
			band.sprite = game.heroBottomFadeSprite;
			band.type = Image.Type.Simple;
			band.color = new Color(1f, 1f, 1f, 0.92f);          // the fade sprite carries its own 0.6 peak alpha
			band.rectTransform.localScale = new Vector3(1f, -1f, 1f);   // flipped: dark at the top, fading down

			Centered(game, root, "Title", "PAUSED", 0f, 560f, 960f, 130f, 96);
			Centered(game, root, "Subtitle", "THE CITY IS STILL MOVING", 0f, 460f, 900f, 40f, 26, Lilac);
			Image rule = Block(game, root, "TitleRule", 0f, 0f, 520f, 3f, Mulberry);
			Place(rule.rectTransform, 0f, 415f, 520f, 3f, new Vector2(0.5f, 0.5f));

			RectTransform statsRoot = Root(root, "Stats");
			Place(statsRoot, 0f, 230f, 780f, 260f, new Vector2(0.5f, 0.5f));
			Image card = NeonFrame(game, statsRoot, "StatsCard", 0f, -130f, 780f, 260f);
			Transform cardRoot = card.transform;
			s_pauseDistance = StatRow(game, cardRoot, "Distance", "DISTANCE", -70f);
			Block(game, cardRoot, "StatsDivider", 0f, -130f, 720f, 2f, Mulberry);
			s_pauseBest = StatRow(game, cardRoot, "Best", "BEST RECORD", -190f);

			// The buttons keep the shipped object names (the smoke tests and any
			// lookups find "Pause Continue" / "Pause Back To Menu" by name).
			Button resume = game.MakeProfileActionButton(root, "CONTINUE DRIVING", new Vector2(0f, -200f), new Vector2(720f, 150f), true, false, out _, true, true);
			resume.gameObject.name = "Pause Continue";
			game.pauseContinueButton = resume;
			Bind(game, resume, game.ResumeRun);
			Button menu = game.MakeProfileActionButton(root, "BACK TO MENU", new Vector2(0f, -380f), new Vector2(720f, 110f), false, false, out _, true, false);
			menu.gameObject.name = "Pause Back To Menu";
			game.pauseMenuButton = menu;
			Bind(game, menu, game.ShowMainMenu);

			// Sound toggle: top-left as before (the old 82 px pink pill at 82,-92), now a
			// 104 px neon frame (centre 132,-140: spans 80..184 / -88..-192) with the
			// game's own speaker icon (48 px) inside the 72 px glass, clear of the chamfers.
			Image toggleFrame = NeonFrame(game, root, "Pause Sound Toggle", 132f, -140f, 104f, 104f);
			Place(toggleFrame.rectTransform, 132f, -140f, 104f, 104f, new Vector2(0f, 1f));
			Button toggle = ButtonOn(toggleFrame);
			game.pauseSoundButton = toggle;
			game.pauseSoundIcon = game.MakeTabIcon(toggleFrame.transform, "Pause Sound Toggle Icon", game.gameSoundEnabled ? "звук есть" : "звука нет", new Vector2(48f, 48f), Vector2.zero);
			Bind(game, toggle, game.ToggleGameSound);

			/* [user request 2026-09-21: transparent pause] whatever hides the pause root (ResumeRun,
			   ShowMainMenu, StartRun, GameOver) also restores what ShowPause hid. */
			UiScreenLink link = root.gameObject.AddComponent<UiScreenLink>();
			link.onHidden = () => HidePause(game);

			root.gameObject.SetActive(false);
		}

		// Called from PauseRun in place of the old overlay title/tag lines: the shared
		// white "P  A  U  S  E  D" / tag texts are blanked (PauseRun raises them above
		// the pause root, so they would draw over the backdrop) and the stats card takes
		// the run so far in the loss screen's format.
		public static void ShowPause(ShadowRunPrototype game)
		{
			if (game.overlayTitle != null) Caption(game.overlayTitle, "");
			if (game.overlayTag != null) Caption(game.overlayTag, "");
			if (s_pauseDistance != null) Caption(s_pauseDistance, game.FormatGameOverDistance(game.distance));
			if (s_pauseBest != null) Caption(s_pauseBest, game.FormatGameOverDistance(game.best));
			/* [user request 2026-09-21: transparent pause] the shade is see-through now, so the menu
			   details line (overlayStats) and the gem counter, which live in menuSafeRoot under the
			   pause root, are hidden for the pause and put back by HidePause. */
			if (!s_pauseDetails.HasValue && game.menuDetails != null)
			{
				s_pauseDetails = game.menuDetails.activeSelf;
				game.menuDetails.SetActive(false);
			}
			HideGemCounter(game, ref s_pauseGems);
		}

		// Restores what ShowPause hid. Called by ResumeRun / ShowMainMenu (Boot.cs) and by the
		// pause root's own hide, so it is safe to call more than once.
		public static void HidePause(ShadowRunPrototype game)
		{
			if (s_pauseDetails.HasValue)
			{
				bool wasActive = s_pauseDetails.Value;
				s_pauseDetails = null;
				if (game != null && game.menuDetails != null) game.menuDetails.SetActive(wasActive);
			}
			RestoreGemCounter(game, ref s_pauseGems);
		}
	}
}
