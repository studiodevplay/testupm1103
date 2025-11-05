#!/usr/bin/env bash
set -e

A_REPO="git@gitlab.mobvista.com:joypacgames/se_unity_sdk.git"
A_BRANCH="main"
PACKAGE_NAME="com.solarengine.sdk"
PACKAGE_DIR="Packages/$PACKAGE_NAME"

echo "== 🔗 Import from A Repo (First Time) =="

git subtree add \
  --prefix="$PACKAGE_DIR" \
  "$A_REPO" "$A_BRANCH"

echo "✅ Import finished"
echo "Code imported to $PACKAGE_DIR"
