using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   FirstLaunchAuthScreen_CreateInstance_m39AE70E9D74A13D89A374413736E56CE97426DA3 (RuntimeInitializeOnLoads.json: AfterSceneLoad)
//   FirstLaunchAuthScreen_Show_m771D1E34707BE48FCC12ECD7233A43D1C976CC7C
//   FirstLaunchAuthScreen_Awake_mD57A2D7100F0CDA49D8F8D16C255D58B256479F4
//   FirstLaunchAuthScreen_OnDestroy_mA31F15864FC319803E205F4ADB0FFEB6F4724415
//   FirstLaunchAuthScreen_BuildUi_m62B5695E2C43853EA1736481A0F1E83E7961D5E0 (+b__16_0..6)
//   U3CRunEmailAsyncU3Ed__17_MoveNext / U3CRunAppleAsyncU3Ed__18_MoveNext / U3CRunGoogleAsyncU3Ed__19_MoveNext
//   FirstLaunchAuthScreen_ShowEmailForm_mD212882056273F96CEE8E21049D7BE2FB4B0BCEA
//   FirstLaunchAuthScreen_ShowChoices_mFA9040D3A18997C23E30BC2EFA95976F190564E9
//   FirstLaunchAuthScreen_SetInteractable_m57A38030FDEFE17FF03AC8496B335F5C17E336CB
//   FirstLaunchAuthScreen_SetStatus_m3E3AE594E423E61FD39832EC615894A48718E512
//   FirstLaunchAuthScreen_Close_m877971D7F6FF02F90CB68C58BD1F1A1EC0B13D75
//   FirstLaunchAuthScreen_MakeImage / MakeText / MakeButton / MakeArtworkButton / MakeRoundedTextButton /
//   MakeLegalLinkButton / LoadFullSprite / LoadTightSprite / MakeInput / LoadBuiltinRoundedSprite /
//   MakeNormalizedText / SetNormalizedRect / StretchWithInset / SetRect / Stretch
namespace ShadowRun.Services
{
	public sealed class FirstLaunchAuthScreen : MonoBehaviour
	{
		private InputField emailInput;

		private InputField passwordInput;

		private Text statusText;

		private Text emailFormTitle;

		private Button[] actionButtons;

		private GameObject choiceRoot;

		private GameObject emailFormRoot;

		private bool emailRegisterMode;

		public static bool IsVisible { get; private set; }

		// The shipped build shows this WELCOME screen on the first launch (and again from the case reward,
		// purchases, sign-out and account deletion until an account is chosen). Removed on request
		// (2026-09-20): the game starts straight in the lobby as a local, unauthenticated player and the
		// profile page keeps CREATE ACCOUNT / LOG IN for anyone who wants an account. Set to true to
		// restore the shipped first-launch flow.
		public const bool Enabled = false;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void CreateInstance()
		{
			if (Enabled && !SupabaseGameManager.HasCompletedFirstChoice)
			{
				Show();
			}
		}

		public static void Show()
		{
			if (!Enabled)
			{
				return;
			}
			if (FindAnyObjectByType<FirstLaunchAuthScreen>() != null)
			{
				return;
			}
			new GameObject("First Launch Authorization").AddComponent<FirstLaunchAuthScreen>();
		}

		private void Awake()
		{
			IsVisible = true;
			DontDestroyOnLoad(gameObject);
			BuildUi();
			SupabaseGameManager.AuthenticationCompleted += Close;
			SupabaseGameManager.AuthenticationStatusChanged += SetStatus;
		}

		private void OnDestroy()
		{
			IsVisible = false;
			SupabaseGameManager.AuthenticationCompleted -= Close;
			SupabaseGameManager.AuthenticationStatusChanged -= SetStatus;
		}

