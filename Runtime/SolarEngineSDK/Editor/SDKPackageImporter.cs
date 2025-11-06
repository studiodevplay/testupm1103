#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;



[InitializeOnLoad]
public static class SDKInstallChecker
{
    private static bool _checked;

    static SDKInstallChecker()
    {
        Debug.Log("SDKInstallChecker");
        // if (_checked) return;
        // _checked = true;

        // if (!EditorPrefs.GetBool("SDKExtraImported", false))
        // // {
        //     if (EditorUtility.DisplayDialog("SolarEngine 扩展模块",
        //             "检测到可选扩展模块，是否现在导入？",
        //             "导入", "稍后"))
        //     {
        //         SDKPackageImporter.ImportConfig();
        //         SDKPackageImporter.ImportExtraTools();
        //         EditorPrefs.SetBool("SDKExtraImported", true);
        //     }
        }
    //}
}
public static class SDKPackageImporter
{
    private const string PackageName = "com.solarengine.sdk";

    [MenuItem("SolarEngine/导入扩展/导入额外功能包")]
    public static void ImportExtraTools()
    {
        ImportPackage("ExternalDependencyManager.unitypackage");
    }

    [MenuItem("SolarEngine/导入扩展/导入配置模块")]
    public static void ImportConfig()
    {

        ImportPackage("SolarEngineNet.unitypackage");
    }

    private static void ImportPackage(string fileName)
    {
        string packagePath = $"Packages/{PackageName}/~PackagesContent/{fileName}";

        if (!File.Exists(packagePath))
        {
            Debug.LogError($"找不到 {fileName}，可能当前 SDK 不是通过 UPM 引入。");
            return;
        }

        AssetDatabase.ImportPackage(packagePath, true);
        Debug.Log($"已导入扩展包: {fileName}");
    }
}
#endif