#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <AuthenticationServices/AuthenticationServices.h>
#import <SafariServices/SafariServices.h>
#import <AVFoundation/AVFoundation.h>
#import <CommonCrypto/CommonDigest.h>

extern "C" void UnitySendMessage(const char *obj, const char *method, const char *msg);

extern "C" void ShadowRun_PlayImpactHaptic(int strength)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        UIImpactFeedbackStyle style = UIImpactFeedbackStyleLight;
        if (strength >= 2)
            style = UIImpactFeedbackStyleHeavy;
        else if (strength == 1)
            style = UIImpactFeedbackStyleMedium;

        UIImpactFeedbackGenerator *generator = [[UIImpactFeedbackGenerator alloc] initWithStyle:style];
        [generator prepare];
        [generator impactOccurred];
    });
}

static NSString *const SRAuthGameObject = @"Supabase Game Manager";

static void SRSendToUnity(NSString *method, NSString *message)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        UnitySendMessage(SRAuthGameObject.UTF8String, method.UTF8String,
                         (message ?: @"").UTF8String);
    });
}

static UIWindow *SRKeyWindow(void)
{
    if (@available(iOS 13.0, *)) {
        for (UIScene *scene in UIApplication.sharedApplication.connectedScenes) {
            if (scene.activationState != UISceneActivationStateUnattached &&
                [scene isKindOfClass:UIWindowScene.class]) {
                for (UIWindow *window in ((UIWindowScene *)scene).windows)
                    if (window.isKeyWindow) return window;
            }
        }
    }
    for (UIWindow *window in UIApplication.sharedApplication.windows)
        if (window.isKeyWindow) return window;
    return UIApplication.sharedApplication.windows.firstObject;
}

static UIViewController *SRTopViewController(void)
{
    UIViewController *controller = SRKeyWindow().rootViewController;
    while (controller.presentedViewController)
        controller = controller.presentedViewController;
    return controller;
}

@interface SRLegalBrowserDelegate : NSObject <SFSafariViewControllerDelegate>
@end

static SFSafariViewController *SRLegalBrowser;
static SRLegalBrowserDelegate *SRLegalBrowserDelegateInstance;

@implementation SRLegalBrowserDelegate
- (void)safariViewControllerDidFinish:(SFSafariViewController *)controller
{
    SRLegalBrowser = nil;
}
@end

extern "C" void ShadowRun_OpenInAppBrowser(const char *url)
{
    NSString *urlValue = url ? [NSString stringWithUTF8String:url] : @"";
    NSURL *targetURL = [NSURL URLWithString:urlValue];
    if (!targetURL || ![targetURL.scheme.lowercaseString isEqualToString:@"https"])
        return;

    dispatch_async(dispatch_get_main_queue(), ^{
        if (SRLegalBrowser != nil) return;
        UIViewController *presenter = SRTopViewController();
        if (presenter == nil) return;

        SRLegalBrowserDelegateInstance = [SRLegalBrowserDelegate new];
        SRLegalBrowser = [[SFSafariViewController alloc] initWithURL:targetURL];
        SRLegalBrowser.delegate = SRLegalBrowserDelegateInstance;
        SRLegalBrowser.dismissButtonStyle = SFSafariViewControllerDismissButtonStyleClose;
        [presenter presentViewController:SRLegalBrowser animated:YES completion:nil];
    });
}

static UIAlertController *SRAccountConfirmationAlert;

