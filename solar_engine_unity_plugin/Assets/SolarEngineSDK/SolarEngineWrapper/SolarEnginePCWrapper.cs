#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using Newtonsoft.Json;
using UnityEngine;

namespace SolarEngine
{
    public partial class Analytics : MonoBehaviour
    {
      
        private static Dictionary<string, object> GetPresetProperties()
        {
          Debug.Log("Unity Editor: GetPresetProperties");
          return null;
        }

        private static void PreInitSeSdk(string appKey)
        {
          Debug.Log("Unity Editor: PreInitSeSdk");
        }


        private static void Init(string appKey, object userId, SEConfig config)
        {
           
           Debug.Log("Unity Editor: Init");
        }

        private static void Init(string appKey, string userId, SEConfig config, RCConfig rcConfig)
        {
         Debug.Log("Unity Editor: Init");
        }

        private static void SetVisitorID(string visitorId)
        {
           Debug.Log("Unity Editor: SetVisitorID");
        }

        private static string GetVisitorID()
        {
          Debug.Log("Unity Editor: GetVisitorID");
          return null;
        }

        private static void Login(string accountId)
        {
         Debug.Log("Unity Editor: Login");
        }

        private static string GetAccountId()
        {
          Debug.Log("Unity Editor: GetAccountId");
          return null;
        }

        private static void Logout()
        {
          Debug.Log("Unity Editor: Logout");
        }

        private static void SetGaid(string gaid)
        {
          Debug.Log("Unity Editor: SetGaid");
        }

        private static void SetChannel(string channel)
        {
            Debug.Log("Unity Editor: SetChannel");
        }

        private static void SetGDPRArea(bool isGDPRArea)
        {
            Debug.Log("Unity Editor: SetGDPRArea");
        }

        private static string GetDistinctId()
        {
         Debug.Log("Unity Editor: GetDistinctId");
         return null;
        }

        private static void GetDistinctId(Action<Distinct>dis)
        {
            Debug.Log("Unity Editor: GetDistinctId");
        }

        private static void SetSuperProperties(Dictionary<string, object> userProperties)
        {
            Debug.Log("Unity Editor: SetSuperProperties");
        }

        private static void UnsetSuperProperty(string key)
        {
           Debug.Log("Unity Editor: UnsetSuperProperty");
        }

        private static void ClearSuperProperties()
        {
           Debug.Log("Unity Editor: ClearSuperProperties");
        }

        private static void EventStart(string timerEventName)
        {
            Debug.Log("Unity Editor: EventStart");
        }

        private static void EventFinish(string timerEventName, Dictionary<string, object> attributes)
        {
           Debug.Log("Unity Editor: EventFinish");
        }

       

        private static void UserUpdate(Dictionary<string, object> userProperties)
        {
           Debug.Log("Unity Editor: UserUpdate");
        }

        private static void UserInit(Dictionary<string, object> userProperties)
        {
          Debug.Log("Unity Editor: UserInit");
        }

        private static void UserAdd(Dictionary<string, object> userProperties)
        {
         Debug.Log("Unity Editor: UserAdd");
        }

        private static void UserAppend(Dictionary<string, object> userProperties)
        {
            Debug.Log("Unity Editor: UserAppend");
        }

        private static void UserUnset(string[] keys)
        {
           Debug.Log("Unity Editor: UserUnset");
        }

        private static void UserDelete(SEUserDeleteType deleteType)
        {
            Debug.Log("Unity Editor: UserDelete");
        }

        private static string GetAttribution()
        {
            Debug.Log("Unity Editor: GetAttribution");
            return null;

        }

        private static void TrackFirstEvent(SEBaseAttributes attributes)
        {
           Debug.Log("Unity Editor: TrackFirstEvent");
        }

        private static void ReportIAPEvent(ProductsAttributes attributes)
        {
         Debug.Log("Unity Editor: ReportIAPEvent");
        }

        private static void ReportIAIEvent(AppImpressionAttributes attributes)
        {
          Debug.Log("Unity Editor: ReportIAIEvent");
        }

