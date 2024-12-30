using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SolarEngine.Build
{
    public class DefineSymbolsEditor
    {
        const string SOLORENGINE_DISABLE_REMOTECONFIG = "SOLORENGINE_DISABLE_REMOTECONFIG";
        const string SE_DIS_RC = "SE_DIS_RC";
        const string SE_MINI_DIS_RC = "SE_MINI_DIS_RC";

        public static void add_DISABLE_REMOTECONFIG(BuildTargetGroup target,bool isminigame)
        {
          
            string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            string newvalue=isminigame? SE_MINI_DIS_RC : SE_DIS_RC;

            if (str.Contains(SOLORENGINE_DISABLE_REMOTECONFIG))
            {
                str = str.Replace(SOLORENGINE_DISABLE_REMOTECONFIG, newvalue);
              
            }
            else if(!str.Contains(newvalue))
            {
                str += $";{newvalue}";                
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target,
                str);
          
          
            
        }
        
      

        public static void delete_DISABLE_REMOTECONFIG(BuildTargetGroup target,bool isMiniGame)
        {
            string str = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
            
            string define=isMiniGame? SE_MINI_DIS_RC : SE_DIS_RC;
            if (str.Contains(SOLORENGINE_DISABLE_REMOTECONFIG))
            {
                str = str.Replace(SOLORENGINE_DISABLE_REMOTECONFIG, "");
             
            }

            if (str.Contains(define))
            {
                str = str.Replace(define, "");
             
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(target,
                str);
        }
        
        
    }
    
    
    
    // public  class 
}