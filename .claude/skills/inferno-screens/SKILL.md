---
name: inferno-screens
description: Reach, drive and screenshot any screen of Inferno Drive in the running Editor — lobby (DRIVE page), garage, profile/settings, gem shop, case opening, a locked map, the in-run HUD, pause and game over — through the MCP bridge, without disturbing the player's save. Use this whenever a change to this game needs to be looked at ("show me", "does the shop look right", "capture the HUD", "check the locked map"), when verifying a UI edit in this project, or when a play-test must open a specific screen; it knows the button names, the PlayerPrefs keys that select map / daily-case state, which camera to render, and how to put everything back.
---

# Inferno Drive screens

The game builds itself at Play (`ShadowRunPrototype.AutoStart`); every screen is uGUI on one Screen-Space-Overlay canvas and is reached by tapping code-built buttons. General MCP mechanics (RunCommand shape, waiting for compiles, the background-Editor traps, the capture code) are in the user-level `unity-editor-mcp` skill — read its `references/snippets.md` for the capture template. This file is the game-specific map.

## Before you drive anything

- Is the player in a run or a menu? `Cases` button `activeInHierarchy` ⇒ lobby DRIVE page; `WATCH AD + OPEN CASE Profile Button` active ⇒ case screen; no menu buttons and `Time.frameCount` racing ⇒ a run. Wait for a menu before toggling Play, refreshing, or opening overlays — a recompile ends their run.
- Other Claude sessions may be driving the same Editor (they announce themselves). One bridge, one Editor: do not refresh/enter Play while a peer is capturing.
- Everything below saves and restores what it touches. The user's save (PlayerPrefs, Supabase guest profile) is live.

## Navigation map (GameObject names of `Button`s; find with `activeInHierarchy`)

| Screen | How to get there |
|---|---|
| Lobby tabs | `Garage`, `Home` (DRIVE page), `Profile` — the dock tabs |
| Map selector | `PreviousMap` / `NextMap`; selection persists in `PlayerPrefs "InfernoDrive.SelectedMap"` (0–5); unlocks in `"InfernoDrive.MapUnlocked.<i>"` (maps 0–1 free; prices 150/250/400/600) |
| Gem shop | `GemCounter` (the top-right counter) or its `Plus` segment; close with `Gem Shop Close Button` |
| Case screen | `Cases` (the lobby card) or `game.SendMessage("ShowCaseOverlay")`; close: `Close Case Button` / `SendMessage("HideCaseOverlay")` |
| Open a case | `SendMessage("StartCaseOpening")` skips the ad and rolls; ~15 s for the roulette; consumes one daily case (`"InfernoDrive.DailyCases.Opened"`, day key `"InfernoDrive.DailyCases.Day"` = `YYYYMMDD`, 3/day) |
| Start a run | the PLAY plate: `Drive` (its `Button`), or `primaryOverlayButton` is not reachable — use the button |
| Pause / resume | `Pause` HUD button; `Pause Continue`; game over shows `TRY AGAIN Profile Button`, `OPEN CASE Profile Button`, `BACK TO MENU Profile Button` |
| Daily goals | claim buttons are `CollectGoal` (indices 7–9), `GoalGo` rows |

Text objects worth reading back: `CaseSubtitle` (daily count line), `Amount` under `GemCounter`, `MapName`, `MapCounter`, `LockPrice`, `Timer`.

## Cameras and capture

- Menus: the game's `Camera` exists during runs; in menus `Main Camera` (disabled) is the one to render — `cam.Render()` works on a disabled camera. Never a temporary camera with an empty culling mask: the canvas is on layer 0.
- Always disable the camera's URP post-processing for the capture; the storm/rain map presets otherwise smear the UI with halos that do not exist in the Game view.
- Reference resolution 1080×1920; write to `/tmp/inferno-shots/claude-<nn>-<name>.png` (the smoke tests write their own numbered shots to the same folder).
- The lobby's per-second refresh (`UpdateDailyGoalsTimer`) updates the case count and countdown — after changing prefs, wait ≥1.2 s before capturing.

## Locked-map capture recipe

Step `NextMap` until `MapUnlocked(Selected())` is false (replicate the two prefs lookups in the command — `InfernoLobby` is `internal`), capture, then step back with `PreviousMap` until the original index is restored (stepping through the UI keeps `RefreshMap`/`ApplySelectedMap` consistent; writing the pref alone does not).

## Exhausted-daily-cases recipe

