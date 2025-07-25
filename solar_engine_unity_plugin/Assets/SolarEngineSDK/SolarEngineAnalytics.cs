using System;
using System.Collections;
using System.Collections.Generic;
using SolarEngine;
using UnityEngine;

namespace SolarEngine
{
   

    public class SolarEngineAnalytics : MonoBehaviour
    {
        [HideInInspector] public bool startManually = true;
        [HideInInspector] public bool preInitSeSdk=true;

        [HideInInspector] public string appKey;
        [HideInInspector] public bool logEnabled;
        [HideInInspector] public bool isDebugModel;
        [HideInInspector] public bool isGDPRArea;
        
        [HideInInspector] public bool isCoppaEnabled;
        [HideInInspector] public bool isKidsAppEnabled;
        [HideInInspector] public bool isEnable2GReporting;
        [HideInInspector] public bool deferredDeeplinkenable;
        [HideInInspector] public int attAuthorizationWaitingInterval;
        [HideInInspector] public string fbAppID;
        [HideInInspector] public string caid;
        
        
        [HideInInspector] public bool isUseRemoteConfig;
        [HideInInspector] public RCMergeType mergeType;
        [HideInInspector] public Dictionary<string, object> customIDProperties;
        [HideInInspector] public Dictionary<string, object> customIDEventProperties;
        [HideInInspector] public Dictionary<string, object> customIDUserProperties;
        
        [HideInInspector]public bool isUseCustomDomain;
        [HideInInspector] public string receiverDomain  ;
        [HideInInspector] public string ruleDomain;
        [HideInInspector] public string receiverTcpHost;
        [HideInInspector] public string ruleTcpHost;
        [HideInInspector] public string gatewayTcpHost;
        
        [HideInInspector] public string unionid;
        [HideInInspector] public string openid;
        [HideInInspector] public string anonymous_openid;
        [HideInInspector] public bool isInitTencentAdvertisingGameSDK;
        [HideInInspector] public int reportingToTencentSdk;
        
        [HideInInspector] public int user_action_set_id;
        [HideInInspector] public string secret_key;
        [HideInInspector] public string appid;
        [HideInInspector] public bool tencentSdkIsAutoTrack = true;
        
        
        
        
        [HideInInspector] public Analytics.SESDKInitCompletedCallback initCompletedCallback;
        [HideInInspector] public bool isShowSetting;
      
      private  void Start()
        {
          
            Debug.Log("SolarEngineAnalytics: Start");
            DontDestroyOnLoad(transform.gameObject);
            if (!this.startManually)
            {
                InitAnalytics();
            }

        }
        

        public void InitAnalytics()
        {
              if (this.preInitSeSdk)
                {
                    Analytics.preInitSeSdk(this.appKey);
                }

              SEConfig seConfig = new SEConfig();
             
                seConfig.isDebugModel = this.isDebugModel;
                seConfig.logEnabled = this.logEnabled;
                seConfig.isGDPRArea = this.isGDPRArea;
                seConfig.isCoppaEnabled = this.isCoppaEnabled;
                seConfig.isKidsAppEnabled = this.isKidsAppEnabled;
                seConfig.isEnable2GReporting = this.isEnable2GReporting;
                seConfig.deferredDeeplinkenable = this.deferredDeeplinkenable;
                seConfig.attAuthorizationWaitingInterval = this.attAuthorizationWaitingInterval;
                seConfig.fbAppID = this.fbAppID;
                seConfig.caid = this.caid;
                
                Debug.Log(seConfig.initCompletedCallback);
                
                Debug.Log(seConfig.attributionCallback);
                RCConfig rcConfig = new RCConfig
                {
                    enable = this.isUseRemoteConfig,
                    mergeType = this.mergeType
                };

                
                seConfig.customDomain = new SECustomDomain
                {
                    enable = this.isUseCustomDomain,
                    receiverDomain = this.receiverDomain,
                    ruleDomain = this.ruleDomain,
                    receiverTcpHost = this.receiverTcpHost,
                    ruleTcpHost = this.ruleTcpHost,
                    gatewayTcpHost = this.gatewayTcpHost
                };
                
                seConfig.miniGameInitParams = new MiniGameInitParams
                {
                    unionid = this.unionid,
                    openid = this.openid,
                    anonymous_openid = this.anonymous_openid,
                    isInitTencentAdvertisingGameSDK = this.isInitTencentAdvertisingGameSDK,
                    reportingToTencentSdk = this.reportingToTencentSdk,
                    tencentAdvertisingGameSDKInitParams = new TencentAdvertisingGameSDKInitParams
                    {
                        appid = this.appid,
                        secret_key = this.secret_key,
                        user_action_set_id = this.user_action_set_id,
                        tencentSdkIsAutoTrack = this.tencentSdkIsAutoTrack
                    }
                };
               
               
                if(  rcConfig.enable)
                    Analytics.initSeSdk(this.appKey, seConfig,rcConfig);
                else
                    Analytics.initSeSdk(this.appKey, seConfig);
            
        }

     

    }
}
