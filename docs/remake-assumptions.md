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

## Rendering setup recovered from the shipped build (stage 7 parity pass)
The ripper left every package-script asset as an empty shell with a made-up script GUID: the URP pipeline
assets, renderer data, volume profile, Bloom, SSAO feature and the scene's UniversalAdditionalCameraData /
UniversalAdditionalLightData / Volume components had no fields and did not bind to the package scripts, so
the game ran on the built-in pipeline and `UniversalAdditionalCameraData.cameraStack` threw. The shipped
build was made with Unity 6000.4.5f1 (URP 17.4); its serialized objects were read back from
`Data/data.unity3d` (UnityPy raw bytes + the 6000.4 field layouts from the IL2CPP structs) and rewritten as
17.6 YAML with the real package GUIDs, same asset GUIDs:
- `Mobile_RPAsset` (the only quality level, "Mobile"): Forward renderer, no renderer features, native
  render pass, HDR on, MSAA off, render scale 0.86, FSR upscaling with sharpness override 0.82, depth
  texture on, opaque texture off, shadows off (main/additional/soft), 1 cascade, shadow distance 28,
  additional lights per object 4, reflection probe blending/box/atlas on, SRP batcher + dynamic batching,
  adaptive performance on, LUT 32, LOD cross-fade blue noise; `PC_RPAsset` differs in 2x bilinear opaque
  downsampling, 2048 main shadowmap, cascade splits, biases 0.1/0.5, cookie 2048/format 3, and its renderer
  (`PC_Renderer`) is Forward+ with the ScreenSpaceAmbientOcclusion feature (intensity 0.4, radius 0.3,
  direct lighting 0.25, falloff 100, depth-normals source, 1 sample, blur low) and stencil ref 1 / Equal.
- Volume profile `SampleSceneProfile` = one Bloom: threshold 1.1, intensity 0.35, scatter 0.5,
  skipIterations 0, dirt intensity 0, high-quality filtering off (overrides), clamp/tint/downscale/
  maxIterations left at defaults (tint value 0.877/0.203/0.203 not overridden). The scene's Global Volume
  is global, priority 0, weight 1 with this profile; the camera data has post-processing on, volume layer
  mask Default, TAA quality 3 / 0.1 / 0.9 (unused), light data defaults.
- The renderer's `postProcessData` points at the URP package asset (the ripped copy was empty).

## Shaders: the game's own five re-authored from Metal source, third-party ones still compiled-only
Ripped shaders are compiled YAML shader assets (iOS Metal programs only) that cannot render in the editor
or on other platforms. Their LZ4 `compressedBlob` still holds the Metal *source*, so the game's own
shaders were re-authored as HLSL from it, keeping the ripped GUIDs: `ShadowRun/TrafficLampUnlit`,
`ShadowRun/VehicleSmokeMobile`, `ShadowRun/RampUnlit`, `ShadowRun/SolidColorSkybox`, `Inferno Drive/UI
Grayscale` (render state, tags, queues, property defaults and the exact fragment maths are in each file's
header comment). `Universal Render Pipeline/Lit|Unlit`, the `Hidden/…` URP/core shaders (package copies) and
`Shader Graphs/glTF-pbrMetallicRoughness` (from `com.unity.cloud.gltfast` 6.20.0, editor-only dependency —
the shipped build linked no glTFast assembly, only its shader) were deleted and the 109 + 20 materials
remapped to the package GUIDs; `VolumetricSpotlight` is the original shader graph from
github.com/Josephy5/FakeVolumetricLight-Unity. Still compiled-only (Asset Store packages to import):
Cartoon FX Remaster (`Cartoon FX/Remaster/*`, 193 materials), FastSky (`GapperGames/…`), Pulsar Neutron
Star (`Shader Graphs/SkyBox_SH|SunCore|HaloPulse`), MonoLimboStudio (`Shader Graphs/VolumLight[ Triplanir]`).
The shipped build never contained `ShadowRun/Sky360NoFog`, `ShadowRun/Sky360CloudLayer` or
`UI/IconAlphaTint`: the code's `Shader.Find` fallbacks (URP/Unlit, no tint material) are what shipped.

## Android export (the shipped build was iOS-only)
`UnityEngine.iOS.Device.deferSystemGesturesMode` (LockPortraitOrientation) is compiled only for iOS. EDM4U's
own Gradle 5.1.1 cannot run on the editor's JDK 17 (Groovy `Java7` NoClassDefFoundError), so the Android
resolver is switched to template patching (`ProjectSettings/GvhProjectSettings.xml`, Unity's Gradle templates
copied to `Assets/Plugins/Android/`) and `InfernoBuild.BuildAndroid` forces `PlayServicesResolver.MenuForceResolve`
before `BuildPlayer`. The UnityAds Android adapter version (5.11.0 / unity-ads 4.19.0) is LevelPlay 9.5.0's
pairing of the shipped iOS pod 5.9.0.0.

## User-requested deviations from the shipped build
- 2026-09-20 "remove welcome screen": `FirstLaunchAuthScreen.Enabled = false` — the first-launch WELCOME /
  sign-in screen is never shown (not at boot, not from the case reward, purchases, sign-out or account
  deletion). The game starts as a local, unauthenticated player; the profile page still offers CREATE
  ACCOUNT / LOG IN. Flip the constant to restore the shipped flow.
- 2026-09-20 "add more lighting on the cars" (tagged `/* [user request 2026-09-20: brighter cars] */`,
  original values kept in `// original:` comments):
  - garage preview (`MakeGaragePreviewWorld`): ceiling spot 45°/4.8 → 56°/6.4; the "Weak Front Fill"
    point light (0.8) became a 4.5 cool fill at (-3.2, 2.1, -3.2); a new warm "Garage Preview Key Light"
    spot (9.5, 70°) from the camera side; `StyleVehicleForGarage` darkens the paint 8 % instead of 18 %.
  - in-game: a warm "Player Showcase Light" spot parented to the player root (`MakePlayerShowcaseLight`,
    11 / 72°); `MakeLights` / `ApplyVisualQuality` key light and ambient raised, and — because
    `UpdateSceneLighting` overrides both every frame while the Dynamic Weather System is not ready — the
    per-frame pulse too (key 0.95/1.12/1.28 → 1.45/1.7/1.95, colour floor (0.82,0.18,0.16) → (0.9,0.3,0.26),
    ambient roughly ×1.7).
  - **Editor-only correctness fix found on the way:** `StyleVehicleForGarage` / `StyleGarageScene` mutate
    *shared* materials (lerp toward black, `_EMISSION`, queue 2000, `_Surface` 0) on every preview rebuild.
    A player reloads the assets fresh on each launch, but in the editor the writes land on the `.mat`
    assets and were saved to disk, so repeated play sessions had converged the sportcar paint
    (`main.mat` (0.106, 0.262, 0.416) → (0.015, 0.012, 0.014)) and the glass to opaque black — that, not
    the light count, was why the cars looked black. `tools/remake/restore_material_colours.py` restored
    50 materials from the baseline commit (keeping the URP shader relink), and
    `Assets/Editor/Build/InfernoMaterialGuard.cs` now snapshots every material asset when play mode
    starts, restores it when play mode ends and blocks saving those assets while playing. The shipped
    AMG materials (`W222 - Car Color.mat` = (0.015, 0.012, 0.014)) were already converged in the shipped
    data — the developer's editor had the same problem — and are left as shipped (the editions paint
    them through property blocks anyway).