Save both prefs, set `Day = today's key` and `Opened = 3`, wait 1.5 s (card refresh), capture lobby, `ShowCaseOverlay`, wait 0.6 s, capture, `HideCaseOverlay`, restore both prefs (delete them if they were unset).

## After any lobby / screen edit: the layout audit

`Assets/Tests/PlayMode/LayoutAuditTests.cs` (`NoOverlapsOnAnyScreen`) opens every screen and fails on any pair of overlapping sibling rects. Run it inside the open Editor with the menu `Inferno Drive > Run Layout Audit Test` (`InfernoTestRunner.RunLayoutAudit`, or `EditorApplication.ExecuteMenuItem(...)` from a RunCommand); it only opens screens, so it is safe while the Supabase client is signed in. Grep `Logs/Editor.log` for `[InfernoTests]`.

**Do not run the full PlayMode smoke suite in the open Editor** (`Inferno Drive > Run PlayMode Tests`, or `SmokeTests` by filter) while the game is signed in — it claims goals, opens cases and buys things against the user's *real* cloud profile (it happened once on 2026-09-22). `InfernoTestRunner` now refuses and writes the reason to the results path; use `tools/remake/test.sh PlayMode` with the Editor closed instead, or sign out first.

## Known chronic console errors (not yours)

`Failed to load …/Resources/vehiclesmokemobile.shader` and `trafficlampunlit.shader` at boot (Resources loader tripping over text shaders; they still load), URP "Dynamic Batching has been removed", and Unity Cloud token-refresh errors. Anything else is new.

## Added 2026-09-24

- **STYLE sheet** is a bottom sheet (`StyleSheet`, 920x840 anchored 196 px above the page bottom). While it is open the
  garage's `Garage 3D Scene Preview` RawImage has `uvRect.y = -540/height` and the `Garage Background Image` (the
  platform picture) `offsetMin/Max.y = 540`, so the car sits in the top band; `Close` restores both. The car it styles is
  the *active* vehicle (`shadowRunSelectedVehicle`), while the 3D preview shows the *selected card's* car
  (`SelectGarageVehicleCard(i)` on a locked card only previews; on an owned card it changes the active vehicle).
- **Neon Night day/night**: `game.SendMessage("DebugAdvanceNeonCycle", 35.5f)` during a run skips to the next fade
  (Editor-only hook). Read the state back with `RenderSettings.skybox.GetFloat("_Blend")` (0 night, 1 day),
  `RenderSettings.fogColor`, and the directional `Red Rim Light`. The lobby always sits at blend 0.
- **Unfocused Editor**: the player loop does not advance; tours must call `EditorApplication.Step()` on every
  `EditorApplication.update` tick and set `EditorApplication.isPaused = false` at the end (see the Tour commands in the
  2026-09-24 session). The Game view may not be portrait (`Screen` was 1689x1460) - the 1080x1920 capture still lays the
  canvas out correctly, but `Screen.*`-based code (uvRect heights, safe areas) sees the real view.
- **No body kits**: `Transform`s named `Kit *` must not exist; `IsVariant(id)` marks the five editions.

## Added 2026-09-27

- **Reaching the garage.** Click the `Garage` dock button **once**, then wait. Every click calls `SelectMenuTab` and
  restarts the page tween, so a poll-then-click-again loop never lets the page land (this looks exactly like "the
  tab is broken"; it is not). Detect arrival by the `Garage Screen` object being `activeInHierarchy`, **not** by
  `StyleButton` — an open STYLE sheet hides the style button's root. `SendMessage("ShowGarageScreen")` is the
  no-animation alternative.
- **Making the purchase row appear.** `UpdateGarageVehicleCardVisuals` sets
  `showPurchase = selected card is not the active vehicle && !IsVehicleUnlocked(id)` (Garage.cs) — no auth or
  profile term. So `SelectGarageVehicleCard(i)` on a *locked* card that is not the active car shows
  `Locked Vehicle Purchase Action` by itself. A hidden row means an owned or already-active car is selected, not a
  broken session. Never `SetActive(true)` that root for a screenshot: it then shows a button the player cannot
  reach in that state.
- **The garage action row** (`GarageActionRowBottom` 530 + `GarageActionRowHeight` 100, both in Garage.cs) carries
  two buttons that must stay on one level and differ in shape: `Unlock Selected Vehicle Button` (wide cut-corner
  plate, x -280..280) and `StyleButton` (capsule, x 300..470, `InfernoUi.MakePill`). InfernoStyle derives the
  STYLE root's centre and height from those same two constants, so moving the row moves both.
