#!/usr/bin/env python3
"""Copy script GUIDs from a decompiled stub folder onto the real package's scripts.

usage: relink_stub_guids.py <StubFolder> <ImportedFolder>

For every <Class>.cs under StubFolder that has a .meta with a guid, finds the file with the same
basename under ImportedFolder and rewrites its .meta guid to the stub's, so prefabs / assets that
reference the stub (ripped from the shipped build) bind to the imported package's scripts.
Afterwards delete the stub folder."""
import os, re, sys

stub, real = sys.argv[1], sys.argv[2]
real_by_name = {}
for root, _, files in os.walk(real):
    for f in files:
        if f.endswith('.cs'):
            real_by_name.setdefault(f, []).append(os.path.join(root, f))
changed = 0
for root, _, files in os.walk(stub):
    for f in files:
        if not f.endswith('.cs.meta'):
            continue
        cs = f[:-5]
        m = re.search(r'^guid: ([0-9a-f]{32})', open(os.path.join(root, f)).read(), re.M)
        if not m or cs not in real_by_name:
            print('no match for', cs)
            continue
        if len(real_by_name[cs]) > 1:
            print('ambiguous', cs, real_by_name[cs])
            continue
        target = real_by_name[cs][0] + '.meta'
        text = open(target).read()
        new = re.sub(r'^guid: [0-9a-f]{32}', 'guid: ' + m.group(1), text, count=1, flags=re.M)
        if new != text:
            open(target, 'w').write(new)
            changed += 1
            print('relinked', cs, '->', m.group(1))
print('relinked', changed, 'scripts')
