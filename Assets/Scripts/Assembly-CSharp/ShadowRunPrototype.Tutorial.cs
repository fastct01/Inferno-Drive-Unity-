using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

// Translated from Il2CppOutput Assembly-CSharp.cpp:
//   ShadowRunPrototype_MakeFirstRunTutorial_mA91C55763A99EA5151A083D00D808E2FAAE162AF
//   ShadowRunPrototype_BeginFirstRunTutorialIfNeeded_m6F1D6F32B71BDECFE361086C32CCCD08153C149A
//   ShadowRunPrototype_RestoreFirstRunTutorialAfterPause_m01BADB106A24416DB0318546DB594CED210ACEE6
//   ShadowRunPrototype_HideFirstRunTutorialForPause_m74CEED3648A9AA93F1D441DE1F9254934205A790
//   ShadowRunPrototype_CancelFirstRunTutorial_m13F413989A636F96B1D834BA982FC410126DCFEA
//   ShadowRunPrototype_CompleteFirstRunTutorial_m9FF50DF977531CAFBABF776072C888E8EC57ED62
//   ShadowRunPrototype_ReleaseFirstRunTutorialReferences_m2CD58FC632E06190A755DE75277000F814828C02
//   ShadowRunPrototype_SetFirstRunTutorialStage_m73E53031864AE301DB0EC0E0F08559D22CB8242C
//   ShadowRunPrototype_SetFirstRunTutorialCopy_m76FB6FAF8D981558F03BCB120267929AE494D062
//   ShadowRunPrototype_CenterFirstRunTutorialText_mD99E75280E4A927995F5EAE223C3EB57AAC6E45A
//   ShadowRunPrototype_LayoutFirstRunTutorialForScreen_m4953A5791F0A61D4F0BD522D6D1C66F9C0B866DF
//   ShadowRunPrototype_SetFirstRunTutorialHand_m34DFC87F0B98470F6A0FF37F8445648AE5CE3B96
//   ShadowRunPrototype_PointFirstRunTutorialHandAtButton_mF2FE14D1798B3DD5AC9E53115FF35F7762EE4F44
//   ShadowRunPrototype_UpdateFirstRunTutorialProgress_mA4A5B7CA2D04FA1C0475A4975A208D0DADCD977C
//   ShadowRunPrototype_CopyFirstRunTutorialTarget_m1F258C43329B63B22594884D298093764A3DF8EF
//   ShadowRunPrototype_SetFirstRunTutorialHighlightArea_m2A788A8025F3D48C66E7C6EB1E0746763B23AC88
//   ShadowRunPrototype_UpdateFirstRunTutorial_mD73DF5FEE427D2932FBFD48B3818B4160297432A
//   ShadowRunPrototype_UpdateFirstRunTutorialHandAnimation_mE6C7FFAEA0A14DA08401AACDCA019303D9DE2053
//   ShadowRunPrototype_FirstRunTutorialDoubleTapRequested_mDA544E4AE6273B2A0EE080EC4E9FBB76DA218035
//   ShadowRunPrototype_RegisterFirstRunTutorialTap_m8FF883F9E95120D9A72A70270784AFF7F17A09F7
//   ShadowRunPrototype_NotifyFirstRunTutorialBeamPressed_mB3DB82A56DDF935F54A647B902804049F46D03A8
//   ShadowRunPrototype_ReadFirstRunTutorialAccelerationDrag_mA215F7BAA21D13352032843B0491168BA0780828
public sealed partial class ShadowRunPrototype
{
	private void MakeFirstRunTutorial(Transform parent)
	{
		firstRunTutorialRussian = Application.systemLanguage == SystemLanguage.Russian;
		firstRunTutorialRoot = new GameObject("First Run Tutorial");
		firstRunTutorialRoot.transform.SetParent(parent, false);
		RectTransform rootRect = firstRunTutorialRoot.AddComponent<RectTransform>();
		rootRect.anchorMin = Vector2.zero;
		rootRect.anchorMax = Vector2.one;
		rootRect.offsetMin = Vector2.zero;
		rootRect.offsetMax = Vector2.zero;
		firstRunTutorialRootRect = rootRect;
		firstRunTutorialGroup = firstRunTutorialRoot.AddComponent<CanvasGroup>();
		firstRunTutorialGroup.blocksRaycasts = false;
		firstRunTutorialGroup.interactable = false;
		Image shade = new GameObject("Tutorial Full Screen Shade").AddComponent<Image>();
		shade.transform.SetParent(firstRunTutorialRoot.transform, false);
		shade.color = new Color(0.002f, 0.001f, 0.01f, 0.57f);
		shade.raycastTarget = false;
		shade.rectTransform.anchorMin = Vector2.zero;
		shade.rectTransform.anchorMax = Vector2.one;
		shade.rectTransform.offsetMin = Vector2.zero;
		shade.rectTransform.offsetMax = Vector2.zero;
		Image cardGlow = MakeUiBlock(firstRunTutorialRoot.transform, "Tutorial Card Glow", new Vector2(0f, 270f), new Vector2(1030f, 320f), TextAnchor.MiddleCenter, new Color(0.62f, 0.2f, 1f, 0.16f));
		cardGlow.sprite = roundedGlowSprite;
		cardGlow.type = Image.Type.Sliced;
		cardGlow.raycastTarget = false;
		firstRunTutorialCardGlowRect = cardGlow.rectTransform;
		Image card = MakeUiBlock(firstRunTutorialRoot.transform, "Tutorial Card", new Vector2(0f, 270f), new Vector2(1000f, 286f), TextAnchor.MiddleCenter, new Color(0.01f, 0.005f, 0.026f, 0.97f));
		card.sprite = roundedGlassSprite;
		card.type = Image.Type.Sliced;
		card.raycastTarget = false;
		firstRunTutorialCardRect = card.rectTransform;
		Image inner = MakeUiBlock(card.transform, "Tutorial Card Inner", Vector2.zero, new Vector2(974f, 260f), TextAnchor.MiddleCenter, new Color(0.15f, 0.055f, 0.26f, 0.16f));
		inner.sprite = roundedGlassSprite;
		inner.type = Image.Type.Sliced;
		inner.raycastTarget = false;
		Image sideAccent = MakeUiBlock(card.transform, "Tutorial Side Accent", new Vector2(-494f, 0f), new Vector2(6f, 162f), TextAnchor.MiddleCenter, new Color(0.7f, 0.3f, 1f, 0.96f));
		sideAccent.raycastTarget = false;
		Image cardAccent = MakeUiBlock(card.transform, "Tutorial Card Accent", new Vector2(0f, -140f), new Vector2(650f, 4f), TextAnchor.MiddleCenter, new Color(0.7f, 0.3f, 1f, 0.96f));
		cardAccent.raycastTarget = false;
		RectTransform copyRoot = new GameObject("Tutorial Centered Instructions").AddComponent<RectTransform>();
		copyRoot.SetParent(card.transform, false);
		copyRoot.anchorMin = Vector2.zero;
		copyRoot.anchorMax = Vector2.one;
		copyRoot.offsetMin = Vector2.zero;
		copyRoot.offsetMax = Vector2.zero;
		firstRunTutorialCopyGroup = copyRoot.gameObject.AddComponent<CanvasGroup>();
		firstRunTutorialCopyGroup.blocksRaycasts = false;
		firstRunTutorialCopyGroup.interactable = false;
		firstRunTutorialStepText = MakeText(copyRoot, "DRIVER TRAINING  01/05", 20, TextAnchor.MiddleCenter, new Vector2(0f, 96f));
		firstRunTutorialStepText.color = new Color(0.7f, 0.48f, 1f, 1f);
		firstRunTutorialStepText.fontStyle = FontStyle.Bold;
		firstRunTutorialStepText.rectTransform.sizeDelta = new Vector2(930f, 38f);
		firstRunTutorialStepText.raycastTarget = false;
		firstRunTutorialTitleText = MakeText(copyRoot, "", 40, TextAnchor.MiddleCenter, new Vector2(0f, 25f));
		firstRunTutorialTitleText.fontStyle = FontStyle.Bold;
		firstRunTutorialTitleText.resizeTextForBestFit = true;
		firstRunTutorialTitleText.resizeTextMinSize = 27;
		firstRunTutorialTitleText.resizeTextMaxSize = 40;
		firstRunTutorialTitleText.rectTransform.sizeDelta = new Vector2(930f, 92f);
		firstRunTutorialTitleText.raycastTarget = false;
		firstRunTutorialHintText = MakeText(copyRoot, "", 22, TextAnchor.MiddleCenter, new Vector2(0f, -58f));
		firstRunTutorialHintText.color = new Color(0.78f, 0.72f, 0.86f, 0.96f);
		firstRunTutorialHintText.resizeTextForBestFit = true;
		firstRunTutorialHintText.resizeTextMinSize = 17;
		firstRunTutorialHintText.resizeTextMaxSize = 22;
		firstRunTutorialHintText.rectTransform.sizeDelta = new Vector2(930f, 52f);
		firstRunTutorialHintText.raycastTarget = false;
		for (int i = 0; i < firstRunTutorialProgressDots.Length; i++)
		{
			int number = i + 1;
			Image dot = MakeUiBlock(card.transform, "Tutorial Progress " + number.ToString(), new Vector2((i - (firstRunTutorialProgressDots.Length - 1) * 0.5f) * 32f, -116f), new Vector2(12f, 12f), TextAnchor.MiddleCenter, new Color(0.34f, 0.25f, 0.45f, 0.88f));
			dot.sprite = softCircleSprite;
			dot.raycastTarget = false;
			firstRunTutorialProgressDots[i] = dot;
		}
		Image skipPill = MakeUiBlock(firstRunTutorialRoot.transform, "Tutorial Skip Pill", new Vector2(0f, 70f), new Vector2(430f, 48f), TextAnchor.MiddleCenter, new Color(0.015f, 0.008f, 0.032f, 0.88f));
		skipPill.sprite = roundedGlassSprite;
		skipPill.type = Image.Type.Sliced;
		skipPill.raycastTarget = false;
		firstRunTutorialSkipRect = skipPill.rectTransform;
		Text skipText = MakeText(skipPill.transform, firstRunTutorialRussian ? "ДВОЙНОЕ КАСАНИЕ — ПРОПУСТИТЬ" : "DOUBLE TAP ANYWHERE TO SKIP", 16, TextAnchor.MiddleCenter, Vector2.zero);
		skipText.color = new Color(0.72f, 0.64f, 0.82f, 0.94f);
		skipText.fontStyle = FontStyle.Bold;
		skipText.rectTransform.sizeDelta = new Vector2(400f, 44f);
		skipText.raycastTarget = false;
		firstRunTutorialHighlightImage = MakeUiBlock(firstRunTutorialRoot.transform, "Tutorial Target Glow", Vector2.zero, new Vector2(260f, 144f), TextAnchor.LowerRight, new Color(0.68f, 0.3f, 1f, 0.2f));
		firstRunTutorialHighlightImage.sprite = roundedGlowSprite;
		firstRunTutorialHighlightImage.type = Image.Type.Sliced;
		firstRunTutorialHighlightImage.raycastTarget = false;
		firstRunTutorialHighlight = firstRunTutorialHighlightImage.rectTransform;
		Image targetEdge = MakeUiBlock(firstRunTutorialHighlight, "Tutorial Target Edge", Vector2.zero, new Vector2(246f, 130f), TextAnchor.MiddleCenter, new Color(0.94f, 0.8f, 1f, 0.17f));
		targetEdge.sprite = roundedGlassSprite;
		targetEdge.type = Image.Type.Sliced;
		targetEdge.raycastTarget = false;
		firstRunTutorialTargetCore = targetEdge.rectTransform;
		firstRunTutorialHandSprite = LoadResourceSprite("рука иконка");
		if (firstRunTutorialHandSprite != null)
		{
			firstRunTutorialHandImage = MakeUiBlock(firstRunTutorialRoot.transform, "Tutorial Hand Pointer", Vector2.zero, new Vector2(126f, 126f), TextAnchor.LowerCenter, Color.white);
			firstRunTutorialHandImage.sprite = firstRunTutorialHandSprite;
			firstRunTutorialHandImage.type = Image.Type.Simple;
			firstRunTutorialHandImage.preserveAspect = true;
			firstRunTutorialHandImage.raycastTarget = false;
			firstRunTutorialHandRect = firstRunTutorialHandImage.rectTransform;
			firstRunTutorialHandImage.gameObject.SetActive(false);
		}
		firstRunTutorialRoot.SetActive(false);
	}

