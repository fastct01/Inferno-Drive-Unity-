# PROMPT — Remake *Inferno Drive* in Unity, exactly

You are a senior Unity/C# engineer with reverse‑engineering experience. Your job is to
**re‑implement the game "Inferno Drive" (internal name ShadowRun) in C#, inside the recovered
Unity project, so that it behaves exactly like the shipped build** — same screens, same
numbers, same rules, same feel — and then add the features that were bolted on after the
original source was lost. You are not designing a game; you are transcribing one whose
source is gone but whose compiled form is fully readable.

Work autonomously, stage by stage, compiling and play‑testing after every stage. Do not
stop to ask questions that the sources below can answer. When a detail is genuinely
unrecoverable, choose the behaviour that matches the shipped build's observable result,
and log the assumption in `docs/remake-assumptions.md`.

---

## 0. Where everything is

| What | Path |
|---|---|
| **Recovered Unity project** (opens in Unity 6000.6.2f1, compiles with 0 errors, does nothing at runtime) | `~/Downloads/InfernoDrive-Unity` |
| Script **stubs** with every class, field, constant, nested type and method signature — the skeleton you fill in | `InfernoDrive-Unity/Assets/Scripts/Assembly-CSharp/` — chiefly `ShadowRunPrototype.cs` (679 methods, 855 fields, 143 constants) and `ShadowRun/Services/*.cs` |
| **The logic** (only surviving form): IL2CPP‑generated C++ of the original C# | `car точно финал/Il2CppOutputProject/Source/il2cppOutput/Assembly-CSharp.cpp`, `Assembly-CSharp__1.cpp`, `Assembly-CSharp__2.cpp` |
| Features added *after* the source was lost, as hand‑written C++ that patches the generated code (port these too, in stage 6) | `car точно финал/Il2CppOutputProject/Source/il2cppOutput/InfernoLobby.inc`, `InfernoSettings.inc`, `InfernoDriving.inc`, `InfernoDrivingRules.h`, `InfernoHazards.inc`, `InfernoGarage.inc`, `InfernoStyle.inc` and `tools/apply_lobby_patches.py` (its rules say *which generated function* each feature hooks and how) |
| Design notes and specs written during that work (already partly in C#) | `car точно финал/docs/lobby-implementation.md`, `docs/main-menu-redesign.md`, `docs/settings-screen-upgrade.md` |
| All art, meshes, materials, shaders, audio, fonts, prefabs (real, ripped from the build) | `InfernoDrive-Unity/Assets/{Resources,Texture2D,Sprite,Mesh,Material,Shader,Font,GameObject}` |
| The shipped, working game for side‑by‑side comparison (runs on this Mac via Xcode, "My Mac (Designed for iPhone)") | `car точно финал/Unity-iPhone.xcworkspace` — see `.claude/skills/run-inferno-drive/SKILL.md` |
| String‑literal table (every UI string, resource name, PlayerPrefs key) | decode from `car точно финал/Data/Managed/Metadata/global-metadata.dat` — see §2.3 |

Unity editor: `/Applications/Unity/Hub/Editor/6000.6.2f1/Unity.app/Contents/MacOS/Unity`.
Compile check: `Unity -batchmode -nographics -projectPath <project> -logFile <log> -quit`
then `grep 'error CS' <log>`. Play‑mode tests: run EditMode/PlayMode tests with
`-runTests -testPlatform PlayMode`.

---

## 1. Non‑negotiables

1. **Exactness over taste.** Every constant, threshold, layout coordinate, colour, string,
   PlayerPrefs key, timing and probability must be the shipped value. The stubs already
   contain the 143 constants (e.g. `RoadWidth = 10.2f`, `SegmentLength = 18f`,
   `SegmentCount = 42`, `CityLoop = 528f`, `TrafficPoolSize = 12`, `PlayerMinX/MaxX = ±3.85f`,
   `HighBeamCooldownDuration = 10f`, `CaseVehiclePrizeChance = 0.06f`,
   `GarageVehicleCrystalPrice = 100`, `MenuTargetFrameRate = 30`, `TargetFrameRate = 60`,
   `WeatherPhaseDuration = 35f`, …). Literals inside method bodies come from the C++.
2. **Keep the names.** Same class names, same method names, same field names as the stubs.
   Scene/prefab references and the reverse‑engineering notes all key on them.
3. **Same architecture.** The game creates itself from code: a
   `[RuntimeInitializeOnLoadMethod]` `ShadowRunPrototype.AutoStart` (load type 0 =
   AfterSceneLoad) adds the component to a new GameObject; `Awake`/`MakeUI` build every
   menu, HUD element and world object procedurally with `MakeText`, `MakeUiBlock`,
   `MakeReferenceArtImage`, etc. The scene has only Main Camera, Directional Light and a
   Global Volume. Do not convert this into prefab‑based UI; reproduce the builder methods.
   Boot order from `Data/RuntimeInitializeOnLoads.json`: `SupabaseBootstrap.CreateInstance`,
   `SupabaseGameManager.CreateInstance`, `RevenueCatPurchaseService.CreateInstance`,
   `LevelPlayAdsService.Bootstrap` (BeforeSceneLoad = 1), then `ShadowRunPrototype.AutoStart`
   and `FirstLaunchAuthScreen.CreateInstance` (AfterSceneLoad = 0).
4. **Translate, don't paraphrase.** For each method: open its C++ definition, translate it
   statement by statement into idiomatic C#, keeping control flow and numbers identical.
   Then simplify only where the C++ is an obvious compiler artefact (see §2). Never invent a
   "cleaner" rule that changes behaviour.
5. **One method at a time, compile often.** Never leave the project uncompilable at the end of
   a work session.
6. **Third‑party code is not yours to rewrite.** Cartoon FX Remaster, Dynamic Weather System,
   Pulsar Neutron Star, MonoLimboStudio, FastSky, "FakeVolumetricLight" are Asset Store
   packages — their stubs must be replaced by the real packages (owner's Asset Store account).
   Supabase, RevenueCat, LevelPlay are SDKs — install the real ones. Write adapters only
   where the game's own `ShadowRun.Services` classes wrap them.

---

## 2. How to read the IL2CPP C++ (your source of truth)

2.1 **Finding a method.** Definitions look like
`IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR <ret> ShadowRunPrototype_<Name>_m<40‑hex> (ShadowRunPrototype_t… * __this, <args>, const RuntimeMethod* method)` followed by `{`. Declarations end with `;` — skip them. `grep -n '^IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR .* ShadowRunPrototype_UpdatePlayer_m' Assembly-CSharp*.cpp` finds it; the body ends at the next line that is exactly `}`.

2.2 **Reading a body.** `L_n` are SSA temporaries; `V_n` are the C# locals; `G_Bn_k` are phi
merges (a value that came from one of several branches — in C# it is just the variable).
`goto IL_xxxx` / labels are the C# `if/else/while/for/switch` structure; recover it from the
label graph (a backward `goto` = a loop; forward `goto` past a block = `if`). `___fieldName` is
`this.fieldName`. `X__ctor(&L, …)` is `new X(…)` for structs; `il2cpp_codegen_object_new(Type)` +
`__ctor` is `new` for classes. `SZArrayNew(T[], n)` is `new T[n]`; `GetAt/SetAt` index it.
`VirtualActionInvoker1<Color>::Invoke(23, img, c)` is a virtual call — slot 23 on Graphic is
`color` set, 25 is `raycastTarget`, 75 on Text is `text`; when in doubt look at how a known
method (e.g. `MakeText`) uses the same slot. `Mathf_*_inline`, `Vector3_op_*_inline` are the
obvious operators. `il2cpp_codegen_add/subtract/multiply` are plain arithmetic.
`String_Format` / `String_Concat` are `string.Format` / `+`. Async methods are compiled into
`<Name>d__N` state machines with a `MoveNext`; read the `MoveNext` and write a normal
`async Task` method.

2.3 **String literals.** They appear as `_stringLiteral<40‑hex>`. The hex is the **SHA‑1 of
the UTF‑16LE bytes** of the string. Build the lookup once:
```python
import struct, hashlib
d = open('Data/Managed/Metadata/global-metadata.dat','rb').read()
tabOff, tabBytes, count, dataOff = struct.unpack('<IIII', d[8:24])
offs = struct.unpack('<%dI' % count, d[tabOff:tabOff+4*count])
table = {}
for i in range(count-1):
    raw = d[dataOff+offs[i]:dataOff+offs[i+1]]
    s = raw.decode('utf-8')
    table[hashlib.sha1(s.encode('utf-16-le')).hexdigest().upper()] = s
```
(the table holds every UI string, resource path and PlayerPrefs key; many are Russian —
keep them byte‑identical, e.g. resource `"кнопка в гараж"`, `"сердце"`, `"алмаз 1"`.)

2.4 **Metadata‑only calls.** `Resources_Load_TisX(name)` = `Resources.Load<X>(name)`;
`GameObject_GetComponent_TisX` / `AddComponent_TisX` are the generic versions;
`Component_get_transform`, `Transform_Find`, `RectTransform_set_anchoredPosition` … map 1:1 to
the Unity API.

2.5 **Where a feature is hooked by the post‑source patches**, `tools/apply_lobby_patches.py`
tells you the exact generated function and line pattern; the `.inc` files hold the C++ of the
feature (written by hand, readable, with comments) — port them as normal C# members of
`ShadowRunPrototype` or small partial classes.

---

## 3. The game, screen by screen (what "exact" means)

### 3.1 Boot & auth
`SupabaseBootstrap` restores a persisted session (PlayerPrefs keys `shadowRunCrystals`,
`shadowRunPendingCrystalDelta`, `shadowRunInFlightCrystalDelta`, `shadowRunCloudDirty`,
`shadowRunBest`, `shadowRunSelectedVehicle`…), `SupabaseGameManager` holds
`PlayerProfile { Id, Crystals, Car1..Car4 }`, `IsAuthenticated`, `IsGuestAccount`,
`IsCloudReady`, `HasPendingLocalChanges`, `SetCrystals`, `SyncProfileAfterEconomyChangeAsync`,
`SignInAsGuestOnlineAsync`, `SignInWithEmailAsync`, `RegisterWithEmailAsync`,
`StartAppleSignInAsync`, `StartGoogleSignInAsync`, `SignOutAsync`, `DeleteAccountAsync`,
`QueueCaseReward`, `SyncGaragePurchaseAsync`, `IsVehicleUnlocked/SetVehicleUnlocked`.
`FirstLaunchAuthScreen` shows once (choices: Apple / Google / Email / Guest, legal links).
Logs are in Russian (`"[Supabase] Профиль загружен из облака."`) — keep them.

### 3.2 Lobby (home)
Ported from `InfernoLobby.inc` (this *is* the current design; the original `MakeHomeMenu`
in the C++ is the pre‑redesign version — implement the `.inc` one). Portrait, 960×1440
content root scaled to fit the safe area above a 184 px dock. Black backdrop. Elements, all
drawn with a procedural 9‑sliced **neon frame** (`FramePixel`: 128 px octagon, 26 px chamfer,
4 px violet contour `#C860FF`, 10 px glow, dark glass `#09050F`):
* **Map selector** at the top: `‹` `›` (108×92) and `MAP n / 6` + name. Six maps =
  `{DAYLIGHT CITY, NEON NIGHT, RAIN RUN, STORM CHASE, FALLEN SKY, PLANET HORIZON}` mapping 1:1
  onto weather presets `{Day, Night, Rain, Storm, FallenSky, Planets}`; descriptions
  `"Golden streets. Open throttle."`, `"Electric skyline. Endless asphalt."`,
  `"Rain-lit roads. Stay sharp."`, `"Lightning overhead. Keep your line."`,
  `"A sky full of falling stars."`, `"A different world above the road."`. Selection
  persists in PlayerPrefs `InfernoDrive.SelectedMap` (default 1). Prices
  `{0, 0, 150, 250, 400, 600}` crystals; locked maps show LOCKED + price over the art and the
  PLAY button reads `U N L O C K   <price>`; tapping buys (through `SetCrystals` +
  `SyncProfileAfterEconomyChangeAsync`) or shows `NEED n MORE CRYSTALS`; unlocked state in
  `InfernoDrive.MapUnlocked.<i>`.
* **Arena card** with the map artwork (NEON NIGHT shows `Data/Raw/InfernoLobby/neon_night.png`
  centre‑cropped to 820×376; others the hero art `"drive menu final"` via `MakeHome101Artwork`).
* **PLAY**: neon frame around an electric plate (`PlatePixel`), gloss sweep
  (`MakeHomeDriveShine`). `ShowMainMenu` labels it `"P  L  A  Y"`, `PauseRun` relabels
  `"C  O  N  T  I  N  U  E"`; click → `StartRun`.
* **N SERIES CASE** card (282×370) showing the live 3D case render (`EnsureCasePreviewWorld`,
  `casePreviewTexture` 990×630 into a RawImage) → `ShowCaseOverlay`.
* **DAILY GOALS** card (652×370): three goals from `PickDailyGoals(3)`, `CLAIM`/`DONE` chips,
  countdown `UpdateDailyGoalsTimer`, reward per kind via `GoalReward(kind)` (see
  `InfernoLobby.inc`), claimed mask in `shadowRun.daily.claimedMask`, date key
  `shadowRun.daily.date`.
* **Gem counter** top‑right (252×100): crystal 56 px at x −88, amount centred at x 40;
  tap → `ShowGemShop`. The crystal image everywhere is `Data/Raw/InfernoLobby/crystal.png`
  (game resources `diamons_final`, `алмаз 1`, `алмаз 3` resolve to it; the gem shop keeps its
  own pack art).
* **Dock**: GARAGE · DRIVE · PROFILE, each a glass‑bevel frame (`GlassPixel`: 14 px lavender
  rim lit from the top, 48 px slice border), selected tab full tint, others `#7F739C`.
  Pages share one stationary viewport; `SelectMenuTab(tab, instant)`; `menuTabTweenDuration
  0.13`.
No brand text, no "choose your map" hint.

### 3.3 Garage
`MakeGarageScreen`, `MakeGarageVehicleCard`, `UpdateGarageVehicleCardVisuals`,
`PurchaseSelectedGarageVehicleAsync` (price 100 crystals per vehicle; texts
`"Purchase for {0}"`, `"Need {0} more to purchase"`, `"{0} OF {1} CARS UNLOCKED"`,
`"Syncing..."`), 3D preview on layer 29 rendered to `"Garage Scene Preview Texture"` with
auto‑rotate after 0.5 s and drag (`GaragePreviewHorizontalOffset 0.25`). Vehicles:
`sportcar`, `flying_car`, `IFNAL_MODEL_P900`, `car3` (+ the 20 editions from
`InfernoGarage.inc`). Header "MY GARAGE" centred on the page.
Customisation (`InfernoStyle.inc`): STYLE button opens a centred sheet with TIRES / FLAGS /
TRAILS rows, 4 chips each, `‹ ›` pagers, crystal prices (e.g. tires 60/60, flags 50/50/80,
trails 50/70), OWNED / SELECTED states, close `X`.

### 3.4 Profile / settings
`MakeSettingsScreen` + `InfernoSettings.inc`: account panel (type, identity, sync, guest
Sign up / Log in, LOG IN at the page bottom), AUDIO card with MASTER (PlayerPrefs
`InfernoDrive.MasterVolume` 0–100, `AudioListener.volume = enabled ? master/100 : 0`),
MUSIC (`musicVolume`), EFFECTS (`sfxVolume`) sliders; GAME card with VIBRATION switch
(`vibrationOn`); graphics index 0..2 → `ApplySettings` render scale
`{0.72, 0.86, 1.0}`; plain black backdrop; content scaled to fit above the dock.

### 3.5 Case opening
`ShowCaseOverlay` → `StartCaseOpening`: closed case (3D model with lid animation,
`CasePreviewYawLimit 60`), roulette of `CasePrize` cards (`CaseRouletteCardWidth 250`,
height 310, spacing 6, slot 256, viewport 920×350, image height 241.8, rarity strip 80.6),
`CaseVehiclePrizeChance 0.06`, diamond prizes (`+5 DIAMONDS` etc.), result panel
(`CRYSTAL REWARD`), `SPIN AGAIN` / collect, rewarded ad path via `LevelPlayAdsService`
(`"AD LOADING  •  TRY AGAIN"`, `"WATCH AD  •  AGAIN"`), audio clips `рулетка звук`,
`звук бима`, `caseOpeningAudioClip`.

### 3.6 Gem shop
`MakeGemShopOverlay`: `"Diamond Shop Artwork"`, `MakeGemPackCard` per pack
(`"Gem Pack "`, frame, neon glow, sale flag), `MakeShopPurchaseButton`, RevenueCat products
via `RevenueCatPurchaseService` (`PurchaseOutcome`), `AddGemsFromShop`, close button.

### 3.7 Driving (the core loop) — `Update` → `UpdatePlayer`, `UpdateWorld`, `UpdateRamps`,
`UpdateSpawns`, `UpdateHighBeams`, `UpdateCamera`, `UpdateSkyClouds`, `UpdateHud`, `UpdateEvents`
* Endless road: `SegmentCount 42` × `SegmentLength 18`, `RoadWidth 10.2`, city loop 528 with 17
  building rows at 31.06 spacing, street lamps every 30 at x ±5.72, neon fences (15 per
  segment at x 6.08), road texture length 1900.
* Player: lanes within ±3.85, half width 0.92, boost (`touchBoostHeld`, Space/↑/W), drift,
  jump/ramps (`RampWidth 2.68`, `RampLength 4.75`, `RampHeight 1.12`, pool 2, debris), high
  beams (5 s window, 10 s cooldown, 20 m range, 0.12 s flash, layer 28, volumetric cone
  prefab), engine audio (i6 German pack, volume 0.85), speedometer (scale 0.64).
  **No brake / slow‑down** (removed post‑source: no HUD brake button, S/↓ do nothing,
  tutorial skips its brake stage).
* Speed: `speedT = clamp01(speed / 82)`; automatic **speed stages by distance**
  (`InfernoDrivingRules.h`: thresholds and multipliers there); collision impact + recovery
  (`Impact`, `ResetRecovery`, `AdvanceSpeed`) — copy the header's constants.
* Traffic: pool 12, `TrafficVehicleResource` W222 AMG model, lanes, stopped‑traffic pass
  lane, respawn behind 7.5–10.5 / horizon −320, wrecks with pieces, traffic lights
  (`TrafficLightCount 0` — disabled), driver styles 0..4 with **muted paint**: navy
  `(0.05,0.10,0.30)→(0.10,0.18,0.42)`, dark red `(0.34,0.03,0.05)→(0.52,0.06,0.07)`,
  dark green (see `TRAFFIC_PAINTS`), graphite default; `NextPaintStyle` cycles `{0,4,3}`.
  Contact: `ResolveTrafficContact` costs 1 half‑unit of life, 1.5 s invulnerability,
  explosions `CFXR Explosion 1` (rear) / `CFXR3 Fire Explosion B` (side), `_WOW_`/`_POW_`
  texts, haptics (`PlayCollisionHaptic`, `PlayImpactHaptic` if `vibrationOn`).
* Lives: `lifeHalfUnits` starts at 6 (3 hearts); HUD hearts 64.4 px at x −73·i, root
  top‑right (−56,−48), each with a half‑fill child and the grayscale "lost" material;
  the heart sprite is the procedural neon heart (`HeartPixel` in `InfernoLobby.inc`).
* Camera: `UpdateCamera` FOV `SmoothDamp` to `lerp(…)` by speedT (max 38 rate), Perlin
  shake `amp = 0.0025 + drift·0.0022 + impactShake·0.032`, sampled every 1/30 s
  (`nextShakeSampleTime`), `cameraLookTarget` smoothing; `AdvancedCameraController` adaptive
  quality **disabled** (its `Screen.SetResolution` flip‑flop shakes the screen).
* Hazards (`InfernoHazards.inc`): puddles that can spin the car, wrecking balls swinging
  in from the sides — copy their spawn distances, sizes, spin torque and damage exactly.
* In‑run crystal pickups + counter (`InfernoDriving.inc`: `s_runCollected`, gem icon +
  count top centre, reset in `Reset`, bumped in `Collect`); road crystals are camera‑facing
  textured quads.
* HUD: pause button (white bars on dark‑violet rim), high‑beam button (1.3×), speedometer,
  distance, hearts, tutorial hand/targets (`FirstRunTutorial*`, key
  `infernoDriveFirstRunTutorialV7`, stage timeout 25 s), `AD LOADING` states.
* Frame pacing: 60 in run, 30 in menus (`UpdateTargetFrameRateForState`), gameplay dt
  capped at 1/30.

### 3.8 Weather / sky (per map)
`BuildWeatherPresets` → `weatherPresetCycle[6]` = Day, Night, Rain, Storm, FallenSky,
Planets (`BuildDayPreset`, `BuildNightPreset`, `LoadWeatherPresetAsset` + fallbacks,
`BuildFallenSkyPreset`, `BuildPlanetsPreset`); transitions 4.25 s (night→day 4, fallen sky
7.5, planets 6.5); **no automatic cycling** (`AdvanceWeatherPreset` disabled — the selected
map's preset is applied on `StartRun`). FastSky material `FastSky/Materials/RealisticSky`,
360° sky cylinder (`sky_360_hd`, `clouds_panorama_8k`, rotation 0.18, emission 1.85, rain
desaturation 0.72, base yaw 177.74), Pulsar night skybox + StarCore prefabs on layer 27,
lightning every 3 s / 0.24 s flash, rain loop `WeatherAudio/low_rain_loop`, falling stars
`CFXR4 Falling Stars`.

### 3.9 Game over
`GameOver` → `InfernoLobby::BuildGameOver` design: black backdrop, `YOU LOST`,
`BETTER LUCK NEXT TIME`, stats card with DISTANCE and BEST RECORD (`FormatGameOverDistance`,
best in `shadowRunBest`), TRY AGAIN (→ `StartRun`), BACK TO MENU (→ `ShowMainMenu`).
`RegisterDailyRunCompleted` on death.

### 3.10 Daily goals
Kinds `Distance, Runs, Boosts, Drift, CarsPassed, CasesOpened`; progress prefs
`shadowRun.daily.{distanceMeters,runs,boosts,driftSeconds,carsPassed,casesOpened}`;
titles like `"Night Marathon"` (10 km), `"Traffic Phantom"` (25 cars), `"Case Opener"`
(1 case); reset by date key; rewards via `GoalReward`.

### 3.11 Economy rules
Crystals live in `SupabaseGameManager.Crystals` (cloud) mirrored in `menuGemCount`;
**every change** goes `menuGemCount = Crystals ± delta; SetCrystals(menuGemCount);
SyncProfileAfterEconomyChangeAsync(); gemAmountText.text = …`. Sources: daily goals, case
prizes, in‑run pickups, shop; sinks: garage vehicle (100), map unlocks, customisation.

---

## 4. Assets & resource names you must keep
Load by the exact original names (they are `Resources.Load` keys in the C++): `drive menu
final`, `case menu final`, `goals menu final`, `diamons_final`, `алмаз 1`, `алмаз 3`, `сундук`,
`рамка`, `кнопка в гараж`, `гараж кнопка`, `коллект`, `плашка для меню`, `99`, `101`, `249`,
`эпл`, `гугл`, `кейс кнопка открытия`, `play again button`, `back to menu button`, `кнопка из
паузы`, `кристал плашка`, `айди кнопка`, `сердце`, `pause screen`, `Lost Life Icon Grayscale`,
`Shaders/RampUnlit`, `рампа`, `sportcar`, `flying_car`, `IFNAL_MODEL_P900`, `car3`,
`Meshy_AI_Lone_Street_Lamp_0617180213_texture`,
`uploads_files_4881612_uploads_files_3750868_MB.W222.AMG`, the CFXR prefab paths in the
constants, `sky_360_hd`, `clouds_panorama_8k`, `Pulsar Neutron Star/…`,
`FakeVolumetricLight-Unity-main/VolumatricSpotlight/Prefabs/VolumetricSpotLight_Cone`,
`Car Engine Sound - i6 German Free/Assets/Audio/i6_german_free/…`, `gamebeat2`,
`WeatherAudio/low_rain_loop`. They are present in `Assets/Resources` of the recovered project
(check each; some Asset Store content returns only when the packages are re‑imported).
New art: `Data/Raw/InfernoLobby/{crystal.png, neon_night.png, case.png}` → move into
`Assets/Resources/InfernoLobby/`. App icon: the magenta‑glow helmet racer in
`Unity-iPhone/Images.xcassets/AppIcon.appiconset/Icon-Store-1024.png`.
Palette: black `#000000`, ink `#050309`, night `#080513`, slate `#120C1F`, grape `#420E7A`,
electric `#9433FF`, violet `#B840FF`, magenta `#BF66FF`, orchid `#B26BFF`, lavender `#948CD1`,
lilac `#AD8ADB`, blossom `#D199F5`, pearl `#E0D6F0`, frame text `#FAE6FF`.
Player settings: portrait only, bundle id `com.baalattackgroup.infernodrive`
(`com.fastcomet.infernodrive.demo` for personal‑team builds), iOS 15+, IL2CPP, URP.

---

## 5. Stages (each ends with: 0 compile errors, a play‑mode test, a commit)

1. **Skeleton runs.** `AutoStart` creates the object; `Awake` builds camera/light/road
   segments/city loop; `Update` loop with state flags (`running`, `paused`,
   `showingGameOver`); keyboard input (`KeyDown/KeyHeld`: Space=32, Enter=13, Esc=27,
   arrows 273–276, WASD 119/97/115/100, shift 303/304). Result: an empty road you can drive.
2. **Driving loop complete.** Player, traffic, ramps, wrecks, high beams, camera, HUD,
   lives, game over, speedometer, engine/weather audio, tutorial. Compare against the shipped
   build side by side.
3. **Lobby + navigation.** Lobby (3.2), pages, dock, gem counter, game‑over screen, pause.
4. **Meta systems.** Garage (purchase, preview), daily goals, cases (preview world,
   roulette, prizes), gem shop, profile/settings, weather presets per map.
5. **Services.** Real Supabase / RevenueCat / LevelPlay SDKs behind the existing
   `ShadowRun.Services` classes; guest mode must work offline.
6. **Post‑source features.** Maps unlock, hearts art, muted traffic, hazards, speed stages,
   in‑run crystals, garage editions, customisation, settings sliders — ported from the
   `.inc` files.
7. **Polish & parity pass.** Walk every screen of the shipped build next to the remake with
   a checklist; fix every difference; export iOS and Android.

---

## 6. Working rules
* Read the whole C++ method before writing a line; write the C# next to the stub's
  signature; keep `private`/`static` as declared.
* Where the C++ shows `PlayerPrefs`, `Resources.Load`, `Random`, `Time` usage — keep the same
  API (e.g. `Random.value`, `Mathf.PerlinNoise(t·22, 0.31)`).
* UI coordinates are in the 1080×1920 reference canvas (`ApplyAnchor(rect, anchorIndex)`
  with anchors 0..8 = TextAnchor order); do not "improve" them.
* Every string the player can see must be byte‑identical to the literal table.
* Log with the same `Debug.Log` messages the build prints (useful for parity diffing).
* Tests: for every pure rule (speed stages, impact recovery, goal rewards, map prices,
  paint style cycling, case prize roll) write an EditMode test with the numbers from the
  sources. `tools/check_driving_rules.cpp` in the export shows the expected values for the
  driving rules.
* Commit after each method group with messages naming the C++ functions translated.
* Keep `docs/remake-progress.md`: a table of all 679 `ShadowRunPrototype` methods with
  status (todo / translated / verified‑in‑play).

Deliverable: the recovered project, fully implemented, building for iOS from Unity, playing
identically to the shipped build plus the post‑source features — and the two docs above.
