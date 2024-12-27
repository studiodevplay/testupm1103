using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
#if SOLARENGINE_WECHAT && TUANJIE_WEIXINMINIGAME
using WeChatWASM;
#endif

    public  static class SolarEngineMiniGameConvertCore
    {
        private const string SolarEngineMiniGameConvertCoreLOG = "SolarEngineMiniGameConvertCore";
        const string dyCpJsPath = "Assets/StreamingAssets/__cp_js_files";
        const string sourceJsPath = "Assets/Plugins/SolarEngine/MiniGame/SolarEngineJsHelper.js";
     

#if SOLARENGINE_WECHAT && TUANJIE_WEIXINMINIGAME
        public class WxChatConvertCore: LifeCycleBase
        {
            public override void beforeCopyDefault()
            {
                // 读取你的自定义模板目录并对其中的资源做动态修改
                var tmp = BuildTemplateHelper.CustomTemplateDir;
                if (!Directory.Exists(tmp))
                {
                    Debug.Log(
                        $"{SolarEngineMiniGameConvertCoreLOG} WeChat Custom template directory does not exist! Create ");

                    Directory.CreateDirectory(tmp);

                }
                string tmpPath = Path.Combine(tmp, "SolarEngineJsHelper.js");
                if( File.Exists(tmpPath))
                    File.Delete(tmpPath);
                
                File.Copy(sourceJsPath, tmp);
                Debug.Log($"{SolarEngineMiniGameConvertCoreLOG} SolarEngineHelper.js Copy successfully");
                
            }

         
            public override void afterBuildTemplate()
            {
                
                addJsCode();
            }
            

            static void addJsCode()
            {
                string jsCode = "import 'SolarEngineJsHelper.js';";
                string gamejsPath = Path.Combine(UnityUtil.GetEditorConf().ProjectConf.DST, "minigame", "game.js");


                File.AppendAllText(gamejsPath, jsCode);


                Debug.Log($"{SolarEngineMiniGameConvertCoreLOG} SolarEngineHelper Content inserted successfully");



            }

        }
        
#endif
    
    }


