#!/usr/bin/env bash
set -e

# === 配置 ===
A_LOCAL_PATH="/Users/gaomengqing/UniytProject/SE/se_unity_sdk/solar_engine_unity_plugin"
PACKAGE_NAME="com.solarengine.sdk"
PACKAGE_DIR="Packages/$PACKAGE_NAME"

echo "== 🚀 Copy selected folders from local A repo =="

# 清理目标目录（可选）
rm -rf "$PACKAGE_DIR"
mkdir -p "$PACKAGE_DIR"

# 定义安全拷贝函数
copy_if_exists() {
    local SRC="$1"
    local DST="$2"
    if [ -d "$SRC" ]; then
        mkdir -p "$(dirname "$DST")"
        cp -R "$SRC" "$DST"
        echo "✅ Copied: $SRC -> $DST"
    else
        echo "⚠️  Source folder does not exist, skipped: $SRC"
    fi
}

# 拷贝 Core
copy_if_exists "$A_LOCAL_PATH/Assets/SolarEngineSDK" "$PACKAGE_DIR/SolarEngineSDK"

# 拷贝 Runtime
copy_if_exists "$A_LOCAL_PATH/Assets/Plugins/SolarEngine" "$PACKAGE_DIR/Plugins/SolarEngine"

# 拷贝 Editor（可选）
# copy_if_exists "$A_LOCAL_PATH/Assets/SolarEngine/Editor" "$PACKAGE_DIR/Editor"

# 拷贝 Samples~（可选）
copy_if_exists "$A_LOCAL_PATH/Assets/SolarEngine/Samples" "$PACKAGE_DIR/Samples~"

echo "== ✅ Selected folders copied to $PACKAGE_DIR =="
