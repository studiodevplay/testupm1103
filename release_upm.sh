#!/usr/bin/env bash
set -e

# ==== Config ====
A_REPO="git@gitlab.mobvista.com:joypacgames/se_unity_sdk.git"
A_BRANCH="main"
PACKAGE_NAME="com.solarengine.sdk"
PACKAGE_DIR="Packages/$PACKAGE_NAME"
PACKAGE_JSON="$PACKAGE_DIR/package.json"

# ==== Load utils ====
source ./version_utils.sh

echo "== 🔥 SolarEngine UPM Release =="

echo "Step1: Pull latest code from A repo..."
git subtree pull \
  --prefix="$PACKAGE_DIR" \
  "$A_REPO" "$A_BRANCH"

echo "Step2: Bump version..."
CURRENT_VERSION=$(get_current_version "$PACKAGE_JSON")
echo "Current version: $CURRENT_VERSION"

# Default bump = patch, support params: ./release.sh minor
BUMP_TYPE=${1:-patch}
NEW_VERSION=$(bump_version "$CURRENT_VERSION" "$BUMP_TYPE")
echo "New version: $NEW_VERSION"

echo "Updating package.json..."
sed -i "" "s/\"version\": \".*\"/\"version\": \"$NEW_VERSION\"/" "$PACKAGE_JSON"

echo "Step3: Generate CHANGELOG entry..."
DATE=$(date +"%Y-%m-%d")
echo -e "## $NEW_VERSION - $DATE\n- Update from A repo\n\n$(cat CHANGELOG.md 2>/dev/null)" > CHANGELOG.md

echo "Step4: Commit & Tag & Push..."
git add .
git commit -m "chore: release $NEW_VERSION"
git tag "$NEW_VERSION"
git push
git push --tags

echo "✅ Release $NEW_VERSION published!"
echo "Tag: $NEW_VERSION"