extern "C" void ShadowRun_ShowAccountConfirmation(int action, const char *unityGameObjectName)
{
    NSString *targetName = unityGameObjectName
        ? [NSString stringWithUTF8String:unityGameObjectName]
        : @"Shadow Run Prototype";

    dispatch_async(dispatch_get_main_queue(), ^{
        // Repeated taps must not stack several destructive system alerts.
        if (SRAccountConfirmationAlert != nil)
            return;

        BOOL deleting = action == 1;
        NSString *title = deleting ? @"Delete Account?" : @"Sign Out?";
        NSString *message = deleting
            ? @"This permanently deletes your cars, crystals and login. This cannot be undone."
            : @"Are you sure you want to sign out of this account?";
        NSString *confirmTitle = deleting ? @"Delete Account" : @"Sign Out";
        NSString *callback = deleting
            ? @"OnNativeProfileDeleteConfirmed"
            : @"OnNativeProfileSignOutConfirmed";

        UIAlertController *alert = [UIAlertController
            alertControllerWithTitle:title
            message:message
            preferredStyle:UIAlertControllerStyleAlert];
        SRAccountConfirmationAlert = alert;

        [alert addAction:[UIAlertAction
            actionWithTitle:@"Cancel"
            style:UIAlertActionStyleCancel
            handler:^(__unused UIAlertAction *selectedAction) {
                SRAccountConfirmationAlert = nil;
            }]];
        [alert addAction:[UIAlertAction
            actionWithTitle:confirmTitle
            style:UIAlertActionStyleDestructive
            handler:^(__unused UIAlertAction *selectedAction) {
                SRAccountConfirmationAlert = nil;
                UnitySendMessage(targetName.UTF8String, callback.UTF8String, "");
            }]];

        UIViewController *presenter = SRTopViewController();
        if (presenter == nil) {
            SRAccountConfirmationAlert = nil;
            return;
        }
        [presenter presentViewController:alert animated:YES completion:nil];
    });
}

extern "C" void ShadowRun_ConfigureGameAudioSession(void)
{
    dispatch_async(dispatch_get_main_queue(), ^{
        AVAudioSession *session = AVAudioSession.sharedInstance;
        [session setCategory:AVAudioSessionCategoryAmbient
                 withOptions:AVAudioSessionCategoryOptionMixWithOthers
                       error:nil];
        [session setActive:YES error:nil];
    });
}

extern "C" int ShadowRun_IsOtherAudioPlaying(void)
{
    return AVAudioSession.sharedInstance.secondaryAudioShouldBeSilencedHint ? 1 : 0;
}

static NSString *SRSHA256(NSString *value)
{
    NSData *data = [value dataUsingEncoding:NSUTF8StringEncoding];
    unsigned char digest[CC_SHA256_DIGEST_LENGTH];
    CC_SHA256(data.bytes, (CC_LONG)data.length, digest);
    NSMutableString *result = [NSMutableString stringWithCapacity:CC_SHA256_DIGEST_LENGTH * 2];
    for (NSInteger index = 0; index < CC_SHA256_DIGEST_LENGTH; index++)
        [result appendFormat:@"%02x", digest[index]];
    return result;
}

@interface SRNativeAuthDelegate : NSObject
    <ASAuthorizationControllerDelegate,
     ASAuthorizationControllerPresentationContextProviding,
     ASWebAuthenticationPresentationContextProviding>
@property(nonatomic, copy) NSString *appleRawNonce;
@end

@implementation SRNativeAuthDelegate
- (ASPresentationAnchor)presentationAnchorForAuthorizationController:(ASAuthorizationController *)controller
{
    return SRKeyWindow();
}

- (ASPresentationAnchor)presentationAnchorForWebAuthenticationSession:(ASWebAuthenticationSession *)session API_AVAILABLE(ios(13.0))
{
    return SRKeyWindow();
}

- (void)authorizationController:(ASAuthorizationController *)controller didCompleteWithAuthorization:(ASAuthorization *)authorization
{
    if (![authorization.credential isKindOfClass:ASAuthorizationAppleIDCredential.class]) {
        SRSendToUnity(@"OnNativeAuthError", @"Apple returned an unsupported credential.");
        return;
    }
    ASAuthorizationAppleIDCredential *credential = (ASAuthorizationAppleIDCredential *)authorization.credential;
    NSString *identityToken = credential.identityToken
        ? [[NSString alloc] initWithData:credential.identityToken encoding:NSUTF8StringEncoding]
        : nil;
    if (identityToken.length == 0 || self.appleRawNonce.length == 0) {
        SRSendToUnity(@"OnNativeAuthError", @"Apple did not return a valid identity token.");
        return;
    }
    NSString *payload = [NSString stringWithFormat:@"%@\n%@", identityToken, self.appleRawNonce];
    self.appleRawNonce = nil;
    SRSendToUnity(@"OnNativeAppleCredential", payload);
}

