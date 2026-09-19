# Third-party packages: what is installed, what still has to be imported

The shipped build (`car точно финал`, `Data/Managed/Metadata/global-metadata.dat` assembly table,
`Podfile.lock`, `Libraries/`) links the versions below. The recovered project only had decompiled
skeletons of these assemblies (every method `return null;`). Per the remake rules third-party code is
never rewritten; the real packages are installed instead.

## Installed (stage 5, exact shipped versions)

| Assembly | Version | Source | Where |
|---|---|---|---|
| Supabase | 1.1.1 | nuget.org | `Assets/Plugins/Supabase/` |
| Supabase.Gotrue | 6.0.3 | nuget.org | `Assets/Plugins/Supabase/` |
| Supabase.Postgrest | 4.0.3 | nuget.org | `Assets/Plugins/Supabase/` |
| Supabase.Realtime | 7.0.2 | nuget.org | `Assets/Plugins/Supabase/` |
| Supabase.Storage | 2.0.2 | nuget.org | `Assets/Plugins/Supabase/` |
| Supabase.Functions | 2.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| Supabase.Core | 1.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| Websocket.Client | 5.1.1 | nuget.org | `Assets/Plugins/Supabase/` |
| MimeMapping | 3.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| System.IdentityModel.Tokens.Jwt + Microsoft.IdentityModel.{JsonWebTokens,Tokens,Logging,Abstractions} | 7.5.1 | nuget.org | `Assets/Plugins/Supabase/` |
| System.Reactive | 6.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| Microsoft.Extensions.Logging.Abstractions | 8.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.0 (transitive) | nuget.org | `Assets/Plugins/Supabase/` |
| Microsoft.IO.RecyclableMemoryStream | 3.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| System.Threading.Channels | 8.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| System.Text.Json / System.Text.Encodings.Web | 4.7.2 (assembly 4.0.1 / 4.0.5) | nuget.org | `Assets/Plugins/Supabase/` |
| Microsoft.Bcl.AsyncInterfaces | 1.1.0 (transitive) | nuget.org | `Assets/Plugins/Supabase/` |
| System.Runtime.CompilerServices.Unsafe | 6.0.0 | nuget.org | `Assets/Plugins/Supabase/` |
| Newtonsoft.Json | 13.0 | `com.unity.nuget.newtonsoft-json` 3.2.2 (already in the manifest) | UPM |
| RevenueCat purchases-unity | 9.1.0 (PurchasesHybridCommon 18.8.0, RevenueCat iOS 5.74.0) | github.com/RevenueCat/purchases-unity release `Purchases.unitypackage` | `Assets/RevenueCat/`, `Assets/ExternalDependencyManager/` (EDM4U 1.2.187) |
| Unity LevelPlay (Ads Mediation) | 9.5.0 (IronSourceSDK 9.5.0.0) | Unity registry `com.unity.services.levelplay` | `Packages/manifest.json` |
| LevelPlay Unity Ads adapter | 5.11.0.0 (iOS pod IronSourceUnityAdsAdapter 5.9.0.0, UnityAds 4.19.0) | LevelPlay dependency XMLs | `Assets/LevelPlay/Editor/*.xml` |
| ShadowRunNativeAuth.mm (the game's own iOS plugin: haptics, in-app browser, Apple / web sign-in, audio session) | shipped | `car точно финал/Libraries/Plugins/iOS` | `Assets/Plugins/iOS/` |

`Assets/Resources/supabase.json` and `revenuecat.json` are the shipped configs (ripped from the build).

Verification: `RevenueCat/Plugins/iOS/PurchasesUnityHelper.m` and the LevelPlay `Runtime/Plugins/iOS/*` sources
are byte-identical to the ones in the shipped Xcode project. NuGet lib picks: `netstandard2.1` where the
package ships it, else `netstandard2.0` (Unity 6 API compatibility level .NET Standard 2.1).

## Still decompiled skeletons — import from the Asset Store

These two are paid/free Asset Store packages that cannot be fetched without the store account, so their
recovered stubs stay in place (the project compiles; the prefabs and ScriptableObjects that reference them
keep their script GUIDs):

| Package | Stub folder | Used by |
|---|---|---|
| Cartoon FX Remaster (JMO Assets) — `CFXRRuntime.dll`: `CFXR_Effect`, `CFXR_ParticleText`, `CFXR_ParticleTextFontAsset`, `CFXR_EmissionBySurface` | `Assets/Scripts/CFXRRuntime/` | the CFXR prefabs under `Assets/Resources/jmo assets/cartoon fx remaster/` (`CFXR Explosion 1`, `CFXR3 Fire Explosion B`, `CFXR3 _WOW_`, `CFXR _POW_`, `CFXR4 Falling Stars`) |
| Dynamic Weather System — `DynamicWeatherSystem.Runtime.dll`: `WeatherManager`, `WeatherStateData`, `SkyModule`, `LightModule`, `FogModule`, `RainModule`, `AudioModule`, `StarfieldModule`, `WeatherModule`, `WeatherDemoUI` | `Assets/Scripts/DynamicWeatherSystem.Runtime/` | `ShadowRunPrototype.WeatherPresets/Sky/SkyEffects/Audio` (`BuildWeatherPresets`, `SetWeatherState`, `weatherManager.CurrentState`, `AudioModule.SetMuted`) |

Effect while the stubs are in: CFXR particle systems still play (Unity-native), but `CFXR_Effect` behaviour
(camera shake, auto-destruct, particle text) is inert; the weather presets are built but `WeatherManager`
never transitions the sky/fog/light/rain — maps look alike until the real package is imported.

Import procedure (both packages):
1. Package Manager → My Assets → import the package into the project (accept its default folder).
2. Delete the stub folder listed above **but keep its `.meta` GUIDs**: for every stub script, copy the
   `guid:` from `Assets/Scripts/<Stub>/…/<Class>.cs.meta` into the imported package's `<Class>.cs.meta`
   (one class per file in both), then delete the stub folder. This keeps the prefab / `.asset` references
   (`m_Script: {fileID: 11500000, guid: …}`) pointing at the real scripts.
   Alternatively run `tools/remake/relink_stub_guids.py <StubFolder> <ImportedFolder>` which does the same.
3. Reimport; check `Assets/Resources/jmo assets/...` prefabs show `CFXR_Effect` (not "Missing script") and
   `WeatherPresets` assets load.

## iOS build notes
- Pods come from EDM4U (`Assets/ExternalDependencyManager`): `PurchasesHybridCommon 18.8.0`,
  `IronSourceSDK 9.5.0.0`, `IronSourceUnityAdsAdapter 5.9.0.0` — the same set as the shipped `Podfile`
  (platform iOS 15.0, static linkage). Run EDM4U's *iOS Resolver → Install Cocoapods* / let the post-build
  step generate the Podfile.
- `ShadowRunNativeAuth.mm` needs `AuthenticationServices`, `SafariServices`, `AVFoundation` frameworks
  (Unity links them automatically from the `#import`s when the plugin is in `Assets/Plugins/iOS`).
