using System.Threading.Tasks;
using ShadowRun.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Translated from Il2CppOutput Assembly-CSharp.cpp / Assembly-CSharp__1.cpp:
//   ShadowRunPrototype_MakeSettingsScreen_m29352590532697CF636FDE3DEE5E04C4D80ACC59
//   ShadowRunPrototype_U3CMakeSettingsScreenU3Eb__477_0 / b__477_1
//   ShadowRunPrototype_MakeProfilePanel_m1F325FC75C589FE13B6DDC3F6C140E6B300E353D
//   ShadowRunPrototype_MakeProfileActionButton_m6BF45886CC6E1CD20A1B8782BDA15DBB6EC3FE54
//   ShadowRunPrototype_MakeProfileArtworkAuthButton_mA9A99E019B7851D639087B858EE442A0885D15DD
//   ShadowRunPrototype_MakeProfileInput_mC8CE17CD7DCC688F37134E9C760AC9EED7273EA4
//   ShadowRunPrototype_BuildProfileAuthForm_m8F584E1D44567794BDEE14D2100A976E4E2DCFF8
//   ShadowRunPrototype_U3CBuildProfileAuthFormU3Eb__482_0 / _1 / _2
//   ShadowRunPrototype_BuildSignedInProfileActions_m034881BB08D7897A946FB5BF09C96544545B4662
//   ShadowRunPrototype_LayoutProfileScreen_m74A28E9897F67E5561A54EF5927CF2BE48A27F27
//   ShadowRunPrototype_RefreshProfileUi_m489C1E132BE5E56F7796DF07D4147822A38FD62B
//   ShadowRunPrototype_ShowProfileAuthForm_mF1B89FE60D58E18F2FA8294A07510F22F3BF6F42
//   ShadowRunPrototype_HideProfileAuthForm_m608653401950EB3EB55554E940AECB63183E97AB
//   U3CRunProfileAuthAsyncU3Ed__488_MoveNext_m97C572D587DFE2C5B3E04DFEBA0E2D4A402F70B5
//   U3CRunProfileOAuthAsyncU3Ed__489_MoveNext_mD180A59F2AA1212D139583B2283DFD2571EFF2E7
//   ShadowRunPrototype_RequestProfileSignOutConfirmation_mE063ACEF685B8F32C58D7F8A226DDF6BCF18973F
//   ShadowRunPrototype_RequestProfileDeleteConfirmation_mBEC994913BAE1E58C2DDB00604D6C26DC8705E1C
//   ShadowRunPrototype_OnNativeProfileSignOutConfirmed_mE166DC7837E8CD8D1039B7559851475C01DF4291
//   ShadowRunPrototype_OnNativeProfileDeleteConfirmed_mCAA0DC543DA01E517762B5B8E43C40AFCF157F47
//   U3CRunProfileSignOutAsyncU3Ed__494_MoveNext_m541C2B84A9C113B704307708DAF4B84A5300E38E
//   U3CRunProfileDeleteAsyncU3Ed__495_MoveNext_m81E666337066F4F8A3413DF72E6D1254F614D38E
//   ShadowRunPrototype_SetProfileActionsInteractable_m525E52DEE3F8B41D6B0993E2E7A5F52D00248B7F
//   ShadowRunPrototype_SetProfileAuthenticationStatus_m6AFB459607C1C9350702B05B0E3513F872BCB8F2
//   ShadowRunPrototype_MakeSettingsSlider_m56A2386E93119327B2A8DD6B095387A7B0747992
//   ShadowRunPrototype_MakeSettingsStepper_m1B3FE9776334CE2F07EB75FA19CC86F05388929C
//   ShadowRunPrototype_MakeSettingsToggle_m894F23798BF95E82985585FB8EE52081DABE608C
//   ShadowRunPrototype_MakeSettingsLabel_m3EEF716DC1BCF486BEA0346D4433C5F102F3A659
//   ShadowRunPrototype_MakeSettingsAction_mBF1DF40B4110EA830147CD4ACDE5709761E8F056
//   ShadowRunPrototype_MakeSettingsArrow_m6E5F7F3A64FAAB2D5891EC6DDCA4840CBBA61859
//   ShadowRunPrototype_MakeSettingsClickArea_m06D915BF07A5D5AC0B3E38FD6A72517CE1BF834A
//   ShadowRunPrototype_ShowSettingsScreen_m39FBEF63EA5896916CF7E10C818E19B0F7C915C4
//   ShadowRunPrototype_HideSettingsScreen_m908769C2CFC29B0618F57099CAA7D82FC1388C47
//   ShadowRunPrototype_UpdateSettingsUi_mBE2FCBE25BF5E6A7C1962C0CD2BF8B86B3678313
//   ShadowRunPrototype_UpdateToggle_m42A6E1839DB742CD55DE0238E82F10056157810B
//   ShadowRunPrototype_ApplyGarageOwnershipFromProfile_mBC09E4C1A609F538C8F2B3804A54158AC098FB2B
public sealed partial class ShadowRunPrototype
{
	private void MakeSettingsScreen(Transform parent)
	{
		settingsRoot = new GameObject("Settings Screen");
		settingsRoot.transform.SetParent(parent, false);
		RectTransform rootRect = settingsRoot.AddComponent<RectTransform>();
		rootRect.anchorMin = Vector2.zero;
		rootRect.anchorMax = Vector2.one;
		rootRect.offsetMin = Vector2.zero;
		rootRect.offsetMax = Vector2.zero;
		Image background = settingsRoot.AddComponent<Image>();
		// original: new Color(0.004f, 0.002f, 0.012f, 1f)
		background.color = new Color(0f, 0f, 0f, 1f); /* [InfernoLobby patch] */
		background.raycastTarget = true;
		RectTransform viewport = new GameObject("Profile Background Viewport", typeof(RectTransform), typeof(RectMask2D)).GetComponent<RectTransform>();
		viewport.SetParent(settingsRoot.transform, false);
		viewport.anchorMin = Vector2.zero;
		viewport.anchorMax = Vector2.one;
		viewport.offsetMin = Vector2.zero;
		viewport.offsetMax = Vector2.zero;
		Image art = new GameObject("Profile Background Art", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
		art.transform.SetParent(viewport, false);
		art.sprite = LoadResourceSprite("фон профиль");
		art.gameObject.SetActive(false); /* [InfernoLobby patch] */
		art.color = Color.white;
		art.preserveAspect = false;
		art.raycastTarget = false;
		RectTransform artRect = art.rectTransform;
		artRect.anchorMin = Vector2.zero;
		artRect.anchorMax = Vector2.one;
		artRect.offsetMin = Vector2.zero;
		artRect.offsetMax = Vector2.zero;
		AspectRatioFitter fitter = art.gameObject.AddComponent<AspectRatioFitter>();
		fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
		fitter.aspectRatio = art.sprite != null ? art.sprite.rect.width / Mathf.Max(1f, art.sprite.rect.height) : 0.462581336f;
		Image ambient = MakeUiBlock(settingsRoot.transform, "Profile Ambient Glow", new Vector2(0f, 160f), new Vector2(1040f, 1180f), TextAnchor.MiddleCenter, new Color(0.26f, 0.055f, 0.48f, 0.1f));
		ambient.sprite = caseRadialGlowSprite;
		ambient.gameObject.SetActive(false); /* [InfernoLobby patch] */
		ambient.raycastTarget = false;
		profilePageHeaderRoot = MakeMenuPageHeader(settingsRoot.transform, "PROFILE", "CLOUD IDENTITY  •  LIVE GARAGE SYNC", out Text subtitle);
		subtitle.gameObject.SetActive(false); /* [InfernoLobby patch] */
		profileContentRoot = new GameObject("Profile Adaptive Content").AddComponent<RectTransform>();
		profileContentRoot.SetParent(settingsRoot.transform, false);
		profileContentRoot.anchorMin = Vector2.zero;
		profileContentRoot.anchorMax = Vector2.one;
		profileContentRoot.offsetMin = Vector2.zero;
		profileContentRoot.offsetMax = Vector2.zero;
		Image identityCard = MakeProfilePanel(profileContentRoot, "Identity Card", new Vector2(0f, -280f), new Vector2(920f, 190f), new Color(0.018f, 0.01f, 0.036f, 0.92f), Color.clear, false);
		Image accent = MakeUiBlock(identityCard.transform, "Identity Accent", new Vector2(-448f, 0f), new Vector2(5f, 96f), TextAnchor.MiddleCenter, new Color(0.72f, 0.25f, 1f, 0.92f));
		accent.raycastTarget = false;
		profileSoundButton = MakeSoundToggleButton(identityCard.transform, "Profile Sound Toggle", TextAnchor.MiddleCenter, new Vector2(260f, 0f), new Vector2(78f, 78f), out profileSoundIcon);
		profileSoundButton.gameObject.SetActive(false); /* [InfernoLobby patch] */
		profileSoundButton.onClick.AddListener(ToggleGameSound);
		Image avatarGlow = MakeUiBlock(identityCard.transform, "Avatar Glow", new Vector2(-330f, 0f), new Vector2(132f, 132f), TextAnchor.MiddleCenter, new Color(0.58f, 0.2f, 1f, 0.15f));
		avatarGlow.sprite = caseRadialGlowSprite;
		avatarGlow.raycastTarget = false;
		Image avatar = MakeUiBlock(identityCard.transform, "Avatar", new Vector2(-330f, 0f), new Vector2(88f, 88f), TextAnchor.MiddleCenter, Color.white);
		avatar.sprite = LoadTightMenuArtSprite("айди кнопка");
		avatar.type = Image.Type.Simple;
		avatar.preserveAspect = true;
		avatar.gameObject.SetActive(false); /* [InfernoLobby patch] */
		avatar.raycastTarget = false;
		profileAccountTypeText = MakeText(identityCard.transform, "GUEST ACCOUNT", 29, TextAnchor.MiddleLeft, new Vector2(220f, 22f));
		profileAccountTypeText.fontStyle = FontStyle.Bold;
		profileAccountTypeText.color = new Color(0.7f, 0.42f, 1f, 1f);
		profileAccountTypeText.GetComponent<RectTransform>().sizeDelta = new Vector2(670f, 46f);
		profileAccountTypeText.raycastTarget = false;
		profileIdentityText = MakeText(identityCard.transform, "CONNECTING", 21, TextAnchor.MiddleLeft, new Vector2(220f, -29f));
		profileIdentityText.fontStyle = FontStyle.Normal;
		profileIdentityText.color = new Color(0.82f, 0.76f, 0.9f, 0.96f);
		profileIdentityText.resizeTextForBestFit = true;
		profileIdentityText.resizeTextMinSize = 16;
		profileIdentityText.resizeTextMaxSize = 21;
		profileIdentityText.GetComponent<RectTransform>().sizeDelta = new Vector2(670f, 36f);
		profileIdentityText.raycastTarget = false;
		profileSyncText = MakeText(identityCard.transform, "", 1, TextAnchor.MiddleLeft, Vector2.zero);
		profileSyncText.color = Color.clear;
		profileSyncText.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		profileSyncText.raycastTarget = false;
		profileSyncText.gameObject.SetActive(false);
		profileGuestRoot = new GameObject("Guest Account Actions");
		profileGuestRoot.transform.SetParent(profileContentRoot, false);
		RectTransform guestRect = profileGuestRoot.AddComponent<RectTransform>();
		ApplyAnchor(guestRect, TextAnchor.UpperCenter);
		guestRect.anchoredPosition = new Vector2(0f, -510f);
		guestRect.sizeDelta = new Vector2(920f, 172f);
		Image guestPanel = MakeProfilePanel(guestRect, "Guest Actions", Vector2.zero, guestRect.sizeDelta, new Color(0.018f, 0.01f, 0.036f, 0.84f), Color.clear, false);
		guestPanel.gameObject.SetActive(false); /* [InfernoLobby patch] */
		profileSignUpButton = MakeProfileActionButton(guestPanel.transform, "CREATE ACCOUNT", new Vector2(-202f, 0f), new Vector2(386f, 82f), true, false, out _, true, true);
		((RectTransform)profileSignUpButton.transform).anchoredPosition = new Vector2(0f, 0f); /* [InfernoLobby patch] */
		profileSignUpButton.onClick.AddListener(() => ShowProfileAuthForm(true));
		profileLoginButton = MakeProfileActionButton(guestPanel.transform, "LOG IN", new Vector2(202f, 0f), new Vector2(386f, 82f), false, false, out _, true, true);
		profileLoginButton.transform.SetParent(guestRect, false); /* [InfernoLobby patch] */
		profileLoginButton.gameObject.SetActive(false); /* [InfernoLobby patch] */
		profileLoginButton.onClick.AddListener(() => ShowProfileAuthForm(false));
		BuildProfileAuthForm();
		BuildSignedInProfileActions();
		profileStatusText = MakeText(profileContentRoot, "", 17, TextAnchor.UpperCenter, new Vector2(0f, -710f));
		profileStatusText.color = new Color(0.66f, 0.58f, 0.78f, 0.94f);
		profileStatusText.resizeTextForBestFit = true;
		profileStatusText.resizeTextMinSize = 14;
		profileStatusText.resizeTextMaxSize = 18;
		profileStatusText.GetComponent<RectTransform>().sizeDelta = new Vector2(860f, 58f);
		profileStatusText.raycastTarget = false;
		settingsRoot.SetActive(false);
		LayoutProfileScreen();
		RefreshProfileUi();
		InfernoSettings.Build(this); /* [InfernoLobby patch] */
		InfernoSettings.StyleAccount(this, identityCard); /* [InfernoLobby patch] */
	}

	private Image MakeProfilePanel(Transform parent, string name, Vector2 pos, Vector2 size, Color fill, Color borderColor, bool useGeneratedOutline = true)
	{
		Image panel = MakeUiBlock(parent, name, pos, size, TextAnchor.UpperCenter, fill);
		panel.sprite = roundedGlassSprite;
		panel.type = Image.Type.Sliced;
		panel.raycastTarget = false;
		if (useGeneratedOutline)
		{
			Outline outline = panel.gameObject.AddComponent<Outline>();
			outline.effectColor = borderColor;
			outline.effectDistance = new Vector2(2f, -2f);
			outline.useGraphicAlpha = true;
		}
		return panel;
	}

	private Button MakeProfileActionButton(Transform parent, string label, Vector2 pos, Vector2 size, bool primary, bool danger, out Text labelText, bool neonContour = false, bool addNeonGlow = true)
	{
		if (neonContour && addNeonGlow)
		{
			Image glow = MakeUiBlock(parent, label + " Neon Glow", pos, size + new Vector2(14f, 14f), TextAnchor.MiddleCenter, danger ? new Color(1f, 0.04f, 0.16f, 0.22f) : new Color(0.52f, 0.08f, 0.96f, 0.24f));
			glow.sprite = roundedGlassSprite;
			glow.type = Image.Type.Sliced;
			glow.raycastTarget = false;
		}
		Button button = new GameObject(label + " Profile Button").AddComponent<Button>();
		button.transform.SetParent(parent, false);
		Image background = button.gameObject.AddComponent<Image>();
		background.sprite = roundedGlassSprite;
		background.type = Image.Type.Sliced;
		background.color = neonContour
			? (danger ? new Color(1f, 0.055f, 0.18f, 1f) : new Color(0.76f, 0.2f, 1f, 1f))
			: (danger ? new Color(0.16f, 0.02f, 0.05f, 0.96f) : (primary ? new Color(0.36f, 0.075f, 0.62f, 1f) : new Color(0.05f, 0.026f, 0.082f, 0.98f)));
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.MiddleCenter);
		rect.anchoredPosition = pos;
		rect.sizeDelta = size;
		if (neonContour)
		{
			Image fill = MakeUiBlock(button.transform, "Button Dark Purple Fill", Vector2.zero, size - new Vector2(8f, 8f), TextAnchor.MiddleCenter, danger ? new Color(0.115f, 0.008f, 0.026f, 1f) : new Color(0.06f, 0.014f, 0.105f, 1f));
			fill.sprite = roundedGlassSprite;
			fill.type = Image.Type.Sliced;
			fill.raycastTarget = false;
			button.targetGraphic = fill;
		}
		else
		{
			button.targetGraphic = background;
			Image accent = MakeUiBlock(button.transform, "Button Accent", new Vector2(0f, size.y * 0.5f - 2f), new Vector2(size.x - 38f, 3f), TextAnchor.MiddleCenter, danger ? new Color(1f, 0.18f, 0.34f, 0.72f) : new Color(0.78f, 0.34f, 1f, primary ? 0.92f : 0.38f));
			accent.raycastTarget = false;
		}
		labelText = MakeText(button.transform, label, 22, TextAnchor.MiddleCenter, Vector2.zero);
		labelText.fontStyle = FontStyle.Bold;
		labelText.color = danger ? new Color(1f, 0.72f, 0.78f, 1f) : Color.white;
		labelText.resizeTextForBestFit = true;
		labelText.resizeTextMinSize = 15;
		labelText.resizeTextMaxSize = 22;
		labelText.GetComponent<RectTransform>().sizeDelta = size - new Vector2(24f, 12f);
		labelText.raycastTarget = false;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.12f, 1.06f, 1.16f, 1f);
		colors.pressedColor = new Color(0.7f, 0.62f, 0.8f, 1f);
		colors.fadeDuration = 0.06f;
		button.colors = colors;
		return button;
	}

	private Button MakeProfileArtworkAuthButton(Transform parent, string name, string resource, Vector2 pos, Vector2 size)
	{
		Image image = MakeUiBlock(parent, name, pos, size, TextAnchor.MiddleCenter, Color.white);
		image.sprite = LoadTightMenuArtSprite(resource);
		image.type = Image.Type.Simple;
		image.preserveAspect = false;
		image.raycastTarget = true;
		Button button = image.gameObject.AddComponent<Button>();
		button.targetGraphic = image;
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white;
		colors.highlightedColor = new Color(1.06f, 1.06f, 1.06f, 1f);
		colors.pressedColor = new Color(0.8f, 0.74f, 0.88f, 1f);
		colors.disabledColor = new Color(0.42f, 0.42f, 0.46f, 0.7f);
		colors.fadeDuration = 0.08f;
		button.colors = colors;
		return button;
	}

	private InputField MakeProfileInput(Transform parent, string name, string placeholderValue, Vector2 pos, bool password)
	{
		Image background = MakeUiBlock(parent, name + " Input", pos, new Vector2(790f, 82f), TextAnchor.MiddleCenter, new Color(0.028f, 0.014f, 0.052f, 1f));
		background.sprite = roundedGlassSprite;
		background.type = Image.Type.Sliced;
		Image accent = MakeUiBlock(background.transform, "Input Accent", new Vector2(-393f, 0f), new Vector2(3f, 50f), TextAnchor.MiddleCenter, new Color(0.66f, 0.28f, 0.94f, 0.7f));
		accent.raycastTarget = false;
		InputField input = background.gameObject.AddComponent<InputField>();
		input.targetGraphic = background;
		input.contentType = password ? InputField.ContentType.Password : InputField.ContentType.EmailAddress;
		Text text = MakeText(background.transform, "", 24, TextAnchor.MiddleLeft, new Vector2(24f, 0f));
		text.color = Color.white;
		text.raycastTarget = true;
		text.GetComponent<RectTransform>().sizeDelta = new Vector2(730f, 76f);
		input.textComponent = text;
		Text placeholder = MakeText(background.transform, placeholderValue, 22, TextAnchor.MiddleLeft, new Vector2(24f, 0f));
		placeholder.color = new Color(0.44f, 0.38f, 0.54f, 1f);
		placeholder.GetComponent<RectTransform>().sizeDelta = new Vector2(730f, 76f);
		input.placeholder = placeholder;
		return input;
	}

	private void BuildProfileAuthForm()
	{
		profileAuthFormRoot = new GameObject("Profile Auth Form");
		profileAuthFormRoot.transform.SetParent(profileContentRoot, false);
		RectTransform rect = profileAuthFormRoot.AddComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.UpperCenter);
		rect.anchoredPosition = new Vector2(0f, -605f);
		rect.sizeDelta = new Vector2(920f, 680f);
		Image card = MakeProfilePanel(rect, "Auth Form Card", Vector2.zero, rect.sizeDelta, new Color(0.018f, 0.01f, 0.036f, 0.96f), Color.clear, false);
		profileAuthTitleText = MakeText(card.transform, "SECURE GUEST ACCOUNT", 29, TextAnchor.MiddleCenter, new Vector2(0f, 286f));
		profileAuthTitleText.fontStyle = FontStyle.Bold;
		profileAuthTitleText.color = Color.white;
		profileAuthTitleText.GetComponent<RectTransform>().sizeDelta = new Vector2(760f, 54f);
		Button apple = MakeProfileArtworkAuthButton(card.transform, "Apple Authorization", "эпл", new Vector2(0f, 202f), new Vector2(520f, 108f));
		apple.onClick.AddListener(() => { _ = RunProfileOAuthAsync(true); });
		Button google = MakeProfileArtworkAuthButton(card.transform, "Google Authorization", "гугл", new Vector2(0f, 91f), new Vector2(520f, 104f));
		google.onClick.AddListener(() => { _ = RunProfileOAuthAsync(false); });
		Text divider = MakeText(card.transform, "—   OR USE EMAIL   —", 16, TextAnchor.MiddleCenter, new Vector2(0f, 20f));
		divider.color = new Color(0.58f, 0.48f, 0.68f, 0.92f);
		divider.GetComponent<RectTransform>().sizeDelta = new Vector2(760f, 30f);
		divider.raycastTarget = false;
		profileEmailInput = MakeProfileInput(card.transform, "Profile Email", "EMAIL", new Vector2(0f, -57f), false);
		profilePasswordInput = MakeProfileInput(card.transform, "Profile Password", "PASSWORD  •  6+ CHARACTERS", new Vector2(0f, -151f), true);
		profileAuthSubmitButton = MakeProfileActionButton(card.transform, "CREATE ACCOUNT", new Vector2(-205f, -257f), new Vector2(370f, 82f), true, false, out profileAuthSubmitText, true, true);
		profileAuthSubmitButton.onClick.AddListener(() => { _ = RunProfileAuthAsync(); });
		Button cancel = MakeProfileActionButton(card.transform, "CANCEL", new Vector2(205f, -257f), new Vector2(370f, 82f), false, false, out _, true, true);
		cancel.onClick.AddListener(HideProfileAuthForm);
		profileAuthFormRoot.SetActive(false);
	}

	private void BuildSignedInProfileActions()
	{
		profileAccountRoot = new GameObject("Signed Account Actions");
		profileAccountRoot.transform.SetParent(profileContentRoot, false);
		RectTransform rect = profileAccountRoot.AddComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.UpperCenter);
		rect.anchoredPosition = new Vector2(0f, -605f);
		rect.sizeDelta = new Vector2(920f, 390f);
		Image card = MakeProfilePanel(rect, "Account Management Card", Vector2.zero, rect.sizeDelta, new Color(0.018f, 0.01f, 0.036f, 0.94f), Color.clear, false);
		Text heading = MakeText(card.transform, "ACCOUNT", 21, TextAnchor.MiddleLeft, new Vector2(90f, 126f));
		heading.fontStyle = FontStyle.Bold;
		heading.color = new Color(0.66f, 0.48f, 0.86f, 1f);
		heading.GetComponent<RectTransform>().sizeDelta = new Vector2(740f, 38f);
		profileEmailText = MakeText(card.transform, "", 32, TextAnchor.MiddleLeft, new Vector2(90f, 72f));
		profileEmailText.fontStyle = FontStyle.Bold;
		profileEmailText.color = Color.white;
		profileEmailText.resizeTextForBestFit = true;
		profileEmailText.resizeTextMinSize = 18;
		profileEmailText.resizeTextMaxSize = 32;
		profileEmailText.GetComponent<RectTransform>().sizeDelta = new Vector2(740f, 56f);
		profileSignOutButton = MakeProfileActionButton(card.transform, "SIGN OUT", new Vector2(-205f, -62f), new Vector2(370f, 90f), false, false, out _, true, false);
		profileSignOutButton.gameObject.SetActive(false); /* [InfernoLobby patch] */
		profileSignOutButton.onClick.AddListener(RequestProfileSignOutConfirmation);
		profileDeleteButton = MakeProfileActionButton(card.transform, "DELETE ACCOUNT", new Vector2(205f, -62f), new Vector2(370f, 90f), false, true, out profileDeleteButtonText, true, false);
		((RectTransform)profileDeleteButton.transform).anchoredPosition = new Vector2(0f, -62f); /* [InfernoLobby patch] */
		profileDeleteButton.onClick.AddListener(RequestProfileDeleteConfirmation);
		Text warning = MakeText(card.transform, "Deletion permanently removes cloud progress and cannot be undone.", 17, TextAnchor.MiddleCenter, new Vector2(0f, -142f));
		warning.color = new Color(0.72f, 0.46f, 0.56f, 0.9f);
		warning.GetComponent<RectTransform>().sizeDelta = new Vector2(790f, 42f);
		warning.raycastTarget = false;
		profileAccountRoot.SetActive(false);
	}

	private void LayoutProfileScreen()
	{
		if (profileContentRoot == null)
		{
			return;
		}
		float height = overlayRect != null && overlayRect.rect.height > 0f ? overlayRect.rect.height : 1920f;
		float topInset = Screen.height > 0 ? (Screen.height - Screen.safeArea.yMax) / Screen.height * height : 0f;
		float bottomInset = Screen.height > 0 ? Screen.safeArea.yMin / Screen.height * height : 0f;
		profileContentRoot.offsetMax = new Vector2(0f, -topInset);
		profileContentRoot.offsetMin = new Vector2(0f, bottomInset + 190f);
		profileContentRoot.localScale = Vector3.one;
		InfernoSettings.Relayout(this); /* [InfernoLobby patch] */
	}

	private void RefreshProfileUi()
	{
		if (profileIdentityText == null)
		{
			return;
		}
		bool authenticated = SupabaseGameManager.IsAuthenticated;
		bool remembered = SupabaseGameManager.HasRememberedAccount;
		bool guest = remembered && SupabaseGameManager.IsRememberedGuestAccount;
		profileIdentityText.text = remembered ? SupabaseGameManager.AccountDisplayId : "NO ACCOUNT";
		profileAccountTypeText.text = guest ? "GUEST ACCOUNT" : (remembered ? "VERIFIED ACCOUNT" : "SIGN IN REQUIRED");
		profileAccountTypeText.color = guest ? new Color(0.7f, 0.42f, 1f, 1f) : (remembered ? new Color(0.3f, 0.9f, 0.72f, 1f) : new Color(0.54f, 0.48f, 0.66f, 1f));
		if (!authenticated && remembered)
		{
			profileSyncText.text = "●  OFFLINE — ACCOUNT SAVED";
			profileSyncText.color = new Color(1f, 0.62f, 0.24f, 0.92f);
		}
		else if (!SupabaseGameManager.IsCloudReady)
		{
			profileSyncText.text = "●  OFFLINE PROGRESS";
			profileSyncText.color = new Color(1f, 0.62f, 0.24f, 0.92f);
		}
		else if (!authenticated)
		{
			profileSyncText.text = "●  RESTORING CLOUD SESSION";
			profileSyncText.color = new Color(0.52f, 0.62f, 0.84f, 0.92f);
		}
		else if (!SupabaseGameManager.ProfileMatchesSession)
		{
			profileSyncText.text = "●  PROFILE ID MISMATCH — SYNCING";
			profileSyncText.color = new Color(1f, 0.32f, 0.46f, 0.96f);
		}
		else
		{
			profileSyncText.text = "●  CLOUD PROFILE SYNCED";
			profileSyncText.color = new Color(0.3f, 0.9f, 0.72f, 0.96f);
		}
		if (profileEmailText != null)
		{
			profileEmailText.text = string.IsNullOrWhiteSpace(SupabaseGameManager.CurrentUserEmail) ? SupabaseGameManager.AccountDisplayId : SupabaseGameManager.CurrentUserEmail;
		}
		bool formOpen = profileAuthFormRoot != null && profileAuthFormRoot.activeSelf;
		if (profileGuestRoot != null)
		{
			profileGuestRoot.SetActive((!remembered || guest) && !formOpen);
		}
		if (profileAccountRoot != null)
		{
			profileAccountRoot.SetActive(remembered && !guest);
		}
		if (remembered && !guest && profileAuthFormRoot != null)
		{
			profileAuthFormRoot.SetActive(false);
		}
	}

	private void ShowProfileAuthForm(bool register)
	{
		profileAuthRegisterMode = register;
		if (profileGuestRoot != null)
		{
			profileGuestRoot.SetActive(false);
		}
		if (profileAuthFormRoot != null)
		{
			profileAuthFormRoot.SetActive(true);
		}
		if (profileAuthTitleText != null)
		{
			profileAuthTitleText.text = register ? "SECURE GUEST ACCOUNT" : "LOGIN TO EXISTING ACCOUNT";
		}
		if (profileAuthSubmitText != null)
		{
			profileAuthSubmitText.text = register ? "CREATE ACCOUNT" : "LOGIN";
		}
		if (profileStatusText != null)
		{
			profileStatusText.text = string.Empty;
		}
	}

	private void HideProfileAuthForm()
	{
		if (profileAuthFormRoot != null)
		{
			profileAuthFormRoot.SetActive(false);
		}
		RefreshProfileUi();
	}

	private async Task RunProfileAuthAsync()
	{
		if (profileAuthBusy || profileEmailInput == null || profilePasswordInput == null)
		{
			return;
		}
		profileAuthBusy = true;
		SetProfileActionsInteractable(false);
		if (profileStatusText != null)
		{
			profileStatusText.text = profileAuthRegisterMode ? "CREATING SECURE ACCOUNT..." : "SIGNING IN...";
		}
		bool success;
		if (profileAuthRegisterMode)
		{
			success = await SupabaseGameManager.RegisterWithEmailAsync(profileEmailInput.text, profilePasswordInput.text);
		}
		else
		{
			success = await SupabaseGameManager.SignInWithEmailAsync(profileEmailInput.text, profilePasswordInput.text);
		}
		profileAuthBusy = false;
		SetProfileActionsInteractable(true);
		if (success)
		{
			profilePasswordInput.text = string.Empty;
			HideProfileAuthForm();
		}
		RefreshProfileUi();
	}

	private async Task RunProfileOAuthAsync(bool apple)
	{
		if (profileAuthBusy)
		{
			return;
		}
		profileAuthBusy = true;
		SetProfileActionsInteractable(false);
		if (profileStatusText != null)
		{
			profileStatusText.text = apple ? "OPENING APPLE SIGN IN..." : "OPENING GOOGLE SIGN IN...";
		}
		if (apple)
		{
			await SupabaseGameManager.StartAppleSignInAsync();
		}
		else
		{
			await SupabaseGameManager.StartGoogleSignInAsync();
		}
		profileAuthBusy = false;
		SetProfileActionsInteractable(true);
		RefreshProfileUi();
	}

	private void RequestProfileSignOutConfirmation()
	{
		if (profileAuthBusy)
		{
			return;
		}
		ShadowRun_ShowAccountConfirmation(0, gameObject.name);
	}

	private void RequestProfileDeleteConfirmation()
	{
		if (profileAuthBusy)
		{
			return;
		}
		ShadowRun_ShowAccountConfirmation(1, gameObject.name);
	}

	public void OnNativeProfileSignOutConfirmed(string message)
	{
		_ = RunProfileSignOutAsync();
	}

	public void OnNativeProfileDeleteConfirmed(string message)
	{
		_ = RunProfileDeleteAsync();
	}

	private async Task RunProfileSignOutAsync()
	{
		if (profileAuthBusy)
		{
			return;
		}
		profileAuthBusy = true;
		SetProfileActionsInteractable(false);
		if (profileStatusText != null)
		{
			profileStatusText.text = "SYNCING AND SIGNING OUT...";
		}
		await SupabaseGameManager.SignOutAsync();
		profileAuthBusy = false;
		SetProfileActionsInteractable(true);
		RefreshProfileUi();
	}

	private async Task RunProfileDeleteAsync()
	{
		if (profileAuthBusy)
		{
			return;
		}
		profileAuthBusy = true;
		SetProfileActionsInteractable(false);
		if (profileStatusText != null)
		{
			profileStatusText.text = "DELETING ACCOUNT...";
		}
		await SupabaseGameManager.DeleteAccountAsync();
		profileAuthBusy = false;
		if (profileDeleteButtonText != null)
		{
			profileDeleteButtonText.text = "DELETE ACCOUNT";
		}
		SetProfileActionsInteractable(true);
		RefreshProfileUi();
	}

	private void SetProfileActionsInteractable(bool value)
	{
		if (profileSignUpButton != null)
		{
			profileSignUpButton.interactable = value;
		}
		if (profileLoginButton != null)
		{
			profileLoginButton.interactable = value;
		}
		if (profileAuthSubmitButton != null)
		{
			profileAuthSubmitButton.interactable = value;
		}
		if (profileSignOutButton != null)
		{
			profileSignOutButton.interactable = value;
		}
		if (profileDeleteButton != null)
		{
			profileDeleteButton.interactable = value;
		}
		if (profileEmailInput != null)
		{
			profileEmailInput.interactable = value;
		}
		if (profilePasswordInput != null)
		{
			profilePasswordInput.interactable = value;
		}
	}

	private void SetProfileAuthenticationStatus(string message)
	{
		if (profileStatusText != null)
		{
			profileStatusText.text = string.Empty;
		}
		RefreshProfileUi();
	}

	private Text MakeSettingsSlider(string label, float y, int value, UnityAction action)
	{
		MakeSettingsLabel(label, y);
		Image line = MakeUiBlock(settingsRoot.transform, label + " Slider Line", new Vector2(438f, y - 18f), new Vector2(300f, 2f), TextAnchor.UpperLeft, new Color(0.7f, 0.02f, 0.05f, 0.72f));
		Image fill = MakeUiBlock(line.transform, label + " Slider Fill", Vector2.zero, new Vector2(210f, 2f), TextAnchor.MiddleLeft, new Color(1f, 0.02f, 0.045f, 0.95f));
		Image dot = MakeUiBlock(settingsRoot.transform, label + " Dot", new Vector2(654f, y - 18f), new Vector2(18f, 18f), TextAnchor.UpperLeft, new Color(1f, 0.02f, 0.045f, 0.95f));
		dot.sprite = softCircleSprite;
		Text valueText = MakeText(settingsRoot.transform, value.ToString() + "%", 17, TextAnchor.UpperRight, new Vector2(-88f, y - 31f));
		valueText.color = new Color(0.95f, 0.035f, 0.055f);
		valueText.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 36f);
		MakeSettingsClickArea(y, action);
		if (label == "MUSIC")
		{
			musicSliderFill = fill;
			return valueText;
		}
		sfxSliderFill = fill;
		return valueText;
	}

	private Text MakeSettingsStepper(string label, float y, UnityAction left, UnityAction right)
	{
		MakeSettingsLabel(label, y);
		MakeSettingsArrow(new Vector2(456f, y - 35f), "<", left);
		MakeSettingsArrow(new Vector2(786f, y - 35f), ">", right);
		Text valueText = MakeText(settingsRoot.transform, "", 18, TextAnchor.UpperCenter, new Vector2(0f, y - 31f));
		valueText.color = new Color(0.78f, 0.72f, 0.74f);
		valueText.GetComponent<RectTransform>().sizeDelta = new Vector2(260f, 42f);
		return valueText;
	}

	private Text MakeSettingsToggle(string label, float y, UnityAction action, out Image dot)
	{
		MakeSettingsLabel(label, y);
		Text valueText = MakeText(settingsRoot.transform, "", 18, TextAnchor.UpperRight, new Vector2(-146f, y - 31f));
		valueText.color = new Color(0.78f, 0.72f, 0.74f);
		valueText.GetComponent<RectTransform>().sizeDelta = new Vector2(140f, 42f);
		dot = MakeUiBlock(settingsRoot.transform, label + " Toggle Dot", new Vector2(-82f, y - 26f), new Vector2(16f, 16f), TextAnchor.UpperRight, Color.white);
		dot.sprite = softCircleSprite;
		MakeSettingsClickArea(y, action);
		return valueText;
	}

	private void MakeSettingsLabel(string label, float y)
	{
		Text text = MakeText(settingsRoot.transform, label, 18, TextAnchor.UpperLeft, new Vector2(84f, y - 32f));
		text.color = new Color(0.72f, 0.66f, 0.68f);
		text.GetComponent<RectTransform>().sizeDelta = new Vector2(310f, 42f);
	}

	private Button MakeSettingsAction(string label, Vector2 pos, TextAnchor anchor)
	{
		Button button = new GameObject(label + " Button").AddComponent<Button>();
		button.transform.SetParent(settingsRoot.transform);
		Image background = button.gameObject.AddComponent<Image>();
		background.color = Color.clear;
		button.targetGraphic = background;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, anchor);
		rect.anchoredPosition = pos;
		rect.sizeDelta = new Vector2(210f, 64f);
		Text text = MakeText(button.transform, label, 18, TextAnchor.MiddleCenter, Vector2.zero);
		text.color = new Color(1f, 0.025f, 0.045f);
		text.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;
		return button;
	}

	private void MakeSettingsArrow(Vector2 pos, string label, UnityAction action)
	{
		Button button = new GameObject("Settings Arrow " + label).AddComponent<Button>();
		button.transform.SetParent(settingsRoot.transform);
		Image background = button.gameObject.AddComponent<Image>();
		background.color = Color.clear;
		button.targetGraphic = background;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.UpperLeft);
		rect.anchoredPosition = pos;
		rect.sizeDelta = new Vector2(52f, 48f);
		Text text = MakeText(button.transform, label, 22, TextAnchor.MiddleCenter, Vector2.zero);
		text.color = new Color(0.74f, 0.68f, 0.7f);
		text.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;
		button.onClick.AddListener(action);
	}

	private void MakeSettingsClickArea(float y, UnityAction action)
	{
		Button button = new GameObject("Settings Click Area").AddComponent<Button>();
		button.transform.SetParent(settingsRoot.transform);
		Image background = button.gameObject.AddComponent<Image>();
		background.color = Color.clear;
		RectTransform rect = button.GetComponent<RectTransform>();
		ApplyAnchor(rect, TextAnchor.UpperCenter);
		rect.anchoredPosition = new Vector2(0f, y - 28f);
		rect.sizeDelta = new Vector2(930f, 74f);
		button.targetGraphic = background;
		button.onClick.AddListener(action);
	}

	private void ShowSettingsScreen()
	{
		if (settingsRoot == null)
		{
			return;
		}
		overlayTitle.text = "";
		overlayTag.text = "";
		UpdateSettingsUi();
		InfernoSettings.Refresh(this); /* [InfernoLobby patch] */
		SelectMenuTab(2, false);
	}

	private void HideSettingsScreen()
	{
		SelectMenuTab(1, false);
	}

	private void UpdateSettingsUi()
	{
		musicVolume = Mathf.Clamp(musicVolume, 0, 100);
		sfxVolume = Mathf.Clamp(sfxVolume, 0, 100);
		graphicsIndex = Mathf.Clamp(graphicsIndex, 0, 2);
		frameRateIndex = 1;
		if (musicValueText != null)
		{
			musicValueText.text = musicVolume.ToString() + "%";
		}
		if (sfxValueText != null)
		{
			sfxValueText.text = sfxVolume.ToString() + "%";
		}
		if (musicSliderFill != null)
		{
			musicSliderFill.rectTransform.sizeDelta = new Vector2(300f * musicVolume / 100f, 2f);
		}
		if (sfxSliderFill != null)
		{
			sfxSliderFill.rectTransform.sizeDelta = new Vector2(300f * sfxVolume / 100f, 2f);
		}
		if (graphicsValueText != null)
		{
			graphicsValueText.text = graphicsIndex == 0 ? "LOW" : (graphicsIndex == 1 ? "HIGH" : "ULTRA");
		}
		if (frameRateValueText != null)
		{
			frameRateValueText.text = "60 FPS";
		}
		if (languageValueText != null)
		{
			languageValueText.text = languageIndex == 0 ? "ENGLISH" : "RUSSIAN";
		}
		UpdateToggle(vibrationValueText, vibrationDot, vibrationOn);
		UpdateToggle(darkModeValueText, darkModeDot, darkModeOn);
		UpdateToggle(tutorialValueText, tutorialDot, tutorialHints);
	}

	private void UpdateToggle(Text text, Image dot, bool value)
	{
		if (text != null)
		{
			text.text = value ? "ON" : "OFF";
		}
		if (dot != null)
		{
			dot.color = value ? new Color(1f, 0.025f, 0.045f) : new Color(0.2f, 0.2f, 0.2f);
		}
	}

	private void ApplyGarageOwnershipFromProfile()
	{
		if (garageVehicleCards == null)
		{
			return;
		}
		if (!SupabaseGameManager.IsVehicleUnlocked(ActiveVehicle.Id))
		{
			for (int i = 0; i < garageVehicleCards.Length; i++)
			{
				string vehicleId = garageVehicleCards[i].VehicleId;
				if (SupabaseGameManager.IsVehicleUnlocked(vehicleId))
				{
					SetActiveVehicleDefinition(CreateVehicleDefinition(vehicleId));
					garageSelectedVehicleCardIndex = i;
					break;
				}
			}
		}
		UpdateGarageUi();
	}
}
