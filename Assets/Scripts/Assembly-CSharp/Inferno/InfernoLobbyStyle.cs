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

		public static Sprite PanelSprite() { if (s_panel == null) s_panel = MakePanelSprite(PanelFill, PanelRim, 250, 2f); return s_panel; }
		public static Sprite PanelLiftSprite() { if (s_panelLift == null) s_panelLift = MakePanelSprite(PanelLift, 0x4A4456, 255, 2f); return s_panelLift; }
		public static Sprite OutlineSprite() { if (s_outline == null) s_outline = MakePanelSprite(0x14111B, 0x6A6478, 150, 2.5f); return s_outline; }

		public static Image Panel(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height)
		{
			Image image = Block(game, parent, name, x, y, width, height, 0xFFFFFF);
			image.sprite = PanelSprite();
			image.type = Image.Type.Sliced;
			return image;
		}

		public static Image OutlineBox(ShadowRunPrototype game, Transform parent, string name, float x, float y, float width, float height)
		{
			Image image = Block(game, parent, name, x, y, width, height, 0xFFFFFF);
			image.sprite = OutlineSprite();
			image.type = Image.Type.Sliced;
			return image;
		}

		// ---- PLAY plate: a right-leaning parallelogram, magenta gradient with light streaks ----
		private const int PlayW = 512, PlayH = 112, PlaySlant = 26;
		private static readonly float[] PlayStreaks = { 0.16f, 0.30f, 0.36f, 0.64f, 0.72f, 0.88f };

		public static Sprite PlayPlateSprite()
		{
			if (s_playPlate != null) return s_playPlate;
			Texture2D texture = new Texture2D(PlayW, PlayH, TextureFormat.RGBA32, false);
			Color32[] px = new Color32[PlayW * PlayH];
			for (int y = 0; y < PlayH; y++)
			{
				float v = y / (float)(PlayH - 1);                         // 0 bottom .. 1 top
				float xl = PlaySlant * v, xr = PlayW - PlaySlant * (1f - v); // top edge shifted right: leans like italics
				for (int x = 0; x < PlayW; x++)
				{
					float cx = x + 0.5f, cy = y + 0.5f;
					float d = Math.Min(Math.Min((cx - xl) * 0.97f, (xr - cx) * 0.97f), Math.Min(cy, PlayH - cy));
					float a = Math.Max(0f, Math.Min(1f, d));
					float u = x / (float)(PlayW - 1);
					float r = Mathf.Lerp(158f, 232f, u) + 18f * v, g = Mathf.Lerp(12f, 44f, u) + 12f * v, b = Mathf.Lerp(196f, 255f, u);
					float s = 0f;
					for (int i = 0; i < PlayStreaks.Length; i++)
					{
						float t = Math.Abs((u - v * 0.10f) - PlayStreaks[i]);
						s += Math.Max(0f, 1f - t / 0.028f) * (i % 2 == 0 ? 0.42f : 0.26f);
					}
					r += 70f * s; g += 110f * s; b += 30f * s;
					if (d < 2.2f) { r = 255f; g = 176f; b = 255f; }   // bright rim
					Set(ref px[y * PlayW + x], (int)r, (int)g, (int)b, (int)(255f * a));
				}
			}
			texture.SetPixels32(px);
			texture.Apply();
			texture.wrapMode = TextureWrapMode.Clamp;
			s_playPlate = Sprite.Create(texture, new Rect(0f, 0f, PlayW, PlayH), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
			return s_playPlate;
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
					rings.Add(new Ring { cx = 32, cy = 32, r = 21f, w = w });
					Path(s, w, 32, 32, 32, 18);
					Path(s, w, 32, 32, 42, 38);
					break;
				case "car":
					Path(s, w, 8, 42, 8, 32, 16, 30, 22, 20, 42, 20, 50, 30, 56, 32, 56, 42, 8, 42);
					rings.Add(new Ring { cx = 18, cy = 43, r = 5f, w = w, fill = true });
					rings.Add(new Ring { cx = 46, cy = 43, r = 5f, w = w, fill = true });
					break;
				case "road":
					Path(s, w, 12, 56, 20, 32, 27, 8);
					Path(s, w, 52, 56, 44, 32, 37, 8);
					Path(s, w - 1f, 32, 52, 32, 44);
					Path(s, w - 1f, 32, 36, 32, 28);
					Path(s, w - 1f, 32, 20, 32, 13);
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
					Path(s, w, 16, 30, 48, 30, 48, 54, 16, 54, 16, 30);
					Arc(s, w, 32, 30, 10f, 0f, 180f);
					rings.Add(new Ring { cx = 32, cy = 42, r = 3f, w = w, fill = true });
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
		private static Button IconButton(ShadowRunPrototype game, Transform parent, string name, string icon, float x, float y, float width, float height, int index)
		{
			Image panel = Panel(game, parent, name, x, y, width, height);
			panel.sprite = PanelLiftSprite();
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
			Image panel = Panel(game, parent, "Cases", -320f, -1290f, 320f, 500f);
			Button button = ButtonOn(panel);
			Connect(game, button, 5);   // ShowCaseOverlay
			RectTransform caseRoot = panel.rectTransform;
			game.homeCaseArtRect = caseRoot;
			Label(game, caseRoot, "Corner", "///", -122f, -28f, 60f, 22f, 16, Steel, 3);
			// The game's own 3D case render (the texture the case screen draws); bound in HomePreview().
			RectTransform artRect = Root(caseRoot, "CaseArt");
			s_caseArt = artRect.gameObject.AddComponent<RawImage>();
			s_caseArtBound = false;
			Place(artRect, 0f, -186f, 262f, 190f);
			s_caseArt.raycastTarget = false;
			s_caseTitle = Label(game, caseRoot, "Label", "N SERIES CASE", 0f, -334f, 292f, 40f, 26, Pearl);
			s_caseTitle.fontStyle = FontStyle.BoldAndItalic;
			InfernoDailyCases.BindCard(Label(game, caseRoot, "CaseSubtitle", "", 0f, -370f, 292f, 26f, 17, Lilac));
			Image open = OutlineBox(game, caseRoot, "OpenCase", 0f, -440f, 272f, 60f);   // decorative: the whole card is the button
			Label(game, open.transform, "Label", "OPEN CASE", 0f, -30f, 250f, 40f, 22, Pearl);
		}

		private static readonly Image[] s_goalFill = new Image[3];
		private static readonly Image[] s_goalGo = new Image[3];
		private static readonly Image[] s_goalIcon = new Image[3];
		private static readonly string[] GoalIcons = { "road", "road", "drive", "road", "car", "case" };   // by DailyGoalKind: distance, runs, boosts, drift, cars passed, cases

		private static void GoalCard(ShadowRunPrototype game, Transform parent)
		{
			Image card = Panel(game, parent, "DailyGoals", 170f, -1290f, 620f, 500f);
			RectTransform root = card.rectTransform;
			game.homeGoalsArtRect = root;
			Text title = Label(game, root, "Title", "DAILY GOALS", -150f, -48f, 300f, 42f, 30, Pearl, 3);
			title.fontStyle = FontStyle.BoldAndItalic;
			IconImage(game, root, "Clock", "clock", 176f, -48f, 24f, Steel);
			game.dailyGoalsTimerText = Label(game, root, "Timer", "", 246f, -48f, 120f, 34f, 22, Pearl, 3);
			Block(game, root, "TitleRule", 0f, -86f, 564f, 2f, Smoke);
			HomeGoalData[] goals = game.PickDailyGoals(3);
			for (int i = 0; i < 3; i++)
			{
				HomeGoalData data = goals[i];
				float y = -128f - i * 128f;
				s_goalIcon[i] = IconImage(game, root, "GoalIcon", GoalIcons[(int)data.Kind], -258f, y - 18f, 42f, Pearl);
				Text titleText = Label(game, root, "GoalTitle", data.Title, -40f, y, 330f, 32f, 23, Pearl, 3);
				Text progress = Label(game, root, "GoalProgress", data.Progress, -40f, y - 30f, 330f, 24f, 18, Lilac, 3);
				// progress bar under the text (fill from HomeGoalData.Fill)
				Block(game, root, "GoalTrack", -105f, y - 54f, 200f, 6f, Smoke);
				Image fill = Block(game, root, "GoalFill", -205f, y - 54f, 200f, 6f, Neon);
				fill.rectTransform.pivot = new Vector2(0f, 0.5f);
				fill.rectTransform.anchoredPosition = new Vector2(-205f, y - 54f);
				s_goalFill[i] = fill;
				// reward chip: crystal + "+N"
				Image rewardGem = Block(game, root, "GoalRewardGem", 122f, y - 16f, 26f, 26f, 0xFFFFFF);
				rewardGem.sprite = GemSprite(game);
				rewardGem.preserveAspect = true;
				s_goalReward[i] = Label(game, root, "GoalReward", "+" + GoalReward((int)data.Kind), 164f, y - 16f, 56f, 26f, 21, Blossom, 3);
				// CLAIM (complete, unclaimed) / DONE (claimed) / GO (in progress)
				Image image = Block(game, root, "CollectGoal", 250f, y - 16f, 104f, 50f, 0xFFFFFF);
				image.sprite = PlayPlateSprite();
				image.type = Image.Type.Simple;
				Button button = ButtonOn(image);
				Label(game, image.transform, "Label", "CLAIM", 0f, -25f, 96f, 36f, 20, 0xFFFFFF);
				if (i == 0) Connect(game, button, 7);
				if (i == 1) Connect(game, button, 8);
				if (i == 2) Connect(game, button, 9);
				Image claimed = OutlineBox(game, root, "GoalClaimed", 250f, y - 16f, 104f, 50f);
				Label(game, claimed.transform, "Label", "DONE", 0f, -25f, 96f, 36f, 20, Steel);
				Image go = OutlineBox(game, root, "GoalGo", 250f, y - 16f, 104f, 50f);
				Label(game, go.transform, "Label", "GO", 0f, -25f, 96f, 36f, 20, Pearl);
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
				if (i < 2) Block(game, root, "GoalDivider", 0f, y - 78f, 564f, 2f, Smoke);
			}
			game.dailyGoalsDateKey = game.CurrentDailyGoalsDateKey();
			game.UpdateDailyGoalsTimer(true);
			RefreshGoalRows(game);
		}

		// GO on a goal row: the case goal opens the case, everything else starts a run.
		private static void GoalGo(ShadowRunPrototype game, int row)
		{
			HomeGoalData[] goals = game.PickDailyGoals(3);
			if (goals != null && row < goals.Length && goals[row] != null && goals[row].Kind == DailyGoalKind.CasesOpened) { Click(game, 5); return; }
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
				if (s_goalFill[i] != null) s_goalFill[i].rectTransform.sizeDelta = new Vector2(Math.Max(0f, 200f * Mathf.Clamp01(data.Fill)), 6f);
				if (s_goalIcon[i] != null) s_goalIcon[i].sprite = Icon(GoalIcons[Math.Max(0, Math.Min(GoalIcons.Length - 1, (int)data.Kind))]);
				bool claimed = game.IsDailyGoalClaimed(data.Kind), complete = game.IsDailyGoalComplete(data.Kind);
				if (s_goalGo[i] != null) s_goalGo[i].gameObject.SetActive(!claimed && !complete);
			}
		}

		// ---- dock (tab bar): icon over label, the active tab on a lifted plate with a neon underline ----
		private static readonly string[] TabNames = { "Garage", "Home", "Profile" };

		private static void DockTab(ShadowRunPrototype game, Transform parent, string name, string title, string icon, float x, int index)
		{
			RectTransform tab = Root(parent, name);
			Place(tab, x, -92f, 320f, 184f);
			Image plate = Block(game, tab, "Active", 0f, -84f, 300f, 150f, 0xFFFFFF);
			plate.sprite = PanelLiftSprite();
			plate.type = Image.Type.Sliced;
			Image hit = Block(game, tab, "Hit", 0f, -92f, 320f, 184f, 0x000000);
			hit.color = new Color(0f, 0f, 0f, 0.001f);   // invisible, still catches taps
			Button button = ButtonOn(hit);
			Connect(game, button, index);
			IconImage(game, tab, "Icon", icon, 0f, -64f, 44f, 0xFFFFFF).raycastTarget = false;
			Label(game, tab, "Label", title, 0f, -128f, 280f, 30f, 22, Pearl).raycastTarget = false;
			Block(game, tab, "Underline", 0f, -156f, 200f, 4f, Neon).raycastTarget = false;
		}

		private static void DockVisual(ShadowRunPrototype game, int selected)
		{
			if (game.menuTabBarRoot == null) return;
			for (int i = 0; i < 3; i++)
			{
				Transform tab = Find(game.menuTabBarRoot, TabNames[i]);
				if (tab == null) continue;
				bool on = i == selected;
				Transform plate = tab.Find("Active"), underline = tab.Find("Underline"), icon = tab.Find("Icon"), label = tab.Find("Label");
				if (plate != null) plate.gameObject.SetActive(on);
				if (underline != null) underline.gameObject.SetActive(on);
				if (icon != null) Paint(icon.GetComponent<Image>(), on ? Neon : Steel);
				if (label != null) label.GetComponent<Text>().color = Hex(on ? 0xFFFFFFu : Steel);
			}
		}

		// ---- gem counter: dark panel + a "+" segment that opens the shop --------------------
		private static bool s_gemStyled;

		private static void StyleGemCounter(ShadowRunPrototype game)
		{
			if (s_gemStyled || game.gemCounterRect == null) return;
			s_gemStyled = true;
			Image frame = game.gemCounterRect.GetComponent<Image>();
			if (frame != null) { frame.sprite = PanelSprite(); frame.type = Image.Type.Sliced; frame.color = Color.white; }
			if (game.gemAmountText != null)
			{
				RectTransform amount = game.gemAmountText.rectTransform;
				amount.anchoredPosition = new Vector2(24f, -50f);
				amount.sizeDelta = new Vector2(120f, 66f);
			}
			Image plus = Block(game, game.gemCounterRect, "Plus", 116f, -50f, 60f, 66f, 0xFFFFFF);
			plus.sprite = PanelLiftSprite();
			plus.type = Image.Type.Sliced;
			Button button = ButtonOn(plus);
			Connect(game, button, 6);   // ShowGemShop
			IconImage(game, plus.transform, "Icon", "plus", 0f, -33f, 26f, 0xFFFFFF).raycastTarget = false;
		}
	}
}