	private void BeginFirstRunTutorialIfNeeded()
	{
		if (PlayerPrefs.GetInt("infernoDriveFirstRunTutorialV7", 0) == 1)
		{
			return;
		}
		if (firstRunTutorialRoot == null)
		{
			MakeFirstRunTutorial(hudRoot.transform);
		}
		firstRunTutorialActive = true;
		firstRunTutorialVisibleTime = 0f;
		firstRunTutorialInputTime = 0f;
		firstRunTutorialSecondaryInputTime = 0f;
		firstRunTutorialAccelerationHoldLearned = false;
		firstRunTutorialMouseDragTracking = false;
		firstRunTutorialLastTapTime = -10f;
		firstRunTutorialLastTapFrame = -1;
		firstRunTutorialTapIgnoreUntil = Time.unscaledTime + 0.55f;
		firstRunTutorialRoot.SetActive(true);
		firstRunTutorialRoot.transform.SetAsLastSibling();
		Canvas.ForceUpdateCanvases();
		LayoutFirstRunTutorialForScreen();
		SetFirstRunTutorialStage(1);
	}

	private void RestoreFirstRunTutorialAfterPause()
	{
		if (!firstRunTutorialActive || firstRunTutorialRoot == null)
		{
			return;
		}
		firstRunTutorialRoot.SetActive(true);
		firstRunTutorialRoot.transform.SetAsLastSibling();
	}

