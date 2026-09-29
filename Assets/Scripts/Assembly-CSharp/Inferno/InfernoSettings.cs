using System;
using UnityEngine;
using UnityEngine.UI;

// Port of Il2CppOutput/InfernoSettings.inc: volume sliders (master, music, effects) and a vibration
// switch, laid out under the account panel that MakeSettingsScreen builds. Wired in by the hooks kept
// in ShadowRunPrototype (Update -> Tick, MakeSettingsScreen -> Build/StyleAccount, ShowSettingsScreen ->
// Refresh, LayoutProfileScreen -> Arrange/Relayout, LayoutMenuPageHeaders -> CenterHeader, ApplyGameSoundState ->
// ListenerVolume).
// [user request 2026-09-21: 3D UI] The page follows the user's settings mockup: SETTINGS heading with
// its subtitle, an ACCOUNT card (label, GUEST ACCOUNT, GUEST_ID_<8 digits>, a vertical divider and an
// avatar button that opens the auth form), an AUDIO card with three sliders whose knobs are skewed
// rhombus slabs on glowing violet tracks, and a GAME card with the VIBRATION ON/OFF pill toggle. The
// sliders are still real UnityEngine.UI.Sliders named MasterSlider / MusicSlider / EffectsSlider polled
// by Tick; the toggle keeps the VibrationSwitch name and its Button.
public sealed partial class ShadowRunPrototype
{
	internal static partial class InfernoSettings
	{
		private const int Master = 0, Music = 1, Effects = 2, SliderCount = 3;
		// original: PanelWidth = 920f, RowStep = 78f, FirstRow = -100f, TrackWidth = 450f, Inset = 50f
		private const float PanelWidth = 980f, RowStep = 84f, FirstRow = -138f, TrackWidth = 518f;
		private const float Inset = 55f;                   // inner padding of the cards (content spans -435..435)
		private const float SaveDelay = 0.6f;  // seconds after the last slider change before PlayerPrefs is written
		private const string MasterKey = "InfernoDrive.MasterVolume";
		private const string GuestIdKey = "InfernoDrive.GuestId";

		private static GameObject s_root;             // settingsRoot this UI was built for
		private static readonly Slider[] s_sliders = new Slider[SliderCount];
		private static readonly Text[] s_values = new Text[SliderCount];
		private static Button s_vibration;            // the VibrationSwitch pill toggle
		private static RectTransform s_audio, s_gameCard;   // the two settings cards (Arrange moves / hides them)
		private static int s_master = 100;
		private static bool s_masterLoaded;
		private static bool s_masterApplied;
		private static float s_dirtyFor = -1f;                // seconds since the last unsaved change; <0 when clean
		private static Sprite s_knobSprite;        // chrome ball (the kit's KnobSprite)
		private static float s_fitWidth = -1f, s_fitHeight = -1f; // overlay size the content scale was computed for
		// MakeUiBlock anchors AND pivots at the requested corner, so the account panel's
		// y (-300) is its top edge: it spans -300..-510. Everything below hangs off that.
		// Card heights from the mockup (1080-wide units): ACCOUNT 210, AUDIO 373, GAME 201, 46 px
		// gaps; the first card starts ~60 px under the heading's subtitle (which ends at ~-235).
		// original: ContentTop = 280f, ContentBottom = 1044f
		public const float ContentTop = 300f;              // account panel top (kept fixed under the page header)
		public const float AccountHeight = 210f, Gap = 46f;
		public const float AudioTop = ContentTop + AccountHeight + Gap, AudioHeight = 373f;          // 556..929
		public const float GameTop = AudioTop + AudioHeight + Gap, GameHeight = 201f;               // 975..1176
		public const float AccountActionsHeight = 390f, AuthFormHeight = 680f;
		private static float s_contentBottom = GameTop + GameHeight + 26f;   // lowest content y the page reaches (GAME section bottom + margin)

