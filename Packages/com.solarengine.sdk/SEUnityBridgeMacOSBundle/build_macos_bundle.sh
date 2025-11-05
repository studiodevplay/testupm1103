#!/bin/bash
set -e

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
GRAY='\033[0;37m'
RESET='\033[0m'

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$SCRIPT_DIR"
BUILD_OUTPUT_DIR="$SCRIPT_DIR/build"
BUNDLE_OUTPUT_PATH="$SCRIPT_DIR/../solar_engine_unity_plugin/Assets/Plugins/SolarEngine/macOS"
UNIVERSAL_BUNDLE_NAME="SEUnityBridgeMacOSBundle.bundle"
SCHEME_NAME="SEUnityBridgeMacOSBundle"
CONFIGURATION="Release"

echo -e "${GRAY}🧹 清理旧构建目录...${RESET}"
rm -rf "$BUILD_OUTPUT_DIR"
mkdir -p "$BUILD_OUTPUT_DIR/arm64" "$BUILD_OUTPUT_DIR/x86_64" "$BUNDLE_OUTPUT_PATH"

echo -e "${YELLOW}🔨 构建 arm64...${RESET}"
xcodebuild -project "$PROJECT_PATH/$SCHEME_NAME.xcodeproj" \
  -scheme "$SCHEME_NAME" -configuration "$CONFIGURATION" \
  -sdk macosx ARCHS=arm64 ONLY_ACTIVE_ARCH=NO \
  BUILD_DIR="$BUILD_OUTPUT_DIR/arm64" clean build > /dev/null

echo -e "${YELLOW}🔨 构建 x86_64...${RESET}"
xcodebuild -project "$PROJECT_PATH/$SCHEME_NAME.xcodeproj" \
  -scheme "$SCHEME_NAME" -configuration "$CONFIGURATION" \
  -sdk macosx ARCHS=x86_64 ONLY_ACTIVE_ARCH=NO \
  BUILD_DIR="$BUILD_OUTPUT_DIR/x86_64" clean build > /dev/null

echo -e "${YELLOW}🔗 合并架构为 Universal Binary...${RESET}"

ARM_BINARY="$BUILD_OUTPUT_DIR/arm64/$CONFIGURATION/$UNIVERSAL_BUNDLE_NAME/Contents/MacOS/$SCHEME_NAME"
X86_BINARY="$BUILD_OUTPUT_DIR/x86_64/$CONFIGURATION/$UNIVERSAL_BUNDLE_NAME/Contents/MacOS/$SCHEME_NAME"
UNIVERSAL_DIR="$BUILD_OUTPUT_DIR/universal/$CONFIGURATION/$UNIVERSAL_BUNDLE_NAME/Contents/MacOS"
UNIVERSAL_BINARY="$UNIVERSAL_DIR/$SCHEME_NAME"

# 先创建 universal 目录
mkdir -p "$BUILD_OUTPUT_DIR/universal/$CONFIGURATION"

# 复制 arm64 bundle 资源结构作为壳
cp -R "$BUILD_OUTPUT_DIR/arm64/$CONFIGURATION/$UNIVERSAL_BUNDLE_NAME" "$BUILD_OUTPUT_DIR/universal/$CONFIGURATION/"

# 再创建 Contents/MacOS 目录（防止 cp 复制导致文件丢失）
mkdir -p "$UNIVERSAL_DIR"

# 合并架构二进制文件
lipo -create "$ARM_BINARY" "$X86_BINARY" -output "$UNIVERSAL_BINARY"

echo -e "${GREEN}✅ Universal Binary 已生成${RESET}"

# 删除旧插件目录
if [ -d "$BUNDLE_OUTPUT_PATH/$UNIVERSAL_BUNDLE_NAME" ]; then
  echo -e "${GRAY}🗑️ 删除旧插件目录: $BUNDLE_OUTPUT_PATH/$UNIVERSAL_BUNDLE_NAME${RESET}"
  rm -rf "$BUNDLE_OUTPUT_PATH/$UNIVERSAL_BUNDLE_NAME"
fi

# 拷贝最新 bundle 到 Unity 插件目录
cp -R "$BUILD_OUTPUT_DIR/universal/$CONFIGURATION/$UNIVERSAL_BUNDLE_NAME" "$BUNDLE_OUTPUT_PATH/"
echo -e "${GREEN}📦 拷贝完成: $BUNDLE_OUTPUT_PATH/$UNIVERSAL_BUNDLE_NAME${RESET}"

# 显示最终架构信息
ARCH_INFO=$(lipo -info "$BUNDLE_OUTPUT_PATH/$UNIVERSAL_BUNDLE_NAME/Contents/MacOS/$SCHEME_NAME" 2>/dev/null || echo "❌ 未找到目标文件")
echo -e "${YELLOW}🔍 架构信息: $ARCH_INFO${RESET}"

echo -e "${GREEN}🎉 构建完成，Universal Bundle 已准备好！${RESET}"