using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Translated from Il2CppOutput Assembly-CSharp.cpp / Assembly-CSharp__1.cpp:
//   ShadowRunPrototype_MakeHomeMenu_mCBAE7252300EFC6C322F1F5077E75392B567859F
//   ShadowRunPrototype_ConfigureHomeCardLayout_mEF9D881E910253D156E08E62FAEBCB4E4BE79B60
//   ShadowRunPrototype_MakeGemCounterWidget_m4CB66A1F1767E52818477565586E128A8A281E0E
//   ShadowRunPrototype_PositionGemCounterSafeArea_mAB6FE2573855F3E029C01E03D02B6B307989828F
//   ShadowRunPrototype_MakeGemShopOverlay_m63B3273BC1D9FF34DE4565878242184A16A866E3
//   ShadowRunPrototype_MakeGemPackCard_m384D467652327518B5EE1C8BAFF3C421AA482F03 (+DisplayClass367_0 b__0/b__1)
//   ShadowRunPrototype_MakeShopPurchaseButton_mAE4C4A2D88BD1975DFBE7D289AEED4BA33D342AB
//   ShadowRunPrototype_ShowGemShop_m68D001934ECE790464F0FD6531C9E1F5DA53B550
//   ShadowRunPrototype_HideGemShop_m1199252BAE8D6A3B8E59200EB831B9F76AB14354
//   U3CAddGemsFromShopU3Ed__371_MoveNext_m0F1E943FCF0DA3A7A58CB78D4DC3797468FD1937
//   ShadowRunPrototype_MakeHomeHeroCard_m0DF5A0B3AC2A73E57F3B288D6F42B7CF4D1121FE
//   ShadowRunPrototype_MakeHomeDriveShine_mFA6766F89E43D31BAFE6C176E092395638BEF0DA
//   ShadowRunPrototype_CreateDriveGlossSprite_m4BEADFC702BD0F2BBDBCC2C0C0A1079F75409E68
//   ShadowRunPrototype_MakeHome101Artwork_mCCC58691FDC6B374334A63B187DBB02A12F3935F
//   ShadowRunPrototype_MakeWeatherPill_m97A08398DDC5D3EAD2E441B6FBB88155C2F2378E
//   ShadowRunPrototype_MakeHomeCaseCard_mDEAA8DF59A29760041822A2D80C0BA890A005390 (+b__377_0/b__377_1)
//   ShadowRunPrototype_MakeHomeGoalsCard_m6DECDBCD7B8D1F6D095155395A5345B9ECA16065
//   ShadowRunPrototype_PickDailyGoals_m3E256CFD8B04864E033B542002666510ECC2ABA9
//   ShadowRunPrototype_CreateDailyGoalPool_mE2D1B3FAF72CBBF7885794394818EF47B8499D42
//   ShadowRunPrototype_MakeDailyGoal_m9D6403DA61E7388EB55CA64F5BA9111FB972308C
//   ShadowRunPrototype_MakeGoalRow_m9E9F77581176B1B6F4F4D99CF08E1647D1BE7FBF (+DisplayClass436_0 b__0)
//   ShadowRunPrototype_SetGoalRowTextRect_mC62C8AFB01A871E187608AD67334232FC291A1BA
//   ShadowRunPrototype_SetGoalTextAnchor_mC0083846F866E9E20BE888359363A332F09702B9
//   ShadowRunPrototype_UpdateDailyGoalsTimer_mBF4036F9C82BDEE93AC5C983AA668DB7C6494DD6
//   ShadowRunPrototype_LayoutHomeMenu_m07BA277A668DB50F4A3B6FA0C90C855790F512FE
//   ShadowRunPrototype_LayoutHomeHeroArt_m583F434B1D34A6FB86437988B34CE1396659A428
public sealed partial class ShadowRunPrototype
{
	private void MakeHomeMenu(Transform parent)
	{
		InfernoLobby.Build(this, parent); /* [InfernoLobby patch] */
	}

