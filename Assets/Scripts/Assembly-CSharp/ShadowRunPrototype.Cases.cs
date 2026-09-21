using System;
using System.Collections.Generic;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Translated from Il2CppOutput Assembly-CSharp.cpp / Assembly-CSharp__1.cpp:
//   ShadowRunPrototype_MakeCaseOverlay_mFFAD2FF24196574F3A3845E0AF10B33E2D905CD8
//   ShadowRunPrototype_MakeCaseOpeningPreviewLayer_m4237504BBDD5103004F44592714486FC4C4961CF
//   ShadowRunPrototype_MakeCaseDisplay_m0371333F22DEE670146151F20F722E6944E41E13
//   ShadowRunPrototype_MakeCaseBottomArea_m7D10BE3A37A4084973C0ABF61CA959608B2E7964
//   ShadowRunPrototype_MakeCaseRoulette_mE31F14D91132087BC4E27D8EBF7A8FFED356220F
//   ShadowRunPrototype_MakeCaseResultPanel_mB40F82EE91D70256C04869A53F2823ACC62025FB
//   ShadowRunPrototype_MakeCornerAccent_m97E7785B57DA8DE55FF3D1C140ADAFA210E8D5FD
//   ShadowRunPrototype_ShowCaseResultPanel_mCCE64E53C443B205C8B1F77BF2428045FAD5D1E3
//   ShadowRunPrototype_UpdateCaseResultVisual_m0480D71BECFBF537175F48205620A36463A0B70B
//   ShadowRunPrototype_CollectCaseReward_m44D40A5BDBD8700A283B4A9601A4F8D2183252F6
//   ShadowRunPrototype_ApplyCaseRewardOnce_mFBF873D135C7E38844A8BFBECD9BAD79BD67045B
//   ShadowRunPrototype_SpinCaseAgain_m71CE33FB3FFC4149D3DC0752527F6BF033090A31
//   ShadowRunPrototype_RequestCaseRewardedOpening_m55E3EB5E1E5014EB277E425DDA170F94562A361D
//   ShadowRunPrototype_RequestCaseRewardedOpening_m733EE872618D246878C74FF1F2380DEF163F405E
//   U3CU3Ec__DisplayClass391_0_U3CRequestCaseRewardedOpeningU3Eb__0 / b__1
//   ShadowRunPrototype_CaseOpeningRequestIsCurrent_mABBD0CA13AFFE8B4BB3EA028071965335D6EA2B8
//   ShadowRunPrototype_SetCaseAdUnavailable_m131E8E732E500781CAD5D4101A4B7B3E12F645C0
//   ShadowRunPrototype_EnsureCasePreviewWorld_m840C6C9516318468D3D125EB7E88D04B10A7C780
//   ShadowRunPrototype_NormalizeCasePreviewModel_m276B9C1F2350E78510084279B82FCAEACAEC9A35
//   ShadowRunPrototype_AlignCaseHeadToClosedBodyPose_m50F10E234DCA5D683F39D7CA7AD860A979DE9E4A
//   ShadowRunPrototype_StyleCasePreviewModel_m9A3901F55F23B70BCE51BF8798EB5475056A8ADB
//   ShadowRunPrototype_SetCasePreviewVisible_m664CF3FAFBA4F399578E411DBA12C754A6AFF08A
//   ShadowRunPrototype_AddCasePreviewDragEvents_m71FA6C3A293F41E643D07EE15FCD178047B71A66 (+b__401_0)
//   ShadowRunPrototype_DragCasePreview_m8CDDB3D2D0B09D872A4DC3FDC056AF1ACDE72B14
//   ShadowRunPrototype_ApplyCasePreviewPose_m3B660A1C4BD4EEB24369AA201AF0EA888FA4AB03
//   ShadowRunPrototype_ResetCasePreviewPose_mB1C9D995360EC21E58E3177357FDB351BFBBCB74
//   ShadowRunPrototype_StartCaseOpening_m60F429AFE6CB71A756EF62B47CE7D72A45EF72D2
//   ShadowRunPrototype_BeginCaseRoulette_mB1ACAA8388DCB5B3ECCCAAAC8C9761217132D11A
//   ShadowRunPrototype_RollCasePrize_m16366C1161B2DA2971912726020EE7DE573B9620
//   ShadowRunPrototype_GetLockedCaseVehicles_mFA812555DACA7AEF9C0AA997B665A82CF2919ECD
//   ShadowRunPrototype_RandomRouletteDisplayPrize_mAC79B3AAEDDBA9B2EB97812DF5CC3FB1B060634F
//   ShadowRunPrototype_CaseVehicleDisplayName_m6115D4C1C0B05548C83A6D7DC13DCC72A83C0906
//   ShadowRunPrototype_CaseVehicleImageResource_m8C14096ED78F58DFC5EEB0359DADB3D3253E6103
//   ShadowRunPrototype_CaseDiamondImageResource_mEF8A5A0B3D657BB1B7E10124E5C1DD2B877AD907
//   ShadowRunPrototype_CaseRoulettePrizeArtSize_m545E5802DFFA491F59B93E4974B67F40294C915F
//   ShadowRunPrototype_CaseResultPrizeArtSize_m0C4B0D4C0880363F33910C60617A4F854F8D2016
//   ShadowRunPrototype_CasePrizeColor_mC66DD1CCAC53A353E9CEAA13FC66A246B7900702
//   ShadowRunPrototype_CasePrizeRarity_m002A5C6CE36C403B885AE3E61F865B9C626DB567
//   ShadowRunPrototype_BuildCaseRouletteItems_mE3BC06A8ECC0A36D98CC68338C67A2B716781961
//   ShadowRunPrototype_MakeCasePrizeCard_mEFA5148861512B69B6D5423F188DE1C06B4D7E62
//   ShadowRunPrototype_UpdateCaseRouletteFocus_m1306F5CD6F085DC368BDF3DD637900DA0350CBB4
//   ShadowRunPrototype_UpdateCaseRouletteChrome_mCFBB57F4A2ED846B1A847EF360F6C3ED5F385235
//   ShadowRunPrototype_UpdateCaseOpening_mFE61A0C3B9203C59D6A552CFBAC1C0D993644FF8
//   ShadowRunPrototype_HermitePosition_m814C5773C3F85888BB9A246E1F5C08DA8A8BE61B
//   ShadowRunPrototype_UpdateCaseOpenAnimation_m4CE86675D83664FCA1FB7A150D97F7A1B5DDA876
//   ShadowRunPrototype_FinishCaseOpenAnimationAndStartRoulette_m6C40C9B918005A92081FECE883576D65A37D631E
//   ShadowRunPrototype_CompleteCaseOpening_m9F02FE010661F0A27E64FAB7BE5AA0D07394FC51
//   ShadowRunPrototype_ResetCaseOpening_m2E0CD37E6845C0718B880B0C28D2F0ED1D38C816
//   ShadowRunPrototype_PlayCaseSequenceAudio_m99B5BFEF794780B512977B754CDDE68050CE7442
//   ShadowRunPrototype_StopCaseSequenceAudio_m77203FD0382A5F26B90F179245B158874951F4D8
//   ShadowRunPrototype_ShowCaseOverlay_mD96ECB92824763E4E65E4E4B429536A6C677CB63
//   ShadowRunPrototype_HideCaseOverlay_m4608675945C5D0A0C494E75371770E01D933890E
//   ShadowRunPrototype_MakeCaseActionButton_mA82AB7295712C09F5DDA824694F0E4A708777C38
public sealed partial class ShadowRunPrototype
{
	private void MakeCaseOverlay(Transform parent)
	{
		caseOverlayRoot = new GameObject("Case Overlay");
		caseOverlayRoot.transform.SetParent(parent, false);
		RectTransform rect = caseOverlayRoot.AddComponent<RectTransform>();
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		Image dim = caseOverlayRoot.AddComponent<Image>();
		dim.color = new Color(0.01f, 0.006f, 0.025f, 0.72f);
		Button dimButton = caseOverlayRoot.AddComponent<Button>();
		dimButton.targetGraphic = dim;
		dimButton.onClick.AddListener(HideCaseOverlay);
		caseOpeningWindowRoot = new GameObject("CaseOpeningWindow").AddComponent<RectTransform>();
		caseOpeningWindowRoot.SetParent(caseOverlayRoot.transform, false);
		caseOpeningWindowRoot.anchorMin = new Vector2(0f, 0.5f);
		caseOpeningWindowRoot.anchorMax = new Vector2(1f, 0.5f);
		caseOpeningWindowRoot.pivot = new Vector2(0.5f, 0.5f);
		caseOpeningWindowRoot.anchoredPosition = Vector2.zero;
		caseOpeningWindowRoot.sizeDelta = new Vector2(0f, 1460f);
		Image window = caseOpeningWindowRoot.gameObject.AddComponent<Image>();
		window.color = new Color(0.031f, 0.02f, 0.075f, 0.985f);
		window.sprite = roundedGlassSprite;
		window.type = Image.Type.Sliced;
		caseOpeningWindowRoot.gameObject.AddComponent<Button>().targetGraphic = window;
		Button close = MakeIconButton(caseOpeningWindowRoot, "Close Case", new Vector2(34f, -34f), new Vector2(56f, 56f), TextAnchor.UpperLeft, "×", 34);
		close.onClick.AddListener(HideCaseOverlay);
		Text title = MakeText(caseOpeningWindowRoot, "CASE OPENING", 24, TextAnchor.UpperCenter, new Vector2(0f, -52f));
		title.fontStyle = FontStyle.Bold;
		title.color = new Color(0.75f, 0.4f, 1f, 1f);
		title.raycastTarget = false;
		title.GetComponent<RectTransform>().sizeDelta = new Vector2(560f, 42f);
		MakeCaseDisplay(caseOpeningWindowRoot);
		MakeCaseRoulette(caseOpeningWindowRoot);
		MakeCaseBottomArea(caseOpeningWindowRoot);
		MakeCaseResultPanel(caseOpeningWindowRoot);
		MakeCaseOpeningPreviewLayer(caseOpeningWindowRoot);
		InfernoDailyCases.BindOverlay(this);   /* [user request 2026-09-21] "N OF 3 CASES LEFT TODAY" line */
		caseOverlayRoot.SetActive(false);
	}

