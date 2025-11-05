#!/usr/bin/env bash
set -e

# === 配置 ===
A_LOCAL_PATH="/Users/gaomengqing/UnityProject/ARepo"
PACKAGE_NAME="com.solarengine.sdk"
PACKAGE_DIR="Packages/$PACKAGE_NAME"

echo "== 🚀 Copy selected folders from local A repo =="

# 清理目标目录（可选）
rm -rf "$PACKAGE_DIR"
mkdir -p "$PACKAGE_DIR"

# 拷贝 Core
cp -R "$A_LOCAL_PATH/Assets/SolarEngine/Core" "$PACKAGE_DIR/Core"

# 拷贝 Runtime
cp -R "$A_LOCAL_PATH/Assets/SolarEngine/Runtime" "$PACKAGE_DIR/Runtime"

# 拷贝 Editor
cp -R "$A_LOCAL_PATH/Assets/SolarEngine/Editor" "$PACKAGE_DIR/Editor"

# 可选 Samples~
if [ -d "$A_LOCAL_PATH/Assets/SolarEngine/Samples" ]; then
    cp -R "$A_LOCAL_PATH/Assets/SolarEngine/Samples" "$PACKAGE_DIR/Samples~"
fi

echo "✅ Selected folders copied to $PACKAGE_DIR"