- (void)authorizationController:(ASAuthorizationController *)controller didCompleteWithError:(NSError *)error
{
    self.appleRawNonce = nil;
    if (error.code == ASAuthorizationErrorCanceled)
        SRSendToUnity(@"OnNativeAuthCancelled", @"Apple sign in was cancelled.");
    else
        SRSendToUnity(@"OnNativeAuthError", error.localizedDescription);
}
@end

static SRNativeAuthDelegate *SRAuthDelegate;
static ASWebAuthenticationSession *SRWebAuthSession;

extern "C" void ShadowRun_StartAppleSignIn(const char *rawNonce)
{
    // P/Invoke owns rawNonce only for the duration of this native call. Copy it
    // before dispatching; otherwise the main-thread block can read freed memory.
    NSString *nonce = rawNonce ? [NSString stringWithUTF8String:rawNonce] : @"";
    if (nonce == nil) nonce = @"";
    dispatch_async(dispatch_get_main_queue(), ^{
        if (@available(iOS 13.0, *)) {
            if (nonce.length == 0) {
                SRSendToUnity(@"OnNativeAuthError", @"Apple sign in nonce is missing.");
                return;
            }
            SRAuthDelegate = [SRNativeAuthDelegate new];
            SRAuthDelegate.appleRawNonce = nonce;
            ASAuthorizationAppleIDProvider *provider = [ASAuthorizationAppleIDProvider new];
            ASAuthorizationAppleIDRequest *request = [provider createRequest];
            request.requestedScopes = @[ASAuthorizationScopeFullName, ASAuthorizationScopeEmail];
            request.nonce = SRSHA256(nonce);
            ASAuthorizationController *controller = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[request]];
            controller.delegate = SRAuthDelegate;
            controller.presentationContextProvider = SRAuthDelegate;
            [controller performRequests];
        } else {
            SRSendToUnity(@"OnNativeAuthError", @"Sign in with Apple requires iOS 13 or newer.");
        }
    });
}

extern "C" void ShadowRun_StartWebAuth(const char *url, const char *callbackScheme)
{
    // The marshalled UTF-8 buffers are temporary as well. Materialize owned
    // NSString values synchronously before the asynchronous main-thread hop.
    NSString *urlValue = url ? [NSString stringWithUTF8String:url] : @"";
    NSString *scheme = callbackScheme ? [NSString stringWithUTF8String:callbackScheme] : @"";
    if (urlValue == nil) urlValue = @"";
    if (scheme == nil) scheme = @"";
    dispatch_async(dispatch_get_main_queue(), ^{
        if (@available(iOS 12.0, *)) {
            NSURL *authorizationURL = [NSURL URLWithString:urlValue];
            if (!authorizationURL || scheme.length == 0) {
                SRSendToUnity(@"OnNativeAuthError", @"The authorization URL is invalid.");
                return;
            }
            if (!SRAuthDelegate) SRAuthDelegate = [SRNativeAuthDelegate new];
            [SRWebAuthSession cancel];
            SRWebAuthSession = [[ASWebAuthenticationSession alloc]
                initWithURL:authorizationURL
                callbackURLScheme:scheme
                completionHandler:^(NSURL *callbackURL, NSError *error) {
                    SRWebAuthSession = nil;
                    if (callbackURL)
                        SRSendToUnity(@"OnNativeOAuthCallback", callbackURL.absoluteString);
                    else if (error.code == ASWebAuthenticationSessionErrorCodeCanceledLogin)
                        SRSendToUnity(@"OnNativeAuthCancelled", @"Sign in was cancelled.");
                    else
                        SRSendToUnity(@"OnNativeAuthError", error.localizedDescription);
                }];
            if (@available(iOS 13.0, *)) {
                SRWebAuthSession.presentationContextProvider = SRAuthDelegate;
                SRWebAuthSession.prefersEphemeralWebBrowserSession = NO;
            }
            if (![SRWebAuthSession start]) {
                SRWebAuthSession = nil;
                SRSendToUnity(@"OnNativeAuthError", @"Could not open the secure sign-in window.");
            }
        } else {
            SRSendToUnity(@"OnNativeAuthError", @"Secure sign in requires iOS 12 or newer.");
        }
    });
}
