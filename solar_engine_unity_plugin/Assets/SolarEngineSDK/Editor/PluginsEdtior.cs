using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class PluginsEdtior : MonoBehaviour
{
   
   private const string SolorEngine = "[SolorEngine]";



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

  // [MenuItem(DisableAll, false, 0)]
   public static void disableAll ()
   {
      HidePath(RemoteConfigsCSPath);

   }
   public static void showAll ()
   {
      // showMiniGame();
      // showiOS();
      // showAndroid();
      ShowPath(RemoteConfigsCSPath);

   }
  
  // [MenuItem(DisableMiniGame, false, 0)]
   public static void disableMiniGame ()
   {
       HideFile(MiniGameRemoteConfigsPathMiniDll);
       HideFile(MiniGameRemoteConfigsPathMiniCS);
      
   }
   
   public static void showMiniGame ()
   {
       ShowFile(MiniGameRemoteConfigsPathMiniDll); 
       ShowFile(MiniGameRemoteConfigsPathMiniCS);
      
   }
  // [MenuItem(DisableiOS, false, 0)]
   public static void disableiOS ()
   {
      HideFile(RemoteConfigsPathiOSCS);
       HideFile(RemoteConfigsPathiOSMM);
       HideFile(RemoteConfigsPathiOSH); 
       HidePath(RemoteConfigsPathiOSXml);
   }
   public static void showiOS ()
   {
       ShowFile(RemoteConfigsPathiOSCS);
       ShowFile(RemoteConfigsPathiOSMM);
       ShowFile(RemoteConfigsPathiOSH); 
       ShowPath(RemoteConfigsPathiOSXml);
   }
   
   
  // [MenuItem(DisableAndroid, false, 0)]
   public static void disableAndroid ()
   { 
       HideFile(RemoteConfigsPathAndroidCS);
       HideFile(ConfigsPathAndroidJar);
      HidePath(RemoteConfigsPathAndroidXml);
   }
   public static void showAndroid ()
   {
       ShowFile(RemoteConfigsPathAndroidCS);
       ShowFile(ConfigsPathAndroidJar);
       ShowPath(RemoteConfigsPathAndroidXml);
   }


   public static void disableOaid()
   {
       HidePath(oaidPath);
   }
   public static void showOaid()
   {
       ShowPath(oaidPath);
   }
   private static void DisablePath(string path)
   {
      if (System.IO.Directory.Exists(path))
      {
       
         System.IO.Directory.Delete(path,true);
         Debug.Log($"{SolorEngine}Disable:  " + path);
      }
      else if (System.IO.Directory.Exists(path + "~"))
      {

      }
      else
      {
         Debug.LogError($"{SolorEngine}不存在 " + path);
      }

   }

  
   private static void DisableFile(string path)
        {
            if (File.Exists(path))
            {
              // System.IO. File.Move(path, path + "~");
              System.IO.File.Delete(path);
               Debug.Log($"{SolorEngine}Disable:  " + path);
            }
            else
            {
                Debug.LogError($"{SolorEngine}不存在 " + path);
            }
        }

   
 
   public static void HideFile(string path)
   {
       if (File.Exists(path))
       {
           System.IO. File.Move(path, path + "~");
           Debug.Log($"{SolorEngine}Successfully moved the file from '{path}' to '{path + "~"}'. The file at path '{path}' is now hidden.");
       }
       else
       {
           Debug.LogWarning($"{SolorEngine}The file at path '{path}' does not exist, so it cannot be hidden. It appears to be already disabled.");
       }
   }

   public static void HidePath(string path)
   {
       if (System.IO.Directory.Exists(path + "~"))
       {
           Debug.Log($"{SolorEngine}{path } already Hide. ");
           return;
       }
       if (System.IO.Directory.Exists(path))
       {
           System.IO.Directory.Move(path, path + "~");
           Debug.Log($"{SolorEngine}Successfully moved the directory from '{path}' to '{path + "~"}'. The directory at path '{path}' is now hidden.");
         
       }
       else if (System.IO.Directory.Exists(path + "~"))
       {
           Debug.Log($"{SolorEngine}The directory with the '~' suffix at path '{path + "~"}' already exists. It seems the directory was already hidden previously.");
       }
       else
       {
           Debug.LogWarning($"{SolorEngine}The directory at path '{path}' does not exist.");
       }
   }

   public static void ShowFile(string path)
   {
       if (File.Exists(path))
       {
           Debug.Log($"{SolorEngine}The file at path '{path}' already exists.");
       }
       else if (File.Exists(path + "~"))
       {
           System.IO. File.Move(path + "~", path);
           Debug.Log($"{SolorEngine}Successfully moved the file from '{path + "~"}' to '{path}'.");
       }
       else
       {
           Debug.LogWarning($"{SolorEngine}The file at path '{path}' does not exist");
       }
   }
 

   public static void ShowPath(string path)
   {
    
        if (System.IO.Directory.Exists(path + "~"))
       {
           if (System.IO.Directory.Exists(path))
           {
               System.IO.Directory.Delete(path) ;
           }
           System.IO.Directory.Move(path + "~", path);
        
           Debug.Log($"{SolorEngine}Successfully moved the directory from '{path + "~"}' to '{path}'.");
       }
       else
       {
           Debug.LogWarning($"{SolorEngine}The directory at path '{path}' does not exist");
       }
   }
  
   
   
   
}
