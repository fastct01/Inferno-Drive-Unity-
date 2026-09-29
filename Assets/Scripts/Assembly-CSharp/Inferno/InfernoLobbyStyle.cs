using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// [user request 2026-09-21] Lobby restyle to the supplied mockup: dark chamfered panels
// with a thin steel rim instead of the violet neon frames, a slanted magenta PLAY plate
// (the light sweep from MakeHomeDriveShine runs across it), an outlined OPEN CASE button,
// goal rows with icons and progress bars, and a tab dock with icons. Everything here is
// drawn in code — no icon art survived the rip (the goals' Space_Exploration_GUI_Kit/Icons
// resources do not exist in this project).
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoLobby
	{
		public const uint PanelFill = 0x0D0B12, PanelLift = 0x1B1824, PanelRim = 0x37323F, Steel = 0x9A94A8, Smoke = 0x2A2632;
		public const uint Neon = 0xE33CFF, NeonDeep = 0xA014D0, NeonSoft = 0xFF7CF5;

		// ---- chamfered panel: TL / BR corners cut, thin rim -----------------------------
		private const int PanelChamfer = 26, PanelInset = 2;
		private static Sprite s_panel, s_panelLift, s_outline, s_playPlate;

		// Signed distance to the panel shape (positive inside); texture y grows upward.
		private static float Chamfered(int x, int y, int inset, int chamfer)
		{
			const int n = FrameTex - 1;
			float d = Math.Min(Math.Min((float)(x - inset), (float)(n - inset - x)), Math.Min((float)(y - inset), (float)(n - inset - y)));
			d = Math.Min(d, ((x - inset) + (n - inset - y) - chamfer) / 1.41421f);   // top-left cut
			d = Math.Min(d, ((n - inset - x) + (y - inset) - chamfer) / 1.41421f);   // bottom-right cut
			return d;
		}

		private static Sprite MakePanelSprite(uint fill, uint rim, int fillAlpha, float rimWidth)
		{
			Color f = Hex(fill), r = Hex(rim);
			return SlicedSprite((ref Color32 px, int x, int y) =>
			{
				float d = Chamfered(x, y, PanelInset, PanelChamfer);
				if (d < -0.5f) { Set(ref px, 0, 0, 0, 0); return; }
				float edge = Math.Min(1f, d + 0.5f);
				if (d < rimWidth) Set(ref px, (int)(r.r * 255), (int)(r.g * 255), (int)(r.b * 255), (int)(255f * edge));
				else if (d < rimWidth + 1f) Set(ref px, (int)((r.r + f.r) * 127), (int)((r.g + f.g) * 127), (int)((r.b + f.b) * 127), fillAlpha);
				else Set(ref px, (int)(f.r * 255), (int)(f.g * 255), (int)(f.b * 255), fillAlpha);
			}, FrameBorder);
		}

		/* [user request 2026-09-21: 3D UI] the lobby's panels now come from the widget kit: the
		   mockup's cut-corner card (violet contour + glow, lit top edge), its lifted grey variant
		   and the steel outline box, each cut to suit the widget's size. MakePanelSprite above stays
		   for reference. original: MakePanelSprite(PanelFill, PanelRim, 250, 2f) / (PanelLift,
		   0x4A4456, 255, 2f) / (0x14111B, 0x6A6478, 150, 2.5f) */
		public static Sprite PanelSprite() { if (s_panel == null) s_panel = InfernoUi.ChamferFrameSprite(InfernoUi.Chamfer.Large); return s_panel; }
		public static Sprite PanelLiftSprite() { if (s_panelLift == null) s_panelLift = InfernoUi.ChamferLiftSprite(InfernoUi.Chamfer.Medium); return s_panelLift; }
		public static Sprite OutlineSprite() { if (s_outline == null) s_outline = InfernoUi.ChamferOutlineSprite(InfernoUi.Chamfer.Small); return s_outline; }

		public static Image Panel(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height)
		{
			Image image = Block(game, parent, name, x, y, width, height, 0xFFFFFF);
			image.sprite = InfernoUi.ChamferFrameSprite(InfernoUi.ChamferFor(width, height));   // original: PanelSprite()
			image.type = Image.Type.Sliced;
			return image;
		}

		public static Image OutlineBox(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height)
		{
			Image image = Block(game, parent, name, x, y, width, height, 0xFFFFFF);
			image.sprite = InfernoUi.ChamferOutlineSprite(InfernoUi.ChamferFor(width, height));   // original: OutlineSprite()
			image.type = Image.Type.Sliced;
			return image;
		}

		/* [user request 2026-09-22: lobby mockup] The home page's steel language (LOBBY-SPEC.md): near-black
		   panels (#0A0A0E) with a thin 2 px steel-grey rim and a faint lighter run along their top-left
		   edges — no violet contour, no glow — and corners cut per panel: big top-left / bottom-right,
		   tiny top-right / bottom-left. Every box is rasterised at its exact pixel size (Image.Type.Simple)
		   and cached per size / cuts / look, so the cuts are never stretched by a 9-slice. Magenta is kept
		   for the PLAY plate, the progress fills and the gems. */
		public const uint MockFill = 0x0A0A0E, MockRim = 0x767686, MockRimLit = 0xAAAAB9, MockLift = 0x16161C, MockKey = 0x1E1E24;
		public const uint MockGreyLilac = 0xB4AFC3, MockSmall = 0xA0A0AF, MockGrey = 0xC8C8D2, MockRule = 0x282830, MockDivider = 0x23232A;
		public const uint MockMark = 0x787886, MockIcon = 0xAAA5BE, MockBar = 0xC800FF;
		public enum Look { Panel, Lift, Key, Outline }   // fill: #0A0A0E / (22,22,28) arrows / (30,30,36) "+" key / translucent buttons
		private static readonly Dictionary<string, Sprite> s_cut = new Dictionary<string, Sprite>();

		// Signed distance (px, positive inside) from the diagonal that cuts a corner `cut.x` wide and
		// `cut.y` tall; a, e = the pixel's distances from the corner's two edges. No cut: far inside.
		private static float CutEdge(float a, float e, Vector2 cut)
		{
			if (cut.x <= 0f || cut.y <= 0f) return 1e6f;
			return (cut.y * a + cut.x * e - cut.x * cut.y) / Mathf.Sqrt(cut.x * cut.x + cut.y * cut.y);
		}

		// Signed distance to the w x h rectangle with its four corners cut (pixel centres, y up).
		// `lit` / `rest`: distance to the nearest top-left edge (top, left, TL cut) and to the others,
		// so the rim can carry its lighter run along the top-left.
		private static float CutDistance(float x, float y, float w, float h, Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl, out float lit, out float rest)
		{
			float l = x, r = w - x, b = y, t = h - y;
			lit = Math.Min(Math.Min(l, t), CutEdge(l, t, tl));
			rest = Math.Min(Math.Min(r, b), Math.Min(CutEdge(r, t, tr), Math.Min(CutEdge(r, b, br), CutEdge(l, b, bl))));
			return Math.Min(lit, rest);
		}

		private static Sprite ExactSprite(int w, int h, Color32[] px)
		{
			Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
			texture.SetPixels32(px);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;
			return Sprite.Create(texture, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
		}

		// A w x h steel box with 45-degree corner cuts (tl, tr, br, bl in px; 0 = square corner).
		public static Sprite CutSprite(int w, int h, int tl, int tr, int br, int bl, Look look)
		{
			w = Math.Max(4, w); h = Math.Max(4, h);
			string key = w + "x" + h + ":" + tl + "," + tr + "," + br + "," + bl + ":" + (int)look;
			Sprite cached;
			if (s_cut.TryGetValue(key, out cached) && cached != null) return cached;
			Color fill = Hex(look == Look.Lift ? MockLift : look == Look.Key ? MockKey : MockFill), rim = Hex(MockRim), litRim = Hex(MockRimLit);
			float fillAlpha = look == Look.Outline ? 205f : 255f;
			const float rimWidth = 2f;
			Vector2 cTL = new Vector2(tl, tl), cTR = new Vector2(tr, tr), cBR = new Vector2(br, br), cBL = new Vector2(bl, bl);
			Color32[] px = new Color32[w * h];
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					float lit, rest;
					float d = CutDistance(x + 0.5f, y + 0.5f, w, h, cTL, cTR, cBR, cBL, out lit, out rest);
					if (d < -0.5f) { Set(ref px[y * w + x], 0, 0, 0, 0); continue; }
					float edge = Math.Min(1f, d + 0.5f);
					float run = Mathf.Clamp01((rest - lit) / 8f + 0.5f);          // 1 where a top-left edge is the nearest
					Color rimHere = Color.Lerp(rim, litRim, 0.7f * run);
					float cov = Mathf.Clamp01(rimWidth - d + 0.5f);                // rim coverage, 1 px anti-aliased seam
					Color c = Color.Lerp(fill, rimHere, cov);
					float a = Mathf.Lerp(fillAlpha, 255f, cov) * edge;
					Set(ref px[y * w + x], (int)(c.r * 255f), (int)(c.g * 255f), (int)(c.b * 255f), (int)a);
				}
			}
			Sprite sprite = ExactSprite(w, h, px);
			s_cut[key] = sprite;
			return sprite;
		}

		// A steel box placed like Block (pivot centre, anchor top-centre of the parent).
		public static Image SteelBox(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height, int tl, int tr, int br, int bl, Look look = Look.Panel)
		{
			Image image = Block(game, parent, name, x, y, width, height, 0xFFFFFF);
			image.sprite = CutSprite(Mathf.RoundToInt(width), Mathf.RoundToInt(height), tl, tr, br, bl, look);
			image.type = Image.Type.Simple;
			return image;
		}

		// Letter-spaced label (InfernoUi.Spaced) that keeps its exact size: Label() never goes below 16 px.
		private static Text SpacedLabel(ShadowRunPrototype game, Transform parent, string name, string value, float x, float y, float width, float height, int size, uint color, int alignment = 4)
		{
			Text text = Label(game, parent, name, InfernoUi.Spaced(value), x, y, width, height, size, color, alignment);
			text.resizeTextMinSize = Math.Min(size, text.resizeTextMinSize);
			return text;
		}

		/* [user request 2026-09-22: lobby mockup] Sub-space tracking for labels whose text is written plain by other
		   code (the goals timer, the daily-case caption): InfernoTracking shifts each glyph quad by `spacing` px
		   (page units = canvas px) and re-centres the run for the label's alignment, so the tracking survives
		   every later `text.text = ...`. InfernoUi.Spaced (a whole space per gap) is too wide for the timer. */
		private static Text Tracked(Text text, float spacing)
		{
			InfernoTracking tracking = text.gameObject.AddComponent<InfernoTracking>();
			tracking.spacing = spacing;
			return text;
		}

		/* [user request 2026-09-22: lobby mockup] The case card shows the 990x630 case-preview render (the case screen
		   draws it whole at 880x560). The crate body only covers ~535x290 of it, centred at (494, 407 from the top)
		   for the fixed preview camera, so drawn whole into the card's rect it read as a 154x106 thumbnail. This crops
		   the RawImage's uvRect to a window around the body sized so the body spans `bodyWidth` of the rect's width,
		   with the window's aspect matching the rect (no distortion). The crate's yaw (drag on the case screen)
		   changes its extents a little, hence the margin. */
		private const float CaseTexW = 990f, CaseTexH = 630f, CaseBodyCx = 494f, CaseBodyCy = 407f, CaseBodyW = 535f;
		private static void CaseArtCrop(RawImage art, float rectWidth, float rectHeight, float bodyWidth)
		{
			float w = CaseBodyW * rectWidth / bodyWidth, h = w * rectHeight / rectWidth;   // window in texture px
			float u0 = Mathf.Clamp((CaseBodyCx - w / 2f) / CaseTexW, 0f, 1f - w / CaseTexW);
			float v0 = Mathf.Clamp((CaseTexH - CaseBodyCy - h / 2f) / CaseTexH, 0f, 1f - h / CaseTexH);
			art.uvRect = new Rect(u0, v0, Math.Min(1f, w / CaseTexW), Math.Min(1f, h / CaseTexH));
		}

		// ---- PLAY plate: an asymmetric octagon, magenta gradient with a light sweep and scratches ----
		/* [user request 2026-09-22: lobby mockup] 960x212 at exact size, cuts TL 92x103, TR 48x46, BR 60x63,
		   BL 32x46; (205,25,215) top-left -> (95,5,160) right/bottom, a diagonal lighter sweep, faint grunge
		   scratches in the left third, a 2 px (235,150,255) edge line along the top-left cut and the bottom
		   edge, and a separate soft glow sprite (PlayGlowSprite, 24 px, alpha 0.35) drawn behind it.
		   original: a right-leaning parallelogram, PlayW = 512, PlayH = 112, PlaySlant = 26,
		   PlayStreaks = { 0.16f, 0.30f, 0.36f, 0.64f, 0.72f, 0.88f }, bright rim (255,176,255) */
		private const int PlayW = 960, PlayH = 212, PlayGlowPad = 28;
		private static readonly Vector2 PlayTL = new Vector2(92f, 103f), PlayTR = new Vector2(48f, 46f), PlayBR = new Vector2(60f, 63f), PlayBL = new Vector2(32f, 46f);
		private static readonly float[] PlayStreaks = { 0.30f, 0.62f };   // sweep centres (u shifted by the slope)
		private static Sprite s_playGlow;

		// Faint diagonal scratches in the left third of the plate (deterministic LCG, no art survived).
		private static List<Stroke> PlayScratches()
		{
			List<Stroke> s = new List<Stroke>();
			uint seed = 7u;
			for (int i = 0; i < 18; i++)
			{
				seed = seed * 1664525u + 1013904223u; float u0 = 0.03f + (seed >> 8) / 16777216f * 0.30f;
				seed = seed * 1664525u + 1013904223u; float v0 = (seed >> 8) / 16777216f;
				seed = seed * 1664525u + 1013904223u; float len = 24f + (seed >> 8) / 16777216f * 90f;
				seed = seed * 1664525u + 1013904223u; float ang = Mathf.Deg2Rad * (52f + (seed >> 8) / 16777216f * 34f);
				float ax = u0 * PlayW, ay = v0 * PlayH;
				s.Add(new Stroke { ax = ax, ay = ay, bx = ax + Mathf.Cos(ang) * len, by = ay + Mathf.Sin(ang) * len, w = i % 3 == 0 ? 1.6f : 1f });
			}
			return s;
		}

		public static Sprite PlayPlateSprite()
		{
			if (s_playPlate != null) return s_playPlate;
			List<Stroke> scratches = PlayScratches();
			Color32[] px = new Color32[PlayW * PlayH];
			for (int y = 0; y < PlayH; y++)
			{
				float cy = y + 0.5f, v = cy / PlayH;                              // 0 bottom .. 1 top
				for (int x = 0; x < PlayW; x++)
				{
					float cx = x + 0.5f, u = cx / PlayW, lit, rest;
					float d = CutDistance(cx, cy, PlayW, PlayH, PlayTL, PlayTR, PlayBR, PlayBL, out lit, out rest);
					if (d < -0.5f) { Set(ref px[y * PlayW + x], 0, 0, 0, 0); continue; }
					float a = Math.Min(1f, d + 0.5f);
					float t = Mathf.Clamp01(0.65f * u + 0.35f * (1f - v));            // top-left -> right/bottom
					float r = Mathf.Lerp(205f, 95f, t), g = Mathf.Lerp(25f, 5f, t), b = Mathf.Lerp(215f, 160f, t);
					float s = 0f;                                                      // diagonal lighter sweep
					for (int i = 0; i < PlayStreaks.Length; i++)
					{
						float k = Math.Abs((u - (1f - v) * 0.16f) - PlayStreaks[i]);
						s += Math.Max(0f, 1f - k / (i == 0 ? 0.085f : 0.16f)) * (i == 0 ? 0.55f : 0.22f);
					}
					r += 58f * s; g += 42f * s; b += 60f * s;
					if (u < 0.40f)                                                     // grunge scratches, left third
					{
						float k = 0f;
						for (int i = 0; i < scratches.Count; i++)
						{
							Stroke st = scratches[i];
							float dist = SegmentDistance(cx, cy, st) - st.w * 0.5f;
							if (dist < 0.5f) k = Math.Max(k, Mathf.Clamp01(0.5f - dist) * (i % 2 == 0 ? 0.18f : -0.14f));
						}
						r += 110f * k; g += 90f * k; b += 110f * k;
					}
					if (CutEdge(cx, PlayH - cy, PlayTL) < 2f || cy < 2f) { r = 235f; g = 150f; b = 255f; }   // lit TL cut + bottom edge line
					Set(ref px[y * PlayW + x], (int)r, (int)g, (int)b, (int)(255f * a));
				}
			}
			s_playPlate = ExactSprite(PlayW, PlayH, px);
			return s_playPlate;
		}

		// Soft magenta glow around the plate: alpha 0.35 at the edge falling off over PlayGlowPad px; the
		// inside stays filled so no seam shows under the plate's anti-aliased edge.
		public static Sprite PlayGlowSprite()
		{
			if (s_playGlow != null) return s_playGlow;
			int w = PlayW + 2 * PlayGlowPad, h = PlayH + 2 * PlayGlowPad;
			Color32[] px = new Color32[w * h];
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
				{
					float lit, rest;
					float d = CutDistance(x + 0.5f - PlayGlowPad, y + 0.5f - PlayGlowPad, PlayW, PlayH, PlayTL, PlayTR, PlayBR, PlayBL, out lit, out rest);
					float k = Mathf.Clamp01(1f + d / 24f);
					Set(ref px[y * w + x], 215, 40, 230, (int)(255f * 0.35f * k * k));
				}
			s_playGlow = ExactSprite(w, h, px);
			return s_playGlow;
		}

		// ---- icons: strokes and rings rasterised with anti-aliasing into 64 px sprites ----
		private struct Stroke { public float ax, ay, bx, by, w; }
		private struct Ring { public float cx, cy, r, w; public bool fill; }
		private static readonly Dictionary<string, Sprite> s_icons = new Dictionary<string, Sprite>();
		private const int IconTex = 64;

		private static void Path(List<Stroke> strokes, float w, params float[] p)
		{
			for (int i = 0; i + 3 < p.Length; i += 2)
				strokes.Add(new Stroke { ax = p[i], ay = p[i + 1], bx = p[i + 2], by = p[i + 3], w = w });
		}

		private static void Arc(List<Stroke> strokes, float w, float cx, float cy, float r, float a0, float a1, int steps = 10)
		{
			float px = 0f, py = 0f;
			for (int i = 0; i <= steps; i++)
			{
				float a = Mathf.Deg2Rad * Mathf.Lerp(a0, a1, i / (float)steps);
				float x = cx + r * Mathf.Cos(a), y = cy - r * Mathf.Sin(a);
				if (i > 0) strokes.Add(new Stroke { ax = px, ay = py, bx = x, by = y, w = w });
				px = x; py = y;
			}
		}

		private static float SegmentDistance(float x, float y, Stroke s)
		{
			float dx = s.bx - s.ax, dy = s.by - s.ay, len = dx * dx + dy * dy;
			float t = len > 0f ? Mathf.Clamp01(((x - s.ax) * dx + (y - s.ay) * dy) / len) : 0f;
			float qx = s.ax + dx * t - x, qy = s.ay + dy * t - y;
			return Mathf.Sqrt(qx * qx + qy * qy);
		}

		// Coordinates are given in a 64 x 64 space with y pointing down (like a screen).
		private static void IconShape(string name, List<Stroke> s, List<Ring> rings)
		{
			const float w = 4.2f;
			switch (name)
			{
				case "garage":
					Path(s, w, 8, 30, 32, 11, 56, 30);                         // roof
					Path(s, w, 13, 27, 13, 54, 51, 54, 51, 27);                // walls
					Path(s, w - 1f, 23, 54, 23, 38, 41, 38, 41, 54);           // door
					Path(s, w - 1.4f, 26, 46, 38, 46);                         // slat
					break;
				case "drive":
					Path(s, 7f, 20, 50, 30, 14);
					Path(s, 7f, 36, 50, 46, 14);
					break;
				case "profile":
					rings.Add(new Ring { cx = 32, cy = 22, r = 9.5f, w = w });
					Arc(s, w, 32, 62, 20f, 20f, 160f);                           // shoulders
					break;
				case "clock":
					/* [user request 2026-09-22: lobby mockup] the dial fills the 64 space (r 28 + a 6-unit stroke = 2 px at
					   the 22 px goal-card size) so the glyph reads 22 px, not a 16 px speck with a 1 px stroke.
					   original: rings.Add(new Ring { cx = 32, cy = 32, r = 21f, w = w }); Path(s, w, 32, 32, 32, 18);
					   Path(s, w, 32, 32, 42, 38); */
					rings.Add(new Ring { cx = 32, cy = 32, r = 28f, w = 6f });
					Path(s, 6f, 32, 32, 32, 16);
					Path(s, 6f, 32, 32, 43, 39);
					break;
				case "car":
					Path(s, w, 8, 42, 8, 32, 16, 30, 22, 20, 42, 20, 50, 30, 56, 32, 56, 42, 8, 42);
					rings.Add(new Ring { cx = 18, cy = 43, r = 5f, w = w, fill = true });
					rings.Add(new Ring { cx = 46, cy = 43, r = 5f, w = w, fill = true });
					break;
				case "road":
					/* [user request 2026-09-22: lobby mockup] the road spans the full 64 space (y 3..61) so the goal-row
					   glyph is ~52 px tall at its 52 px size instead of 44; same stroke weight.
					   original: Path(s, w, 12, 56, 20, 32, 27, 8); Path(s, w, 52, 56, 44, 32, 37, 8);
					   Path(s, w - 1f, 32, 52, 32, 44); Path(s, w - 1f, 32, 36, 32, 28); Path(s, w - 1f, 32, 20, 32, 13); */
					Path(s, w, 8, 61, 17.5f, 32, 26, 3);
					Path(s, w, 56, 61, 46.5f, 32, 38, 3);
					Path(s, w - 1f, 32, 56, 32, 47);
					Path(s, w - 1f, 32, 37, 32, 27);
					Path(s, w - 1f, 32, 17, 32, 8);
					break;
				case "case":
					Path(s, w, 10, 27, 54, 27, 54, 52, 10, 52, 10, 27);
					Path(s, w, 12, 27, 16, 15, 48, 15, 52, 27);
					Path(s, w - 1f, 27, 34, 37, 34, 37, 41, 27, 41, 27, 34);
					break;
				case "chevron-left":
					Path(s, 5f, 38, 14, 22, 32, 38, 50);
					break;
				case "chevron-right":
					Path(s, 5f, 26, 14, 42, 32, 26, 50);
					break;
				case "plus":
					Path(s, 5f, 32, 17, 32, 47);
					Path(s, 5f, 17, 32, 47, 32);
					break;
				case "lock":
					/* [user request 2026-09-22: lobby mockup] padlock filling the 64 space (body x 8..56, y 30..60, shackle
					   r 16) so the LOCKED plate's glyph reads ~32x27 px with a 2 px stroke at its 34 px size.
					   original: Path(s, w, 16, 30, 48, 30, 48, 54, 16, 54, 16, 30); Arc(s, w, 32, 30, 10f, 0f, 180f);
					   rings.Add(new Ring { cx = 32, cy = 42, r = 3f, w = w, fill = true }); */
					Path(s, w, 8, 30, 56, 30, 56, 60, 8, 60, 8, 30);
					Arc(s, w, 32, 30, 16f, 0f, 180f);
					rings.Add(new Ring { cx = 32, cy = 45, r = 3.5f, w = w, fill = true });
					break;
			}
		}

		public static Sprite Icon(string name)
		{
			Sprite cached;
			if (s_icons.TryGetValue(name, out cached) && cached != null) return cached;
			List<Stroke> strokes = new List<Stroke>();
			List<Ring> rings = new List<Ring>();
			IconShape(name, strokes, rings);
			Texture2D texture = new Texture2D(IconTex, IconTex, TextureFormat.RGBA32, false);
			Color32[] px = new Color32[IconTex * IconTex];
			for (int ty = 0; ty < IconTex; ty++)
			{
				float y = IconTex - 1 - ty + 0.5f;   // screen-space y (down) -> texture row (up)
				for (int x = 0; x < IconTex; x++)
				{
					float cx = x + 0.5f, alpha = 0f;
					for (int i = 0; i < strokes.Count; i++)
						alpha = Math.Max(alpha, Mathf.Clamp01(strokes[i].w * 0.5f + 0.5f - SegmentDistance(cx, y, strokes[i])));
					for (int i = 0; i < rings.Count; i++)
					{
						Ring ring = rings[i];
						float dist = Mathf.Sqrt((cx - ring.cx) * (cx - ring.cx) + (y - ring.cy) * (y - ring.cy));
						float d = ring.fill ? ring.r - dist : ring.w * 0.5f - Math.Abs(dist - ring.r);
						alpha = Math.Max(alpha, Mathf.Clamp01(d + 0.5f));
					}
					Set(ref px[ty * IconTex + x], 255, 255, 255, (int)(255f * alpha));
				}
			}
			texture.SetPixels32(px);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;
			Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, IconTex, IconTex), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
			s_icons[name] = sprite;
			return sprite;
		}

		public static Image IconImage(ShadowRunPrototype game, Transform parent, string name, string icon, float x, float y, float size, uint color)
		{
			Image image = Block(game, parent, name, x, y, size, size, color);
			image.sprite = Icon(icon);
			image.type = Image.Type.Simple;
			image.preserveAspect = true;
			return image;
		}

		// A square panel with an icon in it (the map arrows).
		/* [user request 2026-09-22: lobby mockup] darker (22,22,28) fill, steel rim, the cuts given per button
		   (the outer corner follows the bar's corner). original: Image panel = Panel(game, parent, name, x, y,
		   width, height); panel.sprite = InfernoUi.ChamferLiftSprite(InfernoUi.ChamferFor(width, height)); */
		private static Button IconButton(ShadowRunPrototype game, Transform parent, string name, string icon, float x, float y, float width, float height, int index, int tl, int tr, int br, int bl)
		{
			Image panel = SteelBox(game, parent, name, x, y, width, height, tl, tr, br, bl, Look.Lift);
			Button button = ButtonOn(panel);
			IconImage(game, panel.transform, "Icon", icon, 0f, -height / 2f, 40f, 0xFFFFFF).raycastTarget = false;
			Connect(game, button, index);
			return button;
		}

		// Text glow: a soft neon outline plus a drop shadow, both scaled with the text alpha.
		public static void Glow(Text text, uint color, float alpha, float distance)
		{
			Outline outline = text.gameObject.AddComponent<Outline>();
			outline.effectColor = Hex(color, alpha);
			outline.effectDistance = new Vector2(distance, -distance);
			outline.useGraphicAlpha = true;
			Shadow shadow = text.gameObject.AddComponent<Shadow>();
			shadow.effectColor = Hex(color, alpha * 0.6f);
			shadow.effectDistance = new Vector2(0f, -distance * 1.6f);
			shadow.useGraphicAlpha = true;
		}

		// ---- the two cards -----------------------------------------------------------------
		private static Text s_caseTitle;
		private static Image s_playHalo;

		private static void CaseCard(ShadowRunPrototype game, Transform parent)
		{
			/* [user request 2026-09-22: lobby mockup] card at canvas x 40..396, y 1113..1596 (page units = canvas px,
			   see Build: centre (cx - 540, 16 - cy)); cuts TL 18 / BR 18 / TR 0 / BL 0 ([user request 2026-09-27: lobby
			   corners], was 37 / 35 / 6 / 6). Inside (x from the card's
			   centre 218, y from its top 1113): "///" at x 58, the render centred at y 1300, the name at y 1462,
			   the caption at y 1492 and the outlined OPEN CASE at x 63..373, y 1516..1576.
			   original: Panel(game, parent, "Cases", -320f, -1290f, 320f, 500f) */
			/* [user request 2026-09-27: lobby corners] squarer card: the two big 45-degree slices halved (37/35 -> 18) and the
			   small TR/BL cuts squared (6 -> 0), so the card reads as a rectangle with one 18 px facet at TL and BR (18 is the
			   LockPlate's cut). The square OPEN CASE key's BR corner now sits (43-18)/sqrt2 = 17.7 px from the card's diagonal
			   (was 5.7 px, a visible pinch) and the "///" mark clears the TL cut by ~12 px (its rect poked 1.4 px into the 37 cut).
			   round 1 (2026-09-22): 37, 6, 35, 6 */
			Image panel = SteelBox(game, parent, "Cases", -322f, -1338.5f, 356f, 483f, 18, 0, 18, 0);
			Button button = ButtonOn(panel);
			Connect(game, button, 5);   // ShowCaseOverlay
			RectTransform caseRoot = panel.rectTransform;
			game.homeCaseArtRect = caseRoot;
			Label(game, caseRoot, "Corner", "///", -130f, -28f, 60f, 22f, 16, MockMark, 3);   // original: -122f, -28f, ..., Steel
			// The game's own 3D case render (the texture the case screen draws); bound in HomePreview().
			RectTransform artRect = Root(caseRoot, "CaseArt");
			s_caseArt = artRect.gameObject.AddComponent<RawImage>();
			s_caseArtBound = false;
			// 320x230 rect (x 58..378, y 1185..1415) cropped so the crate body itself reads ~272 px wide, centred at y 1300
			Place(artRect, 0f, -187f, 320f, 230f);   // original: 0f, -186f, 262f, 190f; round 1: 0f, -187f, 285f, 230f
			CaseArtCrop(s_caseArt, 320f, 230f, 272f);
			s_caseArt.raycastTarget = false;
			s_caseTitle = Label(game, caseRoot, "Label", "N SERIES CASE", 0f, -349f, 320f, 36f, 27, 0xFFFFFF);   // original: 0f, -334f, 292f, 40f, 26, Pearl
			s_caseTitle.fontStyle = FontStyle.BoldAndItalic;
			// RefreshCard writes the caption plain, so its ~0.25 em tracking comes from InfernoTracking (3.5 px at 14 px)
			InfernoDailyCases.BindCard(Tracked(SpacedLabel(game, caseRoot, "CaseSubtitle", "", 0f, -379f, 320f, 22f, 14, MockGreyLilac), 3.5f));   // original: Label(..., 0f, -370f, 292f, 26f, 17, Lilac)
			// [user request 2026-09-27: square lobby buttons] square corners (no cuts); original: cuts 12, 6, 12, 6
			Image open = SteelBox(game, caseRoot, "OpenCase", 0f, -433f, 310f, 60f, 0, 0, 0, 0, Look.Outline);   // decorative: the whole card is the button; original: OutlineBox(..., 0f, -440f, 272f, 60f)
			SpacedLabel(game, open.transform, "Label", "OPEN CASE", 0f, -30f, 290f, 40f, 22, 0xFFFFFF);   // original: Label(..., 250f, 40f, 22, Pearl)
		}

		private static readonly Image[] s_goalFill = new Image[3];
		private static readonly Image[] s_goalGo = new Image[3];
		private static readonly Image[] s_goalIcon = new Image[3];
		private static readonly string[] GoalIcons = { "road", "road", "drive", "road", "car", "case" };   // by DailyGoalKind: distance, runs, boosts, drift, cars passed, cases

		/* [user request 2026-09-22: lobby mockup] row centres / dividers in card units (y from the card's top,
		   canvas 1113): rows at canvas y 1274 / 1392 / 1513, dividers at 1335 / 1455. */
		private static readonly float[] GoalRowY = { -161f, -279f, -400f };
		private static readonly float[] GoalDividerY = { -222f, -342f };
		private const float GoalBarWidth = 220f;   // original: 200f

		private static void GoalCard(ShadowRunPrototype game, Transform parent)
		{
			/* [user request 2026-09-22: lobby mockup] card at canvas x 425..1040, y 1113..1596; cuts TL/BR 18, TR/BL 0
			   ([user request 2026-09-27: lobby corners] same absolute cuts as the case card beside it so the two cards on one
			   baseline read as one kit; round 1 was TL/BR 46, TR/BL 8).
			   Inside (x from the card's centre 732.5, y from its top): title left at x 461, clock + timer ending at
			   x 1004, the rule at y 1205; per row a bare 52 px line-art icon centred at x 486, title / progress /
			   bar left at x 549, the reward gem at x 817 with "+N" from x 838 and the 107x60 outlined button at
			   x 905..1012. original: Panel(game, parent, "DailyGoals", 170f, -1290f, 620f, 500f) */
			Image card = SteelBox(game, parent, "DailyGoals", 192.5f, -1338.5f, 615f, 483f, 18, 0, 18, 0);   /* [user request 2026-09-27: lobby corners] round 1: 46, 8, 46, 8 */
			RectTransform root = card.rectTransform;
			game.homeGoalsArtRect = root;
			Text title = Label(game, root, "Title", "DAILY GOALS", -101.5f, -56f, 340f, 44f, 34, 0xFFFFFF, 3);   // original: -150f, -48f, 300f, 42f, 30, Pearl
			title.fontStyle = FontStyle.BoldAndItalic;
			/* [user request 2026-09-21: no overlaps] clock 176 → 170 and timer 246 → 250: the 24 px icon (158..182) and the
			   left-aligned timer (190..310) were touching by 2 px (layout audit) */
			// [user request 2026-09-22: lobby mockup] 22 px clock at x 878..900 (centre 889), the timer right-aligned in 904..1004
			// with 2 px tracking (8 glyphs: ~95 px, starting ≈909 — 0.25 em would run into the clock)
			IconImage(game, root, "Clock", "clock", 156.5f, -56f, 22f, MockGreyLilac);   // original: 170f, -48f, 24f, Steel; round 1: 151.5f
			game.dailyGoalsTimerText = Tracked(Label(game, root, "Timer", "", 221.5f, -56f, 100f, 30f, 20, MockGrey, 5), 2f);   // original: 250f, -48f, 120f, 34f, 22, Pearl, 3
			Block(game, root, "TitleRule", 0f, -92f, 543f, 2f, MockRule);   // original: 0f, -86f, 564f, 2f, Smoke
			HomeGoalData[] goals = game.PickDailyGoals(3);
			for (int i = 0; i < 3; i++)
			{
				HomeGoalData data = goals[i];
				float y = GoalRowY[i];   // original: -128f - i * 128f
				/* [user request 2026-09-22: lobby mockup] bare line-art glyph, no tile behind it.
				   original: Image tile = Block(game, root, "GoalTile", -258f, y - 18f, 66f, 66f, 0xFFFFFF); tile.sprite =
				   InfernoUi.ChamferLiftSprite(InfernoUi.Chamfer.Small); tile.type = Image.Type.Sliced;
				   original: s_goalIcon[i] = IconImage(game, root, "GoalIcon", GoalIcons[(int)data.Kind], -258f, y - 18f, 40f, Pearl); */
				s_goalIcon[i] = IconImage(game, root, "GoalIcon", GoalIcons[(int)data.Kind], -246.5f, y, 52f, MockIcon);
				Text titleText = Label(game, root, "GoalTitle", data.Title, -58f, y + 23f, 251f, 32f, 25, 0xFFFFFF, 3);   // original: -40f, y, 330f, 32f, 23, Pearl
				Text progress = Label(game, root, "GoalProgress", data.Progress, -58f, y - 6f, 251f, 26f, 20, MockGreyLilac, 3);   // original: -40f, y - 30f, 330f, 24f, 18, Lilac
				// progress bar under the text (fill from HomeGoalData.Fill): x 549..769, 7 px, at row + 32
				Block(game, root, "GoalTrack", -73.5f, y - 32f, GoalBarWidth, 7f, MockRule);   // original: -105f, y - 54f, 200f, 6f, Smoke
				Image fill = Block(game, root, "GoalFill", -183.5f, y - 32f, GoalBarWidth, 7f, MockBar);   // original: -205f, y - 54f, 200f, 6f, Neon
				fill.rectTransform.pivot = new Vector2(0f, 0.5f);
				fill.rectTransform.anchoredPosition = new Vector2(-183.5f, y - 32f);
				s_goalFill[i] = fill;
				// reward chip: crystal + "+N"
				Image rewardGem = Block(game, root, "GoalRewardGem", 84.5f, y, 25f, 25f, 0xFFFFFF);   // original: 122f, y - 16f, 26f, 26f
				rewardGem.sprite = GemSprite(game);
				rewardGem.preserveAspect = true;
				s_goalReward[i] = Label(game, root, "GoalReward", "+" + GoalReward((int)data.Kind), 135.5f, y, 60f, 28f, 22, 0xFFFFFF, 3);   // original: 164f, y - 16f, 56f, 26f, 21, Blossom
				// CLAIM (complete, unclaimed) / DONE (claimed) / GO (in progress): the same outlined steel box
				// [user request 2026-09-27: square lobby buttons] square corners (no cuts) on all three; original: cuts 12, 6, 12, 6
				/* original: Image image = Block(game, root, "CollectGoal", 250f, y - 16f, 104f, 50f, 0xFFFFFF); image.sprite =
				   InfernoUi.ChamferFrameSprite(InfernoUi.Chamfer.Small); image.type = Image.Type.Sliced; (violet contour) */
				Image image = SteelBox(game, root, "CollectGoal", 226f, y, 107f, 60f, 0, 0, 0, 0, Look.Outline);
				Button button = ButtonOn(image);
				SpacedLabel(game, image.transform, "Label", "CLAIM", 0f, -30f, 100f, 40f, 18, 0xFFFFFF);   // original: Label(..., 0f, -25f, 96f, 36f, 20, NeonSoft)
				if (i == 0) Connect(game, button, 7);
				if (i == 1) Connect(game, button, 8);
				if (i == 2) Connect(game, button, 9);
				Image claimed = SteelBox(game, root, "GoalClaimed", 226f, y, 107f, 60f, 0, 0, 0, 0, Look.Outline);   // original: OutlineBox(..., 250f, y - 16f, 104f, 50f)
				SpacedLabel(game, claimed.transform, "Label", "DONE", 0f, -30f, 100f, 40f, 18, MockGrey);   // original: Label(..., 0f, -25f, 96f, 36f, 20, Steel)
				Image go = SteelBox(game, root, "GoalGo", 226f, y, 107f, 60f, 0, 0, 0, 0, Look.Outline);   // original: OutlineBox(..., 250f, y - 16f, 104f, 50f)
				SpacedLabel(game, go.transform, "Label", "GO", 0f, -30f, 100f, 40f, 18, 0xFFFFFF);   // original: Label(..., 0f, -25f, 96f, 36f, 20, Pearl)
				Button goButton = ButtonOn(go);
				int row = i;
				goButton.onClick.AddListener(() => GoalGo(game, row));
				s_goalGo[i] = go;
				game.dailyGoalTitleTexts[i] = titleText;
				game.dailyGoalProgressTexts[i] = progress;
				game.dailyGoalCollectImages[i] = image;
				game.dailyGoalClaimedImages[i] = claimed;
				bool done = game.IsDailyGoalClaimed(data.Kind);
				image.gameObject.SetActive(!done && game.IsDailyGoalComplete(data.Kind));
				claimed.gameObject.SetActive(done);
				if (i < 2) Block(game, root, "GoalDivider", 0f, GoalDividerY[i], 543f, 1f, MockDivider);   // original: 0f, y - 78f, 564f, 2f, Smoke
			}
			game.dailyGoalsDateKey = game.CurrentDailyGoalsDateKey();
			game.UpdateDailyGoalsTimer(true);
			RefreshGoalRows(game);
		}

		/* [user request 2026-09-29: GO starts the run] "when I press go in the box with cases the game should start
		   instead of case opening": the cases-goal row's GO used to open the case screen, now every GO taps the PLAY
		   plate like the other two rows. The case screen is still reached from the lobby Cases card (CaseCard:
		   Connect(game, button, 5)) and from the loss screen's case box.
		   original: // GO on a goal row: the case goal opens the case, everything else starts a run.
		   original: HomeGoalData[] goals = game.PickDailyGoals(3);
		   original: if (goals != null && row < goals.Length && goals[row] != null && goals[row].Kind == DailyGoalKind.CasesOpened) { Click(game, 5); return; } */
		// GO on a goal row starts the run, whatever the goal is. (row is kept so the call site stays untouched.)
		private static void GoalGo(ShadowRunPrototype game, int row)
		{
			if (game.primaryOverlayButton != null && game.primaryOverlayButton.gameObject.activeInHierarchy) game.primaryOverlayButton.onClick.Invoke();
		}

		// Called with RefreshGoalRewards (once a second from UpdateDailyGoalsTimer): the
		// progress bars, the icon per goal kind and the GO button's visibility.
		private static void RefreshGoalRows(ShadowRunPrototype game)
		{
			if (s_goalFill[0] == null) return;
			HomeGoalData[] goals = game.PickDailyGoals(3);
			if (goals == null) return;
			for (int i = 0; i < 3 && i < goals.Length; i++)
			{
				HomeGoalData data = goals[i];
				if (data == null) continue;
				if (s_goalFill[i] != null) s_goalFill[i].rectTransform.sizeDelta = new Vector2(Math.Max(0f, GoalBarWidth * Mathf.Clamp01(data.Fill)), 7f);   /* [user request 2026-09-22: lobby mockup] original: 200f * ..., 6f */
				if (s_goalIcon[i] != null) s_goalIcon[i].sprite = Icon(GoalIcons[Math.Max(0, Math.Min(GoalIcons.Length - 1, (int)data.Kind))]);
				bool claimed = game.IsDailyGoalClaimed(data.Kind), complete = game.IsDailyGoalComplete(data.Kind);
				if (s_goalGo[i] != null) s_goalGo[i].gameObject.SetActive(!claimed && !complete);
			}
		}

		// ---- dock (tab bar): icon over label, the active tab on a lifted plate with a neon underline ----
		private static readonly string[] TabNames = { "Garage", "Home", "Profile" };

		/* [user request 2026-09-21: 3D UI] the tabs are rhomboids (InfernoUi.MakeTab3D): slanted
		   parallelograms side by side, icon above a letter-spaced label; the active one is lighter
		   with the violet glow bar along its bottom edge. The Button sits on the object named
		   Garage / Home / Profile as before.
		   original: RectTransform tab = Root(parent, name); Place(tab, x, -92f, 320f, 184f);
		   original: Image plate = Block(game, tab, "Active", 0f, -84f, 300f, 150f, 0xFFFFFF); plate.sprite = PanelLiftSprite(); ...
		   original: Image hit = Block(game, tab, "Hit", 0f, -92f, 320f, 184f, 0x000000); hit.color = new Color(0f, 0f, 0f, 0.001f);
		   original: IconImage(game, tab, "Icon", icon, 0f, -64f, 44f, 0xFFFFFF); Label(game, tab, "Label", title, 0f, -128f, 280f, 30f, 22, Pearl);
		   original: Block(game, tab, "Underline", 0f, -156f, 200f, 4f, Neon); */
		private static void DockTab(ShadowRunPrototype game, Transform parent, string name, string title, string icon, float x, int index)
		{
			Button button = InfernoUi.MakeTab3D(game, parent, name, title, icon, x, -92f, 330f, 150f, new Vector2(0.5f, 1f));
			Connect(game, button, index);
		}

		private static void DockVisual(ShadowRunPrototype game, int selected)
		{
			if (game.menuTabBarRoot == null) return;
			for (int i = 0; i < 3; i++)
			{
				Transform tab = Find(game.menuTabBarRoot, TabNames[i]);
				if (tab == null) continue;
				Button button = tab.GetComponent<Button>();
				if (button != null) InfernoUi.SetTabActive(button, i == selected);
			}
		}

		// ---- gem counter: dark panel + a "+" segment that opens the shop --------------------
		private static bool s_gemStyled;

		private static void StyleGemCounter(ShadowRunPrototype game)
		{
			if (s_gemStyled || game.gemCounterRect == null) return;
			s_gemStyled = true;
			/* [user request 2026-09-22: lobby mockup] the counter is now built in its final steel look by
			   InfernoLobby.Gems (283x84 box, crystal, count, separate 66x66 "+" key), so there is nothing left
			   to restyle here; kept as the once-only hook Show() calls.
			   original:
			   Image frame = game.gemCounterRect.GetComponent<Image>();
			   // [user request 2026-09-21: 3D UI] chamfered counter frame (medium cut for its 100 px height)
			   // with a separate chamfered "+" key at its right end that opens the shop.
			   // original: frame.sprite = PanelSprite(); plus at x 116, 60x66, PanelLiftSprite()
			   if (frame != null) { frame.sprite = InfernoUi.ChamferFrameSprite(InfernoUi.Chamfer.Medium); frame.type = Image.Type.Sliced; frame.color = Color.white; }
			   if (game.gemAmountText != null)
			   {
			       RectTransform amount = game.gemAmountText.rectTransform;
			       amount.anchoredPosition = new Vector2(20f, -50f);
			       amount.sizeDelta = new Vector2(116f, 66f);
			   }
			   Image plus = Block(game, game.gemCounterRect, "Plus", 112f, -50f, 62f, 62f, 0xFFFFFF);
			   plus.sprite = InfernoUi.ChamferLiftSprite(InfernoUi.Chamfer.Small);
			   plus.type = Image.Type.Sliced;
			   Button button = ButtonOn(plus);
			   Connect(game, button, 6);   // ShowGemShop
			   IconImage(game, plus.transform, "Icon", "plus", 0f, -31f, 26f, 0xFFFFFF).raycastTarget = false; */
		}
	}
}

