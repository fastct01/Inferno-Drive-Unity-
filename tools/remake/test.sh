#!/bin/bash
# Runs the EditMode (default) or PlayMode tests in batchmode and prints a summary.
#   tools/remake/test.sh [EditMode|PlayMode] [results.xml]
PLATFORM=${1:-EditMode}
OUT=${2:-/tmp/inferno-tests-$PLATFORM.xml}
LOG=/tmp/inferno-tests-$PLATFORM.log
PROJECT="$(cd "$(dirname "$0")/../.." && pwd)"
UNITY=/Applications/Unity/Hub/Editor/6000.6.2f1/Unity.app/Contents/MacOS/Unity
rm -f "$OUT"
EXTRA=""
[ "$PLATFORM" = "EditMode" ] && EXTRA="-nographics"
# GUI=1 runs the editor with its window (PlayMode captures then use a 1080x1920 Game view)
[ "${GUI:-0}" = "1" ] && BATCH="" || BATCH="-batchmode"
"$UNITY" $BATCH $EXTRA -projectPath "$PROJECT" -runTests -testPlatform "$PLATFORM" -testResults "$OUT" -logFile "$LOG" >/dev/null 2>&1
echo "exit=$?"
python3 - "$OUT" <<'PY'
import sys, os, xml.etree.ElementTree as ET
p = sys.argv[1]
if not os.path.exists(p):
    print('no results file (compile error? see the log)'); sys.exit(1)
r = ET.parse(p).getroot()
print({k: r.get(k) for k in ('total', 'passed', 'failed', 'skipped', 'result')})
for tc in r.iter('test-case'):
    if tc.get('result') != 'Passed':
        print('FAIL', tc.get('fullname'))
        f = tc.find('failure')
        if f is not None:
            for tag in ('message', 'stack-trace'):
                e = f.find(tag)
                if e is not None and e.text: print('   ', e.text.strip()[:800])
sys.exit(0 if r.get('result') == 'Passed' else 1)
PY
