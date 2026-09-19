using System.Runtime.InteropServices;
using RevenueCat;

public class PurchasesWrapperiOS : IPurchasesWrapper
{
	private class ProductsRequest
	{
		public string[] productIdentifiers;
	}

	private class CanMakePaymentsRequest
	{
		public int[] features;
	}

	private class ShowInAppMessagesRequest
	{
		public int[] messageTypes;
	}

	[PreserveSig]
	private static extern void _RCSetupPurchases(string gameObject, string apiKey, string appUserId, string purchasesAreCompletedBy, string storeKitVersion, string userDefaultsSuiteName, string dangerousSettingsJson, bool shouldShowInAppMessagesAutomatically, string entitlementVerificationMode);

	public void Setup(string gameObject, string apiKey, string appUserId, Purchases.PurchasesAreCompletedBy purchasesAreCompletedBy, Purchases.StoreKitVersion storeKitVersion, string userDefaultsSuiteName, bool useAmazon, string dangerousSettingsJson, bool shouldShowInAppMessagesAutomatically, bool pendingTransactionsForPrepaidPlansEnabled)
	{
	}

	public void Setup(string gameObject, string apiKey, string appUserId, Purchases.PurchasesAreCompletedBy purchasesAreCompletedBy, Purchases.StoreKitVersion storeKitVersion, string userDefaultsSuiteName, bool useAmazon, string dangerousSettingsJson, bool shouldShowInAppMessagesAutomatically, Purchases.EntitlementVerificationMode entitlementVerificationMode, bool pendingTransactionsForPrepaidPlansEnabled)
	{
	}

	[PreserveSig]
	private static extern void _RCGetStorefront();

	public void GetStorefront()
	{
	}

	[PreserveSig]
	private static extern void _RCGetProducts(string productIdentifiersJson, string type);

	public void GetProducts(string[] productIdentifiers, string type = "subs")
	{
	}

	[PreserveSig]
	private static extern void _RCPurchaseProduct(string productIdentifier, string signedDiscountTimestamp);

	public void PurchaseProduct(string productIdentifier, string type = "subs", string oldSku = null, Purchases.ProrationMode prorationMode = Purchases.ProrationMode.UnknownSubscriptionUpgradeDowngradePolicy, bool googleIsPersonalizedPrice = false, string presentedOfferingIdentifier = null, Purchases.PromotionalOffer discount = null)
	{
	}

	[PreserveSig]
	private static extern void _RCPurchasePackage(string packageIdentifier, string presentedOfferingContextJSON, string signedDiscountTimestamp);

	public void PurchasePackage(Purchases.Package packageToPurchase, string oldSku = null, Purchases.ProrationMode prorationMode = Purchases.ProrationMode.UnknownSubscriptionUpgradeDowngradePolicy, bool googleIsPersonalizedPrice = false, Purchases.PromotionalOffer discount = null)
	{
	}

	public void PurchaseSubscriptionOption(Purchases.SubscriptionOption subscriptionOption, Purchases.GoogleProductChangeInfo googleProductChangeInfo = null, bool googleIsPersonalizedPrice = false)
	{
	}

	[PreserveSig]
	private static extern void _RCRestorePurchases();

	public void RestorePurchases()
	{
	}

	[PreserveSig]
	private static extern void _RCSyncPurchases();

	public void SyncPurchases()
	{
	}

	public void SyncAmazonPurchase(string productID, string receiptID, string amazonUserID, string isoCurrencyCode, double price)
	{
	}

	public void GetAmazonLWAConsentStatus()
	{
	}

	[PreserveSig]
	private static extern void _RCLogIn(string appUserId);

	public void LogIn(string appUserId)
	{
	}

	[PreserveSig]
	private static extern void _RCLogOut();

	public void LogOut()
	{
	}

	[PreserveSig]
	private static extern void _RCSetAllowSharingStoreAccount(bool allow);

	public void SetAllowSharingStoreAccount(bool allow)
	{
	}

	[PreserveSig]
	private static extern void _RCSetDebugLogsEnabled(bool enabled);

	public void SetDebugLogsEnabled(bool enabled)
	{
	}

	[PreserveSig]
	private static extern void _RCSetLogLevel(string level);

	public void SetLogLevel(Purchases.LogLevel level)
	{
	}

