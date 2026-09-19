using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

// Translated from Il2CppOutput Assembly-CSharp__1.cpp:
//   RevenueCatPurchaseService__ctor_mBD2FDB4016BD3E87585AC06A5F3497413F300C0A
//   RevenueCatPurchaseService_get_IsConfigured_mE1ECBAB33AC185B837DCE0A27FB2361D0638624D
//   RevenueCatPurchaseService_get_ConfigurationError_mB76A67F3E2A9CFB882932889AC996093822C4D5D
//   RevenueCatPurchaseService_CreateInstance_m4D206CB8558B2920E66E74AF3B58AF0FE2516DA2 (RuntimeInitializeOnLoads.json: BeforeSceneLoad)
//   RevenueCatPurchaseService_Awake_m6BB5E8AB077DF83DAD02D4E8F0C5281BD3477BDB
//   U3CStartU3Ed__21_MoveNext_m622EB734DE5B00D5B4560784FC32168D3CE86BDD
//   RevenueCatPurchaseService_OnDestroy_mAB1702A29875AE40222D50D6FE3EF51F63197E1A
//   U3CPurchaseCrystalsAsyncU3Ed__23_MoveNext_mB1CB2ABE650308DFA7CDC3F51A8FBE65A3A84C31
//   RevenueCatPurchaseService_GetLocalizedPrice_mB0FD17CEA2B5D5B7BBFFB5B7774B7143F3B34B19
//   U3CPurchaseAsyncU3Ed__25_MoveNext_m4C0AE5080C161EFEFD6C8BE4F5D436788725F4F2 (+DisplayClass25_0 b__0)
//   RevenueCatPurchaseService_EnsureReadyForCurrentAccountAsync_m3E1BE108310DAD49E1470350D5EFFA6D8FAA9835
//   U3CEnsureReadyForCurrentAccountCoreAsyncU3Ed__27_MoveNext_m5F609FCD01047DD7190B8FCAE019652F58E403D2
//   RevenueCatPurchaseService_IdentifyAsync_mD4049310AAB7AF05B331DE6F992AC3887F2BF17F (+DisplayClass28_0 b__0)
//   RevenueCatPurchaseService_LoadProductsAsync_m8E9E2A4CD0F2B1F33FF1BF19903C1E1E3175983C (+<>c b__29_0, DisplayClass29_0 b__1)
//   RevenueCatPurchaseService_HandleAccountChanged_m33206D1000F938B1AEB7B96BCBAB081E93DFAE06
//   RevenueCatPurchaseService_LoadConfiguration_m3126ABEE7E1288B864C950537A619132CDDD59A7
//   RevenueCatPurchaseService_IsPublicSdkKey_m60E137ED192C455C391772D6A9AF6E98E765C510
//   PurchaseOutcome_Completed_m0159128008884065FF8F16299487C7FF95682181 / PurchaseOutcome_Failed_m30E3AC41B36E03F773E694EA3033F690543044E5
namespace ShadowRun.Services
{
	public sealed class RevenueCatPurchaseService : MonoBehaviour
	{
		[Serializable]
		private sealed class ProductConfig
		{
			public string product_id;

			public int crystals;
		}

		[Serializable]
		private sealed class RevenueCatConfig
		{
			public string apple_public_sdk_key;

			public string google_public_sdk_key;

			public ProductConfig[] products;
		}

		public sealed class PurchaseOutcome
		{
			public bool Success { get; private set; }

			public bool Cancelled { get; private set; }

			public string ProductId { get; private set; }

			public string TransactionId { get; private set; }

			public string Message { get; private set; }

			public static PurchaseOutcome Completed(string productId, string transactionId)
			{
				return new PurchaseOutcome
				{
					Success = true,
					ProductId = productId,
					TransactionId = transactionId,
					Message = "Purchase verified. Waiting for the cloud reward."
				};
			}

			public static PurchaseOutcome Failed(string message, bool cancelled = false)
			{
				return new PurchaseOutcome
				{
					Cancelled = cancelled,
					Message = message ?? "Purchase failed."
				};
			}
		}

		private const string ConfigResourceName = "revenuecat";

		private static RevenueCatPurchaseService instance;

		private readonly Dictionary<int, ProductConfig> productsByAmount = new Dictionary<int, ProductConfig>();

		private readonly Dictionary<string, Purchases.StoreProduct> storeProducts = new Dictionary<string, Purchases.StoreProduct>(StringComparer.Ordinal);

		private Purchases purchases;