	private void MakeCaseOpeningPreviewLayer(Transform parent)
	{
		caseOpeningPreviewRoot = new GameObject("3D Opening Animation Layer").AddComponent<RectTransform>();
		caseOpeningPreviewRoot.SetParent(parent, false);
		ApplyAnchor(caseOpeningPreviewRoot, TextAnchor.MiddleCenter);
		caseOpeningPreviewRoot.anchoredPosition = new Vector2(0f, 60f);
		caseOpeningPreviewRoot.sizeDelta = new Vector2(880f, 640f);
		CanvasGroup group = caseOpeningPreviewRoot.gameObject.AddComponent<CanvasGroup>();
		group.alpha = 1f;
		group.blocksRaycasts = true;
		group.interactable = true;
		Image glow = MakeUiBlock(caseOpeningPreviewRoot, "Opening Animation Purple Glow", Vector2.zero, new Vector2(880f, 560f), TextAnchor.MiddleCenter, new Color(0.659f, 0.333f, 0.969f, 0.42f));
		glow.sprite = caseRadialGlowSprite;
		glow.raycastTarget = false;
		casePreviewImage = new GameObject("Case 3D Preview").AddComponent<RawImage>();
		casePreviewImage.transform.SetParent(caseOpeningPreviewRoot, false);
		casePreviewImage.color = Color.white;
		casePreviewImage.raycastTarget = false;
		RectTransform previewRect = casePreviewImage.rectTransform;
		ApplyAnchor(previewRect, TextAnchor.MiddleCenter);
		previewRect.anchoredPosition = Vector2.zero;
		previewRect.sizeDelta = new Vector2(880f, 560f);
		Image dragSurface = MakeUiBlock(caseOpeningPreviewRoot, "Case Preview Drag Surface", Vector2.zero, new Vector2(880f, 500f), TextAnchor.MiddleCenter, new Color(1f, 1f, 1f, 0f));
		dragSurface.raycastTarget = true;
		AddCasePreviewDragEvents(dragSurface.gameObject);
		caseOpeningPreviewRoot.gameObject.SetActive(false);
	}

	private void MakeCaseDisplay(Transform parent)
	{
		caseClosedRoot = new GameObject("CaseDisplay").AddComponent<RectTransform>();
		caseClosedRoot.SetParent(parent, false);
		ApplyAnchor(caseClosedRoot, TextAnchor.UpperCenter);
		caseClosedRoot.anchoredPosition = new Vector2(0f, -105f);
		caseClosedRoot.sizeDelta = new Vector2(900f, 280f);
		caseClosedGroup = caseClosedRoot.gameObject.AddComponent<CanvasGroup>();
		caseClosedGroup.alpha = 1f;
		RectTransform fallback = new GameObject("Static Case Fallback Visual").AddComponent<RectTransform>();
		fallback.SetParent(caseClosedRoot, false);
		ApplyAnchor(fallback, TextAnchor.UpperCenter);
		fallback.anchoredPosition = new Vector2(0f, -18f);
		fallback.sizeDelta = new Vector2(860f, 148f);
		caseClosedStaticVisualRoot = fallback.gameObject;
		Image glow = MakeUiBlock(fallback, "CaseGlow radial purple blur", new Vector2(0f, -48f), new Vector2(520f, 300f), TextAnchor.UpperCenter, new Color(0.659f, 0.333f, 0.969f, 0.4f));
		glow.sprite = caseRadialGlowSprite;
		glow.raycastTarget = false;
		Image caseImage = MakeResourceImage(fallback, "CaseImage object-fit contain", "case", new Vector2(0f, -36f), new Vector2(180f, 132f), TextAnchor.UpperCenter, true);
		if (caseImage != null)
		{
			caseImage.raycastTarget = false;
			FitImageAspect(caseImage, new Vector2(180f, 132f));
		}
		Text title = MakeText(caseClosedRoot, "N SERIES CASE", 42, TextAnchor.UpperCenter, new Vector2(0f, -154f));
		title.fontStyle = FontStyle.Bold;
		title.color = Color.white;
		title.raycastTarget = false;
		title.GetComponent<RectTransform>().sizeDelta = new Vector2(760f, 58f);
		Text dropsLabel = MakeText(caseClosedRoot, "CRYSTAL DROPS", 15, TextAnchor.UpperCenter, new Vector2(-160f, -214f));
		dropsLabel.color = new Color(0.55f, 0.42f, 0.72f, 0.54f);
		dropsLabel.raycastTarget = false;
		dropsLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(220f, 26f);
		Text dropsValue = MakeText(caseClosedRoot, "2  ·  3  ·  5  ·  10", 20, TextAnchor.UpperCenter, new Vector2(-160f, -242f));
		dropsValue.fontStyle = FontStyle.Bold;
		dropsValue.color = new Color(0.75f, 0.4f, 1f, 1f);
		dropsValue.raycastTarget = false;
		dropsValue.GetComponent<RectTransform>().sizeDelta = new Vector2(220f, 32f);
		Text jackpotLabel = MakeText(caseClosedRoot, "VEHICLE JACKPOT", 15, TextAnchor.UpperCenter, new Vector2(160f, -214f));
		jackpotLabel.color = new Color(0.55f, 0.42f, 0.72f, 0.54f);
		jackpotLabel.raycastTarget = false;
		jackpotLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(220f, 26f);
		Text jackpotValue = MakeText(caseClosedRoot, "6% CHANCE", 20, TextAnchor.UpperCenter, new Vector2(160f, -242f));
		jackpotValue.fontStyle = FontStyle.Bold;
		jackpotValue.color = new Color(0.75f, 0.4f, 1f, 1f);
		jackpotValue.raycastTarget = false;
		jackpotValue.GetComponent<RectTransform>().sizeDelta = new Vector2(220f, 32f);
	}