/* [user request 2026-09-22: lobby mockup] Letter tracking for uGUI Text (see InfernoLobby.Tracked): Text emits one
   quad (4 vertices) per character in string order, so each character's quad is shifted by `spacing` times its
   index on its line, and the line is shifted back by the label's horizontal alignment (left 0, centre 1/2, right 1)
   so the anchored edge stays put. Added by code only (AddComponent), never through the inspector. */
internal sealed class InfernoTracking : BaseMeshEffect
{
	private float m_Spacing;

	public float spacing
	{
		get { return m_Spacing; }
		set
		{
			if (Mathf.Approximately(m_Spacing, value)) return;
			m_Spacing = value;
			if (graphic != null) graphic.SetVerticesDirty();
		}
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive() || Mathf.Approximately(m_Spacing, 0f)) return;
		Text text = graphic as Text;
		if (text == null) return;
		string value = text.text;
		if (string.IsNullOrEmpty(value)) return;
		int quads = vh.currentVertCount / 4;
		float align = ((int)text.alignment % 3) * 0.5f;   // TextAnchor columns: left / centre / right
		UIVertex vertex = new UIVertex();
		int lineStart = 0;
		while (lineStart < value.Length && lineStart < quads)
		{
			int lineEnd = value.IndexOf('\n', lineStart);
			if (lineEnd < 0) lineEnd = value.Length;
			int count = lineEnd - lineStart;
			/* [user request 2026-09-22: lobby mockup] re-centre from the glyph extents, not the character count: the
			   generator does not always emit one quad per character, so counting left a centred run ~2 gaps off. */
			float before = 0f, after = 0f, minX = float.MaxValue, maxX = float.MinValue;
			for (int c = 0; c < count; c++)
			{
				int quad = lineStart + c;
				if (quad >= quads) break;
				for (int k = 0; k < 4; k++)
				{
					vh.PopulateUIVertex(ref vertex, quad * 4 + k);
					if (vertex.position.x < minX) minX = vertex.position.x;
					if (vertex.position.x > maxX) maxX = vertex.position.x;
				}
			}
			if (minX <= maxX) before = Mathf.Lerp(minX, maxX, align);   // the anchor point the alignment keeps
			minX = float.MaxValue; maxX = float.MinValue;
			for (int c = 0; c < count; c++)
			{
				int quad = lineStart + c;
				if (quad >= quads) break;
				float dx = m_Spacing * c;
				for (int k = 0; k < 4; k++)
				{
					int index = quad * 4 + k;
					vh.PopulateUIVertex(ref vertex, index);
					vertex.position.x += dx;
					if (vertex.position.x < minX) minX = vertex.position.x;
					if (vertex.position.x > maxX) maxX = vertex.position.x;
					vh.SetUIVertex(vertex, index);
				}
			}
			if (minX <= maxX) after = Mathf.Lerp(minX, maxX, align);
			float correction = before - after;
			if (correction != 0f)
			{
				for (int c = 0; c < count; c++)
				{
					int quad = lineStart + c;
					if (quad >= quads) break;
					for (int k = 0; k < 4; k++)
					{
						int index = quad * 4 + k;
						vh.PopulateUIVertex(ref vertex, index);
						vertex.position.x += correction;
						vh.SetUIVertex(vertex, index);
					}
				}
			}
			// original: float shift = -align * m_Spacing * (count - 1); dx = shift + m_Spacing * c;
			lineStart = lineEnd + 1;   // the newline character owns a quad of its own
		}
	}
}
