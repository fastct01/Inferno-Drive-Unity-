using System;
using UnityEngine;
using UnityEngine.UI;

// Port of Il2CppOutput/InfernoSettings.inc: volume sliders (master, music, effects) and a vibration
// switch, laid out under the account panel that MakeSettingsScreen builds. Wired in by the hooks kept
// in ShadowRunPrototype (Update -> Tick, MakeSettingsScreen -> Build/StyleAccount, ShowSettingsScreen ->
// Refresh, LayoutProfileScreen -> Relayout, LayoutMenuPageHeaders -> CenterHeader, ApplyGameSoundState ->
// ListenerVolume).
public sealed partial class ShadowRunPrototype
{
	private static partial class InfernoSettings
	{
		private const int Master = 0, Music = 1, Effects = 2, SliderCount = 3;
		private const float PanelWidth = 920f, RowStep = 78f, FirstRow = -100f, TrackWidth = 450f;
		private const float Inset = 50f;                   // inner padding of the cards (content spans -410..410)
		private const float SaveDelay = 0.6f;  // seconds after the last slider change before PlayerPrefs is written
		private const string MasterKey = "InfernoDrive.MasterVolume";

		private static GameObject s_root;             // settingsRoot this UI was built for
		private static readonly Slider[] s_sliders = new Slider[SliderCount];
		private static readonly Text[] s_values = new Text[SliderCount];
		private static Image s_vibrationPill;
		private static Image s_vibrationKnob;
		private static Text s_vibrationText;
		private static int s_master = 100;
		private static bool s_masterLoaded;
		private static bool s_masterApplied;
		private static float s_dirtyFor = -1f;                // seconds since the last unsaved change; <0 when clean
		private static Sprite s_knobSprite;        // chrome ball (slider / switch knobs)
		private static float s_fitWidth = -1f, s_fitHeight = -1f; // overlay size the content scale was computed for
		// MakeUiBlock anchors AND pivots at the requested corner, so the account panel's
		// y (-280) is its top edge: it spans -280..-470. Everything below hangs off that.
		private const float ContentTop = 280f;             // account panel top (kept fixed under the page header)
		private const float ContentBottom = 1044f;         // lowest content y the page reaches (GAME section bottom + margin)

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
			bool vibration = game.vibrationOn;
			if (s_vibrationPill != null) InfernoLobby.Paint(s_vibrationPill, vibration ? InfernoLobby.Electric : InfernoLobby.Slate);
			if (s_vibrationKnob != null)
			{
				s_vibrationKnob.rectTransform.anchoredPosition = new Vector2(vibration ? 48.0f : -48.0f, -28.0f);
			}
			if (s_vibrationText != null)
			{
				InfernoLobby.Caption(s_vibrationText, vibration ? "ON" : "OFF");
				s_vibrationText.rectTransform.anchoredPosition = new Vector2(vibration ? -22.0f : 22.0f, -28.0f);
			}
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
		// header) so everything down to ContentBottom fits above the dock.
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
			float scale = Math.Max(0.5f, Math.Min(1.0f, (available - 24f - ContentTop) / (ContentBottom - ContentTop)));
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

		private static void Hook(ShadowRunPrototype game, Button button, Action<ShadowRunPrototype> fn)
		{
			button.onClick.AddListener(() => fn(game));
		}

		private static void Chrome(Image image)
		{
			EnsureSprites();
			image.sprite = s_knobSprite;
			image.type = Image.Type.Simple;
			image.color = InfernoLobby.Hex(0xFFFFFF);
		}

		private static void Stretch(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
		}