	[PreserveSig]
	private static extern void _RCSetLogHandler();

	public void SetLogHandler()
	{
	}

	[PreserveSig]
	private static extern void _RCSetProxyURLString(string proxyURL);

	public void SetProxyURL(string proxyURL)
	{
	}

	[PreserveSig]
	private static extern string _RCGetAppUserID();

	public string GetAppUserId()
	{
		return null;
	}

	[PreserveSig]
	private static extern void _RCGetCustomerInfo();

	public void GetCustomerInfo()
	{
	}

	[PreserveSig]
	private static extern void _RCGetOfferings();

	public void GetOfferings()
	{
	}

	[PreserveSig]
	private static extern void _RCGetCurrentOfferingForPlacement(string placementIdentifier);

	public void GetCurrentOfferingForPlacement(string placementIdentifier)
	{
	}

	[PreserveSig]
	private static extern void _RCSyncAttributesAndOfferingsIfNeeded();

	public void SyncAttributesAndOfferingsIfNeeded()
	{
	}

	[PreserveSig]
	private static extern void _RCEnableAdServicesAttributionTokenCollection();

	public void EnableAdServicesAttributionTokenCollection()
	{
	}

	[PreserveSig]
	private static extern bool _RCIsAnonymous();

	public bool IsAnonymous()
	{
		return false;
	}

	[PreserveSig]
	private static extern bool _RCIsConfigured();

	public bool IsConfigured()
	{
		return false;
	}

	[PreserveSig]
	private static extern void _RCCheckTrialOrIntroductoryPriceEligibility(string productIdentifiersJson);

	public void CheckTrialOrIntroductoryPriceEligibility(string[] productIdentifiers)
	{
	}

	[PreserveSig]
	private static extern void _RCInvalidateCustomerInfoCache();

	public void InvalidateCustomerInfoCache()
	{
	}

	[PreserveSig]
	private static extern void _RCPresentCodeRedemptionSheet();

	public void PresentCodeRedemptionSheet()
	{
	}

	[PreserveSig]
	private static extern void _RCRecordPurchase(string productID);

