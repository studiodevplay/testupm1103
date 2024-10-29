using System.Collections;
using System.Collections.Generic;
using SolarEngine;
using UnityEditor;
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
            finishHandle();
        }

    }
    [MenuItem("SolarEngineSDK/Historical Configuration ", false, 0)]
    static void finishHandle()
    {
        checkXmlHandle();
        checkPlugins();
        AssetDatabase.Refresh();
        ShowTips("Successfully",  "SolarEngineSDK successfully updated historical configuration");
    }
    
    private static void checkXmlHandle()
    {
        if (SolarEngineSettings.isCN)
          XmlModifier.cnxml(true);
        else if(SolarEngineSettings.isOversea)
            XmlModifier.Overseaxml(true);
    }

    private static void checkPlugins()
    {
        if (SolarEngineSettings.isDisAll)
            PluginsEdtior.disableAll();
        if (SolarEngineSettings.isDixiOS)
            PluginsEdtior.disableiOS();
        if (SolarEngineSettings.isDisAndroid)
            PluginsEdtior.disableAndroid();
        if (SolarEngineSettings.isDisMiniGame)
            PluginsEdtior.disableMiniGame();
        if(SolarEngineSettings.isDisOaid)
            PluginsEdtior.disableOaid();
    }

    /// <summary>
    /// 展示提示.
    /// </summary>
    /// <param name="title">标题.</param>
    /// <param name="content">具体内容.</param>
    public static void ShowTips(string title, string content)
    {
        // 展示提示信息.
        EditorUtility.DisplayDialog(title, content, "OK");
    }

    
}
