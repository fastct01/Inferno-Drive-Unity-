# Remake assumptions

Decisions taken where the shipped build's source is genuinely unrecoverable or where the
target Unity version (6000.6.2f1) forbids the original API. Each entry names the method,
what the shipped build did, and what the remake does instead.

## Native plugin entry points (editor / non-iOS)
`ShadowRun_PlayImpactHaptic`, `ShadowRun_ShowAccountConfirmation`, `ShadowRun_ConfigureGameAudioSession`,
`ShadowRun_IsOtherAudioPlaying`, `ShadowRun_OpenInAppBrowser`, `ShadowRun_StartAppleSignIn`, `ShadowRun_StartWebAuth`
are `[DllImport("__Internal")]` into `Libraries/Plugins/iOS/ShadowRunNativeAuth.mm`. Cpp2IL only preserved
`[PreserveSig]`, so the attribute was restored by hand. Outside an iOS player the symbols do not exist; the
remake compiles no-op fallbacks (`ShadowRun_IsOtherAudioPlaying` returns 0) under `#else` so the editor can
play. The shipped build never ran outside iOS, so this is unobservable in the product.

## `Object.GetInstanceID()` (UpdateTrafficAi turn-signal blink phase)
The shipped code offset each car's indicator blink by `|GetInstanceID()| * 0.017`. Unity 6000.6 makes
`GetInstanceID()` and the `EntityId -> int` conversion obsolete-as-error. The remake uses
`unchecked((int)GetEntityId().GetRawData())`, whose low 32 bits carry the instance id. Instance ids differ
per session anyway, so only the "each car blinks out of phase" behaviour is observable, and that is kept.

## Dead code left by the post-source patches
Where a patch (`InfernoDrivingRules`, `InfernoDriving`, …) overwrote a value that the generated code still
computes (e.g. `UpdatePlayer`'s original cruise `Lerp(29, 40, distance/2600)` and the linear
`MoveTowards` acceleration), the remake keeps the shipped (patched) behaviour and records the original
expression in a `// original:` comment instead of executing it.

## Active input handling set to "Both"
The recovered `ProjectSettings.asset` had `activeInputHandler: 0` (legacy Input Manager only), but the
shipped IL2CPP build links both `UnityEngine.InputLegacyModule` (used by `GarageDebugger`,
`PerformanceProfiler`, `SimpleCameraController` via `Input.GetKey*`) and `Unity.InputSystem` (used by
`ShadowRunPrototype.KeyHeld/KeyDown`, touch reading, the tutorial's `Touchscreen.current` / `Mouse.current`).
Set to `2` (Both) so the translated code receives input from both APIs like the shipped build.

## AdvancedCameraController adaptive quality
`Update` reads `bool adaptive = false;` — the shipped C++ was patched (`apply_lobby_patches.py`) to replace
`__this->___adaptiveQuality` with a constant `false`, disabling adaptive resolution toggling. The original
read is kept as an `// original:` comment.

## Third-party SDK sources in the recovered project are stubs
`Assets/Scripts/{Supabase*, Websocket.Client, revenuecat.purchases-unity, Unity.LevelPlay, CFXRRuntime,
DynamicWeatherSystem.Runtime}` are decompiled skeletons (method bodies `return null;`). The shipped build
links these exact versions (from global-metadata assembly definitions):
Supabase 1.1.1, Supabase.Gotrue 6.0.3, Supabase.Postgrest 4.0.3, Supabase.Realtime 7.0.2,
Supabase.Storage 2.0.2, Supabase.Functions 2.0.0, Supabase.Core 1.0.0, Websocket.Client 5.1.1,
MimeMapping 3.0.0, Newtonsoft.Json 13.0 (com.unity.nuget.newtonsoft-json 3.2.2),
plus revenuecat.purchases-unity and Unity.LevelPlay (versions not recorded in metadata).
The game code is translated against the public API of these stubs; the real packages are to be installed
in place of the stub folders (stage 5/6) — the stubs must not be "implemented" by hand.

## IL2CPP interface slot numbers
`InterfaceFuncInvokerN::Invoke(slot, IFoo_il2cpp_TypeInfo_var, ...)` slots were decoded with
`scratchpad/meta.py type <Interface>` against the shipped `global-metadata.dat` (v39 layout with
compressed index widths), not against the recovered stub sources, whose member order can differ.

## Hand-written patch namespaces (`Inferno*.inc`) → nested static partial classes
The C++ patch namespaces `InfernoLobby/Settings/Driving/Hazards/Garage/Style` are ported one-to-one into
`Assets/Scripts/Assembly-CSharp/Inferno/*.cs` as `private static partial class` members of `ShadowRunPrototype`
(so they reach the game's private fields the way the C++ reached `___field` members directly). Mechanical
differences, all behaviour-neutral:
- `Init()` calls (`il2cpp_codegen_initialize_runtime_metadata`) have no C# equivalent and are dropped.
- Closed-instance delegate trampolines (`Hook<Fn>` / `Trampoline<Fn>` building a `MethodInfo` by hand,
  `ChipClick<C,S>` / `PageClick<C,D>` templates) become `onClick.AddListener(() => Fn(game))` lambdas.
- `Keep(field, value)` (assignment + write barrier) is a plain assignment.
- `std::fmod` → C# `%` on floats; `-std::expm1(-x)` → `1f - Mathf.Exp(-x)`.
- `InfernoGarage::Utf8` narrowed resource paths to ASCII (`?` for non-ASCII) for its fixed `char[160]`
  cache keys; the C# `Picture` cache keys on the same narrowed string and keeps the 48-slot limit.
- `Data/Raw/InfernoLobby/*.png` (PNG files in the iOS app bundle, decoded natively) are shipped as
  `Assets/Resources/InfernoLobby/**.bytes` and decoded with `Texture2D.LoadImage` (`InfernoLobby.LoadTexture`).
- The `extern "C" InfernoGarage_IsUnlocked/SetUnlocked` bridge (cross-TU call from the profile code) is
  `ShadowRunPrototype.InfernoGarageIsUnlocked/SetUnlocked` in `Inferno/InfernoGarageBridge.cs`.
- Unity "alive" checks (`Object_op_Inequality(x, null)`) map to the overloaded `!= null`; where the C++ first
  tested the raw pointer, C# uses `ReferenceEquals` for that test (InfernoStyle.Wave).

## Third-party packages installed vs. still stubbed
Stage 5 installed the exact shipped versions of the Supabase stack (NuGet DLLs in `Assets/Plugins/Supabase`),
RevenueCat purchases-unity 9.1.0 (GitHub release) and Unity LevelPlay 9.5.0 (UPM) — versions read from the
metadata assembly table, `Podfile.lock` and `PurchasesUnityHelper.m`; see `docs/third-party-imports.md`.
Cartoon FX Remaster and Dynamic Weather System are Asset Store packages that cannot be downloaded here, so
their decompiled skeletons remain (with the ripped script GUIDs) until they are imported by hand; the doc
gives the relink procedure (`tools/remake/relink_stub_guids.py`). The Android UnityAds adapter version
(5.11.0 / unity-ads 4.19.0) is the LevelPlay 9.5.0 pairing of the shipped iOS pod 5.9.0.0 — the shipped
build was iOS-only, so no Android adapter version is recorded anywhere.