		private RevenueCatConfig config;

		private Task<bool> identifyTask;

		private Task<bool> productsTask;

		private string identifiedUserId = string.Empty;

		private bool purchaseInProgress;

		private bool sdkStarted;

		private string configurationError;

		public static bool IsConfigured => instance != null && string.IsNullOrEmpty(instance.configurationError);

		public static string ConfigurationError => instance?.configurationError ?? "RevenueCat is not initialized.";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CreateInstance()
		{
			if (FindAnyObjectByType<RevenueCatPurchaseService>() != null)
			{
				return;
			}
			GameObject host = new GameObject("RevenueCat Purchases");
			DontDestroyOnLoad(host);
			host.AddComponent<RevenueCatPurchaseService>();
		}

		private void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;
			DontDestroyOnLoad(gameObject);
			LoadConfiguration();
			if (!string.IsNullOrEmpty(configurationError))
			{
				return;
			}
			purchases = gameObject.AddComponent<Purchases>();
			purchases.useRuntimeSetup = false;
			purchases.revenueCatAPIKeyApple = config.apple_public_sdk_key.Trim();
			purchases.revenueCatAPIKeyGoogle = config.google_public_sdk_key.Trim();
			purchases.autoSyncPurchases = true;
			purchases.purchasesAreCompletedBy = Purchases.PurchasesAreCompletedBy.RevenueCat;
			purchases.productIdentifiers = Array.Empty<string>();
			SupabaseGameManager.ProfileChanged += HandleAccountChanged;
			SupabaseGameManager.AuthenticationCompleted += HandleAccountChanged;
		}

		private async void Start()
		{
			if (purchases == null)
			{
				return;
			}
			await Task.Yield();
			sdkStarted = true;
			await EnsureReadyForCurrentAccountAsync();
		}

		private void OnDestroy()
		{
			if (instance != this)
			{
				return;
			}
			SupabaseGameManager.ProfileChanged -= HandleAccountChanged;
			SupabaseGameManager.AuthenticationCompleted -= HandleAccountChanged;
			instance = null;
		}

		public static async Task<PurchaseOutcome> PurchaseCrystalsAsync(int crystalAmount)
		{
			if (instance == null)
			{
				return PurchaseOutcome.Failed("RevenueCat is not initialized.", false);
			}
			return await instance.PurchaseAsync(crystalAmount);
		}

		public static string GetLocalizedPrice(int crystalAmount, string fallback)
		{
			if (instance == null || !instance.productsByAmount.TryGetValue(crystalAmount, out ProductConfig configured) || !instance.storeProducts.TryGetValue(configured.product_id, out Purchases.StoreProduct product) || string.IsNullOrWhiteSpace(product.PriceString))
			{
				return fallback;
			}
			return product.PriceString;
		}

		private async Task<PurchaseOutcome> PurchaseAsync(int crystalAmount)
		{
			if (!string.IsNullOrEmpty(configurationError))
			{
				return PurchaseOutcome.Failed(configurationError, false);
			}
			if (purchaseInProgress)
			{
				return PurchaseOutcome.Failed("Another purchase is already in progress.", false);
			}
			if (!productsByAmount.TryGetValue(crystalAmount, out ProductConfig configured))
			{
				return PurchaseOutcome.Failed(string.Format("No RevenueCat product is mapped to {0} crystals.", crystalAmount), false);
			}
			if (!await EnsureReadyForCurrentAccountAsync())
			{
				return PurchaseOutcome.Failed("Sign in and connect to the internet before purchasing.", false);
			}
			purchaseInProgress = true;
			try
			{
				TaskCompletionSource<PurchaseOutcome> completion = new TaskCompletionSource<PurchaseOutcome>();
				purchases.PurchaseProduct(configured.product_id, result =>
				{
					if (result == null)
					{
						completion.TrySetResult(PurchaseOutcome.Failed("RevenueCat returned no purchase result.", false));
						return;
					}
					if (result.UserCancelled)
					{
						completion.TrySetResult(PurchaseOutcome.Failed("Purchase cancelled.", true));
						return;
					}
					if (result.Error != null)
					{
						completion.TrySetResult(PurchaseOutcome.Failed(result.Error.Message, false));
						return;
					}
					if (result.StoreTransaction == null || !string.Equals(result.ProductIdentifier, configured.product_id, StringComparison.Ordinal))
					{
						completion.TrySetResult(PurchaseOutcome.Failed("The store transaction did not match the selected product.", false));
						return;
					}
					completion.TrySetResult(PurchaseOutcome.Completed(result.ProductIdentifier, result.StoreTransaction.TransactionIdentifier));
				}, "inapp", null, Purchases.ProrationMode.UnknownSubscriptionUpgradeDowngradePolicy, false);
				return await completion.Task;
			}
			finally
			{
				purchaseInProgress = false;
			}
		}