		private void BuildUi()
		{
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 30000;
			gameObject.AddComponent<GraphicRaycaster>();
			CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = new Vector2(1080f, 1920f);
			scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			scaler.matchWidthOrHeight = 0f;
			Image backdrop = MakeImage(transform, "Authorization Backdrop", Color.black);
			Stretch(backdrop.rectTransform);
			backdrop.raycastTarget = true;
			Image background = MakeImage(backdrop.transform, "Authorization Background", Color.white);
			Stretch(background.rectTransform);
			background.sprite = LoadFullSprite("фон авторизация");
			background.preserveAspect = false;
			background.raycastTarget = true;
			AspectRatioFitter fitter = background.gameObject.AddComponent<AspectRatioFitter>();
			fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
			fitter.aspectRatio = 0.462581336f;
			choiceRoot = new GameObject("Authorization Choices", typeof(RectTransform));
			choiceRoot.transform.SetParent(background.transform, false);
			Stretch(choiceRoot.GetComponent<RectTransform>());
			Button apple = MakeArtworkButton(choiceRoot.transform, "Apple", "эпл", new Rect(62f, 716f, 904f, 220f), new Rect(0.214f, 0.578f, 0.572f, 0.0644f));
			apple.onClick.AddListener(() => { _ = RunAppleAsync(); });
			Button google = MakeArtworkButton(choiceRoot.transform, "Google", "гугл", new Rect(60f, 734f, 904f, 206f), new Rect(0.214f, 0.646f, 0.572f, 0.0603f));
			google.onClick.AddListener(() => { _ = RunGoogleAsync(); });
			Button login = MakeRoundedTextButton(choiceRoot.transform, "LOG IN", new Rect(0.2005f, 0.755f, 0.244f, 0.0352f), 26);
			login.onClick.AddListener(() => ShowEmailForm(false));
			Button signUp = MakeRoundedTextButton(choiceRoot.transform, "SIGN UP", new Rect(0.5555f, 0.755f, 0.244f, 0.0352f), 26);
			signUp.onClick.AddListener(() => ShowEmailForm(true));
			Button guest = MakeArtworkButton(choiceRoot.transform, "Guest", "гест", new Rect(194f, 430f, 1150f, 238f), new Rect(0.15f, 0.805f, 0.7f, 0.0671f));
			guest.onClick.AddListener(SupabaseGameManager.PlayAsGuest);
			MakeNormalizedText(choiceRoot.transform, "By continuing, you agree to our", 16, FontStyle.Normal, new Rect(0.2f, 0.874f, 0.6f, 0.014f), new Color(0.72f, 0.7f, 0.76f, 1f));
			Button terms = MakeLegalLinkButton(choiceRoot.transform, "Terms of Service", new Rect(0.26f, 0.888f, 0.23f, 0.022f), () => InAppLegalBrowser.Open(InAppLegalBrowser.TermsOfServiceUrl));
			MakeNormalizedText(choiceRoot.transform, "•", 16, FontStyle.Normal, new Rect(0.49f, 0.888f, 0.02f, 0.022f), new Color(0.72f, 0.7f, 0.76f, 1f));
			Button privacy = MakeLegalLinkButton(choiceRoot.transform, "Privacy Policy", new Rect(0.51f, 0.888f, 0.23f, 0.022f), () => InAppLegalBrowser.Open(InAppLegalBrowser.PrivacyPolicyUrl));
			emailFormRoot = new GameObject("Email Authorization Form", typeof(RectTransform));
			emailFormRoot.transform.SetParent(background.transform, false);
			Stretch(emailFormRoot.GetComponent<RectTransform>());
			Image mask = MakeImage(emailFormRoot.transform, "Email Form Mask", new Color(0.005f, 0.003f, 0.01f, 1f));
			mask.raycastTarget = true;
			SetNormalizedRect(mask.rectTransform, new Rect(0.153f, 0.535f, 0.694f, 0.345f));
			emailFormTitle = MakeNormalizedText(mask.transform, "ACCOUNT LOGIN", 27, FontStyle.Bold, new Rect(0.08f, 0.08f, 0.84f, 0.12f), new Color(0.86f, 0.58f, 1f, 1f));
			emailInput = MakeInput(mask.transform, "Email", "EMAIL", new Rect(0.07f, 0.25f, 0.86f, 0.13f), false);
			passwordInput = MakeInput(mask.transform, "Password", "PASSWORD  •  6+ CHARACTERS", new Rect(0.07f, 0.41f, 0.86f, 0.13f), true);
			Button submit = MakeRoundedTextButton(mask.transform, "CONTINUE", new Rect(0.07f, 0.62f, 0.4f, 0.13f), 22);
			submit.onClick.AddListener(() => { _ = RunEmailAsync(emailRegisterMode); });
			Button cancel = MakeRoundedTextButton(mask.transform, "CANCEL", new Rect(0.53f, 0.62f, 0.4f, 0.13f), 22);
			cancel.onClick.AddListener(ShowChoices);
			emailFormRoot.SetActive(false);
			statusText = MakeNormalizedText(background.transform, "", 18, FontStyle.Normal, new Rect(0.12f, 0.944f, 0.76f, 0.035f), new Color(0.82f, 0.7f, 0.94f, 1f));
			statusText.resizeTextForBestFit = true;
			statusText.resizeTextMinSize = 14;
			statusText.resizeTextMaxSize = 20;
			actionButtons = new Button[9] { guest, apple, google, login, signUp, submit, cancel, terms, privacy };
		}