	private void HideFirstRunTutorialForPause()
	{
		if (firstRunTutorialRoot != null)
		{
			firstRunTutorialRoot.SetActive(false);
		}
	}

	private void CancelFirstRunTutorial()
	{
		firstRunTutorialActive = false;
		firstRunTutorialInputTime = 0f;
		firstRunTutorialSecondaryInputTime = 0f;
		firstRunTutorialAccelerationHoldLearned = false;
		firstRunTutorialMouseDragTracking = false;
		if (firstRunTutorialRoot != null)
		{
			firstRunTutorialRoot.SetActive(false);
		}
	}

	private void CompleteFirstRunTutorial()
	{
		PlayerPrefs.SetInt("infernoDriveFirstRunTutorialV7", 1);
		GameObject root = firstRunTutorialRoot;
		CancelFirstRunTutorial();
		if (root != null)
		{
			Destroy(root);
		}
		ReleaseFirstRunTutorialReferences();
	}

	private void ReleaseFirstRunTutorialReferences()
	{
		firstRunTutorialRoot = null;
		firstRunTutorialRootRect = null;
		firstRunTutorialCardRect = null;
		firstRunTutorialCardGlowRect = null;
		firstRunTutorialSkipRect = null;
		firstRunTutorialGroup = null;
		firstRunTutorialStepText = null;
		firstRunTutorialTitleText = null;
		firstRunTutorialHintText = null;
		firstRunTutorialCopyGroup = null;
		firstRunTutorialHighlight = null;
		firstRunTutorialHighlightImage = null;
		firstRunTutorialTargetCore = null;
		firstRunTutorialHandImage = null;
		firstRunTutorialHandRect = null;
		firstRunTutorialHandSprite = null;
		for (int i = 0; i < firstRunTutorialProgressDots.Length; i++)
		{
			firstRunTutorialProgressDots[i] = null;
		}
	}

