using System.Collections;
using System.Collections.Generic;
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
#if UNITY_EDITOR


public static class PackageChecker
{
    private static string packagePath = "";
    public static bool IsUPMPackageInstalled(string packageName="com.solarengine.sdk")
    {
        var listRequest = Client.List(true, false);
        while (!listRequest.IsCompleted) {} // 等待完成

        if (listRequest.Status == StatusCode.Success)
        {
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
