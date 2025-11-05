#!/usr/bin/env bash
set -e

PACKAGE_NAME="com.solarengine.sdk"
PACKAGE_DIR="Packages/$PACKAGE_NAME"

echo "== 🧱 Init UPM Repo Structure =="

# Create UPM folder if missing
mkdir -p "$PACKAGE_DIR"

# Create .gitignore if missing
if [ ! -f ".gitignore" ]; then
cat > .gitignore <<EOF
# Ignore Unity temp files
Temp/
Library/
Obj/
Logs/
MemoryCaptures/
UserSettings/
EOF
fi

# Create placeholder package.json if missing
if [ ! -f "$PACKAGE_DIR/package.json" ]; then
cat > "$PACKAGE_DIR/package.json" <<EOF
{
  "name": "$PACKAGE_NAME",
  "version": "1.0.0",
  "displayName": "SolarEngine Unity SDK",
  "description": "SolarEngine analytics SDK for Unity.",
  "unity": "2019.4",
  "author": { "name": "SolarEngine" }
}
EOF
fi

echo "✅ Init completed: $PACKAGE_DIR"
echo "Next step: run add_from_A.sh"
