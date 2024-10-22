using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class SolarEngineEdtior : MonoBehaviour
{
   private const string SolarEngineSDK = "SolarEngineSDK";
   private const string RemoteConfig = SolarEngineSDK + "/RemoteConfig";
   private const string RemoteConfigDelete = RemoteConfig + "/RemoteConfigDelete";
   private const string DeleteAll = RemoteConfigDelete + "/DeleteAll";
   private const string DeleteMiniGame = RemoteConfigDelete + "/DeleteMiniGame";

   private const string RemoteConfigShow = RemoteConfig + "/RemoteConfigShow";
   private const string RemoteConfigShowAll = RemoteConfigShow + "/RemoteConfigShowAll";
   private const string ShowMiniGame = RemoteConfigShow + "/ShowMiniGame";
   
   
   
   
   

   private const string PluginsSolarEnginePath = "Assets/Plugins/SolarEngine/";
   private const string MiniGameRemoteConfigsPath = "Assets/SolarEngineSDK/RemoteConfigWrapper";
   private const string MiniGameRemoteConfigsPathMini = "Assets/SolarEngineSDK/RemoteConfigWrapper/SESDKRemoteConfigMiniGameWrapper.cs";
   private const string MiniGameRemoteConfigsPathMinidll = PluginsSolarEnginePath+"MiniGame/MiniGameRemoteConfig.dll";

   
   // [MenuItem(DeleteAll, false, 0)]
   // public static void deleteAllRemoteConfig ()
   // {
   //       HidePath(MiniGameRemoteConfigsPath);
   //     
   // }
   //
   [MenuItem(DeleteMiniGame, false, 0)]
   public static void deleteRemoteConfigMiniGame ()
   {
         HideFile(MiniGameRemoteConfigsPathMinidll);
         HideFile(MiniGameRemoteConfigsPathMini);
      
   }
   
   // [MenuItem(RemoteConfigShowAll, false, 0)]
   // public static void showAllRemoteConfig ()
   // {
   //       ShowPath(MiniGameRemoteConfigsPath);
   //    
   // }
   
   [MenuItem(ShowMiniGame, false, 0)]
   public static void showRemoteConfigMiniGame ()
   {
      showFile(MiniGameRemoteConfigsPathMinidll);
      showFile(MiniGameRemoteConfigsPathMini);
      
   }
   
   
   
   
   

   
   private static void HidePath(string path)
   {
      if (System.IO.Directory.Exists(path))
      {
         System.IO.Directory.Move(path, path + "~");
         Debug.Log("delete:  " + path);
      }
      else if (System.IO.Directory.Exists(path + "~"))
      {

      }
      else
      {
         Debug.LogError("不存在 " + path);
      }

   }

   private static void ShowPath(string path)
   {
      if (System.IO.Directory.Exists(path))
      {

      }
      else if (System.IO.Directory.Exists(path + "~"))
      {
         System.IO.Directory.Move(path + "~", path);
      }
      else
      {
         Debug.LogError("不存在 " + path);
      }

   }
   private static void HideFile(string path)
        {
            if (File.Exists(path))
            {
               System.IO.  File.Move(path, path + "~");
               Debug.Log("delete:  " + path);
            }
            else
            {
                Debug.LogError("不存在 " + path);
            }
        }
   private static void showFile(string path)
      {
         if (File.Exists(path))
         {
      
         }
         else if (File.Exists(path + "~"))
         {
            System.IO.  File.Move(path + "~", path);
            Debug.Log("show:  " + path);
         }
         else
         {
            Debug.LogError("不存在 " + path);
         }
      }
}
