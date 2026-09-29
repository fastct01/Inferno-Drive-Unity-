---
name: shared-checkout-commit
description: Commit your own work in this Inferno Drive checkout, which several Claude sessions edit at the same time — stage only your hunks from files that also carry a peer's uncommitted edits, use the project's commit identity and "Stage N / on request" message style, and verify before and after. Use this for any "commit", "save my changes", "push this", or "clean up git" request in ~/Downloads/InfernoDrive-Unity, and read it before touching git status in this repo at all — a plain `git add <file>` here sweeps a teammate's half-done work into your commit.
---

# Committing in the shared Inferno Drive checkout

Several Claude sessions (and their subagents) work in this one working tree at once, on different features, with **uncommitted** edits often sitting in the same files (`InfernoLobby.cs`, `ShadowRunPrototype.Cases.cs`, `SmokeTests.cs`, …). `git status` therefore shows 60–80 changed files that are not yours, and dozens of `Assets/Resources/*.mat` that nobody changed on purpose (runtime material darkening that the Editor saved — `tools/remake/restore_material_colours.py` undoes it). The rules below exist so a commit contains exactly one session's intent.

## Never

- `git add -A`, `git add .`, `git commit -a`, `git stash`, `git checkout -- <file>`, `git reset --hard`. Each one moves or destroys a peer's work.
- Rewriting a shared file wholesale (Write tool). Exact-string edits only, so your hunks stay separable from theirs.

## Marking your work

Tag every hunk you make with a comment marker that no one else uses, e.g. `/* [user request 2026-09-21 restyle] */` or `// [feature-x]`. The staging script keys on these. A method you *delete* has no marker — remember one distinctive string from its body for the filter.

## Staging only your hunks

```bash
python3 .claude/skills/shared-checkout-commit/scripts/stage-own-hunks.py \
  --mine "[user request 2026-09-21 restyle]" --mine "InfernoLobbyStyle" \
  --theirs "pause/UI fixes" --theirs "BuildPause" \
  Assets/Scripts/Assembly-CSharp/Inferno/InfernoLobby.cs Assets/Scripts/Assembly-CSharp/ShadowRunPrototype.Cases.cs
```

- It diffs each file with `-U1` (small context so neighbouring hunks by different authors do not merge), keeps hunks containing any `--mine` string, refuses hunks that match both lists ("MIXED" → split your edit so it does not touch their lines), and lists the hunks it left alone. Then it `git apply --cached` a patch of the kept hunks.
- Files only you touched, and new files (with their `.meta`), can be `git add`ed directly — check `git log -1 -- <file>` and the peer's notices first if unsure.
- Afterwards, verify:
  ```bash
  git diff --cached --stat                          # only your files/lines
  git diff -- <shared file> | grep '^@@'           # what remains unstaged must all be theirs
  git show :<shared file> | grep -c '<old symbol>'  # the staged file must be self-consistent (0 stale references)
  ```
  A half-staged replacement (new method staged, old one not deleted) commits a file that does not compile — that last grep catches it.

## Commit

```bash
git -c user.name="Inferno Remake" -c user.email="nme3015@gmail.com" commit -q -F - <<'MSG'
<Stage N | UI fixes | Feature> on request: one-line summary

- what changed, file by file, with the numbers that matter (positions, constants, before → after)
- what was verified and how (tests green, captures, probes)

Co-Authored-By: Claude Opus 5 (1M context) <noreply@anthropic.com>
MSG
```

Commit only when the user asks. Do not push unless asked.

## Coordination

Peer sessions announce themselves over cross-session messages: reply with the exact files/regions you are editing, and expect the same. Before an Editor recompile or Play-mode toggle, remember the MCP bridge is one channel per Editor — a peer capturing screenshots loses its state when you refresh. Re-read `git log` after a peer commits; their sweep may have included a file you were about to stage.
