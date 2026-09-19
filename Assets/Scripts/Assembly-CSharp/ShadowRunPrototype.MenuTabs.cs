using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Translated from Il2CppOutput Assembly-CSharp.cpp:
//   ShadowRunPrototype_MakeMenuTabBar_mCC2FF52704A4702A1C2D79CA1B7D5332561EBE0D
//   ShadowRunPrototype_MakeMenuTabButton_m55A5E3A823FE95699E04F47072456A87DDBC32F4
//   U3CU3Ec__DisplayClass445_0_U3CMakeMenuTabButtonU3Eb__0 (Assembly-CSharp__1.cpp)
//   ShadowRunPrototype_MakeTabIcon_m8AA13CFA5C3ACB9D5249B7189305E81773474048
//   ShadowRunPrototype_GetMenuTabIconTintMaterial_mEB6997F474ED678D247295169AA9F84624212AA5
//   ShadowRunPrototype_MakeTabRawIcon_mFB1E52A364449FDE3492AF6909F2039B37DD2B00
//   ShadowRunPrototype_MakeTabIconGlow_m231394A27C4F0685892D4FD272331BD0300B011C
//   ShadowRunPrototype_UpdateMenuTabBar_mD19E687ED993C42570766B40BE235B86B359BD36
//   ShadowRunPrototype_SetReferenceTabVisual_m5A54F7150AB9872E8FE97CF84F1DC2C8AC63C404
//   ShadowRunPrototype_SelectMenuTab_m41933C8C8327730EF00E9902959EF35B8E6F410E
//   ShadowRunPrototype_BeginMenuTabTransition_m2D96F14ECCF2386AF97835BE4690101867D8116B
//   ShadowRunPrototype_PositionMenuTabs_m3E74C6583F0AD3AED22403ABDE5FC4F3CF870289
//   ShadowRunPrototype_MakeMenuPageHeader_m80A383D28C12CC886B0736674B778B72D27471A5
//   ShadowRunPrototype_LayoutMenuPageHeaders_m8E2A8D5EA5C1993758D8B74F0E1A099B090E7599
//   ShadowRunPrototype_LayoutMenuPageHeader_m26581E6BA261351EE6F9FFFDC1B75EDCBB7BD643
//   ShadowRunPrototype_SetMenuPagePosition_m534C3FC3FE5B1EEE291913F70E514148C85F521D
//   ShadowRunPrototype_GetMenuViewportWidth_m3B6ECA517A3F0B9BAAE4111EC225DDF84EA0E9CF
//   ShadowRunPrototype_UpdateMenuAnimation_mE0BD3E5E79E99386D0753EE34FA8F2EBC006961E
//   ShadowRunPrototype_UpdateHomeDriveShine_m05840F959B50E45506B545C4218E7E0998901998
//   ShadowRunPrototype_LoadTightMenuArtSprite_m94D7A2198A2E6BE8FADCE208ABFD79225C14FD21
public sealed partial class ShadowRunPrototype
{
	private void MakeMenuTabBar(Transform parent)
	{
		InfernoLobby.Tabs(this, parent); /* [InfernoLobby patch] */
	}

