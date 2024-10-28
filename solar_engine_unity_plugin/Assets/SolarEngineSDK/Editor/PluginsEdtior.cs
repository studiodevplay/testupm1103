using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class PluginsEdtior : MonoBehaviour
{
   private const string SolarEngineSDK = "SolarEngineSDK";
   private const string RemoteConfig = SolarEngineSDK + "/RemoteConfig";
   
   
   
   
   
   private const string RemoteConfigDisable= RemoteConfig + "/Disable";
   private const string DisableAll = RemoteConfigDisable + "/DisableAll";
   private const string DisableMiniGame = RemoteConfigDisable + "/DisableMiniGame";
   private const string DisableiOS= RemoteConfigDisable + "/DisableiOS";
   private const string DisableAndroid = RemoteConfigDisable + "/DisableAndroid";



   private const string PluginsSolarEnginePath = "Assets/Plugins/SolarEngine/";
   private const string RemoteConfigsCSPath = "Assets/SolarEngineSDK/RemoteConfigWrapper";
   private const string RemoteConfigXmlPath = SolarEngineNet+"SolarEnginePlugins/RemoteConfigSDK";
   
   private const string SolarEngineNet = "Assets/SolarEngineNet/";
   
   
   
   
   
   
   private const string oaidPath = SolarEngineNet+"SolarEnginePlugins/Oaid";
   
   //minigamepath
   private const string MiniGameRemoteConfigsPathMiniCS = "Assets/SolarEngineSDK/RemoteConfigWrapper/SESDKRemoteConfigMiniGameWrapper.cs";
   private const string MiniGameRemoteConfigsPathMiniDll = PluginsSolarEnginePath+"MiniGame/MiniGameRemoteConfig.dll";

   
   //iospath
   private const string RemoteConfigsPathiOSCS = "Assets/SolarEngineSDK/RemoteConfigWrapper/SESDKRemoteConfigiOSWrapper.cs";
   private const string RemoteConfigsPathiOSMM = PluginsSolarEnginePath+"iOS/wrappers/SESDKRemoteConfigUnityBridge.mm";
   private const string RemoteConfigsPathiOSH =  PluginsSolarEnginePath+"iOS/wrappers/SESDKRemoteConfigUnityBridge.h";
   private const string RemoteConfigsPathiOSXml = RemoteConfigXmlPath+"/iOS";
   //androidpath
   private const string RemoteConfigsPathAndroidCS = "Assets/SolarEngineSDK/RemoteConfigWrapper/SESDKRemoteConfigAndroidWrapper.cs";
   private const string ConfigsPathAndroidJar = PluginsSolarEnginePath+"Android/libs/se_remote_config_unity_bridge.jar";
   private const string RemoteConfigsPathAndroidXml = RemoteConfigXmlPath+"/Android";

   [MenuItem(DisableAll, false, 0)]
   public static void disableAll ()
   {
      DisableFile(MiniGameRemoteConfigsPathMiniDll);
      DisableFile(RemoteConfigsPathiOSMM);
      DisableFile(RemoteConfigsPathiOSH);
      DisableFile(ConfigsPathAndroidJar);
      
      DisablePath(RemoteConfigsCSPath);
      DisablePath(RemoteConfigXmlPath);
      
   }
  
   [MenuItem(DisableMiniGame, false, 0)]
   public static void disableMiniGame ()
   {
      DisableFile(MiniGameRemoteConfigsPathMiniDll);
      DisableFile(MiniGameRemoteConfigsPathMiniCS);
      
   }
   [MenuItem(DisableiOS, false, 0)]
   public static void disableiOS ()
   {
    DisableFile(RemoteConfigsPathiOSCS);
    DisableFile(RemoteConfigsPathiOSMM);
    DisableFile(RemoteConfigsPathiOSH);
    DisablePath(RemoteConfigsPathiOSXml);
   }
   
   [MenuItem(DisableAndroid, false, 0)]
   public static void disableAndroid ()
   {
      DisableFile(RemoteConfigsPathAndroidCS);
      DisableFile(ConfigsPathAndroidJar);
      DisablePath(RemoteConfigsPathAndroidXml);
   }


   public static void disableOaid()
   {
      DisablePath(oaidPath);
   }
   private static void DisablePath(string path)
   {
      if (System.IO.Directory.Exists(path))
      {
       
         System.IO.Directory.Delete(path,true);
         Debug.Log("Disable:  " + path);
      }
      else if (System.IO.Directory.Exists(path + "~"))
      {

      }
      else
      {
         Debug.LogError("不存在 " + path);
      }

   }

  
   private static void DisableFile(string path)
        {
            if (File.Exists(path))
            {
              // System.IO. File.Move(path, path + "~");
              System.IO.File.Delete(path);
               Debug.Log("Disable:  " + path);
            }
            else
            {
                Debug.LogError("不存在 " + path);
            }
        }


 
  
   
   
   
}
