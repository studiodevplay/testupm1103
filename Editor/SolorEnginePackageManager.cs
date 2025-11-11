using System.Collections;
using System.Collections.Generic;
using System.IO;
using SolarEngine;
using SolarEngineSDK.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
[InitializeOnLoad]
public class SolorEnginePackageManager : MonoBehaviour
{

    private static readonly string _packageName = "solarengine-unity-sdk";
    static SolorEnginePackageManager()
    {
        AssetDatabase.importPackageCompleted += OnImportFinishHandle;
    }
    static void OnImportFinishHandle(string packageName)
    {
        if (packageName ==_packageName)
        {
            finishHandle(false);
        }

    }
    
   //[MenuItem("SolarEngineSDK/SDK Edit Settings/Apply Settings Panel", false, 0)]

    static void finishHandle()
    {
     
        
     
    }

  
    
    static void finishHandle(bool isShow=false)
    {

        ApplySetting._applySetting(isShow);
    }
    

    
    
    [MenuItem("SolarEngineSDK/Documentation/UnityDocumentation", false, 0)]
    static void unityDocumentation()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/51FROeEQ");
    }
    
    [MenuItem(ConstString.MenuItem.iOSChangelog, false, 0)]
    static void solarEngineDocsiOS()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/geng-xin-ri-zhi-RAvv");
    }
    [MenuItem(ConstString.MenuItem.androidChangelog, false, 0)]
    static void solarEngineDocsAndroid()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/geng-xin-ri-zhi");
    }
    
    private const string storageWarning = "You can only choose either China or Overseas！";
    private const string nostorageWarning = "You must choose either China or Overseas!";
  

  


    
}

[InitializeOnLoad]
public static class SDKInstallChecker
{
    private const string PackageName = "com.solarengine.sdk";

    private static bool _checked;

    static SDKInstallChecker()
    {
        if (_checked) return;
        _checked = true;

        if (!EditorPrefs.GetBool("SDKExtraImported", false))
        {
            if (EditorUtility.DisplayDialog("SolarEngine 扩展模块",
                    "检测到可选扩展模块，是否现在导入？",
                    "导入", "稍后"))
            {
                ImportConfig();
              //  EditorPrefs.SetBool("SDKExtraImported", true);
            }
        }
    }
    [MenuItem("SolarEngine/导入扩展/导入配置模块")]
    public static void ImportConfig()
    {
        ImportPackage("solarengine-unity-sdk-upm.unitypackage");
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
   

#if UNITY_EDITOR


public static class PackageChecker
{
    private static string packagePath = "";
    public static bool IsUPMPackageInstalled(string packageName="com.solarengine.sdk")
    {
        Debug.Log("[SolarEngine] IsUPMPackageInstalled");
        var listRequest = Client.List(true, false);
        Debug.Log("[SolarEngine] ListRequest Status:" + listRequest.Status);
        while (!listRequest.IsCompleted) {} // 等待完成

        Debug.Log("[SolarEngine] ListRequest Result:" + listRequest.Result);
        if (listRequest.Status == StatusCode.Success)
        {
            Debug.Log("[SolarEngine] ListRequest Result:" + listRequest.Result);
            foreach (var pkg in listRequest.Result)
            {
                
                if (pkg.name == packageName)
                    packagePath = pkg.resolvedPath;
                return true;
            }
        }
        return false;
    }

    public static string GetPackagePath()
    {
        return packagePath;
    }
    
}
#endif