		private async Task RunEmailAsync(bool register)
		{
			SetInteractable(false);
			SetStatus(register ? "CREATING ACCOUNT..." : "SIGNING IN...");
			if (register)
			{
				await SupabaseGameManager.RegisterWithEmailAsync(emailInput.text, passwordInput.text);
			}
			else
			{
				await SupabaseGameManager.SignInWithEmailAsync(emailInput.text, passwordInput.text);
			}
			SetInteractable(true);
		}

		private async Task RunAppleAsync()
		{
			SetInteractable(false);
			await SupabaseGameManager.StartAppleSignInAsync();
			SetInteractable(true);
		}

		private async Task RunGoogleAsync()
		{
			SetInteractable(false);
			await SupabaseGameManager.StartGoogleSignInAsync();
			SetInteractable(true);
		}

		private void ShowEmailForm(bool register)
		{
			emailRegisterMode = register;
			if (choiceRoot != null)
			{
				choiceRoot.SetActive(false);
			}
			if (emailFormRoot != null)
			{
				emailFormRoot.SetActive(true);
			}
			if (emailFormTitle != null)
			{
				emailFormTitle.text = register ? "CREATE ACCOUNT" : "ACCOUNT LOGIN";
			}
			SetStatus("");
		}

		private void ShowChoices()
		{
			if (emailFormRoot != null)
			{
				emailFormRoot.SetActive(false);
			}
			if (choiceRoot != null)
			{
				choiceRoot.SetActive(true);
			}
			if (passwordInput != null)
			{
				passwordInput.text = string.Empty;
			}
			SetStatus("");
		}

		private void SetInteractable(bool value)
		{
			if (actionButtons == null)
			{
				return;
			}
			Button[] buttons = actionButtons;
			for (int i = 0; i < buttons.Length; i++)
			{
				Button button = buttons[i];
				if (button != null)
				{
					button.interactable = value;
				}
			}
		}

		private void SetStatus(string message)
		{
			if (statusText != null)
			{
				statusText.text = message;
			}
		}

		private void Close()
		{
			Destroy(gameObject);
		}

		private static Image MakeImage(Transform parent, string name, Color color)
		{
			Image image = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
			image.transform.SetParent(parent, false);
			image.color = color;
			return image;
		}

		private static Text MakeText(Transform parent, string value, int size, FontStyle style, Vector2 position, Vector2 dimensions, Color color)
		{
			Text text = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
			text.transform.SetParent(parent, false);
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			text.text = value;
			text.fontSize = size;
			text.fontStyle = style;
			text.alignment = TextAnchor.MiddleCenter;
			text.color = color;
			text.raycastTarget = false;
			SetRect(text.rectTransform, new Vector2(0.5f, 0.5f), dimensions, position);
			return text;
		}