	private void SetFirstRunTutorialStage(int stage)
	{
		if (stage == 3) stage = 4; /* [InfernoLobby patch] stage 3 ("hold brake") is skipped */
		firstRunTutorialStage = stage;
		firstRunTutorialStageTime = 0f;
		firstRunTutorialInputTime = 0f;
		firstRunTutorialSecondaryInputTime = 0f;
		firstRunTutorialAccelerationHoldLearned = false;
		firstRunTutorialMouseDragTracking = false;
		firstRunTutorialInputLockUntil = Time.unscaledTime + 0.22f;
		if (firstRunTutorialCopyGroup != null)
		{
			firstRunTutorialCopyGroup.alpha = 0f;
		}
		UpdateFirstRunTutorialProgress(stage);
		switch (stage)
		{
		case 0:
			SetFirstRunTutorialCopy(firstRunTutorialRussian ? "ОБУЧЕНИЕ" : "DRIVER TRAINING", firstRunTutorialRussian ? "УПРАВЛЕНИЕ ГОТОВО" : "CONTROLS ONLINE", firstRunTutorialRussian ? "БЫСТРО РАЗБЕРЁМСЯ" : "LET'S MAKE THIS QUICK");
			SetFirstRunTutorialHighlightArea(new Vector2(0f, 480f), new Vector2(620f, 300f), TextAnchor.LowerCenter, new Color(0.58f, 0.24f, 1f, 0.14f));
			SetFirstRunTutorialHand(true, TextAnchor.LowerCenter, new Vector2(0f, 650f), 0f, 120f);
			return;
		case 1:
			SetFirstRunTutorialCopy(firstRunTutorialRussian ? "ОБУЧЕНИЕ  01/05" : "DRIVER TRAINING  01/05", firstRunTutorialRussian ? "ЗАЖМИТЕ И ВЕДИТЕ В СТОРОНУ" : "HOLD, THEN SLIDE SIDEWAYS", firstRunTutorialRussian ? "УСКОРЯЙТЕСЬ И ПОВОРАЧИВАЙТЕ ОДНИМ ПАЛЬЦЕМ" : "ACCELERATE AND STEER WITH ONE FINGER");
			SetFirstRunTutorialHighlightArea(new Vector2(0f, 0f), new Vector2(1000f, 820f), TextAnchor.LowerCenter, new Color(0.5f, 0.2f, 1f, 0.13f));
			SetFirstRunTutorialHand(true, TextAnchor.LowerCenter, new Vector2(0f, 355f), 0f, 185f);
			return;
		case 2:
			SetFirstRunTutorialCopy(firstRunTutorialRussian ? "ОБУЧЕНИЕ  02/05" : "DRIVER TRAINING  02/05", firstRunTutorialRussian ? "НАЖМИТЕ  BEAM" : "TAP  BEAM", firstRunTutorialRussian ? "ПОПРОСИТЕ МАШИНУ ВПЕРЕДИ УСТУПИТЬ" : "ASK TRAFFIC AHEAD TO MOVE ASIDE");
			CopyFirstRunTutorialTarget(highBeamHudButtonRect, new Color(0.68f, 0.34f, 1f, 0.28f));
			PointFirstRunTutorialHandAtButton(highBeamHudButtonRect);
			return;
		case 3:
			SetFirstRunTutorialCopy(firstRunTutorialRussian ? "ОБУЧЕНИЕ  03/05" : "DRIVER TRAINING  03/05", firstRunTutorialRussian ? "ЗАЖМИТЕ  BRAKE" : "HOLD  BRAKE", firstRunTutorialRussian ? "СНИЖАЙТЕ СКОРОСТЬ ПЕРЕД УЗКИМ ПРОЕЗДОМ" : "CONTROL SPEED BEFORE A TIGHT GAP");
			CopyFirstRunTutorialTarget(brakeHudButtonRect, new Color(1f, 0.12f, 0.32f, 0.25f));
			PointFirstRunTutorialHandAtButton(brakeHudButtonRect);
			return;
		case 4:
			SetFirstRunTutorialCopy(firstRunTutorialRussian ? "ОБУЧЕНИЕ  04/05" : "DRIVER TRAINING  04/05", firstRunTutorialRussian ? "НАЖМИТЕ СЛЕВА" : "TOUCH THE LEFT SIDE", firstRunTutorialRussian ? "ПОВЕРНИТЕ ВЛЕВО" : "STEER LEFT");
			SetFirstRunTutorialHighlightArea(new Vector2(18f, 300f), new Vector2(320f, 980f), TextAnchor.LowerLeft, new Color(0.48f, 0.2f, 1f, 0.16f));
			SetFirstRunTutorialHand(true, TextAnchor.LowerLeft, new Vector2(170f, 560f), 22f, 165f);
			return;
		case 5:
			SetFirstRunTutorialCopy(firstRunTutorialRussian ? "ОБУЧЕНИЕ  05/05" : "DRIVER TRAINING  05/05", firstRunTutorialRussian ? "НАЖМИТЕ СПРАВА" : "TOUCH THE RIGHT SIDE", firstRunTutorialRussian ? "ПОВЕРНИТЕ ВПРАВО" : "STEER RIGHT");
			SetFirstRunTutorialHighlightArea(new Vector2(-18f, 300f), new Vector2(320f, 980f), TextAnchor.LowerRight, new Color(0.48f, 0.2f, 1f, 0.16f));
			SetFirstRunTutorialHand(true, TextAnchor.LowerRight, new Vector2(-170f, 560f), -22f, 165f);
			return;
		case 6:
			SetFirstRunTutorialCopy(firstRunTutorialRussian ? "ОБУЧЕНИЕ ЗАВЕРШЕНО" : "TRAINING COMPLETE", firstRunTutorialRussian ? "ВЫ ГОТОВЫ" : "YOU'RE READY", firstRunTutorialRussian ? "ПОКОРИТЕ НОЧЬ" : "OWN THE NIGHT");
			SetFirstRunTutorialHighlightArea(new Vector2(0f, 630f), new Vector2(780f, 420f), TextAnchor.LowerCenter, new Color(0.76f, 0.28f, 1f, 0.18f));
			SetFirstRunTutorialHand(false, TextAnchor.LowerCenter, Vector2.zero, 0f, 126f);
			return;
		default:
			CompleteFirstRunTutorial();
			return;
		}
	}

