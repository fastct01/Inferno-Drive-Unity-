# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

**Inferno Drive** (internal name *ShadowRun*): a portrait mobile arcade driving game being **re-implemented in C# inside this recovered Unity 6000.6.2f1 / URP project** so it behaves exactly like the shipped iOS build. The original source was lost; the assets were ripped from the build and the logic survives only as IL2CPP-generated C++. The remake is the job here — `REMAKE-PROMPT.md` is the brief and is binding.

**Status (read `docs/remake-progress.md`, not `README.md`, for this):** all 657 `ShadowRunPrototype` methods are translated; ~490 are verified-in-play by the play-mode smoke tests. The game boots and plays in the Editor. The README's "does nothing at runtime" line describes the pre-remake baseline and is stale. Stage 7 (exports, parity verification, coverage) is in progress; check `git log` and `git status` first — several Claude sessions have worked in this checkout and **uncommitted work-in-progress is normal** (see *Git* below).

## Sources of truth

| Need | Where |
|---|---|
| The brief: stages, non-negotiables, what "done" means | `REMAKE-PROMPT.md` |
| The logic to transcribe (IL2CPP C++ of the original C#) | `~/Downloads/car точно финал/Il2CppOutputProject/Source/il2cppOutput/Assembly-CSharp*.cpp` |
| Post-source features (hand-written C++ patches) to port | same dir: `Inferno{Lobby,Settings,Driving,Hazards,Garage,Style}.inc`, `InfernoDrivingRules.h`, `tools/apply_lobby_patches.py` |
| Design notes from that work | `~/Downloads/car точно финал/docs/*.md` |
| Per-method status + C++ function name | `docs/remake-progress.md` (regenerate, don't hand-edit) |
| Decisions where the source is unrecoverable / API forbidden | `docs/remake-assumptions.md` — **append here whenever you make one** |
| Screen-by-screen parity checklist vs. shipped build | `docs/remake-verified.txt`, captures in `docs/parity/*.png` |
| Third-party stubs vs. real Asset Store imports | `docs/third-party-imports.md` |
| The shipped, working game for side-by-side comparison | `~/Downloads/car точно финал/Unity-iPhone.xcworkspace` → destination **My Mac (Designed for iPad)** → ⌘R (recipe: its `.claude/skills/run-inferno-drive/SKILL.md`). `open` on the pre-built app does not work from this shell. |

## Commands (all from the repo root; Editor must be closed for batch-mode ones)

```bash
tools/remake/compile.sh                 # batch compile check, prints `error CS` lines (~1 min)
tools/remake/test.sh                    # EditMode rule tests (Assets/Editor/Tests)
tools/remake/test.sh PlayMode           # play-mode smoke tests (Assets/Tests/PlayMode/SmokeTests.cs), ~minutes
FILTER="SmokeTests.EconomyMenusAndTutorial" tools/remake/test.sh PlayMode   # one test (regex)
GUI=1 tools/remake/test.sh PlayMode     # with the Editor window: 1080x1920 Game view, screenshots to $INFERNO_SHOTS
tools/remake/build.sh settings|ios|android   # apply shipped PlayerSettings / export Builds/iOS (Xcode proj) / Builds/Android/*.apk
tools/remake/stubs.py progress          # regenerate docs/remake-progress.md
tools/remake/coverage_status.py <dir>   # mark verified-in-play from a -enableCodeCoverage PlayMode run (command in its header)
tools/remake/m2.py <MethodName>         # pseudo-C# view of one IL2CPP function (asy.py for async state machines)
tools/remake/meta.py type <TypeName>    # methods + vtable slots from global-metadata.dat
tools/remake/restore_material_colours.py [--dry-run]   # undo runtime material darkening that got saved to .mat files
```

Play in the Editor: menu **Inferno Drive > Play (portrait)** (`InfernoPlay.Play` — sets a 1080×1920 Game view, enters Play), or from a cold start `Unity -projectPath . -executeMethod InfernoPlay.Play`. The game creates itself; nothing needs placing in the scene.

PlayMode tests drive the real economy. `test.sh` moves the Supabase session file (`~/Library/Application Support/BaalAttackGroup/Inferno Drive/supabase-session.json*`) aside so a guest cloud profile is never mutated — do the same if you run the tests any other way. The `UISprite.psd` assert during boot is expected (editor-only builtin lookup) and is filtered; every other error/exception fails the test.

### Driving the open Editor (MCP)

With `com.unity.ai.assistant` in the manifest, the `mcp__unity-mcp__*` tools reach the running Editor: `Unity_GetConsoleLogs` after every script change, `Unity_RunCommand` (`internal class CommandScript : IRunCommand`) for `AssetDatabase.Refresh()`, `InfernoPlay.Play()`, `EditorApplication.isPlaying`, scene inspection and `Unity_Camera_Capture` (pass the Main Camera **GameObject** instance ID). Only one Editor may have this project open (`pgrep -fi "MacOS/Unity -projectpath"`); never delete `Temp/UnityLockfile`. While the Editor is unfocused the play loop crawls and keyboards are disabled — for scripted play tests set `Application.runInBackground = true`, `InputSystem.settings.backgroundBehavior = IgnoreFocus`, `editorInputBehaviorInPlayMode = AllDeviceInputAlwaysGoesToGameView`, inject with a virtual `Keyboard` + `StateEvent`, and assert on world state, not `InputAction.ReadValue()` from `EditorApplication.update`.

## Architecture

- **The game is code, not scene.** `Assets/Scenes/SampleScene.unity` holds only camera/light/volume. `[RuntimeInitializeOnLoadMethod(AfterSceneLoad)] ShadowRunPrototype.AutoStart` (in `ShadowRunPrototype.Boot.cs`) adds the single `ShadowRunPrototype` MonoBehaviour to a new GameObject; `Awake`/`MakeUI` build the world, uGUI and everything else at runtime, loading ripped assets from `Assets/Resources/` **by name**. Renaming or moving anything in `Resources/` breaks the shipped lookups.
- **`ShadowRunPrototype` is one `sealed partial class`** spread over `Assets/Scripts/Assembly-CSharp/ShadowRunPrototype.*.cs` by area (Boot, World, Vehicle, Player, TrafficSpawn/Ai/Contact, Ramps, HighBeams, Cases, Garage, Home, MenuTabs, Profile, Progress, Tutorial, UiCore, UiSprites, Materials, Sky/SkyEffects/WeatherPresets, Audio, Effects, Native). `ShadowRunPrototype.cs` holds the declarations/constants (the 143 shipped constants — never change a value) and any not-yet-moved stubs. New method bodies go in the area file; run `stubs.py strip` to remove the emptied stub.
- **`Inferno/*.cs`** = the features added after the source was lost (lobby, settings, driving rules, hazards, garage, style), ported from the `.inc` C++ patches. `InfernoDrivingRules.cs` is pure logic covered by `Assets/Editor/Tests/*RulesTests.cs` — keep it MonoBehaviour-free.
- **`ShadowRun/Services/`** = backend services (Supabase auth/profile, purchases, ads). Real SDKs are Asset Store / NuGet imports; some are still compile shims — see `docs/third-party-imports.md` before touching `CFXRRuntime`, `DynamicWeatherSystem.Runtime`, `RevenueCat`, `LevelPlay`, `PulsarNeutronStar`, `MonoLimboStudio`.
- **`ShadowRunPrototype.Native.cs`**: `[DllImport("__Internal")]` iOS entry points with `#else` no-op fallbacks so the Editor and Android can run. Anything `UnityEngine.iOS` must be guarded the same way (Android export depends on it).
- **Assemblies:** game code is `Assembly-CSharp` (no asmdef). Third-party runtime code and the PlayMode tests have their own asmdefs; `Assets/Editor/**` is `Assembly-CSharp-Editor`.
- **Editor tooling (`Assets/Editor/`):** `Build/InfernoBuild.cs` (shipped PlayerSettings: bundle `com.baalattackgroup.infernodrive`, IL2CPP, portrait, `shadowrun` URL scheme; iOS/Android export entry points), `Build/InfernoPlay.cs`, `Build/InfernoMaterialGuard.cs` (stops runtime-darkened *shared* materials being saved to disk), `AssetRipperPatches/` (keeps ripped YAML shaders importable and unsaved), `Tests/` (EditMode rule tests).
- **Rendering:** URP, per-quality RP assets in `Assets/MonoBehaviour/{Mobile,PC}_RPAsset.asset` + renderers, values parsed from the shipped build (`tools/remake/shipped-data/`). Game shaders were re-authored from the ripped Metal source (`shader_recover.py`). Post-processing via `com.unity.postprocessing` *and* URP volumes, as shipped.

## Non-negotiables (from the brief)

1. **Exactness over taste** — every constant, threshold, layout coordinate, colour, string, PlayerPrefs key, timing and probability is the shipped value; take literals from the C++ (`m2.py`), strings from `strings.json`.
2. **Keep the names** — same class/method/field names as the stubs; scene/prefab references and all the docs key on them.
3. **Same architecture** — self-building game, one prototype class, uGUI built in code. Don't "clean it up" into prefabs/ScriptableObjects.
4. Compile (`compile.sh`) and play-test (`test.sh PlayMode`) after every stage; record unrecoverable decisions in `docs/remake-assumptions.md`; refresh `docs/remake-progress.md` with `stubs.py progress` / `coverage_status.py`.
5. `Object.GetInstanceID()` is obsolete-as-error in 6000.6 — use `GetEntityId()` and note the swap in assumptions where the shipped code depended on the value.

## Gotchas

- **Material drift.** The game darkens shared materials at runtime; before `InfernoMaterialGuard` existed the Editor saved that into `Assets/Resources/*.mat` and `Assets/Material/Lit.mat`. If `git status` shows dozens of modified `.mat` files, that is the cause — `restore_material_colours.py --dry-run` shows what would be reset. Don't commit darkened materials.
- **Batch-mode Unity fails if the project is open in an Editor** (lock file). Check `pgrep` before `compile.sh` / `test.sh` / `build.sh`. A batch compile that sits 20+ min at `Csc … PlayModeTests.dll` is a wedged Roslyn server from another open Editor — kill `VBCSCompiler`/`csc` processes and it finishes.
- **PlayMode tests: never add an `AudioListener` or reload `SampleScene` mid-test.** This Mac has no audio output device and the Editor wedges silently (0 % CPU) in the case-opening audio; the "no audio listeners" warning is harmless. Test-made PlayerPrefs live in `~/Library/Preferences/unity.BaalAttackGroup.Inferno Drive.plist` (snapshotted/restored by the tests).
- The user does not want the Editor window restarted for every step — batch a set of changes, restart once, then `Inferno Drive > Play (portrait)`.
- **Exports need the iOS/Android build-support modules** for 6000.6.2f1 (`Unity Hub -- --headless install-modules --version 6000.6.2f1 -m ios android --cm`). `Builds/` is gitignored.
- **Coverage runs** require `com.unity.testtools.codecoverage` (installed) and `-enableCodeCoverage -coverageOptions "generateAdditionalMetrics;assemblyFilters:+Assembly-CSharp"`.
- The `.meta` GUID rules apply: move/rename/delete assets together with their `.meta`; `relink_stub_guids.py` exists precisely because prefab references point at stub-script GUIDs.

## Git

Several Claude sessions share this one checkout. **Never `git add -A`** — stage your own paths by name; a peer's sweep commit can otherwise carry your half-done files (and vice versa). Re-read `git log`/`git status` after a peer commits. Commit only when the user asks, as `git -c user.name="Inferno Remake" -c user.email="nme3015@gmail.com" commit …`; messages follow the existing `Stage 7: <what> — <evidence>` style and end with the Claude `Co-Authored-By` line.