	public void RecordPurchase(string productID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetSimulatesAskToBuyInSandbox(bool enabled);

	public void SetSimulatesAskToBuyInSandbox(bool enabled)
	{
	}

	[PreserveSig]
	private static extern void _RCSetAttributes(string attributesJson);

	public void SetAttributes(string attributesJson)
	{
	}

	[PreserveSig]
	private static extern void _RCSetEmail(string email);

	public void SetEmail(string email)
	{
	}

	[PreserveSig]
	private static extern void _RCSetPhoneNumber(string phoneNumber);

	public void SetPhoneNumber(string phoneNumber)
	{
	}

	[PreserveSig]
	private static extern void _RCSetDisplayName(string displayName);

	public void SetDisplayName(string displayName)
	{
	}

	[PreserveSig]
	private static extern void _RCSetPushToken(string token);

	public void SetPushToken(string token)
	{
	}

	[PreserveSig]
	private static extern void _RCSetAdjustID(string adjustID);

	public void SetAdjustID(string adjustID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetAppsflyerID(string appsflyerID);

	public void SetAppsflyerID(string appsflyerID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetFBAnonymousID(string fbAnonymousID);

	public void SetFBAnonymousID(string fbAnonymousID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetMparticleID(string mparticleID);

	public void SetMparticleID(string mparticleID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetOnesignalID(string onesignalID);

	public void SetOnesignalID(string onesignalID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetOnesignalUserID(string onesignalUserID);

	public void SetOnesignalUserID(string onesignalUserID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetAirshipChannelID(string airshipChannelID);

	public void SetAirshipChannelID(string airshipChannelID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetCleverTapID(string cleverTapID);

	public void SetCleverTapID(string cleverTapID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetMixpanelDistinctID(string mixpanelDistinctID);

	public void SetMixpanelDistinctID(string mixpanelDistinctID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetFirebaseAppInstanceID(string firebaseAppInstanceID);

	public void SetFirebaseAppInstanceID(string firebaseAppInstanceID)
	{
	}

	[PreserveSig]
	private static extern void _RCSetMediaSource(string mediaSource);

	public void SetMediaSource(string mediaSource)
	{
	}

	[PreserveSig]
	private static extern void _RCSetCampaign(string campaign);

	public void SetCampaign(string campaign)
	{
	}

	[PreserveSig]
	private static extern void _RCSetAdGroup(string adGroup);

	public void SetAdGroup(string adGroup)
	{
	}

	[PreserveSig]
	private static extern void _RCSetAd(string ad);

	public void SetAd(string ad)
	{
	}

	[PreserveSig]
	private static extern void _RCSetKeyword(string keyword);

	public void SetKeyword(string keyword)
	{
	}

	[PreserveSig]
	private static extern void _RCSetCreative(string creative);

	public void SetCreative(string creative)
	{
	}

	[PreserveSig]
	private static extern void _RCCollectDeviceIdentifiers();

	public void CollectDeviceIdentifiers()
	{
	}

	[PreserveSig]
	private static extern void _RCCanMakePayments(string featuresJson);

	public void CanMakePayments(Purchases.BillingFeature[] features)
	{
	}

	[PreserveSig]
	private static extern void _RCGetPromotionalOffer(string productIdentifier, string discountIdentifier);

	public void GetPromotionalOffer(string productIdentifier, string discountIdentifier)
	{
	}

	[PreserveSig]
	private static extern void _RCShowInAppMessages(string messagesJson);

	public void ShowInAppMessages(Purchases.InAppMessageType[] messageTypes)
	{
	}

	[PreserveSig]
	private static extern void _RCParseAsWebPurchaseRedemption(string urlString);

	public void ParseAsWebPurchaseRedemption(string urlString)
	{
	}

	[PreserveSig]
	private static extern void _RCRedeemWebPurchase(string resultJson);

	public void RedeemWebPurchase(Purchases.WebPurchaseRedemption webPurchaseRedemption)
	{
	}

	[PreserveSig]
	private static extern void _RCGetVirtualCurrencies();

	public void GetVirtualCurrencies()
	{
	}

	[PreserveSig]
	private static extern string _RCGetCachedVirtualCurrencies();

	public string GetCachedVirtualCurrencies()
	{
		return null;
	}

	[PreserveSig]
	private static extern void _RCInvalidateVirtualCurrenciesCache();

	public void InvalidateVirtualCurrenciesCache()
	{
	}

	[PreserveSig]
	private static extern void _RCGetEligibleWinBackOffersForProduct(string productIdentifier);

	public void GetEligibleWinBackOffersForProduct(Purchases.StoreProduct storeProduct)
	{
	}

	[PreserveSig]
	private static extern void _RCGetEligibleWinBackOffersForPackage(string productIdentifier);

	public void GetEligibleWinBackOffersForPackage(Purchases.Package package)
	{
	}

	[PreserveSig]
	private static extern void _RCPurchaseProductWithWinBackOffer(string productIdentifier, string winBackOfferIdentifier);

	public void PurchaseProductWithWinBackOffer(Purchases.StoreProduct storeProduct, Purchases.WinBackOffer winBackOffer)
	{
	}

	[PreserveSig]
	private static extern void _RCPurchasePackageWithWinBackOffer(string packageIdentifier, string presentedOfferingContextJson, string winBackOfferIdentifier);

	public void PurchasePackageWithWinBackOffer(Purchases.Package package, Purchases.WinBackOffer winBackOffer)
	{
	}

	[PreserveSig]
	private static extern void _RCTrackCustomPaywallImpression(string paywallId, string offeringId);

	public void TrackCustomPaywallImpression(Purchases.CustomPaywallImpressionParams parameters)
	{
	}

	[PreserveSig]
	private static extern void _RCTrackAdDisplayed(string dataJson);

	public void TrackAdDisplayed(AdDisplayedData data)
	{
	}

	[PreserveSig]
	private static extern void _RCTrackAdOpened(string dataJson);

	public void TrackAdOpened(AdOpenedData data)
	{
	}

	[PreserveSig]
	private static extern void _RCTrackAdRevenue(string dataJson);

	public void TrackAdRevenue(AdRevenueData data)
	{
	}

	[PreserveSig]
	private static extern void _RCTrackAdLoaded(string dataJson);

	public void TrackAdLoaded(AdLoadedData data)
	{
	}

	[PreserveSig]
	private static extern void _RCTrackAdFailedToLoad(string dataJson);

	public void TrackAdFailedToLoad(AdFailedToLoadData data)
	{
	}
}