	private void SetFirstRunTutorialCopy(string step, string title, string hint)
	{
		firstRunTutorialStepText.text = step;
		firstRunTutorialTitleText.text = title;
		firstRunTutorialHintText.text = hint;
		CenterFirstRunTutorialText(firstRunTutorialStepText);
		CenterFirstRunTutorialText(firstRunTutorialTitleText);
		CenterFirstRunTutorialText(firstRunTutorialHintText);
	}

	private void CenterFirstRunTutorialText(Text text)
	{
		if (text == null)
		{
			return;
		}
		text.alignment = TextAnchor.MiddleCenter;
		RectTransform rect = text.rectTransform;
		Vector2 center = new Vector2(0.5f, 0.5f);
		rect.pivot = center;
		rect.anchorMax = center;
		rect.anchorMin = center;
		rect.anchoredPosition = new Vector2(0f, rect.anchoredPosition.y);
	}

	private void LayoutFirstRunTutorialForScreen()
	{
		if (firstRunTutorialRootRect == null)
		{
			return;
		}
		float width = firstRunTutorialRootRect.rect.width > 1f ? firstRunTutorialRootRect.rect.width : 1080f;
		float height = firstRunTutorialRootRect.rect.height > 1f ? firstRunTutorialRootRect.rect.height : 1920f;
		float scale = Mathf.Clamp((width - 54f) / 1000f, 0.82f, 1f);
		float cardY = Mathf.Clamp(height * 0.14f, 225f, 285f);
		if (firstRunTutorialCardRect != null)
		{
			firstRunTutorialCardRect.localScale = Vector3.one * scale;
			firstRunTutorialCardRect.anchoredPosition = new Vector2(0f, cardY);
		}
		if (firstRunTutorialCardGlowRect != null)
		{
			firstRunTutorialCardGlowRect.localScale = Vector3.one * scale;
			firstRunTutorialCardGlowRect.anchoredPosition = new Vector2(0f, cardY);
		}
		if (firstRunTutorialSkipRect != null)
		{
			firstRunTutorialSkipRect.localScale = Vector3.one * Mathf.Min(1f, scale + 0.05f);
			firstRunTutorialSkipRect.anchoredPosition = new Vector2(0f, cardY - 198f * scale);
		}
	}

