using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SolarEngine.Build
{
    public class DefineSymbolsEditor
    {

      
         [MenuItem("SolarEngineSDK/test1 ", false, 0)]

        public static void add_DISABLE_REMOTECONFIG(BuildTargetGroup target)
        {
            string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (str.Contains("SOLORENGINE_DISABLE_REMOTECONFIG"))
            {
                
            }
            else
            {
                // Debug.LogError(str.Contains("SOLARENGINE_BYTEDANCE"));
                // if (str.Contains("SOLARENGINE_BYTEDANCE"))
                // {
                    str += ";SOLORENGINE_DISABLE_REMOTECONFIG";
                 
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(target,
                        str);
              //  }
             
              
            }
          
          
            
        }
        [MenuItem("SolarEngineSDK/test2 ", false, 0)]

        public static void delete_DISABLE_REMOTECONFIG(BuildTargetGroup target)
        {
            string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (str.Contains("SOLORENGINE_DISABLE_REMOTECONFIG"))
            {
                str = str.Replace("SOLORENGINE_DISABLE_REMOTECONFIG", "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target,
                    str);
            }

            
        }
        
        
    }
}