	private void MakeMenuTabButton(string label, string iconResource, float x, float iconSize, int tabIndex, out Text labelText, out Image icon, out CanvasGroup glow)
	{
		Button button = new GameObject(label + " Tab").AddComponent<Button>();
		button.transform.SetParent(menuTabBarRoot);
		Image background = button.gameObject.AddComponent<Image>();
		background.color = Color.clear;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.MiddleCenter);
		rect.anchoredPosition = new Vector2(x, 0f);
		rect.sizeDelta = new Vector2(246f, 150f);
		button.targetGraphic = background;
		button.transition = Selectable.Transition.None;
		button.onClick.AddListener(() => SelectMenuTab(tabIndex, false));
		Material tint = GetMenuTabIconTintMaterial();
		glow = MakeTabIconGlow(button.transform, label + " Icon Photo Glow", new Vector2(210f, 132f), new Vector2(0f, 26f));
		icon = MakeTabIcon(button.transform, label + " Icon", iconResource, new Vector2(iconSize, iconSize), new Vector2(0f, 26f));
		icon.material = tint;
		icon.color = icon.sprite != null ? new Color(0.43f, 0.48f, 0.58f, 0.88f) : Color.clear;
		labelText = MakeText(button.transform, label, 23, TextAnchor.MiddleCenter, new Vector2(0f, -51f));
		labelText.fontStyle = FontStyle.Bold;
		labelText.color = new Color(0.58f, 0.55f, 0.82f, 1f);
		labelText.GetComponent<RectTransform>().sizeDelta = new Vector2(250f, 44f);
		labelText.raycastTarget = false;
	}

	private Image MakeTabIcon(Transform parent, string name, string resource, Vector2 size, Vector2 pos)
	{
		Image image = new GameObject(name).AddComponent<Image>();
		image.transform.SetParent(parent, false);
		image.sprite = LoadResourceSprite(resource);
		if (image.sprite != null && image.sprite.texture != null)
		{
			image.sprite.texture.filterMode = FilterMode.Bilinear;
			image.sprite.texture.anisoLevel = 8;
			image.sprite.texture.wrapMode = TextureWrapMode.Clamp;
		}
		image.preserveAspect = true;
		image.raycastTarget = false;
		if (image.sprite == null)
		{
			image.color = Color.clear;
		}
		RectTransform rect = image.rectTransform;
		ApplyAnchor(rect, TextAnchor.MiddleCenter);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		return image;
	}

	private Material GetMenuTabIconTintMaterial()
	{
		if (menuTabIconTintMaterial != null)
		{
			return menuTabIconTintMaterial;
		}
		Shader shader = Shader.Find("UI/IconAlphaTint");
		if (shader == null)
		{
			return null;
		}
		menuTabIconTintMaterial = new Material(shader)
		{
			name = "Menu Tab Icon Alpha Tint"
		};
		return menuTabIconTintMaterial;
	}

	private RawImage MakeTabRawIcon(Transform parent, string name, Texture texture, Vector2 size, Vector2 pos)
	{
		RawImage image = new GameObject(name).AddComponent<RawImage>();
		image.transform.SetParent(parent);
		image.texture = texture;
		image.raycastTarget = false;
		RectTransform rect = image.rectTransform;
		ApplyAnchor(rect, TextAnchor.MiddleCenter);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		return image;
	}

	private CanvasGroup MakeTabIconGlow(Transform parent, string name, Vector2 size, Vector2 pos)
	{
		RectTransform root = new GameObject(name).AddComponent<RectTransform>();
		root.SetParent(parent, false);
		ApplyAnchor(root, TextAnchor.MiddleCenter);
		root.anchoredPosition = pos;
		root.sizeDelta = size;
		CanvasGroup group = root.gameObject.AddComponent<CanvasGroup>();
		group.alpha = 0f;
		group.blocksRaycasts = false;
		group.interactable = false;
		Image glow = MakeUiBlock(root, "Photo Glow", Vector2.zero, size, TextAnchor.MiddleCenter, new Color(0.69f, 0.38f, 1f, 0.16f));
		glow.sprite = caseRadialGlowSprite;
		glow.raycastTarget = false;
		Image core = MakeUiBlock(root, "Photo Glow Core", Vector2.zero, size * 0.58f, TextAnchor.MiddleCenter, new Color(0.86f, 0.66f, 1f, 0.1f));
		core.sprite = caseRadialGlowSprite;
		core.raycastTarget = false;
		return group;
	}

	private void UpdateMenuTabBar(float tabPosition)
	{
		tabPosition = Mathf.Clamp(tabPosition, 0f, 2f);
		if (menuTabActiveRoot != null)
		{
			menuTabActiveRoot.anchoredPosition = new Vector2((tabPosition - 1f) * 281f, 0f);
		}
		SetReferenceTabVisual(garageTabLabel, garageTabIcon, garageTabGlow, 1f - Mathf.Clamp01(Mathf.Abs(tabPosition)));
		SetReferenceTabVisual(homeTabLabel, homeTabIcon, homeTabGlow, 1f - Mathf.Clamp01(Mathf.Abs(tabPosition - 1f)));
		SetReferenceTabVisual(profileTabLabel, profileTabIcon, profileTabGlow, 1f - Mathf.Clamp01(Mathf.Abs(tabPosition - 2f)));
	}

	private void SetReferenceTabVisual(Text label, Image icon, CanvasGroup glow, float weight)
	{
		weight = Mathf.Clamp01(weight);
		Color iconIdle = new Color(0.42f, 0.47f, 0.58f, 0.84f);
		Color labelIdle = new Color(0.47f, 0.52f, 0.64f, 1f);
		Color labelActive = new Color(0.86f, 0.63f, 1f, 0.96f);
		Color iconActive = new Color(1f, 0.34f, 0.84f, 1f);
		if (label != null)
		{
			label.color = Color.Lerp(labelIdle, labelActive, weight);
		}
		if (icon != null)
		{
			icon.color = Color.Lerp(iconIdle, iconActive, weight);
		}
		if (glow != null)
		{
			glow.alpha = weight;
			glow.gameObject.SetActive(weight > 0.001f);
		}
	}

	private void SelectMenuTab(int tab, bool instant)
	{
		InfernoStyle.CloseSheet(this); /* [InfernoLobby patch] */
		InfernoLobby.Show(this); /* [InfernoLobby patch] */
		menuTargetTab = Mathf.Clamp(tab, 0, 2);
		if (settingsRoot != null)
		{
			settingsRoot.SetActive(true);
		}
		if (garageRoot != null)
		{
			garageRoot.SetActive(true);
		}
		if (menuMainTabRoot != null)
		{
			menuMainTabRoot.gameObject.SetActive(true);
		}
		SetGaragePreviewVisible(menuTargetTab == 0);
		if (menuTargetTab == 2)
		{
			RefreshProfileUi();
		}
		if (instant)
		{
			menuActiveTab = menuTargetTab;
		}
		BeginMenuTabTransition(instant);
		PositionMenuTabs(instant);
		UpdateMenuTabBar(menuTargetTab);
	}

	private void BeginMenuTabTransition(bool instant)
	{
		if (menuTabsRoot == null || overlayRect == null)
		{
			return;
		}
		float width = GetMenuViewportWidth();
		menuTabStartX = menuTabsRoot.anchoredPosition.x;
		menuTabEndX = -menuTargetTab * width;
		menuTabTweenElapsed = instant ? 0.13f : 0f;
	}

	private void PositionMenuTabs(bool instant)
	{
		InfernoLobby.PositionPages(this); /* [InfernoLobby patch] */
	}

	private RectTransform MakeMenuPageHeader(Transform parent, string titleText, string subtitleText, out Text subtitle)
	{
		RectTransform root = new GameObject(titleText + " Page Header").AddComponent<RectTransform>();
		root.SetParent(parent, false);
		root.anchorMin = new Vector2(0f, 1f);
		root.anchorMax = new Vector2(0f, 1f);
		root.pivot = new Vector2(0f, 1f);
		root.localScale = Vector3.one;
		root.sizeDelta = new Vector2(620f, 112f);
		Text title = MakeText(root, titleText, 48, TextAnchor.UpperLeft, Vector2.zero);
		title.fontStyle = FontStyle.Bold;
		title.color = Color.white;
		title.resizeTextForBestFit = true;
		title.resizeTextMinSize = 34;
		title.resizeTextMaxSize = 48;
		RectTransform titleRect = title.GetComponent<RectTransform>();
		titleRect.anchorMin = new Vector2(0f, 1f);
		titleRect.anchorMax = new Vector2(1f, 1f);
		titleRect.pivot = new Vector2(0f, 1f);
		titleRect.anchoredPosition = Vector2.zero;
		titleRect.sizeDelta = new Vector2(0f, 58f);
		subtitle = null;
		if (!string.IsNullOrEmpty(subtitleText))
		{
			subtitle = MakeText(root, subtitleText, 22, TextAnchor.UpperLeft, Vector2.zero);
			subtitle.fontStyle = FontStyle.Normal;
			subtitle.color = new Color(0.68f, 0.54f, 0.86f, 0.92f);
			subtitle.resizeTextForBestFit = true;
			subtitle.resizeTextMinSize = 16;
			subtitle.resizeTextMaxSize = 22;
			RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
			subtitleRect.anchorMin = new Vector2(0f, 1f);
			subtitleRect.anchorMax = new Vector2(1f, 1f);
			subtitleRect.pivot = new Vector2(0f, 1f);
			subtitleRect.anchoredPosition = new Vector2(2f, -58f);
			subtitleRect.sizeDelta = new Vector2(0f, 38f);
		}
		LayoutMenuPageHeaders();
		return root;
	}

	private void LayoutMenuPageHeaders()
	{
		float width = overlayRect != null && overlayRect.rect.width > 0f ? overlayRect.rect.width : 1080f;
		float height = overlayRect != null && overlayRect.rect.height > 0f ? overlayRect.rect.height : 1920f;
		float topInset = 0f;
		if (Screen.height > 0)
		{
			topInset = (Screen.height - Screen.safeArea.yMax) / Screen.height * height;
		}
		float left = Mathf.Clamp(width * 0.055f, 52f, 72f);
		float top = Mathf.Max(34f, topInset + 18f);
		float headerWidth = Mathf.Clamp(width - left * 2f, 520f, 650f);
		LayoutMenuPageHeader(homePageHeaderRoot, left, top, headerWidth);
		// original: LayoutMenuPageHeader(garagePageHeaderRoot, left, top, headerWidth);
		LayoutMenuPageHeader(garagePageHeaderRoot, (width - headerWidth) * 0.5f, top + 90f, headerWidth); /* [InfernoLobby patch] */
		InfernoSettings.CenterHeader(garagePageHeaderRoot); /* [InfernoLobby patch] */
		// original: LayoutMenuPageHeader(profilePageHeaderRoot, left, top, headerWidth);
		LayoutMenuPageHeader(profilePageHeaderRoot, (width - headerWidth) * 0.5f, top + 90f, headerWidth); /* [InfernoLobby patch] */
		InfernoSettings.CenterHeader(profilePageHeaderRoot); /* [InfernoLobby patch] */
		LayoutProfileScreen();
	}

	private static void LayoutMenuPageHeader(RectTransform root, float left, float top, float width)
	{
		if (root == null)
		{
			return;
		}
		root.anchoredPosition = new Vector2(left, -top);
		root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		root.localScale = Vector3.one;
	}

	private void SetMenuPagePosition(GameObject page, float index, float width)
	{
		if (page == null)
		{
			return;
		}
		RectTransform rect = page.GetComponent<RectTransform>();
		if (rect == null)
		{
			return;
		}
		rect.anchorMin = new Vector2(0f, 0f);
		rect.anchorMax = new Vector2(0f, 1f);
		rect.pivot = new Vector2(0f, 0.5f);
		rect.sizeDelta = new Vector2(width, 0f);
		rect.anchoredPosition = new Vector2(index * width, 0f);
	}

	private float GetMenuViewportWidth()
	{
		if (menuSafeRoot != null && menuSafeRoot.rect.width > 1f)
		{
			return menuSafeRoot.rect.width;
		}
		return Mathf.Max(1f, overlayRect != null ? overlayRect.rect.width : 1080f);
	}

	private void UpdateMenuAnimation(float dt)
	{
		if (overlay == null || !overlay.activeSelf)
		{
			return;
		}
		ApplySafeArea();
		UpdateDailyGoalsTimer(false);
		UpdateCaseOpening(dt);
		if (menuTabsRoot != null && menuTabsRoot.gameObject.activeSelf)
		{
			PositionMenuTabs(false);
		}
		if (menuSceneRoot != null)
		{
			if (menuSceneRoot.localScale != Vector3.one)
			{
				menuSceneRoot.localScale = Vector3.one;
			}
			if (menuSceneRoot.anchoredPosition != Vector2.zero)
			{
				menuSceneRoot.anchoredPosition = Vector2.zero;
			}
		}
		UpdateHomeDriveShine();
		for (int i = 0; i < menuCloudLayers.Count; i++)
		{
			MenuCloudLayer layer = menuCloudLayers[i];
			if (layer.Root == null)
			{
				continue;
			}
			Vector2 pos = layer.Root.anchoredPosition;
			pos.x += layer.Speed * dt;
			float limit = 640f + layer.Width * 0.5f;
			if (pos.x > limit)
			{
				pos.x = -limit;
			}
			if (pos.x < -limit)
			{
				pos.x = limit;
			}
			layer.Root.anchoredPosition = pos;
		}
	}

	private void UpdateHomeDriveShine()
	{
		if (homeDriveShineRect == null || homeDriveShineImage == null || homeDriveArtRect == null || !homeDriveArtRect.gameObject.activeInHierarchy)
		{
			return;
		}
		float cycle = Mathf.Repeat(Time.unscaledTime, 2.14f);
		if (cycle >= 0.56f)
		{
			Color hidden = homeDriveShineImage.color;
			hidden.a = 0f;
			homeDriveShineImage.color = hidden;
			return;
		}
		float t = Mathf.SmoothStep(0f, 1f, cycle / 0.56f);
		float width = Mathf.Max(700f, homeDriveArtRect.rect.width);
		homeDriveShineRect.anchoredPosition = new Vector2(Mathf.Lerp(-width * 0.62f, width * 0.62f, t), 0f);
		Color color = homeDriveShineImage.color;
		color.a = Mathf.Sin(t * Mathf.PI) * 0.42f;
		homeDriveShineImage.color = color;
	}

	private Sprite LoadTightMenuArtSprite(string resource)
	{
		{ Sprite png = InfernoGarage.Picture(this, resource); if (png != null) return png; } /* [InfernoLobby patch] */
		if (InfernoLobby.IsDiamondResource(resource)) { Sprite gem = InfernoLobby.GemSprite(this); if (gem != null) return gem; } /* [InfernoLobby patch] */
		Texture2D texture = Resources.Load<Texture2D>(resource);
		if (texture == null)
		{
			return LoadResourceSprite(resource);
		}
		Rect rect;
		if (resource == "drive menu final")
		{
			rect = new Rect(182f, 432f, 1171f, 216f);
		}
		else if (resource == "car2menu")
		{
			rect = new Rect(123f, 200f, 1303f, 632f);
		}
		else if (resource == "финал машина меню")
		{
			rect = new Rect(72f, 125f, 1392f, 790f);
		}
		else if (resource == "плашка для меню")
		{
			rect = new Rect(160f, 311f, 1220f, 457f);
		}
		else if (resource == "99")
		{
			rect = new Rect(145f, 265f, 1250f, 525f);
		}
		else if (resource == "101")
		{
			rect = new Rect(204f, 247f, 1128f, 694f);
		}
		else if (resource == "коллект")
		{
			rect = new Rect(580f, 478f, 375f, 92f);
		}
		else if (resource == "гараж кнопка")
		{
			rect = new Rect(209f, 454f, 1119f, 262f);
		}
		else if (resource == "кнопка в гараж")
		{
			float sx = texture.width / 1536f;
			float sy = texture.height / 1024f;
			rect = new Rect(260f * sx, 445f * sy, 1005f * sx, 189f * sy);
		}
		else if (resource == "алмаз 1")
		{
			rect = new Rect(641f, 378f, 273f, 208f);
		}
		else if (resource == "алмаз 3")
		{
			rect = new Rect(526f, 307f, 487f, 438f);
		}
		else if (resource == "сундук")
		{
			rect = new Rect(378f, 208f, 844f, 713f);
		}
		else if (resource == "рамка")
		{
			rect = new Rect(120f, 370f, 1296f, 430f);
		}
		else if (resource == "эпл")
		{
			rect = new Rect(62f, 716f, 904f, 220f);
		}
		else if (resource == "гугл")
		{
			rect = new Rect(60f, 734f, 904f, 206f);
		}
		else if (resource == "кейс кнопка открытия")
		{
			rect = new Rect(120f, 350f, 1296f, 410f);
		}
		else if (resource == "play again button")
		{
			rect = new Rect(161f, 654f, 701f, 156f);
		}
		else if (resource == "back to menu button")
		{
			rect = new Rect(238f, 483f, 1058f, 165f);
		}
		else if (resource == "кнопка из паузы")
		{
			rect = new Rect(320f, 410f, 910f, 230f);
		}
		else if (resource == "diamons_final")
		{
			rect = new Rect(128f, 438f, 1280f, 276f);
		}
		else if (resource == "кристал плашка")
		{
			rect = new Rect(190f, 270f, 1190f, 450f);
		}
		else if (resource == "айди кнопка")
		{
			float sx = texture.width / 1536f;
			float sy = texture.height / 1024f;
			rect = new Rect(337f * sx, 107f * sy, 870f * sx, 870f * sy);
		}
		else if (resource == "case menu final")
		{
			rect = new Rect(108f, 374f, 805f, 1063f);
		}
		else if (resource == "goals menu final")
		{
			rect = new Rect(114f, 409f, 797f, 967f);
		}
		else
		{
			return LoadResourceSprite(resource);
		}
		rect.x = Mathf.Clamp(rect.x, 0f, texture.width - 1f);
		rect.y = Mathf.Clamp(rect.y, 0f, texture.height - 1f);
		rect.width = Mathf.Min(rect.width, texture.width - rect.x);
		rect.height = Mathf.Min(rect.height, texture.height - rect.y);
		return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
	}
}