		private Task<bool> EnsureReadyForCurrentAccountAsync()
		{
			return EnsureReadyForCurrentAccountCoreAsync();
		}

		private async Task<bool> EnsureReadyForCurrentAccountCoreAsync()
		{
			if (!sdkStarted || purchases == null || !string.IsNullOrEmpty(configurationError))
			{
				return false;
			}
			if (!SupabaseGameManager.IsAuthenticated || !SupabaseGameManager.ProfileMatchesSession)
			{
				return false;
			}
			string userId = SupabaseGameManager.CurrentUserId;
			if (!string.Equals(identifiedUserId, userId, StringComparison.Ordinal))
			{
				if (identifyTask == null || identifyTask.IsCompleted)
				{
					identifyTask = IdentifyAsync(userId);
				}
				if (!await identifyTask)
				{
					return false;
				}
			}
			if (productsTask == null || (productsTask.IsCompleted && storeProducts.Count == 0))
			{
				productsTask = LoadProductsAsync();
			}
			return await productsTask;
		}

		private Task<bool> IdentifyAsync(string userId)
		{
			TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
			purchases.LogIn(userId, (customerInfo, created, error) =>
			{
				if (error != null)
				{
					Debug.LogError("[RevenueCat] Could not identify Supabase account: " + error.Message);
					completion.TrySetResult(false);
					return;
				}
				identifiedUserId = userId;
				Debug.Log("[RevenueCat] Customer linked to Supabase UUID " + userId + ".");
				completion.TrySetResult(customerInfo != null);
			});
			return completion.Task;
		}

		private Task<bool> LoadProductsAsync()
		{
			TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
			string[] identifiers = productsByAmount.Values.Select(product => product.product_id).Distinct().ToArray();
			purchases.GetProducts(identifiers, (products, error) =>
			{
				storeProducts.Clear();
				if (error != null)
				{
					Debug.LogError("[RevenueCat] Product loading failed: " + error.Message);
					completion.TrySetResult(false);
					return;
				}
				if (products != null)
				{
					foreach (Purchases.StoreProduct product in products)
					{
						storeProducts[product.Identifier] = product;
					}
				}
				bool complete = identifiers.All(storeProducts.ContainsKey);
				if (!complete)
				{
					Debug.LogError("[RevenueCat] App Store did not return every configured consumable product.");
				}
				completion.TrySetResult(complete);
			}, "inapp");
			return completion.Task;
		}

		private void HandleAccountChanged()
		{
			if (!sdkStarted || purchases == null || !SupabaseGameManager.IsAuthenticated)
			{
				return;
			}
			_ = EnsureReadyForCurrentAccountAsync();
		}

		private void LoadConfiguration()
		{
			TextAsset asset = Resources.Load<TextAsset>(ConfigResourceName);
			if (asset == null)
			{
				configurationError = "Missing Assets/Resources/revenuecat.json.";
				return;
			}
			config = JsonUtility.FromJson<RevenueCatConfig>(asset.text);
			if (config == null)
			{
				configurationError = "RevenueCat configuration is invalid.";
				return;
			}
			if (!IsPublicSdkKey(config.apple_public_sdk_key))
			{
				configurationError = "RevenueCat Apple public SDK key is missing.";
			}
			if (config.products == null || config.products.Length != 3)
			{
				configurationError = "RevenueCat must contain exactly three crystal products.";
				return;
			}
			ProductConfig[] products = config.products;
			for (int i = 0; i < products.Length; i++)
			{
				ProductConfig product = products[i];
				if (product == null || string.IsNullOrWhiteSpace(product.product_id) || product.crystals <= 0 || productsByAmount.ContainsKey(product.crystals))
				{
					configurationError = "RevenueCat product mapping is incomplete or duplicated.";
					productsByAmount.Clear();
					return;
				}
				productsByAmount.Add(product.crystals, product);
			}
		}

		private static bool IsPublicSdkKey(string value)
		{
			return !string.IsNullOrWhiteSpace(value) && (value.StartsWith("appl_", StringComparison.Ordinal) || value.StartsWith("goog_", StringComparison.Ordinal));
		}
	}
}
