# Inferno Drive — recovered Unity project

Recovered from the shipped iOS export (`car точно финал`) after the original
Unity project was lost. Opens in **Unity 6000.6.2f1** (Unity Hub → Add →
this folder). Compiles with 0 errors.

## What is real
* `Assets/Scenes/SampleScene.unity` and every prefab, mesh, texture, material,
  shader, font and audio clip — the game's full content, ripped with
  AssetRipper 2.0 from `Data/data.unity3d`.
* Script *declarations*: every class, field, nested type and method signature
  of the game (`Assets/Scripts/Assembly-CSharp`, e.g. `ShadowRunPrototype.cs`),
  reconstructed from the IL2CPP metadata with Cpp2IL. Scene/prefab references
  to scripts are intact.

## What is NOT real
* Method bodies are empty. IL2CPP compiled the C# away; the logic exists only
  as generated C++ in the export (`Il2CppOutputProject/Source/il2cppOutput/
  Assembly-CSharp*.cpp`), which is the reference for a rewrite.
* Third-party code is stubs too:
  * Asset Store packages — re-import from the Asset Store to restore them:
    Cartoon FX Remaster (`CFXRRuntime`), Dynamic Weather System
    (`DynamicWeatherSystem.Runtime`), Pulsar Neutron Star, MonoLimboStudio.
  * SDKs — install the real ones: Supabase C# (NuGet), RevenueCat Unity SDK
    (`revenuecat.purchases-unity`), Unity LevelPlay/IronSource
    (`Unity.LevelPlay`). `Assets/Scripts/Websocket.Client` is a compile shim.
* NuGet libraries the game embedded (System.Text.Json, Microsoft.*, …) were
  removed; they come back automatically with the real SDKs.

## Packages
`Packages/manifest.json` lists the packages the shipped game used, at the
versions recommended by Unity 6000.6 (Input System, URP, UGUI/TextMeshPro,
Burst, Collections, Mathematics, AI Navigation, Timeline, Visual Scripting,
Post-processing, Services Core, Newtonsoft JSON).
