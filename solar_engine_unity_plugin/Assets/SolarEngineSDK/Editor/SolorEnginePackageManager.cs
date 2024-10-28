using System.Collections;
using System.Collections.Generic;
using SolarEngine;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class SolorEnginePackageManager : MonoBehaviour
{

    private static  readonly string _packageName = "solarengine-unity-sdk" + SolarEngine.Analytics.sdk_version;
    static SolorEnginePackageManager()
    {
        AssetDatabase.importPackageCompleted += OnImportFinishHandle;
    }
    static void OnImportFinishHandle(string packageName)
    {
        if (packageName ==_packageName)
        {
            
        }
        Debug.LogError(packageName);

    }
    [MenuItem("SolarEngineSDK/PackageManager", false, 0)]
    static void finishHandle()
    {
        xmlHandle();
    }
    
    private static void xmlHandle()
    {
        if (SolarEngineSettings.isCN)
          XmlModifier.cnxml(true);
        else if(SolarEngineSettings.isOversea)
            XmlModifier.Overseaxml(true);
    }

    
}