- 2026-09-20 "road has a more road-like gray texture" (tagged `[user request 2026-09-20: gray road]`):
  `ApplyRoadTexture` tint (0.0175, 0.019, 0.021) → (0.3, 0.34, 0.41), smoothness 0.58 → 0.4, and a faint
  emission masked by the asphalt map (`ApplyRoadEmission`, (0.4, 0.48, 0.58)) so the gray survives the red
  key light; `DisableRoadTexture` flat colour → the same gray; a procedural asphalt texture
  (`MakeProceduralAsphaltTexture`) replaces the flat plane when the Resources map is missing.
- 2026-09-20 "pause menu follows the rest of the design" (tagged `[user request 2026-09-20: pause/UI fixes]`):
  `MakePauseActions` now builds the pause screen through `InfernoLobby.BuildPause` in the loss screen's
  language (black backdrop, PAUSED / THE CITY IS STILL MOVING / rule, neon stats card with DISTANCE and
  BEST RECORD, CONTINUE DRIVING + BACK TO MENU profile-style buttons, the sound toggle as a 104 px neon
  frame top-left); `PauseRun` calls `InfernoLobby.ShowPause` (blanks the shared overlay title/tag, fills
  the card) instead of setting the white "P A U S E D" texts. Object names (`Pause Continue`,
  `Pause Back To Menu`, `Pause Sound Toggle`) and all field wiring are unchanged.
- 2026-09-20 "fix text running into boxes": gem counter stone 56 px @ x−88 → 48 px @ x−76 (the crystal
  icon no longer overlaps the frame edge); STYLE sheet grown 790 → 920 px with the three rows, title,
  close button, chips, captions and notice re-spaced, chip names best-fit 18→13 px, swatches shrunk,
  and the sheet moved onto its own root with a black plate; case result bonus pill text best-fit 14–19.
