using System;
using System.Threading.Tasks;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Translated from Il2CppOutput Assembly-CSharp.cpp / Assembly-CSharp__1.cpp:
//   ShadowRunPrototype_MakeGarageScreen_m37578D36046D5F6D700D6FCAF41E553EC684F1F6
//   ShadowRunPrototype_MakeGaragePurchaseAction_mC9B68EA99EA6CABECE45570021555A9FA0677197 (+b__237_0)
//   U3CPurchaseSelectedGarageVehicleAsyncU3Ed__238_MoveNext_m53605AD679F7BC788D5380082D5107038DF9097E
//   ShadowRunPrototype_MakeGarageVehicleSelector_mD7EBE2455BF7EAEF3C817038249BE5A659B0AE68
//   ShadowRunPrototype_CreateGarageVehicleCards_mDF1B4701D7F6DAB63ED5E5948EA3603C24C61E98
//   ShadowRunPrototype_MakeGarageVehicleCard_m81E36C706217D6B02EA6D869567B4371C7058233
//   ShadowRunPrototype_GetGarageLockSprite_m25B751724252CB28C1347AC5D3B1A807361E7A7B
//   ShadowRunPrototype_MakeGarageRarityBadge_mE2F0B17327545A5FA11A74AAF53916C9BB25B639
//   ShadowRunPrototype_GetGarageVehicleCardFillSprite_mE7526CF7194167B6642829E995783ECAC999BC24
//   ShadowRunPrototype_MakeGarageCardStat_m13804EC13E5FC3CBBD366AB80C85A86BD8C296E7
//   ShadowRunPrototype_SelectGarageVehicleCard_m7F1A8D174DF0F8938E2E619F22A94153F5B35309
//   ShadowRunPrototype_QueueGarageVehicleSelection_m9844C2CDE8C7E88A2E52938654929EAB63363C74
//   ShadowRunPrototype_SelectGarageVehicleCardFromClick_mD294F42368C0BF80258DA57EF65348253F6128D0
//   ShadowRunPrototype_SelectGarageVehicleCardAtPointer_m04237F81F0597D2F3BC372979A355A52C7337276
//   ShadowRunPrototype_AddGarageVehicleCardDragEvents_m58E34365F18FB485074B1BEB3BB7197D3C6CD031
//   U3CU3Ec__DisplayClass250_0_U3CAddGarageVehicleCardDragEventsU3Eb__0..5
//   ShadowRunPrototype_BeginGarageVehicleCardPointer_m0C7DBFDD9E8184329D50F01CE93E4EE939B87B75
//   ShadowRunPrototype_InitializeGarageVehicleCardDrag_mAB49B47583C607118FE7F395B2CD5B52CEAAC891
//   ShadowRunPrototype_BeginGarageVehicleCardDrag_mBA47A0085ACD63CD807695B5B482168F7BB75C03
//   ShadowRunPrototype_DragGarageVehicleCard_mFA3BF5E3F0D2892BE4788F622263C83A591DB3F8
//   ShadowRunPrototype_EndGarageVehicleCardDrag_mAA274BC2BEB1F0A95C2701009F18538779190B1D
//   ShadowRunPrototype_ClampGarageVehicleCarouselX_mE575BAFD8EBCCC66459C84BBD0E35DE1CBB1CD2C
//   ShadowRunPrototype_SnapGarageVehicleScrollTo_m565D2098FC7C25C5AF124E8A85B60816F23237D5
//   ShadowRunPrototype_UpdateGarageSelectedCardFromScroll_m780E7BED70B416FA5AD39856B83B0F108F298DD5
//   ShadowRunPrototype_GetNearestGarageVehicleCardIndex_m08B8E4BFBC0E1E16AF72AD7D93842AFBFF6A42D7
//   ShadowRunPrototype_GetNearestGarageVehicleCardIndex_m4D16116B7044724075022B1B6149DAE941CA295A
//   ShadowRunPrototype_GetGarageVehicleViewportWidth_m623BAE742A90DF1BA2B6E1FDEE7081FE6F4AC6DC
//   ShadowRunPrototype_UpdateGarageVehicleScrollBounds_mC85B8A543CBD7E88B95609942A5AA99666676035
//   ShadowRunPrototype_UpdateGarageVehicleCarousel_m80A7B2B0BA97EC1ADDDDC8BDF6EA1057EE54CDA9
//   ShadowRunPrototype_GetGarageVehicleCardIndexForVehicle_m3ADEFA99D35AB4A3B0CA034DFD521C59127E39B5
//   ShadowRunPrototype_MakeGarageVehicleButton_m7C289AB84931B9FF75C3091FBA2DE76572571C37
//   ShadowRunPrototype_MakeGarageBackground_m891825A753CFA0242ACD70DB6016B881AEA2C1BE
//   ShadowRunPrototype_MakeGarageAtmosphere_m9411BCA140C514E487FF4E109159B45BDC21D1CE
//   ShadowRunPrototype_MakeGarageCar_m4FBF29E9D8EED30A032E89D1DF61AF47A68B5B0C
//   ShadowRunPrototype_AddGaragePreviewDragEvents_mE22BD399A27BD277D9BA78AC16C5E87796A7EF82 (+b__269_0..3)
//   ShadowRunPrototype_BeginGaragePreviewDrag_m9758550EB42E05CC93FD73E8FC6E88F33DB53E5E
//   ShadowRunPrototype_DragGaragePreview_mF4596AF02CC1B8D6FC152DB9C87D6ADD70505A87
//   ShadowRunPrototype_EndGaragePreviewDrag_m9DB01EDE1497F213D3A50043C719EEA92EC2B8CC
//   ShadowRunPrototype_MakeGaragePreviewWorld_m5408F18A8EB977635CFAC93F26DB37AEA9CBE497
//   ShadowRunPrototype_BuildImportedGarageScene_mC4F2E89807AAFEE4B346F79922908A192288D75F
//   ShadowRunPrototype_NormalizeGarageScene_mC2BE216F0B1087E462D82D603075A4265C5DC0BC
//   ShadowRunPrototype_StyleImportedGarageScene_m66332FBD32F31F5C25C6E8C0576ACAB3A4D12B45
//   ShadowRunPrototype_FitGarageVehicle_mD961F838E9D9869D213CBDA17069E8D659FA4E74
//   ShadowRunPrototype_StyleVehicleForGarage_mE4C2B6960DC4519647470EA4DE3211E0AE223BE1
//   ShadowRunPrototype_SetGaragePreviewVisible_mE560796A10B7D5F7038DFD472171974C5F37BB3E
//   ShadowRunPrototype_UpdateGaragePreview_m6FD302B6CADF065FC659B993F22FE63F1DBE904D
//   ShadowRunPrototype_ApplyGaragePreviewRotation_mE3009E21D709E0325371B16BAAE287C247CC19CC
//   ShadowRunPrototype_ShowGarageScreen_m8DD30499870E458B54137562CE7B9285F79E0CAD
//   ShadowRunPrototype_HideGarageScreen_m6426442EB34A2D51D4278FF2CD5C8C6F7CD0A944
//   ShadowRunPrototype_UpdateGarageUi_m631BE55BBD33955775560B41F6FF537EAB7F21D9
//   ShadowRunPrototype_UpdateGarageVehicleCardVisuals_mB7AA3673531DE92122A81152963CD89D6A64FC65
//   ShadowRunPrototype_SetGarageVehicleButtonVisual_m131EE6D7E384FC21E4E91F3383240AE57D60F28B
//   ShadowRunPrototype_RebuildGaragePreviewVehicle_mC0D374B718E069AC08A32BF1405F04F822143840
//   ShadowRunPrototype_PreviewGarageVehicle_mE418128C0C62315CD99054600F6E7D51911CCE00
public sealed partial class ShadowRunPrototype
{
	private void MakeGarageScreen(Transform parent)
	{
		garageRoot = new GameObject("Garage Screen");
		garageRoot.transform.SetParent(parent);
		RectTransform rect = garageRoot.AddComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		garageRoot.AddComponent<Image>().color = Color.black;
		MakeGarageBackground();
		MakeGarageCar();
		garagePageHeaderRoot = MakeMenuPageHeader(garageRoot.transform, "MY GARAGE", "0 OF 0 CARS UNLOCKED", out garageUnlockedCountText);
		MakeGarageVehicleSelector();
		MakeGaragePurchaseAction();
		// MakeGarageTuningCard(); /* [user request 2026-09-21: cars] */ /* [user request 2026-09-22: tuning card removed from the garage] */
		garageRoot.SetActive(false);
		SetGaragePreviewVisible(false);
		UpdateGarageUi();
		InfernoStyle.Build(this); /* [InfernoLobby patch] */
	}

	/* [user request 2026-09-21: cars] */
	// Garage bottom layout, y measured from the page bottom (every rect 16+ px from its neighbours):
	//   dock (InfernoLobby.Tabs, 1000x184 centred at 112)      20..204
	//   tuning card 960x290 centred at 367                      222..512   (18 px above the dock)
	//   action row 100 tall centred at 580                      530..630   (18 px above the card)
	//     PURCHASE 560 wide at x 0                              -280..280
	//     STYLE 170 wide at x 385 (InfernoStyle.Build)          300..470   (20 px right of PURCHASE)
	//   card carousel (top-anchored -56, 560 tall)             1304..1864  (674 px above the action row: the 3D preview's band)
	private const float GarageTuningCardWidth = 960f;
	private const float GarageTuningCardHeight = 290f;
	private const float GarageTuningCardCentreY = 367f;
	private const float GarageActionRowBottom = 530f;
	private const float GarageActionRowHeight = 100f;
	private const float GaragePurchaseButtonWidth = 560f;
	private const float GarageTuningRowPitch = 80f;          // rows at +80 / 0 / -80 of the card centre: 64 px buttons leave 16 px between rows
	private const float GarageMiniBarWidth = 106f;           // three columns in the card's 338 px info panel: 3 x 106 + 2 x 10
	private const float GarageMiniBarGap = 10f;
	/* [user request 2026-09-27: card data] */
	// The card's 338x150 info panel (bottom 22 px above the card edge) stacks, from its top: the name (0..-42, best fit
	// 22..34), the subtitle (-42..-70, 14..21), the price row (-74..-106, 26 px) and the three mini-bar labels (-112..-132,
	// 15 px) over their 8 px bars (-134..-142) - the rows end 30 px above the card edge (was 57 with 28/17/18/12 px text
	// and 5 px bars). The locked card's blur strip ends at the price row's top, so it reads these too.
	private const float GarageCardInfoBottom = 22f;
	private const float GarageCardInfoHeight = 150f;
	private const float GarageCardNameHeight = 42f;          // original: 36
	private const int GarageCardNameFontMin = 22;            // original: 18
	private const int GarageCardNameFontMax = 34;            // original: 28
	private const float GarageCardSubtitleHeight = 28f;      // original: 24
	private const int GarageCardSubtitleFontMin = 14;        // original: 12
	private const int GarageCardSubtitleFontMax = 21;        // original: 17
	private const float GarageCardPriceY = -74f;             // original: -64
	private const float GarageCardPriceHeight = 32f;         // original: 22
	private const int GarageCardPriceFont = 26;              // original: 18
	private const float GarageCardStatLabelY = -112f;        // original: -92
	private const float GarageCardStatLabelHeight = 20f;     // original: 16
	private const int GarageCardStatLabelFont = 15;          // original: 12
	private const float GarageMiniBarHeight = 8f;            // original: 5
	private const float GarageMiniBarOffset = 22f;           // bar top below its label's top: label 20 + 2 px gap (original: 18)
	private const float GarageCardPhotoSlotWidth = 336f;     // 392 - 56
	private const float GarageCardPhotoSlotHeight = 142f;
	private RectTransform garageTuningRoot;
	private readonly Text[] garageTuningLabels = new Text[InfernoUpgrades.StatCount];
	private readonly RectTransform[] garageTuningBars = new RectTransform[InfernoUpgrades.StatCount];
	private readonly Text[] garageTuningLevels = new Text[InfernoUpgrades.StatCount];
	private readonly Button[] garageTuningButtons = new Button[InfernoUpgrades.StatCount];
	private Text[] garageVehicleCardPriceTexts;
	private Image[][] garageVehicleCardStatFills;
	private Image[] garageVehicleCardPhotos;

