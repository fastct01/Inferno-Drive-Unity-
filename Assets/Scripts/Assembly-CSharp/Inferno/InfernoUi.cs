using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// [user request 2026-09-21: 3D UI] The widget kit behind every menu: procedural sprites in the
// language of the user's mockups (panels with asymmetric cut-off corners — a large cut top-left
// and bottom-right, a small one top-right and bottom-left — a thin violet contour with a soft
// glow and a lit top edge; rhomboid dock tabs; skewed rhombus slider knobs on glowing tracks; an
// ON/OFF pill toggle), the press-down feedback component, and the factories the other screens
// build with. Everything is drawn in code (no art survived the rip) and cached per shape.
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoUi
	{
		public enum Style { Frame, Plate, Danger, Tab, Ghost }
		public enum Chamfer { Large, Medium, Small }   // 50/18, 28/10, 14/6 px corner cuts (TL+BR large, TR+BL small)

		// Lobby palette (InfernoLobby / InfernoLobbyStyle) so the kit matches the untouched screens.
		private const uint Steel = 0x9A94A8, Smoke = 0x2A2632, Neon = 0xE33CFF, NeonSoft = 0xFF7CF5;
		private const uint Electric = 0x9433FF, Violet = 0xB840FF, Lilac = 0xAD8ADB, Pearl = 0xE0D6F0, FrameText = 0xFAE6FF;

		// The game instance the last factory ran for: InfernoLobby.ButtonOn has no game parameter,
		// and Elevate needs it to recognise the game's own flat sprites (roundedGlassSprite).
		private static ShadowRunPrototype s_game;
		public static void Remember(ShadowRunPrototype game) { if (game != null) s_game = game; }
		private static ShadowRunPrototype Game(ShadowRunPrototype game) { Remember(game); return game != null ? game : s_game; }

		// ================================================================================
		// Sprite generation
		// ================================================================================
		private delegate void PixelShader(ref Color32 px, float x, float y);   // x, y = pixel centres, y up

		private static readonly HashSet<Sprite> s_kit = new HashSet<Sprite>();                    // every sprite this kit made
		private static readonly Dictionary<Sprite, float> s_rhomboid = new Dictionary<Sprite, float>();   // tab sprites -> slant (px)
		private static readonly HashSet<Sprite> s_recessed = new HashSet<Sprite>();               // tracks: no drop shadow

		public static bool IsKitSprite(Sprite sprite) { return sprite != null && s_kit.Contains(sprite); }
		public static bool IsRhomboid(Sprite sprite) { return sprite != null && s_rhomboid.ContainsKey(sprite); }
		// Horizontal inset (px) of the sprite's drawn shape from its rect: the slant of a rhomboid
		// tab, 0 for everything else. The layout audit compares the drawn shapes, not the rects.
		public static float VisualInset(Sprite sprite)
		{
			float slant;
			return sprite != null && s_rhomboid.TryGetValue(sprite, out slant) ? slant : 0f;
		}

		private static Sprite Generate(int width, int height, PixelShader shader, Vector4 border)
		{
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			Color32[] px = new Color32[width * height];
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					shader(ref px[y * width + x], x + 0.5f, y + 0.5f);
			texture.SetPixels32(px);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;
			Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
			s_kit.Add(sprite);
			return sprite;
		}

		private static Sprite Generate(int size, PixelShader shader, int border)
		{
			return Generate(size, size, shader, new Vector4(border, border, border, border));
		}

		private static float Sat(float v) { return v < 0f ? 0f : v > 1f ? 1f : v; }
		private static float Lerp(float a, float b, float t) { return a + (b - a) * t; }
		private static Vector3 Rgb(uint hex) { return new Vector3((hex >> 16) & 255, (hex >> 8) & 255, hex & 255); }
		private static Vector3 Rgb(float r, float g, float b) { return new Vector3(r, g, b); }
		private static Vector3 Mix(Vector3 a, Vector3 b, float t) { return a + (b - a) * Sat(t); }

		private static void Set(ref Color32 px, Vector3 rgb, float alpha)
		{
			px.r = (byte)Math.Max(0f, Math.Min(255f, rgb.x));
			px.g = (byte)Math.Max(0f, Math.Min(255f, rgb.y));
			px.b = (byte)Math.Max(0f, Math.Min(255f, rgb.z));
			px.a = (byte)Math.Max(0f, Math.Min(255f, alpha));
		}

		// ---- chamfered ("cut-corner") shapes ---------------------------------------------
		private struct Spec { public int tex, border, glow, big, small; public float contour; }

		// Per chamfer size: texture, 9-slice border (glow + big cut + contour), glow width, the two
		// cut sizes and the contour width. The 36 px middle is what stretches.
		private static readonly Spec[] Specs =
		{
			new Spec { tex = 160, border = 62, glow = 8, big = 50, small = 18, contour = 3f },
			new Spec { tex = 112, border = 38, glow = 7, big = 28, small = 10, contour = 2.5f },
			new Spec { tex = 80, border = 22, glow = 6, big = 14, small = 6, contour = 2f },
		};

		// Signed distance (px, positive inside) to the square inset by `inset` from the texture
		// edge with the four corners cut: big top-left / bottom-right, small top-right / bottom-left.
		private static float Cut(float x, float y, float inset, float big, float small, int tex)
		{
			float l = x - inset, r = tex - inset - x, b = y - inset, t = tex - inset - y;
			float d = Math.Min(Math.Min(l, r), Math.Min(b, t));
			d = Math.Min(d, (l + t - big) * 0.70711f);     // top-left cut
			d = Math.Min(d, (r + b - big) * 0.70711f);     // bottom-right cut
			d = Math.Min(d, (r + t - small) * 0.70711f);   // top-right cut
			d = Math.Min(d, (l + b - small) * 0.70711f);   // bottom-left cut
			return d;
		}

		private enum Kind { Frame, Plate, Lift, Outline, Danger, Core }

		// One shader for the whole chamfered family: outer glow, contour (lit at the top), a dark
		// seam, an inner shadow, the body gradient with a lit top edge and a faint sheen.
		private static void ChamferPixel(ref Color32 px, float x, float y, Spec s, Kind kind)
		{
			float inset = kind == Kind.Core ? 0.5f : s.glow;
			float d = Cut(x, y, inset, s.big, s.small, s.tex);
			float v = y / s.tex;                                       // 0 bottom .. 1 top
			float edge = Sat(d + 0.5f);                                // coverage of the shape
			float g = Sat(1f + d / s.glow);
			Vector3 glowRgb = Rgb(184, 64, 255);
			float glowA = 110f * g * g;
			if (kind == Kind.Danger) { glowRgb = Rgb(255, 40, 100); glowA = 120f * g * g; }
			else if (kind == Kind.Lift || kind == Kind.Outline || kind == Kind.Core) glowA = 0f;
			if (edge <= 0f) { Set(ref px, glowRgb, glowA); return; }

			float dt = (s.tex - inset) - y;                            // depth below the top edge of the shape
			float c = s.contour;
			Vector3 body, contour, lit;
			float bodyA = 250f, litK = 0.55f, shadowK = 0.30f, sheenK = 0.06f;
			switch (kind)
			{
				case Kind.Plate:
				case Kind.Core:
					body = Mix(Rgb(134, 38, 230), Rgb(190, 112, 255), v);
					contour = Mix(Rgb(200, 120, 255), Rgb(240, 200, 255), v);
					lit = Rgb(255, 236, 255); litK = 0.5f; shadowK = 0.22f; sheenK = 0.10f; bodyA = 255f;
					break;
				case Kind.Danger:
					body = Mix(Rgb(150, 10, 45), Rgb(215, 35, 80), v);
					contour = Mix(Rgb(255, 90, 140), Rgb(255, 170, 190), v);
					lit = Rgb(255, 220, 230); litK = 0.5f; shadowK = 0.22f; sheenK = 0.10f; bodyA = 255f;
					break;
				case Kind.Lift:
					body = Mix(Rgb(21, 18, 28), Rgb(38, 34, 48), v);
					contour = Mix(Rgb(70, 64, 84), Rgb(150, 142, 168), v);
					lit = Rgb(200, 190, 220); litK = 0.5f; shadowK = 0.18f; sheenK = 0.08f; bodyA = 255f;
					break;
				case Kind.Outline:
					body = Rgb(14, 12, 20);
					contour = Mix(Rgb(96, 90, 110), Rgb(150, 144, 166), v);
					lit = Rgb(150, 144, 166); litK = 0.25f; shadowK = 0f; sheenK = 0f; bodyA = 150f;
					break;
				default:
					body = Mix(Rgb(9, 7, 13), Rgb(24, 20, 34), v);
					contour = Mix(Rgb(150, 72, 220), Rgb(222, 150, 255), v);
					lit = Rgb(150, 118, 205);
					break;
			}
			float inside = d - c;                                       // px inside the contour
			if (shadowK > 0f) { float sh = Sat(1f - inside / 7f); body *= 1f - shadowK * sh * sh; }
			if (sheenK > 0f) { float sn = Sat(1f - (dt - c) / 14f); body += (new Vector3(255f, 255f, 255f) - body) * (sheenK * sn * sn); }
			if (inside >= 0f)
			{
				float le = Sat(1f - Math.Abs(dt - c - 1.2f) / 1.4f);     // lit top edge, just inside the contour
				body = Mix(body, lit, litK * le);
				if (kind == Kind.Frame || kind == Kind.Plate || kind == Kind.Core || kind == Kind.Danger)
				{
					float sm = Sat(1f - Math.Abs(inside - 0.7f) / 0.9f);   // dark seam under the contour
					body = Mix(body, kind == Kind.Frame ? Rgb(96, 36, 168) : body * 0.55f, 0.55f * sm);
				}
				if (kind == Kind.Plate || kind == Kind.Core || kind == Kind.Danger)
				{
					float bb = Sat(1f - inside / 3f) * Sat((s.tex * 0.5f - y) / 6f);   // darker bottom lip
					body *= 1f - 0.28f * bb;
				}
			}
			float cov = kind == Kind.Core ? 0f : Sat(c - d + 0.5f);   // contour coverage
			Vector3 rgb = Mix(body, contour, cov);
			float a = Lerp(bodyA, 255f, cov);
			Set(ref px, Mix(glowRgb, rgb, edge), Lerp(glowA, a, edge));
		}

		private static readonly Sprite[] s_frame = new Sprite[3], s_plate = new Sprite[3], s_lift = new Sprite[3], s_outline = new Sprite[3], s_danger = new Sprite[3], s_shadow = new Sprite[3], s_core = new Sprite[3];
		private static Sprite s_glass, s_rounded, s_roundedShadow, s_track, s_fill, s_fillGlow, s_knobShadow, s_softCircle, s_pip, s_rhombus, s_glowBar, s_fadeUp, s_vertical;

		private static Sprite ChamferSprite(Sprite[] cache, Chamfer size, Kind kind)
		{
			int i = (int)size;
			if (cache[i] == null)
			{
				Spec s = Specs[i];
				cache[i] = Generate(s.tex, (ref Color32 px, float x, float y) => ChamferPixel(ref px, x, y, s, kind), s.border);
			}
			return cache[i];
		}

		public static Sprite ChamferFrameSprite(Chamfer size) { return ChamferSprite(s_frame, size, Kind.Frame); }
		public static Sprite ChamferPlateSprite(Chamfer size) { return ChamferSprite(s_plate, size, Kind.Plate); }
		public static Sprite ChamferLiftSprite(Chamfer size) { return ChamferSprite(s_lift, size, Kind.Lift); }       // lighter grey, steel rim (arrows, "+", tiles)
		public static Sprite ChamferOutlineSprite(Chamfer size) { return ChamferSprite(s_outline, size, Kind.Outline); }   // translucent, steel outline (OPEN CASE / GO / DONE)
		public static Sprite DangerPlateSprite(Chamfer size) { return ChamferSprite(s_danger, size, Kind.Danger); }

		// The chamfer that suits a rect: buttons and small widgets get proportionally smaller cuts.
		public static Chamfer ChamferFor(float width, float height)
		{
			float m = Math.Min(width, height);
			return m < 100f ? Chamfer.Small : m <= 160f ? Chamfer.Medium : Chamfer.Large;
		}

		public static Sprite BevelFrameSprite() { return ChamferFrameSprite(Chamfer.Large); }
		public static Sprite DangerPlateSprite() { return DangerPlateSprite(Chamfer.Medium); }

		// Edge-to-edge violet plate without its own contour: the filling for a frame's inside
		// (the old PLAY core, selected chips, mask shapes), drop-in for InfernoLobby.NeonPlateSprite.
		public static Sprite ChamferCoreSprite(Chamfer size) { return ChamferSprite(s_core, size, Kind.Core); }
		public static Sprite BevelPlateSprite() { return ChamferCoreSprite(Chamfer.Large); }

		// Soft drop shadow in the panel's shape: the body shape inset a little, alpha falling off
		// over 12 px outward. Drawn behind a button at (0, -7), 6 px larger.
		public static Sprite ShadowSprite(Chamfer size)
		{
			int i = (int)size;
			if (s_shadow[i] == null)
			{
				Spec s = Specs[i];
				float inset = s.glow + 5f;
				s_shadow[i] = Generate(s.tex, (ref Color32 px, float x, float y) =>
				{
					float d = Cut(x, y, inset, s.big, s.small, s.tex);
					float a = Sat((d + 12f) / 12f);
					Set(ref px, Vector3.zero, 255f * (float)Math.Pow(a, 1.6));
				}, s.border);
			}
			return s_shadow[i];
		}

		public static Sprite ShadowSprite() { return ShadowSprite(Chamfer.Large); }

		// Glass bevel (drop-in for InfernoLobby.GlassFrameSprite): wide translucent lavender rim lit
		// from the top, a thin dark seam, dark glass inside — on the medium chamfer, 128 px, border 48.
		public static Sprite BevelGlassSprite()
		{
			if (s_glass != null) return s_glass;
			const int tex = 128;
			s_glass = Generate(tex, (ref Color32 px, float x, float y) =>
			{
				float d = Cut(x, y, 6f, 28f, 10f, tex);
				float v = y / tex;
				float dt = (tex - 6f) - y;
				if (d < 0f) { float k = Sat(1f + d / 6f); Set(ref px, Rgb(170, 90, 255), 70f * k * k); return; }
				if (d < 1.5f) { Set(ref px, Rgb(236, 226, 255), 255f); return; }
				if (d < 12f)
				{
					float depth = (d - 1.5f) / 10.5f;
					float baseValue = 150f + 70f * v - 25f * depth;
					Set(ref px, Rgb(baseValue, baseValue - 12f, Math.Min(255f, baseValue + 30f)), 232f);
					return;
				}
				if (d < 14f) { Set(ref px, Rgb(40, 22, 66), 255f); return; }
				float sn = Sat(1f - (dt - 14f) / 16f);
				Set(ref px, Rgb(12, 7, 22) + new Vector3(18f, 14f, 24f) * (sn * sn), 238f);
			}, 48);
			return s_glass;
		}

		// Rounded bevel (256x64, radius 18 border, drop-in for button uses of roundedGlassSprite):
		// glow, violet contour, dark glass with a lit top edge.
		private static float RoundedDist(float x, float y, float inset, float radius, int w, int h)
		{
			float hx = w * 0.5f - inset - radius, hy = h * 0.5f - inset - radius;
			float qx = Math.Abs(x - w * 0.5f) - hx, qy = Math.Abs(y - h * 0.5f) - hy;
			float ox = Math.Max(qx, 0f), oy = Math.Max(qy, 0f);
			float outside = (float)Math.Sqrt(ox * ox + oy * oy) + Math.Min(Math.Max(qx, qy), 0f) - radius;
			return -outside;
		}

		public static Sprite BevelRoundedSprite()
		{
			if (s_rounded != null) return s_rounded;
			const int w = 256, h = 64;
			s_rounded = Generate(w, h, (ref Color32 px, float x, float y) =>
			{
				float d = RoundedDist(x, y, 4f, 14f, w, h);
				float v = y / h;
				float edge = Sat(d + 0.5f);
				float g = Sat(1f + d / 4f);
				Vector3 glowRgb = Rgb(184, 64, 255);
				float glowA = 100f * g * g;
				if (edge <= 0f) { Set(ref px, glowRgb, glowA); return; }
				float dt = (h - 4f) - y;
				Vector3 body = Mix(Rgb(9, 7, 13), Rgb(24, 20, 34), v);
				float inside = d - 2f;
				float sh = Sat(1f - inside / 6f); body *= 1f - 0.30f * sh * sh;
				float sn = Sat(1f - (dt - 2f) / 12f); body += (new Vector3(255f, 255f, 255f) - body) * (0.06f * sn * sn);
				if (inside >= 0f)
				{
					float le = Sat(1f - Math.Abs(dt - 3.2f) / 1.4f);
					body = Mix(body, Rgb(150, 118, 205), 0.55f * le);
				}
				float cov = Sat(2f - d + 0.5f);
				Vector3 rgb = Mix(body, Mix(Rgb(150, 72, 220), Rgb(222, 150, 255), v), cov);
				Set(ref px, Mix(glowRgb, rgb, edge), Lerp(glowA, Lerp(250f, 255f, cov), edge));
			}, new Vector4(18f, 18f, 18f, 18f));
			return s_rounded;
		}

		private static Sprite RoundedShadowSprite()
		{
			if (s_roundedShadow != null) return s_roundedShadow;
			const int w = 256, h = 64;
			s_roundedShadow = Generate(w, h, (ref Color32 px, float x, float y) =>
			{
				float d = RoundedDist(x, y, 10f, 10f, w, h);
				float a = Sat((d + 10f) / 10f);
				Set(ref px, Vector3.zero, 255f * (float)Math.Pow(a, 1.6));
			}, new Vector4(18f, 18f, 18f, 18f));
			return s_roundedShadow;
		}

		// ---- slider parts ------------------------------------------------------------------
		// Capsule 64x32 with 16 px caps (sliced border 16); positive inside.
		private static float Capsule(float x, float y, float radius)
		{
			float cx = Math.Max(16f, Math.Min(48f, x));
			float dx = x - cx, dy = y - 16f;
			return radius - (float)Math.Sqrt(dx * dx + dy * dy);
		}

		// Recessed groove: dark body, an inner shadow along the top, a light lip at the bottom.
		public static Sprite TrackSprite()
		{
			if (s_track != null) return s_track;
			s_track = Generate(64, 32, (ref Color32 px, float x, float y) =>
			{
				float d = Capsule(x, y, 15.5f);
				float edge = Sat(d + 0.5f);
				Vector3 body = Rgb(34, 28, 46);
				float sh = Sat(1f - d / 5f) * Sat((y - 16f) / 6f);      // inner shadow at the top
				body *= 1f - 0.5f * sh;
				float lt = Sat(1f - d / 2.5f) * Sat((14f - y) / 6f);    // lit lip at the bottom
				body = Mix(body, Rgb(92, 84, 112), 0.6f * lt);
				float rm = Sat(1f - Math.Abs(d - 0.8f) / 0.9f);          // dark outline
				body = Mix(body, Rgb(10, 8, 14), 0.7f * rm);
				Set(ref px, body, 255f * edge);
			}, new Vector4(16f, 16f, 16f, 16f));
			s_recessed.Add(s_track);
			return s_track;
		}

		// Glossy capsule in white (tinted by the Image): lighter top, a thin highlight line.
		public static Sprite FillSprite()
		{
			if (s_fill != null) return s_fill;
			s_fill = Generate(64, 32, (ref Color32 px, float x, float y) =>
			{
				float d = Capsule(x, y, 15.5f);
				float edge = Sat(d + 0.5f);
				float v = y / 32f;
				float lum = Lerp(0.66f, 0.92f, v);
				float hl = Sat(1f - Math.Abs((32f - y) - 5f) / 1.2f);
				lum = Lerp(lum, 1f, 0.9f * hl);
				Set(ref px, new Vector3(lum, lum, lum) * 255f, 255f * edge);
			}, new Vector4(16f, 16f, 16f, 16f));
			return s_fill;
		}

		// Soft glow around a fill: alpha peaks on the capsule's axis and fades to its edge.
		private static Sprite FillGlowSprite()
		{
			if (s_fillGlow != null) return s_fillGlow;
			s_fillGlow = Generate(64, 32, (ref Color32 px, float x, float y) =>
			{
				float d = Capsule(x, y, 16f);
				float a = Sat(d / 9f);
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * (float)Math.Pow(a, 1.4));
			}, new Vector4(16f, 16f, 16f, 16f));
			return s_fillGlow;
		}

		public static Sprite KnobSprite() { return InfernoSettings.KnobSprite(); }   // the chrome ball

		public static Sprite KnobShadowSprite()
		{
			if (s_knobShadow != null) return s_knobShadow;
			s_knobShadow = Generate(64, (ref Color32 px, float x, float y) =>
			{
				float dist = (float)Math.Sqrt((x - 32f) * (x - 32f) + (y - 32f) * (y - 32f));
				float a = Sat(1f - dist / 30f);
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * (float)Math.Pow(a, 1.6));
			}, 0);
			return s_knobShadow;
		}

		public static Sprite SoftCircleSprite()
		{
			if (s_softCircle != null) return s_softCircle;
			s_softCircle = Generate(48, (ref Color32 px, float x, float y) =>
			{
				float dist = (float)Math.Sqrt((x - 24f) * (x - 24f) + (y - 24f) * (y - 24f));
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * Sat((23f - dist) / 4f));
			}, 0);
			return s_softCircle;
		}

		/* [user request 2026-09-27: dots inside the bar] Where the slider's five tick dots sit, as an offset from
		   the track's LEFT edge. They used to be centred on the fractions 0, 1/4 ... 1 of the track, so the first and
		   last dot were centred exactly on the bar's ends and hung half outside it - and since the track is a capsule,
		   the part that hung over the rounded cap showed no bar behind it at all. They are now inset by PipInset from
		   each end, which clears the dot's own 2.5 px radius and the cap's curvature, so the bar is at its full 16 px
		   height behind every dot. The middle dot still lands on the track's centre and the spacing stays even.
		   Pure function so InfernoUiTests can check the containment for every width without building a UI.
		   original: pipRect.anchorMin = pipRect.anchorMax = new Vector2(i / 4f, 0.5f) with anchoredPosition zero */
		public const float PipSize = 5f;
		public const float PipInset = 10f;

		public static float PipX(int index, float trackWidth)
		{
			int i = index < 0 ? 0 : index > 4 ? 4 : index;
			float inset = PipInset;
			float maxInset = (trackWidth - PipSize) * 0.5f;   // a track too short for the nominal inset still keeps its dots on the bar
			if (maxInset < 0f) maxInset = 0f;
			if (inset > maxInset) inset = maxInset;
			return inset + (trackWidth - 2f * inset) * (i / 4f);
		}

		public static Sprite PipSprite()
		{
			if (s_pip != null) return s_pip;
			s_pip = Generate(16, (ref Color32 px, float x, float y) =>
			{
				float dist = (float)Math.Sqrt((x - 8f) * (x - 8f) + (y - 8f) * (y - 8f));
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * Sat(6.5f - dist + 0.5f));
			}, 0);
			return s_pip;
		}

		// Slider / toggle knob: a skewed rhombus slab (leans like "/"), dark body, the top and left
		// edges lit steel, the bottom and right edges a violet glow, a centre tick. 100x80, shown at 50x40.
		public static Sprite RhombusKnobSprite()
		{
			if (s_rhombus != null) return s_rhombus;
			const int w = 100, h = 80;
			const float inset = 7f, slant = 20f;
			float span = h - 2f * inset;
			float cos = span / (float)Math.Sqrt(span * span + slant * slant);
			s_rhombus = Generate(w, h, (ref Color32 px, float x, float y) =>
			{
				float t = (y - inset) / span;
				float xl = inset + slant * t, xr = (w - inset) - slant * (1f - t);
				float dl = (x - xl) * cos, dr = (xr - x) * cos, db = y - inset, dtop = (h - inset) - y;
				float d = Math.Min(Math.Min(dl, dr), Math.Min(db, dtop));
				float edge = Sat(d + 0.5f);
				float g = Sat(1f + d / 7f);
				Vector3 glowRgb = Rgb(200, 90, 255);
				float glowA = 95f * g * g;
				if (edge <= 0f) { Set(ref px, glowRgb, glowA); return; }
				float v = y / h;
				Vector3 body = Mix(Rgb(20, 16, 28), Rgb(40, 34, 52), v);
				float sn = Sat(1f - dtop / 26f);
				body += (new Vector3(255f, 255f, 255f) - body) * (0.08f * sn * sn);
				float litEdge = Sat(1f - Math.Min(dtop, dl) / 3.2f);
				float glowEdge = Sat(1f - Math.Min(db, dr) / 3.2f);
				body = Mix(body, Rgb(206, 196, 226), 0.9f * litEdge);
				body = Mix(body, Rgb(196, 80, 255), 0.95f * glowEdge);
				float tick = Sat(2.2f - Math.Abs(x - w * 0.5f)) * Sat(12f - Math.Abs(y - h * 0.5f) + 0.5f);
				body = Mix(body, Rgb(176, 146, 226), 0.9f * tick);
				Set(ref px, Mix(glowRgb, body, edge), Lerp(glowA, 255f, edge));
			}, Vector4.zero);
			return s_rhombus;
		}

		// ---- rhomboid tabs -------------------------------------------------------------------
		private static readonly Dictionary<string, Sprite> s_tabs = new Dictionary<string, Sprite>();
		private static readonly Dictionary<string, Sprite> s_pills = new Dictionary<string, Sprite>();   /* [user request 2026-09-27: action row] */

		// A parallelogram at the exact size (Image.Type.Simple), sides slanted like "/" (~18 deg).
		// Active: lighter grey gradient with a lit rim; inactive: near-black with a faint rim.
		public static Sprite TabSprite(int width, int height, bool active)
		{
			string key = width + "x" + height + (active ? "a" : "i");
			Sprite cached;
			if (s_tabs.TryGetValue(key, out cached) && cached != null) return cached;
			int w = Math.Max(8, width), h = Math.Max(8, height);
			float slant = (float)Math.Round(h * 0.325f);
			float cos = h / (float)Math.Sqrt(h * h + slant * slant);
			Sprite sprite = Generate(w, h, (ref Color32 px, float x, float y) =>
			{
				float t = y / h;
				float xl = 1f + (slant - 1f) * t, xr = (w - 1f) - (slant - 1f) * (1f - t);
				float dl = (x - xl) * cos, dr = (xr - x) * cos, db = y - 1f, dtop = (h - 1f) - y;
				float d = Math.Min(Math.Min(dl, dr), Math.Min(db, dtop));
				float edge = Sat(d + 0.5f);
				if (edge <= 0f) { Set(ref px, Vector3.zero, 0f); return; }
				Vector3 body, rim;
				float bodyA;
				if (active)
				{
					body = Mix(Rgb(26, 23, 32), Rgb(60, 56, 70), t);
					rim = Mix(Rgb(62, 56, 74), Rgb(130, 122, 148), t);
					bodyA = 255f;
					float sn = Sat(1f - dtop / (h * 0.35f));
					body += (new Vector3(255f, 255f, 255f) - body) * (0.06f * sn * sn);
				}
				else
				{
					body = Mix(Rgb(15, 13, 19), Rgb(23, 21, 29), t);
					rim = Mix(Rgb(34, 31, 42), Rgb(52, 48, 62), t);
					bodyA = 235f;
				}
				float rm = Sat(2f - d + 0.5f);
				float le = active ? Sat(1f - Math.Abs(dtop - 3f) / 1.4f) : 0f;
				body = Mix(body, Rgb(190, 182, 210), 0.45f * le);
				Set(ref px, Mix(body, rim, rm), bodyA * edge);
			}, Vector4.zero);
			s_tabs[key] = sprite;
			s_rhomboid[sprite] = slant;
			return sprite;
		}

		/* [user request 2026-09-26: dock underline fits the tab] The active tab's line, drawn at its exact pixel
		   size as a parallelogram with the tab's own slant (dx = slantPx over its height), flat brightness across
		   the whole width, a 1 px anti-aliased edge on the slanted ends and no taper — so it lies along the bottom
		   edge with ends that follow the slanted sides instead of a stretched 128 px strip with square, soft ends.
		   White, tinted by the caller. Cached per size.
		   [user request 2026-09-27: dock underline is a plain line] rows are not anti-aliased (every row is fully
		   opaque) so a 4 px line stays four crisp pixels with no soft halo above or below.
		   original: float d = Math.Min(Math.Min((x - xl) * cos, (xr - x) * cos), Math.Min(y, h - y)); */
		public static Sprite TabBarSprite(int width, int height, float slantPx)
		{
			int w = Math.Max(4, width), h = Math.Max(2, height);
			string key = "bar" + w + "x" + h + "s" + Math.Round(slantPx * 10f);
			Sprite cached;
			if (s_tabs.TryGetValue(key, out cached) && cached != null) return cached;
			float cos = h / (float)Math.Sqrt(h * h + slantPx * slantPx);
			Sprite sprite = Generate(w, h, (ref Color32 px, float x, float y) =>
			{
				float t = y / h;
				float xl = slantPx * t, xr = w - slantPx * (1f - t);
				float d = Math.Min((x - xl) * cos, (xr - x) * cos);
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * Sat(d + 0.5f));
			}, Vector4.zero);
			s_tabs[key] = sprite;
			return sprite;
		}

		// Horizontal violet bar with soft ends (the active tab's glow line), white for tinting.
		// Soft-ended bright bar. No longer used by the dock tab (MakeTab3D draws its underline with
		// TabBarSprite, sized to the parallelogram's bottom edge); kept for any other lens-shaped glow.
		private static Sprite GlowBarSprite()
		{
			if (s_glowBar != null) return s_glowBar;
			s_glowBar = Generate(128, 8, (ref Color32 px, float x, float y) =>
			{
				float u = x / 128f;
				float a = Sat(u / 0.3f) * Sat((1f - u) / 0.3f);
				float vy = Sat(1f - Math.Abs(y - 4f) / 4f + 0.25f);
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * a * a * vy);
			}, Vector4.zero);
			return s_glowBar;
		}

		// Alpha 1 at the bottom fading to 0 at the top (the haze above the glow bar).
		private static Sprite FadeUpSprite()
		{
			if (s_fadeUp != null) return s_fadeUp;
			s_fadeUp = Generate(8, 32, (ref Color32 px, float x, float y) =>
			{
				float v = 1f - y / 32f;
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * v * v);
			}, Vector4.zero);
			return s_fadeUp;
		}

		// Thin vertical divider: white, soft ends.
		private static Sprite VerticalLineSprite()
		{
			if (s_vertical != null) return s_vertical;
			s_vertical = Generate(4, 64, (ref Color32 px, float x, float y) =>
			{
				float v = y / 64f;
				float a = Sat(v / 0.15f) * Sat((1f - v) / 0.15f);
				Set(ref px, new Vector3(255f, 255f, 255f), 255f * a);
			}, Vector4.zero);
			return s_vertical;
		}

		// Procedural icons: garage / drive / profile / clock / car / road / case / gem-plus (+ the
		// lobby's chevrons and lock), rasterised by InfernoLobby.Icon.
		public static Sprite IconSprite(string icon)
		{
			if (string.IsNullOrEmpty(icon)) return null;
			return InfernoLobby.Icon(icon == "gem-plus" ? "plus" : icon);
		}

		// ================================================================================
		// Building blocks
		// ================================================================================
		private static Image NewImage(Transform parent, string name)
		{
			Image image = new GameObject(name).AddComponent<Image>();
			image.transform.SetParent(parent, false);
			image.raycastTarget = false;
			return image;
		}

		private static void Place(RectTransform rect, float x, float y, float w, float h, Vector2? anchor)
		{
			InfernoLobby.Place(rect, x, y, w, h, anchor ?? new Vector2(0.5f, 0.5f));
		}

		private static void Stretch(RectTransform rect, float left, float bottom, float right, float top)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.offsetMin = new Vector2(left, bottom);
			rect.offsetMax = new Vector2(-right, -top);
		}

		// Letter-spaced caps ("G A R A G E"); words keep a wider gap. uGUI Text has no tracking.
		public static string Spaced(string value)
		{
			if (string.IsNullOrEmpty(value)) return value;
			var sb = new System.Text.StringBuilder(value.Length * 2);
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (c == ' ') { sb.Append("  "); continue; }
				sb.Append(c);
				if (i < value.Length - 1) sb.Append(' ');
			}
			return sb.ToString();
		}

		private static Navigation NoNavigation()
		{
			Navigation navigation = Navigation.defaultNavigation;
			navigation.mode = Navigation.Mode.None;
			return navigation;
		}

		private static void ButtonColors(Button button)
		{
			ColorBlock colors = button.colors;
			colors.normalColor = Color.white;
			colors.highlightedColor = new Color(1.08f, 1.04f, 1.1f, 1f);
			colors.pressedColor = new Color(0.78f, 0.7f, 0.86f, 1f);
			colors.selectedColor = Color.white;
			colors.disabledColor = new Color(0.45f, 0.42f, 0.5f, 0.8f);
			colors.colorMultiplier = 1f;
			colors.fadeDuration = 0.06f;
			button.colors = colors;
		}

		private static uint LabelColor(Style style)
		{
			switch (style)
			{
				case Style.Plate: return 0xFFFFFF;
				case Style.Danger: return 0xFFD9E2;
				case Style.Tab: return Steel;
				default: return FrameText;
			}
		}

		// Re-skins an Image with the style's sprite (chamfer picked from the rect size).
		private static void ApplyStyle(Image body, Style style, float width, float height)
		{
			Chamfer size = ChamferFor(width, height);
			body.color = Color.white;
			body.type = Image.Type.Sliced;
			switch (style)
			{
				case Style.Plate: body.sprite = ChamferPlateSprite(size); break;
				case Style.Danger: body.sprite = DangerPlateSprite(size); break;
				case Style.Tab:
					body.sprite = TabSprite(Mathf.RoundToInt(width), Mathf.RoundToInt(height), false);
					body.type = Image.Type.Simple;
					break;
				case Style.Ghost: body.sprite = ChamferFrameSprite(size); body.color = new Color(1f, 1f, 1f, 0.72f); break;
				default: body.sprite = ChamferFrameSprite(size); break;
			}
		}

		private static void RectSize(RectTransform rect, out float width, out float height)
		{
			Rect r = rect.rect;
			width = r.width > 1f ? r.width : Math.Abs(rect.sizeDelta.x);
			height = r.height > 1f ? r.height : Math.Abs(rect.sizeDelta.y);
		}

		// ---- buttons -------------------------------------------------------------------------
		// Shadow sibling ("<name> Shadow") inserted just before the body; the body Image GameObject
		// is named `name` and carries the Button; child Text "Label". Pivot centre; anchor (0.5, 0.5).
		public static Button MakeButton3D(ShadowRunPrototype game, Transform parent, string name, string label, float x, float y, float w, float h, Style style, int size = 28, Vector2? anchor = null)
		{
			game = Game(game);
			Image body = NewImage(parent, name);
			body.raycastTarget = true;
			Place(body.rectTransform, x, y, w, h, anchor);
			ApplyStyle(body, style, w, h);
			Button button = body.gameObject.AddComponent<Button>();
			button.targetGraphic = body;
			button.navigation = NoNavigation();
			ButtonColors(button);
			if (label != null)
			{
				Text text = InfernoLobby.Label(game, body.transform, "Label", label, 0f, -h / 2f, w - 16f, h - 8f, size, LabelColor(style));
				if (style == Style.Tab) text.text = Spaced(label);
			}
			Elevate(game, body, style);
			return button;
		}

		public static Text ButtonLabel(Button button)
		{
			if (button == null) return null;
			Transform label = button.transform.Find("Label");
			return label != null ? label.GetComponent<Text>() : null;
		}

		// Swaps the body's look. A rhomboid tab body switches between its active (Plate) and
		// inactive (Tab) looks instead of taking a chamfered plate.
		public static void SetButtonStyle(Button button, Style style)
		{
			if (button == null) return;
			Image body = button.GetComponent<Image>();
			if (body == null) return;
			if (IsRhomboid(body.sprite) || style == Style.Tab && button.transform.Find("GlowBar") != null)
			{
				SetTabActive(button, style == Style.Plate);
				return;
			}
			float w, h;
			RectSize(body.rectTransform, out w, out h);
			ApplyStyle(body, style, w, h);
			Text label = ButtonLabel(button);
			if (label != null) label.color = InfernoLobby.Hex(LabelColor(style));
			UiPressFeedback feedback = FindFeedback(body);
			if (feedback != null) feedback.RefreshShadow();
		}

		/* [user request 2026-09-27: action row] */
		// A capsule at the exact size (Image.Type.Simple, like TabSprite): semicircular ends of radius h / 2, the
		// rounded frame's dark body gradient, lit top edge, violet rim and outer glow. Its silhouette is the one
		// shape in the kit that cannot be mistaken for a cut-corner plate, which is what a button sharing a row
		// with one needs.
		public static Sprite PillSprite(int width, int height)
		{
			string key = width + "x" + height;
			Sprite cached;
			if (s_pills.TryGetValue(key, out cached) && cached != null) return cached;
			int w = Math.Max(8, width), h = Math.Max(8, height);
			float radius = h * 0.5f - 1f;
			Sprite sprite = Generate(w, h, (ref Color32 px, float x, float y) =>
			{
				float cx = Math.Min(Math.Max(x, radius + 1f), w - 1f - radius);
				float dx = x - cx, dy = y - h * 0.5f;
				float d = radius - (float)Math.Sqrt(dx * dx + dy * dy);   // > 0 inside the capsule
				float v = y / h;
				float edge = Sat(d + 0.5f);
				float g = Sat(1f + d / 4f);
				Vector3 glowRgb = Rgb(184, 64, 255);
				float glowA = 100f * g * g;
				if (edge <= 0f) { Set(ref px, glowRgb, glowA); return; }
				float dt = (h - 4f) - y;
				Vector3 body = Mix(Rgb(9, 7, 13), Rgb(24, 20, 34), v);
				float inside = d - 2f;
				float sh = Sat(1f - inside / 6f); body *= 1f - 0.30f * sh * sh;
				float sn = Sat(1f - (dt - 2f) / 12f); body += (new Vector3(255f, 255f, 255f) - body) * (0.06f * sn * sn);
				if (inside >= 0f)
				{
					float le = Sat(1f - Math.Abs(dt - 3.2f) / 1.4f);
					body = Mix(body, Rgb(150, 118, 205), 0.55f * le);
				}
				float cov = Sat(2f - d + 0.5f);
				Vector3 rgb = Mix(body, Mix(Rgb(150, 72, 220), Rgb(222, 150, 255), v), cov);
				Set(ref px, Mix(glowRgb, rgb, edge), Lerp(glowA, Lerp(250f, 255f, cov), edge));
			}, Vector4.zero);
			s_pills[key] = sprite;
			return sprite;
		}

		/* [user request 2026-09-27: action row] */
		// Re-skins a button's body as that capsule and swaps its drop shadow to the rounded one. For a pair of
		// buttons that share a row and must differ in SHAPE rather than only in fill: the garage's wide cut-corner
		// PURCHASE plate beside the capsule STYLE key. The rect, height, label and press feedback are untouched,
		// so both buttons keep the same level.
		public static void MakePill(ShadowRunPrototype game, Button button)
		{
			if (button == null) return;
			Image body = button.GetComponent<Image>();
			if (body == null) return;
			Rect rect = body.rectTransform.rect;
			int w = Mathf.RoundToInt(rect.width > 1f ? rect.width : Math.Abs(body.rectTransform.sizeDelta.x));
			int h = Mathf.RoundToInt(rect.height > 1f ? rect.height : Math.Abs(body.rectTransform.sizeDelta.y));
			body.sprite = PillSprite(w, h);
			body.type = Image.Type.Simple;
			body.color = Color.white;
			UiPressFeedback feedback = FindFeedback(body);
			if (feedback != null && feedback.shadowImage != null)
			{
				feedback.shadowImage.sprite = RoundedShadowSprite();
				feedback.shadowImage.type = Image.Type.Sliced;
				feedback.RefreshShadow();
			}
		}

		private static UiPressFeedback FindFeedback(Image body)
		{
			UiPressFeedback[] all = body.GetComponentsInParent<UiPressFeedback>(true);
			for (int i = 0; i < all.Length; i++) if (all[i].body == body.rectTransform) return all[i];
			return null;
		}

		// Retrofits an existing Image-button: re-skins flat art (plain blocks, the game's rounded
		// glass), inserts a "<name> Shadow" sibling behind it and adds the press feedback. Bodies
		// that already wear a kit sprite keep it; invisible hit areas are left alone.
		public static void Elevate(ShadowRunPrototype game, Image body, Style style)
		{
			if (body == null) return;
			game = Game(game);
			if (body.color.a < 0.05f) return;
			if (FindFeedback(body) != null) return;
			RectTransform rect = body.rectTransform;
			float w, h;
			RectSize(rect, out w, out h);
			bool kit = IsKitSprite(body.sprite);
			if (!kit && style != Style.Tab)
			{
				bool flat = body.sprite == null || (game != null && body.sprite == game.roundedGlassSprite) || body.type == Image.Type.Sliced;
				if (flat) ApplyStyle(body, style, w, h);
			}
			// Only kit-shaped bodies get the drop shadow: custom art (the slanted PLAY plate, photo
			// buttons) would show a rectangular shadow poking out of its silhouette.
			RectTransform shadow = null;
			bool rhomboid = IsRhomboid(body.sprite);
			if (IsKitSprite(body.sprite) && !rhomboid && style != Style.Tab && !s_recessed.Contains(body.sprite)) shadow = MakeShadow(body, w, h);
			Button button = body.GetComponent<Button>();
			if (button == null) button = body.GetComponentInParent<Button>();
			GameObject host = button != null ? button.gameObject : body.gameObject;
			UiPressFeedback feedback = host.AddComponent<UiPressFeedback>();
			Transform label = body.transform.Find("Label");
			feedback.Bind(body, button, label != null ? label as RectTransform : null, shadow);
		}

		private static RectTransform MakeShadow(Image body, float w, float h)
		{
			RectTransform rect = body.rectTransform;
			Image shadow = NewImage(rect.parent, body.name + " Shadow");
			shadow.sprite = body.sprite == s_rounded ? RoundedShadowSprite() : ShadowSprite(ChamferFor(w, h));
			shadow.type = Image.Type.Sliced;
			shadow.color = new Color(0f, 0f, 0f, 0.55f);
			RectTransform s = shadow.rectTransform;
			s.anchorMin = rect.anchorMin;
			s.anchorMax = rect.anchorMax;
			s.pivot = rect.pivot;
			s.sizeDelta = rect.sizeDelta + UiPressFeedback.ShadowPad;
			s.anchoredPosition = rect.anchoredPosition + UiPressFeedback.ShadowOffset;
			s.localScale = rect.localScale;
			s.localRotation = rect.localRotation;
			s.SetSiblingIndex(rect.GetSiblingIndex());
			if (rect.parent != null && rect.parent.GetComponent<LayoutGroup>() != null)
				shadow.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
			shadow.gameObject.SetActive(body.gameObject.activeSelf);
			return s;
		}

		// ---- panel ---------------------------------------------------------------------------
		// Chamfered card: frame, fill and lit top edge come from one sliced sprite. Not a raycast target.
		public static Image MakePanel(ShadowRunPrototype game, Transform parent, string name, float x, float y, float w, float h, Vector2? anchor = null)
		{
			Game(game);
			Image panel = NewImage(parent, name);
			Place(panel.rectTransform, x, y, w, h, anchor);
			panel.sprite = ChamferFrameSprite(ChamferFor(w, h));
			panel.type = Image.Type.Sliced;
			panel.color = Color.white;
			return panel;
		}

		// ---- rhomboid tab ---------------------------------------------------------------------
		// Icon above a letter-spaced label; SetTabActive lightens the fill and shows the violet glow bar.
		public static Button MakeTab3D(ShadowRunPrototype game, Transform parent, string name, string label, string icon, float x, float y, float w, float h, Vector2? anchor = null)
		{
			game = Game(game);
			Image body = NewImage(parent, name);
			body.raycastTarget = true;
			Place(body.rectTransform, x, y, w, h, anchor);
			int wi = Mathf.RoundToInt(w), hi = Mathf.RoundToInt(h);
			body.sprite = TabSprite(wi, hi, false);
			body.type = Image.Type.Simple;
			body.color = Color.white;
			Button button = body.gameObject.AddComponent<Button>();
			button.targetGraphic = body;
			button.navigation = NoNavigation();
			ButtonColors(button);
			float slant = VisualInset(body.sprite);
			float inner = w - 2f * slant;
			/* [user request 2026-09-26: dock underline fits the tab] TabSprite's parallelogram leans right going up:
			   its bottom edge runs from sprite x 1 to w - slant, i.e. it is centred (1 - slant) / 2 px LEFT of the
			   tab's centre, and its sides keep moving at slant / h px per px of height. The old bar was centred on
			   the tab and inner + 10 wide, so it overhung the bottom-right corner and stopped well short of the
			   bottom-left one. Now: the bar sits barLift px above the bottom rim, spans the edge available at its
			   own height minus a 12 px inset from each rim, is centred on that span, and is drawn as a parallelogram
			   with the same slant (TabBarSprite).
			   original: Place(haze, 0f, -h / 2f + 34f, inner - 8f, 48f); Place(bar, 0f, -h / 2f + 9f, inner + 10f, 8f) with GlowBarSprite() */
			/* [user request 2026-09-27: dock underline is a plain line] no haze above the bar and a 4 px bar with crisp
			   rows: the active tab's underline is just a line. SetTabActive still looks for a "Glow" child and tolerates
			   its absence. original: barHeight = 6f; Image haze = NewImage(body.transform, "Glow"); haze.sprite =
			   FadeUpSprite(); haze.type = Image.Type.Simple; haze.color = InfernoLobby.Hex(Neon, 0.28f);
			   Place(haze.rectTransform, barCenter + slant / h * 24f, -h / 2f + barLift + 24f, barWidth - 8f, 44f, null);
			   haze.gameObject.SetActive(false); */
			const float barHeight = 4f, barLift = 7f, barInset = 12f;
			float yc = barLift + barHeight / 2f;                                              // bar centre, px above the bottom
			float edgeL = 1f + (slant - 1f) * (yc / h), edgeR = (w - 1f) - (slant - 1f) * (1f - yc / h);
			float barWidth = Mathf.Max(24f, (edgeR - edgeL) - 2f * barInset);
			float barCenter = (edgeL + edgeR) / 2f - w / 2f;
			float barSlant = slant / h * barHeight;
			// the line along the bottom edge (only on the active tab)
			Image bar = NewImage(body.transform, "GlowBar");
			bar.sprite = TabBarSprite(Mathf.RoundToInt(barWidth), Mathf.RoundToInt(barHeight), barSlant);
			bar.type = Image.Type.Simple;
			bar.color = InfernoLobby.Hex(Neon);
			Place(bar.rectTransform, barCenter, -h / 2f + yc, barWidth, barHeight, null);
			bar.gameObject.SetActive(false);
			Sprite glyph = IconSprite(icon);
			if (glyph != null)
			{
				Image image = NewImage(body.transform, "Icon");
				image.sprite = glyph;
				image.type = Image.Type.Simple;
				image.preserveAspect = true;
				image.color = InfernoLobby.Hex(Steel);
				Place(image.rectTransform, 0f, h * 0.14f, 48f, 48f, null);
			}
			if (label != null)
			{
				Text text = InfernoLobby.Label(game, body.transform, "Label", Spaced(label), 0f, -h / 2f - h * 0.22f, inner, 30f, 22, Steel);
				text.resizeTextMinSize = 14;
			}
			Elevate(game, body, Style.Tab);
			return button;
		}

		public static void SetTabActive(Button tab, bool active)
		{
			if (tab == null) return;
			Image body = tab.GetComponent<Image>();
			if (body == null) return;
			float w, h;
			RectSize(body.rectTransform, out w, out h);
			body.sprite = TabSprite(Mathf.RoundToInt(w), Mathf.RoundToInt(h), active);
			body.type = Image.Type.Simple;
			Transform t = tab.transform;
			Transform haze = t.Find("Glow"), bar = t.Find("GlowBar"), icon = t.Find("Icon"), label = t.Find("Label");
			if (haze != null) haze.gameObject.SetActive(active);
			if (bar != null) bar.gameObject.SetActive(active);
			if (icon != null) icon.GetComponent<Image>().color = InfernoLobby.Hex(active ? NeonSoft : Steel);
			if (label != null) label.GetComponent<Text>().color = InfernoLobby.Hex(active ? 0xFFFFFFu : Steel);
		}

		// ---- toggle -----------------------------------------------------------------------------
		// Violet pill with ON / OFF text and a rhombus knob that slides; the body carries the Button.
		public static Button MakeToggle3D(ShadowRunPrototype game, Transform parent, string name, float x, float y, float w, float h, Vector2? anchor = null)
		{
			game = Game(game);
			Image body = NewImage(parent, name);
			body.raycastTarget = true;
			Place(body.rectTransform, x, y, w, h, anchor);
			body.sprite = ChamferFrameSprite(Chamfer.Small);
			body.type = Image.Type.Sliced;
			body.color = Color.white;
			Button button = body.gameObject.AddComponent<Button>();
			button.targetGraphic = body;
			button.navigation = NoNavigation();
			ButtonColors(button);
			float knobW = Math.Min(48f, w * 0.3f), knobH = Math.Min(h - 10f, knobW * 0.8f);
			Text state = InfernoLobby.Label(game, body.transform, "State", "OFF", 0f, -h / 2f, w - knobW - 28f, h - 12f, 22, Steel);
			Image shadow = NewImage(body.transform, "Knob Shadow");
			shadow.sprite = KnobShadowSprite();
			shadow.type = Image.Type.Simple;
			shadow.color = new Color(0f, 0f, 0f, 0.5f);
			Place(shadow.rectTransform, 0f, -4f, knobW * 1.4f, knobH * 1.6f, null);
			Image knob = NewImage(body.transform, "Knob");
			knob.sprite = RhombusKnobSprite();
			knob.type = Image.Type.Simple;
			Place(knob.rectTransform, 0f, 0f, knobW, knobH, null);
			UiToggleAnim anim = body.gameObject.AddComponent<UiToggleAnim>();
			anim.knob = knob.rectTransform;
			anim.knobShadow = shadow.rectTransform;
			anim.state = state;
			anim.travel = w / 2f - knobW / 2f - 8f;
			anim.textShift = knobW / 2f + 6f;
			anim.Apply(false, true);
			Elevate(game, body, Style.Frame);
			return button;
		}

		public static void SetToggle(Button toggle, bool on)
		{
			if (toggle == null) return;
			Image body = toggle.GetComponent<Image>();
			if (body != null)
			{
				body.sprite = on ? ChamferPlateSprite(Chamfer.Small) : ChamferFrameSprite(Chamfer.Small);
				body.type = Image.Type.Sliced;
			}
			UiToggleAnim anim = toggle.GetComponent<UiToggleAnim>();
			// Slide only while the toggle is on screen and the player loop is ticking (an unfocused
			// editor does not run Update, so a pending slide would leave the knob on the text).
			if (anim != null) anim.Apply(on, !toggle.gameObject.activeInHierarchy || !Application.isFocused);
		}

		// ---- slider -------------------------------------------------------------------------------
		// Real UnityEngine.UI.Slider (wholeNumbers, min..max, navigation None). Children: "Track"
		// (with "Fill" inside), 5 "Pip"s, "Knob" (the handle: "Knob Shadow" + the rhombus "Knob Face").
		// The GameObject named `name` is the 64 px hit area and carries the Slider.
		public static Slider MakeSlider3D(ShadowRunPrototype game, Transform parent, string name, float x, float y, float w, float h, int min, int max, Vector2? anchor = null)
		{
			Game(game);
			Image host = NewImage(parent, name);
			host.color = new Color(0f, 0f, 0f, 0f);
			host.raycastTarget = true;
			RectTransform hostRect = host.rectTransform;
			Place(hostRect, x, y, w, h, anchor);

			Image track = NewImage(hostRect, "Track");
			track.sprite = TrackSprite();
			track.type = Image.Type.Sliced;
			RectTransform trackRect = track.rectTransform;
			trackRect.anchorMin = new Vector2(0f, 0.5f);
			trackRect.anchorMax = new Vector2(1f, 0.5f);
			trackRect.pivot = new Vector2(0.5f, 0.5f);
			trackRect.sizeDelta = new Vector2(0f, 16f);
			trackRect.anchoredPosition = Vector2.zero;

			Image fill = NewImage(trackRect, "Fill");
			fill.sprite = FillSprite();
			fill.type = Image.Type.Sliced;
			fill.color = InfernoLobby.Hex(Electric);
			RectTransform fillRect = fill.rectTransform;
			Stretch(fillRect, 2f, 2f, 2f, 2f);
			Image glow = NewImage(fillRect, "Fill Glow");
			glow.sprite = FillGlowSprite();
			glow.type = Image.Type.Sliced;
			glow.color = InfernoLobby.Hex(Neon, 0.42f);
			Stretch(glow.rectTransform, -6f, -7f, -6f, -7f);
			glow.transform.SetAsFirstSibling();

			for (int i = 0; i < 5; i++)
			{
				Image pip = NewImage(hostRect, "Pip");
				pip.sprite = PipSprite();
				pip.type = Image.Type.Simple;
				pip.color = InfernoLobby.Hex(Lilac, 0.3f);
				RectTransform pipRect = pip.rectTransform;
				// [user request 2026-09-27: dots inside the bar] anchored to the track's left edge and inset (PipX),
				// so no dot overhangs the bar. original: anchor (i / 4f, 0.5f) with anchoredPosition zero
				pipRect.anchorMin = pipRect.anchorMax = new Vector2(0f, 0.5f);
				pipRect.pivot = new Vector2(0.5f, 0.5f);
				pipRect.sizeDelta = new Vector2(PipSize, PipSize);
				pipRect.anchoredPosition = new Vector2(PipX(i, w), 0f);
			}

			RectTransform knob = new GameObject("Knob").AddComponent<RectTransform>();
			knob.SetParent(hostRect, false);
			knob.anchorMin = new Vector2(0f, 0f);
			knob.anchorMax = new Vector2(0f, 1f);
			knob.pivot = new Vector2(0.5f, 0.5f);
			knob.sizeDelta = new Vector2(52f, 40f - h);   // Slider stretches the handle over the host; keep it 52x40
			knob.anchoredPosition = Vector2.zero;
			Image knobShadow = NewImage(knob, "Knob Shadow");
			knobShadow.sprite = KnobShadowSprite();
			knobShadow.type = Image.Type.Simple;
			knobShadow.color = new Color(0f, 0f, 0f, 0.5f);
			Place(knobShadow.rectTransform, 2f, -5f, 68f, 56f, null);
			Image face = NewImage(knob, "Knob Face");
			face.sprite = RhombusKnobSprite();
			face.type = Image.Type.Simple;
			face.raycastTarget = true;
			Place(face.rectTransform, 0f, 0f, 52f, 40f, null);

			Slider slider = host.gameObject.AddComponent<Slider>();
			slider.targetGraphic = face;
			slider.navigation = NoNavigation();
			slider.minValue = min;
			slider.maxValue = max;
			slider.wholeNumbers = true;
			slider.fillRect = fillRect;
			slider.handleRect = knob;
			slider.SetValueWithoutNotify(min);
			return slider;
		}

		// ---- segment bar ----------------------------------------------------------------------------
		// Recessed track with `segments` glossy slots; SetSegments lights the first `filled`.
		public static RectTransform MakeSegmentBar(ShadowRunPrototype game, Transform parent, string name, float x, float y, float w, float h, int segments, Color fill, Vector2? anchor = null)
		{
			Game(game);
			RectTransform bar = new GameObject(name).AddComponent<RectTransform>();
			bar.SetParent(parent, false);
			Place(bar, x, y, w, h, anchor);
			Image track = NewImage(bar, "Track");
			track.sprite = TrackSprite();
			track.type = Image.Type.Sliced;
			Stretch(track.rectTransform, 0f, 0f, 0f, 0f);
			segments = Math.Max(1, segments);
			const float gap = 4f, inset = 3f;
			float slot = (w - 2f * inset - gap * (segments - 1)) / segments;
			for (int i = 0; i < segments; i++)
			{
				Image segment = NewImage(bar, "Segment");
				segment.sprite = FillSprite();
				segment.type = Image.Type.Sliced;
				segment.color = new Color(fill.r, fill.g, fill.b, 0.16f);
				RectTransform rect = segment.rectTransform;
				rect.anchorMin = new Vector2(0f, 0f);
				rect.anchorMax = new Vector2(0f, 1f);
				rect.pivot = new Vector2(0f, 0.5f);
				rect.sizeDelta = new Vector2(slot, -2f * inset);
				rect.anchoredPosition = new Vector2(inset + i * (slot + gap), 0f);
			}
			return bar;
		}

		public static void SetSegments(RectTransform bar, int filled)
		{
			if (bar == null) return;
			int index = 0;
			for (int i = 0; i < bar.childCount; i++)
			{
				Transform child = bar.GetChild(i);
				if (child.name != "Segment") continue;
				Image image = child.GetComponent<Image>();
				if (image == null) continue;
				Color color = image.color;
				color.a = index < filled ? 1f : 0.16f;
				image.color = color;
				index++;
			}
		}

		// ---- type ------------------------------------------------------------------------------------
		// Bold-italic display title with a letter-spaced lilac subtitle under it. Returns the title;
		// the root (named `name`) is a w x 110 box placed with the usual pivot-centre semantics.
		public static Text Heading(ShadowRunPrototype game, Transform parent, string name, string title, string subtitle, float x, float y, float w, Vector2? anchor = null)
		{
			game = Game(game);
			RectTransform root = new GameObject(name).AddComponent<RectTransform>();
			root.SetParent(parent, false);
			Place(root, x, y, w, 110f, anchor);
			Text text = InfernoLobby.Label(game, root, "Title", title, 0f, -34f, w, 68f, 58, 0xFFFFFF, 3);
			text.fontStyle = FontStyle.BoldAndItalic;
			text.resizeTextMinSize = 36;
			if (!string.IsNullOrEmpty(subtitle))
			{
				Text sub = InfernoLobby.Label(game, root, "Subtitle", Spaced(subtitle), 2f, -88f, w, 30f, 22, Lilac, 3);
				sub.resizeTextMinSize = 14;
			}
			return text;
		}

		// Small caps letter-spaced lilac section label with a thin divider line under it.
		public static Text SectionLabel(ShadowRunPrototype game, Transform parent, string name, string label, float x, float y, float w, bool rule = true)
		{
			game = Game(game);
			Text text = InfernoLobby.Label(game, parent, name, Spaced(label), x, y, w, 28f, 20, Lilac, 3);
			text.resizeTextMinSize = 14;
			if (rule)
			{
				Image line = NewImage(parent, name + " Rule");
				line.color = InfernoLobby.Hex(Smoke);
				InfernoLobby.Place(line.rectTransform, x, y - 26f, w, 2f, text.rectTransform.anchorMin);
			}
			return text;
		}

		// Vertical divider line (soft ends), white-tinted by `color`.
		public static Image Divider(Transform parent, string name, float x, float y, float height, uint color)
		{
			Image line = NewImage(parent, name);
			line.sprite = VerticalLineSprite();
			line.type = Image.Type.Simple;
			line.color = InfernoLobby.Hex(color);
			InfernoLobby.Place(line.rectTransform, x, y, 2f, height);
			return line;
		}
	}
}