- 2026-09-21/22 "3D interface, six different maps, ten car models with rarities / paint / upgrades, transparent
  pause, no overlapping elements" (tagged `[user request 2026-09-21: 3D UI | maps | cars | style | transparent
  pause | no overlaps]`, originals kept in `// original:` comments; plan of record
  `~/.claude/plans/bubbly-plotting-wilkinson.md`). Four parallel work streams plus an orchestrator:
  - **UI kit** `Inferno/InfernoUi.cs`: procedural chamfered ("cut-corner") 9-slice panels (asymmetric 50/18,
    28/10, 14/6 px cuts, violet contour + glow + lit top edge, drop shadow), rhomboid tabs with icons and a
    glowing active bar, rhombus-knob sliders on glowing tracks, ON/OFF pill toggle, segment bars, bold-italic
    headings and letter-spaced section labels, `UiPressFeedback` press-down. `InfernoLobby.NeonFrameSprite /
    NeonPlateSprite / GlassFrameSprite` and `ButtonOn` route through the kit, so every existing frame, button and
    tab switched at once; `MakeProfileActionButton`, the HUD BEAM / pause buttons, the shop and case buttons were
    retrofitted. The Settings page (`InfernoSettings` / `Profile.cs`) follows the user's mockup: SETTINGS heading,
    ACCOUNT card with `GUEST_ID_<8 digits>` (`InfernoDrive.GuestId`) and an avatar button that opens the auth
    form, AUDIO card with the three sliders, GAME card with the VIBRATION toggle; the auth form / account actions
    no longer sit on the audio and game sections (`InfernoSettings.Arrange`).
  - **Pause** is see-through: `Pause Actions/Shade` (0.03, 0.02, 0.07, 0.62) over the still-rendering world; the
    menu details line and the gem counter are hidden by `ShowPause` and restored by `HidePause` (also fired by a
    `UiScreenLink` when the pause root or the game-over screen is hidden).
  - **Maps** `Inferno/InfernoMaps.cs`: a map is a full environment profile now (the weather stub never did
    anything). Procedural 1024x512 panorama skies on `Skybox/Panoramic` (nebula PNG for PLANET HORIZON), fog /
    ambient / key light, road surface kinds Asphalt / Wet / Sand / Snow / Grid (textures generated at runtime,
    `RoadTextureTiling` kept so the scroll still works), lane / edge / verge colours, shipped buildings, fences and
    lamps toggled and tinted, primitive scenery under `cityRoot` registered in `cityObjects` (trees, totems, guard
    rails, dunes / rocks / cacti, pines and snow banks, crystal spires and floating rocks), camera-following
    particles (rain + splashes, snow, dust, motes), lightning flashes on STORM CHASE, aurora drift, rain loop.
    Hooks: `Awake` after `MakeCar`, `InfernoLobby.ApplySelectedMap`, `UpdateWorld` → `Tick`, `Freeze` in
    Start/Pause/ResumeRun; the shipped per-frame red overrides (`UpdateSceneLighting`, `EnsureWeatherSkyboxActive`,
    `ApplyFastSkyRuntime`, `ForceFastSkyDayClouds`, `ApplyVisualQuality`) yield while `InfernoMaps.OwnsLighting /
    OwnsSky` — NEON NIGHT alone keeps the shipped pulse. Names, prices and unlock keys are unchanged; the six card
    pictures `Resources/InfernoLobby/<slug>.bytes` are rendered from the maps themselves (`Inferno Drive > Capture
    Map Thumbnails`, `InfernoLobby.MapSlugs`).
  - **Cars** `Inferno/InfernoCars.cs` + `InfernoUpgrades.cs`: the 29 entries (4 base + 25 recolour "editions")
    became 10 models in price order — 5 meshes (sportcar, W222, Cayman, P900, flying car) + 5 kit-bashed variants
    built from primitives at `CreateConfiguredVehicleModel` (PHANTOM RS, SOVEREIGN ARMOUR, CAYMAN CLUBSPORT, P900
    SPYDER, AERO WRAITH X). Four rarities COMMON / RARE / EPIC / LEGENDARY colour the badges and set the prices
    (0 starter, 120, 300, 340, 650, 700, 750, 1200, 1300, 1500); the literal 100-crystal price became
    `InfernoCars.Price`; case prizes pick a locked car weighted 40/30/20/10 by rarity (`CaseVehiclePrizeChance`
    untouched). `InfernoGarage`'s entry points forward to the catalogue; the four shipped ids keep the cloud
    columns, the other six use `InfernoDrive.Vehicle.<id>.Unlocked`. The P900's origin-pivoted wheels got real
    pivots so they spin and take tyre styles. Upgrades ENGINE / HANDLING / NITRO, levels 0-5,
    `InfernoDrive.Upgrade.<id>.<Stat>`, price = round(base × (1 + 0.5·level)) with base 40 / 60 / 90 / 130 per
    rarity; applied in `UpdatePlayer` through `InfernoDrivingRules.Cruise / CruiseTop / BoostTop (…, bonus)` and
    `AdvanceSpeed (…, boostRateMultiplier)` (old signatures delegate). Garage: rarity badge, price row, three
    mini bars per card, tuning card with segment bars, PURCHASE / STYLE action row; locked cards use a dark tint
    instead of the "249" blur strip. `GarageVehicleCardData` is `internal` now (the catalogue returns it).
  - **Style** `Inferno/InfernoStyle.cs`: tabbed sheet TIRES / FLAGS / TRAILS / PAINT (rhomboid tabs, 2x4 chips,
    pager), modal over the garage (carousel and header hidden while open). PAINT: 12 colours bought once
    (`InfernoDrive.Style.Paint.<i>.Owned`), chosen per car (`InfernoDrive.Paint.<carId>.Selected`), applied by
    `InfernoCars.Decorate` with metallic / smoothness. Trails: textured HDR ribbons with width and alpha
    gradients, a hot core, spark particles, and a static ribbon behind the garage preview car; tyres: instanced
    metallic / emissive rim materials (shared assets untouched); flags: 256x160 texture on a 12x6 cloth mesh
    with a vertex wave, lit, chrome pole (shorter in the garage preview so it stays under the card carousel).
  - **Layout audit** `Inferno/InfernoLayoutAudit.cs` (+ `Inferno Drive > Audit Layout`, PlayMode test
    `LayoutAuditTests.NoOverlapsOnAnyScreen`): sibling overlaps and texts leaving their drawn box on every screen
    and state. It found and we fixed: goal clock / timer touch, long car names wrapping into the subtitle, the gem
    shop BEST tag on the pack art, the STYLE sheet over the carousel. The case screen's caption above `CaseDisplay`
    and its 3D render layer are intentional and excluded.
  - **Editor tooling** `Assets/Editor/Build/InfernoTestRunner.cs` (`Inferno Drive > Run EditMode / PlayMode Tests /
    Run Layout Audit Test`, summary in `Logs/inferno-tests-<mode>.txt`). Caveat learned the hard way: inside the
    open Editor the Supabase client can still be signed in from the user's play session, so the play-mode smoke
    tests (goal claims, a case opening, a garage purchase) would run against the real cloud profile — on
    2026-09-22 one such run pushed test balances (crystals 350 → 275) to user d746d42c…; the runner now refuses
    PlayMode runs while signed in (`PlayModeRunIsSafe`); use `tools/remake/test.sh PlayMode` (batch, session
    moved aside) for the smoke suite. Also: the unfocused Editor does not advance the player loop at all
    (`Time.frameCount` frozen even with `runInBackground`), so scripted tours step it with
    `EditorApplication.Step()`.
  - Not done / assumptions: the 6 maps are the existing six rebuilt (not six more); paint is a STYLE tab, not a
    garage strip; the game-over screen stays opaque; the lobby home page / dock / goal rows / gem counter restyle
    is the other session's (commit 7402436) and was only nudged (clock / timer 8 px apart).