        private static void ReportAdClickEvent(AdClickAttributes attributes)
        {
         Debug.Log("Unity Editor: ReportAdClickEvent");
        }

        private static void ReportRegisterEvent(RegisterAttributes attributes)
        {
         Debug.Log("Unity Editor: ReportRegisterEvent");
        }

        private static void ReportLoginEvent(LoginAttributes attributes)
        {
          Debug.Log("Unity Editor: ReportLoginEvent");
        }

        private static void ReportOrderEvent(OrderAttributes attributes)
        {
          Debug.Log("Unity Editor: ReportOrderEvent");
        }

        private static void AppAttrEvent(AppAttributes attributes)
        {
          Debug.Log("Unity Editor: AppAttrEvent");
        }


        private static void SetPresetEvent(SEConstant_Preset_EventType eventType, Dictionary<string, object> attributes)
        {
          Debug.Log("Unity Editor: SetPresetEvent");
        }

        private static void ReportCustomEvent(string customEventName, Dictionary<string, object> attributes)
        {
         Debug.Log("Unity Editor: ReportCustomEvent");
        }

        private static void ReportCustomEventWithPreAttributes(string customEventName,
            Dictionary<string, object> customAttributes, Dictionary<string, object> preAttributes)
        {
         Debug.Log("Unity Editor: ReportCustomEventWithPreAttributes");
        }

        private static void ReportEventImmediately()
        {
            Debug.Log("Unity Editor: ReportEventImmediately");
        }

        private static void HandleDeepLinkUrl(string url)
        {
            Debug.Log("Unity Editor: HandleDeepLinkUrl not found");
        }


        private static void DeeplinkCompletionHandler(SESDKDeeplinkCallback callback)
        {
            Debug.Log("Unity Editor: DeeplinkCompletionHandler not found");

        }


        private static void DelayDeeplinkCompletionHandler(SESDKDelayDeeplinkCallback callback)
        {
            Debug.Log("Unity Editor: DelayDeeplinkCompletionHandler not found");
        }

        private static  void SetReferrerTitle(string title)
        {
            Debug.Log("Unity Editor: SetReferrerTitle ");
        }

        private static void SetXcxPageTitle(string title)
        {
           Debug.Log("Unity Editor: SetXcxPageTitle ");
        }
        
        
        
            /// <summary>
        /// 仅支持iOS
        /// SolarEngine 封装系统requestTrackingAuthorizationWithCompletionHandler接口
        /// callback 回调用户授权状态: 0: Not Determined；1: Restricted；2: Denied；3: Authorized ；999: system error
        /// </summary>
        private static void RequestTrackingAuthorizationWithCompletionHandler(SESDKATTCompletedCallback callback) {
         Debug.Log("Unity Editor: RequestTrackingAuthorizationWithCompletionHandler");

        }

        /// <summary>
        /// 仅支持iOS
        /// SolarEngine 封装系统updatePostbackConversionValue
        /// </summary>
        private static void UpdatePostbackConversionValue(int conversionValue, SKANUpdateCompletionHandler callback)
        {

            Debug.Log("Unity Editor: UpdatePostbackConversionValue");

        }
        /// <summary>
        /// 仅支持iOS
        /// SolarEngine 封装系统updateConversionValueCoarseValue
        /// </summary>
        private static void UpdateConversionValueCoarseValue(int fineValue, String coarseValue, SKANUpdateCompletionHandler callback)
        {

            Debug.Log("Unity Editor: UpdateConversionValueCoarseValue");


        }
        /// 仅支持iOS
        /// SolarEngine 封装系统updateConversionValueCoarseValueLockWindow
        /// </summary>
        private static void UpdateConversionValueCoarseValueLockWindow(int fineValue, String coarseValue, bool lockWindow, SKANUpdateCompletionHandler callback)
        {

            Debug.Log("Unity Editor: UpdateConversionValueCoarseValueLockWindow");

        }

     

    }
}
#endif