		// A real UnityEngine.UI.Slider (drag, tap, 0-100 whole numbers). The host is a
		// transparent 64 px tall hit area so a finger can grab the 12 px bar; the Slider
		// drives the fill's anchorMax.x and the knob's anchor.x.
		private static Slider MakeSlider(ShadowRunPrototype game, Transform parent, string name, float x, float y, int initial)
		{
			Image host = InfernoLobby.Block(game, parent, name, x, y, TrackWidth, 64f, InfernoLobby.Black);
			host.color = InfernoLobby.Hex(InfernoLobby.Black, 0f);
			host.raycastTarget = true;
			RectTransform hostRect = host.rectTransform;

			Image bar = InfernoLobby.Block(game, hostRect, "Bar", 0f, -32f, TrackWidth, 12f, InfernoLobby.Plum);
			RectTransform barRect = bar.rectTransform;
			Image fill = InfernoLobby.Block(game, barRect, "Fill", 0f, -6f, TrackWidth, 12f, InfernoLobby.Electric);
			RectTransform fillRect = fill.rectTransform;
			Stretch(fillRect);

			Image knob = InfernoLobby.Block(game, hostRect, "Knob", 0f, -32f, 44f, 44f, InfernoLobby.Pearl);
			Chrome(knob);
			knob.raycastTarget = true;
			RectTransform knobRect = knob.rectTransform;
			// Slider.UpdateVisuals drives the handle anchors to (v,0)-(v,1), i.e. it
			// stretches the knob over the 64 px host; a negative height delta keeps it 44x44.
			knobRect.anchorMin = new Vector2(0f, 0f);
			knobRect.anchorMax = new Vector2(0f, 1f);
			knobRect.sizeDelta = new Vector2(44f, -20f);
			knobRect.anchoredPosition = Vector2.zero;

			Slider slider = host.gameObject.AddComponent<Slider>();
			slider.targetGraphic = knob;
			Navigation navigation = Navigation.defaultNavigation;
			navigation.mode = Navigation.Mode.None;
			slider.navigation = navigation;
			slider.minValue = 0f;
			slider.maxValue = 100f;
			slider.wholeNumbers = true;
			slider.fillRect = fillRect;
			slider.handleRect = knobRect;
			slider.SetValueWithoutNotify(initial);
			return slider;
		}

		// Row geometry inside a card (x from the card centre): label -410..-170,
		// slider track -150..300 (knob reaches 322 at 100%), value text 330..410.
		private static Text RowLabel(ShadowRunPrototype game, Transform parent, string name, string title, float y)
		{
			return InfernoLobby.Label(game, parent, name, title, -PanelWidth / 2f + Inset + 120f, y, 240f, 40f, 26, InfernoLobby.Pearl, 3);
		}

		private static void SliderRow(ShadowRunPrototype game, Transform parent, int index, string name, string title, float y)
		{
			RowLabel(game, parent, name, title, y);
			s_sliders[index] = MakeSlider(game, parent, name, 75f, y, Value(game, index));
			s_values[index] = InfernoLobby.Label(game, parent, "Value", "", 370f, y, 80f, 40f, 26, InfernoLobby.Lilac, 5);
			Percent(s_values[index], Value(game, index));
		}

		private static RectTransform Section(ShadowRunPrototype game, Transform parent, string name, string title, float top, float height)
		{
			Image card = InfernoLobby.NeonFrame(game, parent, name, 0f, top - height / 2f, PanelWidth, height);
			RectTransform rect = card.rectTransform;
			InfernoLobby.Label(game, rect, "Title", title, -PanelWidth / 2f + Inset + 120f, -38f, 240f, 36f, 22, InfernoLobby.Lilac, 3);
			InfernoLobby.Block(game, rect, "Rule", 0f, -64f, PanelWidth - 2f * Inset, 2f, InfernoLobby.Mulberry);
			return rect;
		}

