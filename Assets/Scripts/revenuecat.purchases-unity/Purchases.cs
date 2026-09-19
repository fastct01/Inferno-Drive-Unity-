using System;
using System.Collections.Generic;
using RevenueCat;
using RevenueCat.SimpleJSON;
using UnityEngine;

public class Purchases : MonoBehaviour
{
	public enum AttributionNetwork
	{
		APPLE_SEARCH_ADS = 0,
		ADJUST = 1,
		APPSFLYER = 2,
		BRANCH = 3,
		TENJIN = 4,
		FACEBOOK = 5
	}

	public enum BillingFeature
	{
		Subscriptions = 0,
		SubscriptionsUpdate = 1,
		InAppItemsOnVR = 2,
		SubscriptionsOnVR = 3,
		PriceChangeConfirmation = 4
	}

	public class CustomPaywallImpressionParams
	{
		private string PaywallId__BackingField;

		private string OfferingId__BackingField;

		public string PaywallId
		{
			get
			{
				return PaywallId__BackingField;
			}
			private set
			{
				PaywallId__BackingField = value;
			}
		}

		public string OfferingId
		{
			get
			{
				return OfferingId__BackingField;
			}
			private set
			{
				OfferingId__BackingField = value;
			}
		}

		public CustomPaywallImpressionParams(string paywallId = null, string offeringId = null)
		{
		}
	}

	public class CustomerInfo
	{
		public EntitlementInfos Entitlements;

		public List<string> ActiveSubscriptions;

		public List<string> AllPurchasedProductIdentifiers;

		public DateTime? LatestExpirationDate;

		public DateTime FirstSeen;

		public string OriginalAppUserId;

		public DateTime RequestDate;

		public DateTime? OriginalPurchaseDate;

		public Dictionary<string, DateTime?> AllExpirationDates;

		public Dictionary<string, DateTime?> AllPurchaseDates;

		public string OriginalApplicationVersion;

		public string ManagementURL;

		public List<StoreTransaction> NonSubscriptionTransactions;

		public Dictionary<string, SubscriptionInfo> SubscriptionsByProductIdentifier;

