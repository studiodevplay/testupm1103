#!/usr/bin/env bash
set -e

# ==== Config ====
VERSION_FILE="/Users/gaomengqing/UniytProject/UPM/testupm_upm/Packages/com.solarengine.sdk/SolarEngineSDK/Util/SolarEngineConstant.cs"
PACKAGE_NAME="com.solarengine.sdk"
PACKAGE_DIR="Packages/$PACKAGE_NAME"
PACKAGE_JSON="$PACKAGE_DIR/package.json"
FIXED_VERSION="1.3.1.0"

# ==== Colors ====
GREEN="\033[32m"
YELLOW="\033[33m"
CYAN="\033[36m"
BOLD="\033[1m"
RESET="\033[0m"

echo -e "${CYAN}${BOLD}== 🔥 SolarEngine UPM Release ==${RESET}"

# ==== Step0: 读取 version ====
if [ -f "$VERSION_FILE" ]; then
    CURRENT_VERSION=$(grep -o 'sdk_version\s*=\s*"[0-9]\+\.[0-9]\+\.[0-9]\+\.[0-9]\+"' "$VERSION_FILE" \
        | head -1 \
        | sed -E 's/.*"([0-9]+\.[0-9]+\.[0-9]+\.[0-9]+)".*/\1/')
    echo -e "${GREEN}✅ Current version found:${RESET} $CURRENT_VERSION"
else
    echo -e "${YELLOW}⚠️ Version file not found: $VERSION_FILE${RESET}"
    exit 1
fi

# ==== Step1: 固定版本 ====
NEW_VERSION="$FIXED_VERSION"
echo -e "${CYAN}📢 Using fixed version:${RESET} $NEW_VERSION"

# ==== Step2: 更新 package.json（可选） ====
if [ -f "$PACKAGE_JSON" ]; then
    echo -e "${YELLOW}Updating package.json version...${RESET}"
    sed -i "" "s/\"version\": \".*\"/\"version\": \"$NEW_VERSION\"/" "$PACKAGE_JSON"
    echo -e "${GREEN}✅ package.json updated${RESET}"
else
    echo -e "${YELLOW}⚠️ package.json not found at $PACKAGE_JSON, skipping${RESET}"
fi

# ==== Step3: 更新 SolarEngineConstant.cs ====
echo -e "${YELLOW}Updating sdk_version in SolarEngineConstant.cs...${RESET}"
sed -i "" "s/sdk_version\s*=\s*\"[0-9]\+\.[0-9]\+\.[0-9]\+\.[0-9]\+\"/sdk_version = \"$NEW_VERSION\"/" "$VERSION_FILE"
echo -e "${GREEN}✅ SolarEngineConstant.cs updated${RESET}"

# ==== Step4: 更新 CHANGELOG ====
DATE=$(date +"%Y-%m-%d")
echo -e "${YELLOW}Generating CHANGELOG entry...${RESET}"
echo -e "## $NEW_VERSION - $DATE\n- Fixed version release\n\n$(cat CHANGELOG.md 2>/dev/null)" > CHANGELOG.md
echo -e "${GREEN}✅ CHANGELOG.md updated${RESET}"

# ==== Step5: Git 提交 ====
echo -e "${CYAN}🚀 Committing changes to Git...${RESET}"
git add "$PACKAGE_JSON" "$VERSION_FILE" CHANGELOG.md 2>/dev/null || git add "$VERSION_FILE" CHANGELOG.md
git commit -m "chore: release $NEW_VERSION"
git tag "$NEW_VERSION"
git push
git push --tags
echo -e "${GREEN}✅ Git commit, tag, and push completed${RESET}"

echo -e "${BOLD}${GREEN}🎉 Release script finished. Version fixed at: $NEW_VERSION${RESET}"