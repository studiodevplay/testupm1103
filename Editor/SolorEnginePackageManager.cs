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
public static class SolarEnginePackageEvents
{
    private const string PackageName = "com.solarengine.sdk";
    private const string MarkerKey = "SolarEngineSDK_FirstImportDone";

    static SolarEnginePackageEvents()
    {
        Debug.Log("[SolarEngine] SolarEnginePackageEvents 初始化");
        Events.registeredPackages += OnRegisteredPackages;
    }

    private static void OnRegisteredPackages(PackageRegistrationEventArgs args)
    {
        // ✅ 处理新增包（安装）
        foreach (var added in args.added)
        {
            if (added.name == PackageName)
            {
                Debug.Log($"[SolarEngine] 检测到包安装：{PackageName}");

                if (!EditorPrefs.GetBool(MarkerKey, false))
                {
                    Debug.Log("[SolarEngine] 执行首次导入扩展包逻辑...");
                    EditorApplication.delayCall += RunAutoImport;
                }
            }
        }

        // 🗑️ 处理删除包（卸载）
        foreach (var removed in args.removed)
        {
            if (removed.name == PackageName)
            {
                Debug.Log($"[SolarEngine] 检测到包卸载：{PackageName}，清除标记");
                EditorPrefs.DeleteKey(MarkerKey);
            }
        }
    }

    private static void RunAutoImport()
    {
        try
        {
            string pkg = $"Packages/{PackageName}/PackageResources/solarengine-unity-sdk-upm.unitypackage";

            if (File.Exists(pkg))
            {
                AssetDatabase.ImportPackage(pkg, false);
                Debug.Log("[SolarEngine] 扩展包自动导入完成 ✅");

                // 写入标记（只在成功后写）
                EditorPrefs.SetBool(MarkerKey, true);
            }
            else
            {
                Debug.LogWarning("[SolarEngine] 未找到扩展包：" + pkg);
                // 不写入标记，下次会继续尝试
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[SolarEngine] 导入扩展包异常：" + ex.Message);
            // 不写标记，下次会继续尝试
        }
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
