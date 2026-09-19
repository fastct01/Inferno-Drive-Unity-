#!/bin/bash
# tools/remake/build.sh ios|android|settings — batchmode player export (needs the iOS / Android build
# support modules installed in Unity Hub for 6000.6.2f1). Output: Builds/iOS (Xcode project), Builds/Android/InfernoDrive.apk.
PROJECT="$(cd "$(dirname "$0")/../.." && pwd)"
UNITY=/Applications/Unity/Hub/Editor/6000.6.2f1/Unity.app/Contents/MacOS/Unity
LOG=/tmp/inferno-build-$1.log
case "$1" in
  ios)      "$UNITY" -batchmode -nographics -projectPath "$PROJECT" -buildTarget iOS -executeMethod InfernoBuild.BuildIOS -logFile "$LOG" -quit ;;
  android)  "$UNITY" -batchmode -nographics -projectPath "$PROJECT" -buildTarget Android -executeMethod InfernoBuild.BuildAndroid -logFile "$LOG" -quit ;;
  settings) "$UNITY" -batchmode -nographics -projectPath "$PROJECT" -executeMethod InfernoBuild.ConfigurePlayerSettings -logFile "$LOG" -quit ;;
  *) echo "usage: $0 ios|android|settings"; exit 2 ;;
esac
RC=$?
echo "exit=$RC"; grep -n "\[InfernoBuild\]\|error CS\|Error building\|BuildFailedException\|not installed\|Unsupported\|is not supported" "$LOG" | grep -v PackageCache | head -20
exit $RC