// Press-down feedback for the kit's buttons: on pointer down the body scales to 0.96, its label
// dips 3 px, the drop shadow closes in (0,-2) at half alpha and the body darkens to 0.9; up / exit
// restores. Respects Button.interactable. The component also keeps the shadow sibling glued to
// the body (position, size, anchors, sibling order, active state) since other code moves and
// hides buttons after they are built.
internal sealed class UiPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	public static readonly Vector2 ShadowOffset = new Vector2(0f, -7f);
	public static readonly Vector2 ShadowPad = new Vector2(6f, 6f);

	public RectTransform body;
	public Image bodyImage;
	public Button button;
	public RectTransform label;
	public RectTransform shadow;
	public Image shadowImage;

	private bool down;
	private Vector3 scale0;
	private Vector2 label0;
	private Color color0;
	private float shadowAlpha0;

	public void Bind(Image image, Button target, RectTransform labelRect, RectTransform shadowRect)
	{
		body = image.rectTransform;
		bodyImage = image;
		button = target;
		label = labelRect;
		shadow = shadowRect;
		shadowImage = shadowRect != null ? shadowRect.GetComponent<Image>() : null;
		SyncShadow();
	}

	public void RefreshShadow() { SyncShadow(); }

	private void OnEnable()
	{
		SyncShadow();
		if (shadow != null) shadow.gameObject.SetActive(true);
	}

	private void OnDisable()
	{
		Release();
		if (shadow != null) shadow.gameObject.SetActive(false);
	}

	private void LateUpdate()
	{
		SyncShadow();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (down || body == null) return;
		if (button != null && !button.interactable) return;
		down = true;
		scale0 = body.localScale;
		body.localScale = new Vector3(scale0.x * 0.96f, scale0.y * 0.96f, scale0.z);
		if (label != null) { label0 = label.anchoredPosition; label.anchoredPosition = label0 + new Vector2(0f, -3f); }
		if (bodyImage != null) { color0 = bodyImage.color; bodyImage.color = new Color(color0.r * 0.9f, color0.g * 0.9f, color0.b * 0.9f, color0.a); }
		if (shadowImage != null) { shadowAlpha0 = shadowImage.color.a; Color c = shadowImage.color; c.a = shadowAlpha0 * 0.5f; shadowImage.color = c; }
		SyncShadow();
	}

	public void OnPointerUp(PointerEventData eventData) { Release(); }
	public void OnPointerExit(PointerEventData eventData) { Release(); }

	private void Release()
	{
		if (!down) return;
		down = false;
		if (body != null) body.localScale = scale0;
		if (label != null) label.anchoredPosition = label0;
		if (bodyImage != null) bodyImage.color = color0;
		if (shadowImage != null) { Color c = shadowImage.color; c.a = shadowAlpha0; shadowImage.color = c; }
		SyncShadow();
	}

	private void SyncShadow()
	{
		if (shadow == null || body == null) return;
		Transform parent = body.parent;
		if (parent == null) return;
		if (shadow.parent != parent) shadow.SetParent(parent, false);
		int bodyIndex = body.GetSiblingIndex(), shadowIndex = shadow.GetSiblingIndex();
		if (shadowIndex != bodyIndex - 1) shadow.SetSiblingIndex(shadowIndex < bodyIndex ? bodyIndex - 1 : bodyIndex);
		if (shadow.anchorMin != body.anchorMin) shadow.anchorMin = body.anchorMin;
		if (shadow.anchorMax != body.anchorMax) shadow.anchorMax = body.anchorMax;
		if (shadow.pivot != body.pivot) shadow.pivot = body.pivot;
		Vector2 size = body.sizeDelta + ShadowPad;
		if (shadow.sizeDelta != size) shadow.sizeDelta = size;
		Vector2 position = body.anchoredPosition + ShadowOffset + (down ? new Vector2(0f, 5f) : Vector2.zero);
		if (shadow.anchoredPosition != position) shadow.anchoredPosition = position;
		Vector3 scale = down ? scale0 : body.localScale;
		if (shadow.localScale != scale) shadow.localScale = scale;
		if (shadow.localRotation != body.localRotation) shadow.localRotation = body.localRotation;
		bool active = body.gameObject.activeSelf && isActiveAndEnabled;
		if (shadow.gameObject.activeSelf != active) shadow.gameObject.SetActive(active);
	}
}

