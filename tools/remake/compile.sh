#!/bin/zsh
# Batch compile check for the recovered project. Prints error count and errors.
LOG=${1:-/tmp/inferno-unity-compile.log}
START=$(date +%s)
/Applications/Unity/Hub/Editor/6000.6.2f1/Unity.app/Contents/MacOS/Unity -batchmode -nographics -projectPath "$HOME/Downloads/InfernoDrive-Unity" -logFile "$LOG" -quit
RC=$?
END=$(date +%s)
echo "exit=$RC elapsed=$((END-START))s"
echo "errors: $(grep -c 'error CS' "$LOG")"
grep 'error CS' "$LOG" | sed 's/^.*Assets/Assets/' | sort -u | head -60
