#!/bin/bash

# 获取当前脚本所在目录，即 macos_project 路径
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# 相对路径配置
PROJECT_PATH="$SCRIPT_DIR"
BUILD_OUTPUT_DIR="$SCRIPT_DIR/build"
BUNDLE_OUTPUT_PATH="$SCRIPT_DIR/../solar_engine_unity_plugin/Assets/Plugins/SolarEngine/macOS"

# 项目配置
SCHEME_NAME="SEUnityBridgeMacOSBundle"        # ❗替换为你的 scheme 名称
CONFIGURATION="Release"
SDK="macosx"

# 清理旧构建
rm -rf "$BUILD_OUTPUT_DIR"
mkdir -p "$BUILD_OUTPUT_DIR"
mkdir -p "$BUNDLE_OUTPUT_PATH"

echo "🛠️ 开始使用 Xcode 构建 .bundle..."

# 执行构建
xcodebuild -project "$PROJECT_PATH/SEUnityBridgeMacOSBundle.xcodeproj" \
  -scheme "$SCHEME_NAME" \
  -configuration "$CONFIGURATION" \
  -sdk "$SDK" \
  BUILD_DIR="$BUILD_OUTPUT_DIR" \
  clean build

# 查找 .bundle 并复制到 Unity 插件路径
find "$BUILD_OUTPUT_DIR" -name "*.bundle" -exec cp -R {} "$BUNDLE_OUTPUT_PATH" \;

echo "✅ 构建完成！Bundle 已输出至: $BUNDLE_OUTPUT_PATH"
