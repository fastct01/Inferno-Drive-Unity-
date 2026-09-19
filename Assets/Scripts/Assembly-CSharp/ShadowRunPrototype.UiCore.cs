using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Translated from Assembly-CSharp.cpp: ShadowRunPrototype_MakeUI, _MakeText, _ApplyAnchor, _GetUiFont, _UsesMenuFontGlyphs,
// _MakeUiBlock, _LoadResourceSprite, _MakeResourceImage, _MakeResourceRawImage, _MakeButton, _MakeImageButton,
// _ApplyRawCover, _FitRawImageAspect, _FitImageAspect, _ApplyObjectFitCover, _MakeProgressBar, _MakeFigmaProgressBar,
// _MakeIconButton, _MakeThinGlassPanel (x4), _MakeGlassPanel, _MakeGlassButton, _LoadMenuButtonTexture, _AddButtonIcon,
// _MakeSpacedWord, _AddButtonSpacing, _MakeGameplayTouchControls, _MakeGameplayTouchZone (+ DisplayClass340_0 closures),
// _IsHudActionPoint, _MakeSpeedometer, _CreateSpeedometerNeedleSprite, _MakeBar, _MakeHudPauseButton, _MakeHudBrakeButton,
// _ReleaseBrakePointer, _MakeHudHighBeamButton, _MakeHudActionButton, _MakeHudActionIcon, _MakeOverlay, _MakeGameOverScreen,
// _MakePauseActions, _MakePauseArtButton, _MakeSoundToggleButton, _MakeGameOverValue, _MakeGameOverArtButton,
// _MakeReferenceArtImage, _MakeReferenceArtButton, _MakeMenuBackdrop, _ApplySafeArea.
public sealed partial class ShadowRunPrototype
{
	private void MakeUI()
	{
		if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
		{
			GameObject eventSystemObject = new GameObject("EventSystem");
			eventSystemObject.AddComponent<EventSystem>();
			eventSystemObject.AddComponent<InputSystemUIInputModule>();
		}
		else
		{
			EventSystem existing = UnityEngine.Object.FindFirstObjectByType<EventSystem>();
			if (existing.GetComponent<InputSystemUIInputModule>() == null)
			{
				existing.gameObject.AddComponent<InputSystemUIInputModule>();
			}
		}
		EventSystem eventSystem = EventSystem.current != null ? EventSystem.current : UnityEngine.Object.FindFirstObjectByType<EventSystem>();
		if (eventSystem != null)
		{
			float dpi = Screen.dpi > 0f ? Screen.dpi : 326f;
			eventSystem.pixelDragThreshold = Application.isMobilePlatform ? Mathf.RoundToInt(Mathf.Clamp(dpi * 0.05f, 16f, 28f)) : 10;
		}
		Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.pixelPerfect = true;
		CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1080f, 1920f);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 0.5f;
		canvas.gameObject.AddComponent<GraphicRaycaster>();
		menuUiFont = Resources.Load<Font>("Space_Exploration_GUI_Kit/Fonts/Font Sources/static/Righteous-Regular");
		if (menuUiFont == null)
		{
			menuUiFont = Resources.Load<Font>("Space_Exploration_GUI_Kit/Fonts/Font Sources/static/Asap-Bold");
		}
		softCircleSprite = MakeCircleSprite(96);
		caseRadialGlowSprite = MakeRadialGlowSprite(192);
		roundedGlassSprite = MakeRoundedRectSprite(256, 64, 18, 1f);
		roundedGlowSprite = MakeRoundedRectSprite(256, 64, 24, 0.55f);
		casePrimaryButtonSprite = MakeCaseActionButtonSprite(1024, 192, 54f, true);
		caseSecondaryButtonSprite = MakeCaseActionButtonSprite(640, 176, 46f, false);
		gemCounterSprite = MakeGemCounterSprite(720, 220, 54f);
		purpleProgressSprite = MakeGradientBarSprite(256, 16, new Color(0.486f, 0.227f, 0.929f, 1f), new Color(0.753f, 0.518f, 0.988f, 1f));
		heroBottomFadeSprite = MakeVerticalFadeSprite(32, 256);
		caseRarityStripSprite = MakeCaseRarityStripSprite(32, 64);
		caseRouletteRailSprite = MakeCaseRouletteRailSprite(256, 4);
		caseRouletteLeftVignetteSprite = MakeHorizontalFadeSprite(128, 4, true);
		caseRouletteRightVignetteSprite = MakeHorizontalFadeSprite(128, 4, false);
		caseRouletteScanlineSprite = MakeCaseScanlineSprite(32, 64);
		caseTriangleDownSprite = MakeTriangleSprite(44, 26, true);
		caseTriangleUpSprite = MakeTriangleSprite(44, 26, false);
		menuTabFrameSprite = MakeReferenceTabFrameSprite(2000, 640, 184f);
		menuTabActiveSprite = MakeReferenceActiveTabSprite(600, 452, 108f);
		hudRoot = new GameObject("HUD");
		hudRoot.transform.SetParent(canvas.transform);
		RectTransform hudRect = hudRoot.AddComponent<RectTransform>();
		hudRect.anchorMin = Vector2.zero;
		hudRect.anchorMax = Vector2.one;
		hudRect.offsetMin = Vector2.zero;
		hudRect.offsetMax = Vector2.zero;
		MakeGameplayTouchControls(hudRoot.transform);
		scoreText = MakeText(hudRoot.transform, "0m", 77, TextAnchor.UpperLeft, new Vector2(48f, -40f));
		scoreText.raycastTarget = false;
		scoreText.rectTransform.sizeDelta = new Vector2(420f, 126f);
		bestText = MakeText(hudRoot.transform, "BEST 0m", 28, TextAnchor.UpperLeft, new Vector2(50f, -121f));
		bestText.raycastTarget = false;
		bestText.rectTransform.sizeDelta = new Vector2(420f, 126f);
		bestText.color = new Color(0.55f, 0.48f, 0.7f);
		RectTransform lives = new GameObject("Lives").AddComponent<RectTransform>();
		lives.SetParent(hudRoot.transform);
		lives.anchorMin = lives.anchorMax = new Vector2(1f, 1f);
		lives.pivot = new Vector2(1f, 1f);
		lives.anchoredPosition = new Vector2(-56f, -48f);
		lives.sizeDelta = new Vector2(255f, 73f);
		Sprite heartSprite = LoadResourceSprite("сердце");
		Shader grayscaleShader = Resources.Load<Shader>("Shaders/UIGrayscale");
		if (grayscaleShader != null && lostLifeIconMaterial == null)
		{
			lostLifeIconMaterial = new Material(grayscaleShader);
			lostLifeIconMaterial.name = "Lost Life Icon Grayscale";
		}
		for (int i = 0; i < 3; i++)
		{
			Image heart = new GameObject("Life Heart").AddComponent<Image>();
			heart.transform.SetParent(lives);
			heart.sprite = heartSprite != null ? heartSprite : softCircleSprite;
			heart.material = lostLifeIconMaterial;
			heart.color = new Color(0.52f, 0.52f, 0.56f, 0.72f);
			heart.preserveAspect = true;
			heart.raycastTarget = false;
			RectTransform heartRect = heart.GetComponent<RectTransform>();
			heartRect.anchorMin = heartRect.anchorMax = new Vector2(1f, 1f);
			heartRect.pivot = new Vector2(1f, 1f);
			heartRect.anchoredPosition = new Vector2(-i * 73f, 0f);
			heartRect.sizeDelta = new Vector2(64.4f, 64.4f);
			Image fill = new GameObject("Life Heart Active Fill").AddComponent<Image>();
			fill.transform.SetParent(heart.transform, false);
			fill.sprite = heart.sprite;
			fill.color = Color.white;
			fill.preserveAspect = true;
			fill.type = Image.Type.Filled;
			fill.fillMethod = Image.FillMethod.Horizontal;
			fill.fillOrigin = (int)Image.OriginHorizontal.Right;
			fill.fillAmount = 1f;
			fill.raycastTarget = false;
			RectTransform fillRect = fill.rectTransform;
			fillRect.anchorMin = Vector2.zero;
			fillRect.anchorMax = Vector2.one;
			fillRect.offsetMin = Vector2.zero;
			fillRect.offsetMax = Vector2.zero;
			lifeDots.Add(fill);
		}
		MakeSpeedometer(hudRoot.transform);
		MakeHudBrakeButton(hudRoot.transform);
		MakeHudHighBeamButton(hudRoot.transform);
		MakeHudPauseButton(hudRoot.transform);
		MakeOverlay(canvas.transform);
	}

	private Text MakeText(Transform parent, string content, int size, TextAnchor anchor, Vector2 pos)
	{
		Text text = new GameObject("Text").AddComponent<Text>();
		text.transform.SetParent(parent);
		text.text = content;
		text.font = GetUiFont(content);
		text.fontSize = size;
		text.fontStyle = FontStyle.Normal;
		text.alignment = anchor;
		text.color = Color.white;
		RectTransform rect = text.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = new Vector2(420f, 90f);
		return text;
	}

	private void ApplyAnchor(RectTransform rect, TextAnchor anchor)
	{
		Vector2 pivot = new Vector2(0.5f, 0.5f);
		switch (anchor)
		{
			case TextAnchor.UpperLeft: pivot = new Vector2(0f, 1f); break;
			case TextAnchor.UpperCenter: pivot = new Vector2(0.5f, 1f); break;
			case TextAnchor.UpperRight: pivot = new Vector2(1f, 1f); break;
			case TextAnchor.MiddleLeft: pivot = new Vector2(0f, 0.5f); break;
			case TextAnchor.MiddleRight: pivot = new Vector2(1f, 0.5f); break;
			case TextAnchor.LowerLeft: pivot = new Vector2(0f, 0f); break;
			case TextAnchor.LowerCenter: pivot = new Vector2(0.5f, 0f); break;
			case TextAnchor.LowerRight: pivot = new Vector2(1f, 0f); break;
		}
		rect.anchorMin = pivot;
		rect.anchorMax = pivot;
		rect.pivot = pivot;
	}

	private Font GetUiFont(string content)
	{
		if (menuUiFont != null && UsesMenuFontGlyphs(content))
		{
			return menuUiFont;
		}
		return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
	}

	private bool UsesMenuFontGlyphs(string content)
	{
		if (string.IsNullOrEmpty(content))
		{
			return true;
		}
		for (int i = 0; i < content.Length; i++)
		{
			if (content[i] > '~')
			{
				return false;
			}
		}
		return true;
	}

	private Image MakeUiBlock(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color color)
	{
		Image image = new GameObject(name).AddComponent<Image>();
		image.transform.SetParent(parent);
		image.color = color;
		RectTransform rect = image.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		return image;
	}

	private Sprite LoadResourceSprite(string path)
	{
		/* [InfernoLobby patch] */ if (InfernoLobby.IsHeartResource(path)) return InfernoLobby.HeartSprite();
		/* [InfernoLobby patch] */ { Sprite png = InfernoGarage.Picture(this, path); if (png != null) return png; }
		/* [InfernoLobby patch] */ if (InfernoLobby.IsDiamondResource(path)) { Sprite gem = InfernoLobby.GemSprite(this); if (gem != null) return gem; }
		Sprite sprite = Resources.Load<Sprite>(path);
		if (sprite != null)
		{
			return sprite;
		}
		Texture2D texture = Resources.Load<Texture2D>(path);
		if (texture == null)
		{
			return null;
		}
		return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
	}

	private Image MakeResourceImage(Transform parent, string name, string resource, Vector2 pos, Vector2 size, TextAnchor anchor, bool preserveAspect)
	{
		Sprite sprite = LoadResourceSprite(resource);
		if (sprite == null)
		{
			return null;
		}
		sprite.texture.filterMode = FilterMode.Bilinear;
		sprite.texture.anisoLevel = 1;
		sprite.texture.wrapMode = TextureWrapMode.Clamp;
		Image image = new GameObject(name).AddComponent<Image>();
		image.transform.SetParent(parent);
		image.sprite = sprite;
		image.color = Color.white;
		image.preserveAspect = preserveAspect;
		RectTransform rect = image.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		return image;
	}

	private RawImage MakeResourceRawImage(Transform parent, string name, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		Texture2D texture = Resources.Load<Texture2D>(resource);
		if (texture == null)
		{
			return null;
		}
		texture.filterMode = FilterMode.Trilinear;
		texture.anisoLevel = 8;
		texture.wrapMode = TextureWrapMode.Clamp;
		RawImage image = new GameObject(name).AddComponent<RawImage>();
		image.transform.SetParent(parent);
		image.texture = texture;
		image.color = Color.white;
		image.raycastTarget = false;
		RectTransform rect = image.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		return image;
	}

	private Button MakeButton(Transform parent, string label, Vector2 pos, Vector2 size, TextAnchor anchor, Color bg, Color fg)
	{
		Button button = new GameObject(label + " Button").AddComponent<Button>();
		button.transform.SetParent(parent);
		Image image = button.gameObject.AddComponent<Image>();
		image.color = bg;
		image.sprite = roundedGlassSprite;
		image.type = Image.Type.Sliced;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		button.targetGraphic = image;
		ColorBlock colors = button.colors;
		colors.normalColor = bg;
		colors.highlightedColor = bg + new Color(0.08f, 0.02f, 0.02f, 0f);
		colors.pressedColor = new Color(bg.r * 0.72f, bg.g * 0.72f, bg.b * 0.72f, bg.a);
		colors.selectedColor = colors.highlightedColor;
		button.colors = colors;
		MakeUiBlock(button.transform, "Glass Red Glow", Vector2.zero, new Vector2(size.x + 28f, size.y + 20f), TextAnchor.MiddleCenter, new Color(0.92f, 0f, 0.03f, 0.11f));
		MakeUiBlock(button.transform, "Glass Wash", Vector2.zero, new Vector2(size.x - 8f, size.y - 8f), TextAnchor.MiddleCenter, new Color(1f, 0.84f, 0.86f, 0.035f));
		MakeUiBlock(button.transform, "Button Top Line", new Vector2(0f, size.y * 0.5f - 2f), new Vector2(size.x, 2f), TextAnchor.MiddleCenter, new Color(1f, 0.08f, 0.06f, 0.72f));
		MakeUiBlock(button.transform, "Button Bottom Line", new Vector2(0f, -size.y * 0.5f + 2f), new Vector2(size.x, 1f), TextAnchor.MiddleCenter, new Color(1f, 0.08f, 0.06f, 0.38f));
		MakeUiBlock(button.transform, "Button Left Line", new Vector2(-size.x * 0.5f + 2f, 0f), new Vector2(2f, size.y), TextAnchor.MiddleCenter, new Color(1f, 0.08f, 0.06f, 0.28f));
		MakeUiBlock(button.transform, "Button Right Line", new Vector2(size.x * 0.5f - 2f, 0f), new Vector2(2f, size.y), TextAnchor.MiddleCenter, new Color(1f, 0.08f, 0.06f, 0.18f));
		Text text = MakeText(button.transform, label, Mathf.RoundToInt(size.y * 0.32f), TextAnchor.MiddleCenter, Vector2.zero);
		text.color = fg;
		text.GetComponent<RectTransform>().sizeDelta = size;
		text.transform.SetAsLastSibling();
		return button;
	}

	private Button MakeImageButton(Transform parent, string label, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		Button button = new GameObject(label + " Image Button").AddComponent<Button>();
		button.transform.SetParent(parent);
		RawImage image = button.gameObject.AddComponent<RawImage>();
		image.texture = Resources.Load<Texture2D>(resource);
		if (image.texture != null)
		{
			image.texture.filterMode = FilterMode.Trilinear;
			image.texture.anisoLevel = 8;
			image.texture.wrapMode = TextureWrapMode.Clamp;
		}
		if (resource == "drive_button2")
		{
			image.uvRect = new Rect(0.065f, 0.24f, 0.87f, 0.49f);
		}
		image.color = Color.white;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		button.targetGraphic = image;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1f, 1f, 1f, 0.92f);
		colors.pressedColor = new Color(0.75f, 0.75f, 0.82f, 1f);
		button.colors = colors;
		Text text = MakeText(button.transform, "D R I V E", 1, TextAnchor.MiddleCenter, Vector2.zero);
		text.name = "Button Label";
		text.color = Color.clear;
		text.raycastTarget = false;
		text.GetComponent<RectTransform>().sizeDelta = size;
		return button;
	}

	private void ApplyRawCover(RawImage image, float targetAspect)
	{
		if (image == null || image.texture == null || image.texture.height <= 0)
		{
			return;
		}
		float aspect = (float)image.texture.width / image.texture.height;
		if (aspect > targetAspect)
		{
			float visible = targetAspect / aspect;
			image.uvRect = new Rect((1f - visible) * 0.5f, 0f, visible, 1f);
			return;
		}
		float visibleHeight = aspect / targetAspect;
		image.uvRect = new Rect(0f, (1f - visibleHeight) * 0.5f, 1f, visibleHeight);
	}

	private void FitRawImageAspect(RawImage image, Vector2 maxSize)
	{
		if (image == null)
		{
			return;
		}
		image.uvRect = new Rect(0f, 0f, 1f, 1f);
		image.rectTransform.localScale = Vector3.one;
		if (image.texture == null || image.texture.width <= 0 || image.texture.height <= 0)
		{
			image.rectTransform.sizeDelta = maxSize;
			return;
		}
		float aspect = (float)image.texture.width / image.texture.height;
		float maxAspect = maxSize.x / Mathf.Max(1f, maxSize.y);
		image.rectTransform.sizeDelta = aspect > maxAspect ? new Vector2(maxSize.x, maxSize.x / aspect) : new Vector2(maxSize.y * aspect, maxSize.y);
	}

	private void FitImageAspect(Image image, Vector2 maxSize)
	{
		if (image == null)
		{
			return;
		}
		image.rectTransform.localScale = Vector3.one;
		if (image.sprite == null || image.sprite.rect.width <= 0f || image.sprite.rect.height <= 0f)
		{
			image.rectTransform.sizeDelta = maxSize;
			return;
		}
		float aspect = image.sprite.rect.width / image.sprite.rect.height;
		float maxAspect = maxSize.x / Mathf.Max(1f, maxSize.y);
		image.rectTransform.sizeDelta = aspect > maxAspect ? new Vector2(maxSize.x, maxSize.x / aspect) : new Vector2(maxSize.y * aspect, maxSize.y);
	}

	private void ApplyObjectFitCover(RectTransform img, RectTransform viewport, Sprite sprite)
	{
		float viewportWidth = viewport.rect.width;
		float viewportHeight = viewport.rect.height;
		float spriteWidth = sprite.rect.width;
		float spriteHeight = sprite.rect.height;
		float scale = Mathf.Max(viewportWidth / spriteWidth, viewportHeight / spriteHeight);
		img.anchorMin = img.anchorMax = new Vector2(0.5f, 0.5f);
		img.pivot = new Vector2(0.5f, 0.5f);
		img.anchoredPosition = Vector2.zero;
		img.sizeDelta = new Vector2(spriteWidth * scale, spriteHeight * scale);
	}

	private void MakeProgressBar(Transform parent, Vector2 pos, Vector2 size, float progress, Color fillColor)
	{
		Image background = MakeUiBlock(parent, "Progress BG", pos, size, TextAnchor.LowerLeft, new Color(0.08f, 0.12f, 0.2f, 0.9f));
		background.sprite = roundedGlassSprite;
		background.type = Image.Type.Sliced;
		background.raycastTarget = false;
		Image fill = MakeUiBlock(background.transform, "Progress Fill", Vector2.zero, new Vector2(size.x * Mathf.Clamp01(progress), size.y), TextAnchor.MiddleLeft, fillColor);
		fill.sprite = roundedGlassSprite;
		fill.type = Image.Type.Sliced;
		fill.raycastTarget = false;
	}

	private void MakeFigmaProgressBar(Transform parent, Vector2 pos, Vector2 size, float progress)
	{
		Image background = MakeUiBlock(parent, "Progress BG", pos, size, TextAnchor.LowerLeft, new Color(0.545f, 0.361f, 0.965f, 0.2f));
		background.sprite = roundedGlassSprite;
		background.type = Image.Type.Sliced;
		background.raycastTarget = false;
		float fillWidth = size.x * Mathf.Clamp01(progress);
		if (fillWidth <= 0.01f)
		{
			return;
		}
		Image fill = MakeUiBlock(background.transform, "Progress Fill", Vector2.zero, new Vector2(fillWidth, size.y), TextAnchor.MiddleLeft, Color.white);
		fill.sprite = purpleProgressSprite;
		fill.type = Image.Type.Simple;
		fill.raycastTarget = false;
	}

	private Button MakeIconButton(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, string iconLabel, int iconSize)
	{
		Button button = new GameObject(name + " Button").AddComponent<Button>();
		button.transform.SetParent(parent);
		Image image = button.gameObject.AddComponent<Image>();
		image.color = new Color(0.035f, 0.055f, 0.095f, 0.86f);
		image.sprite = roundedGlassSprite;
		image.type = Image.Type.Sliced;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		button.targetGraphic = image;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1f, 1f, 1f, 0.9f);
		colors.pressedColor = new Color(0.78f, 0.78f, 0.86f, 1f);
		button.colors = colors;
		Text icon = MakeText(button.transform, iconLabel, iconSize, TextAnchor.MiddleCenter, Vector2.zero);
		icon.color = Color.white;
		icon.GetComponent<RectTransform>().sizeDelta = size;
		icon.raycastTarget = false;
		return button;
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		return MakeThinGlassPanel(parent, name, pos, size, anchor, new Color(0.004f, 0.009f, 0.024f, 0.78f));
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill)
	{
		return MakeThinGlassPanel(parent, name, pos, size, anchor, fill, new Color(0.18f, 0.25f, 0.43f, 0.2f));
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill, Color borderColor)
	{
		return MakeThinGlassPanel(parent, name, pos, size, anchor, fill, borderColor, 1.5f);
	}

	private Image MakeThinGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill, Color borderColor, float borderThickness)
	{
		Image border = MakeUiBlock(parent, name, pos, size, anchor, borderColor);
		border.sprite = roundedGlassSprite;
		border.type = Image.Type.Sliced;
		border.raycastTarget = false;
		float inset = Mathf.Max(0.5f, borderThickness) * 2f;
		Image inner = MakeUiBlock(border.transform, "Glass Fill", Vector2.zero, size - new Vector2(inset, inset), TextAnchor.MiddleCenter, fill);
		inner.sprite = roundedGlassSprite;
		inner.type = Image.Type.Sliced;
		inner.raycastTarget = false;
		return border;
	}

	private Image MakeGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, TextAnchor anchor, Color fill, Color stroke)
	{
		Image glow = MakeUiBlock(parent, name + " Glow", pos, size + new Vector2(14f, 14f), anchor, new Color(stroke.r, stroke.g, stroke.b, 0.13f));
		glow.sprite = roundedGlowSprite;
		glow.type = Image.Type.Sliced;
		glow.raycastTarget = false;
		Image panel = MakeUiBlock(parent, name, pos, size, anchor, fill);
		panel.sprite = roundedGlassSprite;
		panel.type = Image.Type.Sliced;
		panel.raycastTarget = false;
		Image topLine = MakeUiBlock(panel.transform, "Panel Top Line", new Vector2(0f, size.y * 0.5f - 2f), new Vector2(size.x - 40f, 2f), TextAnchor.MiddleCenter, stroke);
		topLine.raycastTarget = false;
		Image bottomLine = MakeUiBlock(panel.transform, "Panel Bottom Line", new Vector2(0f, -size.y * 0.5f + 2f), new Vector2(size.x - 40f, 2f), TextAnchor.MiddleCenter, new Color(stroke.r, stroke.g, stroke.b, stroke.a * 0.55f));
		bottomLine.raycastTarget = false;
		return panel;
	}

	private Button MakeGlassButton(Transform parent, string label, Vector2 pos, Vector2 size, bool primary)
	{
		Button button = new GameObject(label + " Glass Button").AddComponent<Button>();
		button.transform.SetParent(parent);
		RawImage image = button.gameObject.AddComponent<RawImage>();
		image.color = Color.white;
		image.texture = LoadMenuButtonTexture(label);
		image.raycastTarget = true;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.UpperCenter);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		button.targetGraphic = image;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1f, 1f, 1f, 0.92f);
		colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
		colors.selectedColor = colors.highlightedColor;
		colors.disabledColor = new Color(1f, 1f, 1f, 0.45f);
		button.colors = colors;
		Text text = MakeText(button.transform, primary ? "P  L  A  Y" : AddButtonSpacing(label), 1, TextAnchor.MiddleCenter, Vector2.zero);
		text.name = "Button Label";
		text.color = Color.clear;
		text.raycastTarget = false;
		text.GetComponent<RectTransform>().sizeDelta = size;
		return button;
	}

	private Texture2D LoadMenuButtonTexture(string label)
	{
		Texture2D texture = null;
		if (label == "PLAY")
		{
			texture = Resources.Load<Texture2D>("drive_button2");
		}
		else if (label == "GARAGE")
		{
			texture = Resources.Load<Texture2D>("garage_but (2)");
		}
		else if (label == "SETTINGS")
		{
			texture = Resources.Load<Texture2D>("settings_but (2)");
		}
		if (texture == null)
		{
			return null;
		}
		texture.filterMode = FilterMode.Trilinear;
		texture.anisoLevel = 8;
		texture.wrapMode = TextureWrapMode.Clamp;
		return texture;
	}

	private void AddButtonIcon(Transform parent, string label, Vector2 size, bool primary, Color red)
	{
		Vector2 iconPos = new Vector2(primary ? -205f : -105f, 0f);
		if (label == "PLAY")
		{
			Text play = MakeText(parent, "▷", 58, TextAnchor.MiddleCenter, iconPos);
			play.color = red;
			play.GetComponent<RectTransform>().sizeDelta = new Vector2(82f, 82f);
			play.transform.SetAsLastSibling();
			return;
		}
		if (label == "GARAGE")
		{
			Image body = MakeUiBlock(parent, "Car Icon Body", iconPos + new Vector2(0f, -4f), new Vector2(54f, 16f), TextAnchor.MiddleCenter, red);
			Image roof = MakeUiBlock(parent, "Car Icon Roof", iconPos + new Vector2(0f, 9f), new Vector2(36f, 14f), TextAnchor.MiddleCenter, red);
			Image roofCut = MakeUiBlock(parent, "Car Icon Roof Cut", iconPos + new Vector2(0f, 7f), new Vector2(26f, 8f), TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.96f));
			Image wheelL = MakeUiBlock(parent, "Car Icon Wheel L", iconPos + new Vector2(-18f, -15f), new Vector2(12f, 12f), TextAnchor.MiddleCenter, red);
			Image wheelR = MakeUiBlock(parent, "Car Icon Wheel R", iconPos + new Vector2(18f, -15f), new Vector2(12f, 12f), TextAnchor.MiddleCenter, red);
			wheelL.sprite = softCircleSprite;
			wheelR.sprite = softCircleSprite;
			body.transform.SetAsLastSibling();
			roof.transform.SetAsLastSibling();
			roofCut.transform.SetAsLastSibling();
			wheelL.transform.SetAsLastSibling();
			wheelR.transform.SetAsLastSibling();
			return;
		}
		Text gear = MakeText(parent, "⚙", 46, TextAnchor.MiddleCenter, iconPos);
		gear.color = red;
		gear.GetComponent<RectTransform>().sizeDelta = new Vector2(76f, 76f);
		gear.transform.SetAsLastSibling();
	}

	private void MakeSpacedWord(Transform parent, string word, Vector2 pos, int size, float spacing, Color color, bool glow)
	{
		float startX = -spacing * (word.Length - 1) * 0.5f;
		for (int i = 0; i < word.Length; i++)
		{
			if (glow)
			{
				Text halo = MakeText(parent, word[i].ToString(), size + 8, TextAnchor.MiddleCenter, new Vector2(startX + i * spacing, pos.y));
				halo.fontStyle = FontStyle.Bold;
				halo.color = new Color(color.r, color.g, color.b, 0.16f);
				halo.GetComponent<RectTransform>().sizeDelta = new Vector2(120f, 180f);
				halo.rectTransform.localScale = new Vector3(0.72f, 1.12f, 1f);
			}
			Text letter = MakeText(parent, word[i].ToString(), size, TextAnchor.MiddleCenter, new Vector2(startX + i * spacing, pos.y));
			letter.fontStyle = FontStyle.Bold;
			letter.color = color;
			letter.GetComponent<RectTransform>().sizeDelta = new Vector2(120f, 180f);
			letter.rectTransform.localScale = new Vector3(glow ? 0.58f : 0.72f, glow ? 1.16f : 1f, 1f);
		}
	}

	private string AddButtonSpacing(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		string result = "";
		for (int i = 0; i < value.Length; i++)
		{
			result += value[i].ToString();
			if (i < value.Length - 1)
			{
				result += "  ";
			}
		}
		return result;
	}

	private void MakeGameplayTouchControls(Transform parent)
	{
		MakeGameplayTouchZone(parent, "Steer Left Touch Zone", new Vector2(0f, 0.16f), new Vector2(0.5f, 0.88f), touchSteerLeftPointers, null);
		MakeGameplayTouchZone(parent, "Steer Right Touch Zone", new Vector2(0.5f, 0.16f), new Vector2(1f, 0.88f), touchSteerRightPointers, null);
		MakeGameplayTouchZone(parent, "Boost Touch Zone", new Vector2(0.34f, 0.1f), new Vector2(0.66f, 0.49f), touchBoostPointers, () => touchBoostHeld = touchBoostPointers.Count > 0);
	}

	private void MakeGameplayTouchZone(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, HashSet<int> activePointers, Action changed)
	{
		Image zone = new GameObject(name).AddComponent<Image>();
		zone.transform.SetParent(parent, false);
		zone.color = Color.clear;
		zone.raycastTarget = true;
		RectTransform rect = zone.rectTransform;
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		EventTrigger trigger = zone.gameObject.AddComponent<EventTrigger>();
		void ReleasePointer(BaseEventData data)
		{
			activePointers.Remove(((PointerEventData)data).pointerId);
			changed?.Invoke();
		}
		EventTrigger.Entry down = new EventTrigger.Entry();
		down.eventID = EventTriggerType.PointerDown;
		down.callback.AddListener(data =>
		{
			activePointers.Add(((PointerEventData)data).pointerId);
			changed?.Invoke();
		});
		trigger.triggers.Add(down);
		EventTrigger.Entry up = new EventTrigger.Entry();
		up.eventID = EventTriggerType.PointerUp;
		up.callback.AddListener(ReleasePointer);
		trigger.triggers.Add(up);
		EventTrigger.Entry cancel = new EventTrigger.Entry();
		cancel.eventID = EventTriggerType.Cancel;
		cancel.callback.AddListener(ReleasePointer);
		trigger.triggers.Add(cancel);
	}

	private bool IsHudActionPoint(Vector2 screenPoint)
	{
		if (brakeHudButtonRect != null && RectTransformUtility.RectangleContainsScreenPoint(brakeHudButtonRect, screenPoint))
		{
			return true;
		}
		if (highBeamHudButtonRect != null && RectTransformUtility.RectangleContainsScreenPoint(highBeamHudButtonRect, screenPoint))
		{
			return true;
		}
		if (pauseHudButtonRect != null)
		{
			return RectTransformUtility.RectangleContainsScreenPoint(pauseHudButtonRect, screenPoint);
		}
		return false;
	}

	private void MakeSpeedometer(Transform parent)
	{
		RectTransform root = new GameObject("Speedometer").AddComponent<RectTransform>();
		root.SetParent(parent, false);
		root.gameObject.AddComponent<CanvasGroup>().alpha = 1f;
		root.anchorMin = root.anchorMax = Vector2.zero;
		root.pivot = Vector2.zero;
		root.anchoredPosition = new Vector2(-17f, 26f);
		root.sizeDelta = new Vector2(522f, 348f);
		root.localScale = Vector3.one * SpeedometerScale;
		Image dial = new GameObject("Speedometer Dial").AddComponent<Image>();
		dial.transform.SetParent(root);
		dial.sprite = LoadResourceSprite("спидометр финал");
		if (dial.sprite != null)
		{
			dial.sprite.texture.filterMode = FilterMode.Bilinear;
			dial.sprite.texture.anisoLevel = 1;
			dial.sprite.texture.wrapMode = TextureWrapMode.Clamp;
		}
		dial.color = Color.white;
		dial.raycastTarget = false;
		dial.preserveAspect = true;
		RectTransform dialRect = dial.GetComponent<RectTransform>();
		dialRect.anchorMin = Vector2.zero;
		dialRect.anchorMax = Vector2.one;
		dialRect.offsetMin = Vector2.zero;
		dialRect.offsetMax = Vector2.zero;
		speedometerNeedleSprite = CreateSpeedometerNeedleSprite();
		speedometerNeedleGlow = null;
		Image needle = MakeUiBlock(root, "Speedometer Needle", new Vector2(261f, 174f), new Vector2(13f, 132f), TextAnchor.LowerLeft, new Color(1f, 0.025f, 0.02f, 1f));
		needle.sprite = speedometerNeedleSprite;
		needle.type = Image.Type.Simple;
		speedometerNeedle = needle.rectTransform;
		speedometerNeedle.pivot = new Vector2(0.5f, 0.06f);
		speedometerNeedle.anchoredPosition = new Vector2(261f, 174f);
		speedometerNeedle.localRotation = Quaternion.Euler(0f, 0f, 135f);
		Image cap = MakeUiBlock(root, "Speedometer Needle Cap", new Vector2(0f, 0f), new Vector2(30f, 30f), TextAnchor.MiddleCenter, new Color(0.02f, 0.015f, 0.016f, 0.96f));
		cap.sprite = softCircleSprite;
		cap.raycastTarget = false;
		speedometerValueText = MakeText(root, "000", 66, TextAnchor.MiddleCenter, new Vector2(0f, -8f));
		speedometerValueText.color = new Color(1f, 1f, 1f, 0.96f);
		speedometerValueText.fontStyle = FontStyle.Bold;
		speedometerValueText.raycastTarget = false;
		speedometerValueText.resizeTextForBestFit = true;
		speedometerValueText.resizeTextMinSize = 42;
		speedometerValueText.resizeTextMaxSize = 66;
		speedometerValueText.GetComponent<RectTransform>().sizeDelta = new Vector2(180f, 82f);
	}

	private Sprite CreateSpeedometerNeedleSprite()
	{
		Texture2D texture = new Texture2D(64, 256, TextureFormat.RGBA32, false, true);
		texture.name = "Speedometer Needle Clean AA";
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.anisoLevel = 1;
		Color32[] pixels = new Color32[16384];
		for (int y = 0; y < 256; y++)
		{
			float v = (y + 0.5f) / 256f;
			float halfWidth = v < 0.84f ? Mathf.Lerp(25f, 8f, v / 0.84f) : Mathf.Lerp(8f, 0f, (v - 0.84f) / 0.16f);
			for (int x = 0; x < 64; x++)
			{
				float edge = Mathf.Abs(x + 0.5f - 32f) - halfWidth;
				float alpha = 1f - Mathf.SmoothStep(-1.25f, 1.25f, edge);
				pixels[y * 64 + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(alpha * 255f));
			}
		}
		texture.SetPixels32(pixels);
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, 64f, 256f), new Vector2(0.5f, 0.06f), 100f, 0u, SpriteMeshType.Tight);
	}

	private Image MakeBar(Transform parent, Vector2 pos, Color color)
	{
		Image background = new GameObject("Meter BG").AddComponent<Image>();
		background.transform.SetParent(parent);
		background.color = new Color(0.02f, 0.01f, 0.04f, 0.88f);
		background.raycastTarget = false;
		RectTransform backgroundRect = background.GetComponent<RectTransform>();
		backgroundRect.anchorMin = backgroundRect.anchorMax = new Vector2(0f, 0f);
		backgroundRect.pivot = new Vector2(0f, 0f);
		backgroundRect.anchoredPosition = pos;
		backgroundRect.sizeDelta = new Vector2(430f, 12f);
		Image fill = new GameObject("Meter Fill").AddComponent<Image>();
		fill.transform.SetParent(background.transform);
		fill.color = color;
		fill.raycastTarget = false;
		fill.type = Image.Type.Filled;
		fill.fillMethod = Image.FillMethod.Horizontal;
		fill.fillOrigin = (int)Image.OriginHorizontal.Left;
		fill.fillAmount = 0f;
		RectTransform fillRect = fill.GetComponent<RectTransform>();
		fillRect.anchorMin = Vector2.zero;
		fillRect.anchorMax = Vector2.one;
		fillRect.offsetMin = Vector2.zero;
		fillRect.offsetMax = Vector2.zero;
		return fill;
	}

	private void MakeHudPauseButton(Transform parent)
	{
		Button button = new GameObject("Pause HUD Button").AddComponent<Button>();
		button.transform.SetParent(parent, false);
		Image frame = button.gameObject.AddComponent<Image>();
		frame.sprite = roundedGlassSprite;
		frame.type = Image.Type.Sliced;
		frame.color = new Color(0.26f, 0.055f, 0.48f, 0.96f); /* [InfernoLobby patch] */
		RectTransform rect = button.GetComponent<RectTransform>();
		pauseHudButtonRect = rect;
		ApplyAnchor(rect, TextAnchor.UpperRight);
		rect.anchoredPosition = new Vector2(-56f, -128f);
		rect.sizeDelta = new Vector2(116.2f, 103.6f);
		Image surface = MakeUiBlock(button.transform, "Pause Dark Surface", Vector2.zero, new Vector2(106.4f, 93.8f), TextAnchor.MiddleCenter, new Color(0.025f, 0.006f, 0.03f, 0.98f));
		surface.sprite = roundedGlassSprite;
		surface.type = Image.Type.Sliced;
		surface.raycastTarget = false;
		button.targetGraphic = surface;
		for (int i = 0; i < 2; i++)
		{
			// original: 9.8x40.6 bars at +-11.2 (tools/apply_lobby_patches.py)
			Image bar = MakeUiBlock(button.transform, "Pause Bar", new Vector2(i == 0 ? -14f : 14f, 0f), new Vector2(14f, 50f), TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 1f)); /* [InfernoLobby patch] */
			bar.sprite = roundedGlassSprite;
			bar.type = Image.Type.Sliced;
			bar.raycastTarget = false;
		}
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.08f, 1.02f, 1.08f, 1f);
		colors.pressedColor = new Color(0.72f, 0.58f, 0.7f, 1f);
		colors.fadeDuration = 0.06f;
		button.colors = colors;
		button.onClick.AddListener(PauseRun);
		/* [InfernoLobby patch] */ InfernoDriving.StylePauseButton(this);
	}

	private void MakeHudBrakeButton(Transform parent)
	{
		Button button = MakeHudActionButton(parent, "BRAKE", new Vector2(-34f, 168f), new Vector2(232.8f, 116.4f), new Color(1f, 0.12f, 0.32f, 1f));
		brakeHudButton = button;
		brakeHudButtonRect = button.GetComponent<RectTransform>();
		/* [InfernoLobby patch] */ brakeHudButtonRect.gameObject.SetActive(false); brakeHudButtonRect = null;
		EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
		EventTrigger.Entry down = new EventTrigger.Entry();
		down.eventID = EventTriggerType.PointerDown;
		down.callback.AddListener(data =>
		{
			brakeButtonPointers.Add(((PointerEventData)data).pointerId);
			brakeButtonHeld = brakeButtonPointers.Count > 0;
		});
		trigger.triggers.Add(down);
		EventTrigger.Entry up = new EventTrigger.Entry();
		up.eventID = EventTriggerType.PointerUp;
		up.callback.AddListener(ReleaseBrakePointer);
		trigger.triggers.Add(up);
		EventTrigger.Entry cancel = new EventTrigger.Entry();
		cancel.eventID = EventTriggerType.Cancel;
		cancel.callback.AddListener(ReleaseBrakePointer);
		trigger.triggers.Add(cancel);
	}

	private void ReleaseBrakePointer(BaseEventData data)
	{
		brakeButtonPointers.Remove(((PointerEventData)data).pointerId);
		brakeButtonHeld = brakeButtonPointers.Count > 0;
	}

	private void MakeHudHighBeamButton(Transform parent)
	{
		// original position: new Vector2(-296f, 168f) (tools/apply_lobby_patches.py)
		Button button = MakeHudActionButton(parent, "BEAM", new Vector2(-150f, 168f), new Vector2(232.8f, 116.4f), new Color(0.68f, 0.34f, 1f, 1f)); /* [InfernoLobby patch] */
		highBeamHudButton = button;
		highBeamHudButtonRect = button.GetComponent<RectTransform>();
		Image mask = MakeUiBlock(button.transform, "BEAM Cooldown Mask", Vector2.zero, new Vector2(228.8f, 112.4f), TextAnchor.MiddleCenter, Color.white);
		mask.sprite = roundedGlassSprite;
		mask.type = Image.Type.Sliced;
		mask.raycastTarget = false;
		mask.gameObject.AddComponent<Mask>().showMaskGraphic = false;
		mask.rectTransform.SetSiblingIndex(2);
		highBeamCooldownFill = MakeUiBlock(mask.transform, "BEAM Recharge Fill", Vector2.zero, new Vector2(228.8f, 112.4f), TextAnchor.MiddleCenter, new Color(0.56f, 0.16f, 0.94f, 0.58f));
		highBeamCooldownFill.sprite = roundedGlassSprite;
		highBeamCooldownFill.type = Image.Type.Filled;
		highBeamCooldownFill.fillMethod = Image.FillMethod.Vertical;
		highBeamCooldownFill.fillOrigin = (int)Image.OriginVertical.Bottom;
		highBeamCooldownFill.fillAmount = 0f;
		highBeamCooldownFill.raycastTarget = false;
		highBeamCooldownFill.gameObject.SetActive(false);
		EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
		EventTrigger.Entry down = new EventTrigger.Entry();
		down.eventID = EventTriggerType.PointerDown;
		down.callback.AddListener(data =>
		{
			TriggerHighBeamFlash();
			NotifyFirstRunTutorialBeamPressed();
		});
		trigger.triggers.Add(down);
		/* [InfernoLobby patch] */ highBeamHudButtonRect.localScale = new Vector3(1.3f, 1.3f, 1f);
	}

	private Button MakeHudActionButton(Transform parent, string label, Vector2 pos, Vector2 size, Color accent)
	{
		Button button = new GameObject(label + " Button").AddComponent<Button>();
		button.transform.SetParent(parent, false);
		Image frame = button.gameObject.AddComponent<Image>();
		frame.sprite = roundedGlassSprite;
		frame.type = Image.Type.Sliced;
		frame.color = new Color(accent.r, accent.g, accent.b, 0.74f);
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.LowerRight);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		Image glow = MakeUiBlock(button.transform, label + " Glow", Vector2.zero, size + new Vector2(18f, 14f), TextAnchor.MiddleCenter, new Color(accent.r, accent.g, accent.b, 0.12f));
		glow.sprite = roundedGlassSprite;
		glow.type = Image.Type.Sliced;
		glow.raycastTarget = false;
		Image surface = MakeUiBlock(button.transform, label + " Surface", Vector2.zero, size - new Vector2(4f, 4f), TextAnchor.MiddleCenter, new Color(0.018f, 0.009f, 0.035f, 0.94f));
		surface.sprite = roundedGlassSprite;
		surface.type = Image.Type.Sliced;
		surface.raycastTarget = false;
		button.targetGraphic = surface;
		Image sheen = MakeUiBlock(button.transform, label + " Upper Sheen", new Vector2(0f, size.y * 0.5f - 8f), new Vector2(size.x - 30f, 2f), TextAnchor.MiddleCenter, new Color(0.92f, 0.72f, 1f, 0.28f));
		sheen.raycastTarget = false;
		Image innerGlow = MakeUiBlock(button.transform, label + " Inner Glow", new Vector2(-size.x * 0.2f, 0f), new Vector2(size.x * 0.5f, size.y - 18f), TextAnchor.MiddleCenter, new Color(accent.r, accent.g, accent.b, 0.075f));
		innerGlow.sprite = roundedGlowSprite;
		innerGlow.type = Image.Type.Sliced;
		innerGlow.raycastTarget = false;
		Image accentLine = MakeUiBlock(button.transform, label + " Accent", new Vector2(0f, -size.y * 0.5f + 7f), new Vector2(size.x * 0.48f, 3f), TextAnchor.MiddleCenter, new Color(accent.r, accent.g, accent.b, 0.94f));
		accentLine.raycastTarget = false;
		MakeHudActionIcon(button.transform, label, size, accent);
		Text text = MakeText(button.transform, label, 32, TextAnchor.MiddleCenter, new Vector2(34f, 2f));
		text.color = new Color(0.98f, 0.94f, 1f, 1f);
		text.fontStyle = FontStyle.Bold;
		text.raycastTarget = false;
		text.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x - 72f, size.y);
		text.transform.SetAsLastSibling();
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.08f, 1.02f, 1.12f, 1f);
		colors.pressedColor = new Color(0.72f, 0.64f, 0.82f, 1f);
		colors.selectedColor = colors.highlightedColor;
		colors.disabledColor = new Color(0.35f, 0.32f, 0.42f, 0.72f);
		colors.colorMultiplier = 1f;
		colors.fadeDuration = 0.06f;
		button.colors = colors;
		return button;
	}

	private void MakeHudActionIcon(Transform parent, string label, Vector2 size, Color accent)
	{
		Color bright = new Color(Mathf.Lerp(accent.r, 1f, 0.42f), Mathf.Lerp(accent.g, 1f, 0.42f), Mathf.Lerp(accent.b, 1f, 0.42f), 0.98f);
		Vector2 iconPos = new Vector2(-70f, 0f);
		Image halo = MakeUiBlock(parent, label + " Icon Halo", iconPos, new Vector2(62f, 62f), TextAnchor.MiddleCenter, new Color(accent.r, accent.g, accent.b, 0.13f));
		halo.sprite = softCircleSprite;
		halo.raycastTarget = false;
		Sprite supplied = LoadResourceSprite(label == "BRAKE" ? "тормоза иконка" : "бим иконка");
		if (supplied != null)
		{
			halo.color = new Color(bright.r, bright.g, bright.b, 0.96f);
			halo.rectTransform.sizeDelta = new Vector2(54f, 54f);
			Image icon = MakeUiBlock(parent, label + " Supplied Icon", iconPos, new Vector2(39f, 39f), TextAnchor.MiddleCenter, new Color(0.018f, 0.008f, 0.032f, 1f));
			icon.sprite = supplied;
			icon.type = Image.Type.Simple;
			icon.preserveAspect = true;
			icon.raycastTarget = false;
			return;
		}
		if (label == "BRAKE")
		{
			Image disc = MakeUiBlock(parent, "Brake Disc", iconPos, new Vector2(46f, 46f), TextAnchor.MiddleCenter, bright);
			disc.sprite = softCircleSprite;
			disc.raycastTarget = false;
			Image hub = MakeUiBlock(parent, "Brake Hub", iconPos, new Vector2(30f, 30f), TextAnchor.MiddleCenter, new Color(0.025f, 0.01f, 0.038f, 1f));
			hub.sprite = softCircleSprite;
			hub.raycastTarget = false;
			Image axle = MakeUiBlock(parent, "Brake Axle", iconPos, new Vector2(10f, 10f), TextAnchor.MiddleCenter, bright);
			axle.sprite = softCircleSprite;
			axle.raycastTarget = false;
			Image caliper = MakeUiBlock(parent, "Brake Caliper", iconPos + new Vector2(19f, 0f), new Vector2(10f, 34f), TextAnchor.MiddleCenter, new Color(1f, 0.72f, 0.8f, 1f));
			caliper.sprite = roundedGlassSprite;
			caliper.type = Image.Type.Sliced;
			caliper.raycastTarget = false;
			return;
		}
		Image lamp = MakeUiBlock(parent, "Beam Lamp", iconPos + new Vector2(-7f, 0f), new Vector2(28f, 42f), TextAnchor.MiddleCenter, bright);
		lamp.sprite = roundedGlassSprite;
		lamp.type = Image.Type.Sliced;
		lamp.raycastTarget = false;
		for (int i = -1; i <= 1; i++)
		{
			Image ray = MakeUiBlock(parent, "Beam Ray", iconPos + new Vector2(16f, i * 15f), new Vector2(28f, 5f), TextAnchor.MiddleCenter, bright);
			ray.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -i * 11f);
			ray.raycastTarget = false;
		}
	}

	private void MakeOverlay(Transform parent)
	{
		overlay = new GameObject("Overlay");
		overlay.transform.SetParent(parent);
		overlay.AddComponent<Image>().color = Color.clear;
		overlayRect = overlay.GetComponent<RectTransform>();
		overlayRect.anchorMin = Vector2.zero;
		overlayRect.anchorMax = Vector2.one;
		overlayRect.offsetMin = Vector2.zero;
		overlayRect.offsetMax = Vector2.zero;
		menuSceneRoot = new GameObject("Menu Scene").AddComponent<RectTransform>();
		menuSceneRoot.SetParent(overlay.transform);
		menuSceneRoot.anchorMin = Vector2.zero;
		menuSceneRoot.anchorMax = Vector2.one;
		menuSceneRoot.offsetMin = Vector2.zero;
		menuSceneRoot.offsetMax = Vector2.zero;
		menuParallax.Add(menuSceneRoot);
		MakeMenuBackdrop(menuSceneRoot);
		menuSafeRoot = new GameObject("Menu Safe Area").AddComponent<RectTransform>();
		menuSafeRoot.SetParent(overlay.transform);
		menuSafeRoot.anchorMin = Vector2.zero;
		menuSafeRoot.anchorMax = Vector2.one;
		menuSafeRoot.offsetMin = Vector2.zero;
		menuSafeRoot.offsetMax = Vector2.zero;
		menuTabsRoot = new GameObject("Menu Tabs").AddComponent<RectTransform>();
		menuTabsRoot.SetParent(menuSafeRoot);
		menuTabsRoot.anchorMin = Vector2.zero;
		menuTabsRoot.anchorMax = Vector2.one;
		menuTabsRoot.offsetMin = Vector2.zero;
		menuTabsRoot.offsetMax = Vector2.zero;
		menuMainTabRoot = new GameObject("Main Menu Tab").AddComponent<RectTransform>();
		menuMainTabRoot.SetParent(menuTabsRoot);
		menuMainTabRoot.anchorMin = Vector2.zero;
		menuMainTabRoot.anchorMax = Vector2.one;
		menuMainTabRoot.offsetMin = Vector2.zero;
		menuMainTabRoot.offsetMax = Vector2.zero;
		menuTitleRoot = new GameObject("Menu Title").AddComponent<RectTransform>();
		menuTitleRoot.SetParent(menuSafeRoot);
		ApplyAnchor(menuTitleRoot, TextAnchor.UpperCenter);
		menuTitleRoot.anchoredPosition = new Vector2(0f, -250f);
		menuTitleRoot.sizeDelta = new Vector2(980f, 300f);
		MakeSpacedWord(menuTitleRoot, "NIGHT", new Vector2(0f, 0f), 138, 168f, new Color(0.98f, 0.95f, 0.91f), true);
		MakeSpacedWord(menuTitleRoot, "RUNNER", new Vector2(0f, -158f), 48, 128f, new Color(0.92f, 0.018f, 0.035f), false);
		overlayTitle = MakeText(menuSafeRoot, "", 96, TextAnchor.UpperCenter, new Vector2(0f, -246f));
		overlayTitle.color = new Color(0.97f, 0.93f, 0.9f);
		overlayTitle.raycastTarget = false;
		overlayTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(1040f, 180f);
		overlayTag = MakeText(menuSafeRoot, "", 48, TextAnchor.UpperCenter, new Vector2(0f, -470f));
		overlayTag.color = new Color(0.9f, 0.025f, 0.04f);
		overlayTag.raycastTarget = false;
		overlayTag.GetComponent<RectTransform>().sizeDelta = new Vector2(940f, 80f);
		menuDetails = new GameObject("Menu Details");
		menuDetails.transform.SetParent(menuSafeRoot);
		RectTransform detailsRect = menuDetails.AddComponent<RectTransform>();
		ApplyAnchor(detailsRect, TextAnchor.LowerCenter);
		detailsRect.anchoredPosition = new Vector2(0f, 650f);
		detailsRect.sizeDelta = new Vector2(820f, 70f);
		overlayStats = MakeText(menuDetails.transform, "", 22, TextAnchor.MiddleCenter, Vector2.zero);
		overlayStats.color = new Color(0.82f, 0.74f, 0.78f);
		overlayStats.raycastTarget = false;
		overlayStats.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 80f);
		MakeHomeMenu(menuMainTabRoot);
		MakeGemCounterWidget(menuSafeRoot);
		MakeSettingsScreen(menuTabsRoot);
		MakeGarageScreen(menuTabsRoot);
		MakeMenuTabBar(menuSafeRoot);
		MakeGemShopOverlay(menuSafeRoot);
		MakeCaseOverlay(menuSafeRoot);
		MakePauseActions(menuSafeRoot);
		MakeGameOverScreen(overlay.transform);
		PositionMenuTabs(true);
		SelectMenuTab(1, true);
		ApplySafeArea();
	}

	private void MakeGameOverScreen(Transform parent)
	{
		/* [InfernoLobby patch] */ InfernoLobby.BuildGameOver(this, parent);
	}

	private void MakePauseActions(Transform parent)
	{
		pauseActionsRoot = new GameObject("Pause Actions");
		pauseActionsRoot.transform.SetParent(parent, false);
		RectTransform rect = pauseActionsRoot.AddComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		Image shade = MakeUiBlock(pauseActionsRoot.transform, "Pause Controls Soft Shade", new Vector2(0f, -30f), new Vector2(820f, 1460f), TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.8f));
		shade.sprite = caseRadialGlowSprite;
		shade.type = Image.Type.Simple;
		shade.raycastTarget = false;
		pauseContinueButton = MakePauseArtButton(pauseActionsRoot.transform, "Pause Continue", "кнопка из паузы", new Vector2(0f, 143.75f), new Vector2(640f, 160f));
		pauseContinueButton.onClick.AddListener(ResumeRun);
		pauseMenuButton = MakePauseArtButton(pauseActionsRoot.transform, "Pause Back To Menu", "back to menu button", new Vector2(0f, 21.25f), new Vector2(640f, 160f));
		pauseMenuButton.onClick.AddListener(ShowMainMenu);
		pauseSoundButton = MakeSoundToggleButton(pauseActionsRoot.transform, "Pause Sound Toggle", TextAnchor.UpperLeft, new Vector2(82f, -92f), new Vector2(82f, 82f), out pauseSoundIcon);
		pauseSoundButton.onClick.AddListener(ToggleGameSound);
		if (pauseSoundIcon != null)
		{
			pauseSoundIcon.rectTransform.sizeDelta = new Vector2(63f, 63f);
			pauseSoundIcon.rectTransform.anchoredPosition = new Vector2(0f, -5f);
		}
		pauseActionsRoot.SetActive(false);
	}

	private Button MakePauseArtButton(Transform parent, string objectName, string resource, Vector2 position, Vector2 size)
	{
		Image art = MakeReferenceArtImage(parent, objectName, resource, position, size, TextAnchor.MiddleCenter);
		art.raycastTarget = true;
		art.preserveAspect = true;
		Button button = art.gameObject.AddComponent<Button>();
		button.targetGraphic = art;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.06f, 1.03f, 1.08f, 1f);
		colors.pressedColor = new Color(0.8f, 0.74f, 0.88f, 1f);
		colors.selectedColor = colors.highlightedColor;
		colors.fadeDuration = 0.08f;
		button.colors = colors;
		return button;
	}

	private Button MakeSoundToggleButton(Transform parent, string objectName, TextAnchor anchor, Vector2 position, Vector2 size, out Image icon)
	{
		Button button = new GameObject(objectName).AddComponent<Button>();
		button.transform.SetParent(parent, false);
		Image frame = button.gameObject.AddComponent<Image>();
		frame.sprite = roundedGlassSprite != null ? roundedGlassSprite : softCircleSprite;
		frame.type = Image.Type.Sliced;
		frame.color = new Color(0.96f, 0.045f, 0.4f, 0.96f);
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = position;
		rect.sizeDelta = size;
		button.targetGraphic = frame;
		Image surface = MakeUiBlock(button.transform, objectName + " Surface", Vector2.zero, size - new Vector2(6f, 6f), TextAnchor.MiddleCenter, new Color(0.01f, 0.004f, 0.022f, 0.98f));
		surface.sprite = roundedGlassSprite != null ? roundedGlassSprite : softCircleSprite;
		surface.type = Image.Type.Sliced;
		surface.raycastTarget = false;
		icon = MakeTabIcon(button.transform, objectName + " Icon", gameSoundEnabled ? "звук есть" : "звука нет", size * 0.66f, Vector2.zero);
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.08f, 1.02f, 1.06f, 1f);
		colors.pressedColor = new Color(0.72f, 0.62f, 0.7f, 1f);
		colors.selectedColor = colors.highlightedColor;
		colors.fadeDuration = 0.07f;
		button.colors = colors;
		return button;
	}

	private Text MakeGameOverValue(Transform parent, Vector2 position)
	{
		Text text = MakeText(parent, "0.00 KM", 33, TextAnchor.MiddleCenter, position);
		text.alignment = TextAnchor.MiddleRight;
		text.fontStyle = FontStyle.Bold;
		text.color = new Color(0.77f, 0.25f, 1f, 1f);
		text.raycastTarget = false;
		text.resizeTextForBestFit = true;
		text.resizeTextMinSize = 22;
		text.resizeTextMaxSize = 33;
		text.rectTransform.sizeDelta = new Vector2(300f, 64f);
		return text;
	}

	private Button MakeGameOverArtButton(Transform parent, string objectName, string resource, Vector2 position, Vector2 size)
	{
		Image art = MakeReferenceArtImage(parent, objectName, resource, position, size, TextAnchor.LowerCenter);
		art.raycastTarget = true;
		art.preserveAspect = true;
		Button button = art.gameObject.AddComponent<Button>();
		button.targetGraphic = art;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.06f, 1.03f, 1.08f, 1f);
		colors.pressedColor = new Color(0.82f, 0.78f, 0.9f, 1f);
		colors.selectedColor = colors.highlightedColor;
		colors.fadeDuration = 0.08f;
		button.colors = colors;
		return button;
	}

	private Image MakeReferenceArtImage(Transform parent, string objectName, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		Image image = new GameObject(objectName).AddComponent<Image>();
		image.transform.SetParent(parent, false);
		image.sprite = LoadTightMenuArtSprite(resource);
		image.preserveAspect = true;
		image.type = Image.Type.Simple;
		image.color = Color.white;
		image.raycastTarget = false;
		if (image.sprite != null && image.sprite.texture != null)
		{
			image.sprite.texture.filterMode = FilterMode.Bilinear;
			image.sprite.texture.wrapMode = TextureWrapMode.Clamp;
			image.sprite.texture.anisoLevel = 1;
		}
		RectTransform rect = image.rectTransform;
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		return image;
	}

	private Button MakeReferenceArtButton(Transform parent, string objectName, string resource, Vector2 pos, Vector2 size, TextAnchor anchor)
	{
		Image art = MakeReferenceArtImage(parent, objectName, resource, pos, size, anchor);
		art.raycastTarget = true;
		Button button = art.gameObject.AddComponent<Button>();
		button.targetGraphic = art;
		button.transition = Selectable.Transition.ColorTint;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.04f, 1.04f, 1.04f, 1f);
		colors.pressedColor = new Color(0.88f, 0.84f, 0.94f, 1f);
		colors.fadeDuration = 0.08f;
		button.colors = colors;
		Text label = MakeText(button.transform, "", 1, TextAnchor.MiddleCenter, Vector2.zero);
		label.name = "Button Label";
		label.color = Color.clear;
		label.raycastTarget = false;
		label.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		return button;
	}

	private void MakeMenuBackdrop(Transform parent)
	{
		/* [InfernoLobby patch] */ InfernoLobby.Backdrop(this, parent);
	}

	private void ApplySafeArea()
	{
		if (menuSafeRoot == null)
		{
			return;
		}
		Rect safeArea = Screen.safeArea;
		if (Screen.width <= 0 || Screen.height <= 0)
		{
			return;
		}
		if (lastSafePos == safeArea.position && lastSafeSize == safeArea.size)
		{
			PositionGemCounterSafeArea();
			LayoutHomeMenu();
			return;
		}
		lastSafePos = safeArea.position;
		lastSafeSize = safeArea.size;
		menuSafeRoot.anchorMin = Vector2.zero;
		menuSafeRoot.anchorMax = Vector2.one;
		menuSafeRoot.offsetMin = Vector2.zero;
		menuSafeRoot.offsetMax = Vector2.zero;
		PositionGemCounterSafeArea();
		if (menuBackgroundRect != null && overlay != null)
		{
			if (overlayRect == null)
			{
				overlayRect = overlay.GetComponent<RectTransform>();
			}
			float width = overlayRect.rect.width;
			float height = overlayRect.rect.height;
			if (width <= 0f || height <= 0f)
			{
				width = 1080f;
				height = 1920f;
			}
			float aspect = menuBackgroundAspect > 0f ? menuBackgroundAspect : 0.5625f;
			menuBackgroundRect.sizeDelta = width / height > aspect ? new Vector2(width, width / aspect) : new Vector2(height * aspect, height);
		}
		LayoutHomeMenu();
		PositionMenuTabs(true);
	}
}
