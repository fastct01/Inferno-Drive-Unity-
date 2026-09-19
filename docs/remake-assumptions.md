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
