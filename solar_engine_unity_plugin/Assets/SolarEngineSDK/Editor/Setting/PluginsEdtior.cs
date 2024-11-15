using System;
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
   public static bool disableMiniGame ()
   {
     return  HideFile(MiniGameRemoteConfigsPathMiniDll)&&
       HideFile(MiniGameRemoteConfigsPathMiniCS);
      
   }
   
   public static bool showMiniGame ()
   {
     return  ShowFile(MiniGameRemoteConfigsPathMiniDll)&&
       ShowFile(MiniGameRemoteConfigsPathMiniCS);
      
   }
  // [MenuItem(DisableiOS, false, 0)]
   public static bool disableiOS ()
   {
       return  
        HideFile(RemoteConfigsPathiOSCS)&&
       HideFile(RemoteConfigsPathiOSMM)&&
       HideFile(RemoteConfigsPathiOSH)&&
       HidePath(RemoteConfigsPathiOSXml);
   }
   public static bool showiOS ()
   {
   
      return
       ShowFile(RemoteConfigsPathiOSCS)&&
       ShowFile(RemoteConfigsPathiOSMM)&&
       ShowFile(RemoteConfigsPathiOSH)&&
       ShowPath(RemoteConfigsPathiOSXml);
   }
   
   
  // [MenuItem(DisableAndroid, false, 0)]
   public static bool disableAndroid ()
   { 
     return HideFile(RemoteConfigsPathAndroidCS)&&
       HideFile(ConfigsPathAndroidJar)&&
      HidePath(RemoteConfigsPathAndroidXml);
   }
   public static bool showAndroid ()
   {
       return ShowFile(RemoteConfigsPathAndroidCS)&&
       ShowFile(ConfigsPathAndroidJar)&&
       ShowPath(RemoteConfigsPathAndroidXml);
   }


   public static bool disableOaid()
   {
       return HidePath(oaidPath);
   }
   public static bool showOaid()
   {
     return  ShowPath(oaidPath);
   }

   
 
   public static bool HideFile(string path)
   {
      
       if (File.Exists(path))
       {
           System.IO. File.Move(path, path + "~");

           return true;
           //  Debug.LogWarning($"{SolorEngine} The file at path '{path}' is successfully hidden.");
       }
       else
       {
           
           //Debug.LogWarning($"{SolorEngine}The file at path '{path}' does not exist, so it cannot be hidden. It appears to be already disabled.");
       }
       return false;
   }

   public static bool HidePath(string path)
   {
    
           if (System.IO.Directory.Exists(path + "~"))
           {
           
               return true;
           }
           if (System.IO.Directory.Exists(path))
           {
               System.IO.Directory.Move(path, path + "~");
               return true;
           
         
           }
   

       return false;


   }

   public static bool ShowFile(string path)
   {
       try
       {
           if (File.Exists(path))
           {
               return true;
           }
           else if (File.Exists(path + "~"))
           {
               System.IO. File.Move(path + "~", path);
               return true;
           }
       }
       catch (Exception e)
       {
           Debug.LogError(SolorEngine+e);

           return false;
       }
    
     

       return false;
   }
 

   public static bool ShowPath(string path)
   {
       try
       {
           if (System.IO.Directory.Exists(path + "~"))
           {
               if (System.IO.Directory.Exists(path))
               {
                   System.IO.Directory.Delete(path) ;
               }
               System.IO.Directory.Move(path + "~", path);

               return true;
           }
           else if( System.IO.Directory.Exists(path))
           {
               return true;
           }
           else
           {
               return false;
           }
       }
       catch (Exception e)
       {
           Debug.LogError(SolorEngine+e);
           return false;
          
       }
    
      
   }
  
   
   
   
}