		// ---------- generated chrome knob sprite ----------
		private static float Clamp01(float v) { return v < 0f ? 0f : v > 1f ? 1f : v; }
		private static float Lerp(float a, float b, float t) { return a + (b - a) * t; }
		private static float Bell(float x, float center, float width) { float d = (x - center) / width; return (float)Math.Exp(-d * d); }

		private static Texture2D NewTexture(int width, int height)
		{
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);   // RGBA32, no mips
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;
			return texture;
		}

		private static Sprite FinishSprite(Texture2D texture, int width, int height, int border)
		{
			texture.Apply();
			Rect rect = new Rect(0f, 0f, width, height);
			Vector4 borders = new Vector4(border, border, border, border);
			return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, borders);
		}

		private static void Steel(Texture2D texture, int x, int y, float light, float alpha)
		{
			light = Clamp01(light);
			Color color = new Color(Clamp01(light * 0.95f), Clamp01(light * 0.96f), Clamp01(light * 1.02f), alpha);
			texture.SetPixel(x, y, color);
		}

		// Chrome ball: lit sphere with a specular hot spot, a dark reflected horizon and a bright rim.
		private static Sprite MakeKnobSprite(int size)
		{
			Texture2D texture = NewTexture(size, size);
			float center = size / 2.0f, radius = size * 0.48f;
			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					float nx = (x + 0.5f - center) / radius, ny = (y + 0.5f - center) / radius;
					float edge = radius - (float)Math.Sqrt(nx * nx + ny * ny) * radius;
					float alpha = Clamp01(edge + 0.5f);
					float nz = (float)Math.Sqrt(Math.Max(0f, 1f - nx * nx - ny * ny));
					float diffuse = Clamp01(nx * -0.45f + ny * 0.60f + nz * 0.66f);
					float specular = (float)Math.Pow(Clamp01(nx * -0.35f + ny * 0.55f + nz * 0.76f), 40.0) * 0.9f;
					float rim = (float)Math.Pow(1f - nz, 3.0) * 0.30f;
					float light = 0.26f + diffuse * 0.55f + specular + rim - Bell(ny, -0.15f, 0.12f) * 0.18f;
					light *= Lerp(0.30f, 1.0f, Clamp01(edge / 2.0f));
					Steel(texture, x, y, light, alpha);
				}
			}
			return FinishSprite(texture, size, size, 0);
		}

		private static void EnsureSprites()
		{
			if (s_knobSprite == null) s_knobSprite = MakeKnobSprite(96);
		}

		// The chrome ball for the kit (InfernoUi.KnobSprite).
		internal static Sprite KnobSprite()
		{
			EnsureSprites();
			return s_knobSprite;
		}

		// ---------- state ----------
		private static int Clamp100(int value) { return Math.Max(0, Math.Min(100, value)); }

		private static int MasterVolume()
		{
			if (!s_masterLoaded)
			{
				s_master = Clamp100(PlayerPrefs.GetInt(MasterKey, 100));
				s_masterLoaded = true;
			}
			return s_master;
		}

		// A stable local guest id ("GUEST_ID_11743276"): eight digits drawn once and kept in
		// PlayerPrefs, shown on the ACCOUNT card while no cloud account is signed in.
		public static string GuestId()
		{
			int id = PlayerPrefs.GetInt(GuestIdKey, 0);
			if (id < 10000000 || id > 99999999)
			{
				id = UnityEngine.Random.Range(10000000, 100000000);
				PlayerPrefs.SetInt(GuestIdKey, id);
				PlayerPrefs.Save();
			}
			return "GUEST_ID_" + id;
		}

		// AudioListener.volume = on/off switch x master slider. Called by ApplyGameSoundState.
		public static float ListenerVolume(ShadowRunPrototype game)
		{
			return game.gameSoundEnabled ? MasterVolume() / 100.0f : 0.0f;
		}

		private static void ApplyMaster(ShadowRunPrototype game)
		{
			AudioListener.volume = ListenerVolume(game);
			s_masterApplied = true;
		}

		private static bool Alive(ShadowRunPrototype game)
		{
			return s_root != null && game.settingsRoot == s_root;
		}

		private static int Value(ShadowRunPrototype game, int index)
		{
			if (index == Master) return MasterVolume();
			return Clamp100(index == Music ? game.musicVolume : game.sfxVolume);
		}

		private static void Percent(Text text, int value)
		{
			if (text == null) return;
			InfernoLobby.Caption(text, value + "%");
		}

		public static void Refresh(ShadowRunPrototype game)
		{
			if (!Alive(game)) return;
			for (int i = 0; i < SliderCount; i++)
			{
				if (s_sliders[i] != null) s_sliders[i].SetValueWithoutNotify(Value(game, i));
				Percent(s_values[i], Value(game, i));
			}
			// original: pill painted Electric / Slate, knob at +-48, "ON" / "OFF" text at -+22
			if (s_vibration != null) InfernoUi.SetToggle(s_vibration, game.vibrationOn);
		}

		private static void Save(ShadowRunPrototype game)
		{
			PlayerPrefs.SetInt(MasterKey, MasterVolume());
			game.SaveSettings();   // also writes music/sfx and calls PlayerPrefs.Save
			s_dirtyFor = -1f;
		}

		// Fits the settings content to the screen. LayoutProfileScreen (the game) insets
		// profileContentRoot by the safe-area top, reserves 190 px for the dock and
		// resets its scale to 1; the patcher calls Relayout right after it. We scale the
		// root about the account panel's top edge (ContentTop stays put under the page
		// header) so everything down to the content bottom fits above the dock.
		private static void Fit(ShadowRunPrototype game)
		{
			RectTransform root = game.profileContentRoot;
			if (root == null || game.overlayRect == null) return;
			Rect bounds = game.overlayRect.rect;
			float width = bounds.width, height = bounds.height;
			if (width <= 0f || height <= 0f || (width == s_fitWidth && height == s_fitHeight)) return;
			float screenWidth = Screen.width, screenHeight = Screen.height;
			if (screenWidth <= 0f || screenHeight <= 0f) return;
			s_fitWidth = width; s_fitHeight = height;
			Rect safe = Screen.safeArea;
			float top = Math.Max(0f, screenHeight - safe.yMin - safe.height) / screenHeight * height;
			float bottom = Math.Max(0f, safe.yMin) / screenHeight * height;
			float available = height - top - bottom - 190f;               // same box LayoutProfileScreen gives the root
			float scale = Math.Max(0.5f, Math.Min(1.0f, (available - 24f - ContentTop) / (s_contentBottom - ContentTop)));
			root.offsetMax = new Vector2(0f, -top);
			root.offsetMin = new Vector2(0f, bottom + 190f);
			root.pivot = new Vector2(0.5f, 1f);
			root.localScale = new Vector3(scale, scale, 1f);
			// shift down so content y = -ContentTop lands at the same page position at every scale
			root.anchoredPosition = new Vector2(0f, -top - ContentTop * (1f - scale));
		}

		// Called by the patcher at the end of ShadowRunPrototype.LayoutProfileScreen.
		public static void Relayout(ShadowRunPrototype game)
		{
			s_fitWidth = s_fitHeight = -1f;
			if (Alive(game)) Fit(game);
		}

		/* [user request 2026-09-21: overlaps] The page laid out from what is visible: while the
		   auth form (680 tall, under the ACCOUNT card) is open the AUDIO and GAME cards are hidden;
		   while the signed-in account card (390 tall) shows, both move down by its height plus the
		   gap (Audio -992..-1365, Game -1411..-1612; the dock limit is -1730). The content bottom
		   the Fit scale uses follows. Called from LayoutProfileScreen and whenever the profile
		   roots change (RefreshProfileUi / ShowProfileAuthForm). */
		public static void Arrange(ShadowRunPrototype game)
		{
			if (!Alive(game)) { Relayout(game); return; }
			bool form = game.profileAuthFormRoot != null && game.profileAuthFormRoot.activeSelf;
			bool account = game.profileAccountRoot != null && game.profileAccountRoot.activeSelf;
			float shift = account ? AccountActionsHeight + Gap : 0f;
			if (s_audio != null)
			{
				s_audio.gameObject.SetActive(!form);
				s_audio.anchoredPosition = new Vector2(0f, -(AudioTop + shift) - AudioHeight / 2f);
			}
			if (s_gameCard != null)
			{
				s_gameCard.gameObject.SetActive(!form);
				s_gameCard.anchoredPosition = new Vector2(0f, -(GameTop + shift) - GameHeight / 2f);
			}
			float bottom = form ? AudioTop + AuthFormHeight + 26f : GameTop + GameHeight + 26f + shift;
			if (Math.Abs(bottom - s_contentBottom) > 0.5f) { s_contentBottom = bottom; s_fitWidth = s_fitHeight = -1f; }
			Relayout(game);
		}

		// Runs every frame from ShadowRunPrototype.Update: pulls slider values into the game
		// fields (UpdateBackgroundMusic / UpdateVehicleEngineAudio read them each frame) and
		// persists shortly after the finger lifts.
		public static void Tick(ShadowRunPrototype game)
		{
			if (!s_masterApplied) ApplyMaster(game);
			if (!Alive(game)) return;
			if (!s_root.activeInHierarchy)
			{
				if (s_dirtyFor >= 0f) Save(game);
				return;
			}
			Fit(game);
			for (int i = 0; i < SliderCount; i++)
			{
				if (s_sliders[i] == null) continue;
				int value = Clamp100((int)Math.Round(s_sliders[i].value));
				if (value == Value(game, i)) continue;
				if (i == Master) { s_master = value; ApplyMaster(game); }
				else if (i == Music) game.musicVolume = value;
				else game.sfxVolume = value;
				Percent(s_values[i], value);
				s_dirtyFor = 0f;
			}
			if (s_dirtyFor >= 0f)
			{
				s_dirtyFor += Time.unscaledDeltaTime;
				if (s_dirtyFor >= SaveDelay) Save(game);
			}
		}

		// --- click callbacks
		private static void ToggleVibration(ShadowRunPrototype game)
		{
			game.vibrationOn = !game.vibrationOn;
			game.SaveSettings();
			Refresh(game);
		}

		private static void OpenAccountForm(ShadowRunPrototype game)
		{
			// the avatar key: secure the guest account (the shipped CREATE ACCOUNT action); a
			// verified account has its own actions card
			if (ShadowRun.Services.SupabaseGameManager.HasRememberedAccount && !ShadowRun.Services.SupabaseGameManager.IsRememberedGuestAccount) return;
			game.ShowProfileAuthForm(true);
		}

		private static void Hook(ShadowRunPrototype game, Button button, Action<ShadowRunPrototype> fn)
		{
			button.onClick.AddListener(() => fn(game));
		}

		// A real UnityEngine.UI.Slider (drag, tap, 0-100 whole numbers) from the kit: 64 px tall
		// hit area, recessed track, glowing violet fill, rhombus knob.
		// original: transparent host + 12 px Plum bar + Electric fill + 44 px chrome ball
		private static Slider MakeSlider(ShadowRunPrototype game, Transform parent, string name, float x, float y, int initial)
		{
			Slider slider = InfernoUi.MakeSlider3D(game, parent, name, x, y, TrackWidth, 64f, 0, 100, new Vector2(0.5f, 1f));
			slider.SetValueWithoutNotify(initial);
			return slider;
		}

		// Row geometry inside a card (x from the card centre): label -435..-235, slider track
		// -212..306 (the 52 px knob reaches 332 at 100%), value text 346..436 right-aligned.
		private static Text RowLabel(ShadowRunPrototype game, Transform parent, string name, string title, float y)
		{
			Text label = InfernoLobby.Label(game, parent, name, title, -PanelWidth / 2f + Inset + 100f, y, 200f, 40f, 27, InfernoLobby.Pearl, 3);
			label.fontStyle = FontStyle.BoldAndItalic;   // the mockup's condensed italic labels
			return label;
		}

		private static void SliderRow(ShadowRunPrototype game, Transform parent, int index, string name, string title, float y)
		{
			RowLabel(game, parent, name, title, y);
			s_sliders[index] = MakeSlider(game, parent, name, 47f, y, Value(game, index));
			s_values[index] = InfernoLobby.Label(game, parent, "Value", "", 391f, y, 90f, 40f, 27, InfernoLobby.Pearl, 5);
			s_values[index].fontStyle = FontStyle.BoldAndItalic;
			Percent(s_values[index], Value(game, index));
		}

		private static RectTransform Section(ShadowRunPrototype game, Transform parent, string name, string title, float top, float height)
		{
			Image card = InfernoLobby.NeonFrame(game, parent, name, 0f, top - height / 2f, PanelWidth, height);
			RectTransform rect = card.rectTransform;
			// original: Label "Title" at -38 (22 px Lilac) + "Rule" at -64
			InfernoUi.SectionLabel(game, rect, "Title", title, -PanelWidth / 2f + Inset + (PanelWidth - 2f * Inset) / 2f, -50f, PanelWidth - 2f * Inset);
			return rect;
		}

		// Keep the existing account text objects so authentication refreshes still update
		// them, and lay the card out like the mockup's ACCOUNT card: section label, the bold
		// italic account type, the guest id line, the status line, a vertical divider and the
		// avatar key on the right.
		public static void StyleAccount(ShadowRunPrototype game, Image panel)
		{
			if (panel == null || game.profileAccountTypeText == null || game.profileIdentityText == null) return;
			panel.sprite = InfernoLobby.NeonFrameSprite(game, PanelWidth, AccountHeight);
			panel.type = Image.Type.Sliced;
			InfernoLobby.Paint(panel, 0xFFFFFF);
			Transform root = panel.transform;
			RectTransform type = game.profileAccountTypeText.rectTransform;
			RectTransform identity = game.profileIdentityText.rectTransform;
			RectTransform status = game.profileStatusText != null ? game.profileStatusText.rectTransform : null;
			// The old accent stripe, portrait glow and controls belong to the old layout.
			int count = root.childCount;
			for (int i = 0; i < count; i++)
			{
				Transform child = root.GetChild(i);
				if (child != type && child != identity && child != status)
					child.gameObject.SetActive(false);
			}
			float left = -PanelWidth / 2f + Inset;
			float textWidth = 640f;
			InfernoUi.SectionLabel(game, root, "AccountLabel", "ACCOUNT", left + textWidth / 2f, -46f, textWidth, false);
			// original: type at (0, -64) 820x44 MiddleLeft 24..30 px; identity at (0, -120) 820x40
			InfernoLobby.Place(type, left + textWidth / 2f, -98f, textWidth, 44f);
			InfernoLobby.Place(identity, left + textWidth / 2f, -150f, textWidth, 32f);
			game.profileAccountTypeText.alignment = TextAnchor.MiddleLeft;
			game.profileAccountTypeText.fontStyle = FontStyle.BoldAndItalic;
			game.profileAccountTypeText.resizeTextForBestFit = true;
			game.profileAccountTypeText.resizeTextMinSize = 26;
			game.profileAccountTypeText.resizeTextMaxSize = 36;
			game.profileAccountTypeText.color = InfernoLobby.Hex(0xFFFFFF);
			game.profileIdentityText.alignment = TextAnchor.MiddleLeft;
			game.profileIdentityText.fontStyle = FontStyle.BoldAndItalic;
			game.profileIdentityText.resizeTextMinSize = 18;
			game.profileIdentityText.resizeTextMaxSize = 24;
			game.profileIdentityText.color = InfernoLobby.Hex(InfernoLobby.Steel);
			if (status != null)
			{
				InfernoLobby.Place(status, left + textWidth / 2f, -186f, textWidth, 24f);
				game.profileStatusText.alignment = TextAnchor.MiddleLeft;
			}
			InfernoUi.Divider(root, "AccountDivider", 331f, -105f, 110f, InfernoLobby.Smoke);
			// avatar key: the person glyph on a transparent hit area, press feedback only
			Image avatar = InfernoLobby.Block(game, root, "Profile Avatar Button", 405f, -105f, 92f, 92f, 0x000000);
			avatar.color = new Color(0f, 0f, 0f, 0.001f);   // invisible, still catches taps
			Button button = InfernoLobby.ButtonOn(avatar, false);
			InfernoLobby.IconImage(game, avatar.transform, "Icon", "profile", 0f, -46f, 56f, InfernoLobby.Steel).raycastTarget = false;
			Hook(game, button, OpenAccountForm);
		}

		// Called at the end of MakeSettingsScreen with profileContentRoot already built.
		public static void Build(ShadowRunPrototype game)
		{
			if (game.profileContentRoot == null || game.settingsRoot == null) return;
			s_root = game.settingsRoot;
			for (int i = 0; i < SliderCount; i++) { s_sliders[i] = null; s_values[i] = null; }
			s_vibration = null;
			s_fitWidth = s_fitHeight = -1f;
			Transform parent = game.profileContentRoot;

			// Layout (content y, root top = 0): account panel -300..-510 (its y is its top
			// edge), AUDIO -556..-929, GAME -975..-1176; Arrange moves the two cards when the
			// auth form or the signed-in actions card is showing. The guest LOG IN button is
			// hidden by the patcher (no login on this page).
			// original: AUDIO -500..-830 (rows at -100/-178/-256), GAME -860..-1020
			// AUDIO: master / music / effects sliders. Master multiplies with the
			// game's saved sound-enabled state (ListenerVolume).
			RectTransform audio = Section(game, parent, "AudioSection", "AUDIO", -AudioTop, AudioHeight);
			s_audio = audio;
			SliderRow(game, audio, Master, "MasterSlider", "MASTER", FirstRow);
			SliderRow(game, audio, Music, "MusicSlider", "MUSIC", FirstRow - RowStep);
			SliderRow(game, audio, Effects, "EffectsSlider", "EFFECTS", FirstRow - 2f * RowStep);

			// GAME: vibration switch (PlayCollisionHaptic/PlayImpactHaptic read vibrationOn).
			RectTransform gameCard = Section(game, parent, "GameSection", "GAME", -GameTop, GameHeight);
			s_gameCard = gameCard;
			RowLabel(game, gameCard, "VibrationLabel", "VIBRATION", -133f);
			// original: InfernoLobby.Card "VibrationSwitch" 150x56 at 335 (Slate / Grape) + "State" text + chrome "Knob"
			s_vibration = InfernoUi.MakeToggle3D(game, gameCard, "VibrationSwitch", 356f, -133f, 176f, 54f, new Vector2(0.5f, 1f));
			Hook(game, s_vibration, ToggleVibration);

			Refresh(game);
			Arrange(game);
		}

		// Garage and profile page headers. The patcher calls this from
		// LayoutMenuPageHeaders after placing each header root at a computed left
		// offset; that offset is measured from overlayRect, which can still be unsized
		// while the menu is being built, so re-anchor the root to the page's horizontal
		// centre (keeping the top the game chose) and centre its texts across the
		// header width. `left` keeps the texts left-aligned inside the centred root (the
		// settings heading of the mockup).
		public static void CenterHeader(RectTransform root, bool left = false)
		{
			if (root == null) return;
			float top = root.anchoredPosition.y;
			root.anchorMin = new Vector2(0.5f, 1f);
			root.anchorMax = new Vector2(0.5f, 1f);
			root.pivot = new Vector2(0.5f, 1f);
			root.anchoredPosition = new Vector2(0f, top);
			int count = root.childCount;
			for (int i = 0; i < count; i++)
			{
				Transform child = root.GetChild(i);
				Text text = child.GetComponent<Text>();
				if (text != null) text.alignment = left ? TextAnchor.UpperLeft : TextAnchor.UpperCenter;
			}
		}
	}
}