		public CustomerInfo(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	[Serializable]
	public class DangerousSettings
	{
		public readonly bool AutoSyncPurchases;

		public DangerousSettings(bool autoSyncPurchases)
		{
		}

		public JSONNode Serialize()
		{
			return null;
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class Discount
	{
		public readonly string Identifier;

		public readonly float Price;

		public readonly string PriceString;

		public readonly int Cycles;

		public readonly string Period;

		public readonly string PeriodUnit;

		public readonly int PeriodNumberOfUnits;

		public Discount(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class EntitlementInfo
	{
		public readonly string Identifier;

		public readonly bool IsActive;

		public readonly bool WillRenew;

		public readonly string PeriodType;

		public readonly DateTime LatestPurchaseDate;

		public readonly DateTime OriginalPurchaseDate;

		public readonly DateTime? ExpirationDate;

		public readonly string Store;

		public readonly string ProductIdentifier;

		public readonly bool IsSandbox;

		public readonly DateTime? UnsubscribeDetectedAt;

		public readonly DateTime? BillingIssueDetectedAt;

		public readonly VerificationResult Verification;

		public EntitlementInfo(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class EntitlementInfos
	{
		public readonly Dictionary<string, EntitlementInfo> All;

		public readonly Dictionary<string, EntitlementInfo> Active;

		public readonly VerificationResult Verification;

		public EntitlementInfos(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public enum EntitlementVerificationMode
	{
		Disabled = 0,
		Informational = 1
	}

	[Serializable]
	public class Error
	{
		public readonly string Message;

		public readonly int Code;

		public readonly string UnderlyingErrorMessage;

		public readonly string ReadableErrorCode;

		public Error(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class GoogleProductChangeInfo
	{
		public readonly string OldProductIdentifier;

		public readonly ProrationMode ProrationMode;

		public GoogleProductChangeInfo(string OldProductIdentifier, ProrationMode ProrationMode)
		{
		}
	}

	public enum InAppMessageType
	{
		BillingIssue = 0,
		PriceIncreaseConsent = 1,
		Generic = 2,
		WinBackOffer = 3
	}

	public class IntroEligibility
	{
		public readonly IntroEligibilityStatus Status;

		public readonly string Description;

		public IntroEligibility(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public enum IntroEligibilityStatus
	{
		IntroEligibilityStatusUnknown = 0,
		IntroEligibilityStatusIneligible = 1,
		IntroEligibilityStatusEligible = 2,
		IntroEligibilityStatusNoIntroOfferExists = 3
	}

	public class IntroductoryPrice
	{
		public float Price;

		public string PriceString;

		public string Period;

		public string Unit;

		public int NumberOfUnits;

		public int Cycles;

		public IntroductoryPrice(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public enum LogLevel
	{
		Verbose = 0,
		Debug = 1,
		Info = 2,
		Warn = 3,
		Error = 4
	}

	public class Offering
	{
		public readonly string Identifier;

		public readonly string ServerDescription;

		public readonly List<Package> AvailablePackages;

		public readonly Dictionary<string, object> Metadata;

		public readonly Package Lifetime;

		public readonly Package Annual;

		public readonly Package SixMonth;

		public readonly Package ThreeMonth;

		public readonly Package TwoMonth;

		public readonly Package Monthly;

		public readonly Package Weekly;

		public readonly string WebCheckoutUrl;

		public Offering(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}

		private object ParseJsonValue(JSONNode jsonValue)
		{
			return null;
		}
	}

	public class Offerings
	{
		public readonly Dictionary<string, Offering> All;

		public readonly Offering Current;

		public Offerings(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class Package
	{
		public readonly string Identifier;

		public readonly string PackageType;

		public readonly StoreProduct StoreProduct;

		public readonly PresentedOfferingContext PresentedOfferingContext;

		public readonly string WebCheckoutUrl;

		public readonly string OfferingIdentifier;

		public Package(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class PresentedOfferingContext
	{
		public readonly string OfferingIdentifier;

		public readonly string PlacementIdentifier;

		public readonly PresentedOfferingTargetingContext TargetingContext;

		public PresentedOfferingContext(JSONNode response)
		{
		}

		public PresentedOfferingContext(string offeringIdentifier)
		{
		}

		public override string ToString()
		{
			return null;
		}

		public string ToJsonString()
		{
			return null;
		}
	}

	public class PresentedOfferingTargetingContext
	{
		public readonly int Revision;

		public readonly string RuleId;

		public PresentedOfferingTargetingContext(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public enum ProductCategory
	{
		NON_SUBSCRIPTION = 0,
		SUBSCRIPTION = 1,
		UNKNOWN = 2
	}

	public class PromotionalOffer
	{
		public readonly string Identifier;

		public readonly string KeyIdentifier;

		public readonly string Nonce;

		public readonly string Signature;

		public readonly long Timestamp;

		public PromotionalOffer(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public enum ProrationMode
	{
		UnknownSubscriptionUpgradeDowngradePolicy = 0,
		ImmediateWithTimeProration = 1,
		ImmediateAndChargeProratedPrice = 2,
		ImmediateWithoutProration = 3,
		Deferred = 6,
		ImmediateAndChargeFullPrice = 5
	}

	public class PurchaseResult
	{
		public readonly CustomerInfo CustomerInfo;

		public readonly string ProductIdentifier;

		public readonly bool UserCancelled;

		public readonly Error Error;

		public readonly StoreTransaction StoreTransaction;

		public PurchaseResult(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public delegate void GetStorefrontFunc(Storefront storefront);

	public delegate void GetProductsFunc(List<StoreProduct> products, Error error);

	public delegate void MakePurchaseFunc(PurchaseResult purchaseResult);

	public delegate void CustomerInfoFunc(CustomerInfo customerInfo, Error error);

	public delegate void LogInFunc(CustomerInfo customerInfo, bool created, Error error);

	public delegate void LogHandlerFunc(LogLevel logLevel, string message);

	public delegate void GetOfferingsFunc(Offerings offerings, Error error);

	public delegate void GetCurrentOfferingForPlacementFunc(Offering offerings, Error error);

	public delegate void SyncAttributesAndOfferingsIfNeededFunc(Offerings offerings, Error error);

	public delegate void CheckTrialOrIntroductoryPriceEligibilityFunc(Dictionary<string, IntroEligibility> products);

	public delegate void RecordPurchaseFunc(StoreTransaction transaction, Error error);

	public delegate void CanMakePaymentsFunc(bool canMakePayments, Error error);

	public delegate void GetAmazonLWAConsentStatusFunc(bool hasConsented, Error error);

	public delegate void GetPromotionalOfferFunc(PromotionalOffer promotionalOffer, Error error);

	public delegate void ParseAsWebPurchaseRedemptionFunc(WebPurchaseRedemption webPurchaseRedemption);

	public delegate void RedeemWebPurchaseFunc(WebPurchaseRedemptionResult result);

	public delegate void GetVirtualCurrenciesFunc(VirtualCurrencies virtualCurrencies, Error error);

	public delegate void GetEligibleWinBackOffersForProductFunc(WinBackOffer[] winBackOffers, Error error);

	public delegate void GetEligibleWinBackOffersForPackageFunc(WinBackOffer[] winBackOffers, Error error);

	public enum PurchasesAreCompletedBy
	{
		RevenueCat = 0,
		MyApp = 1
	}

	public class PurchasesConfiguration
	{
		public class Builder
		{
			private readonly string _apiKey;

			private string _appUserId;

			private PurchasesAreCompletedBy _purchasesAreCompletedBy;

			private string _userDefaultsSuiteName;

			private bool _useAmazon;

			private DangerousSettings _dangerousSettings;

			private StoreKitVersion _storeKitVersion;

			private bool _shouldShowInAppMessagesAutomatically;

			private EntitlementVerificationMode _entitlementVerificationMode;

			private bool _pendingTransactionsForPrepaidPlansEnabled;

			private Builder(string apiKey)
			{
			}

			public static Builder Init(string apiKey)
			{
				return null;
			}

			public PurchasesConfiguration Build()
			{
				return null;
			}

			public Builder SetAppUserId(string appUserId)
			{
				return null;
			}

			public Builder SetPurchasesAreCompletedBy(PurchasesAreCompletedBy purchasesAreCompletedBy, StoreKitVersion storeKitVersion)
			{
				return null;
			}

			public Builder SetUserDefaultsSuiteName(string userDefaultsSuiteName)
			{
				return null;
			}

			public Builder SetUseAmazon(bool useAmazon)
			{
				return null;
			}

			public Builder SetDangerousSettings(DangerousSettings dangerousSettings)
			{
				return null;
			}

			public Builder SetStoreKitVersion(StoreKitVersion storeKitVersion)
			{
				return null;
			}

			public Builder SetShouldShowInAppMessagesAutomatically(bool shouldShowInAppMessagesAutomatically)
			{
				return null;
			}

			public Builder SetEntitlementVerificationMode(EntitlementVerificationMode entitlementVerificationMode)
			{
				return null;
			}

			public Builder SetPendingTransactionsForPrepaidPlansEnabled(bool pendingTransactionsForPrepaidPlansEnabled)
			{
				return null;
			}
		}

		public readonly string ApiKey;

		public readonly string AppUserId;

		public readonly bool ObserverMode;

		public readonly PurchasesAreCompletedBy PurchasesAreCompletedBy;

		public readonly string UserDefaultsSuiteName;

		public readonly bool UseAmazon;

		public readonly DangerousSettings DangerousSettings;

		public readonly StoreKitVersion StoreKitVersion;

		public readonly bool ShouldShowInAppMessagesAutomatically;

		public readonly EntitlementVerificationMode EntitlementVerificationMode;

		public readonly bool PendingTransactionsForPrepaidPlansEnabled;

		private PurchasesConfiguration(string apiKey, string appUserId, PurchasesAreCompletedBy purchasesAreCompletedBy, string userDefaultsSuiteName, bool useAmazon, DangerousSettings dangerousSettings, StoreKitVersion storeKitVersion, bool shouldShowInAppMessagesAutomatically, EntitlementVerificationMode entitlementVerificationMode, bool pendingTransactionsForPrepaidPlansEnabled)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	private class PurchasesWrapperNoop : IPurchasesWrapper
	{
		public void Setup(string gameObject, string apiKey, string appUserId, PurchasesAreCompletedBy purchasesAreCompletedBy, StoreKitVersion storeKitVersion, string userDefaultsSuiteName, bool useAmazon, string dangerousSettingsJson, bool shouldShowInAppMessagesAutomatically, bool pendingTransactionsForPrepaidPlansEnabled)
		{
		}

		public void Setup(string gameObject, string apiKey, string appUserId, PurchasesAreCompletedBy purchasesAreCompletedBy, StoreKitVersion storeKitVersion, string userDefaultsSuiteName, bool useAmazon, string dangerousSettingsJson, bool shouldShowInAppMessagesAutomatically, EntitlementVerificationMode entitlementVerificationMode, bool pendingTransactionsForPrepaidPlansEnabled)
		{
		}

		public void GetStorefront()
		{
		}

		public void GetProducts(string[] productIdentifiers, string type = "subs")
		{
		}

		public void MakePurchase(string productIdentifier, string type = "subs", string oldSku = null)
		{
		}

		public void PurchaseProduct(string productIdentifier, string type = "subs", string oldSku = null, ProrationMode prorationMode = ProrationMode.UnknownSubscriptionUpgradeDowngradePolicy, bool googleIsPersonalizedPrice = false, string presentedOfferingIdentifier = null, PromotionalOffer discount = null)
		{
		}

		public void PurchasePackage(Package packageToPurchase, string oldSku = null, ProrationMode prorationMode = ProrationMode.UnknownSubscriptionUpgradeDowngradePolicy, bool googleIsPersonalizedPrice = false, PromotionalOffer discount = null)
		{
		}

		public void PurchaseSubscriptionOption(SubscriptionOption subscriptionOption, GoogleProductChangeInfo googleProductChangeInfo = null, bool googleIsPersonalizedPrice = false)
		{
		}

		public void RestorePurchases()
		{
		}

		public void LogIn(string appUserId)
		{
		}

		public void LogOut()
		{
		}

		public void SetAllowSharingStoreAccount(bool allow)
		{
		}

		public string GetAppUserId()
		{
			return null;
		}

		public void SetDebugLogsEnabled(bool enabled)
		{
		}

		public void SetLogLevel(LogLevel level)
		{
		}

		public void SetLogHandler()
		{
		}

		public void SetProxyURL(string proxyURL)
		{
		}

		public void GetCustomerInfo()
		{
		}

		public void GetOfferings()
		{
		}

		public void GetCurrentOfferingForPlacement(string placementIdentifier)
		{
		}

		public void SyncAttributesAndOfferingsIfNeeded()
		{
		}

		public void SyncPurchases()
		{
		}

		public void SyncAmazonPurchase(string productID, string receiptID, string amazonUserID, string isoCurrencyCode, double price)
		{
		}

		public void GetAmazonLWAConsentStatus()
		{
		}

		public void EnableAdServicesAttributionTokenCollection()
		{
		}

		public bool IsAnonymous()
		{
			return false;
		}

		public bool IsConfigured()
		{
			return false;
		}

		public void CheckTrialOrIntroductoryPriceEligibility(string[] productIdentifiers)
		{
		}

		public void InvalidateCustomerInfoCache()
		{
		}

		public void PresentCodeRedemptionSheet()
		{
		}

		public void RecordPurchase(string productID)
		{
		}

		public void SetSimulatesAskToBuyInSandbox(bool enabled)
		{
		}

		public void SetAttributes(string attributesJson)
		{
		}

		public void SetEmail(string email)
		{
		}

		public void SetPhoneNumber(string phoneNumber)
		{
		}

		public void SetDisplayName(string displayName)
		{
		}

		public void SetPushToken(string token)
		{
		}

		public void SetAdjustID(string adjustID)
		{
		}

		public void SetAppsflyerID(string appsflyerID)
		{
		}

		public void SetFBAnonymousID(string fbAnonymousID)
		{
		}

		public void SetMparticleID(string mparticleID)
		{
		}

		public void SetOnesignalID(string onesignalID)
		{
		}

		public void SetOnesignalUserID(string onesignalUserID)
		{
		}

		public void SetAirshipChannelID(string airshipChannelID)
		{
		}

		public void SetCleverTapID(string cleverTapID)
		{
		}

		public void SetMixpanelDistinctID(string mixpanelDistinctID)
		{
		}

		public void SetFirebaseAppInstanceID(string firebaseAppInstanceID)
		{
		}

		public void SetMediaSource(string mediaSource)
		{
		}

		public void SetCampaign(string campaign)
		{
		}

		public void SetAdGroup(string adGroup)
		{
		}

		public void SetAd(string ad)
		{
		}

		public void SetKeyword(string keyword)
		{
		}

		public void SetCreative(string creative)
		{
		}

		public void CollectDeviceIdentifiers()
		{
		}

		public void CanMakePayments(BillingFeature[] features)
		{
		}

		public void GetPromotionalOffer(string productIdentifier, string discountIdentifier)
		{
		}

		public void ShowInAppMessages(InAppMessageType[] messageTypes)
		{
		}

		public void ParseAsWebPurchaseRedemption(string urlString)
		{
		}

		public void RedeemWebPurchase(WebPurchaseRedemption webPurchaseRedemption)
		{
		}

		public void GetVirtualCurrencies()
		{
		}

		public string GetCachedVirtualCurrencies()
		{
			return null;
		}

		public void InvalidateVirtualCurrenciesCache()
		{
		}

		public void GetEligibleWinBackOffersForProduct(StoreProduct storeProduct)
		{
		}

		public void GetEligibleWinBackOffersForPackage(Package package)
		{
		}

		public void PurchaseProductWithWinBackOffer(StoreProduct storeProduct, WinBackOffer winBackOffer)
		{
		}

		public void PurchasePackageWithWinBackOffer(Package package, WinBackOffer winBackOffer)
		{
		}

		public void TrackCustomPaywallImpression(CustomPaywallImpressionParams parameters)
		{
		}

		public void TrackAdDisplayed(AdDisplayedData data)
		{
		}

		public void TrackAdOpened(AdOpenedData data)
		{
		}

		public void TrackAdRevenue(AdRevenueData data)
		{
		}

		public void TrackAdLoaded(AdLoadedData data)
		{
		}

		public void TrackAdFailedToLoad(AdFailedToLoadData data)
		{
		}
	}

	public enum StoreKitVersion
	{
		StoreKit1 = 0,
		StoreKit2 = 1,
		Default = 2
	}

	public class StoreProduct
	{
		public readonly string Title;

		public readonly string Identifier;

		public readonly string Description;

		public readonly float Price;

		public readonly string PriceString;

		public readonly float? PricePerWeek;

		public readonly float? PricePerMonth;

		public readonly float? PricePerYear;

		public readonly string PricePerWeekString;

		public readonly string PricePerMonthString;

		public readonly string PricePerYearString;

		public readonly string CurrencyCode;

		public IntroductoryPrice IntroductoryPrice;

		public readonly ProductCategory ProductCategory;

		public readonly SubscriptionOption DefaultOption;

		public readonly SubscriptionOption[] SubscriptionOptions;

		public readonly PresentedOfferingContext PresentedOfferingContext;

		public readonly string PresentedOfferingIdentifier;

		public readonly Discount[] Discounts;

		public readonly string SubscriptionPeriod;

		public StoreProduct(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class StoreTransaction
	{
		public readonly string TransactionIdentifier;

		public readonly string ProductIdentifier;

		public readonly DateTime PurchaseDate;

		public StoreTransaction(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class Storefront
	{
		public readonly string CountryCode;

		public Storefront(string countryCode)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class SubscriptionInfo
	{
		public readonly string ProductIdentifier;

		public readonly DateTime PurchaseDate;

		public readonly DateTime? OriginalPurchaseDate;

		public readonly DateTime? ExpiresDate;

		public readonly string Store;

		public readonly DateTime? UnsubscribeDetectedAt;

		public readonly bool IsSandbox;

		public readonly DateTime? BillingIssuesDetectedAt;

		public readonly DateTime? GracePeriodExpiresDate;

		public readonly string OwnershipType;

		public readonly string PeriodType;

		public readonly DateTime? RefundedAt;

		public readonly string StoreTransactionId;

		public readonly bool IsActive;

		public readonly bool WillRenew;

		public SubscriptionInfo(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}

		private DateTime? FromResponseISO8601String(JSONNode response, string key)
		{
			return null;
		}
	}

	public class SubscriptionOption
	{
		public class PricingPhase
		{
			public readonly Period BillingPeriod;

			public readonly RecurrenceMode RecurrenceMode;

			public readonly int BillingCycleCount;

			public readonly Price Price;

			public readonly OfferPaymentMode OfferPaymentMode;

			public PricingPhase(JSONNode response)
			{
			}

			public override string ToString()
			{
				return null;
			}
		}

		public class Period
		{
			public readonly PeriodUnit Unit;

			public readonly int Value;

			public readonly string ISO8601;

			public Period(JSONNode response)
			{
			}

			public override string ToString()
			{
				return null;
			}
		}

		public enum PeriodUnit
		{
			DAY = 0,
			WEEK = 1,
			MONTH = 2,
			YEAR = 3,
			UNKNOWN = 4
		}

		public enum RecurrenceMode
		{
			INFINITE_RECURRING = 1,
			FINITE_RECURRING = 2,
			NON_RECURRING = 3,
			UNKNOWN = 4
		}

		public enum OfferPaymentMode
		{
			FREE_TRIAL = 0,
			SINGLE_PAYMENT = 1,
			DISCOUNTED_RECURRING_PAYMENT = 2,
			UNKNOWN = 3
		}

		public class Price
		{
			public readonly string Formatted;

			public readonly int AmountMicros;

			public readonly string CurrencyCode;

			public Price(JSONNode response)
			{
			}

			public override string ToString()
			{
				return null;
			}
		}

		public class InstallmentsInfo
		{
			public readonly int CommitmentPaymentsCount;

			public readonly int RenewalCommitmentPaymentsCount;

			public InstallmentsInfo(JSONNode response)
			{
			}

			public override string ToString()
			{
				return null;
			}
		}

		public readonly string Id;

		public readonly string StoreProductId;

		public readonly string ProductId;

		public readonly PricingPhase[] PricingPhases;

		public readonly string[] Tags;

		public readonly bool IsBasePlan;

		public readonly Period BillingPeriod;

		public readonly bool IsPrepaid;

		public readonly PricingPhase FullPricePhase;

		public readonly PricingPhase FreePhase;

		public readonly PricingPhase IntroPhase;

		public readonly PresentedOfferingContext PresentedOfferingContext;

		public readonly string PresentedOfferingIdentifier;

		public readonly InstallmentsInfo OptionInstallmentsInfo;

		public SubscriptionOption(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public abstract class UpdatedCustomerInfoListener : MonoBehaviour
	{
		public abstract void CustomerInfoReceived(CustomerInfo customerInfo);
	}

	public enum VerificationResult
	{
		NotRequested = 0,
		Verified = 1,
		Failed = 2,
		VerifiedOnDevice = 3
	}

	public class VirtualCurrencies
	{
		public readonly Dictionary<string, VirtualCurrency> All;

		public VirtualCurrencies(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class VirtualCurrency
	{
		public readonly int Balance;

		public readonly string Name;

		public readonly string Code;

		public readonly string ServerDescription;

		public VirtualCurrency(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public class WebPurchaseRedemption
	{
		public readonly string RedemptionLink;

		public WebPurchaseRedemption(string redemptionLink)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public abstract class WebPurchaseRedemptionResult
	{
		public sealed class Success : WebPurchaseRedemptionResult
		{
			public readonly CustomerInfo CustomerInfo;

			public Success(CustomerInfo customerInfo)
			{
			}
		}

		public sealed class RedemptionError : WebPurchaseRedemptionResult
		{
			public readonly Error Error;

			public RedemptionError(Error error)
			{
			}
		}

		public sealed class InvalidToken : WebPurchaseRedemptionResult
		{
			private static readonly InvalidToken Instance__BackingField;

			public static InvalidToken Instance => Instance__BackingField;

			private InvalidToken()
			{
			}
		}

		public sealed class Expired : WebPurchaseRedemptionResult
		{
			private readonly string ObfuscatedEmail__BackingField;

			public string ObfuscatedEmail => ObfuscatedEmail__BackingField;

			public Expired(string obfuscatedEmail)
			{
			}
		}

		public sealed class PurchaseBelongsToOtherUser : WebPurchaseRedemptionResult
		{
			private static readonly PurchaseBelongsToOtherUser Instance__BackingField;

			public static PurchaseBelongsToOtherUser Instance => Instance__BackingField;

			private PurchaseBelongsToOtherUser()
			{
			}
		}

		private WebPurchaseRedemptionResult()
		{
		}

		public static WebPurchaseRedemptionResult FromJson(JSONNode response)
		{
			return null;
		}
	}

	public class WinBackOffer
	{
		public readonly string Identifier;

		public readonly float Price;

		public readonly string PriceString;

		public readonly int Cycles;

		public readonly string Period;

		public readonly string PeriodUnit;

		public readonly int PeriodNumberOfUnits;

		public WinBackOffer(JSONNode response)
		{
		}

		public override string ToString()
		{
			return null;
		}
	}

	public bool useRuntimeSetup;

	public string revenueCatAPIKeyApple;

	public string revenueCatAPIKeyGoogle;

	public string revenueCatAPIKeyAmazon;

	public bool useAmazon;

	public bool autoSyncPurchases;

	public string appUserID;

	public string[] productIdentifiers;

	public UpdatedCustomerInfoListener listener;

	public string userDefaultsSuiteName;

	public PurchasesAreCompletedBy purchasesAreCompletedBy;

	public StoreKitVersion storeKitVersion;

	public bool shouldShowInAppMessagesAutomatically;

	public EntitlementVerificationMode entitlementVerificationMode;

	public bool pendingTransactionsForPrepaidPlansEnabled;

	public string proxyURL;

	private IPurchasesWrapper _wrapper;

	private AdTracker AdTracker__BackingField;

	private GetStorefrontFunc StorefrontCallback__BackingField;

	private GetProductsFunc ProductsCallback__BackingField;

	private MakePurchaseFunc MakePurchaseCallback__BackingField;

	private CustomerInfoFunc RestorePurchasesCallback__BackingField;

	private LogInFunc LogInCallback__BackingField;

	private CustomerInfoFunc LogOutCallback__BackingField;

	private LogHandlerFunc LogHandler__BackingField;

	private CustomerInfoFunc GetCustomerInfoCallback__BackingField;

	private GetOfferingsFunc GetOfferingsCallback__BackingField;

	private GetCurrentOfferingForPlacementFunc GetCurrentOfferingForPlacementCallback__BackingField;

	private SyncAttributesAndOfferingsIfNeededFunc SyncAttributesAndOfferingsIfNeededCallback__BackingField;

	private CustomerInfoFunc SyncPurchasesCallback__BackingField;

	private CheckTrialOrIntroductoryPriceEligibilityFunc CheckTrialOrIntroductoryPriceEligibilityCallback__BackingField;

	private RecordPurchaseFunc RecordPurchaseCallback__BackingField;

	private CanMakePaymentsFunc CanMakePaymentsCallback__BackingField;

	private GetAmazonLWAConsentStatusFunc GetAmazonLWAConsentStatusCallback__BackingField;

	private GetPromotionalOfferFunc GetPromotionalOfferCallback__BackingField;

	private ParseAsWebPurchaseRedemptionFunc ParseAsWebPurchaseRedemptionCallback__BackingField;

	private RedeemWebPurchaseFunc RedeemWebPurchaseCallback__BackingField;

	private GetVirtualCurrenciesFunc GetVirtualCurrenciesCallback__BackingField;

	private GetEligibleWinBackOffersForProductFunc GetEligibleWinBackOffersForProductCallback__BackingField;

	private GetEligibleWinBackOffersForPackageFunc GetEligibleWinBackOffersForPackageCallback__BackingField;

	public AdTracker AdTracker
	{
		get
		{
			return AdTracker__BackingField;
		}
		private set
		{
			AdTracker__BackingField = value;
		}
	}

	private GetStorefrontFunc StorefrontCallback
	{
		get
		{
			return StorefrontCallback__BackingField;
		}
		set
		{
			StorefrontCallback__BackingField = value;
		}
	}

	private GetProductsFunc ProductsCallback
	{
		get
		{
			return ProductsCallback__BackingField;
		}
		set
		{
			ProductsCallback__BackingField = value;
		}
	}

	private MakePurchaseFunc MakePurchaseCallback
	{
		get
		{
			return MakePurchaseCallback__BackingField;
		}
		set
		{
			MakePurchaseCallback__BackingField = value;
		}
	}

	private CustomerInfoFunc RestorePurchasesCallback
	{
		get
		{
			return RestorePurchasesCallback__BackingField;
		}
		set
		{
			RestorePurchasesCallback__BackingField = value;
		}
	}

	private LogInFunc LogInCallback
	{
		get
		{
			return LogInCallback__BackingField;
		}
		set
		{
			LogInCallback__BackingField = value;
		}
	}

	private CustomerInfoFunc LogOutCallback
	{
		get
		{
			return LogOutCallback__BackingField;
		}
		set
		{
			LogOutCallback__BackingField = value;
		}
	}

	private LogHandlerFunc LogHandler
	{
		get
		{
			return LogHandler__BackingField;
		}
		set
		{
			LogHandler__BackingField = value;
		}
	}

	private CustomerInfoFunc GetCustomerInfoCallback
	{
		get
		{
			return GetCustomerInfoCallback__BackingField;
		}
		set
		{
			GetCustomerInfoCallback__BackingField = value;
		}
	}

	private GetOfferingsFunc GetOfferingsCallback
	{
		get
		{
			return GetOfferingsCallback__BackingField;
		}
		set
		{
			GetOfferingsCallback__BackingField = value;
		}
	}

	private GetCurrentOfferingForPlacementFunc GetCurrentOfferingForPlacementCallback
	{
		get
		{
			return GetCurrentOfferingForPlacementCallback__BackingField;
		}
		set
		{
			GetCurrentOfferingForPlacementCallback__BackingField = value;
		}
	}

	private SyncAttributesAndOfferingsIfNeededFunc SyncAttributesAndOfferingsIfNeededCallback
	{
		get
		{
			return SyncAttributesAndOfferingsIfNeededCallback__BackingField;
		}
		set
		{
			SyncAttributesAndOfferingsIfNeededCallback__BackingField = value;
		}
	}

	private CustomerInfoFunc SyncPurchasesCallback
	{
		get
		{
			return SyncPurchasesCallback__BackingField;
		}
		set
		{
			SyncPurchasesCallback__BackingField = value;
		}
	}

	private CheckTrialOrIntroductoryPriceEligibilityFunc CheckTrialOrIntroductoryPriceEligibilityCallback
	{
		get
		{
			return CheckTrialOrIntroductoryPriceEligibilityCallback__BackingField;
		}
		set
		{
			CheckTrialOrIntroductoryPriceEligibilityCallback__BackingField = value;
		}
	}

	private RecordPurchaseFunc RecordPurchaseCallback
	{
		get
		{
			return RecordPurchaseCallback__BackingField;
		}
		set
		{
			RecordPurchaseCallback__BackingField = value;
		}
	}

	private CanMakePaymentsFunc CanMakePaymentsCallback
	{
		get
		{
			return CanMakePaymentsCallback__BackingField;
		}
		set
		{
			CanMakePaymentsCallback__BackingField = value;
		}
	}

	private GetAmazonLWAConsentStatusFunc GetAmazonLWAConsentStatusCallback
	{
		get
		{
			return GetAmazonLWAConsentStatusCallback__BackingField;
		}
		set
		{
			GetAmazonLWAConsentStatusCallback__BackingField = value;
		}
	}

	private GetPromotionalOfferFunc GetPromotionalOfferCallback
	{
		get
		{
			return GetPromotionalOfferCallback__BackingField;
		}
		set
		{
			GetPromotionalOfferCallback__BackingField = value;
		}
	}

	private ParseAsWebPurchaseRedemptionFunc ParseAsWebPurchaseRedemptionCallback
	{
		get
		{
			return ParseAsWebPurchaseRedemptionCallback__BackingField;
		}
		set
		{
			ParseAsWebPurchaseRedemptionCallback__BackingField = value;
		}
	}

	private RedeemWebPurchaseFunc RedeemWebPurchaseCallback
	{
		get
		{
			return RedeemWebPurchaseCallback__BackingField;
		}
		set
		{
			RedeemWebPurchaseCallback__BackingField = value;
		}
	}

	private GetVirtualCurrenciesFunc GetVirtualCurrenciesCallback
	{
		get
		{
			return GetVirtualCurrenciesCallback__BackingField;
		}
		set
		{
			GetVirtualCurrenciesCallback__BackingField = value;
		}
	}

	private GetEligibleWinBackOffersForProductFunc GetEligibleWinBackOffersForProductCallback
	{
		get
		{
			return GetEligibleWinBackOffersForProductCallback__BackingField;
		}
		set
		{
			GetEligibleWinBackOffersForProductCallback__BackingField = value;
		}
	}

	private GetEligibleWinBackOffersForPackageFunc GetEligibleWinBackOffersForPackageCallback
	{
		get
		{
			return GetEligibleWinBackOffersForPackageCallback__BackingField;
		}
		set
		{
			GetEligibleWinBackOffersForPackageCallback__BackingField = value;
		}
	}

	private void Start()
	{
	}

	private void Configure(string newUserId)
	{
	}

	public void Setup(PurchasesConfiguration purchasesConfiguration)
	{
	}

	public void Configure(PurchasesConfiguration purchasesConfiguration)
	{
	}

	private bool IsAndroidEmulator()
	{
		return false;
	}

	public void GetStorefront(GetStorefrontFunc callback)
	{
	}

	public void GetProducts(string[] products, GetProductsFunc callback, string type = "subs")
	{
	}

	public void PurchaseProduct(string productIdentifier, MakePurchaseFunc callback, string type = "subs", string oldSku = null, ProrationMode prorationMode = ProrationMode.UnknownSubscriptionUpgradeDowngradePolicy, bool googleIsPersonalizedPrice = false)
	{
	}

	public void PurchaseDiscountedProduct(string productIdentifier, PromotionalOffer discount, MakePurchaseFunc callback)
	{
	}

	public void PurchasePackage(Package package, MakePurchaseFunc callback, string oldSku = null, ProrationMode prorationMode = ProrationMode.UnknownSubscriptionUpgradeDowngradePolicy, bool googleIsPersonalizedPrice = false)
	{
	}

	public void PurchaseDiscountedPackage(Package package, PromotionalOffer discount, MakePurchaseFunc callback)
	{
	}

	public void PurchaseSubscriptionOption(SubscriptionOption subscriptionOption, MakePurchaseFunc callback, GoogleProductChangeInfo googleProductChangeInfo = null, bool googleIsPersonalizedPrice = false)
	{
	}

	public void RestorePurchases(CustomerInfoFunc callback)
	{
	}

	public void AddAttributionData(string dataJson, AttributionNetwork network, string networkUserId = null)
	{
	}

	public void LogIn(string appUserId, LogInFunc callback)
	{
	}

	public void LogOut(CustomerInfoFunc callback)
	{
	}

	public void SetAllowSharingStoreAccount(bool allow)
	{
	}

	public string GetAppUserId()
	{
		return null;
	}

	public bool IsAnonymous()
	{
		return false;
	}

	public bool IsConfigured()
	{
		return false;
	}

	public void SetDebugLogsEnabled(bool logsEnabled)
	{
	}

	public void SetLogLevel(LogLevel level)
	{
	}

	public void SetLogHandler(LogHandlerFunc logHandler)
	{
	}

	public void GetCustomerInfo(CustomerInfoFunc callback)
	{
	}

	public void GetOfferings(GetOfferingsFunc callback)
	{
	}

	public void GetCurrentOfferingForPlacement(string placementIdentifier, GetCurrentOfferingForPlacementFunc callback)
	{
	}

	public void SyncAttributesAndOfferingsIfNeeded(SyncAttributesAndOfferingsIfNeededFunc callback)
	{
	}

	public void SyncPurchases()
	{
	}

	public void SyncPurchases(CustomerInfoFunc callback)
	{
	}

	public void SyncObserverModeAmazonPurchase(string productID, string receiptID, string amazonUserID, string isoCurrencyCode, double price)
	{
	}

	public void SyncAmazonPurchase(string productID, string receiptID, string amazonUserID, string isoCurrencyCode, double price)
	{
	}

	public void EnableAdServicesAttributionTokenCollection()
	{
	}

	public void CheckTrialOrIntroductoryPriceEligibility(string[] products, CheckTrialOrIntroductoryPriceEligibilityFunc callback)
	{
	}

	public void InvalidateCustomerInfoCache()
	{
	}

	public void PresentCodeRedemptionSheet()
	{
	}

	public void RecordPurchase(string productID, RecordPurchaseFunc callback)
	{
	}

	public void SetSimulatesAskToBuyInSandbox(bool askToBuyEnabled)
	{
	}

	public void SetAttributes(Dictionary<string, string> attributes)
	{
	}

	public void SetEmail(string email)
	{
	}

	public void SetPhoneNumber(string phoneNumber)
	{
	}

	public void SetDisplayName(string displayName)
	{
	}

	public void SetPushToken(string token)
	{
	}

	public void SetAdjustID(string adjustID)
	{
	}

	public void SetAppsflyerID(string appsflyerID)
	{
	}

	public void SetFBAnonymousID(string fbAnonymousID)
	{
	}

	public void SetMparticleID(string mparticleID)
	{
	}

	public void SetOnesignalID(string onesignalID)
	{
	}

	public void SetOnesignalUserID(string onesignalUserID)
	{
	}

	public void SetAirshipChannelID(string airshipChannelID)
	{
	}

	public void SetCleverTapID(string cleverTapID)
	{
	}

	public void SetMixpanelDistinctID(string mixpanelDistinctID)
	{
	}

	public void SetFirebaseAppInstanceID(string firebaseAppInstanceID)
	{
	}

	public void SetMediaSource(string mediaSource)
	{
	}

	public void SetCampaign(string campaign)
	{
	}

	public void SetAdGroup(string adGroup)
	{
	}

	public void SetAd(string ad)
	{
	}

	public void SetKeyword(string keyword)
	{
	}

	public void SetCreative(string creative)
	{
	}

	public void CollectDeviceIdentifiers()
	{
	}

	public void CanMakePayments(BillingFeature[] features, CanMakePaymentsFunc callback)
	{
	}

	public void CanMakePayments(CanMakePaymentsFunc callback)
	{
	}

	public void GetAmazonLWAConsentStatus(GetAmazonLWAConsentStatusFunc callback)
	{
	}

	public void GetPromotionalOffer(StoreProduct storeProduct, Discount discount, GetPromotionalOfferFunc callback)
	{
	}

	public void ShowInAppMessages(InAppMessageType[] messageTypes = null)
	{
	}

	public void TrackCustomPaywallImpression(CustomPaywallImpressionParams parameters)
	{
	}

	public void TrackCustomPaywallImpression()
	{
	}

	public void ParseAsWebPurchaseRedemption(string urlString, ParseAsWebPurchaseRedemptionFunc callback)
	{
	}

	public void RedeemWebPurchase(WebPurchaseRedemption webPurchaseRedemption, RedeemWebPurchaseFunc callback)
	{
	}

	public void GetVirtualCurrencies(GetVirtualCurrenciesFunc callback)
	{
	}

	public VirtualCurrencies GetCachedVirtualCurrencies()
	{
		return null;
	}

	public void InvalidateVirtualCurrenciesCache()
	{
	}

	public void GetEligibleWinBackOffersForProduct(StoreProduct storeProduct, GetEligibleWinBackOffersForProductFunc callback)
	{
	}

	public void GetEligibleWinBackOffersForPackage(Package package, GetEligibleWinBackOffersForPackageFunc callback)
	{
	}

	public void PurchaseProductWithWinBackOffer(StoreProduct storeProduct, WinBackOffer winBackOffer, MakePurchaseFunc callback)
	{
	}

	public void PurchasePackageWithWinBackOffer(Package package, WinBackOffer winBackOffer, MakePurchaseFunc callback)
	{
	}

	private void _receiveStorefront(string storefrontJson)
	{
	}

	private void _receiveProducts(string productsJson)
	{
	}

	private void _getCustomerInfo(string customerInfoJson)
	{
	}

	private void _makePurchase(string makePurchaseResponseJson)
	{
	}

	private void _receiveCustomerInfo(string customerInfoJson)
	{
	}

	private void _handleLog(string logDetailsJson)
	{
	}

	private void _restorePurchases(string customerInfoJson)
	{
	}

	private void _syncPurchases(string customerInfoJson)
	{
	}

	private void _logIn(string logInResultJson)
	{
	}

	private void _logOut(string customerInfoJson)
	{
	}

	private void _getOfferings(string offeringsJson)
	{
	}

	private void _getCurrentOfferingForPlacement(string offeringJson)
	{
	}

	private void _syncAttributesAndOfferingsIfNeeded(string offeringsJson)
	{
	}

	private void _checkTrialOrIntroductoryPriceEligibility(string json)
	{
	}

	private void _recordPurchase(string json)
	{
	}

	private void _canMakePayments(string canMakePaymentsJson)
	{
	}

	private void _getAmazonLWAConsentStatus(string getAmazonLWAConsentStatusJson)
	{
	}

	private void _getPromotionalOffer(string getPromotionalOfferJson)
	{
	}

	private void _parseAsWebPurchaseRedemption(string parseAsWebPurchaseRedemptionJSON)
	{
	}

	private void _redeemWebPurchase(string redeemWebPurchaseJSON)
	{
	}

	private void _getVirtualCurrencies(string getVirtualCurrenciesJson)
	{
	}

	private void _getEligibleWinBackOffersForProduct(string eligibleWinBackOffersJson)
	{
	}

	private void _getEligibleWinBackOffersForPackage(string eligibleWinBackOffersJson)
	{
	}

	private void _purchaseProductWithWinBackOffer(string purchaseProductWithWinBackOfferJson)
	{
	}

	private void _purchasePackageWithWinBackOffer(string purchasePackageWithWinBackOfferJson)
	{
	}

	private static void ReceiveCustomerInfoMethod(string arguments, CustomerInfoFunc callback)
	{
	}

	private static void ReceiveLogInResultMethod(string arguments, LogInFunc callback)
	{
	}

	private static bool ResponseHasError(JSONNode response)
	{
		return false;
	}
}
