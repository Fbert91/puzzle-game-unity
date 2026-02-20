#!/bin/bash

# APK Build Script for PuzzleGameUnity
# Builds release APK using Docker Unity image

set -e

PROJECT_PATH="/root/project"
OUTPUT_PATH="/root/output"
APK_PATH="$OUTPUT_PATH/PuzzleGame-release.apk"

echo "=== PuzzleGameUnity APK Build ==="
echo "Project: $PROJECT_PATH"
echo "Output: $APK_PATH"
echo ""

# Build the APK
echo "Starting Unity build process..."
/opt/Unity/Editor/Unity \
  -projectPath "$PROJECT_PATH" \
  -buildAndroidPlayer "$APK_PATH" \
  -quit \
  -batchmode \
  -logFile - 2>&1 | tee build.log

# Check if build was successful
if [ -f "$APK_PATH" ]; then
  SIZE=$(du -h "$APK_PATH" | cut -f1)
  echo ""
  echo "✅ APK BUILD SUCCESSFUL"
  echo "APK Location: $APK_PATH"
  echo "APK Size: $SIZE"
  exit 0
else
  echo ""
  echo "❌ APK BUILD FAILED"
  echo "Check build.log for details"
  exit 1
fi
