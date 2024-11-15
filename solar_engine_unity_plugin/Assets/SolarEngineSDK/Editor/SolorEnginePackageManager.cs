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
    [MenuItem("SolarEngineSDK/Matching Settings Panel", false, 0)]
    static void finishHandle()
    {
    
        checkPlugins();
        checkXmlHandle();
       // SolarEngineSettingsEditor.a
        
        AssetDatabase.Refresh();
        ShowTips("Successfully",  "SolarEngineSDK Successfully Matching Settings Panel");
    }
    
    // [MenuItem("SolarEngineSDK/Documentation", false, 0)]
    // static void documentation()
    // {
    //     Application.OpenURL("https://help.solar-engine.com/cn/docs/ugKp8t");
    // }
    
    
    [MenuItem("SolarEngineSDK/Documentation/UnityDocumentation", false, 0)]
    static void unityDocumentation()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/ugKp8t");
    }
    
    [MenuItem("SolarEngineSDK/Documentation/iOS ChangleLog", false, 0)]
    static void solarEngineDocsiOS()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/geng-xin-ri-zhi-RAvv");
    }
    [MenuItem("SolarEngineSDK/Documentation/Android ChangleLog", false, 0)]
    static void solarEngineDocsAndroid()
    {
        Application.OpenURL("https://help.solar-engine.com/cn/docs/geng-xin-ri-zhi");
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
        // if (SolarEngineSettings.isDisAll)
        //     PluginsEdtior.disableAll();
        if (!SolarEngineSettings.isUseiOS)
            PluginsEdtior.disableiOS();
        else
        {
            PluginsEdtior.showiOS();
        }
        if (!SolarEngineSettings.isUseAndroid)
            PluginsEdtior.disableAndroid();
        else
        {
            PluginsEdtior.showAndroid();
        }
        if (!SolarEngineSettings.isUseMiniGame)
            PluginsEdtior.disableMiniGame();
        else
        {
            PluginsEdtior.showMiniGame();
        }
        if(!SolarEngineSettings.isUseOaid)
            PluginsEdtior.disableOaid();
        else
        {
            PluginsEdtior.showOaid();
        }
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