	private void MakeGaragePurchaseAction()
	{
		RectTransform root = new GameObject("Locked Vehicle Purchase Action").AddComponent<RectTransform>();
		garagePurchaseActionRoot = root;
		root.SetParent(garageRoot.transform, false);
		ApplyAnchor(root, TextAnchor.LowerCenter);
		/* [user request 2026-09-21: cars] */
		// original: root at (-40, 375) 650x140 holding the "кнопка в гараж" art button (620x121) with a 28 px label at (-46, 0) reading "Purchase for 100"
		root.anchoredPosition = new Vector2(0f, GarageActionRowBottom);
		root.sizeDelta = new Vector2(GaragePurchaseButtonWidth, GarageActionRowHeight);
		garagePurchaseButton = InfernoUi.MakeButton3D(this, root, "Unlock Selected Vehicle Button", "PURCHASE · 0 ◆", 0f, 0f, GaragePurchaseButtonWidth, GarageActionRowHeight, InfernoUi.Style.Plate, 28);
		garagePurchaseButtonText = InfernoUi.ButtonLabel(garagePurchaseButton);
		if (garagePurchaseButtonText != null)
		{
			garagePurchaseButtonText.fontStyle = FontStyle.Bold;
			garagePurchaseButtonText.color = new Color(0.98f, 0.9f, 1f, 1f);
			garagePurchaseButtonText.raycastTarget = false;
			garagePurchaseButtonText.resizeTextForBestFit = true;
			garagePurchaseButtonText.resizeTextMinSize = 18;
			garagePurchaseButtonText.resizeTextMaxSize = 28;
		}
		garagePurchaseButton.onClick.AddListener(() => { _ = PurchaseSelectedGarageVehicleAsync(); });
		root.gameObject.SetActive(false);
	}

	/* [user request 2026-09-21: cars] */
	// The tuning card under the action row: one row per upgrade (ENGINE / HANDLING / NITRO) with the
	// stat's five-segment bar, its level and a buy button showing the next level's price (or MAX).
	// Row geometry (x from the card centre, 20 px gaps): label 190 wide at -345 (-440..-250),
	// segment bar 380x26 at -40 (-230..150), level 70 wide at 205 (170..240), button 180x64 at 350 (260..440).
	private void MakeGarageTuningCard()
	{
		garageTuningRoot = InfernoLobby.Root(garageRoot.transform, "Garage Tuning Card");
		InfernoLobby.Place(garageTuningRoot, 0f, GarageTuningCardCentreY, GarageTuningCardWidth, GarageTuningCardHeight, new Vector2(0.5f, 0f));
		Image frame = InfernoUi.MakePanel(this, garageTuningRoot, "Frame", 0f, 0f, GarageTuningCardWidth, GarageTuningCardHeight);
		frame.raycastTarget = true;   // taps on the card must not spin the preview underneath it
		for (int i = 0; i < InfernoUpgrades.StatCount; i++)
		{
			InfernoUpgrades.Stat stat = (InfernoUpgrades.Stat)i;
			string statName = InfernoUpgrades.StatName(stat);
			float y = GarageTuningRowPitch - GarageTuningRowPitch * i;
			Text label = InfernoLobby.Label(this, garageTuningRoot, "Tuning Label " + statName, statName, -345f, y, 190f, 30f, 26, InfernoLobby.Lilac, (int)TextAnchor.MiddleLeft);
			InfernoLobby.Place(label.rectTransform, -345f, y, 190f, 30f, new Vector2(0.5f, 0.5f));
			garageTuningLabels[i] = label;
			garageTuningBars[i] = InfernoUi.MakeSegmentBar(this, garageTuningRoot, "Tuning Bar " + statName, -40f, y, 380f, 26f, InfernoUpgrades.MaxLevel, GarageTuningFill(stat));
			Text level = InfernoLobby.Label(this, garageTuningRoot, "Tuning Level " + statName, "0/" + InfernoUpgrades.MaxLevel, 205f, y, 70f, 30f, 22, InfernoLobby.Pearl);
			InfernoLobby.Place(level.rectTransform, 205f, y, 70f, 30f, new Vector2(0.5f, 0.5f));
			garageTuningLevels[i] = level;
			string buttonName = "Upgrade " + statName[0] + statName.Substring(1).ToLowerInvariant() + " Button";   // Upgrade Engine Button, ...
			Button button = InfernoUi.MakeButton3D(this, garageTuningRoot, buttonName, "◆ 0", 350f, y, 180f, 64f, InfernoUi.Style.Plate, 24);
			button.onClick.AddListener(() => BuyGarageUpgrade(stat));
			garageTuningButtons[i] = button;
		}
		RefreshGarageTuning();
	}

	private static Color GarageTuningFill(InfernoUpgrades.Stat stat)
	{
		switch (stat)
		{
			case InfernoUpgrades.Stat.Engine: return new Color(1f, 0.50f, 0.18f, 1f);
			case InfernoUpgrades.Stat.Handling: return new Color(0.24f, 0.70f, 1f, 1f);
			default: return new Color(0.76f, 0.30f, 1f, 1f);
		}
	}

	// Shows the selected card's car: levels, prices, MAX; the buy buttons only work on an owned car.
	private void RefreshGarageTuning()
	{
		if (garageTuningRoot == null || garageVehicleCards == null)
		{
			return;
		}
		bool valid = garageSelectedVehicleCardIndex >= 0 && garageSelectedVehicleCardIndex < garageVehicleCards.Length;
		string vehicleId = valid ? garageVehicleCards[garageSelectedVehicleCardIndex].VehicleId : null;
		bool known = InfernoCars.Contains(vehicleId);
		bool owned = known && SupabaseGameManager.IsVehicleUnlocked(vehicleId);
		garageTuningRoot.gameObject.SetActive(known);
		if (!known)
		{
			return;
		}
		for (int i = 0; i < InfernoUpgrades.StatCount; i++)
		{
			InfernoUpgrades.Stat stat = (InfernoUpgrades.Stat)i;
			int level = InfernoUpgrades.Level(vehicleId, stat);
			int price = InfernoUpgrades.Price(vehicleId, stat);
			if (garageTuningBars[i] != null)
			{
				InfernoUi.SetSegments(garageTuningBars[i], level);
			}
			if (garageTuningLevels[i] != null)
			{
				garageTuningLevels[i].text = level + "/" + InfernoUpgrades.MaxLevel;
			}
			if (garageTuningLabels[i] != null)
			{
				garageTuningLabels[i].color = InfernoLobby.Hex(InfernoLobby.Lilac, owned ? 1f : 0.55f);
			}
			Button button = garageTuningButtons[i];
			if (button == null)
			{
				continue;
			}
			Text label = InfernoUi.ButtonLabel(button);
			if (label != null)
			{
				label.text = price < 0 ? "MAX" : "◆ " + price;
			}
			button.interactable = owned && price >= 0 && !garagePurchaseBusy;
		}
	}

	private void BuyGarageUpgrade(InfernoUpgrades.Stat stat)
	{
		if (garagePurchaseBusy || garageVehicleCards == null || garageSelectedVehicleCardIndex < 0 || garageSelectedVehicleCardIndex >= garageVehicleCards.Length)
		{
			return;
		}
		string vehicleId = garageVehicleCards[garageSelectedVehicleCardIndex].VehicleId;
		if (!InfernoCars.Contains(vehicleId) || !SupabaseGameManager.IsVehicleUnlocked(vehicleId))
		{
			RefreshGarageTuning();
			return;
		}
		int price = InfernoUpgrades.Price(vehicleId, stat);
		if (price < 0)
		{
			return;
		}
		int crystals = SupabaseGameManager.Crystals;
		if (crystals == int.MaxValue)
		{
			return;
		}
		if (crystals < price)
		{
			if (garagePurchaseStatusText != null)
			{
				garagePurchaseStatusText.text = string.Format("NEED {0} MORE", price - crystals);
			}
			ShowGemShop();   // an unaffordable upgrade opens the diamond shop, like an unaffordable car
			return;
		}
		if (!InfernoUpgrades.Buy(this, vehicleId, stat))   // spends, saves, re-creates the active definition and refreshes the garage
		{
			UpdateGarageUi();
		}
	}

	// A variant's card shows its runtime-rendered icon once the preview world can render it
	// (InfernoCars.Icon needs a live pipeline, so the card is built with the base photo first).
	private void RefreshGarageCardIcons()
	{
		if (garageVehicleCards == null || garageVehicleCardPhotos == null)
		{
			return;
		}
		for (int i = 0; i < garageVehicleCards.Length && i < garageVehicleCardPhotos.Length; i++)
		{
			Image photo = garageVehicleCardPhotos[i];
			string vehicleId = garageVehicleCards[i].VehicleId;
			if (photo == null || InfernoCars.IdFromPicture(garageVehicleCards[i].IconResource) == null)
			{
				continue;
			}
			if (photo.sprite != null && photo.sprite.name == "Icon " + vehicleId)
			{
				continue;
			}
			Sprite icon = InfernoCars.Icon(this, vehicleId);
			if (icon != null)
			{
				SetGarageCardPhoto(photo, icon, true);
			}
		}
	}

	// original photo size: 1.55 x the slot width by 2.35 x its height (the 1:2 cut-out photos); a
	// rendered 4:3 icon is shown at 240x180 so its car reads about as big as the photographed ones.
	private void SetGarageCardPhoto(Image image, Sprite sprite, bool renderedIcon)
	{
		if (image == null || sprite == null)
		{
			return;
		}
		image.sprite = sprite;
		image.rectTransform.sizeDelta = renderedIcon ? new Vector2(240f, 180f) : new Vector2(GarageCardPhotoSlotWidth * 1.55f, GarageCardPhotoSlotHeight * 2.35f);
	}