	private void SetFirstRunTutorialHand(bool visible, TextAnchor anchor, Vector2 position, float angle, float size)
	{
		if (firstRunTutorialHandImage == null)
		{
			return;
		}
		bool wasVisible = firstRunTutorialHandImage.gameObject.activeSelf;
		firstRunTutorialHandImage.gameObject.SetActive(visible);
		if (!visible)
		{
			return;
		}
		ApplyAnchor(firstRunTutorialHandRect, anchor);
		firstRunTutorialHandBasePosition = position;
		firstRunTutorialHandBaseAngle = angle;
		firstRunTutorialHandTargetSize = size;
		firstRunTutorialHandVelocity = Vector2.zero;
		firstRunTutorialHandAngleVelocity = 0f;
		firstRunTutorialHandSizeVelocity = 0f;
		firstRunTutorialHandAlpha = 0f;
		firstRunTutorialHandImage.color = new Color(1f, 1f, 1f, 0f);
		firstRunTutorialHandRect.anchoredPosition = position + new Vector2(0f, wasVisible ? 34f : 58f);
		firstRunTutorialHandRect.localRotation = Quaternion.Euler(0f, 0f, angle - 8f);
		firstRunTutorialHandSize = size * 0.84f;
		firstRunTutorialHandRect.sizeDelta = Vector2.one * firstRunTutorialHandSize;
		firstRunTutorialHandRect.localScale = Vector3.one;
	}

	private void PointFirstRunTutorialHandAtButton(RectTransform target)
	{
		if (target == null)
		{
			return;
		}
		Vector2 corner = target.anchoredPosition + new Vector2(-(target.sizeDelta.x * 0.5f), target.sizeDelta.y * 0.5f);
		bool beam = target == highBeamHudButtonRect;
		SetFirstRunTutorialHand(true, TextAnchor.LowerRight, corner + new Vector2(beam ? 84f : -84f, target.sizeDelta.y * 0.5f + 142f), beam ? 158f : 202f, 165f);
	}

	private void UpdateFirstRunTutorialProgress(int stage)
	{
		int completed = stage >= 6 ? 5 : Mathf.Clamp(stage - 1, 0, 5);
		for (int i = 0; i < firstRunTutorialProgressDots.Length; i++)
		{
			if (firstRunTutorialProgressDots[i] == null)
			{
				continue;
			}
			bool done = i < completed;
			bool current = stage >= 1 && stage <= 5 && i == stage - 1;
			firstRunTutorialProgressDots[i].color = done ? new Color(0.82f, 0.52f, 1f, 1f) : (current ? new Color(1f, 0.92f, 1f, 1f) : new Color(0.34f, 0.25f, 0.45f, 0.88f));
			firstRunTutorialProgressDots[i].rectTransform.localScale = current ? Vector3.one * 1.35f : Vector3.one;
		}
	}

	private void CopyFirstRunTutorialTarget(RectTransform target, Color color)
	{
		if (target == null)
		{
			return;
		}
		firstRunTutorialHighlight.anchorMin = target.anchorMin;
		firstRunTutorialHighlight.anchorMax = target.anchorMax;
		firstRunTutorialHighlight.pivot = target.pivot;
		firstRunTutorialHighlight.anchoredPosition = target.anchoredPosition + new Vector2(0f, -7f);
		firstRunTutorialHighlight.sizeDelta = target.sizeDelta + new Vector2(34f, 30f);
		if (firstRunTutorialTargetCore != null)
		{
			firstRunTutorialTargetCore.sizeDelta = target.sizeDelta + new Vector2(16f, 12f);
			firstRunTutorialTargetCore.GetComponent<Image>().color = new Color(0.94f, 0.8f, 1f, 0.17f);
		}
		firstRunTutorialHighlightImage.color = color;
	}

	private void SetFirstRunTutorialHighlightArea(Vector2 position, Vector2 size, TextAnchor anchor, Color color)
	{
		ApplyAnchor(firstRunTutorialHighlight, anchor);
		firstRunTutorialHighlight.anchoredPosition = position;
		if (firstRunTutorialRootRect != null && firstRunTutorialRootRect.rect.width > 1f)
		{
			size.x = Mathf.Min(size.x, firstRunTutorialRootRect.rect.width - 30f);
		}
		firstRunTutorialHighlight.sizeDelta = size;
		if (firstRunTutorialTargetCore != null)
		{
			firstRunTutorialTargetCore.sizeDelta = size - new Vector2(20f, 20f);
			float coreAlpha = size.y > 600f ? 0.055f : 0.13f;
			firstRunTutorialTargetCore.GetComponent<Image>().color = new Color(0.94f, 0.8f, 1f, coreAlpha);
		}
		firstRunTutorialHighlightImage.color = color;
	}

