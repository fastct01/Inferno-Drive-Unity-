#!/usr/bin/env python3
"""Restore the shipped (baseline-commit) property values of materials that the game mutated at
runtime while playing inside the editor.

StyleVehicleForGarage / StyleGarageScene darken *shared* materials (lerp toward black, _EMISSION,
renderQueue 2000, _Surface 0) every time the garage preview is rebuilt.  In a player build the
assets are reloaded fresh on every launch; in the editor the mutation lands on the asset itself
and is written to disk, so repeated play sessions converged the car paint and glass to black.

For every material whose `Material:` document differs from the baseline in a colour / keyword /
queue / surface value, this rewrites the `Material:` document from the baseline commit, keeping the
current `m_Shader:` line (the URP shader relink) and any trailing sub-asset documents (URP
AssetVersion MonoBehaviours).

usage: restore_material_colours.py [--baseline 4b28a57] [--dry-run] [paths...]
"""
import argparse
import os
import re
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
MUTATED = re.compile(r"^(_BaseColor|_Color|_EmissionColor|_Surface|_AlphaClip): |^_EMISSION$")


def norm(line):
    return re.sub(r"^\s*(-\s*)?", "", line)


def git(*args):
    return subprocess.run(["git", *args], cwd=ROOT, capture_output=True, text=True, check=True).stdout


def split_docs(text):
    """Return (header, [doc, ...]) where each doc starts with a `--- !u!` line."""
    parts = re.split(r"(?m)^(?=--- !u!)", text)
    return parts[0], parts[1:]


def material_doc(docs):
    for i, d in enumerate(docs):
        if d.startswith("--- !u!21 "):
            return i
    return -1


def mutated(baseline_doc, current_doc):
    b = {norm(l) for l in baseline_doc.splitlines() if MUTATED.match(norm(l))}
    c = {norm(l) for l in current_doc.splitlines() if MUTATED.match(norm(l))}
    return b != c


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--baseline", default="4b28a57")
    ap.add_argument("--dry-run", action="store_true")
    ap.add_argument("paths", nargs="*")
    args = ap.parse_args()
    paths = args.paths or [p for p in git("ls-files", "-z", "--", "Assets/**/*.mat", "Assets/*.mat").split("\0") if p]
    changed = []
    for path in paths:
        try:
            baseline = git("show", f"{args.baseline}:{path}")
        except subprocess.CalledProcessError:
            continue  # material added after the baseline
        with open(os.path.join(ROOT, path), encoding="utf-8") as f:
            current = f.read()
        b_header, b_docs = split_docs(baseline)
        c_header, c_docs = split_docs(current)
        bi, ci = material_doc(b_docs), material_doc(c_docs)
        if bi < 0 or ci < 0:
            continue
        if not mutated(b_docs[bi], c_docs[ci]):
            continue
        shader = re.search(r"(?m)^  m_Shader: .*$", c_docs[ci]).group(0)
        restored = re.sub(r"(?m)^  m_Shader: .*$", lambda m: shader, b_docs[bi], count=1)
        c_docs[ci] = restored
        out = c_header + "".join(c_docs)
        changed.append(path)
        if not args.dry_run:
            with open(os.path.join(ROOT, path), "w", encoding="utf-8") as f:
                f.write(out)
    for p in changed:
        print(("would restore " if args.dry_run else "restored ") + p)
    print(f"{len(changed)} material(s)")


if __name__ == "__main__":
    main()