	private void ConfigureHomeCardLayout(RectTransform card, float preferredWidth)
	{
		if (card == null)
		{
			return;
		}
		LayoutElement element = card.gameObject.GetComponent<LayoutElement>();
		if (element == null)
		{
			element = card.gameObject.AddComponent<LayoutElement>();
		}
		element.minWidth = 0f;
		element.preferredWidth = preferredWidth;
		element.flexibleWidth = 0f;
		element.minHeight = 620f;
		element.preferredHeight = 620f;
		element.flexibleHeight = 0f;
		element.layoutPriority = 10;
		card.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 620f);
	}

	private void MakeGemCounterWidget(Transform parent)
	{
		InfernoLobby.Gems(this, parent); /* [InfernoLobby patch] */
	}

	private void PositionGemCounterSafeArea()
	{
		if (gemCounterRect == null || Screen.width <= 0 || Screen.height <= 0)
		{
			return;
		}
		float width = menuSafeRoot != null && menuSafeRoot.rect.width > 1f ? menuSafeRoot.rect.width : 1080f;
		float height = menuSafeRoot != null && menuSafeRoot.rect.height > 1f ? menuSafeRoot.rect.height : 1920f;
		Rect safe = Screen.safeArea;
		float rightInset = Mathf.Max(0f, Screen.width - safe.xMax) / Screen.width * width;
		float topInset = Mathf.Max(0f, Screen.height - safe.yMax) / Screen.height * height;
		gemCounterRect.anchoredPosition = new Vector2(-rightInset - 22f, -topInset - 18f);
	}

	private void MakeGemShopOverlay(Transform parent)
	{
		InfernoLobby.BeginShopArtScope(); /* [InfernoLobby patch] ShopArtScope keepShopArt (RAII) */
		try
		{
			gemShopRoot = new GameObject("Gem Shop Overlay");
			gemShopRoot.transform.SetParent(parent, false);
			RectTransform rect = gemShopRoot.AddComponent<RectTransform>();
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
			Image dim = gemShopRoot.AddComponent<Image>();
			dim.color = new Color(0f, 0f, 0.008f, 0.84f);
			Button dimButton = gemShopRoot.AddComponent<Button>();
			dimButton.targetGraphic = dim;
			dimButton.onClick.AddListener(HideGemShop);
			Image artwork = MakeResourceImage(gemShopRoot.transform, "Diamond Shop Artwork", "плашка донат", new Vector2(0f, -520f), new Vector2(1080f, 593f), TextAnchor.UpperCenter, true);
			if (artwork == null)
			{
				gemShopRoot.SetActive(false);
				return;
			}
			artwork.raycastTarget = true;
			Button artworkButton = artwork.gameObject.AddComponent<Button>();
			artworkButton.targetGraphic = artwork;
			artworkButton.transition = Selectable.Transition.None;
			MakeGemPackCard(artwork.transform, "алмаз 1", 30, "USD $0.99", new Vector2(-320f, -242f), false);
			MakeGemPackCard(artwork.transform, "алмаз 3", 70, "USD $1.99", new Vector2(0f, -242f), false);
			MakeGemPackCard(artwork.transform, "сундук", 100, "USD $2.49", new Vector2(320f, -242f), true);
			Button close = new GameObject("Gem Shop Close Button").AddComponent<Button>();
			close.transform.SetParent(artwork.transform, false);
			Image closeImage = close.gameObject.AddComponent<Image>();
			closeImage.color = Color.clear;
			close.targetGraphic = closeImage;
			RectTransform closeRect = close.GetComponent<RectTransform>();
			ApplyAnchor(closeRect, TextAnchor.UpperRight);
			closeRect.anchoredPosition = new Vector2(-42f, -54f);
			closeRect.sizeDelta = new Vector2(88f, 88f);
			close.onClick.AddListener(HideGemShop);
			gemShopRoot.SetActive(false);
		}
		finally
		{
			InfernoLobby.EndShopArtScope(); /* [InfernoLobby patch] */
		}
	}

	private void MakeGemPackCard(Transform parent, string iconResource, int amount, string price, Vector2 pos, bool sale)
	{
		RectTransform card = new GameObject("Gem Pack " + amount.ToString()).AddComponent<RectTransform>();
		card.SetParent(parent, false);
		ApplyAnchor(card, TextAnchor.UpperCenter);
		card.anchoredPosition = pos;
		card.sizeDelta = new Vector2(250f, 250f);
		Image glow = MakeUiBlock(card, "Gem Pack Neon Glow", new Vector2(0f, 22f), new Vector2(246f, 180f), TextAnchor.MiddleCenter, new Color(1f, 0.02f, 0.72f, 0.28f));
		glow.sprite = roundedGlowSprite;
		glow.type = Image.Type.Sliced;
		glow.raycastTarget = false;
		Image frame = MakeThinGlassPanel(card, "Gem Pack Frame", new Vector2(0f, 22f), new Vector2(236f, 170f), TextAnchor.MiddleCenter, new Color(0f, 0f, 0f, 0.96f), new Color(1f, 0.02f, 0.72f, 1f), 3f);
		frame.raycastTarget = true;
		Button frameButton = frame.gameObject.AddComponent<Button>();
		frameButton.targetGraphic = frame;
		frameButton.onClick.AddListener(() => AddGemsFromShop(amount));
		Vector2 artSize = iconResource == "алмаз 1" ? new Vector2(77f, 56f) * 1.3f : new Vector2(154f, 112f);
		Vector2 artPos = iconResource == "алмаз 1" ? new Vector2(0f, 35f) : new Vector2(0f, 38f);
		Image art = MakeReferenceArtImage(card, "Pack Reward Art", iconResource, artPos, artSize, TextAnchor.MiddleCenter);
		if (art != null)
		{
			art.preserveAspect = true;
			art.raycastTarget = false;
			/* [user request 2026-09-20] the USD 0.99 pack shows the same faceted crystal as the
			   top-right counter (InfernoLobby.GemSprite); the 1.99 / 2.49 packs keep their own art. */
			if (iconResource == "алмаз 1")
			{
				Sprite counterGem = InfernoLobby.GemSprite(this);
				if (counterGem != null)
				{
					art.sprite = counterGem;
					art.rectTransform.sizeDelta = new Vector2(96f, 96f);
					art.rectTransform.anchoredPosition = new Vector2(0f, 36f);
				}
			}
		}
		if (sale)
		{
			Text best = MakeText(card, "BEST", 15, TextAnchor.MiddleCenter, new Vector2(76f, 94f));
			best.fontStyle = FontStyle.Bold;
			best.color = new Color(1f, 0.62f, 1f, 1f);
			best.raycastTarget = false;
			best.GetComponent<RectTransform>().sizeDelta = new Vector2(72f, 26f);
		}
		Text amountText = MakeText(card, amount.ToString(), 27, TextAnchor.MiddleCenter, new Vector2(0f, -44f));
		amountText.fontStyle = FontStyle.Bold;
		amountText.color = Color.white;
		amountText.raycastTarget = false;
		amountText.GetComponent<RectTransform>().sizeDelta = new Vector2(180f, 42f);
		Button purchase = MakeShopPurchaseButton(card, price, new Vector2(0f, -100f));
		purchase.onClick.AddListener(() => AddGemsFromShop(amount));
		Text priceText = purchase.GetComponentInChildren<Text>();
		if (priceText != null)
		{
			gemShopPriceTexts[amount] = priceText;
		}
	}

	private Button MakeShopPurchaseButton(Transform parent, string price, Vector2 pos)
	{
		Button button = new GameObject(price + " Button").AddComponent<Button>();
		button.transform.SetParent(parent, false);
		Image background = button.gameObject.AddComponent<Image>();
		background.sprite = roundedGlassSprite;
		background.type = Image.Type.Sliced;
		background.color = new Color(0.07f, 0.025f, 0.13f, 0.92f);
		button.targetGraphic = background;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.MiddleCenter);
		rect.anchoredPosition = pos;
		rect.sizeDelta = new Vector2(222f, 50f);
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.12f, 0.92f, 1.18f, 1f);
		colors.pressedColor = new Color(0.72f, 0.45f, 0.88f, 1f);
		button.colors = colors;
		Text label = MakeText(button.transform, price, 28, TextAnchor.MiddleCenter, Vector2.zero);
		label.fontStyle = FontStyle.Bold;
		label.color = new Color(0.96f, 0.84f, 1f, 1f);
		label.raycastTarget = false;
		label.resizeTextForBestFit = true;
		label.resizeTextMinSize = 23;
		label.resizeTextMaxSize = 28;
		label.GetComponent<RectTransform>().sizeDelta = new Vector2(210f, 46f);
		return button;
	}

	private void ShowGemShop()
	{
		if (gemShopRoot == null)
		{
			return;
		}
		foreach (KeyValuePair<int, Text> pair in gemShopPriceTexts)
		{
			if (pair.Value != null)
			{
				pair.Value.text = RevenueCatPurchaseService.GetLocalizedPrice(pair.Key, pair.Value.text);
			}
		}
		gemShopRoot.SetActive(true);
		gemShopRoot.transform.SetAsLastSibling();
	}

	private void HideGemShop()
	{
		if (gemShopRoot != null)
		{
			gemShopRoot.SetActive(false);
		}
	}

	private async void AddGemsFromShop(int amount)
	{
		if (gemPurchaseBusy)
		{
			return;
		}
		gemPurchaseBusy = true;
		if (!await SupabaseGameManager.RequireOnlineEconomyAsync())
		{
			gemPurchaseBusy = false;
			return;
		}
		int balanceBeforePurchase = SupabaseGameManager.Crystals;
		try
		{
			RevenueCatPurchaseService.PurchaseOutcome result = await RevenueCatPurchaseService.PurchaseCrystalsAsync(amount);
			if (!result.Success)
			{
				if (!result.Cancelled)
				{
					Debug.LogError("[RevenueCat] " + result.Message);
				}
				return;
			}
			int expectedBalance = balanceBeforePurchase + amount;
			for (int attempt = 0; attempt < 30; attempt++)
			{
				await Task.Delay(2000);
				await SupabaseGameManager.LoadPlayerFromCloud();
				if (SupabaseGameManager.Crystals >= expectedBalance)
				{
					break;
				}
			}
			menuGemCount = SupabaseGameManager.Crystals;
			if (gemAmountText != null)
			{
				gemAmountText.text = menuGemCount.ToString();
			}
			if (menuGemCount < expectedBalance)
			{
				Debug.LogWarning("[RevenueCat] Purchase " + result.TransactionId + " is verified; the webhook reward is still processing.");
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[RevenueCat] Purchase flow failed: " + ex.Message);
		}
		finally
		{
			gemPurchaseBusy = false;
		}
	}

	private void MakeHomeHeroCard(Transform parent)
	{
		RectTransform root = new GameObject("Home Hero Card").AddComponent<RectTransform>();
		homeHeroRoot = root;
		root.SetParent(parent);
		ApplyAnchor(root, TextAnchor.UpperCenter);
		root.anchoredPosition = new Vector2(0f, -272f);
		root.sizeDelta = new Vector2(920f, 1000f);
		LayoutElement element = root.gameObject.AddComponent<LayoutElement>();
		element.preferredHeight = 650f;
		element.minHeight = 560f;
		element.flexibleWidth = 1f;
		Image driveArt = MakeReferenceArtImage(root, "DRIVE Art", "drive menu final", new Vector2(0f, 270f), new Vector2(1330f, 886.669983f), TextAnchor.LowerCenter);
		homeDriveArtRect = driveArt.rectTransform;
		MakeHomeDriveShine(driveArt);
		RectTransform hit = new GameObject("DRIVE").AddComponent<RectTransform>();
		hit.SetParent(root, false);
		ApplyAnchor(hit, TextAnchor.LowerCenter);
		hit.anchoredPosition = new Vector2(0f, 266f);
		hit.sizeDelta = new Vector2(1016f, 188f);
		homeDriveHitRect = hit;
		Image hitImage = hit.gameObject.AddComponent<Image>();
		hitImage.color = Color.clear;
		hitImage.raycastTarget = true;
		primaryOverlayButton = hit.gameObject.AddComponent<Button>();
		primaryOverlayButton.targetGraphic = hitImage;
		Text label = MakeText(primaryOverlayButton.transform, "", 1, TextAnchor.MiddleCenter, Vector2.zero);
		label.name = "Button Label";
		label.color = Color.clear;
		label.raycastTarget = false;
		label.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		primaryOverlayButton.onClick.AddListener(StartRun);
		primaryButtonLabel = primaryOverlayButton.transform.Find("Button Label").GetComponent<Text>();
	}

	private void MakeHomeDriveShine(Image driveArt)
	{
		if (driveArt == null || driveArt.sprite == null)
		{
			return;
		}
		Image mask = new GameObject("DRIVE Gloss Mask").AddComponent<Image>();
		mask.transform.SetParent(driveArt.transform, false);
		mask.sprite = roundedGlassSprite;
		mask.type = Image.Type.Sliced;
		mask.preserveAspect = false;
		mask.color = Color.white;
		mask.raycastTarget = false;
		RectTransform maskRect = mask.rectTransform;
		maskRect.anchorMin = Vector2.zero;
		maskRect.anchorMax = Vector2.one;
		maskRect.offsetMin = Vector2.zero;
		maskRect.offsetMax = new Vector2(0f, -8f);
		mask.gameObject.AddComponent<Mask>().showMaskGraphic = false;
		homeDriveShineImage = new GameObject("DRIVE Gloss Shine").AddComponent<Image>();
		homeDriveShineImage.transform.SetParent(maskRect, false);
		homeDriveShineImage.sprite = CreateDriveGlossSprite();
		homeDriveShineImage.color = new Color(1f, 0.98f, 0.9f, 0f);
		homeDriveShineImage.raycastTarget = false;
		homeDriveShineRect = homeDriveShineImage.rectTransform;
		Vector2 center = new Vector2(0.5f, 0.5f);
		homeDriveShineRect.anchorMax = center;
		homeDriveShineRect.anchorMin = center;
		homeDriveShineRect.pivot = new Vector2(0.5f, 0.5f);
		homeDriveShineRect.sizeDelta = new Vector2(220f, 700f);   /* [user request 2026-09-21 restyle] wider sweep; was 170 x 620 */
		homeDriveShineRect.localRotation = Quaternion.Euler(0f, 0f, -20f);
	}

	private Sprite CreateDriveGlossSprite()
	{
		Texture2D texture = new Texture2D(64, 4, TextureFormat.RGBA32, false, true);
		texture.name = "Runtime DRIVE Gloss Gradient";
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		Color32[] pixels = new Color32[256];
		for (int x = 0; x < 64; x++)
		{
			float t = x / 63f;
			float ridge = 1f - Mathf.Abs(t * 2f - 1f);
			byte alpha = (byte)Mathf.RoundToInt(Mathf.SmoothStep(0f, 1f, ridge) * 255f);
			Color32 color = new Color32(255, 255, 255, alpha);
			for (int y = 0; y < 4; y++)
			{
				pixels[y * 64 + x] = color;
			}
		}
		texture.SetPixels32(pixels);
		texture.Apply(false, true);
		return Sprite.Create(texture, new Rect(0f, 0f, 64f, 4f), new Vector2(0.5f, 0.5f), 100f);
	}

	private void MakeHome101Artwork(RectTransform layoutRow)
	{
		layoutRow.name = "101 Menu Artwork Layout Row";
		Image art = MakeReferenceArtImage(layoutRow, "101 Menu Artwork Visual", "101", new Vector2(0f, -39f), Vector2.zero, TextAnchor.LowerCenter);
		art.raycastTarget = false;
		if (art.sprite != null && art.sprite.texture != null)
		{
			art.sprite.texture.filterMode = FilterMode.Bilinear;
			art.sprite.texture.wrapMode = TextureWrapMode.Clamp;
			art.sprite.texture.anisoLevel = 1;
		}
		homeHeroImageRect = layoutRow;
		homeHeroArtRect = art.rectTransform;
		if (art.sprite != null)
		{
			homeHeroArtRect.sizeDelta = art.sprite.rect.size;
		}
	}

	private void MakeWeatherPill(Transform parent)
	{
		Image pill = MakeThinGlassPanel(parent, "Weather Pill", new Vector2(-20f, -20f), new Vector2(250f, 62f), TextAnchor.UpperRight, new Color(0.051f, 0.043f, 0.102f, 0.78f), new Color(0.545f, 0.361f, 0.965f, 0.18f));
		Text text = MakeText(pill.transform, "☂  RAIN 17°   23°C", 19, TextAnchor.MiddleCenter, Vector2.zero);
		text.fontStyle = FontStyle.Bold;
		text.color = new Color(0.886f, 0.851f, 0.953f, 1f);
		text.GetComponent<RectTransform>().sizeDelta = new Vector2(232f, 54f);
	}

	private void MakeHomeCaseCard(Transform parent)
	{
		Button card = MakeReferenceArtButton(parent, "Case Card", "case menu final", new Vector2(-44f, -1017f), new Vector2(650f, 975f), TextAnchor.UpperLeft);
		homeCaseArtRect = card.GetComponent<RectTransform>();
		Image image = card.GetComponent<Image>();
		if (image != null)
		{
			image.preserveAspect = false;
		}
		EventTrigger trigger = card.gameObject.GetComponent<EventTrigger>();
		if (trigger == null)
		{
			trigger = card.gameObject.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
		down.callback.AddListener(_ => { caseHomeCardRequiresFreshPointerDown = false; });
		trigger.triggers.Add(down);
		card.onClick.AddListener(() =>
		{
			if (caseHomeCardRequiresFreshPointerDown)
			{
				return;
			}
			ShowCaseOverlay();
		});
	}

	private void MakeHomeGoalsCard(Transform parent)
	{
		Image card = MakeReferenceArtImage(parent, "Daily Goals Card", "goals menu final", new Vector2(-44f, -978f), new Vector2(650f, 975f), TextAnchor.UpperRight);
		homeGoalsArtRect = card.rectTransform;
		card.preserveAspect = false;
		RectTransform textGroup = new GameObject("Daily Goals Dynamic Text Group").AddComponent<RectTransform>();
		textGroup.SetParent(card.transform, false);
		textGroup.anchorMin = Vector2.zero;
		textGroup.anchorMax = Vector2.one;
		textGroup.pivot = new Vector2(0.5f, 0.5f);
		textGroup.offsetMin = Vector2.zero;
		textGroup.offsetMax = Vector2.zero;
		RectTransform timerGroup = new GameObject("Daily Goals Refresh Timer Group").AddComponent<RectTransform>();
		timerGroup.SetParent(textGroup, false);
		timerGroup.anchorMin = new Vector2(0f, 0.79f);
		timerGroup.anchorMax = new Vector2(1f, 0.844f);
		timerGroup.pivot = new Vector2(0.5f, 0.5f);
		timerGroup.offsetMin = Vector2.zero;
		timerGroup.offsetMax = Vector2.zero;
		dailyGoalsTimerText = MakeText(timerGroup, "00:00:00", 21, TextAnchor.MiddleLeft, Vector2.zero);
		dailyGoalsTimerText.fontStyle = FontStyle.Normal;
		dailyGoalsTimerText.color = new Color(0.82f, 0.6f, 0.96f, 1f);
		dailyGoalsTimerText.raycastTarget = false;
		SetGoalTextAnchor(dailyGoalsTimerText, new Vector2(0.415f, 0.5f), new Vector2(-3f, 1.5f), new Vector2(180f, 36f));
		HomeGoalData[] goals = PickDailyGoals(3);
		for (int i = 0; i < goals.Length; i++)
		{
			MakeGoalRow(textGroup, i, goals[i]);
		}
		dailyGoalsDateKey = CurrentDailyGoalsDateKey();
		UpdateDailyGoalsTimer(true);
	}

	private HomeGoalData[] PickDailyGoals(int count)
	{
		HomeGoalData[] pool = CreateDailyGoalPool();
		count = Mathf.Clamp(count, 0, pool.Length);
		HomeGoalData[] picked = new HomeGoalData[count];
		bool[] used = new bool[pool.Length];
		System.Random random = new System.Random(DateTime.Now.Year * 10000 + DateTime.Now.Month * 100 + DateTime.Now.Day);
		for (int i = 0; i < count;)
		{
			int index = random.Next(pool.Length);
			if (used[index])
			{
				continue;
			}
			used[index] = true;
			picked[i] = pool[index];
			i++;
		}
		return picked;
	}

	private HomeGoalData[] CreateDailyGoalPool()
	{
		return new HomeGoalData[6]
		{
			MakeDailyGoal(DailyGoalKind.Distance, "Night Marathon", "exoplanet-3-256", new Color(0.7f, 0.55f, 1f, 1f)),
			MakeDailyGoal(DailyGoalKind.Runs, "Road Regular", "rocket-256", new Color(1f, 0.46f, 0.33f, 1f)),
			MakeDailyGoal(DailyGoalKind.Boosts, "Boost Hunter", "black-hole-256", new Color(0.82f, 0.52f, 1f, 1f)),
			MakeDailyGoal(DailyGoalKind.Drift, "Drift Control", "neptune-256", new Color(0.42f, 0.75f, 1f, 1f)),
			MakeDailyGoal(DailyGoalKind.CarsPassed, "Traffic Phantom", "ufo-1-256", new Color(0.42f, 0.9f, 1f, 1f)),
			MakeDailyGoal(DailyGoalKind.CasesOpened, "Case Opener", "chest-2-256", new Color(1f, 0.68f, 0.43f, 1f))
		};
	}

	private HomeGoalData MakeDailyGoal(DailyGoalKind kind, string title, string iconName, Color accent)
	{
		return new HomeGoalData
		{
			Kind = kind,
			Title = title,
			Progress = FormatDailyGoalProgress(kind),
			Fill = DailyGoalFill(kind),
			IconResource = "Space_Exploration_GUI_Kit/Icons/" + iconName,
			Accent = accent
		};
	}

	private void MakeGoalRow(Transform parent, int index, HomeGoalData data)
	{
		RectTransform row = new GameObject(string.Format("Daily Goal Row {0} Text Group", index + 1)).AddComponent<RectTransform>();
		row.SetParent(parent, false);
		float top = 0.744f - index * 0.19f;
		row.anchorMin = new Vector2(0.275f, top - 0.164f);
		row.anchorMax = new Vector2(0.94f, top);
		row.pivot = new Vector2(0.5f, 0.5f);
		row.offsetMin = Vector2.zero;
		row.offsetMax = Vector2.zero;
		Text title = MakeText(row, data.Title, 20, TextAnchor.MiddleLeft, Vector2.zero);
		title.color = new Color(0.886f, 0.851f, 0.953f, 1f);
		title.raycastTarget = false;
		SetGoalRowTextRect(title, -27f, 30f);
		Text progress = MakeText(row, data.Progress, 17, TextAnchor.MiddleLeft, Vector2.zero);
		progress.color = new Color(0.66f, 0.5f, 0.86f, 1f);
		progress.raycastTarget = false;
		SetGoalRowTextRect(progress, -56f, 26f);
		Button collectButton = MakeReferenceArtButton(row, string.Format("Daily Goal {0} Collect Button", index + 1), "коллект", Vector2.zero, new Vector2(114f, 31f), TextAnchor.MiddleCenter);
		Image collectImage = collectButton.image;
		RectTransform collectRect = collectButton.GetComponent<RectTransform>();
		collectRect.anchorMin = new Vector2(1f, 1f);
		collectRect.anchorMax = new Vector2(1f, 1f);
		collectRect.pivot = new Vector2(1f, 0.5f);
		collectRect.anchoredPosition = new Vector2(-34f, -43f);
		collectRect.localScale = Vector3.one * 1.1f;
		collectButton.onClick.AddListener(() => ClaimVisibleDailyGoal(index, collectButton.gameObject));
		collectButton.gameObject.SetActive(IsDailyGoalComplete(data.Kind) && !IsDailyGoalClaimed(data.Kind));
		Image claimed = MakeReferenceArtImage(row, string.Format("Daily Goal {0} Claimed Icon", index + 1), "галочка иконка", Vector2.zero, new Vector2(42f, 42f), TextAnchor.MiddleCenter);
		RectTransform claimedRect = claimed.rectTransform;
		claimedRect.anchorMin = new Vector2(1f, 1f);
		claimedRect.anchorMax = new Vector2(1f, 1f);
		claimedRect.pivot = new Vector2(0.5f, 0.5f);
		claimedRect.anchoredPosition = new Vector2(-97f, -43f);
		claimed.preserveAspect = true;
		claimed.raycastTarget = false;
		claimed.gameObject.SetActive(IsDailyGoalClaimed(data.Kind));
		if (index >= 0 && index < dailyGoalTitleTexts.Length)
		{
			dailyGoalTitleTexts[index] = title;
			dailyGoalProgressTexts[index] = progress;
			dailyGoalCollectImages[index] = collectImage;
			dailyGoalClaimedImages[index] = claimed;
		}
	}

	private void SetGoalRowTextRect(Text text, float centerOffset, float height)
	{
		if (text == null)
		{
			return;
		}
		RectTransform rect = text.rectTransform;
		rect.anchorMin = new Vector2(0f, 1f);
		rect.anchorMax = new Vector2(1f, 1f);
		rect.pivot = new Vector2(0f, 0.5f);
		rect.anchoredPosition = new Vector2(0f, centerOffset);
		rect.sizeDelta = new Vector2(0f, height);
	}

	private void SetGoalTextAnchor(Text text, Vector2 anchor, Vector2 anchoredPosition, Vector2 size)
	{
		if (text == null)
		{
			return;
		}
		RectTransform rect = text.rectTransform;
		rect.anchorMin = anchor;
		rect.anchorMax = anchor;
		rect.pivot = new Vector2(0f, 0.5f);
		rect.anchoredPosition = anchoredPosition;
		rect.sizeDelta = size;
	}

	private void UpdateDailyGoalsTimer(bool force = false)
	{
		if (dailyGoalsTimerText == null)
		{
			return;
		}
		if (!force && Time.unscaledTime < nextDailyGoalsTimerUpdate)
		{
			return;
		}
		nextDailyGoalsTimerUpdate = Time.unscaledTime + 1f;
		/* [user request 2026-09-21] */ InfernoDailyCases.RefreshCard();
		/* [user request 2026-09-21] */ if (caseOverlayRoot != null && caseOverlayRoot.activeInHierarchy) InfernoDailyCases.RefreshOverlay(this);
		DateTime now = DateTime.Now;
		int dateKey = now.Year * 10000 + now.Month * 100 + now.Day;
		EnsureDailyGoalProgressLoaded();
		if (dateKey != dailyGoalsDateKey)
		{
			dailyGoalsDateKey = dateKey;
		}
		HomeGoalData[] goals = PickDailyGoals(3);
		InfernoLobby.RefreshGoalRewards(this); /* [InfernoLobby patch] */
		for (int i = 0; i < goals.Length; i++)
		{
			if (dailyGoalTitleTexts[i] != null)
			{
				dailyGoalTitleTexts[i].text = goals[i].Title;
			}
			if (dailyGoalProgressTexts[i] != null)
			{
				dailyGoalProgressTexts[i].text = goals[i].Progress;
			}
			if (dailyGoalCollectImages[i] != null)
			{
				dailyGoalCollectImages[i].gameObject.SetActive(IsDailyGoalComplete(goals[i].Kind) && !IsDailyGoalClaimed(goals[i].Kind));
			}
			if (dailyGoalClaimedImages[i] != null)
			{
				dailyGoalClaimedImages[i].gameObject.SetActive(IsDailyGoalClaimed(goals[i].Kind));
			}
		}
		TimeSpan remaining = now.Date.AddDays(1.0) - now;
		int hours = Mathf.Max(0, (int)remaining.TotalHours);
		dailyGoalsTimerText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, remaining.Minutes, remaining.Seconds);
	}

	private void LayoutHomeMenu()
	{
		InfernoLobby.Layout(this); /* [InfernoLobby patch] */
	}

	private void LayoutHomeHeroArt()
	{
		if (homeHeroArtRect == null || homeHeroImageRect == null || homeDriveArtRect == null)
		{
			return;
		}
		Image heroImage = homeHeroArtRect.GetComponent<Image>();
		Image driveImage = homeDriveArtRect.GetComponent<Image>();
		if (heroImage == null || heroImage.sprite == null || driveImage == null || driveImage.sprite == null)
		{
			return;
		}
		float driveAspect = driveImage.sprite.rect.width / Mathf.Max(1f, driveImage.sprite.rect.height);
		float driveVisibleWidth = Mathf.Min(homeDriveArtRect.rect.width, homeDriveArtRect.rect.height * driveAspect);
		Vector3 driveCenter = homeDriveArtRect.rect.center;
		Vector3 leftWorld = homeDriveArtRect.TransformPoint(driveCenter + Vector3.left * (driveVisibleWidth * 0.5f));
		Vector3 rightWorld = homeDriveArtRect.TransformPoint(driveCenter + Vector3.right * (driveVisibleWidth * 0.5f));
		Vector3 leftLocal = homeHeroImageRect.InverseTransformPoint(leftWorld);
		Vector3 rightLocal = homeHeroImageRect.InverseTransformPoint(rightWorld);
		float scale = Mathf.Max(1f, Mathf.Abs(rightLocal.x - leftLocal.x)) / Mathf.Max(1f, heroImage.sprite.rect.width);
		homeHeroArtRect.sizeDelta = heroImage.sprite.rect.size;
		homeHeroArtRect.localScale = Vector3.one * scale;
		homeHeroArtRect.anchoredPosition = new Vector2(0f, -39f);
		float rowHeight = 578f * scale;
		LayoutElement element = homeHeroImageRect.GetComponent<LayoutElement>();
		if (element == null)
		{
			element = homeHeroImageRect.gameObject.AddComponent<LayoutElement>();
		}
		float height = Mathf.Max(1f, rowHeight - 39f);
		element.minHeight = height;
		element.preferredHeight = height;
		element.flexibleHeight = 0f;
	}
}
