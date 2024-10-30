#if (SOLARENGINE_BYTEDANCE||SOLARENGINE_WECHAT||SOLARENGINE_KUAISHOU)&&(!UNITY_EDITOR||SOLORENGINE_DEVELOPEREDITOR)
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
// using SolarEngine.Sample;
using AOT;
using SolarEngine.MiniGameRemoteConfig;
using SolarEngine.MiniGameRemoteConfig.Info;

namespace SolarEngine
{
    public partial class SESDKRemoteConfig : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initializes()
        {

#if SOLARENGINE_WECHAT
            SEAdapterInterface _adapter = new SolarEngine.Platform. WeChatAdapter();
                MiniRemoteConfigInfo.setAdapterInterface(_adapter);
#elif SOLARENGINE_BYTEDANCE
                SEAdapterInterface _adapter = new SolarEngine.Platform.ByteDanceAdapter();
                
                MiniRemoteConfigInfo .setAdapterInterface(_adapter);
#elif SOLARENGINE_KUAISHOU
                SEAdapterInterface _adapter = new  KuaiShouAdapter();
                MiniRemoteConfigInfo.setAdapterInterface(_adapter);
#endif

        }

        private void init(SERemoteConfigInterface se)
        {
            MiniRemoteConfigWrapper.Instance.init(se);
               
        }
   
        private void SESDKSetRemoteDefaultConfig(Dictionary<string, object>[] defaultConfig)
        {

            if (defaultConfig == null)
            {
                return;
            }
           MiniRemoteConfigWrapper.Instance.setRemoteDefaultConfig(defaultConfig);
      
        }


        private void SESDKSetRemoteConfigEventProperties(Dictionary<string, object> properties)
        {

            if (properties == null)
            {
                return;
            }
            MiniRemoteConfigWrapper.Instance.setRemoteConfigEventProperties(properties);
            

        }

        private void SESDKSetRemoteConfigUserProperties(Dictionary<string, object> properties)
        {

            if (properties == null)
            {
                return;
            }
            MiniRemoteConfigWrapper.Instance.setRemoteConfigUserProperties(properties);
    

        }

       
        private string SESDKFastFetchRemoteConfig(string key)
        {
            if (key == null)
            {
                return null;
            }
       
            return MiniRemoteConfigWrapper.Instance.fastFetchRemoteConfig(key);

        }

        private Dictionary<string, object> SESDKFastFetchAllRemoteConfig()
        {
       
            return MiniRemoteConfigWrapper.Instance.fastFetchRemoteConfig();
        }

        private void SESDKAsyncFetchAllRemoteConfig()
        {

            MiniRemoteConfigWrapper.MiniFetchAllRemoteConfigCallback _miniFetchAll=  (MiniRemoteConfigWrapper.MiniFetchAllRemoteConfigCallback)Delegate.CreateDelegate(typeof(MiniRemoteConfigWrapper.MiniFetchAllRemoteConfigCallback), SESDKRemoteConfig.Instance.fetchAllRemoteConfigCallback_private.Target, SESDKRemoteConfig.Instance.fetchAllRemoteConfigCallback_private.Method);
            MiniRemoteConfigWrapper.Instance.asyncFetchRemoteConfig(_miniFetchAll);
        }

        private void SESDKAsyncFetchRemoteConfig(string key)
        {
            if (key == null)
            {
                return;
            }
            MiniRemoteConfigWrapper.MiniFetchRemoteConfigCallback _miniFetch=  (MiniRemoteConfigWrapper.MiniFetchRemoteConfigCallback)Delegate.CreateDelegate(typeof(MiniRemoteConfigWrapper.MiniFetchRemoteConfigCallback), SESDKRemoteConfig.Instance.fetchRemoteConfigCallback_private.Target, SESDKRemoteConfig.Instance.fetchRemoteConfigCallback_private.Method);
          
            MiniRemoteConfigWrapper.Instance.asyncFetchRemoteConfig(key,_miniFetch);

        }


    }
}
#endif