- 2026-09-22 "each map has 5 lanes" (tagged `[user request 2026-09-22: five lanes]`, originals kept in
  `// original:` comments; two parallel streams, road/world and traffic/player). Shared geometry lives in
  `Assets/Scripts/Assembly-CSharp/Inferno/InfernoLanes.cs` so no shipped constant had to change:
  lanes `{-6.2, -3.1, 0, 3.1, 6.2}` at the shipped 3.1 m pitch, `RoadWidth` 10.2 → 17.0 (`HalfRoad` 8.5),
  `Shift` 3.4 = how far every roadside element moves out so it keeps its shipped distance from the edge.
  - **Road / world**: edge lines ±5.1 → ±8.5; two lane marks at ±1.55 → four dividers at ±1.55 / ±4.65;
    verges ±12.09 → ±15.49; neon fences 6.08 → 9.48; street lamps 5.72 → 9.12; buildings 12.5 → 15.9;
    traffic-light signals 6.7 → 10.1 (never built); reflection-probe box 34 → 40.8. The asphalt would have
    stretched, so its x tiling goes 1.35 → 2.25 (`InfernoLanes.TextureTiling`; the y tiling and the whole
    `UpdateRoadTexture` scroll maths are untouched).
  - **Maps**: scenery bands move out by `Shift` with the rng draw order unchanged (so each map's layout is the
    shipped one, just wider); the Snow road's tyre tracks are drawn for five lanes (texture 256 → 512 px wide so
    the grain keeps its size); the Grid road recomputes its cell count to keep 1.89 m squares; particle emitter
    boxes widen by 2 × `Shift` with rate and max particles scaled the same way, so density per metre holds.
  - **Gameplay**: player limits ±3.85 → ±6.95 (and the literals tied to them: edge push-back 3.45 →
    `PlayerMaxX - 0.4`, stops ±3.84 → `PlayerMin/MaxX ∓ 0.01`). Traffic is tuned **per lane**, not per road:
    the pool (shipped const `TrafficPoolSize` 12, untouched) is scaled to 20 by `InfernoDrivingRules.TrafficPoolFor`
    and every spawn interval by `SpawnIntervalScale(5)` = 0.6, so cars-per-lane-per-second is the shipped value;
    `SeedInitialTraffic` seeds 6–10 cars. The 16 shipped spawn formations are three-lane shapes (`0/1/2`, `side` /
    `2 - side` mirrors), so each one now lands whole on a random three-lane **window** of the five
    (`FormationOffset` / `FormationLane`) instead of being rewritten — the shapes and mirroring survive.
    Ramps are single-lane and would otherwise be met 3/5 as often, so their gap is ×0.6 (1000–1200 → 600–720 m).
    Puddle and crystal gaps are deliberately left as shipped (crystals stay reachable; more room to dodge puddles
    is the point of five lanes).
  - **High beams**: the shipped yield rule was "always move left, right only from lane 0" — on five lanes that
    pushed middle-lane traffic across the player's half and gave up when that lane was busy. It now picks either
    in-range neighbour, the safe one, and of two safe ones the one farther from the player; shipped left-first
    order when neither is safe.
  - **Wrecking balls** only dipped to car height for |x| < 5.47 m, i.e. they could not reach the outer lanes:
    gantry 10.6 → 17.67 m, posts ±9.6 → ±13.0, rope 16.27 m (the shipped 1.4 m below the beam), amplitude
    62° → 45° so the ball still turns short of the posts. Reach |x| < 7.4 m, same 2.7 s rhythm.
  - **Camera** (`UpdateCamera` / `ResetCameraImmediate`): FOV base 54.2 → 60.2, height +0.6, distance +1.8
    (reset (0, 2.05, 11.1) → (0, 2.65, 12.9), FOV 54.5 → 60.5); the speed / jump / drift modulation, the
    smoothing and the look target are the shipped ones. Measured at 1080×1920 this shows ±3.2 m beside the car
    (shipped ±2.28) and the whole 17 m road from ~15.5 m ahead. **Showing ±4.5 m at the car is not possible
    without changing the game's look** — portrait needs roughly +20° FOV or +7 m of boom — so it was not done.
  - Verified: 44/44 EditMode tests (3 new — lane geometry, traffic rules, wrecking-ball reach), live runs on
    NEON NIGHT and FALLEN SKY captured, traffic seen on all five lane centres. The lobby map cards were
    re-rendered from the five-lane maps (`Inferno Drive > Capture Map Thumbnails`). Note for later: the gantry
    posts stand at ±13.0 m, inside the scenery band (rocks 11.4–18.4, pines 12.2–15, crystal spires 11.9–20.4),
    so a prop can occasionally share that spot.