		// Keep the existing account text objects so authentication refreshes still update
		// them, but use the same frame and inner text margins as the menu/settings cards.
		public static void StyleAccount(ShadowRunPrototype game, Image panel)
		{
			if (panel == null || game.profileAccountTypeText == null || game.profileIdentityText == null) return;
			panel.sprite = InfernoLobby.NeonFrameSprite(game);
			panel.type = Image.Type.Sliced;
			InfernoLobby.Paint(panel, 0xFFFFFF);
			Transform root = panel.transform;
			RectTransform type = game.profileAccountTypeText.rectTransform;
			RectTransform identity = game.profileIdentityText.rectTransform;
			// The old accent stripe, portrait glow and controls belong to the old layout.
			int count = root.childCount;
			for (int i = 0; i < count; i++)
			{
				Transform child = root.GetChild(i);
				if (child != type && child != identity)
					child.gameObject.SetActive(false);
			}
			InfernoLobby.Place(type, 0f, -64f, PanelWidth - 2f * Inset, 44f);
			InfernoLobby.Place(identity, 0f, -120f, PanelWidth - 2f * Inset, 40f);
			game.profileAccountTypeText.alignment = TextAnchor.MiddleLeft;
			game.profileAccountTypeText.resizeTextForBestFit = true;
			game.profileAccountTypeText.resizeTextMinSize = 24;
			game.profileAccountTypeText.resizeTextMaxSize = 30;
			game.profileIdentityText.alignment = TextAnchor.MiddleLeft;
			game.profileIdentityText.fontStyle = FontStyle.Bold;
			game.profileIdentityText.resizeTextMinSize = 20;
			game.profileIdentityText.resizeTextMaxSize = 26;
			game.profileIdentityText.color = InfernoLobby.Hex(InfernoLobby.Pearl);
		}

		// Called at the end of MakeSettingsScreen with profileContentRoot already built.
		public static void Build(ShadowRunPrototype game)
		{
			if (game.profileContentRoot == null || game.settingsRoot == null) return;
			s_root = game.settingsRoot;
			for (int i = 0; i < SliderCount; i++) { s_sliders[i] = null; s_values[i] = null; }
			s_fitWidth = s_fitHeight = -1f;
			Transform parent = game.profileContentRoot;

			// Layout (content y, root top = 0): account panel -280..-470 (its y is its top
			// edge), AUDIO -500..-830, GAME -860..-1020. The guest LOG IN button is
			// hidden by the patcher (no login on this page).
			// AUDIO: master / music / effects sliders. Master multiplies with the
			// game's saved sound-enabled state (ListenerVolume).
			RectTransform audio = Section(game, parent, "AudioSection", "AUDIO", -500f, 330f);
			SliderRow(game, audio, Master, "MasterSlider", "MASTER", FirstRow);
			SliderRow(game, audio, Music, "MusicSlider", "MUSIC", FirstRow - RowStep);
			SliderRow(game, audio, Effects, "EffectsSlider", "EFFECTS", FirstRow - 2f * RowStep);

			// GAME: vibration switch (PlayCollisionHaptic/PlayImpactHaptic read vibrationOn).
			RectTransform gameCard = Section(game, parent, "GameSection", "GAME", -860f, 160f);
			RowLabel(game, gameCard, "VibrationLabel", "VIBRATION", FirstRow);
			s_vibrationPill = InfernoLobby.Card(game, gameCard, "VibrationSwitch", 335f, FirstRow, 150f, 56f, InfernoLobby.Slate, InfernoLobby.Grape);   // 260..410
			RectTransform pill = s_vibrationPill.rectTransform;
			Hook(game, InfernoLobby.ButtonOn(s_vibrationPill), ToggleVibration);
			s_vibrationText = InfernoLobby.Label(game, pill, "State", "", 22f, -28f, 80f, 40f, 22, InfernoLobby.Pearl);
			s_vibrationKnob = InfernoLobby.Block(game, pill, "Knob", -48f, -28f, 40f, 40f, InfernoLobby.Pearl);
			Chrome(s_vibrationKnob);

			Refresh(game);
		}

		// Garage and profile page headers. The patcher calls this from
		// LayoutMenuPageHeaders after placing each header root at a computed left
		// offset; that offset is measured from overlayRect, which can still be unsized
		// while the menu is being built, so re-anchor the root to the page's horizontal
		// centre (keeping the top the game chose) and centre its texts across the
		// header width.
		public static void CenterHeader(RectTransform root)
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
				if (text != null) text.alignment = TextAnchor.UpperCenter;
			}
		}
	}
}