	private void UpdateFirstRunTutorial(float unscaledDt)
	{
		if (!firstRunTutorialActive)
		{
			return;
		}
		if (FirstRunTutorialDoubleTapRequested())
		{
			CompleteFirstRunTutorial();
			return;
		}
		firstRunTutorialStageTime += Mathf.Min(unscaledDt, 0.1f);
		firstRunTutorialVisibleTime += Mathf.Min(unscaledDt, 0.1f);
		float pulse = 1f + Mathf.Sin(Time.unscaledTime * 5.2f) * 0.035f;
		firstRunTutorialHighlight.localScale = Vector3.one * pulse;
		UpdateFirstRunTutorialHandAnimation(unscaledDt);
		firstRunTutorialGroup.alpha = Mathf.Clamp01(firstRunTutorialVisibleTime * 4f);
		if (firstRunTutorialCopyGroup != null)
		{
			firstRunTutorialCopyGroup.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(firstRunTutorialStageTime / 0.24f));
		}
		if (Time.unscaledTime < firstRunTutorialInputLockUntil)
		{
			return;
		}
		if (firstRunTutorialStage == 1)
		{
			if (IsGameplayAccelerationHeld())
			{
				if (!firstRunTutorialAccelerationHoldLearned)
				{
					firstRunTutorialInputTime += unscaledDt;
					if (firstRunTutorialInputTime >= 0.38f)
					{
						firstRunTutorialAccelerationHoldLearned = true;
					}
				}
				else if (Mathf.Abs(ReadFirstRunTutorialAccelerationDrag()) >= 0.12f)
				{
					firstRunTutorialSecondaryInputTime += unscaledDt;
				}
				else
				{
					firstRunTutorialSecondaryInputTime = 0f;
				}
			}
			else
			{
				firstRunTutorialInputTime = 0f;
				firstRunTutorialSecondaryInputTime = 0f;
				firstRunTutorialAccelerationHoldLearned = false;
			}
		}
		else if (firstRunTutorialStage == 3)
		{
			// original: bool braking = brakeButtonHeld;
			bool braking = true; /* [InfernoLobby patch] */
			firstRunTutorialInputTime = braking ? firstRunTutorialInputTime + unscaledDt : 0f;
		}
		else if (firstRunTutorialStage == 4)
		{
			firstRunTutorialInputTime = touchSteerLeftPointers.Count > touchSteerRightPointers.Count ? firstRunTutorialInputTime + unscaledDt : 0f;
		}
		else if (firstRunTutorialStage == 5)
		{
			firstRunTutorialInputTime = touchSteerRightPointers.Count > touchSteerLeftPointers.Count ? firstRunTutorialInputTime + unscaledDt : 0f;
		}
		if (firstRunTutorialStage == 1 && firstRunTutorialSecondaryInputTime >= 0.16f)
		{
			SetFirstRunTutorialStage(2);
			return;
		}
		if (firstRunTutorialStage == 3 && firstRunTutorialInputTime >= 0.42f)
		{
			SetFirstRunTutorialStage(4);
			return;
		}
		if (firstRunTutorialStage == 4 && firstRunTutorialInputTime >= 0.18f)
		{
			SetFirstRunTutorialStage(5);
			return;
		}
		if (firstRunTutorialStage == 5 && firstRunTutorialInputTime >= 0.18f)
		{
			SetFirstRunTutorialStage(6);
			return;
		}
		if (firstRunTutorialStage == 6 && firstRunTutorialStageTime >= 1.05f)
		{
			CompleteFirstRunTutorial();
			return;
		}
		if (firstRunTutorialStage >= 1 && firstRunTutorialStage <= 5 && firstRunTutorialStageTime >= 25f)
		{
			SetFirstRunTutorialStage(firstRunTutorialStage + 1);
		}
	}

	private void UpdateFirstRunTutorialHandAnimation(float unscaledDt)
	{
		if (firstRunTutorialHandRect == null || !firstRunTutorialHandRect.gameObject.activeSelf)
		{
			return;
		}
		float dt = Mathf.Min(unscaledDt, 0.05f);
		float cycle = Mathf.Repeat(Time.unscaledTime, 1.32f) / 1.32f;
		float press;
		if (cycle < 0.22f)
		{
			press = Mathf.SmoothStep(0f, 1f, cycle / 0.22f);
		}
		else if (cycle < 0.48f)
		{
			press = 1f - Mathf.SmoothStep(0f, 1f, (cycle - 0.22f) / 0.26f);
		}
		else
		{
			press = 0f;
		}
		Vector2 targetPosition = firstRunTutorialHandBasePosition;
		float targetAngle = firstRunTutorialHandBaseAngle;
		if (firstRunTutorialStage == 1)
		{
			float period = 2.15f;
			float phase = Mathf.Repeat(Time.unscaledTime, period) / period;
			float hold = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.04f, 0.18f, phase)) * (1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.82f, 0.96f, phase)));
			float slide = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.28f, 0.68f, phase)) * (1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.8f, 0.96f, phase)));
			float direction = (Mathf.FloorToInt(Time.unscaledTime / period) & 1) != 0 ? -1f : 1f;
			targetPosition += new Vector2(direction * 125f * slide, hold * 28f);
			targetAngle = firstRunTutorialHandBaseAngle + direction * slide * 4f;
			press = hold;
		}
		else if (firstRunTutorialStage == 4 || firstRunTutorialStage == 5)
		{
			targetPosition += new Vector2(0f, press * 24f);
		}
		firstRunTutorialHandRect.anchoredPosition = Vector2.SmoothDamp(firstRunTutorialHandRect.anchoredPosition, targetPosition, ref firstRunTutorialHandVelocity, 0.15f, float.PositiveInfinity, dt);
		float angle = firstRunTutorialHandRect.localEulerAngles.z;
		angle = Mathf.SmoothDampAngle(angle, targetAngle, ref firstRunTutorialHandAngleVelocity, 0.16f, float.PositiveInfinity, dt);
		firstRunTutorialHandRect.localRotation = Quaternion.Euler(0f, 0f, angle);
		float targetSize = firstRunTutorialHandTargetSize * Mathf.Lerp(0.965f, 1.025f, press);
		firstRunTutorialHandSize = Mathf.SmoothDamp(firstRunTutorialHandSize, targetSize, ref firstRunTutorialHandSizeVelocity, 0.14f, float.PositiveInfinity, dt);
		firstRunTutorialHandRect.sizeDelta = Vector2.one * firstRunTutorialHandSize;
		firstRunTutorialHandAlpha = Mathf.MoveTowards(firstRunTutorialHandAlpha, 1f, dt * 5.5f);
		firstRunTutorialHandImage.color = new Color(1f, 1f, 1f, firstRunTutorialHandAlpha);
	}

	private bool FirstRunTutorialDoubleTapRequested()
	{
		if (Time.unscaledTime < firstRunTutorialTapIgnoreUntil)
		{
			return false;
		}
		Touchscreen touchscreen = Touchscreen.current;
		if (touchscreen != null)
		{
			foreach (TouchControl touch in touchscreen.touches)
			{
				if (touch.press.wasPressedThisFrame && RegisterFirstRunTutorialTap())
				{
					return true;
				}
			}
		}
		Mouse mouse = Mouse.current;
		if (mouse != null && mouse.leftButton.wasPressedThisFrame)
		{
			return RegisterFirstRunTutorialTap();
		}
		return false;
	}

	private bool RegisterFirstRunTutorialTap()
	{
		float now = Time.unscaledTime;
		bool doubleTap = firstRunTutorialLastTapFrame >= 0 && firstRunTutorialLastTapFrame != Time.frameCount && now - firstRunTutorialLastTapTime <= 0.36f;
		firstRunTutorialLastTapTime = now;
		firstRunTutorialLastTapFrame = Time.frameCount;
		return doubleTap;
	}

	private void NotifyFirstRunTutorialBeamPressed()
	{
		if (!firstRunTutorialActive || firstRunTutorialStage != 2 || Time.unscaledTime < firstRunTutorialInputLockUntil)
		{
			return;
		}
		SetFirstRunTutorialStage(3);
	}

	private float ReadFirstRunTutorialAccelerationDrag()
	{
		if (Touchscreen.current != null)
		{
			return ReadAccelerationDragSteering();
		}
		Mouse mouse = Mouse.current;
		if (mouse == null || !mouse.leftButton.isPressed)
		{
			firstRunTutorialMouseDragTracking = false;
			return 0f;
		}
		float x = mouse.position.ReadValue().x;
		if (!firstRunTutorialMouseDragTracking)
		{
			firstRunTutorialMouseDragTracking = true;
			firstRunTutorialMouseDragStartX = x;
			return 0f;
		}
		float deadZone = Mathf.Max(18f, Screen.width * 0.035f);
		float fullRange = Mathf.Max(deadZone + 1f, Screen.width * 0.2f);
		float delta = x - firstRunTutorialMouseDragStartX;
		if (Mathf.Abs(delta) <= deadZone)
		{
			return 0f;
		}
		return Mathf.Sign(delta) * Mathf.InverseLerp(deadZone, fullRange, Mathf.Abs(delta));
	}
}