		private static Button MakeButton(Transform parent, string label, Vector2 position, Color background, Color? textColor = null, Vector2? size = null)
		{
			Image image = MakeImage(parent, label + " Button", background);
			SetRect(image.rectTransform, new Vector2(0.5f, 0.5f), size ?? new Vector2(720f, 92f), position);
			Button button = image.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			Outline outline = image.gameObject.AddComponent<Outline>();
			outline.effectColor = new Color(0.76f, 0.38f, 1f, 0.55f);
			outline.effectDistance = new Vector2(1f, -1f);
			MakeText(button.transform, label, 27, FontStyle.Bold, Vector2.zero, size ?? new Vector2(700f, 86f), textColor ?? Color.white);
			return button;
		}

		private static Button MakeArtworkButton(Transform parent, string name, string resource, Rect crop, Rect normalizedBounds)
		{
			Image image = MakeImage(parent, name + " Authorization Button", Color.white);
			image.sprite = LoadTightSprite(resource, crop, false);
			image.type = Image.Type.Simple;
			image.preserveAspect = false;
			if (image.sprite == null)
			{
				Debug.LogError("Authorization artwork was not found in Resources: " + resource + ".png");
			}
			SetNormalizedRect(image.rectTransform, normalizedBounds);
			Button button = image.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			button.transition = Selectable.Transition.ColorTint;
			ColorBlock colors = button.colors;
			colors.normalColor = Color.white;
			colors.highlightedColor = new Color(1.06f, 1.06f, 1.06f, 1f);
			colors.pressedColor = new Color(0.82f, 0.76f, 0.88f, 1f);
			colors.disabledColor = new Color(0.42f, 0.42f, 0.46f, 0.72f);
			colors.fadeDuration = 0.08f;
			button.colors = colors;
			return button;
		}

		private static Button MakeRoundedTextButton(Transform parent, string label, Rect normalizedBounds, int fontSize)
		{
			Image frame = MakeImage(parent, label + " Authorization Button", new Color(0.72f, 0.28f, 0.94f, 0.98f));
			frame.sprite = LoadBuiltinRoundedSprite();
			frame.type = Image.Type.Sliced;
			SetNormalizedRect(frame.rectTransform, normalizedBounds);
			Image interior = MakeImage(frame.transform, "Button Interior", new Color(0.018f, 0.008f, 0.035f, 1f));
			interior.sprite = frame.sprite;
			interior.type = Image.Type.Sliced;
			StretchWithInset(interior.rectTransform, 2f);
			interior.raycastTarget = false;
			Button button = frame.gameObject.AddComponent<Button>();
			button.targetGraphic = frame;
			ColorBlock colors = button.colors;
			colors.normalColor = Color.white;
			colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
			colors.pressedColor = new Color(0.8f, 0.7f, 0.9f, 1f);
			colors.disabledColor = new Color(0.38f, 0.34f, 0.42f, 0.68f);
			colors.fadeDuration = 0.08f;
			button.colors = colors;
			Text text = MakeNormalizedText(interior.transform, label, fontSize, FontStyle.Bold, new Rect(0.06f, 0.08f, 0.88f, 0.84f), new Color(0.82f, 0.48f, 1f, 1f));
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 14;
			text.resizeTextMaxSize = fontSize;
			return button;
		}

		private static Button MakeLegalLinkButton(Transform parent, string label, Rect normalizedBounds, UnityAction onClick)
		{
			Image image = MakeImage(parent, label + " Link", new Color(0f, 0f, 0f, 0.001f));
			SetNormalizedRect(image.rectTransform, normalizedBounds);
			Button button = image.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			ColorBlock colors = button.colors;
			colors.normalColor = Color.white;
			colors.highlightedColor = new Color(1f, 0.9f, 1f, 1f);
			colors.pressedColor = new Color(0.72f, 0.52f, 0.86f, 1f);
			colors.disabledColor = new Color(0.45f, 0.4f, 0.5f, 0.65f);
			colors.fadeDuration = 0.07f;
			button.colors = colors;
			Text text = MakeNormalizedText(image.transform, label, 16, FontStyle.Normal, new Rect(0f, 0f, 1f, 1f), new Color(0.76f, 0.33f, 1f, 1f));
			text.raycastTarget = false;
			button.onClick.AddListener(onClick);
			return button;
		}