- 2026-09-24 "fix the models of the cars (no extra add-ons), fix the extra pluses in the shop, improve the STYLE
  UI, and on NEON NIGHT change from day to night like the first generation" (tags `[user request 2026-09-24: plain
  cars | style sheet | neon night cycle]`, originals kept in `// original:` comments):
  - **Plain cars** `Inferno/InfernoCars.cs`: the five editions (PHANTOM RS, SOVEREIGN ARMOUR, CAYMAN CLUBSPORT,
    P900 SPYDER, AERO WRAITH X) no longer bolt primitive body kits onto their base meshes — `ApplyKit`, the
    `Builder` / `Measure` machinery and the five `Build*` recipes are gone; an edition is the plain model with its
    own factory paint, stance (ride height, wheel scale) and stats, and its card icon is still rendered from the
    preview world. `HasKit` became `IsVariant`. The "+" classes on their garage cards were the "extra pluses":
    "Wide-body / Class S+" → "Sport Edition / Class S", "Armoured / Class A+" → "Raised Edition / Class A",
    "Cup / Class S+" → "Cup Edition / Class S", "Open Proto / Class S+" → "Gold Edition / Class S". The gem
    counter's "+" key (the peer session's lobby restyle) opens the shop and was left alone.
  - **STYLE sheet** `Inferno/InfernoStyle.cs`: a bottom sheet (920 x 840, bottom edge 196 px above the page bottom)
    instead of a 920 x 920 sheet centred over the car. `OpenSheet` scrolls the garage's full-page 3D preview image
    up by 540 px (`LiftPreview`: the car through the RawImage's uvRect so the rect stays the page backdrop, the
    platform - the page's `garage12` background image - through its rect offsets); the MY GARAGE header stays visible so the car and its platform sit in the free upper band and every tyre / flag /
    trail / paint pick shows on the car at once; `CloseSheet` puts it back. Header: 40 px title, car-name
    subtitle, close key, a thin rule; tabs are 84 tall with a swatch glyph (rim / checkered flag / streak / paint
    sphere) over the label; chips get a soft halo in the item's colour, the lock badge moved to the chip's
    top-right corner so the locked art stays visible (shade 0.58 → 0.5); a permanent hint line ("TAP AN ITEM TO
    EQUIP IT - LOCKED ITEMS COST CRYSTALS") shares the slot under the pager with the 2.2 s notices. Button names
    (`StyleButton`, `Tab *`, `Chip`, `Prev`, `Next`, `Close`) are unchanged for the tests.
  - **Neon Night day/night cycle** `Inferno/InfernoMaps.cs` + `Assets/Shader/ShadowRun_PanoramicBlend.shader` +
    `Resources/Inferno/SkyBlend.mat`: the shipped build changed its weather during a run through the Dynamic
    Weather System (a preset every `WeatherPhaseDuration` 35 s, faded over 4.25 s, 4 s night → day), which is a
    stub here, so NEON NIGHT never left the night. The profile now carries a `Day` profile (the shipped Day preset's
    red inferno sky, red fog, key (1, 0.5, 0.36) from (18, -32, 0), ambient (0.22, 0.10, 0.11), lamps off, fence
    glow 6.2 → 2.2) and a `Cycle` state machine (`Advance`, shipped durations as constants) fades sky (a
    two-panorama blend shader with Skybox/Panoramic's maths), fog, camera background, key light, ambient, street
    lamps and fences while a run is in progress; at pure night nothing is touched and the shipped red pulse runs
    as before (`ShippedLightPulse` in Player.cs is that pulse as values, so the blend can mix it). Every run starts
    at night; the first dawn comes 35 s in. Only NEON NIGHT cycles — the other five maps were rebuilt as distinct
    environments on 2026-09-21 and stay static. Editor hook: `SendMessage("DebugAdvanceNeonCycle", seconds)`.
  - Tests: `InfernoCarsTests` / `InfernoGarageStyleTests` follow `IsVariant`; `InfernoMapsTests.NeonNightCyclesBetweenNightAndDay`
    covers the timing.
- 2026-09-22/26 "make the lobby look like the mockup" (tagged `[user request 2026-09-22: lobby mockup]`, originals in
  `// original:` comments; `Inferno/InfernoLobbyStyle.cs` + the home-page regions of `Inferno/InfernoLobby.cs` only —
  garage, settings, dock, pause and game-over untouched). The user's second mockup replaced the violet neon frames of the
  home page with a "steel" language: near-black (10,10,14) panels with a thin (118,118,134) rim lit (170,170,185)
  along the top-left edges, cut corners big at top-left / bottom-right and tiny at top-right / bottom-left, magenta only
  on PLAY, progress fills, gem icons and the active dock underline. `CutSprite(w,h,tl,tr,br,bl,Look)` rasterises each
  box at its exact size with independent (possibly asymmetric) cuts, so no 9-slice geometry is reused. The RacingLobby
  root became 1032x1668 so `Layout` scales it to exactly 1 at 1080x1920 (positions below are canvas px):
  gem counter 283x84 at (767..1050, 25..109) with a 66 px "+" key; map selector 1000x161 at y 143..304 with 110 px
  arrow keys, "MAP n / 6" letter-spaced 17 px and the name bold italic 48 px left-aligned at x 224; the picture a plain
  straight-cornered 1000x476 rectangle (no frame, no accent bar) with a steel LOCKED plate; `MapDescription` kept as a
  20 px status line at y 826 (the smoke test reads it); PLAY a 960x212 octagon (cuts TL 92x103, TR 48x46, BR 60x63,
  BL 32x46) with the magenta gradient, light sweep, scratches, lighter edge line and a 24 px glow, label 74 px; case card
  356x483 (cuts 37/6/35/6; 18/0/18/0 since 2026-09-27, see "lobby corners") with the "///" mark, the crate render cropped to its body (`CaseArtCrop`), name 27 px, the
  daily-case caption and an outlined OPEN CASE; goals card 615x483 (cuts 46/8/46/8; 18/0/18/0 since 2026-09-27) with the title at x 461, the
  clock + timer right-aligned to x 1004, rows at y 1274/1392/1513 with bare line-art icons (no tiles), 25/20 px texts,
  220x7 bars and outlined 107x60 GO / CLAIM / DONE keys. `InfernoTracking` (a BaseMeshEffect) letter-spaces labels whose
  text other code writes plainly (timer, caption) and re-centres them from the glyph extents. Verified by a
  capture → three-lens review → fix loop (case art was half size in round 1 — fixed); round 2 measured every box, cut,
  colour and text within ±2 px of the spec. The mockup image itself was lost in a reboot; `scratchpad/LOBBY-SPEC.md`
  (this session's measurements) was the reference for the reviewers. Left as is: the "+" key later lost its plate at
  the user's request to the other session (kept as an invisible hit area).
- 2026-09-26 "in night drive the sky is much better quality, and come up with obstacles and complications for the
  map gameplay" (tag `[user request 2026-09-26: night drive]`, originals in `// original:` comments):
  - **Sky quality** `Inferno/InfernoMaps.cs`: NEON NIGHT (and its Day profile) render their panorama at 2048x1024
    (`Profile.SkyDetail = 2`; the other maps stay at 1024x512), with an ordered-hash dither before the 8-bit store
    (no banding in the dark gradient), stars at 0.9 (was 0.6) whose count scales with the panorama area while the
    discs stay a pixel or two wide (sharp points, power-curve brightness, 5 % soft big stars, four-point glints on
    the brightest), a faint violet galactic band (haze + a dense field of small stars, `Galaxy = 0.55`), thin high
    wisps lit from below by the city (`Wisps = 0.42`), a skyline-modulated light-pollution band on the horizon
    (`HorizonGlow`), aniso 8, and the map's `Moon` prop turned on. Generation time is logged
    (`[InfernoMaps] sky '...' generated in N ms`).
  - **Night hazards** `Inferno/InfernoNightHazards.cs` (NEON NIGHT only, `Profile.NightHazards`): BLACKOUT zones
    (160-240 m, first at 700-1000 m then every 900-1400 m, only while the cycle is on the night side) where lamps,
    fences, lane lines and building windows flicker out over 28 m, the fog closes to 40-190 m, ambient and key light
    drop, then the power flickers back - `InfernoMaps.SetBlackout` applies it on top of the day/night blend and
    `OwnsLighting` is true while it lasts; ROADBLOCKS (first at 450-700 m, then every 550-900 m, never within 260 m
    of a blackout): road-works barriers (striped planks, legs, cones, amber blinkers) closing 2-3 lanes, POLICE
    checkpoints (patrol cars with red/blue light bars) closing 3, and DEBRIS (3-4 tyre stacks / crates over distinct
    lanes). A barrier or patrol car is a whole heart and speed x0.45 ("ROADBLOCK!" / "CHECKPOINT!"), debris half a
    heart and x0.72 ("DEBRIS!", a jump clears it), cones just bump. Traffic in a closed lane 4-75 m before a row is
    queued one lane toward the nearest open lane (`QueueTrafficLaneChange`) and `TrafficSpawnBlockedByRamp` now also
    asks `InfernoNightHazards.LaneBlocked`, so nothing spawns into a barrier and ramps / gantries keep clear.
    Debris carries reflective tape (emissive bands) so it reads on the dark road. Lifecycle hooks sit next to
    InfernoHazards' (Boot.cs Reset/Hide/Dispose, World.cs Tick). Editor hooks: `SendMessage("DebugNightRow", kind)` with
    kind 0 works / 1 police / 2 debris (+10 keeps the player's lane open and ignores traffic), `DebugNightBlackout`,
    `DebugInvulnerable(seconds)` (call after the run started: ResetRun clears it). Tests: `InfernoNightHazardsTests`.
    Verified in scripted drives (captures /tmp/inferno-shots/claude-11x.png): rows spawn 175 m ahead in the closed lanes,
    traffic steers out, the blackout drops fogEnd 520 -> 190 and the key light to 0.45x, a hit shows ROADBLOCK!.
- 2026-09-26 "input 5000k crystals to unlock all maps" (tag `[user request 2026-09-26: crystals]`): the balance was set to
  5,000,000 in the running game through `SupabaseGameManager.SetCrystals` (pending cloud delta, ProfileChanged refreshes
  the lobby). Seven digits did not fit the 130 px gem counter (the last digit ran under the "+" key), so
  `InfernoLobby.GemText` formats big balances compactly: plain up to 99 999, then "500K", "5M" / "5.2M"; every counter
  write (Progress.cs x2, Home.cs, InfernoLobby.Gems, InfernoStyle.RefreshGems, the Amount label) goes through it.
  Prices and goal rewards stay plain numbers.

- 2026-09-27 "make sure it's just a line and has no glow and make the other elements square" (dock crop + case-card crop;
  tags `[user request 2026-09-27: dock underline is a plain line]` and `[user request 2026-09-27: square lobby buttons]`):
  - **Dock underline** `Inferno/InfernoUi.cs` `MakeTab3D` / `TabBarSprite`: the active tab's underline is a 4 px
    (was 6 px) magenta parallelogram with fully opaque rows (no vertical anti-aliasing, so no soft halo) and the
    `Glow` haze that faded up from it is no longer created (`SetTabActive` still tolerates its absence). It keeps the
    2026-09-26 placement: 7 px above the bottom rim, spanning the slanted edges at its own height minus 12 px insets.
  - **Square buttons** `Inferno/InfernoLobbyStyle.cs`: the outlined OPEN CASE key and the three GO / CLAIM / DONE keys
    in the daily-goals card are drawn with no corner cuts (were TL/BR 12, TR/BL 6, which read as rounded corners at
    60 px tall). Assumption: "the other elements" are the small elements inside the cards; the case, goals and
    map-selector boxes keep the large cut corners the user asked for on 2026-09-22, and the map arrows keep their cuts.
    Garage and settings are untouched as before. (Superseded the same day for the two cards and the right arrow by the
    "lobby corners" entry below: the right arrow's cuts were a mirror of the left one's and did NOT follow the bar's
    bottom-right cut, which is what made it look like it touched the bar.)
- 2026-09-27 "make sure those two buttons are on the same level and are different shape" (the garage action row's
  PURCHASE and STYLE; tag `[user request 2026-09-27: action row]`):
  - **Same level** was already true and is now structural: `InfernoStyle.Build` places the STYLE root at
    `GarageActionRowBottom + GarageActionRowHeight / 2` with height `GarageActionRowHeight` (the same two constants
    `MakeGaragePurchaseAction` uses for the PURCHASE plate) instead of the literals 580 / 100, so the pair cannot
    drift apart if the row moves. Measured live: both rects y -430..-330, centre -380, height 100; drawn violet
    centres 1340.0 and 1339.5 px in a 1080x1920 capture.
  - **Different shape**: STYLE is a capsule now, not a smaller copy of the plate. `InfernoUi.PillSprite(w, h)`
    generates a capsule at the exact rect size (semicircular ends of radius h / 2, the rounded frame's body
    gradient, lit top edge, violet rim and glow; cached per size, `Image.Type.Simple` like `TabSprite`), and
    `InfernoUi.MakePill(game, button)` swaps a built button's body to it and its drop shadow to
    `RoundedShadowSprite`, leaving the rect, label and press feedback alone. PURCHASE keeps the wide cut-corner
    plate. Layout audit clean on the garage, the STYLE sheet and the garage after close.
  - Noted while testing: the garage's purchase row is shown by `UpdateGarageVehicleCardVisuals`
    (`showPurchase = selected card is not the active vehicle && !IsVehicleUnlocked(id)`, Garage.cs) - there is no
    auth or profile term in it, so a hidden row means an owned or already-active car is selected, NOT a dead
    session; select a locked card to see it. Never force `Locked Vehicle Purchase Action` active for a capture: in
    that state it shows a button the player cannot reach. The dock tabs are fine: clicking one calls SelectMenuTab
    and restarts its page tween, so a poll-and-click-again loop never lets the page land - click once, then wait
    (and detect the page by "Garage Screen" being active, not by StyleButton, which an open STYLE sheet hides).

- 2026-09-27 "make sure the speed button and the beam button are on the same level" (in-run HUD screenshot;
  tag `[user request 2026-09-27: one HUD action row]`, originals in `// original:` comments):
  - **Measured first, then changed.** Before: the speed readout was at (34, 96) sized 300x160, so it spanned canvas
    x 34..334, y 96..256 (centre 176); the BEAM button was at (-150, 168) sized 232.8x116.4 at scale 1.45, spanning
    x 592.4..930, y 168..336.8 (centre 252.4). They were 72 px apart at the baseline and 8.8 px different in height.
  - **One row** `Inferno/InfernoDriving.cs`: new shared constants `BeamHudScale` (1.45, the 2026-09-20 "a bit bigger"
    value, now named so nothing drifts from it), `HudRowBottom` 168, `HudRowHeight` = 116.4 * BeamHudScale = 168.78
    and `HudRowInset` 56. `ShadowRunPrototype.UiCore.cs` `MakeSpeedometer` and `MakeHudHighBeamButton` both read them,
    so the two widgets cannot drift apart again; a future row move is one constant.
  - **Result** (re-measured in a scripted run): readout x 56..356 and BEAM x 686.44..1024, both y 168.00..336.78,
    height 168.78, centre y 252.39. Bottom, top, centre and height all identical, and the 56 px side margins mirror
    each other and match the column the Pause button and the hearts row already use.
  - **Why the BEAM button also moved sideways** (94 px right): its 150 px right margin was the gap left for the BRAKE
    button, which this build disables at birth (`MakeHudBrakeButton` calls `SetActive(false)` and nulls the rect), so
    the only bottom-right control sat well inside the HUD's right column. Its size and its 1.45 scale are unchanged.
  - **Chamfer match**: the readout's frame is now built at 1 / BeamHudScale and scaled back up, so its 9-slice corner
    cuts render the same size as the BEAM button's (both Medium, both x1.45) instead of 45 % smaller - a sliced
    sprite's border does not grow with the rect. The number and the caption are siblings, so they stay unscaled and
    crisp. The number / caption keep their proportions in the taller plate (0.60 / 0.19 of its height).
  - Left alone: the readout has no drop shadow while the button does. It is a readout, not a control, so it should
    not carry a button's elevation. Verified: compile clean, capture `Logs/hud-shots/hud-level2.png`, layout audit
    green on all 18 screens.

- 2026-09-27 "can you make design of this box borders more square and make sure this arrow box doesn't hit the box of the
  other div" (two crops: the N SERIES CASE card and the map selector's right arrow; tag `[user request 2026-09-27: lobby
  corners]`, previous values kept as `round 1:` comments). Designed by a four-analyst + judge agent panel, then verified
  by pixel scan of a 1080x1920 capture (`/tmp/inferno-shots/claude-131-lobby.png`). Three integer edits, no rect moves:
  - **Case card** `Inferno/InfernoLobbyStyle.cs` `CaseCard`: cuts TL 37 / TR 6 / BR 35 / BL 6 -> TL 18 / TR 0 / BR 18 /
    BL 0. The two big 45-degree slices are halved and the two small corners are true right angles, so the card reads as a
    rectangle with one 18 px facet (18 is the LockPlate's cut, an existing kit value). A 6 px cut on the 2 px rim read as
    a rounded corner next to the OPEN CASE key that was squared earlier today; that key's square bottom-right corner sat
    5.7 px from the card's 35 px diagonal (about 3.5 px of fill, a visible pinch) and now sits 17.7 px from it, level with
    the key's 18 px bottom margin. The "///" mark, whose rect poked 1.4 px into the old 37 px cut, now clears it by 12 px.
  - **Goals card** `GoalCard`: 46 / 8 / 46 / 8 -> 18 / 0 / 18 / 0, the same absolute values (not proportional): both
    cards share the top/bottom lines 1113/1596 and the same 2 px rim, so equal cuts read as one tool cutting both.
  - **Right arrow** `Inferno/InfernoLobby.cs` `Build` `NextMap`: cuts 0 / 22 / 0 / 12 -> 12 / 0 / 22 / 0 (a 180-degree
    rotation of PreviousMap's TL 22 / BR 12, not a left-right mirror). Cause of the "hit": the bar's cuts are TL/BR 40,
    TR/BL 14, so the mirrored arrow put its 22 px cut beside the bar's small 14 px corner and left its square bottom-right
    corner inside the bar's 40 px chamfer, 4.2 px from the bar's diagonal (2 px of fill between two 2 px rims, which merge
    at phone scale). Now the arrow's 22 px diagonal runs parallel to the bar's 40 px one, 19.8 px away (18 px of fill)
    by geometry, vs the left arrow's 19.1 px; the straight gap to the bar's right rim is 18 px. As rasterised in the
    capture (rim to rim, luminance > 210) the right side measures 19.0-19.1 px on every row of the diagonal (it was a
    wedge closing from 14 to 4.2 px) and the left arrow 17.0 px, unchanged: the bar's TL rim lands ~2 px inside its
    ideal edge and its bottom rim is 1 px thick vs 2 px on top, so the top/bottom gaps read 23 / 25 px on a centred
    arrow. The left arrow's TL chamfer is therefore the selector's tightest point, as it was before this change.
  - Assumption: "more square" means smaller cuts with hard right angles, not a plain rectangle (the user used "fully
    square" for the keys this morning and the comparative here); the fallback if it still reads as cut is 0/0/0/0 on both
    cards, two literals with no layout change. The selector bar (40/14), the left arrow, LockPlate and the gem counter are
    deliberately unchanged: the lobby now has two tiers, angular hero controls (PLAY plate, selector bar with its arrows'
    22 px cuts parallel to it, gem counter, rhomboid dock) over calm near-square information cards and their square keys.
    Arrow positions (+-425, 110x110) and every rect are unchanged, so button hit areas, Connect indices and the layout
    audit (which measures full rects and is blind to chamfer-to-chamfer proximity) are unaffected. Verified: compile
    clean, `Inferno Drive/Audit Layout` clean on the lobby, pixel scan of the capture confirms the 18 px legs, the square
    TR/BL corners on both cards and the parallel arrow diagonals. A second agent panel (four adversarial reviewers +
    judge, own flood-fill measurements) confirmed: OPEN CASE corner 4.9 -> 17.0 px, right arrow 4.24 -> 19.0 px minimum
    with a constant 19.1 px channel along the diagonal, 961 changed pixels in the selector all inside the arrow's box,
    chevron glyph identical, both card rects byte-identical, nothing else moved.
  - **Follow-through** `InfernoLobby.cs` `LockPlate` (the plate over a locked map's picture): 18 / 6 / 18 / 6 -> 18 / 0 /
    18 / 0 so the one remaining near-square panel does not keep the 6 px nicks the cards just lost (same 18 px facet,
    same rationale). Two integer literals in an existing call. Not visible in this save (all maps unlocked) and NOT yet
    compiled when written: the Editor was in Play mode for the user, so it compiles at the next AssetDatabase.Refresh.
  - Left as is, on purpose (reviewer notes for a later round, not part of this request): the gem counter's 10/14/14/10
    cuts and the selector bar's 40/14; a named chamfer scale (none / 12 / 18 / 40) shared by every SteelBox call would
    be the coherent refactor if the user points at those next.

- 2026-09-27 "make sure that the data about cars is bigger and more visible but don't change the outline of the ui" (crop
  of a garage card's info panel: AERO WRAITH X / Interceptor / Class X / ◆ 1500 / POWER HANDLING NITRO bars; tag
  `[user request 2026-09-27: card data]`, previous values kept as `original:` comments). The card itself (392x456, its
  border, background, badge and photo slot) is untouched; only the 338x150 info panel's rows grew into the 57 px that
  sat empty between the bars and the card edge. `ShadowRunPrototype.Garage.cs`, all numbers now `GarageCard*` /
  `GarageMiniBar*` constants read by `MakeGarageVehicleCard`, `MakeGarageCardMiniStat`, the locked-card blur strip and
  `UpdateGarageVehicleCardVisuals` (which resizes the fills and used to hard-code the 5 px height):
  - name: 36 px row, best fit 18..28 -> 42 px row, 22..34 (long names such as SOVEREIGN ARMOUR still shrink to one line);
  - subtitle: 24 px row, 12..17, colour (0.58, 0.5, 0.7, 0.85) -> 28 px row, 14..21, (0.78, 0.7, 0.92, 1);
  - price row: 22 px row at -64, 18 px bold, x 3 -> 32 px row at -74, 26 px, x 4 (the diamond's bearing grows with it);
  - stat labels: 16 px row at -92, 12 px, (0.7, 0.6, 0.84, 0.72) -> 20 px row at -112, 15 px, (0.86, 0.78, 0.97, 0.96);
  - bars: 106x5 at -110, track alpha 0.085, fill alpha 0.88 -> 106x8 at -134, track 0.16, fill 1.0.
  Rows end at -142 of the 150 px panel: 30 px above the card edge (was 57), 18 px top margin of the rarity badge for
  reference. The panel's top (172 above the card edge) does not move because the portrait car photos (1024x2048, fitted
  into 520.8x333.7) already reach down to 141. The locked card's blur strip is now derived from the same constants so it
  still stops exactly at the price row's top (98 above the edge; was 108) and the price stays bright in the rarity colour.
- 2026-09-27 "make sure the bar covers all the small dots and they don't stick out" (tag
  `[user request 2026-09-27: dots inside the bar]`, original in a `// original:` comment):
  - **Which widget.** The only bar-with-dots in the game is the kit's slider (`InfernoUi.MakeSlider3D`), used by the
    settings page for MASTER / MUSIC / EFFECTS (`InfernoSettings.MakeSlider`, `TrackWidth` 518). Its track is 518x16
    with five 5 px tick dots.
  - **The defect.** Each dot was anchored at the fraction `i / 4` of the track and centred there, so the first dot was
    centred on the bar's left end and the last on its right end, each hanging 2.5 px outside. Worse than the numbers
    suggest: `TrackSprite` is a capsule, so the overhanging half lay over the rounded cap where there is no bar at all.
  - **The fix.** Dots are now anchored to the track's left edge at `InfernoUi.PipX(i, width)` - a pure function that
    insets them `PipInset` (10 px) from each end, clearing both the dot's 2.5 px radius and the cap's curvature, so the
    bar is at full height behind every dot. The middle dot still lands on the track's centre and the spacing stays
    even. `PipX` clamps the inset on a track too narrow for it, so the dots stay on the bar at any width.
  - **Tests.** New `Assets/Editor/Tests/InfernoUiTests.cs` (4 tests, EditMode, reflection like the other suites):
    containment at six widths, clearance of the rounded caps, even spacing with a centred middle dot, and index
    clamping plus narrow tracks. Verified live as well: the settings page was opened in a scripted Play session and
    every dot's rect compared with its track's - 15 of 15 inside, capture `Logs/hud-shots/settings-sliders.png`.
  - Not changed: the knob still travels so that its centre reaches the track's ends, which leaves half of it past the
    bar at 0 % and 100 %. That is how a slider handle normally behaves and the user asked only about the dots.

- 2026-09-29 "when I press go in the box with cases the game should start instead of case opening, and add the case
  box for the user to open when the user loses" (tags `[user request 2026-09-29: GO starts the run]` and
  `[user request 2026-09-29: loss case box]`; designed by a four-analyst + two-designer + judge agent panel and verified
  live in the Editor). Two changes, both verified in Play mode:
  - **GO always starts the run** `Inferno/InfernoLobbyStyle.cs` `GoalGo`: the daily-goals row whose goal is
    `DailyGoalKind.CasesOpened` used to open the case screen from its GO key (`Click(game, 5)`); every GO key now taps
    the PLAY plate like the other two rows. Assumption: "the box with cases" is that goal row, because the three GO
    keys inside the daily-goals card are the only controls in the game labelled GO and that row's key was the only one
    that did not start a run (the case card's own key reads OPEN CASE and the whole card is the button). Today's goal
    roll happens to put the cases goal in the top row, next to the case card. Nothing else changes: GO never advanced
    that goal anyway (only `RegisterDailyCaseOpened` does), the row still completes and claims, and the case screen is
    still reached from the lobby card and now from the loss screen.
  - **A real case box on the loss screen** new file `Inferno/InfernoGameOverCase.cs`, called from `InfernoLobby.cs`
    `BuildGameOver`: the flat 720x110 "OPEN CASE" plate is replaced by the lobby "Cases" card rebuilt at 280x380
    (scale 380/483 = 0.7867, cuts TL/BR 14, TR/BL 0) with the live 990x630 crate render, "N SERIES CASE", the daily
    allowance line and the outlined OPEN CASE key. It sits at x -140..140, y -287..93, the band the plate leaves free
    between the stats card (bottom y 100) and the TRY AGAIN glow (top y -294), so no other element on the loss screen
    moves. The whole box is the Button (Connect index 5 -> ShowCaseOverlay), and it keeps the plate's GameObject name
    "OPEN CASE Profile Button", which is the by-name contract used by SmokeTests and the inferno-screens skill, so no
    test needed editing. Its own `UiScreenLink.onShown` binds the render texture once, calls `ResetCasePreviewPose`
    (a finished opening leaves the lid at 76 degrees and `ResetCaseOpening(false)` skips the pose reset) and switches
    the preview camera on; `onHidden` is deliberately null, because `CaseVisibility` disables this root in order to
    show the case sheet, which renders the same camera. The allowance line ticks from
    `InfernoDailyCases.RefreshCard`, called once a second by `UpdateDailyGoalsTimer`, and reads "N OF 3 LEFT TODAY"
    or "NEXT IN hh:mm:ss" once the day's three cases are used up. Tapping it at 0 remaining still opens the sheet,
    which is what the lobby card does; the sheet states the countdown itself.
  - Verified live: pressing GO on the cases goal started a run (HUD up, no case sheet); the loss screen then showed
    the box at exactly the specified rect with the crate bound and "3 OF 3 LEFT TODAY"; stats, TRY AGAIN and BACK TO
    MENU measured byte-identical to before; `Inferno Drive/Audit Layout` clean; tapping the box opened the case sheet
    (gem counter correctly reappearing) and closing it returned to the loss screen with the box intact. Capture
    `/tmp/inferno-shots/claude-140-gameover-case.png`. Note for future captures: on NEON NIGHT the `Pulsar Night
    Overlay Camera` (a URP overlay camera) draws over the UI in the canvas-to-camera capture trick, which is why that
    capture shows a dark moon disc over the subtitle. Players never see it, because a Screen-Space-Overlay canvas
    draws after every camera; it is an artefact of the capture method only.
  - Bugs found by the panel while reading the case flow, all pre-existing and NOT fixed here: closing the sheet during
    the lid animation still spends one of the three daily cases with no prize; `RegisterDailyCaseOpened` is skipped on
    the fallback path when the preview world fails to build, so the cases goal never progresses there;
    `EnsureCasePreviewWorld` leaks three GameObjects and warns every frame if the crate models are missing; and
    `caseHomeCardRequiresFreshPointerDown` is written but never read since the old home card was replaced.
