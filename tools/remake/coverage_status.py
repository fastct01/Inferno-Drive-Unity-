#!/usr/bin/env python3
"""Rewrite the statuses in docs/remake-progress.md from a play-mode Code Coverage run:
   Unity -projectPath . -runTests -testPlatform PlayMode -enableCodeCoverage -coverageResultsPath <dir> \
         -coverageOptions "generateAdditionalMetrics;assemblyFilters:+Assembly-CSharp"
   tools/remake/coverage_status.py <dir>
A ShadowRunPrototype method is `verified-in-play` when OpenCover marks it visited."""
import xml.etree.ElementTree as ET, glob, re, sys, os
cov = sys.argv[1] if len(sys.argv) > 1 else '/tmp/inferno-coverage'
visited = set()
for f in glob.glob(os.path.join(cov, '*-opencov', 'PlayMode', '*.xml')):
    for cls in ET.parse(f).getroot().iter('Class'):
        if not (cls.findtext('FullName') or '').startswith('ShadowRunPrototype'):
            continue
        for m in cls.iter('Method'):
            mm = re.search(r' ([\w\.]+)::(\w+)\((.*)\)$', m.findtext('Name') or '')
            if not mm or mm.group(1) != 'ShadowRunPrototype' or mm.group(2).startswith(('<', '.')):
                continue
            params = mm.group(3)
            key = '%s/%d' % (mm.group(2), 0 if params.strip() == '' else params.count(',') + 1)
            if m.get('visited') == 'true':
                visited.add(key)
p = os.path.join(os.path.dirname(__file__), '..', '..', 'docs', 'remake-progress.md')
lines = open(p).read().split('\n'); hit = 0; n = 0
for i, l in enumerate(lines):
    m = re.match(r'\| `([^`]+)` \| (translated|verified-in-play) \|', l)
    if not m:
        continue
    n += 1
    status = 'verified-in-play' if m.group(1) in visited else 'translated'
    lines[i] = re.sub(r'\| (translated|verified-in-play) \|', '| %s |' % status, l, count=1)
    hit += status == 'verified-in-play'
s = '\n'.join(lines)
s = re.sub(r'Methods: (\d+) — todo (\d+), translated (\d+), verified-in-play (\d+)\.',
           lambda mm: 'Methods: %s — todo %s, translated %d, verified-in-play %d.' % (mm.group(1), mm.group(2), int(mm.group(1)) - hit, hit), s)
open(p, 'w').write(s)
print('rows', n, 'verified-in-play', hit)