	private async Task PurchaseSelectedGarageVehicleAsync()
	{
		if (garagePurchaseBusy || garageVehicleCards == null || garageSelectedVehicleCardIndex < 0 || garageSelectedVehicleCardIndex >= garageVehicleCards.Length)
		{
			return;
		}
		GarageVehicleCardData card = garageVehicleCards[garageSelectedVehicleCardIndex];
		string vehicleId = card.VehicleId;
		if (string.IsNullOrEmpty(vehicleId) || SupabaseGameManager.IsVehicleUnlocked(vehicleId))
		{
			UpdateGarageUi();
			return;
		}
		garagePurchaseBusy = true;
		if (garagePurchaseButton != null)
		{
			garagePurchaseButton.interactable = false;
		}
		if (garagePurchaseButtonText != null)
		{
			garagePurchaseButtonText.text = "CHECKING PROFILE...";
		}
		if (garagePurchaseStatusText != null)
		{
			garagePurchaseStatusText.text = "";
		}
		if (!await SupabaseGameManager.RequireOnlineEconomyAsync())
		{
			/* [user request 2026-09-27: cannot buy cars] Say why nothing happened. The button used to snap back
			   from "CHECKING PROFILE..." to PURCHASE with the status line blank, so an account session the game
			   could not refresh was indistinguishable from a dead button. Set after UpdateGarageUi so the
			   refresh cannot blank it again. */
			garagePurchaseBusy = false;
			UpdateGarageUi();
			if (garagePurchaseStatusText != null)
			{
				garagePurchaseStatusText.text = Application.internetReachability == NetworkReachability.NotReachable ? "NO INTERNET" : "SIGN IN TO BUY";
			}
			return;
		}
		if (SupabaseGameManager.IsVehicleUnlocked(vehicleId))
		{
			garagePurchaseBusy = false;
			UpdateGarageUi();
			return;
		}
		int crystals = SupabaseGameManager.Crystals;
		int price = InfernoCars.Price(vehicleId); /* [user request 2026-09-21: cars] */ // original: the literal 100 (GarageVehicleCrystalPrice) for every car
		if (price < 0 || crystals < price)
		{
			garagePurchaseBusy = false;
			if (garagePurchaseStatusText != null)
			{
				garagePurchaseStatusText.text = price < 0 ? "NOT FOR SALE" : string.Format("NEED {0} MORE", price - crystals);   // original: "NEED {0} MORE CRYSTALS"
			}
			UpdateGarageUi();
			ShowGemShop();
			return;
		}
		SupabaseGameManager.SetCrystals(crystals - price);   // original: crystals - 100
		SupabaseGameManager.SetVehicleUnlocked(vehicleId, true);
		if (garagePurchaseActionRoot != null)
		{
			garagePurchaseActionRoot.gameObject.SetActive(false);
		}
		if (!await SupabaseGameManager.SyncGaragePurchaseAsync(vehicleId))
		{
			Debug.LogWarning("[Supabase] Garage purchase remains queued for verification. Vehicle=" + vehicleId);
		}
		garagePurchaseBusy = false;
		garagePendingVehicleId = vehicleId;
		garagePendingVehicleApplyDelay = 0.05f;
		UpdateGarageUi();
	}

