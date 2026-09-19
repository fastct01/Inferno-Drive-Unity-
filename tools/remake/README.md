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
