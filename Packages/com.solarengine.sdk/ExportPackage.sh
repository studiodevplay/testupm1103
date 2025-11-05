#!/bin/bash
set -x  # 开启调试模式
unity_path=$(cat unity_path.txt)

current_dir=$(pwd)
project_path="$current_dir/solar_engine_unity_plugin"
"$unity_path" -projectPath "$project_path" -executeMethod ExportPackage.exportPackage
return_code=$?

if [ $return_code -ne 0 ]; then
    echo "Unity命令执行出错，返回码为 $return_code"
fi