		private static Sprite LoadFullSprite(string resource)
		{
			Texture2D texture = Resources.Load<Texture2D>(resource);
			if (texture == null)
			{
				return null;
			}
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;
			return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
		}

		private static Sprite LoadTightSprite(string resource, Rect crop, bool sliced)
		{
			Texture2D texture = Resources.Load<Texture2D>(resource);
			if (texture == null)
			{
				return null;
			}
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;
			crop.x = Mathf.Clamp(crop.x, 0f, texture.width - 1f);
			crop.y = Mathf.Clamp(crop.y, 0f, texture.height - 1f);
			crop.width = Mathf.Min(crop.width, texture.width - crop.x);
			crop.height = Mathf.Min(crop.height, texture.height - crop.y);
			Vector4 border = sliced ? new Vector4(105f, 82f, 105f, 82f) : Vector4.zero;
			return Sprite.Create(texture, crop, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
		}

		private static InputField MakeInput(Transform parent, string name, string placeholderValue, Rect normalizedBounds, bool password)
		{
			Image frame = MakeImage(parent, name + " Input", new Color(0.48f, 0.18f, 0.72f, 0.95f));
			frame.sprite = LoadBuiltinRoundedSprite();
			frame.type = Image.Type.Sliced;
			SetNormalizedRect(frame.rectTransform, normalizedBounds);
			Image interior = MakeImage(frame.transform, "Input Interior", new Color(0.035f, 0.025f, 0.06f, 1f));
			interior.sprite = frame.sprite;
			interior.type = Image.Type.Sliced;
			StretchWithInset(interior.rectTransform, 2f);
			InputField input = frame.gameObject.AddComponent<InputField>();
			input.targetGraphic = interior;
			input.contentType = password ? InputField.ContentType.Password : InputField.ContentType.EmailAddress;
			Text text = MakeNormalizedText(interior.transform, "", 23, FontStyle.Normal, new Rect(0.055f, 0.05f, 0.89f, 0.9f), Color.white);
			text.alignment = TextAnchor.MiddleLeft;
			text.raycastTarget = true;
			input.textComponent = text;
			Text placeholder = MakeNormalizedText(interior.transform, placeholderValue, 21, FontStyle.Normal, new Rect(0.055f, 0.05f, 0.89f, 0.9f), new Color(0.48f, 0.42f, 0.58f, 1f));
			placeholder.alignment = TextAnchor.MiddleLeft;
			input.placeholder = placeholder;
			return input;
		}

		private static Sprite LoadBuiltinRoundedSprite()
		{
			return Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
		}

		private static Text MakeNormalizedText(Transform parent, string value, int size, FontStyle style, Rect normalizedBounds, Color color)
		{
			Text text = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
			text.transform.SetParent(parent, false);
			text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			text.text = value;
			text.fontSize = size;
			text.fontStyle = style;
			text.alignment = TextAnchor.MiddleCenter;
			text.color = color;
			text.raycastTarget = false;
			SetNormalizedRect(text.rectTransform, normalizedBounds);
			return text;
		}

		private static void SetNormalizedRect(RectTransform rect, Rect bounds)
		{
			rect.anchorMin = new Vector2(bounds.xMin, 1f - bounds.yMax);
			rect.anchorMax = new Vector2(bounds.xMax, 1f - bounds.yMin);
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
		}

		private static void StretchWithInset(RectTransform rect, float inset)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = new Vector2(inset, inset);
			rect.offsetMax = new Vector2(-inset, -inset);
		}

		private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 size, Vector2 position)
		{
			rect.anchorMin = anchor;
			rect.anchorMax = anchor;
			rect.pivot = new Vector2(0.5f, 0.5f);
			rect.sizeDelta = size;
			rect.anchoredPosition = position;
		}

		private static void Stretch(RectTransform rect)
		{
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
		}
	}
}