	private void MakeGarageVehicleSelector()
	{
		garageVehicleCards = CreateGarageVehicleCards();
		garageSelectedVehicleCardIndex = GetGarageVehicleCardIndexForVehicle(ActiveVehicle.Id);
		RectTransform selector = new GameObject("Garage Vehicle Card Selector").AddComponent<RectTransform>();
		selector.SetParent(garageRoot.transform, false);
		ApplyAnchor(selector, TextAnchor.UpperCenter);
		selector.anchoredPosition = new Vector2(0f, -56f);
		selector.sizeDelta = new Vector2(930f, 560f);
		garageVehicleCardDots = null;
		RectTransform scrollRect = new GameObject("Garage Vehicle Cards Scroll").AddComponent<RectTransform>();
		scrollRect.SetParent(selector, false);
		ApplyAnchor(scrollRect, TextAnchor.UpperCenter);
		scrollRect.anchoredPosition = new Vector2(0f, -200.64f);
		scrollRect.sizeDelta = new Vector2(930f, 468f);
		Image scrollSurface = scrollRect.gameObject.AddComponent<Image>();
		scrollSurface.color = new Color(1f, 1f, 1f, 0.001f);
		scrollSurface.raycastTarget = true;
		scrollSurface.canvasRenderer.cullTransparentMesh = false;
		ScrollRect scroll = scrollRect.gameObject.AddComponent<ScrollRect>();
		scroll.horizontal = true;
		scroll.vertical = false;
		scroll.movementType = ScrollRect.MovementType.Clamped;
		scroll.inertia = true;
		scroll.decelerationRate = 0.08f;
		scroll.scrollSensitivity = 42f;
		scroll.enabled = false;
		RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>();
		viewport.SetParent(scrollRect, false);
		viewport.anchorMin = Vector2.zero;
		viewport.anchorMax = Vector2.one;
		viewport.offsetMin = Vector2.zero;
		viewport.offsetMax = Vector2.zero;
		Image viewportSurface = viewport.gameObject.AddComponent<Image>();
		viewportSurface.color = new Color(1f, 1f, 1f, 0.001f);
		viewportSurface.raycastTarget = true;
		viewportSurface.canvasRenderer.cullTransparentMesh = false;
		viewport.gameObject.AddComponent<RectMask2D>();
		AddGarageVehicleCardDragEvents(viewport.gameObject, -1);
		RectTransform content = new GameObject("Content").AddComponent<RectTransform>();
		content.SetParent(viewport, false);
		content.anchorMin = new Vector2(0f, 0.5f);
		content.anchorMax = new Vector2(0f, 0.5f);
		content.pivot = new Vector2(0f, 0.5f);
		content.anchoredPosition = Vector2.zero;
		scroll.viewport = viewport;
		scroll.content = content;
		garageVehicleScroll = scroll;
		float edgePadding = Mathf.Max(18f, (scrollRect.sizeDelta.x - 392f) * 0.5f);
		HorizontalLayoutGroup layout = content.gameObject.AddComponent<HorizontalLayoutGroup>();
		layout.padding = new RectOffset(Mathf.RoundToInt(18f), Mathf.RoundToInt(edgePadding), 0, 0);
		layout.spacing = 22f;
		layout.childAlignment = TextAnchor.MiddleLeft;
		layout.childControlWidth = true;
		layout.childControlHeight = true;
		layout.childForceExpandWidth = false;
		layout.childForceExpandHeight = false;
		content.sizeDelta = new Vector2(18f + edgePadding + garageVehicleCards.Length * 392f + (garageVehicleCards.Length - 1) * 22f, 456f);
		garageVehicleCardsContent = content;
		garageVehicleCardWidth = 392f;
		garageVehicleCardStep = 414f;
		garageVehicleSidePadding = 18f;
		garageVehicleMaxScroll = Mathf.Max(0f, content.sizeDelta.x - scrollRect.sizeDelta.x);
		garageVehicleCardButtons = new Button[garageVehicleCards.Length];
		garageVehicleCardBorders = new Image[garageVehicleCards.Length];
		garageVehicleCardFills = new Image[garageVehicleCards.Length];
		garageVehicleLockOverlays = new Image[garageVehicleCards.Length];
		garageVehicleActionLabels = new Text[garageVehicleCards.Length];
		/* [user request 2026-09-21: cars] */
		garageVehicleCardPriceTexts = new Text[garageVehicleCards.Length];
		garageVehicleCardStatFills = new Image[garageVehicleCards.Length][];
		garageVehicleCardPhotos = new Image[garageVehicleCards.Length];
		for (int i = 0; i < garageVehicleCards.Length; i++)
		{
			Button card = MakeGarageVehicleCard(content, garageVehicleCards[i], i, Vector2.zero, new Vector2(392f, 456f));
			LayoutElement element = card.gameObject.AddComponent<LayoutElement>();
			element.minWidth = 392f;
			element.preferredWidth = 392f;
			element.minHeight = 456f;
			element.preferredHeight = 456f;
			element.flexibleWidth = 0f;
			element.flexibleHeight = 0f;
			garageVehicleCardButtons[i] = card;
			AddGarageVehicleCardDragEvents(card.gameObject, i);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(content);
		SnapGarageVehicleScrollTo(garageSelectedVehicleCardIndex, true);
		garageSportCarButton = garageVehicleCardButtons.Length != 0 ? garageVehicleCardButtons[0] : null;
		garageFlyingCarButton = garageVehicleCardButtons.Length > 1 ? garageVehicleCardButtons[1] : null;
		garageSportCarLabel = garageVehicleActionLabels.Length != 0 ? garageVehicleActionLabels[0] : null;
		garageFlyingCarLabel = garageVehicleActionLabels.Length > 1 ? garageVehicleActionLabels[1] : null;
		UpdateGarageUi();
	}

	private GarageVehicleCardData[] CreateGarageVehicleCards()
	{
		/* [user request 2026-09-21: cars] */
		// original: four hand-written cards (sportcar "Phantom GT" LEGENDARY, flying_car "Aero Wraith" EPIC,
		// infal_model "IFNAL P900" MYTHIC, car3 "Cayman GT4" LEGENDARY) + InfernoGarage.AppendCards (25 editions);
		// the catalogue's ten cards come in price order with rarity-coloured tints.
		return InfernoGarage.AppendCards(this, InfernoCars.Cards(this));
	}

	private Button MakeGarageVehicleCard(Transform parent, GarageVehicleCardData data, int index, Vector2 pos, Vector2 size)
	{
		GameObject cardObject = new GameObject("VehicleCard_" + (string.IsNullOrEmpty(data.VehicleId) ? index.ToString() : data.VehicleId));
		cardObject.transform.SetParent(parent, false);
		RectTransform rect = cardObject.AddComponent<RectTransform>();
		rect.anchorMin = new Vector2(0f, 0.5f);
		rect.anchorMax = new Vector2(0f, 0.5f);
		rect.pivot = new Vector2(0f, 0.5f);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		Button button = cardObject.AddComponent<Button>();
		button.transition = Selectable.Transition.None;
		Image border = cardObject.AddComponent<Image>();
		border.sprite = roundedGlassSprite;
		border.type = Image.Type.Sliced;
		border.color = data.VisualLocked ? new Color(0.2f, 0.16f, 0.27f, 0.24f) : new Color(data.Color.r, data.Color.g, data.Color.b, 0.62f);
		button.targetGraphic = border;
		garageVehicleCardBorders[index] = border;
		Image background = MakeUiBlock(rect, "Card Background", Vector2.zero, size - new Vector2(4f, 4f), TextAnchor.MiddleCenter, new Color(0.07f, 0.045f, 0.12f, 1f));
		background.sprite = GetGarageVehicleCardFillSprite();
		background.type = Image.Type.Sliced;
		background.raycastTarget = false;
		garageVehicleCardFills[index] = background;
		Image depthGlow = MakeUiBlock(rect, "Card Depth Glow", new Vector2(0f, 58f), new Vector2(size.x * 0.78f, size.y * 0.42f), TextAnchor.MiddleCenter, new Color(data.Color.r, data.Color.g, data.Color.b, data.VisualLocked ? 0.04f : 0.12f));
		depthGlow.sprite = caseRadialGlowSprite;
		depthGlow.raycastTarget = false;
		MakeGarageRarityBadge(rect, data, new Vector2(20f, -18f));
		RectTransform photoSlot = new GameObject("PhotoSlot").AddComponent<RectTransform>();
		photoSlot.SetParent(rect, false);
		ApplyAnchor(photoSlot, TextAnchor.MiddleCenter);
		photoSlot.anchoredPosition = new Vector2(0f, 74f);
		photoSlot.sizeDelta = new Vector2(size.x - 56f, 142f);
		Image photoGlow = MakeUiBlock(photoSlot, "Photo Glow", Vector2.zero, new Vector2(210f, 132f), TextAnchor.MiddleCenter, new Color(data.Color.r, data.Color.g, data.Color.b, 0.16f));
		photoGlow.sprite = caseRadialGlowSprite;
		photoGlow.raycastTarget = false;
		Sprite photo = LoadResourceSprite(data.IconResource);
		if (photo != null)
		{
			Image photoImage = new GameObject("Car Photo").AddComponent<Image>();
			photoImage.transform.SetParent(photoSlot);
			photoImage.color = Color.white;
			photoImage.preserveAspect = true;
			photoImage.raycastTarget = false;
			RectTransform photoRect = photoImage.rectTransform;
			ApplyAnchor(photoRect, TextAnchor.MiddleCenter);
			photoRect.anchoredPosition = new Vector2(0f, 6f);
			/* [user request 2026-09-21: cars] */ // original: photoImage.sprite = photo; photoRect.sizeDelta = new Vector2(photoSlot.sizeDelta.x * 1.55f, photoSlot.sizeDelta.y * 2.35f);
			SetGarageCardPhoto(photoImage, photo, InfernoCars.HasIcon(data.VehicleId) && photo.name == "Icon " + data.VehicleId);
			garageVehicleCardPhotos[index] = photoImage;
		}
		RectTransform info = new GameObject("Info Panel").AddComponent<RectTransform>();
		info.SetParent(rect, false);
		ApplyAnchor(info, TextAnchor.LowerCenter);
		info.anchoredPosition = new Vector2(0f, GarageCardInfoBottom);
		info.sizeDelta = new Vector2(size.x - 54f, GarageCardInfoHeight);
		/* [user request 2026-09-27: card data] the car's data reads bigger and brighter inside the same card: every row's
		   height, font and y comes from the GarageCard* constants (original values noted there); the rows now end 30 px
		   above the card edge instead of 57. */
		Text name = MakeText(info, data.Name, GarageCardNameFontMax, TextAnchor.UpperLeft, Vector2.zero);   // original: 28
		name.fontStyle = FontStyle.Bold;
		name.color = Color.white;
		name.GetComponent<RectTransform>().sizeDelta = new Vector2(info.sizeDelta.x, GarageCardNameHeight);   // original: 36f
		/* [user request 2026-09-21: no overlaps] long model names (AERO WRAITH X, CAYMAN CLUBSPORT) wrapped into the
		   subtitle: name and subtitle shrink to one line instead (best fit, originally 18..28 / 12..17) and the subtitle
		   starts under the name's rect (was -34). */
		name.horizontalOverflow = HorizontalWrapMode.Wrap;
		name.verticalOverflow = VerticalWrapMode.Truncate;
		name.resizeTextForBestFit = true;
		name.resizeTextMinSize = GarageCardNameFontMin;
		name.resizeTextMaxSize = GarageCardNameFontMax;
		Text subtitle = MakeText(info, data.Subtitle, GarageCardSubtitleFontMax, TextAnchor.UpperLeft, new Vector2(2f, -GarageCardNameHeight));   // original: 17 at (2f, -36f)
		subtitle.color = new Color(0.78f, 0.7f, 0.92f, 1f);   // original: (0.58f, 0.5f, 0.7f, 0.85f)
		subtitle.GetComponent<RectTransform>().sizeDelta = new Vector2(info.sizeDelta.x - 4f, GarageCardSubtitleHeight);   // original: (info.sizeDelta.x, 28f), then 24f
		subtitle.horizontalOverflow = HorizontalWrapMode.Wrap;
		subtitle.verticalOverflow = VerticalWrapMode.Truncate;
		subtitle.resizeTextForBestFit = true;
		subtitle.resizeTextMinSize = GarageCardSubtitleFontMin;
		subtitle.resizeTextMaxSize = GarageCardSubtitleFontMax;
		/* [user request 2026-09-21: cars] */ // original: MakeGarageCardStat(info, "POWER", data.PowerValue, data.Power, data.Color, new Vector2(0f, -78f), info.sizeDelta.x, false);
		// Info panel rows (y from its top, 150 tall): name 0..-42, subtitle -42..-70, price row -74..-106,
		// three mini-bar columns (label -112..-132, bar -134..-142) at x 0 / 116 / 232, 106 wide.
		Text priceRow = MakeText(info, "◆ 0", GarageCardPriceFont, TextAnchor.UpperLeft, new Vector2(4f, GarageCardPriceY));   // x 4 (was 0, then 3): the diamond glyph's left bearing must stay inside the panel (layout audit)
		priceRow.fontStyle = FontStyle.Bold;
		priceRow.color = InfernoLobby.Hex(InfernoLobby.Pearl);
		priceRow.raycastTarget = false;
		priceRow.GetComponent<RectTransform>().sizeDelta = new Vector2(info.sizeDelta.x - 8f, GarageCardPriceHeight);   // original: (info.sizeDelta.x - 6f, 22f)
		garageVehicleCardPriceTexts[index] = priceRow;
		garageVehicleCardStatFills[index] = new Image[InfernoUpgrades.StatCount];
		garageVehicleCardStatFills[index][0] = MakeGarageCardMiniStat(info, "POWER", data.Power, data.Color, new Vector2(0f, GarageCardStatLabelY));
		garageVehicleCardStatFills[index][1] = MakeGarageCardMiniStat(info, "HANDLING", data.Handling, data.Color, new Vector2(GarageMiniBarWidth + GarageMiniBarGap, GarageCardStatLabelY));
		garageVehicleCardStatFills[index][2] = MakeGarageCardMiniStat(info, "NITRO", data.Speed, data.Color, new Vector2(2f * (GarageMiniBarWidth + GarageMiniBarGap), GarageCardStatLabelY));
		Text actionLabel = MakeText(rect, "", 1, TextAnchor.MiddleCenter, Vector2.zero);
		actionLabel.color = Color.clear;
		actionLabel.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		garageVehicleActionLabels[index] = actionLabel;
		Vector2 lockSize = size - new Vector2(16f, 16f);
		Image lockMask = MakeUiBlock(rect, "Locked Card Rounded Mask", Vector2.zero, lockSize, TextAnchor.MiddleCenter, Color.white);
		lockMask.sprite = GetGarageVehicleCardFillSprite();
		lockMask.type = Image.Type.Sliced;
		lockMask.raycastTarget = false;
		lockMask.gameObject.AddComponent<Mask>().showMaskGraphic = false;
		garageVehicleLockOverlays[index] = lockMask;
		/* [user request 2026-09-22: cars] */
		// original: the shipped "249" blur strip (white, alpha 0.2, the full mask) with the lock at y 58 and a dark "LOCKED" at y 4.
		// A translucent dark tint dims the photo, name and subtitle (card top 220 down to -120, just above the
		// price row at -120..-142, so the price stays bright in the rarity colour); lock and label are centred on
		// the photo slot (3..145): icon 72 px at 94 (58..130), text 34 px at 38 (21..55), glow behind the icon.
		/* [user request 2026-09-27: card data] */ // original: new Vector2(0f, 50f), new Vector2(lockSize.x, 340f) - the strip's bottom sat on the old price row's top (108 above the card edge)
		float blurTop = size.y * 0.5f - 8f;   // the mask's top edge (220)
		float blurBottom = GarageCardInfoBottom + GarageCardInfoHeight + GarageCardPriceY - size.y * 0.5f;   // the price row's top, in card-centre coordinates (-130)
		Image blur = MakeUiBlock(lockMask.transform, "249 Blur Texture", new Vector2(0f, (blurTop + blurBottom) * 0.5f), new Vector2(lockSize.x, blurTop - blurBottom), TextAnchor.MiddleCenter, new Color(0.02f, 0.01f, 0.05f, 0.6f));
		blur.raycastTarget = false;
		Image lockGlow = MakeUiBlock(lockMask.transform, "Lock Glow", new Vector2(0f, 94f), new Vector2(146f, 146f), TextAnchor.MiddleCenter, new Color(data.Color.r, data.Color.g, data.Color.b, 0.22f));   // original: y 58
		lockGlow.sprite = caseRadialGlowSprite;
		lockGlow.raycastTarget = false;
		Sprite lockSprite = GetGarageLockSprite();
		if (lockSprite != null)
		{
			Image lockIcon = new GameObject("Garage Lock Icon").AddComponent<Image>();
			lockIcon.transform.SetParent(lockMask.transform, false);
			lockIcon.sprite = lockSprite;
			lockIcon.preserveAspect = true;
			lockIcon.raycastTarget = false;
			lockIcon.color = Color.white;
			RectTransform lockRect = lockIcon.rectTransform;
			ApplyAnchor(lockRect, TextAnchor.MiddleCenter);
			lockRect.anchoredPosition = new Vector2(0f, 94f);   // original: 58f
			lockRect.sizeDelta = new Vector2(72f, 72f);
		}
		Text lockedText = MakeText(lockMask.transform, "LOCKED", 24, TextAnchor.MiddleCenter, new Vector2(0f, 38f));   // original: (0, 4)
		lockedText.fontStyle = FontStyle.Bold;
		lockedText.color = new Color(0.92f, 0.86f, 1f, 0.96f);   // original: (0.18, 0.11, 0.28, 0.96) on the light blur strip
		lockedText.raycastTarget = false;
		lockedText.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 34f);
		lockMask.gameObject.SetActive(data.VisualLocked);
		Image surface = MakeUiBlock(rect, "Full Card Interaction Surface", Vector2.zero, size, TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0.001f));
		surface.raycastTarget = true;
		surface.canvasRenderer.cullTransparentMesh = false;
		AddGarageVehicleCardDragEvents(surface.gameObject, index);
		return button;
	}

	private Sprite GetGarageLockSprite()
	{
		if (garageLockSprite != null)
		{
			return garageLockSprite;
		}
		Sprite sprite = LoadResourceSprite("замок иконка");
		if (sprite != null)
		{
			return garageLockSprite = sprite;
		}
		sprite = LoadResourceSprite("Space_Exploration_GUI_Kit/Icons/lock-1-256");
		if (sprite != null)
		{
			return garageLockSprite = sprite;
		}
		sprite = LoadResourceSprite("Space_Exploration_GUI_Kit/Icons/lock-2-256");
		if (sprite != null)
		{
			return garageLockSprite = sprite;
		}
		return garageLockSprite = LoadResourceSprite("Space_Exploration_GUI_Kit/Icons/lock-1-128");
	}

	private void MakeGarageRarityBadge(Transform parent, GarageVehicleCardData data, Vector2 pos)
	{
		Image badge = MakeThinGlassPanel(parent, data.Rarity + " Badge", pos, new Vector2(154f, 38f), TextAnchor.UpperLeft, new Color(0.075f, 0.05f, 0.13f, 0.78f), new Color(data.Color.r, data.Color.g, data.Color.b, 0.36f), 1f);
		badge.raycastTarget = false;
		Text label = MakeText(badge.transform, data.Rarity, 17, TextAnchor.MiddleCenter, Vector2.zero);
		label.fontStyle = FontStyle.Bold;
		label.color = data.Accent;
		label.GetComponent<RectTransform>().sizeDelta = new Vector2(140f, 30f);
	}

	private Sprite GetGarageVehicleCardFillSprite()
	{
		if (garageVehicleCardFillSprite != null)
		{
			return garageVehicleCardFillSprite;
		}
		Texture2D texture = new Texture2D(192, 256, TextureFormat.RGBA32, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Bilinear;
		for (int y = 0; y < 256; y++)
		{
			float v = y / 255f;
			Color top = new Color(0.108f, 0.078f, 0.18f, 1f);
			Color mid = new Color(0.061f, 0.047f, 0.105f, 1f);
			Color bottom = new Color(0.035f, 0.03f, 0.07f, 1f);
			Color column = v > 0.45f ? Color.Lerp(mid, top, Mathf.InverseLerp(0.45f, 1f, v)) : Color.Lerp(bottom, mid, Mathf.InverseLerp(0f, 0.45f, v));
			for (int x = 0; x < 192; x++)
			{
				float u = x / 191f;
				float dx = Mathf.Max(new float[3] { 22 - x, x - 169, 0f });
				float dy = Mathf.Max(new float[3] { 22 - y, y - 233, 0f });
				float distance = Mathf.Sqrt(dx * dx + dy * dy);
				float alpha = Mathf.Clamp01((23f - distance) / 2.5f);
				float highlight = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(u, v), new Vector2(0.22f, 0.72f)) / 0.72f);
				Color pixel = Color.Lerp(column, new Color(0.18f, 0.1f, 0.32f, 1f), highlight * highlight * 0.28f);
				texture.SetPixel(x, y, new Color(pixel.r, pixel.g, pixel.b, alpha));
			}
		}
		texture.Apply(false, true);
		garageVehicleCardFillSprite = Sprite.Create(texture, new Rect(0f, 0f, 192f, 256f), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(22f, 22f, 22f, 22f));
		return garageVehicleCardFillSprite;
	}

	/* [user request 2026-09-21: cars] */
	// One of the card's three mini bars (POWER / HANDLING / NITRO): a 15 px label over a 106x8 bar
	// /* [user request 2026-09-27: card data] */ (original: 12 px over 106x5, dimmer label, bar alphas 0.085 / 0.88);
	// returns the fill so UpdateGarageVehicleCardVisuals can grow it with the upgrade levels.
	private Image MakeGarageCardMiniStat(Transform parent, string label, float amount, Color color, Vector2 pos)
	{
		Text labelText = MakeText(parent, label, GarageCardStatLabelFont, TextAnchor.UpperLeft, pos);
		labelText.fontStyle = FontStyle.Bold;
		labelText.color = new Color(0.86f, 0.78f, 0.97f, 0.96f);   // original: (0.7f, 0.6f, 0.84f, 0.72f)
		labelText.raycastTarget = false;
		labelText.GetComponent<RectTransform>().sizeDelta = new Vector2(GarageMiniBarWidth, GarageCardStatLabelHeight);
		Image barBackground = MakeUiBlock(parent, label + " Bar BG", new Vector2(pos.x, pos.y - GarageMiniBarOffset), new Vector2(GarageMiniBarWidth, GarageMiniBarHeight), TextAnchor.UpperLeft, new Color(1f, 1f, 1f, 0.16f));
		barBackground.sprite = roundedGlassSprite;
		barBackground.type = Image.Type.Sliced;
		barBackground.raycastTarget = false;
		Image barFill = MakeUiBlock(barBackground.transform, label + " Bar Fill", Vector2.zero, new Vector2(Mathf.Clamp01(amount) * GarageMiniBarWidth, GarageMiniBarHeight), TextAnchor.MiddleLeft, new Color(color.r, color.g, color.b, 1f));
		barFill.sprite = roundedGlassSprite;
		barFill.type = Image.Type.Sliced;
		barFill.raycastTarget = false;
		return barFill;
	}

	private void MakeGarageCardStat(Transform parent, string label, string value, float amount, Color color, Vector2 pos, float width, bool dimmed)
	{
		Text labelText = MakeText(parent, label, 13, TextAnchor.UpperLeft, pos);
		labelText.fontStyle = FontStyle.Bold;
		labelText.color = dimmed ? new Color(0.45f, 0.38f, 0.56f, 0.38f) : new Color(0.7f, 0.6f, 0.84f, 0.72f);
		labelText.GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 22f);
		Text valueText = MakeText(parent, value, 13, TextAnchor.UpperRight, new Vector2(-22f, pos.y));
		valueText.fontStyle = FontStyle.Bold;
		valueText.color = dimmed ? new Color(0.52f, 0.44f, 0.64f, 0.38f) : new Color(0.9f, 0.84f, 0.98f, 0.92f);
		valueText.GetComponent<RectTransform>().sizeDelta = new Vector2(150f, 22f);
		Image barBackground = MakeUiBlock(parent, label + " Bar BG", new Vector2(pos.x, pos.y - 21f), new Vector2(width, 5f), TextAnchor.UpperLeft, new Color(1f, 1f, 1f, dimmed ? 0.025f : 0.085f));
		barBackground.sprite = roundedGlassSprite;
		barBackground.type = Image.Type.Sliced;
		barBackground.raycastTarget = false;
		float fillWidth = Mathf.Clamp01(amount) * width;
		Image barFill = MakeUiBlock(barBackground.transform, label + " Bar Fill", Vector2.zero, new Vector2(fillWidth, 5f), TextAnchor.MiddleLeft, new Color(color.r, color.g, color.b, dimmed ? 0.12f : 0.88f));
		barFill.sprite = roundedGlassSprite;
		barFill.type = Image.Type.Sliced;
		barFill.raycastTarget = false;
	}

	private void SelectGarageVehicleCard(int index)
	{
		if (garageVehicleCards == null || index < 0 || index >= garageVehicleCards.Length)
		{
			return;
		}
		garageSelectedVehicleCardIndex = index;
		if (garagePurchaseStatusText != null)
		{
			garagePurchaseStatusText.text = "";
		}
		SnapGarageVehicleScrollTo(index, false);
		UpdateGarageVehicleCardVisuals();
		GarageVehicleCardData card = garageVehicleCards[index];
		string vehicleId = card.VehicleId;
		PreviewGarageVehicle(vehicleId);
		if (!card.VisualLocked && !string.IsNullOrEmpty(vehicleId))
		{
			QueueGarageVehicleSelection(vehicleId);
			return;
		}
		garagePendingVehicleId = null;
		UpdateGarageUi();
	}

	private void QueueGarageVehicleSelection(string vehicleId)
	{
		garagePendingVehicleId = vehicleId;
		garagePendingVehicleApplyDelay = 0.1f;
	}

	private void SelectGarageVehicleCardFromClick(int index)
	{
		if (garageVehicleCardPointerMoved)
		{
			return;
		}
		SelectGarageVehicleCard(index);
	}

	private void SelectGarageVehicleCardAtPointer(PointerEventData eventData)
	{
		if (eventData == null || garageVehicleCardPointerMoved || garageVehicleCardsContent == null || garageVehicleScroll == null || garageVehicleScroll.viewport == null)
		{
			return;
		}
		RectTransform viewport = garageVehicleScroll.viewport;
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, eventData.position, eventData.pressEventCamera, out Vector2 local))
		{
			return;
		}
		int index = Mathf.Clamp(Mathf.RoundToInt((local.x - viewport.rect.xMin - garageVehicleCardsContent.anchoredPosition.x - garageVehicleSidePadding - garageVehicleCardWidth * 0.5f) / Mathf.Max(1f, garageVehicleCardStep)), 0, garageVehicleCards.Length - 1);
		SelectGarageVehicleCard(index);
	}

	private void AddGarageVehicleCardDragEvents(GameObject target, int cardIndex = -1)
	{
		EventTrigger trigger = target.GetComponent<EventTrigger>();
		if (trigger == null)
		{
			trigger = target.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
		down.callback.AddListener(data => BeginGarageVehicleCardPointer(data as PointerEventData));
		trigger.triggers.Add(down);
		EventTrigger.Entry initialize = new EventTrigger.Entry { eventID = EventTriggerType.InitializePotentialDrag };
		initialize.callback.AddListener(data => InitializeGarageVehicleCardDrag(data as PointerEventData));
		trigger.triggers.Add(initialize);
		EventTrigger.Entry begin = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
		begin.callback.AddListener(data => BeginGarageVehicleCardDrag(data as PointerEventData));
		trigger.triggers.Add(begin);
		EventTrigger.Entry drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
		drag.callback.AddListener(data => DragGarageVehicleCard(data as PointerEventData));
		trigger.triggers.Add(drag);
		EventTrigger.Entry end = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
		end.callback.AddListener(data => EndGarageVehicleCardDrag(data as PointerEventData));
		trigger.triggers.Add(end);
		if (cardIndex >= 0)
		{
			EventTrigger.Entry click = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
			click.callback.AddListener(_ => SelectGarageVehicleCardFromClick(cardIndex));
			trigger.triggers.Add(click);
		}
	}

	private void BeginGarageVehicleCardPointer(PointerEventData eventData)
	{
		garageVehicleCardPointerMoved = false;
		garageVehicleCarouselVelocityX = 0f;
	}

	private void InitializeGarageVehicleCardDrag(PointerEventData eventData)
	{
		garageVehicleCardPointerMoved = false;
	}

	private void BeginGarageVehicleCardDrag(PointerEventData eventData)
	{
		if (eventData == null || garageVehicleCardsContent == null)
		{
			return;
		}
		garageVehicleCardPointerMoved = true;
		garageVehicleCarouselDragging = true;
		garageVehicleCarouselVelocityX = 0f;
		garageVehicleCarouselDragVelocityX = 0f;
		garageVehicleCarouselDragStartX = garageVehicleCardsContent.anchoredPosition.x;
		garageVehicleCarouselPointerStartX = eventData.position.x;
	}

	private void DragGarageVehicleCard(PointerEventData eventData)
	{
		if (eventData == null || garageVehicleCardsContent == null)
		{
			return;
		}
		if ((eventData.position - eventData.pressPosition).sqrMagnitude > 36f)
		{
			garageVehicleCardPointerMoved = true;
		}
		float rawX = garageVehicleCarouselDragStartX + eventData.position.x - garageVehicleCarouselPointerStartX;
		float x = ClampGarageVehicleCarouselX(rawX);
		float dt = Mathf.Max(Time.unscaledDeltaTime, 0.001f);
		garageVehicleCarouselDragVelocityX = Mathf.Lerp(garageVehicleCarouselDragVelocityX, eventData.delta.x / dt, 0.35f);
		garageVehicleCardsContent.anchoredPosition = new Vector2(x, garageVehicleCardsContent.anchoredPosition.y);
		garageVehicleCarouselTargetX = x;
		UpdateGarageSelectedCardFromScroll();
	}

	private void EndGarageVehicleCardDrag(PointerEventData eventData)
	{
		garageVehicleCarouselDragging = false;
		float projected = garageVehicleCardsContent != null ? ClampGarageVehicleCarouselX(garageVehicleCardsContent.anchoredPosition.x + garageVehicleCarouselDragVelocityX * 0.16f) : 0f;
		int index = GetNearestGarageVehicleCardIndex(projected);
		garageVehicleCarouselDragVelocityX = 0f;
		SelectGarageVehicleCard(index);
	}

	private float ClampGarageVehicleCarouselX(float x)
	{
		UpdateGarageVehicleScrollBounds();
		return Mathf.Clamp(x, -garageVehicleMaxScroll, 0f);
	}

	private void SnapGarageVehicleScrollTo(int index, bool instant = false)
	{
		if (garageVehicleCardsContent == null || garageVehicleCards == null)
		{
			return;
		}
		UpdateGarageVehicleScrollBounds();
		float viewportWidth = GetGarageVehicleViewportWidth();
		float cardCenter = garageVehicleSidePadding + index * garageVehicleCardStep + garageVehicleCardWidth * 0.5f;
		float x = Mathf.Clamp(viewportWidth * 0.5f - cardCenter, -garageVehicleMaxScroll, 0f);
		garageVehicleCarouselTargetX = x;
		garageVehicleCarouselVelocityX = 0f;
		if (instant)
		{
			garageVehicleCardsContent.anchoredPosition = new Vector2(x, garageVehicleCardsContent.anchoredPosition.y);
		}
	}

	private void UpdateGarageSelectedCardFromScroll()
	{
		if (garageVehicleCards == null || garageVehicleCardsContent == null || garageVehicleCards.Length == 0)
		{
			return;
		}
		float step = garageVehicleCardStep;
		if (step <= 0.001f)
		{
			return;
		}
		float viewportWidth = GetGarageVehicleViewportWidth();
		int index = Mathf.Clamp(Mathf.RoundToInt((-garageVehicleCardsContent.anchoredPosition.x + viewportWidth * 0.5f - garageVehicleSidePadding - garageVehicleCardWidth * 0.5f) / step), 0, garageVehicleCards.Length - 1);
		if (index == garageSelectedVehicleCardIndex)
		{
			return;
		}
		garageSelectedVehicleCardIndex = index;
		UpdateGarageVehicleCardVisuals();
	}

	private int GetNearestGarageVehicleCardIndex()
	{
		if (garageVehicleCardsContent == null)
		{
			return 0;
		}
		return GetNearestGarageVehicleCardIndex(garageVehicleCardsContent.anchoredPosition.x);
	}

	private int GetNearestGarageVehicleCardIndex(float contentX)
	{
		if (garageVehicleCards == null || garageVehicleCardsContent == null || garageVehicleCards.Length == 0)
		{
			return 0;
		}
		float step = Mathf.Max(0.001f, garageVehicleCardStep);
		float viewportWidth = GetGarageVehicleViewportWidth();
		return Mathf.Clamp(Mathf.RoundToInt((-contentX + viewportWidth * 0.5f - garageVehicleSidePadding - garageVehicleCardWidth * 0.5f) / step), 0, garageVehicleCards.Length - 1);
	}

	private float GetGarageVehicleViewportWidth()
	{
		if (garageVehicleScroll == null || garageVehicleScroll.viewport == null)
		{
			return 930f;
		}
		return garageVehicleScroll.viewport.rect.width;
	}

	private void UpdateGarageVehicleScrollBounds()
	{
		if (garageVehicleCardsContent == null)
		{
			return;
		}
		garageVehicleMaxScroll = Mathf.Max(0f, garageVehicleCardsContent.sizeDelta.x - GetGarageVehicleViewportWidth());
	}

	private void UpdateGarageVehicleCarousel(float dt)
	{
		if (garageVehicleCardsContent == null)
		{
			return;
		}
		if (garageVehicleCarouselDragging)
		{
			garagePendingVehicleApplyDelay = Mathf.Max(garagePendingVehicleApplyDelay, 0.08f);
			return;
		}
		float target = ClampGarageVehicleCarouselX(garageVehicleCarouselTargetX);
		float current = garageVehicleCardsContent.anchoredPosition.x;
		float x = Mathf.SmoothDamp(current, target, ref garageVehicleCarouselVelocityX, 0.16f, 7200f, dt);
		if (Mathf.Abs(x - target) < 0.08f && Mathf.Abs(garageVehicleCarouselVelocityX) < 0.08f)
		{
			x = target;
		}
		if (!Mathf.Approximately(current, x))
		{
			garageVehicleCardsContent.anchoredPosition = new Vector2(x, garageVehicleCardsContent.anchoredPosition.y);
		}
		if (!string.IsNullOrEmpty(garagePendingVehicleId) && Mathf.Abs(x - target) < 0.5f)
		{
			garagePendingVehicleApplyDelay -= dt;
			if (garagePendingVehicleApplyDelay <= 0f)
			{
				string vehicleId = garagePendingVehicleId;
				garagePendingVehicleId = null;
				SelectVehicle(vehicleId);
			}
		}
	}

	private int GetGarageVehicleCardIndexForVehicle(string vehicleId)
	{
		if (garageVehicleCards == null || string.IsNullOrEmpty(vehicleId))
		{
			return 0;
		}
		for (int i = 0; i < garageVehicleCards.Length; i++)
		{
			if (string.Equals(garageVehicleCards[i].VehicleId, vehicleId, StringComparison.OrdinalIgnoreCase))
			{
				return i;
			}
		}
		return 0;
	}

	private Button MakeGarageVehicleButton(Transform parent, string label, float x, out Text labelText)
	{
		Button button = new GameObject(label + " Select Button").AddComponent<Button>();
		button.transform.SetParent(parent);
		Image background = button.gameObject.AddComponent<Image>();
		background.sprite = roundedGlassSprite;
		background.type = Image.Type.Sliced;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.MiddleCenter);
		rect.anchoredPosition = new Vector2(x, 0f);
		rect.sizeDelta = new Vector2(220f, 62f);
		button.targetGraphic = background;
		labelText = MakeText(button.transform, label, 20, TextAnchor.MiddleCenter, Vector2.zero);
		labelText.fontStyle = FontStyle.Bold;
		labelText.raycastTarget = false;
		labelText.GetComponent<RectTransform>().sizeDelta = new Vector2(204f, 54f);
		return button;
	}

	private void MakeGarageBackground()
	{
		Image image = new GameObject("Garage Background Image").AddComponent<Image>();
		image.transform.SetParent(garageRoot.transform);
		image.raycastTarget = false;
		image.color = Color.white;
		Sprite sprite = LoadResourceSprite("garage12");
		if (sprite != null)
		{
			image.sprite = sprite;
		}
		image.preserveAspect = true;
		RectTransform rect = image.GetComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.anchoredPosition = Vector2.zero;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
	}

	private void MakeGarageAtmosphere()
	{
		MakeUiBlock(garageRoot.transform, "Garage Top Shade", new Vector2(0f, -120f), new Vector2(1080f, 300f), TextAnchor.UpperCenter, new Color(0f, 0f, 0f, 0.42f));
		MakeUiBlock(garageRoot.transform, "Garage Bottom Shade", new Vector2(0f, 0f), new Vector2(1080f, 690f), TextAnchor.LowerCenter, new Color(0f, 0f, 0f, 0.9f));
		MakeUiBlock(garageRoot.transform, "Garage Mid Smoke", new Vector2(0f, -760f), new Vector2(1080f, 520f), TextAnchor.UpperCenter, new Color(0f, 0f, 0f, 0.26f));
		MakeUiBlock(garageRoot.transform, "Garage Red Horizon", new Vector2(0f, -615f), new Vector2(960f, 5f), TextAnchor.UpperCenter, new Color(1f, 0.02f, 0.04f, 0.52f));
		Image ringGlow = MakeUiBlock(garageRoot.transform, "Garage Car Ring Glow", new Vector2(0f, -610f), new Vector2(820f, 260f), TextAnchor.UpperCenter, new Color(1f, 0.02f, 0.04f, 0.16f));
		ringGlow.sprite = softCircleSprite;
		Image neonRing = MakeUiBlock(garageRoot.transform, "Garage Car Neon Ring", new Vector2(0f, -610f), new Vector2(760f, 220f), TextAnchor.UpperCenter, new Color(1f, 0.02f, 0.04f, 0.52f));
		neonRing.sprite = softCircleSprite;
		Image cutout = MakeUiBlock(garageRoot.transform, "Garage Car Neon Ring Cutout", new Vector2(0f, -610f), new Vector2(715f, 190f), TextAnchor.UpperCenter, new Color(0f, 0f, 0f, 0f));
		cutout.sprite = softCircleSprite;
		for (int i = 0; i < 8; i++)
		{
			float x = -480f + i * 138f;
			float height = 36f + i % 3 * 28f;
			MakeUiBlock(garageRoot.transform, "Garage City Silhouette", new Vector2(x, -560f - height * 0.5f), new Vector2(82f, height), TextAnchor.UpperCenter, new Color(0.08f, 0f, 0.008f, 0.78f));
			MakeUiBlock(garageRoot.transform, "Garage City Windows", new Vector2(x, -572f), new Vector2(54f, 2f), TextAnchor.UpperCenter, new Color(1f, 0.02f, 0.04f, 0.35f));
		}
	}

	private void MakeGarageCar()
	{
		garagePreviewTexture = new RenderTexture(720, 1280, 16, RenderTextureFormat.ARGB32);
		garagePreviewTexture.name = "Garage Scene Preview Texture";
		garagePreviewTexture.antiAliasing = 1;
		garagePreviewTexture.filterMode = FilterMode.Bilinear;
		garagePreviewTexture.Create();
		garageCarPreview = new GameObject("Garage 3D Scene Preview").AddComponent<RawImage>();
		garageCarPreview.transform.SetParent(garageRoot.transform);
		garageCarPreview.texture = garagePreviewTexture;
		garageCarPreview.color = Color.white;
		garageCarPreview.raycastTarget = true;
		RectTransform rect = garageCarPreview.GetComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.anchoredPosition = Vector2.zero;
		rect.sizeDelta = Vector2.zero;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		AddGaragePreviewDragEvents(garageCarPreview.gameObject);
		MakeGaragePreviewWorld();
	}

	private void AddGaragePreviewDragEvents(GameObject target)
	{
		EventTrigger trigger = target.GetComponent<EventTrigger>();
		if (trigger == null)
		{
			trigger = target.AddComponent<EventTrigger>();
		}
		trigger.triggers.Clear();
		EventTrigger.Entry down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
		down.callback.AddListener(data => BeginGaragePreviewDrag(data as PointerEventData));
		trigger.triggers.Add(down);
		EventTrigger.Entry drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
		drag.callback.AddListener(data => DragGaragePreview(data as PointerEventData));
		trigger.triggers.Add(drag);
		EventTrigger.Entry up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
		up.callback.AddListener(_ => EndGaragePreviewDrag());
		trigger.triggers.Add(up);
		EventTrigger.Entry end = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
		end.callback.AddListener(_ => EndGaragePreviewDrag());
		trigger.triggers.Add(end);
	}

	private void BeginGaragePreviewDrag(PointerEventData eventData)
	{
		garagePreviewDragging = true;
		garagePreviewResumeDelay = 0.5f;
	}

	private void DragGaragePreview(PointerEventData eventData)
	{
		if (eventData == null)
		{
			return;
		}
		garagePreviewDragging = true;
		garagePreviewResumeDelay = 0.5f;
		garagePreviewYaw = Mathf.Repeat(garagePreviewYaw - eventData.delta.x * 0.28f, 360f);
		ApplyGaragePreviewRotation();
	}

	private void EndGaragePreviewDrag()
	{
		garagePreviewDragging = false;
		garagePreviewResumeDelay = 0.5f;
	}

	private void MakeGaragePreviewWorld()
	{
		garagePreviewRoot = new GameObject("Garage Preview World").transform;
		garagePreviewRoot.position = new Vector3(4000f, 0f, 0f);
		garageVirtualFloor = new GameObject("Garage Invisible Perspective Floor").transform;
		garageVirtualFloor.SetParent(garagePreviewRoot);
		garageVirtualFloor.localPosition = Vector3.zero;
		garageVirtualFloor.localRotation = Quaternion.identity;
		garageVirtualFloor.localScale = Vector3.one;
		garagePreviewPivot = new GameObject("Garage Preview Pivot").transform;
		garagePreviewPivot.SetParent(garageVirtualFloor);
		garagePreviewPivot.localPosition = new Vector3(0f, 0.04f, -0.75f);
		garagePreviewPivot.localRotation = Quaternion.Euler(0f, -18f, 0f);
		garagePreviewPivot.localScale = Vector3.one;
		garagePreviewCarHolder = new GameObject("Garage Preview Car Holder").transform;
		garagePreviewCarHolder.SetParent(garagePreviewPivot);
		garagePreviewCarHolder.localPosition = new Vector3(0.25f, 0f, 0f);
		garagePreviewCarHolder.localRotation = Quaternion.identity;
		garagePreviewCarHolder.localScale = Vector3.one;
		RebuildGaragePreviewVehicle();
		Light underlight = new GameObject("Garage Preview Red Underlight").AddComponent<Light>();
		underlight.transform.SetParent(garagePreviewRoot);
		underlight.transform.localPosition = new Vector3(0f, 0.32f, -0.65f);
		underlight.type = LightType.Point;
		underlight.range = 8f;
		underlight.intensity = 3.5f;
		underlight.color = new Color(1f, 0.025f, 0.035f);
		Light ceiling = new GameObject("Garage Preview Ceiling Spot").AddComponent<Light>();
		ceiling.transform.SetParent(garagePreviewRoot);
		ceiling.transform.localPosition = new Vector3(-2.3f, 4f, -2.4f);
		ceiling.type = LightType.Spot;
		ceiling.range = 12f;
		ceiling.spotAngle = 56f; /* [user request 2026-09-20: brighter cars] */ // original: 45f
		ceiling.intensity = 6.4f; /* [user request 2026-09-20: brighter cars] */ // original: 4.8f
		ceiling.color = new Color(1f, 0.86f, 0.78f);
		ceiling.transform.LookAt(garagePreviewRoot.position + new Vector3(0f, 0.25f, -0.55f));
		Light rim = new GameObject("Garage Preview Rim Light").AddComponent<Light>();
		rim.transform.SetParent(garagePreviewRoot);
		rim.transform.localPosition = new Vector3(3.5f, 1.5f, 2.5f);
		rim.type = LightType.Point;
		rim.range = 10f;
		rim.intensity = 3.6f;
		rim.color = new Color(1f, 0.015f, 0.03f);
		Light wallNeon = new GameObject("Garage Right Wall Neon Light").AddComponent<Light>();
		wallNeon.transform.SetParent(garagePreviewRoot);
		wallNeon.transform.localPosition = new Vector3(4.2f, 1.8f, 1.5f);
		wallNeon.type = LightType.Point;
		wallNeon.range = 11f;
		wallNeon.intensity = 3.4f;
		wallNeon.color = new Color(1f, 0.02f, 0.04f);
		Light frontFill = new GameObject("Garage Preview Fill Light").AddComponent<Light>(); /* [user request 2026-09-20: brighter cars] */ // original: "Garage Preview Weak Front Fill"
		frontFill.transform.SetParent(garagePreviewRoot);
		frontFill.transform.localPosition = new Vector3(-3.2f, 2.1f, -3.2f); /* [user request 2026-09-20: brighter cars] */ // original: new Vector3(-3f, 1.5f, -5.5f)
		frontFill.type = LightType.Point;
		frontFill.range = 12f; /* [user request 2026-09-20: brighter cars] */ // original: 7f
		frontFill.intensity = 4.5f; /* [user request 2026-09-20: brighter cars] */ // original: 0.8f
		frontFill.color = new Color(0.78f, 0.86f, 1f); /* [user request 2026-09-20: brighter cars] */ // original: new Color(0.74f, 0.82f, 0.95f)
		frontFill.shadows = LightShadows.None; /* [user request 2026-09-20: brighter cars] */
		frontFill.renderMode = LightRenderMode.ForcePixel; /* [user request 2026-09-20: brighter cars] */
		/* [user request 2026-09-20: brighter cars] */ // Warm-white key light from the upper front on the camera side so paint colour and chrome read on the pad.
		Light keyLight = new GameObject("Garage Preview Key Light").AddComponent<Light>();
		keyLight.transform.SetParent(garagePreviewRoot);
		keyLight.transform.localPosition = new Vector3(2.1f, 3.1f, -3.7f);
		keyLight.type = LightType.Spot;
		keyLight.range = 16f;
		keyLight.spotAngle = 70f;
		keyLight.innerSpotAngle = 34f;
		keyLight.intensity = 9.5f;
		keyLight.color = new Color(1f, 0.94f, 0.86f);
		keyLight.shadows = LightShadows.None;
		keyLight.renderMode = LightRenderMode.ForcePixel;
		keyLight.transform.LookAt(garagePreviewRoot.position + new Vector3(0.25f, 0.45f, -0.75f));
		garagePreviewCamera = new GameObject("Garage Preview Camera").AddComponent<Camera>();
		garagePreviewCamera.transform.SetParent(garagePreviewRoot);
		garagePreviewCamera.transform.localPosition = new Vector3(3.35f, 1.2f, -5.8f);
		/* [user request 2026-09-21: cars] */ // original: new Vector3(0f, 0.55f, -0.25f) — aiming 0.15 lower raises the car ~70 px in the frame (frame half-height 2.05 m over 960 px at the car's 6 m),
		// so it sits centred in the 630..1304 band between the action row / tuning card and the card carousel instead of 32 px above the action row
		garagePreviewCamera.transform.LookAt(garagePreviewRoot.position + new Vector3(0f, 0.40f, -0.25f));
		garagePreviewCamera.clearFlags = CameraClearFlags.Color;
		garagePreviewCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		garagePreviewCamera.cullingMask = 536870912;
		garagePreviewCamera.fieldOfView = 38f;
		garagePreviewCamera.nearClipPlane = 0.03f;
		garagePreviewCamera.farClipPlane = 80f;
		garagePreviewCamera.targetTexture = garagePreviewTexture;
		garagePreviewCamera.allowHDR = false;
		garagePreviewCamera.allowMSAA = false;
		garagePreviewCamera.enabled = false;
		garagePreviewRoot.gameObject.SetActive(false);
		Debug.Log(string.Format("[GARAGE] Camera configured - cullingMask set to layer {0}, targetTexture: {1}", 29, garagePreviewTexture.name));
	}

	private void BuildImportedGarageScene()
	{
		GameObject prefab = Resources.Load<GameObject>("garage_scene");
		if (prefab == null)
		{
			Debug.LogError("[GARAGE] Failed to load garage_scene.fbx from Resources/garage_scene");
			return;
		}
		GameObject scene = UnityEngine.Object.Instantiate(prefab);
		scene.name = "Imported Garage Scene";
		scene.SetActive(true);
		scene.transform.SetParent(garagePreviewRoot, false);
		scene.transform.localPosition = Vector3.zero;
		scene.transform.localRotation = Quaternion.identity;
		scene.transform.localScale = Vector3.one;
		Camera[] cameras = scene.GetComponentsInChildren<Camera>(true);
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].enabled = false;
			cameras[i].gameObject.SetActive(false);
		}
		NormalizeGarageScene(scene);
		StyleImportedGarageScene(scene);
		SetLayerRecursively(scene, 29);
		garageSceneRoot = scene.transform;
		Debug.Log("[GARAGE] Imported garage scene successfully loaded");
	}

	private void NormalizeGarageScene(GameObject scene)
	{
		Renderer[] renderers = scene.GetComponentsInChildren<Renderer>(true);
		if (renderers.Length == 0)
		{
			Debug.LogWarning("[GARAGE] No renderers found in garage scene");
			return;
		}
		Bounds bounds = renderers[0].bounds;
		for (int i = 1; i < renderers.Length; i++)
		{
			bounds.Encapsulate(renderers[i].bounds);
		}
		float footprint = Mathf.Max(bounds.size.x, bounds.size.z);
		float scale = footprint > 0.001f ? 8.5f / footprint : 1f;
		scene.transform.localScale = Vector3.one * scale;
		bounds = renderers[0].bounds;
		for (int i = 1; i < renderers.Length; i++)
		{
			bounds.Encapsulate(renderers[i].bounds);
		}
		Vector3 center = garagePreviewRoot.InverseTransformPoint(bounds.center);
		Vector3 min = garagePreviewRoot.InverseTransformPoint(bounds.min);
		Transform sceneTransform = scene.transform;
		sceneTransform.localPosition = sceneTransform.localPosition + new Vector3(-center.x, -min.y, -center.z + 1.6f);
		Debug.Log(string.Format("[GARAGE] Scene normalized - footprint: {0:F2}, scale: {1:F2}, bounds: {2}", footprint, scale, bounds.size));
	}

	private void StyleImportedGarageScene(GameObject scene)
	{
		Renderer[] renderers = scene.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			renderer.enabled = true;
			string lowerName = renderer.gameObject.name.ToLowerInvariant();
			Bounds bounds = renderer.bounds;
			bool isCeiling = bounds.center.y > 2.2f && Mathf.Max(bounds.size.x, bounds.size.z) > 4.5f;
			if (isCeiling || lowerName.Contains("roof") || lowerName.Contains("ceiling"))
			{
				renderer.enabled = false;
				continue;
			}
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			Material[] materials = renderer.sharedMaterials;
			for (int j = 0; j < materials.Length; j++)
			{
				Material material = materials[j];
				if (material == null)
				{
					continue;
				}
				if (material.HasProperty("_BaseColor"))
				{
					Color baseColor = material.GetColor("_BaseColor");
					material.SetColor("_BaseColor", Color.Lerp(baseColor, new Color(0.04f, 0.02f, 0.025f, baseColor.a), 0.08f));
				}
				else if (material.HasProperty("_Color"))
				{
					material.color = Color.Lerp(material.color, new Color(0.04f, 0.02f, 0.025f, material.color.a), 0.08f);
				}
			}
		}
		Collider[] colliders = scene.GetComponentsInChildren<Collider>(true);
		for (int i = 0; i < colliders.Length; i++)
		{
			UnityEngine.Object.Destroy(colliders[i]);
		}
	}

	private void FitGarageVehicle(GameObject previewCar, VehicleDefinition definition)
	{
		if (previewCar == null || definition == null)
		{
			return;
		}
		Transform offset = new GameObject("Garage Sportcar Offset").transform;
		offset.SetParent(garagePreviewCarHolder != null ? garagePreviewCarHolder : garagePreviewPivot);
		offset.localPosition = Vector3.zero;
		offset.localRotation = Quaternion.identity;
		offset.localScale = Vector3.one;
		previewCar.transform.SetParent(offset, false);
		previewCar.transform.localPosition = Vector3.zero;
		previewCar.transform.localRotation = Quaternion.Euler(0f, definition.ModelYawOffset, 0f);
		previewCar.transform.localScale = Vector3.one;
		if (!TryGetActiveRendererBounds(previewCar.transform, out Bounds bounds, definition.BoundsRendererNameKeywords))
		{
			return;
		}
		Vector3 center = offset.InverseTransformPoint(bounds.center);
		Vector3 min = offset.InverseTransformPoint(bounds.min);
		float footprint = Mathf.Max(bounds.size.x, bounds.size.z);
		float scale = footprint > 0.001f ? Mathf.Min(definition.GarageTargetFootprint / footprint, definition.GarageMaximumScale) : 1f;
		previewCar.transform.localScale = Vector3.one * scale;
		previewCar.transform.localPosition = new Vector3(-center.x * scale + definition.GarageHorizontalOffset, -min.y * scale + definition.GarageRoadYOffset, -center.z * scale);
	}

	private void StyleVehicleForGarage(GameObject previewCar)
	{
		Renderer[] renderers = previewCar.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			string lowerName = renderer.gameObject.name.ToLowerInvariant();
			if (lowerName.Contains("ground") || lowerName.Contains("floor") || lowerName.Contains("plane") || lowerName.Contains("shadow"))
			{
				renderer.enabled = false;
				continue;
			}
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			Material[] materials = renderer.sharedMaterials;
			for (int j = 0; j < materials.Length; j++)
			{
				Material material = materials[j];
				if (material == null)
				{
					continue;
				}
				material.color = Color.Lerp(material.color, new Color(0.015f, 0.012f, 0.014f, material.color.a), 0.08f); /* [user request 2026-09-20: brighter cars] */ // original: 0.18f
				material.EnableKeyword("_EMISSION");
				material.SetColor("_EmissionColor", Color.black);
				material.renderQueue = 2000;
				material.SetFloat("_Surface", 0f);
				material.SetFloat("_AlphaClip", 0f);
			}
		}
		Collider[] colliders = previewCar.GetComponentsInChildren<Collider>(true);
		for (int i = 0; i < colliders.Length; i++)
		{
			UnityEngine.Object.Destroy(colliders[i]);
		}
	}

	private void SetGaragePreviewVisible(bool visible)
	{
		if (visible && garagePreviewRoot != null)
		{
			SetLayerRecursively(garagePreviewRoot.gameObject, 29);
		}
		if (garagePreviewRoot != null)
		{
			garagePreviewRoot.gameObject.SetActive(visible);
		}
		if (garagePreviewCamera != null)
		{
			garagePreviewCamera.enabled = visible;
		}
		if (visible)
		{
			Debug.Log(string.Format("[GARAGE] Preview camera enabled - pos: {0}, cullingMask layer: {1}", garagePreviewCamera != null ? garagePreviewCamera.transform.position : Vector3.zero, 29));
		}
	}

	private void UpdateGaragePreview(float dt)
	{
		if (garageRoot == null || !garageRoot.activeInHierarchy || garagePreviewPivot == null)
		{
			return;
		}
		if (garagePreviewCamera == null || !garagePreviewCamera.enabled)
		{
			return;
		}
		if (garagePreviewCarHolder != null)
		{
			float frameDt = Mathf.Min(Time.unscaledDeltaTime, 0.0333333351f);
			if (!garagePreviewDragging)
			{
				if (garagePreviewResumeDelay > 0f)
				{
					garagePreviewResumeDelay = Mathf.Max(0f, garagePreviewResumeDelay - frameDt);
				}
				else
				{
					garagePreviewYaw = Mathf.Repeat(garagePreviewYaw + 18f * frameDt, 360f);
				}
			}
			ApplyGaragePreviewRotation();
		}
	}

	private void ApplyGaragePreviewRotation()
	{
		if (garagePreviewCarHolder == null)
		{
			return;
		}
		garagePreviewCarHolder.localRotation = Quaternion.Euler(0f, garagePreviewYaw, 0f);
		garagePreviewCarHolder.localPosition = new Vector3(0.25f, 0f, 0f);
	}

	private void ShowGarageScreen()
	{
		if (garageRoot == null)
		{
			return;
		}
		overlayTitle.text = "";
		overlayTag.text = "";
		overlayStats.text = "";
		SelectMenuTab(0, false);
		RefreshGarageCardIcons(); /* [user request 2026-09-21: cars] */
		UpdateGarageUi();
	}

	private void HideGarageScreen()
	{
		SelectMenuTab(1, false);
	}

	private void UpdateGarageUi()
	{
		if (garageVehicleCards != null && garageVehicleCardButtons != null)
		{
			if (garageSelectedVehicleCardIndex < 0 || garageSelectedVehicleCardIndex >= garageVehicleCards.Length)
			{
				garageSelectedVehicleCardIndex = GetGarageVehicleCardIndexForVehicle(ActiveVehicle.Id);
			}
			UpdateGarageVehicleCardVisuals();
			return;
		}
		bool sportSelected = string.Equals(ActiveVehicle.Id, "sportcar", StringComparison.OrdinalIgnoreCase);
		bool flyingSelected = string.Equals(ActiveVehicle.Id, "flying_car", StringComparison.OrdinalIgnoreCase);
		SetGarageVehicleButtonVisual(garageSportCarButton, garageSportCarLabel, sportSelected);
		SetGarageVehicleButtonVisual(garageFlyingCarButton, garageFlyingCarLabel, flyingSelected);
	}

	private void UpdateGarageVehicleCardVisuals()
	{
		if (garageVehicleCards == null)
		{
			return;
		}
		int unlocked = 0;
		for (int i = 0; i < garageVehicleCards.Length; i++)
		{
			GarageVehicleCardData card = garageVehicleCards[i];
			card.VisualLocked = !SupabaseGameManager.IsVehicleUnlocked(card.VehicleId);
			if (!card.VisualLocked)
			{
				unlocked++;
			}
		}
		if (garageUnlockedCountText != null)
		{
			garageUnlockedCountText.text = string.Format("{0} OF {1} CARS UNLOCKED", unlocked, garageVehicleCards.Length);
		}
		for (int i = 0; i < garageVehicleCards.Length; i++)
		{
			GarageVehicleCardData card = garageVehicleCards[i];
			bool selected = i == garageSelectedVehicleCardIndex;
			Color tint = card.Color;
			if (garageVehicleCardBorders != null && i < garageVehicleCardBorders.Length && garageVehicleCardBorders[i] != null)
			{
				garageVehicleCardBorders[i].color = selected
					? new Color(tint.r, tint.g, tint.b, card.VisualLocked ? 0.38f : 0.76f)
					: (card.VisualLocked ? new Color(1f, 1f, 1f, 0.055f) : new Color(tint.r, tint.g, tint.b, 0.28f));
			}
			if (garageVehicleCardFills != null && i < garageVehicleCardFills.Length && garageVehicleCardFills[i] != null)
			{
				garageVehicleCardFills[i].color = selected ? new Color(0.09f, 0.064f, 0.145f, 0.98f) : new Color(0.05f, 0.04f, 0.09f, 0.96f);
			}
			if (garageVehicleCardDots != null && i < garageVehicleCardDots.Length && garageVehicleCardDots[i] != null)
			{
				garageVehicleCardDots[i].rectTransform.sizeDelta = selected ? new Vector2(28f, 8f) : new Vector2(8f, 8f);
				garageVehicleCardDots[i].sprite = selected ? roundedGlassSprite : softCircleSprite;
				garageVehicleCardDots[i].type = selected ? Image.Type.Sliced : Image.Type.Simple;
				garageVehicleCardDots[i].color = selected ? new Color(tint.r, tint.g, tint.b, 0.95f) : new Color(1f, 1f, 1f, 0.16f);
			}
			if (garageVehicleLockOverlays != null && i < garageVehicleLockOverlays.Length && garageVehicleLockOverlays[i] != null)
			{
				garageVehicleLockOverlays[i].gameObject.SetActive(card.VisualLocked);
			}
			if (garageVehicleActionLabels != null && i < garageVehicleActionLabels.Length && garageVehicleActionLabels[i] != null)
			{
				garageVehicleActionLabels[i].text = "";
				garageVehicleActionLabels[i].color = Color.clear;
			}
			/* [user request 2026-09-21: cars] */ // price row ("◆ 650" / OWNED / SELECTED) and the three mini bars with the upgrade levels
			bool active = string.Equals(ActiveVehicle.Id, card.VehicleId, StringComparison.OrdinalIgnoreCase);
			if (garageVehicleCardPriceTexts != null && i < garageVehicleCardPriceTexts.Length && garageVehicleCardPriceTexts[i] != null)
			{
				Text priceRow = garageVehicleCardPriceTexts[i];
				int price = InfernoCars.Price(card.VehicleId);
				priceRow.text = active ? "SELECTED" : (!card.VisualLocked ? "OWNED" : (price >= 0 ? "◆ " + price : "◆ —"));
				priceRow.color = active ? InfernoLobby.Hex(InfernoLobby.Blossom) : (!card.VisualLocked ? InfernoLobby.Hex(InfernoLobby.Lilac) : card.Color);   // a locked car's price in its rarity colour /* [user request 2026-09-22: cars] */
			}
			if (garageVehicleCardStatFills != null && i < garageVehicleCardStatFills.Length && garageVehicleCardStatFills[i] != null)
			{
				card.Power = InfernoCars.Stat(card.VehicleId, InfernoUpgrades.Stat.Engine);
				card.Handling = InfernoCars.Stat(card.VehicleId, InfernoUpgrades.Stat.Handling);
				card.Speed = InfernoCars.Stat(card.VehicleId, InfernoUpgrades.Stat.Nitro);
				float[] amounts = { card.Power, card.Handling, card.Speed };
				for (int s = 0; s < garageVehicleCardStatFills[i].Length && s < amounts.Length; s++)
				{
					if (garageVehicleCardStatFills[i][s] != null)
					{
						garageVehicleCardStatFills[i][s].rectTransform.sizeDelta = new Vector2(Mathf.Clamp01(amounts[s]) * GarageMiniBarWidth, GarageMiniBarHeight);   /* [user request 2026-09-27: card data] */ // original: 5f
					}
				}
			}
		}
		bool showPurchase = false;
		if (garageSelectedVehicleCardIndex >= 0 && garageSelectedVehicleCardIndex < garageVehicleCards.Length)
		{
			string vehicleId = garageVehicleCards[garageSelectedVehicleCardIndex].VehicleId;
			showPurchase = !string.Equals(ActiveVehicle.Id, vehicleId, StringComparison.OrdinalIgnoreCase) && !SupabaseGameManager.IsVehicleUnlocked(vehicleId);
		}
		if (garagePurchaseActionRoot != null)
		{
			garagePurchaseActionRoot.gameObject.SetActive(showPurchase);
		}
		if (garagePurchaseButton != null)
		{
			garagePurchaseButton.interactable = showPurchase && !garagePurchaseBusy;
		}
		if (showPurchase && garagePurchaseButtonText != null)
		{
			int crystals = SupabaseGameManager.Crystals;
			/* [user request 2026-09-21: cars] */ // original: crystals >= 100 ? "Purchase for 100" : "Need {100 - crystals} more to purchase"
			int price = InfernoCars.Price(garageVehicleCards[garageSelectedVehicleCardIndex].VehicleId);
			garagePurchaseButtonText.text = garagePurchaseBusy ? "Syncing..." : (price < 0 ? "NOT FOR SALE" : (crystals >= price ? string.Format("PURCHASE · {0} ◆", price) : string.Format("NEED {0} MORE", price - crystals)));
		}
		RefreshGarageTuning(); /* [user request 2026-09-21: cars] */
	}

	private void SetGarageVehicleButtonVisual(Button button, Text label, bool selected)
	{
		if (button == null || label == null)
		{
			return;
		}
		Image background = button.targetGraphic as Image;
		if (background != null)
		{
			background.color = selected ? new Color(0.38f, 0.12f, 0.62f, 0.82f) : new Color(0.045f, 0.04f, 0.08f, 0.82f);
		}
		label.color = selected ? new Color(1f, 0.72f, 0.94f, 1f) : new Color(0.56f, 0.57f, 0.69f, 1f);
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1f, 1f, 1f, 0.9f);
		colors.pressedColor = new Color(0.8f, 0.8f, 0.86f, 1f);
		colors.selectedColor = colors.highlightedColor;
		button.colors = colors;
	}

	private void RebuildGaragePreviewVehicle()
	{
		if (garagePreviewVehicleRoot != null)
		{
			Destroy(garagePreviewVehicleRoot);
		}
		VehicleDefinition definition = garagePreviewVehicleDefinition ?? ActiveVehicle;
		GameObject previewCar = CreateConfiguredVehicleModel(garagePreviewCarHolder, definition.GarageModelName, definition);
		if (previewCar == null)
		{
			return;
		}
		previewCar.SetActive(true);
		previewCar.transform.localPosition = Vector3.zero;
		previewCar.transform.localRotation = Quaternion.identity;
		previewCar.transform.localScale = Vector3.one;
		FitGarageVehicle(previewCar, definition);
		InfernoStyle.Dress(this, previewCar, definition, true); /* [InfernoLobby patch] */
		ConfigureVehicleHover(previewCar, true, definition);
		StyleVehicleForGarage(previewCar);
		garagePreviewVehicleRoot = previewCar.transform.parent.gameObject;
		SetLayerRecursively(garagePreviewVehicleRoot, 29);
	}

	private void PreviewGarageVehicle(string vehicleId)
	{
		if (string.IsNullOrWhiteSpace(vehicleId))
		{
			return;
		}
		if (garagePreviewVehicleRoot != null && garagePreviewVehicleDefinition != null && string.Equals(garagePreviewVehicleDefinition.Id, vehicleId, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		VehicleDefinition definition = CreateVehicleDefinition(vehicleId);
		if (definition == null || Resources.Load<GameObject>(definition.ModelResource) == null)
		{
			return;
		}
		garagePreviewVehicleDefinition = definition;
		if (garagePreviewCarHolder != null)
		{
			RebuildGaragePreviewVehicle();
		}
	}
}