	private void MakeCaseBottomArea(Transform parent)
	{
		caseBottomAreaRoot = new GameObject("BottomArea").AddComponent<RectTransform>();
		caseBottomAreaRoot.SetParent(parent, false);
		ApplyAnchor(caseBottomAreaRoot, TextAnchor.LowerCenter);
		caseBottomAreaRoot.anchoredPosition = new Vector2(0f, 118f);
		caseBottomAreaRoot.sizeDelta = new Vector2(920f, 250f);
		caseOpenButton = MakeProfileActionButton(caseBottomAreaRoot, "WATCH AD + OPEN CASE", new Vector2(0f, 88f), new Vector2(540f, 92f), true, false, out caseOpenButtonLabel, true, false);
		caseOpenButton.onClick.AddListener(RequestCaseRewardedOpening);
		Transform glow = caseBottomAreaRoot.Find("WATCH AD + OPEN CASE Neon Glow");
		caseOpenButtonGlow = glow != null ? glow.gameObject : null;
		caseCostRowRoot = null;
		caseOpeningStatusRoot = new GameObject("OPENING status with dots").AddComponent<RectTransform>();
		caseOpeningStatusRoot.SetParent(caseBottomAreaRoot, false);
		ApplyAnchor(caseOpeningStatusRoot, TextAnchor.MiddleCenter);
		caseOpeningStatusRoot.anchoredPosition = new Vector2(0f, 80f);
		caseOpeningStatusRoot.sizeDelta = new Vector2(360f, 70f);
		Image contour = MakeUiBlock(caseOpeningStatusRoot, "Opening Status Neon Contour", Vector2.zero, caseOpeningStatusRoot.sizeDelta, TextAnchor.MiddleCenter, new Color(0.76f, 0.2f, 1f, 1f));
		contour.sprite = roundedGlassSprite;
		contour.type = Image.Type.Sliced;
		contour.raycastTarget = false;
		Image fill = MakeUiBlock(caseOpeningStatusRoot, "Opening Status Dark Fill", Vector2.zero, caseOpeningStatusRoot.sizeDelta - new Vector2(8f, 8f), TextAnchor.MiddleCenter, new Color(0.06f, 0.014f, 0.105f, 1f));
		fill.sprite = roundedGlassSprite;
		fill.type = Image.Type.Sliced;
		fill.raycastTarget = false;
		caseStatusLabel = MakeText(caseOpeningStatusRoot, "OPENING...", 24, TextAnchor.MiddleCenter, new Vector2(0f, 16f));
		caseStatusLabel.fontStyle = FontStyle.Bold;
		caseStatusLabel.color = Color.white;
		caseStatusLabel.raycastTarget = false;
		caseStatusLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 32f);
		caseOpeningDots = new Image[3];
		for (int i = 0; i < caseOpeningDots.Length; i++)
		{
			Image dot = MakeUiBlock(caseOpeningStatusRoot, "Opening Dot " + i.ToString(), new Vector2((i - 1) * 18f, -18f), new Vector2(8f, 8f), TextAnchor.MiddleCenter, new Color(0.75f, 0.4f, 1f, 1f));
			dot.sprite = softCircleSprite;
			dot.raycastTarget = false;
			caseOpeningDots[i] = dot;
		}
		caseOpeningStatusRoot.gameObject.SetActive(false);
	}

	private void MakeCaseRoulette(Transform parent)
	{
		caseRouletteRoot = new GameObject("Case Roulette State").AddComponent<RectTransform>();
		caseRouletteRoot.SetParent(parent, false);
		ApplyAnchor(caseRouletteRoot, TextAnchor.MiddleCenter);
		caseRouletteRoot.anchoredPosition = new Vector2(0f, 35f);
		caseRouletteRoot.sizeDelta = new Vector2(920f, 402f);
		caseRouletteGroup = caseRouletteRoot.gameObject.AddComponent<CanvasGroup>();
		caseRouletteGroup.alpha = 0f;
		caseRouletteGroup.blocksRaycasts = false;
		caseRouletteGroup.interactable = false;
		Image outerGlow = MakeUiBlock(caseRouletteRoot, "Roulette Outer Violet Glow", Vector2.zero, new Vector2(992f, 444f), TextAnchor.MiddleCenter, new Color(0.56f, 0.18f, 1f, 0.24f));
		outerGlow.sprite = caseRadialGlowSprite;
		outerGlow.raycastTarget = false;
		Image frame = MakeThinGlassPanel(caseRouletteRoot, "Roulette Precision Frame", Vector2.zero, new Vector2(938f, 388f), TextAnchor.MiddleCenter, new Color(0.01f, 0.008f, 0.026f, 0.96f), new Color(0.55f, 0.23f, 0.94f, 0.72f), 1.5f);
		frame.raycastTarget = false;
		RectTransform viewport = new GameObject("Roulette Viewport").AddComponent<RectTransform>();
		viewport.SetParent(caseRouletteRoot, false);
		ApplyAnchor(viewport, TextAnchor.MiddleCenter);
		viewport.anchoredPosition = Vector2.zero;
		viewport.sizeDelta = new Vector2(920f, 350f);
		Image viewportImage = viewport.gameObject.AddComponent<Image>();
		viewportImage.color = new Color(0.008f, 0.007f, 0.02f, 0.98f);
		viewportImage.raycastTarget = false;
		viewport.gameObject.AddComponent<RectMask2D>();
		Image wash = MakeUiBlock(viewport, "Roulette Inner Energy Wash", Vector2.zero, new Vector2(920f, 350f), TextAnchor.MiddleCenter, new Color(0.34f, 0.08f, 0.62f, 0.18f));
		wash.sprite = caseRadialGlowSprite;
		wash.raycastTarget = false;
		caseRouletteContent = new GameObject("Roulette Content").AddComponent<RectTransform>();
		caseRouletteContent.SetParent(viewport, false);
		caseRouletteContent.anchorMin = new Vector2(0f, 0.5f);
		caseRouletteContent.anchorMax = new Vector2(0f, 0.5f);
		caseRouletteContent.pivot = new Vector2(0f, 0.5f);
		caseRouletteContent.anchoredPosition = Vector2.zero;
		Image leftVignette = MakeUiBlock(caseRouletteRoot, "Roulette Left Vignette", new Vector2(-415f, 0f), new Vector2(90f, 350f), TextAnchor.MiddleCenter, Color.white);
		leftVignette.sprite = caseRouletteLeftVignetteSprite;
		leftVignette.raycastTarget = false;
		Image rightVignette = MakeUiBlock(caseRouletteRoot, "Roulette Right Vignette", new Vector2(415f, 0f), new Vector2(90f, 350f), TextAnchor.MiddleCenter, Color.white);
		rightVignette.sprite = caseRouletteRightVignetteSprite;
		rightVignette.raycastTarget = false;
		caseRoulettePulse = MakeUiBlock(caseRouletteRoot, "Winning Column Pulse", Vector2.zero, new Vector2(108f, 350f), TextAnchor.MiddleCenter, new Color(1f, 0.7f, 0.16f, 0f));
		caseRoulettePulse.sprite = caseRadialGlowSprite;
		caseRoulettePulse.raycastTarget = false;
		caseRouletteMarkerGlow = MakeUiBlock(caseRouletteRoot, "Selector Soft Gold Aura", Vector2.zero, new Vector2(58f, 384f), TextAnchor.MiddleCenter, new Color(1f, 0.56f, 0.08f, 0.22f));
		caseRouletteMarkerGlow.sprite = caseRadialGlowSprite;
		caseRouletteMarkerGlow.raycastTarget = false;
		Image spine = MakeUiBlock(caseRouletteRoot, "Selector Obsidian Spine", Vector2.zero, new Vector2(8f, 368f), TextAnchor.MiddleCenter, new Color(0.025f, 0.012f, 0.045f, 0.88f));
		spine.raycastTarget = false;
		Image bloom = MakeUiBlock(caseRouletteRoot, "Selector Inner Gold Bloom", Vector2.zero, new Vector2(18f, 374f), TextAnchor.MiddleCenter, new Color(1f, 0.64f, 0.08f, 0.42f));
		bloom.sprite = caseRadialGlowSprite;
		bloom.raycastTarget = false;
		Image edge = MakeUiBlock(caseRouletteRoot, "Selector Gold Edge", Vector2.zero, new Vector2(4f, 368f), TextAnchor.MiddleCenter, new Color(1f, 0.62f, 0.08f, 0.96f));
		edge.raycastTarget = false;
		caseRouletteMarkerCore = MakeUiBlock(caseRouletteRoot, "Selector White Hot Core", Vector2.zero, new Vector2(1.4f, 368f), TextAnchor.MiddleCenter, new Color(1f, 0.96f, 0.72f, 1f));
		caseRouletteMarkerCore.raycastTarget = false;
		Image topGlow = MakeUiBlock(caseRouletteRoot, "Selector Top Pointer Glow", new Vector2(0f, 190f), new Vector2(48f, 36f), TextAnchor.MiddleCenter, new Color(1f, 0.58f, 0.08f, 0.34f));
		topGlow.sprite = caseRadialGlowSprite;
		topGlow.raycastTarget = false;
		Image topTip = MakeUiBlock(caseRouletteRoot, "Selector Top Precision Tip", new Vector2(0f, 188f), new Vector2(24f, 18f), TextAnchor.MiddleCenter, new Color(1f, 0.92f, 0.52f, 1f));
		topTip.sprite = caseTriangleDownSprite;
		topTip.raycastTarget = false;
		Image bottomGlow = MakeUiBlock(caseRouletteRoot, "Selector Bottom Pointer Glow", new Vector2(0f, -190f), new Vector2(48f, 36f), TextAnchor.MiddleCenter, new Color(1f, 0.58f, 0.08f, 0.34f));
		bottomGlow.sprite = caseRadialGlowSprite;
		bottomGlow.raycastTarget = false;
		Image bottomTip = MakeUiBlock(caseRouletteRoot, "Selector Bottom Precision Tip", new Vector2(0f, -188f), new Vector2(24f, 18f), TextAnchor.MiddleCenter, new Color(1f, 0.92f, 0.52f, 1f));
		bottomTip.sprite = caseTriangleUpSprite;
		bottomTip.raycastTarget = false;
		Image jewelGlow = MakeUiBlock(caseRouletteRoot, "Selector Center Jewel Glow", Vector2.zero, new Vector2(42f, 42f), TextAnchor.MiddleCenter, new Color(1f, 0.57f, 0.08f, 0.34f));
		jewelGlow.sprite = caseRadialGlowSprite;
		jewelGlow.raycastTarget = false;
		Image jewel = MakeUiBlock(caseRouletteRoot, "Selector Center Jewel", Vector2.zero, new Vector2(11f, 11f), TextAnchor.MiddleCenter, new Color(1f, 0.94f, 0.6f, 1f));
		jewel.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 45f);
		jewel.raycastTarget = false;
		Image topRail = MakeUiBlock(caseRouletteRoot, "Roulette Top Rail", new Vector2(0f, 193f), new Vector2(932f, 3f), TextAnchor.MiddleCenter, new Color(0.72f, 0.36f, 1f, 0.92f));
		topRail.sprite = caseRouletteRailSprite;
		topRail.raycastTarget = false;
		Image bottomRail = MakeUiBlock(caseRouletteRoot, "Roulette Bottom Rail", new Vector2(0f, -193f), new Vector2(932f, 3f), TextAnchor.MiddleCenter, new Color(0.72f, 0.36f, 1f, 0.92f));
		bottomRail.sprite = caseRouletteRailSprite;
		bottomRail.raycastTarget = false;
		caseRouletteRoot.gameObject.SetActive(false);
	}

	private void MakeCaseResultPanel(Transform parent)
	{
		caseResultRoot = new GameObject("WinRevealOverlay").AddComponent<RectTransform>();
		caseResultRoot.SetParent(parent, false);
		caseResultRoot.anchorMin = new Vector2(0f, 0.5f);
		caseResultRoot.anchorMax = new Vector2(1f, 0.5f);
		caseResultRoot.pivot = new Vector2(0.5f, 0.5f);
		caseResultRoot.anchoredPosition = Vector2.zero;
		caseResultRoot.sizeDelta = new Vector2(0f, 1460f);
		caseResultGroup = caseResultRoot.gameObject.AddComponent<CanvasGroup>();
		caseResultGroup.alpha = 0f;
		caseResultGroup.blocksRaycasts = false;
		caseResultGroup.interactable = false;
		Image background = MakeUiBlock(caseResultRoot, "radial purple dark background", Vector2.zero, new Vector2(1080f, 1460f), TextAnchor.MiddleCenter, new Color(0.024f, 0.016f, 0.063f, 0.985f));
		background.raycastTarget = true;
		Image glow = MakeUiBlock(caseResultRoot, "WinReveal radial purple glow", new Vector2(0f, 260f), new Vector2(980f, 780f), TextAnchor.MiddleCenter, new Color(0.46f, 0.15f, 0.82f, 0.38f));
		glow.sprite = caseRadialGlowSprite;
		glow.raycastTarget = false;
		for (int i = 0; i < 36; i++)
		{
			float angle = i / 36f * Mathf.PI * 2f;
			float radius = 105f + i * 37 % 90;
			Image particle = MakeUiBlock(caseResultRoot, "Particle Burst " + i.ToString(), new Vector2(Mathf.Cos(angle) * radius, 270f + Mathf.Sin(angle) * radius * 0.72f), new Vector2(4f + i % 5, 4f + i % 5), TextAnchor.MiddleCenter, new Color(0.72f, 0.34f + i % 4 * 0.08f, 1f, 0.62f));
			particle.sprite = softCircleSprite;
			particle.raycastTarget = false;
		}
		Image ring = MakeUiBlock(caseResultRoot, "Winning Image Glow Ring", new Vector2(0f, 270f), new Vector2(650f, 420f), TextAnchor.MiddleCenter, new Color(0.7f, 0.24f, 1f, 0.44f));
		ring.sprite = caseRadialGlowSprite;
		ring.raycastTarget = false;
		Image frame = MakeThinGlassPanel(caseResultRoot, "Result Prize Frame", new Vector2(0f, 270f), new Vector2(560f, 286f), TextAnchor.MiddleCenter, new Color(0.035f, 0.014f, 0.07f, 0.96f), new Color(0.78f, 0.42f, 1f, 0.86f), 2f);
		frame.raycastTarget = false;
		RectTransform mask = new GameObject("Result Prize Image Mask").AddComponent<RectTransform>();
		mask.SetParent(frame.transform, false);
		ApplyAnchor(mask, TextAnchor.MiddleCenter);
		mask.anchoredPosition = Vector2.zero;
		mask.sizeDelta = new Vector2(548f, 274f);
		mask.gameObject.AddComponent<RectMask2D>();
		caseResultPrizeImage = MakeResourceImage(mask, "Dynamic Winning Prize Art", "алмаз 1", Vector2.zero, new Vector2(260f, 260f), TextAnchor.MiddleCenter, true);
		MakeCornerAccent(frame.transform, "Corner TL", new Vector2(-280f, 143f), true, true);
		MakeCornerAccent(frame.transform, "Corner TR", new Vector2(280f, 143f), false, true);
		MakeCornerAccent(frame.transform, "Corner BL", new Vector2(-280f, -143f), true, false);
		MakeCornerAccent(frame.transform, "Corner BR", new Vector2(280f, -143f), false, false);
		caseResultRarityText = MakeText(caseResultRoot, "EPIC CRYSTAL CACHE", 22, TextAnchor.MiddleCenter, new Vector2(0f, 56f));
		caseResultRarityText.fontStyle = FontStyle.Bold;
		caseResultRarityText.color = new Color(0.94f, 0.72f, 1f, 0.94f);
		caseResultRarityText.raycastTarget = false;
		caseResultRarityText.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 42f);
		caseResultTitleText = MakeText(caseResultRoot, "+5 DIAMONDS", 48, TextAnchor.MiddleCenter, new Vector2(0f, -20f));
		caseResultTitleText.fontStyle = FontStyle.Bold;
		caseResultTitleText.color = Color.white;
		caseResultTitleText.lineSpacing = 0.82f;
		caseResultTitleText.raycastTarget = false;
		caseResultTitleText.GetComponent<RectTransform>().sizeDelta = new Vector2(760f, 104f);
		caseResultSubtitleText = MakeText(caseResultRoot, "ADDED TO YOUR BALANCE", 18, TextAnchor.MiddleCenter, new Vector2(0f, -98f));
		caseResultSubtitleText.color = new Color(0.7f, 0.58f, 0.86f, 0.72f);
		caseResultSubtitleText.raycastTarget = false;
		caseResultSubtitleText.GetComponent<RectTransform>().sizeDelta = new Vector2(820f, 36f);
		Image pill = MakeThinGlassPanel(caseResultRoot, "XP pill", new Vector2(0f, -148f), new Vector2(300f, 42f), TextAnchor.MiddleCenter, new Color(0.22f, 0.08f, 0.34f, 0.72f), new Color(0.76f, 0.42f, 1f, 0.34f), 1f);
		pill.raycastTarget = false;
		caseResultBonusText = MakeText(pill.transform, "◆  CRYSTAL REWARD", 19, TextAnchor.MiddleCenter, Vector2.zero);
		caseResultBonusText.fontStyle = FontStyle.Bold;
		caseResultBonusText.color = new Color(1f, 0.74f, 0.14f, 0.96f);
		caseResultBonusText.raycastTarget = false;
		caseResultBonusText.GetComponent<RectTransform>().sizeDelta = new Vector2(300f, 42f);
		caseCollectButton = MakeProfileActionButton(caseResultRoot, "COLLECT", new Vector2(-220f, -238f), new Vector2(400f, 82f), false, false, out _, true, true);
		caseCollectButton.onClick.AddListener(CollectCaseReward);
		caseSpinAgainButton = MakeProfileActionButton(caseResultRoot, "WATCH AD  •  AGAIN", new Vector2(220f, -238f), new Vector2(400f, 82f), true, false, out _, true, true);
		caseSpinAgainButton.onClick.AddListener(SpinCaseAgain);
		caseResultRoot.gameObject.SetActive(false);
	}

	private void MakeCornerAccent(Transform parent, string name, Vector2 pos, bool left, bool top)
	{
		RectTransform root = new GameObject(name).AddComponent<RectTransform>();
		root.SetParent(parent, false);
		ApplyAnchor(root, TextAnchor.MiddleCenter);
		root.anchoredPosition = pos;
		root.sizeDelta = new Vector2(34f, 34f);
		Image horizontal = MakeUiBlock(root, "Corner H", new Vector2(left ? 8f : -8f, top ? -16f : 16f), new Vector2(34f, 2f), TextAnchor.MiddleCenter, new Color(0.78f, 0.42f, 1f, 0.94f));
		horizontal.raycastTarget = false;
		Image vertical = MakeUiBlock(root, "Corner V", new Vector2(left ? -16f : 16f, top ? -8f : 8f), new Vector2(2f, 34f), TextAnchor.MiddleCenter, new Color(0.78f, 0.42f, 1f, 0.94f));
		vertical.raycastTarget = false;
	}

	private void ShowCaseResultPanel()
	{
		UpdateCaseResultVisual();
		if (caseRouletteGroup != null)
		{
			caseRouletteGroup.alpha = 0.22f;
		}
		if (caseOpeningStatusRoot != null)
		{
			caseOpeningStatusRoot.gameObject.SetActive(false);
		}
		if (caseResultRoot != null)
		{
			caseResultRoot.gameObject.SetActive(true);
		}
		if (caseResultGroup != null)
		{
			caseResultGroup.alpha = 1f;
			caseResultGroup.blocksRaycasts = true;
			caseResultGroup.interactable = true;
		}
		InfernoDailyCases.RefreshOverlay(this);   /* [user request 2026-09-21] "again" is disabled once the last case is used */
	}

	private void UpdateCaseResultVisual()
	{
		Color color = CasePrizeColor(caseSelectedPrize);
		bool vehicle = caseSelectedPrize.Kind == CasePrizeKind.Vehicle;
		string resource = vehicle ? CaseVehicleImageResource(caseSelectedPrize.VehicleId) : CaseDiamondImageResource(caseSelectedPrize);
		if (caseResultPrizeImage != null)
		{
			caseResultPrizeImage.sprite = vehicle ? LoadResourceSprite(resource) : LoadTightMenuArtSprite(resource);
			caseResultPrizeImage.preserveAspect = !vehicle;
			caseResultPrizeImage.rectTransform.anchoredPosition = Vector2.zero;
			caseResultPrizeImage.rectTransform.sizeDelta = vehicle ? new Vector2(548f, 274f) : CaseResultPrizeArtSize(caseSelectedPrize);
			RectTransform container = caseResultPrizeImage.rectTransform.parent as RectTransform;
			if (vehicle && container != null && caseResultPrizeImage.sprite != null)
			{
				ApplyObjectFitCover(caseResultPrizeImage.rectTransform, container, caseResultPrizeImage.sprite);
			}
		}
		if (caseResultRarityText != null)
		{
			caseResultRarityText.text = vehicle ? "◆  JACKPOT VEHICLE  ◆" : CasePrizeRarity(caseSelectedPrize) + " CRYSTAL CACHE";
			caseResultRarityText.color = color;
		}
		if (caseResultTitleText != null)
		{
			caseResultTitleText.text = vehicle ? CaseVehicleDisplayName(caseSelectedPrize.VehicleId) : "+" + caseSelectedPrize.Diamonds.ToString() + " DIAMONDS";
		}
		if (caseResultSubtitleText != null)
		{
			caseResultSubtitleText.text = vehicle ? "NEW VEHICLE UNLOCKED" : "ADDED TO YOUR BALANCE";
		}
		if (caseResultBonusText != null)
		{
			caseResultBonusText.text = vehicle ? "★  PERMANENT GARAGE UNLOCK" : "◆  CRYSTAL REWARD";
		}
	}

	private void CollectCaseReward()
	{
		ApplyCaseRewardOnce();
		HideCaseOverlay();
	}

	private void ApplyCaseRewardOnce()
	{
		if (caseRewardApplied || !caseRewardReady)
		{
			return;
		}
		caseRewardApplied = true;
		if (caseSelectedPrize.Kind == CasePrizeKind.Vehicle)
		{
			SupabaseGameManager.QueueCaseReward(caseSelectedPrize.VehicleId, 0);
			return;
		}
		SupabaseGameManager.QueueCaseReward(null, caseSelectedPrize.Diamonds);
	}

	private void SpinCaseAgain()
	{
		RequestCaseRewardedOpening(true);
	}

	private void RequestCaseRewardedOpening()
	{
		RequestCaseRewardedOpening(false);
	}

	private void RequestCaseRewardedOpening(bool openingAgain)
	{
		/* [user request 2026-09-21] daily allowance used up: no ad, the screen shows the reset countdown */
		if (!InfernoDailyCases.CanOpen())
		{
			InfernoDailyCases.RefreshOverlay(this);
			return;
		}
		if (!SupabaseGameManager.IsAuthenticated || !SupabaseGameManager.ProfileMatchesSession)
		{
			FirstLaunchAuthScreen.Show();
			return;
		}
		LevelPlayAdsService ads = LevelPlayAdsService.Instance;
		if (ads == null)
		{
			SetCaseAdUnavailable("ADS ARE LOADING...");
			return;
		}
		if (caseOpenButton != null)
		{
			caseOpenButton.interactable = false;
		}
		if (caseSpinAgainButton != null)
		{
			caseSpinAgainButton.interactable = false;
		}
		caseMusicSuppressed = true;
		int requestSerial = ++caseOpeningRequestSerial;
		ads.ShowCaseRewarded(() =>
		{
			if (!CaseOpeningRequestIsCurrent(requestSerial))
			{
				return;
			}
			if (openingAgain)
			{
				ResetCaseOpening(true);
			}
			StartCaseOpening();
		}, message =>
		{
			if (!CaseOpeningRequestIsCurrent(requestSerial))
			{
				return;
			}
			caseMusicSuppressed = false;
			SetCaseAdUnavailable(message);
		});
	}

	private bool CaseOpeningRequestIsCurrent(int requestSerial)
	{
		if (requestSerial == caseOpeningRequestSerial && caseOverlayRoot != null)
		{
			return caseOverlayRoot.activeInHierarchy;
		}
		return false;
	}

	private void SetCaseAdUnavailable(string message)
	{
		if (caseOpenButton != null)
		{
			caseOpenButton.interactable = true;
		}
		if (caseSpinAgainButton != null)
		{
			caseSpinAgainButton.interactable = true;
		}
		if (caseOpenButtonLabel != null)
		{
			caseOpenButtonLabel.text = "AD LOADING  •  TRY AGAIN";
		}
		if (caseStatusLabel != null)
		{
			caseStatusLabel.text = string.IsNullOrEmpty(message) ? "AD UNAVAILABLE" : message.ToUpperInvariant();
			caseStatusLabel.color = new Color(1f, 0.42f, 0.58f, 0.94f);
		}
	}

	private bool EnsureCasePreviewWorld()
	{
		if (casePreviewReady)
		{
			return true;
		}
		if (casePreviewTexture == null)
		{
			casePreviewTexture = new RenderTexture(990, 630, 16, RenderTextureFormat.ARGB32);
			casePreviewTexture.name = "Case Opening Preview Texture";
			casePreviewTexture.antiAliasing = 1;
			casePreviewTexture.filterMode = FilterMode.Bilinear;
			casePreviewTexture.Create();
		}
		if (casePreviewImage != null)
		{
			casePreviewImage.texture = casePreviewTexture;
		}
		casePreviewRoot = new GameObject("Case Opening Preview World").transform;
		casePreviewRoot.position = new Vector3(5200f, 0f, 0f);
		caseTurntableRoot = new GameObject("Case Opening Turntable Root").transform;
		caseTurntableRoot.SetParent(casePreviewRoot, false);
		caseTurntableRoot.localPosition = Vector3.zero;
		caseTurntableRoot.localRotation = Quaternion.identity;
		caseTurntableRoot.localScale = Vector3.one;
		caseModelRoot = new GameObject("Case Opening Model Root").transform;
		caseModelRoot.SetParent(caseTurntableRoot, false);
		caseModelRoot.localPosition = Vector3.zero;
		caseModelRoot.localRotation = Quaternion.identity;
		caseModelRoot.localScale = Vector3.one;
		GameObject bodyPrefab = Resources.Load<GameObject>("кейс боди финал");
		GameObject headPrefab = Resources.Load<GameObject>("крышка точно финал");
		if (bodyPrefab == null || headPrefab == null)
		{
			Debug.LogWarning("[CASE] Final case models are missing from Resources: кейс боди финал.glb / крышка точно финал.glb");
			return false;
		}
		GameObject body = UnityEngine.Object.Instantiate(bodyPrefab, caseModelRoot);
		body.name = "Case Body";
		body.transform.localPosition = Vector3.zero;
		body.transform.localRotation = Quaternion.identity;
		body.transform.localScale = Vector3.one;
		GameObject head = UnityEngine.Object.Instantiate(headPrefab, caseModelRoot);
		head.name = "Case Head";
		head.transform.localPosition = Vector3.zero;
		head.transform.localRotation = Quaternion.identity;
		head.transform.localScale = Vector3.one;
		caseHeadRoot = head.transform;
		Camera[] cameras = caseModelRoot.GetComponentsInChildren<Camera>(true);
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].enabled = false;
			cameras[i].gameObject.SetActive(false);
		}
		Light[] lights = caseModelRoot.GetComponentsInChildren<Light>(true);
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].enabled = false;
			lights[i].gameObject.SetActive(false);
		}
		NormalizeCasePreviewModel();
		StyleCasePreviewModel();
		casePreviewCamera = new GameObject("Case Preview Camera").AddComponent<Camera>();
		casePreviewCamera.transform.SetParent(casePreviewRoot, false);
		casePreviewCamera.transform.localPosition = new Vector3(0f, 1.24f, -4.95f);
		casePreviewCamera.transform.LookAt(casePreviewRoot.position + new Vector3(0f, 0.58f, 0f));
		casePreviewCamera.clearFlags = CameraClearFlags.Color;
		casePreviewCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
		casePreviewCamera.cullingMask = 536870912;
		casePreviewCamera.fieldOfView = 40f;
		casePreviewCamera.nearClipPlane = 0.03f;
		casePreviewCamera.farClipPlane = 40f;
		casePreviewCamera.targetTexture = casePreviewTexture;
		casePreviewCamera.allowHDR = false;
		casePreviewCamera.allowMSAA = false;
		casePreviewCamera.enabled = false;
		SetLayerRecursively(casePreviewRoot.gameObject, 29);
		casePreviewRoot.gameObject.SetActive(false);
		casePreviewReady = true;
		ResetCasePreviewPose();
		return true;
	}

	private void NormalizeCasePreviewModel()
	{
		caseHasLidAnimation = false;
		caseLidPivot = null;
		if (caseModelRoot == null || caseHeadRoot == null)
		{
			return;
		}
		Renderer[] renderers = caseModelRoot.GetComponentsInChildren<Renderer>(true);
		if (renderers.Length == 0)
		{
			return;
		}
		Bounds worldBounds = renderers[0].bounds;
		for (int i = 1; i < renderers.Length; i++)
		{
			worldBounds.Encapsulate(renderers[i].bounds);
		}
		Bounds localBounds = MakeLocalBounds(caseModelRoot, worldBounds);
		float footprint = Mathf.Max(localBounds.size.x, localBounds.size.z);
		float scale = footprint > 0.001f ? 2.58f / footprint : 1f;
		if (caseHeadRoot != null && caseHeadRoot.GetComponentsInChildren<Renderer>(true).Length != 0)
		{
			AlignCaseHeadToClosedBodyPose();
			Bounds headBounds = CalculateLocalRendererBounds(caseModelRoot, caseHeadRoot.gameObject);
			Vector3 hinge = new Vector3(headBounds.center.x, headBounds.min.y, headBounds.max.z);
			caseLidPivot = new GameObject("Case Lid Hinge Pivot").transform;
			caseLidPivot.SetParent(caseModelRoot, false);
			caseLidPivot.localPosition = hinge;
			caseLidPivot.localRotation = Quaternion.identity;
			caseHeadRoot.SetParent(caseLidPivot, true);
			caseHasLidAnimation = true;
		}
		localBounds = CalculateLocalRendererBounds(caseModelRoot, caseModelRoot.gameObject);
		footprint = Mathf.Max(localBounds.size.x, localBounds.size.z);
		scale = footprint > 0.001f ? 2.58f / footprint : 1f;
		caseModelRoot.localScale = Vector3.one * scale;
		caseModelBaseLocalPosition = new Vector3(-localBounds.center.x * scale, -localBounds.min.y * scale - 0.26f - 0.258f, -localBounds.center.z * scale);
		caseModelRoot.localPosition = caseModelBaseLocalPosition;
	}

	private void AlignCaseHeadToClosedBodyPose()
	{
		if (caseModelRoot == null || caseHeadRoot == null)
		{
			return;
		}
		Transform body = caseModelRoot.Find("Case Body");
		if (body == null)
		{
			return;
		}
		Bounds bodyBounds = CalculateLocalRendererBounds(caseModelRoot, body.gameObject);
		Bounds headBounds = CalculateLocalRendererBounds(caseModelRoot, caseHeadRoot.gameObject);
		if (bodyBounds.size == Vector3.zero || headBounds.size == Vector3.zero)
		{
			return;
		}
		float overlap = Mathf.Max(0.01f, headBounds.size.y * 0.12f);
		Vector3 delta = new Vector3(bodyBounds.center.x - headBounds.center.x, bodyBounds.max.y - headBounds.min.y - overlap, bodyBounds.center.z - headBounds.center.z);
		caseHeadRoot.localPosition = caseHeadRoot.localPosition + delta;
	}

	private void StyleCasePreviewModel()
	{
		if (caseModelRoot == null)
		{
			return;
		}
		Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
		if (shader == null)
		{
			shader = Shader.Find("Unlit/Texture");
		}
		if (shader == null)
		{
			shader = Shader.Find("Sprites/Default");
		}
		Renderer[] renderers = caseModelRoot.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Renderer renderer = renderers[i];
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			Material[] materials = renderer.materials;
			for (int j = 0; j < materials.Length; j++)
			{
				Material material = materials[j];
				if (material == null)
				{
					continue;
				}
				Texture texture = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : material.mainTexture;
				Color color = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") : (material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white);
				int renderQueue = material.renderQueue;
				if (shader != null)
				{
					material.shader = shader;
				}
				if (material.HasProperty("_BaseMap"))
				{
					material.SetTexture("_BaseMap", texture);
				}
				if (material.HasProperty("_MainTex"))
				{
					material.SetTexture("_MainTex", texture);
				}
				if (material.HasProperty("_BaseColor"))
				{
					material.SetColor("_BaseColor", color);
				}
				if (material.HasProperty("_Color"))
				{
					material.SetColor("_Color", color);
				}
				if (renderQueue >= 0)
				{
					material.renderQueue = renderQueue;
				}
			}
		}
	}

	private void SetCasePreviewVisible(bool visible)
	{
		if (casePreviewRoot != null)
		{
			casePreviewRoot.gameObject.SetActive(visible);
		}
		if (casePreviewCamera != null)
		{
			casePreviewCamera.enabled = visible;
		}
	}

	private void AddCasePreviewDragEvents(GameObject target)
	{
		EventTrigger trigger = target.GetComponent<EventTrigger>();
		if (trigger == null)
		{
			trigger = target.AddComponent<EventTrigger>();
		}
		EventTrigger.Entry drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
		drag.callback.AddListener(data => DragCasePreview(data as PointerEventData));
		trigger.triggers.Add(drag);
	}

	private void DragCasePreview(PointerEventData eventData)
	{
		if (eventData == null || caseOpenAnimationActive || caseSpinActive)
		{
			return;
		}
		casePreviewYaw = Mathf.Clamp(casePreviewYaw + eventData.delta.x * 0.26f, -60f, 60f);
		ApplyCasePreviewPose();
	}

	private void ApplyCasePreviewPose()
	{
		if (caseTurntableRoot != null)
		{
			caseTurntableRoot.localRotation = Quaternion.Euler(0f, casePreviewYaw, 0f);
			return;
		}
		if (caseModelRoot != null)
		{
			caseModelRoot.localRotation = Quaternion.Euler(0f, casePreviewYaw, 0f);
		}
	}

	private void ResetCasePreviewPose()
	{
		casePreviewYaw = 0f;
		ApplyCasePreviewPose();
		if (caseModelRoot != null)
		{
			caseModelRoot.localPosition = caseModelBaseLocalPosition;
			caseModelRoot.localRotation = caseTurntableRoot != null ? Quaternion.identity : Quaternion.Euler(0f, casePreviewYaw, 0f);
		}
		if (caseLidPivot != null)
		{
			caseLidPivot.localRotation = Quaternion.identity;
		}
	}

	private void StartCaseOpening()
	{
		if (caseRewardReady)
		{
			HideCaseOverlay();
			return;
		}
		if (caseOpenAnimationActive || caseSpinActive || caseRouletteContent == null)
		{
			return;
		}
		/* [user request 2026-09-21] one of today's cases is spent the moment the lid starts opening */
		if (!InfernoDailyCases.Consume())
		{
			InfernoDailyCases.RefreshOverlay(this);
			return;
		}
		InfernoDailyCases.RefreshOverlay(this);
		caseSelectedPrize = RollCasePrize();
		caseRewardApplied = false;
		if (caseOpenButton != null)
		{
			caseOpenButton.interactable = false;
		}
		if (caseOpenButtonLabel != null)
		{
			caseOpenButtonLabel.text = "";
		}
		if (caseOpenButton != null)
		{
			caseOpenButton.gameObject.SetActive(false);
		}
		if (caseOpenButtonGlow != null)
		{
			caseOpenButtonGlow.SetActive(false);
		}
		if (caseCostRowRoot != null)
		{
			caseCostRowRoot.gameObject.SetActive(false);
		}
		if (caseRouletteRoot != null)
		{
			caseRouletteRoot.gameObject.SetActive(false);
		}
		if (caseRouletteGroup != null)
		{
			caseRouletteGroup.alpha = 0f;
		}
		if (caseOpeningStatusRoot != null)
		{
			caseOpeningStatusRoot.gameObject.SetActive(true);
		}
		if (caseOpeningPreviewRoot != null)
		{
			caseOpeningPreviewRoot.gameObject.SetActive(true);
		}
		if (caseStatusLabel != null)
		{
			caseStatusLabel.text = "UNLOCKING...";
			caseStatusLabel.color = Color.white;
		}
		if (!EnsureCasePreviewWorld())
		{
			if (caseOpeningPreviewRoot != null)
			{
				caseOpeningPreviewRoot.gameObject.SetActive(false);
			}
			BeginCaseRoulette();
			return;
		}
		ResetCasePreviewPose();
		SetCasePreviewVisible(true);
		caseOpenAnimationActive = true;
		caseOpenAnimationElapsed = 0f;
		caseOpeningAudioStarted = false;
	}

	private void BeginCaseRoulette()
	{
		BuildCaseRouletteItems(36, caseSelectedPrize, 250f, 310f, 6f);
		if (caseClosedRoot != null)
		{
			caseClosedRoot.gameObject.SetActive(true);
		}
		if (caseClosedStaticVisualRoot != null)
		{
			caseClosedStaticVisualRoot.SetActive(false);
		}
		if (caseClosedGroup != null)
		{
			caseClosedGroup.alpha = 1f;
		}
		if (caseResultRoot != null)
		{
			caseResultRoot.gameObject.SetActive(false);
		}
		if (caseResultGroup != null)
		{
			caseResultGroup.alpha = 0f;
		}
		if (caseOpeningPreviewRoot != null)
		{
			caseOpeningPreviewRoot.gameObject.SetActive(false);
		}
		SetCasePreviewVisible(false);
		if (caseRouletteRoot != null)
		{
			caseRouletteRoot.gameObject.SetActive(true);
		}
		if (caseRouletteGroup != null)
		{
			caseRouletteGroup.alpha = 1f;
		}
		if (caseOpenButton != null)
		{
			caseOpenButton.gameObject.SetActive(false);
		}
		if (caseOpenButtonGlow != null)
		{
			caseOpenButtonGlow.SetActive(false);
		}
		if (caseCostRowRoot != null)
		{
			caseCostRowRoot.gameObject.SetActive(false);
		}
		if (caseOpeningStatusRoot != null)
		{
			caseOpeningStatusRoot.gameObject.SetActive(true);
		}
		if (caseStatusLabel != null)
		{
			caseStatusLabel.text = "OPENING...";
			caseStatusLabel.color = Color.white;
		}
		if (caseOpenButtonLabel != null)
		{
			caseOpenButtonLabel.text = "";
		}
		caseSpinActive = true;
		caseSpinElapsed = 0f;
		caseSpinDuration = 9f;
		caseSpinStartX = -256f;
		caseSpinTickPulse = 0f;
		caseSpinLastTickIndex = -1;
		PlayCaseSequenceAudio(false);
		float targetOffset = 9346f;
		caseSpinTargetX = 460f - targetOffset;
		caseRouletteContent.anchoredPosition = new Vector2(caseSpinStartX, 0f);
		UpdateCaseRouletteFocus(caseSpinStartX, 250f, 6f, 920f, false);
		UpdateCaseRouletteChrome(0f, 0f);
	}

	private CasePrize RollCasePrize()
	{
		List<string> locked = GetLockedCaseVehicles();
		if (locked.Count > 0 && UnityEngine.Random.value < 0.06f)
		{
			return CasePrize.Vehicle(locked[UnityEngine.Random.Range(0, locked.Count)]);
		}
		float roll = UnityEngine.Random.value;
		if (roll < 0.42f)
		{
			return CasePrize.Diamond(2);
		}
		if (roll < 0.72f)
		{
			return CasePrize.Diamond(3);
		}
		if (roll < 0.91f)
		{
			return CasePrize.Diamond(5);
		}
		return CasePrize.Diamond(10);
	}

	private List<string> GetLockedCaseVehicles()
	{
		List<string> locked = new List<string>(3);
		string[] candidates = new string[3] { "flying_car", "infal_model", "car3" };
		for (int i = 0; i < candidates.Length; i++)
		{
			string vehicleId = candidates[i];
			if (!SupabaseGameManager.IsVehicleUnlocked(vehicleId))
			{
				locked.Add(vehicleId);
			}
		}
		InfernoGarage.AppendLockedCaseVehicles(this, locked); /* [InfernoLobby patch] */
		return locked;
	}

	private CasePrize RandomRouletteDisplayPrize()
	{
		List<string> locked = GetLockedCaseVehicles();
		if (locked.Count > 0 && UnityEngine.Random.value < 0.08f)
		{
			return CasePrize.Vehicle(locked[UnityEngine.Random.Range(0, locked.Count)]);
		}
		float roll = UnityEngine.Random.value;
		if (roll < 0.38f)
		{
			return CasePrize.Diamond(2);
		}
		if (roll < 0.68f)
		{
			return CasePrize.Diamond(3);
		}
		if (roll < 0.89f)
		{
			return CasePrize.Diamond(5);
		}
		return CasePrize.Diamond(10);
	}

	private string CaseVehicleDisplayName(string vehicleId)
	{
		{ string edition = InfernoGarage.CaseName(vehicleId); if (edition != null) return edition; } /* [InfernoLobby patch] */
		if (vehicleId == "flying_car")
		{
			return "NIGHT GLIDER";
		}
		if (vehicleId == "infal_model")
		{
			return "P900 INFERNO";
		}
		if (vehicleId == "car3")
		{
			return "CAYMAN GT4";
		}
		return "N SERIES CAR";
	}

	private string CaseVehicleImageResource(string vehicleId)
	{
		{ string edition = InfernoGarage.CasePicture(vehicleId); if (edition != null) return edition; } /* [InfernoLobby patch] */
		if (vehicleId == "flying_car")
		{
			return "flyingcar photo";
		}
		if (vehicleId == "infal_model")
		{
			return "impala photo";
		}
		if (vehicleId == "car3")
		{
			return "porsche photo";
		}
		return "sportcar photo";
	}

	private string CaseDiamondImageResource(CasePrize prize)
	{
		if (prize.Diamonds >= 10)
		{
			return "сундук";
		}
		if (prize.Diamonds >= 5)
		{
			return "алмаз 3";
		}
		return "алмаз 1";
	}

	private Vector2 CaseRoulettePrizeArtSize(CasePrize prize)
	{
		if (prize.Diamonds >= 10)
		{
			return new Vector2(170f, 142f);
		}
		if (prize.Diamonds >= 5)
		{
			return new Vector2(150f, 132f);
		}
		return new Vector2(140f, 108f);
	}

	private Vector2 CaseResultPrizeArtSize(CasePrize prize)
	{
		if (prize.Diamonds >= 10)
		{
			return new Vector2(330f, 270f);
		}
		if (prize.Diamonds >= 5)
		{
			return new Vector2(290f, 254f);
		}
		return new Vector2(260f, 196f);
	}

	private Color CasePrizeColor(CasePrize prize)
	{
		if (prize.Kind == CasePrizeKind.Vehicle)
		{
			return new Color(0.82f, 0.32f, 1f, 1f);
		}
		if (prize.Diamonds >= 10)
		{
			return new Color(1f, 0.68f, 0.16f, 1f);
		}
		if (prize.Diamonds >= 5)
		{
			return new Color(0.76f, 0.3f, 1f, 1f);
		}
		if (prize.Diamonds >= 3)
		{
			return new Color(0.24f, 0.7f, 1f, 1f);
		}
		return new Color(0.38f, 0.52f, 0.88f, 1f);
	}

	private string CasePrizeRarity(CasePrize prize)
	{
		if (prize.Kind == CasePrizeKind.Vehicle)
		{
			return "JACKPOT VEHICLE";
		}
		if (prize.Diamonds >= 10)
		{
			return "LEGENDARY";
		}
		if (prize.Diamonds >= 5)
		{
			return "EPIC";
		}
		if (prize.Diamonds >= 3)
		{
			return "RARE";
		}
		return "STANDARD";
	}

	private void BuildCaseRouletteItems(int targetIndex, CasePrize targetPrize, float cardWidth, float cardHeight, float spacing)
	{
		if (caseRouletteContent == null)
		{
			return;
		}
		for (int i = caseRouletteContent.childCount - 1; i >= 0; i--)
		{
			Destroy(caseRouletteContent.GetChild(i).gameObject);
		}
		caseRouletteContent.sizeDelta = new Vector2(10f + 48f * cardWidth + 47f * spacing, 350f);
		for (int i = 0; i < 48; i++)
		{
			CasePrize prize = i == targetIndex ? targetPrize : RandomRouletteDisplayPrize();
			MakeCasePrizeCard(caseRouletteContent, i, prize, new Vector2(5f + i * (cardWidth + spacing), 0f), new Vector2(cardWidth, cardHeight), i == targetIndex);
		}
	}

	private void MakeCasePrizeCard(Transform parent, int index, CasePrize prize, Vector2 pos, Vector2 size, bool target)
	{
		Color color = CasePrizeColor(prize);
		string prizeName = prize.Kind == CasePrizeKind.Vehicle ? CaseVehicleDisplayName(prize.VehicleId) : prize.Diamonds.ToString() + " DIAMONDS";
		RectTransform card = new GameObject((target ? "Target " : "") + prizeName + " Prize Card " + index.ToString()).AddComponent<RectTransform>();
		card.SetParent(parent, false);
		card.anchorMin = new Vector2(0f, 0.5f);
		card.anchorMax = new Vector2(0f, 0.5f);
		card.pivot = new Vector2(0.5f, 0.5f);
		card.anchoredPosition = pos + new Vector2(size.x * 0.5f, 0f);
		card.sizeDelta = size;
		card.gameObject.AddComponent<CanvasGroup>().alpha = 0.72f;
		Image shadow = MakeUiBlock(card, "Prize Energy Shadow", Vector2.zero, target ? size + new Vector2(64f, 72f) : size + new Vector2(40f, 48f), TextAnchor.MiddleCenter, new Color(color.r, color.g, color.b, target ? 0.34f : 0.14f));
		shadow.sprite = caseRadialGlowSprite;
		shadow.raycastTarget = false;
		Image border = MakeUiBlock(card, "Card Precision Border", Vector2.zero, size, TextAnchor.MiddleCenter, new Color(color.r, color.g, color.b, target ? 1f : 0.68f));
		border.sprite = roundedGlassSprite;
		border.type = Image.Type.Sliced;
		border.raycastTarget = false;
		Image interior = MakeUiBlock(border.transform, "Card Obsidian Interior", Vector2.zero, size - new Vector2(4f, 4f), TextAnchor.MiddleCenter, new Color(0.01f, 0.011f, 0.026f, 0.995f));
		interior.sprite = roundedGlassSprite;
		interior.type = Image.Type.Sliced;
		interior.raycastTarget = false;
		Image rarityGlow = MakeUiBlock(interior.transform, "Card Rarity Glow", new Vector2(0f, 25f), new Vector2(size.x - 8f, size.y - 16f), TextAnchor.MiddleCenter, new Color(color.r, color.g, color.b, prize.Kind == CasePrizeKind.Vehicle ? 0.28f : 0.16f));
		rarityGlow.sprite = caseRadialGlowSprite;
		rarityGlow.raycastTarget = false;
		Text header = MakeText(interior.transform, prize.Kind == CasePrizeKind.Vehicle ? "UNLOCK" : "CRYSTAL CACHE", 12, TextAnchor.UpperCenter, new Vector2(0f, -14f));
		header.fontStyle = FontStyle.Bold;
		header.color = new Color(0.82f, 0.78f, 0.94f, 0.62f);
		header.raycastTarget = false;
		header.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x - 20f, 24f);
		RectTransform viewport = new GameObject("ImageViewport Cover").AddComponent<RectTransform>();
		viewport.SetParent(interior.transform, false);
		Vector2 topCenter = new Vector2(0.5f, 1f);
		viewport.anchorMax = topCenter;
		viewport.anchorMin = topCenter;
		viewport.pivot = new Vector2(0.5f, 1f);
		viewport.anchoredPosition = new Vector2(0f, -34f);
		viewport.sizeDelta = new Vector2(size.x - 12f, 164f);
		viewport.gameObject.AddComponent<RectMask2D>();
		string resource = prize.Kind == CasePrizeKind.Vehicle ? CaseVehicleImageResource(prize.VehicleId) : CaseDiamondImageResource(prize);
		bool diamond = prize.Kind == CasePrizeKind.Diamonds;
		Image art = MakeResourceImage(viewport, "Prize Hero Art", resource, Vector2.zero, diamond ? CaseRoulettePrizeArtSize(prize) : viewport.sizeDelta, TextAnchor.MiddleCenter, diamond);
		if (art != null)
		{
			if (diamond)
			{
				art.sprite = LoadTightMenuArtSprite(resource);
			}
			art.raycastTarget = false;
			if (!diamond)
			{
				ApplyObjectFitCover(art.rectTransform, viewport, art.sprite);
			}
		}
		if (prize.Kind == CasePrizeKind.Diamonds)
		{
			Text amount = MakeText(interior.transform, "×" + prize.Diamonds.ToString(), 36, TextAnchor.MiddleCenter, new Vector2(0f, -65f));
			amount.fontStyle = FontStyle.Bold;
			amount.color = Color.white;
			amount.raycastTarget = false;
			amount.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x - 20f, 50f);
		}
		Image strip = MakeUiBlock(border.transform, "Bottom Rarity Strip", Vector2.zero, new Vector2(size.x, 80.6f), TextAnchor.LowerCenter, new Color(color.r, color.g, color.b, 0.92f));
		strip.sprite = caseRarityStripSprite;
		strip.raycastTarget = false;
		Image stripBorder = MakeUiBlock(strip.transform, "Rarity Strip Top Border", new Vector2(0f, 39.8f), new Vector2(size.x, 1f), TextAnchor.MiddleCenter, new Color(color.r, color.g, color.b, 0.267f));
		stripBorder.raycastTarget = false;
		Text rarity = MakeText(strip.transform, CasePrizeRarity(prize), 13, TextAnchor.MiddleCenter, new Vector2(0f, 14f));
		rarity.fontStyle = FontStyle.Bold;
		rarity.color = color;
		rarity.raycastTarget = false;
		rarity.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, 22f);
		Text nameText = MakeText(strip.transform, prizeName, 12, TextAnchor.MiddleCenter, new Vector2(0f, -13f));
		nameText.fontStyle = FontStyle.Bold;
		nameText.color = new Color(0.94f, 0.9f, 1f, 0.88f);
		nameText.raycastTarget = false;
		nameText.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, 22f);
	}

	private void UpdateCaseRouletteFocus(float contentX, float cardWidth, float spacing, float viewportWidth, bool tick)
	{
		if (caseRouletteContent == null)
		{
			return;
		}
		float center = viewportWidth * 0.5f;
		float step = cardWidth + spacing;
		int focusIndex = Mathf.RoundToInt((center - contentX - cardWidth * 0.5f) / Mathf.Max(1f, step));
		if (tick && focusIndex != caseSpinLastTickIndex)
		{
			caseSpinLastTickIndex = focusIndex;
			caseSpinTickPulse = 1f;
		}
		for (int i = 0; i < caseRouletteContent.childCount; i++)
		{
			RectTransform child = caseRouletteContent.GetChild(i) as RectTransform;
			if (child == null)
			{
				continue;
			}
			float distance = Mathf.Abs(contentX + child.anchoredPosition.x - center);
			float closeness = 1f - Mathf.Clamp01(distance / 260f);
			float weight = Mathf.SmoothStep(0f, 1f, closeness);
			child.localScale = Vector3.one * Mathf.Lerp(0.94f, 1.045f, weight);
			child.anchoredPosition = new Vector2(child.anchoredPosition.x, 0f);
			child.localRotation = Quaternion.identity;
			CanvasGroup group = child.GetComponent<CanvasGroup>();
			if (group == null)
			{
				group = child.gameObject.AddComponent<CanvasGroup>();
			}
			group.alpha = Mathf.Lerp(0.58f, 1f, weight);
		}
	}

	private void UpdateCaseRouletteChrome(float t, float dt)
	{
		caseSpinTickPulse = Mathf.Max(0f, caseSpinTickPulse - dt * 5.8f);
		float pulse = Mathf.SmoothStep(0f, 1f, caseSpinTickPulse);
		float shimmer = 0.5f + Mathf.Sin((Time.unscaledTime + t) * 9.5f) * 0.5f;
		if (caseRouletteMarkerGlow != null)
		{
			caseRouletteMarkerGlow.color = new Color(1f, 0.56f, 0.08f, 0.18f + pulse * 0.25f + shimmer * 0.04f);
			caseRouletteMarkerGlow.rectTransform.sizeDelta = new Vector2(54f + pulse * 14f, 384f);
		}
		if (caseRouletteMarkerCore != null)
		{
			caseRouletteMarkerCore.color = new Color(1f, 0.96f, 0.72f, 0.92f + pulse * 0.08f);
			caseRouletteMarkerCore.rectTransform.sizeDelta = new Vector2(1.4f + pulse * 0.5f, 368f);
		}
		if (caseRoulettePulse != null)
		{
			caseRoulettePulse.color = new Color(1f, 0.68f, 0.1f, pulse * 0.24f);
			float pulseWidth = 92f + pulse * 44f;
			caseRoulettePulse.rectTransform.sizeDelta = new Vector2(pulseWidth, 350f);
		}
		if (caseOpeningDots != null)
		{
			for (int i = 0; i < caseOpeningDots.Length; i++)
			{
				Image dot = caseOpeningDots[i];
				if (dot == null)
				{
					continue;
				}
				float phase = Mathf.Repeat(Time.unscaledTime / 1.1f - i * 0.18f, 1f);
				float bounce = phase < 0.4f ? Mathf.Sin(phase / 0.4f * Mathf.PI) : 0f;
				float dotSize = Mathf.Lerp(4.8f, 8f, bounce);
				dot.rectTransform.sizeDelta = new Vector2(dotSize, dotSize);
				dot.color = new Color(0.75f, 0.4f, 1f, Mathf.Lerp(0.3f, 1f, bounce));
			}
		}
	}

	private void UpdateCaseOpening(float dt)
	{
		if (caseOpenAnimationActive)
		{
			UpdateCaseOpenAnimation(dt);
			return;
		}
		if (!caseSpinActive || caseRouletteContent == null)
		{
			return;
		}
		caseSpinElapsed = Mathf.Min(caseSpinDuration, caseSpinElapsed + Mathf.Max(0f, dt));
		float t = Mathf.Clamp01(caseSpinElapsed / Mathf.Max(0.01f, caseSpinDuration));
		float fastEnd = caseSpinTargetX + 640f;
		float slowEnd = caseSpinTargetX + 64f;
		float x;
		if (caseSpinElapsed < 3.5f)
		{
			float duration = 3.5f;
			float phase = Mathf.Clamp01(caseSpinElapsed / duration);
			float speed = (fastEnd - caseSpinStartX) / duration;
			x = HermitePosition(caseSpinStartX, fastEnd, speed * duration, -307.2f * duration, phase);
		}
		else if (caseSpinElapsed < 7f)
		{
			float duration = 3.5f;
			float phase = Mathf.Clamp01((caseSpinElapsed - 3.5f) / duration);
			x = HermitePosition(fastEnd, slowEnd, -307.2f * duration, -128f * duration, phase);
		}
		else if (caseSpinElapsed < 7.5f)
		{
			float duration = 0.5f;
			float phase = Mathf.Clamp01((caseSpinElapsed - 7f) / duration);
			x = HermitePosition(slowEnd, caseSpinTargetX, -128f * duration, 0f, phase);
		}
		else
		{
			x = caseSpinTargetX;
		}
		caseRouletteContent.anchoredPosition = new Vector2(x, 0f);
		UpdateCaseRouletteFocus(x, 250f, 6f, 920f, true);
		UpdateCaseRouletteChrome(t, dt);
		if (t >= 1f)
		{
			CompleteCaseOpening();
		}
	}

	private static float HermitePosition(float start, float end, float startTangent, float endTangent, float t)
	{
		t = Mathf.Clamp01(t);
		float t2 = t * t;
		float t3 = t2 * t;
		return (2f * t3 - 3f * t2 + 1f) * start + (t3 - 2f * t2 + t) * startTangent + (-2f * t3 + 3f * t2) * end + (t3 - t2) * endTangent;
	}

	private void UpdateCaseOpenAnimation(float dt)
	{
		float duration = Mathf.Max(0.01f, caseOpenAnimationDuration);
		caseOpenAnimationElapsed = Mathf.Min(duration, caseOpenAnimationElapsed + Mathf.Min(dt, 0.0333333351f));
		float elapsed = caseOpenAnimationElapsed;
		if (!caseOpeningAudioStarted && elapsed >= 1f)
		{
			caseOpeningAudioStarted = true;
			PlayCaseSequenceAudio(true);
		}
		float charge = Mathf.Clamp01(elapsed / 1f);
		float lift = Mathf.Clamp01((elapsed - 1f) / 2f);
		float lidEase = lift * lift * lift * (lift * (lift * 6f - 15f) + 10f);
		float lidOvershoot = Mathf.Sin(lift * Mathf.PI) * Mathf.Pow(lift, 2.6f) * 3.2f;
		float shakeWeight = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(charge / 0.18f)) * (1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((charge - 0.78f) / 0.22f)));
		float shakeX = (Mathf.Sin(elapsed * Mathf.PI * 18f) + Mathf.Sin(elapsed * Mathf.PI * 31f) * 0.32f) * 0.007f * shakeWeight;
		float shakeY = Mathf.Sin(elapsed * Mathf.PI * 23f + 0.8f) * 0.0018f * shakeWeight;
		float shakeRoll = Mathf.Sin(elapsed * Mathf.PI * 19f) * 0.42f * shakeWeight;
		if (caseHasLidAnimation && caseLidPivot != null)
		{
			float lidAngle = Mathf.Lerp(0f, 76f, lidEase) + lidOvershoot;
			caseLidPivot.localRotation = Quaternion.Euler(lidAngle, 0f, 0f);
		}
		if (caseTurntableRoot != null)
		{
			caseTurntableRoot.localRotation = Quaternion.Euler(0f, casePreviewYaw, 0f);
		}
		if (caseModelRoot != null)
		{
			caseModelRoot.localPosition = caseModelBaseLocalPosition + new Vector3(shakeX, shakeY, 0f);
			caseModelRoot.localRotation = Quaternion.Euler(0f, 0f, shakeRoll);
		}
		if (caseStatusLabel != null)
		{
			if (elapsed >= 3f)
			{
				caseStatusLabel.text = "CASE OPENED";
			}
			else if (elapsed >= 1f)
			{
				caseStatusLabel.text = caseHasLidAnimation ? "LIFTING LID..." : "ENERGIZING CASE...";
			}
			else
			{
				caseStatusLabel.text = "UNLOCKING...";
			}
		}
		if (caseOpenAnimationElapsed >= duration)
		{
			FinishCaseOpenAnimationAndStartRoulette();
		}
	}

	private void FinishCaseOpenAnimationAndStartRoulette()
	{
		if (!caseOpenAnimationActive)
		{
			return;
		}
		caseOpenAnimationActive = false;
		RegisterDailyCaseOpened();
		if (caseLidPivot != null)
		{
			caseLidPivot.localRotation = Quaternion.Euler(76f, 0f, 0f);
		}
		if (caseOpeningPreviewRoot != null)
		{
			caseOpeningPreviewRoot.gameObject.SetActive(false);
		}
		SetCasePreviewVisible(false);
		BeginCaseRoulette();
	}

	private void CompleteCaseOpening()
	{
		caseSpinActive = false;
		caseMusicSuppressed = false;
		StopCaseSequenceAudio();
		if (caseRouletteContent != null)
		{
			caseRouletteContent.anchoredPosition = new Vector2(caseSpinTargetX, 0f);
		}
		caseSpinTickPulse = 1f;
		UpdateCaseRouletteFocus(caseSpinTargetX, 250f, 6f, 920f, false);
		UpdateCaseRouletteChrome(1f, 0f);
		ShowCaseResultPanel();
		caseRewardReady = true;
		ApplyCaseRewardOnce();
		if (caseStatusLabel != null)
		{
			caseStatusLabel.text = "";
			caseStatusLabel.color = new Color(0.82f, 0.72f, 0.96f, 0f);
		}
	}

	private void ResetCaseOpening(bool showReadyPreview = true)
	{
		StopCaseSequenceAudio();
		caseOpeningAudioStarted = false;
		caseOpenAnimationActive = false;
		caseOpenAnimationElapsed = 0f;
		caseSpinActive = false;
		caseRewardReady = false;
		caseRewardApplied = false;
		caseSpinTickPulse = 0f;
		caseSpinLastTickIndex = -1;
		bool previewReady = showReadyPreview && EnsureCasePreviewWorld();
		if (previewReady)
		{
			ResetCasePreviewPose();
		}
		if (caseClosedRoot != null)
		{
			caseClosedRoot.gameObject.SetActive(true);
		}
		if (caseClosedStaticVisualRoot != null)
		{
			caseClosedStaticVisualRoot.SetActive(!previewReady);
		}
		if (caseClosedGroup != null)
		{
			caseClosedGroup.alpha = 1f;
		}
		if (caseRouletteContent != null)
		{
			BuildCaseRouletteItems(36, CasePrize.Diamond(5), 250f, 310f, 6f);
			caseRouletteContent.anchoredPosition = new Vector2(-256f, 0f);
			UpdateCaseRouletteFocus(-256f, 250f, 6f, 920f, false);
			UpdateCaseRouletteChrome(0f, 0f);
		}
		if (caseRouletteRoot != null)
		{
			caseRouletteRoot.gameObject.SetActive(false);
		}
		if (caseRouletteGroup != null)
		{
			caseRouletteGroup.alpha = 0f;
			caseRouletteGroup.blocksRaycasts = false;
			caseRouletteGroup.interactable = false;
		}
		if (caseResultRoot != null)
		{
			caseResultRoot.gameObject.SetActive(false);
		}
		if (caseResultGroup != null)
		{
			caseResultGroup.alpha = 0f;
			caseResultGroup.blocksRaycasts = false;
			caseResultGroup.interactable = false;
		}
		if (caseOpeningPreviewRoot != null)
		{
			caseOpeningPreviewRoot.gameObject.SetActive(previewReady);
		}
		SetCasePreviewVisible(previewReady);
		if (caseOpenButton != null)
		{
			caseOpenButton.interactable = true;
		}
		if (caseOpenButton != null)
		{
			caseOpenButton.gameObject.SetActive(true);
		}
		if (caseOpenButtonGlow != null)
		{
			caseOpenButtonGlow.SetActive(true);
		}
		if (caseCostRowRoot != null)
		{
			caseCostRowRoot.gameObject.SetActive(true);
		}
		if (caseOpeningStatusRoot != null)
		{
			caseOpeningStatusRoot.gameObject.SetActive(false);
		}
		if (caseOpenButtonLabel != null)
		{
			caseOpenButtonLabel.text = "WATCH AD + OPEN CASE";
		}
		if (caseStatusLabel != null)
		{
			caseStatusLabel.text = "OPENING...";
			caseStatusLabel.color = Color.white;
		}
	}

	private void PlayCaseSequenceAudio(bool opening)
	{
		if (caseSequenceAudioSource == null)
		{
			GameObject host = new GameObject("Case Sequence Audio");
			host.transform.SetParent(transform, false);
			caseSequenceAudioSource = host.AddComponent<AudioSource>();
			caseSequenceAudioSource.playOnAwake = false;
			caseSequenceAudioSource.loop = false;
			caseSequenceAudioSource.spatialBlend = 0f;
			caseSequenceAudioSource.ignoreListenerPause = true;
		}
		if (opening && caseOpeningAudioClip == null)
		{
			caseOpeningAudioClip = Resources.Load<AudioClip>("открытие кейса звук");
		}
		else if (!opening && caseRouletteAudioClip == null)
		{
			caseRouletteAudioClip = Resources.Load<AudioClip>("рулетка звук");
		}
		AudioClip clip = opening ? caseOpeningAudioClip : caseRouletteAudioClip;
		caseSequenceAudioSource.Stop();
		caseSequenceAudioSource.clip = clip;
		caseSequenceAudioSource.volume = Mathf.Clamp01(sfxVolume / 100f);
		if (clip != null)
		{
			caseSequenceAudioSource.Play();
		}
	}

	private void StopCaseSequenceAudio()
	{
		if (caseSequenceAudioSource == null)
		{
			return;
		}
		caseSequenceAudioSource.Stop();
		caseSequenceAudioSource.clip = null;
	}

	private void ShowCaseOverlay()
	{
		if (caseOverlayRoot == null)
		{
			return;
		}
		ResetCaseOpening(true);
		caseOverlayRoot.transform.SetAsLastSibling();
		caseOverlayRoot.SetActive(true);
		InfernoLobby.CaseVisibility(this, true); /* [InfernoLobby patch] */
		InfernoDailyCases.RefreshOverlay(this);   /* [user request 2026-09-21] after the reset re-enabled the button */
	}

	private void HideCaseOverlay()
	{
		caseOpeningRequestSerial++;
		caseMusicSuppressed = false;
		bool rewardApplied = caseRewardApplied;
		ResetCaseOpening(false);
		SetCasePreviewVisible(false);
		if (caseOverlayRoot != null)
		{
			caseOverlayRoot.SetActive(false);
		}
		caseHomeCardRequiresFreshPointerDown = true;
		if (rewardApplied)
		{
			_ = SupabaseGameManager.SyncProfileAfterCaseClosedAsync();
		}
		InfernoLobby.CaseVisibility(this, false); /* [InfernoLobby patch] */
		InfernoDailyCases.RefreshCard();   /* [user request 2026-09-21] the lobby card shows what is left */
	}

	private Button MakeCaseActionButton(Transform parent, string label, Vector2 pos, Vector2 size, TextAnchor anchor, bool filled)
	{
		Button button = new GameObject(label + " Case Button").AddComponent<Button>();
		button.transform.SetParent(parent);
		Image background = button.gameObject.AddComponent<Image>();
		background.sprite = filled ? casePrimaryButtonSprite : caseSecondaryButtonSprite;
		background.type = Image.Type.Sliced;
		background.color = Color.white;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		button.targetGraphic = background;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = filled ? new Color(1.05f, 1.02f, 1.08f, 1f) : new Color(1.04f, 1.02f, 1.06f, 1f);
		colors.pressedColor = filled ? new Color(0.84f, 0.78f, 0.92f, 1f) : new Color(0.82f, 0.78f, 0.9f, 1f);
		colors.selectedColor = colors.highlightedColor;
		colors.disabledColor = new Color(0.62f, 0.56f, 0.7f, 0.55f);
		colors.fadeDuration = 0.08f;
		button.colors = colors;
		Image glow = MakeUiBlock(button.transform, "Case Button Glow", Vector2.zero, size + new Vector2(20f, 18f), TextAnchor.MiddleCenter, filled ? new Color(0.5f, 0.18f, 0.92f, 0.2f) : new Color(0.5f, 0.18f, 0.92f, 0.08f));
		glow.sprite = roundedGlowSprite;
		glow.type = Image.Type.Sliced;
		glow.raycastTarget = false;
		glow.transform.SetAsFirstSibling();
		Image sheen = MakeUiBlock(button.transform, "Case Button Sheen", new Vector2(0f, size.y * 0.2f), new Vector2(size.x - 36f, size.y * 0.24f), TextAnchor.MiddleCenter, new Color(1f, 0.92f, 1f, filled ? 0.055f : 0.025f));
		sheen.sprite = roundedGlassSprite;
		sheen.type = Image.Type.Sliced;
		sheen.raycastTarget = false;
		Image shade = MakeUiBlock(button.transform, "Case Button Bottom Shade", new Vector2(0f, -size.y * 0.24f), new Vector2(size.x - 30f, size.y * 0.24f), TextAnchor.MiddleCenter, new Color(0.01f, 0.006f, 0.025f, filled ? 0.16f : 0.24f));
		shade.sprite = roundedGlassSprite;
		shade.type = Image.Type.Sliced;
		shade.raycastTarget = false;
		Text text = MakeText(button.transform, label, Mathf.RoundToInt(size.y * 0.245f), TextAnchor.MiddleCenter, Vector2.zero);
		text.fontStyle = FontStyle.Bold;
		text.color = filled ? new Color(0.965f, 0.925f, 1f, 1f) : new Color(0.86f, 0.78f, 0.96f, 0.96f);
		text.raycastTarget = false;
		text.GetComponent<RectTransform>().sizeDelta = size;
		text.transform.SetAsLastSibling();
		return button;
	}
}