// Slides a toggle's rhombus knob (and its ON / OFF text) between the two ends over 0.12 s.
internal sealed class UiToggleAnim : MonoBehaviour
{
	public RectTransform knob;
	public RectTransform knobShadow;
	public Text state;
	public float travel = 56f;
	public float textShift = 30f;

	private bool on;
	private float target, current;
	private bool animating;

	public void Apply(bool value, bool instant)
	{
		on = value;
		target = on ? travel : -travel;
		if (state != null)
		{
			state.text = on ? "ON" : "OFF";
			state.color = ShadowRunPrototype.InfernoLobby.Hex(on ? 0xFFFFFFu : 0x9A94A8u);
			state.rectTransform.anchoredPosition = new Vector2(on ? -textShift : textShift, state.rectTransform.anchoredPosition.y);
		}
		if (instant) { current = target; Put(); animating = false; }
		else animating = true;
	}

	private void Put()
	{
		if (knob != null) knob.anchoredPosition = new Vector2(current, 0f);
		if (knobShadow != null) knobShadow.anchoredPosition = new Vector2(current, -4f);
	}

	// A slide that was pending while the page was hidden lands at once when it shows.
	private void OnEnable()
	{
		if (!animating) return;
		current = target;
		Put();
		animating = false;
	}

	private void Update()
	{
		if (!animating) return;
		float step = (2f * travel) * (Time.unscaledDeltaTime / 0.12f);
		current = Mathf.MoveTowards(current, target, Math.Max(step, 1f));
		Put();
		if (Mathf.Approximately(current, target)) animating = false;
	}
}

// Runs callbacks when a screen root is shown / hidden (the game-over root hides the gem counter
// this way: the shipped GameOver() has no lobby hook).
internal sealed class UiScreenLink : MonoBehaviour
{
	public Action onShown;
	public Action onHidden;
	private void OnEnable() { if (onShown != null) onShown(); }
	private void OnDisable() { if (onHidden != null) onHidden(); }
}
