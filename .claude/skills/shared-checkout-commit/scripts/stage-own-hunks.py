#!/usr/bin/env python3
"""Stage only the hunks that belong to you in files shared with other sessions.

  stage-own-hunks.py --mine MARKER [--mine ...] [--theirs MARKER ...] [--dry-run] FILE [FILE ...]

A hunk is staged when any line it adds or removes contains a --mine marker; refused
(and the run aborted) when it also contains a --theirs marker; left unstaged otherwise.
Uses -U1 diffs so adjacent hunks by different authors are not merged into one.
"""
import argparse, re, subprocess, sys, tempfile

ap = argparse.ArgumentParser()
ap.add_argument("--mine", action="append", required=True, help="substring identifying your hunks (repeatable)")
ap.add_argument("--theirs", action="append", default=[], help="substring identifying a peer's hunks (repeatable)")
ap.add_argument("--dry-run", action="store_true")
ap.add_argument("files", nargs="+")
a = ap.parse_args()

patch = []
for f in a.files:
    diff = subprocess.check_output(["git", "diff", "-U1", "--", f], text=True)
    if not diff.strip():
        print(f"{f}: no unstaged changes"); continue
    header, *hunks = re.split(r"(?m)^(?=@@ )", diff)
    kept = []
    for h in hunks:
        body = "\n".join(l for l in h.splitlines() if l[:1] in "+-" and not l.startswith(("+++", "---")))
        mine = any(m in body for m in a.mine); theirs = any(t in body for t in a.theirs)
        head = h.splitlines()[0]
        if mine and theirs:
            print(f"MIXED hunk in {f}: {head}\n  contains both a --mine and a --theirs marker; split the edit so it does not touch their lines."); sys.exit(1)
        if mine: kept.append(h)
        else: print(f"  left unstaged ({'theirs' if theirs else 'unmarked'}): {f} {head[:40]}")
    print(f"{f}: {len(hunks)} hunks, staging {len(kept)}")
    if kept: patch.append(header + "".join(kept))

if not patch:
    print("nothing to stage"); sys.exit(0)
with tempfile.NamedTemporaryFile("w", suffix=".patch", delete=False) as tmp:
    tmp.write("".join(patch)); path = tmp.name
if a.dry_run:
    print(f"dry run: patch written to {path}"); sys.exit(0)
subprocess.check_call(["git", "apply", "--cached", "--check", path])
subprocess.check_call(["git", "apply", "--cached", path])
print("staged; review with: git diff --cached --stat")
