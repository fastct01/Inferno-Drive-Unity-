#!/bin/bash
# Runs the EditMode (default) or PlayMode tests in batchmode and prints a summary.
#   tools/remake/test.sh [EditMode|PlayMode] [results.xml]
#   FILTER="SmokeTests.EconomyMenusAndTutorial" — only the tests matching the regex (-testFilter)
PLATFORM=${1:-EditMode}
OUT=${2:-/tmp/inferno-tests-$PLATFORM.xml}
LOG=/tmp/inferno-tests-$PLATFORM.log
PROJECT="$(cd "$(dirname "$0")/../.." && pwd)"
UNITY=/Applications/Unity/Hub/Editor/6000.6.2f1/Unity.app/Contents/MacOS/Unity
rm -f "$OUT"
EXTRA=()
[ "$PLATFORM" = "EditMode" ] && EXTRA+=(-nographics)
[ -n "${FILTER:-}" ] && EXTRA+=(-testFilter "$FILTER")
# GUI=1 runs the editor with its window (PlayMode captures then use a 1080x1920 Game view)
[ "${GUI:-0}" = "1" ] && BATCH="" || BATCH="-batchmode"
# The play-mode tests drive the economy (balance 500, map unlock, goal claims, a purchase attempt)
# and must run as a local, unauthenticated player: a persisted Supabase session (a guest account
# created from the welcome screen) would sync every change to that real cloud profile. The session
# file is moved aside for the run and put back afterwards.
SESSION_DIR="$HOME/Library/Application Support/BaalAttackGroup/Inferno Drive"
SESSION_ASIDE=""
if [ "$PLATFORM" = "PlayMode" ] && ls "$SESSION_DIR"/supabase-session.json* >/dev/null 2>&1; then
  SESSION_ASIDE=$(mktemp -d /tmp/inferno-session.XXXXXX)
  mv "$SESSION_DIR"/supabase-session.json* "$SESSION_ASIDE"/
  echo "supabase session moved aside for the test run ($SESSION_ASIDE)"
fi
restore_session() {
  if [ -n "$SESSION_ASIDE" ] && [ -d "$SESSION_ASIDE" ]; then
    rm -f "$SESSION_DIR"/supabase-session.json*   # anything the test run itself created
    mv "$SESSION_ASIDE"/supabase-session.json* "$SESSION_DIR"/ 2>/dev/null
    rmdir "$SESSION_ASIDE" 2>/dev/null
    SESSION_ASIDE=""
  fi
}
trap restore_session EXIT
"$UNITY" $BATCH "${EXTRA[@]}" -projectPath "$PROJECT" -runTests -testPlatform "$PLATFORM" -testResults "$OUT" -logFile "$LOG" >/dev/null 2>&1
echo "exit=$?"
restore_session
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
