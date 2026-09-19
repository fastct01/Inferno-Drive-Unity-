# Remake tooling

Helpers used to translate the IL2CPP C++ (`car точно финал/Il2CppOutputProject/Source/il2cppOutput`) back to C#.
Paths to the shipped build are hard-coded at the top of each script.

- `build_strings.py` — resolves `_stringLiteral<sha1>` names to text (SHA-1 of UTF-16LE over `global-metadata.dat`) → `strings.json`.
- `index_methods.py` — indexes every function definition in the C++ (`methods.json`: file, start/end line).
- `m2.py <Name> [--raw] [--all]` — dense pseudo-C# view of one function (folds SSA temps, decodes literals, keeps `/* [InfernoLobby patch] */` markers).
- `asy.py <MoveNextName>` — same, with async state-machine boilerplate collapsed into `await`.
- `meta.py type <TypeName>` — reads `global-metadata.dat` (v39) and prints a type's methods with their vtable/interface slots; `meta.py vtable <Class>`.
- `stubs.py list|strip <files>|progress` — lists remaining empty stubs in `ShadowRunPrototype.cs`, removes stubs that a partial file now implements, regenerates `docs/remake-progress.md`.
- `find_rva.py` — locates `<PrivateImplementationDetails>` RVA arrays by hash.
- `compile.sh [log]` — batch-mode compile check (`Unity -batchmode -nographics -quit`), prints `error CS` lines.
- `test.sh [EditMode|PlayMode]` — batchmode test run with a pass/fail summary (`Assets/Editor/Tests` = EditMode rule tests ported from `tools/check_driving_rules.cpp` plus lobby / garage / style / roll rules).
- `relink_stub_guids.py <StubFolder> <ImportedFolder>` — after importing an Asset Store package (CFXR, Dynamic Weather System), moves the ripped script GUIDs onto the real scripts so prefab references survive; see `docs/third-party-imports.md`.
- `shader_recover.py <ripped shader .asset>` — prints properties, tags, per-pass render state, keyword names and the compiled **Metal source** (LZ4 `compressedBlob`) of a shader ripped from the shipped build (needs `pip install lz4 pyyaml`); the game's own shaders were re-authored from this output.
- `shipped-data/parse_urp_assets.py <Mobile_RPAsset.bin …>` — parses the raw `UniversalRenderPipelineAsset` / `UniversalRendererData` bytes of the shipped `Data/data.unity3d` (6000.4.5f1 field layouts taken from the IL2CPP structs) — the values behind `Assets/MonoBehaviour/*_RPAsset.asset`. Raw bytes are dumped with UnityPy (`obj.get_raw_data()`).
