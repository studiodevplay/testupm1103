using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace SolarEngine
{

   public partial class Analytics : MonoBehaviour
    {
        private static void initCallBack(SEConfig config)
        {
            if (config.initCompletedCallback != null)
            {
                Analytics.Instance.initCompletedCallback_private = config.initCompletedCallback;
            }

            if (config.attributionCallback != null)
            {
                Analytics.Instance.attributionCallback_private = config.attributionCallback;
            }
        }

        private static string initSeDict(SEConfig config)
        {
            Dictionary<string, object> seDict = new Dictionary<string, object>();
            seDict.Add("isDebugModel", config.isDebugModel);
            seDict.Add("logEnabled", config.logEnabled);
            seDict.Add("isGDPRArea", config.isGDPRArea);
            seDict.Add("isEnable2GReporting", config.isEnable2GReporting);
            seDict.Add("sub_lib_version", sdk_version);
            seDict.Add("attAuthorizationWaitingInterval", config.attAuthorizationWaitingInterval);
            seDict.Add("caid", config.caid);
            seDict.Add("delayDeeplinkEnable", config.delayDeeplinkEnable);
            seDict.Add("isCoppaEnabled", config.isCoppaEnabled);
            seDict.Add("isKidsAppEnabled", config.isKidsAppEnabled);
            initCallBack(config);
            string seJonString = JsonConvert.SerializeObject(seDict);
            return seJonString;
        }

        private static string initRcDict(RCConfig rcConfig)
        {
            Dictionary<string, object> rcDict = new Dictionary<string, object>();

            rcDict.Add("enable", rcConfig.enable);
            rcDict.Add("mergeType", rcConfig.mergeType);

            if (rcConfig.customIDProperties != null)
            {
                rcDict.Add("customIDProperties", rcConfig.customIDProperties);
            }

            if (rcConfig.customIDEventProperties != null)
            {
                rcDict.Add("customIDEventProperties", rcConfig.customIDEventProperties);
            }

            if (rcConfig.customIDUserProperties != null)
            {
                rcDict.Add("customIDUserProperties", rcConfig.customIDUserProperties);
            }

            string rcJonString = JsonConvert.SerializeObject(rcDict);
            return rcJonString;
        }

        #region GetTrackDic

        private static Dictionary<string, object> getFirstDic(SEBaseAttributes attributes, bool isAddCustomProperties=true)
        {
             Dictionary<string, object> attributesDict = new Dictionary<string, object>();

            if (attributes is RegisterAttributes registerAttributes)
            {
                // 处理 RegisterAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE,
                    SolarEngine.Analytics.SEConstant_Register);
                attributesDict.Add(SolarEngine.Analytics.SEConstant_Register_Type, registerAttributes.register_type);
                attributesDict.Add(SolarEngine.Analytics.SEConstant_Register_Status,
                    registerAttributes.register_status);
                if (registerAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_Register_CustomProperties,
                        registerAttributes.customProperties);
                }

                if (registerAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, registerAttributes.checkId);
                }
            }

            if (attributes is LoginAttributes loginAttributes)
            {
                // 处理 LoginAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE, SolarEngine.Analytics.SEConstant_Login);
                attributesDict.Add(SolarEngine.Analytics.SEConstant_Login_Type, loginAttributes.login_type);
                attributesDict.Add(SolarEngine.Analytics.SEConstant_Login_Status, loginAttributes.login_status);
                if (loginAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_Login_CustomProperties,
                        loginAttributes.customProperties);
                }

                if (loginAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, loginAttributes.checkId);
                }
            }

            if (attributes is OrderAttributes orderAttributes)
            {
                // 处理 OrderAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE, SolarEngine.Analytics.SEConstant_Order);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_ID, orderAttributes.order_id);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Currency_Type, orderAttributes.currency_type);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Pay_Type, orderAttributes.pay_type);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Status, orderAttributes.status);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Pay_Amount, orderAttributes.pay_amount);

                if (orderAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_CustomProperties,
                        orderAttributes.customProperties);
                }

                if (orderAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, orderAttributes.checkId);
                }
            }

            if (attributes is AppAttributes appAttributes)
            {
                // 处理 AppAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE,
                    SolarEngine.Analytics.SEConstant_AppAttr);
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Network, appAttributes.ad_network);
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_AttributionPlatform,
                    appAttributes.attribution_platform);
                if (appAttributes.sub_channel != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Sub_Channel, appAttributes.sub_channel);
                }

                if (appAttributes.ad_account_id != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Account_ID,
                        appAttributes.ad_account_id);
                }

                if (appAttributes.ad_account_name != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Account_Name,
                        appAttributes.ad_account_name);
                }

                if (appAttributes.ad_campaign_id != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Campaign_ID,
                        appAttributes.ad_campaign_id);
                }

                if (appAttributes.ad_campaign_name != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Campaign_Name,
                        appAttributes.ad_campaign_name);
                }

                if (appAttributes.ad_offer_id != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Offer_ID, appAttributes.ad_offer_id);
                }

                if (appAttributes.ad_offer_name != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Offer_Name,
                        appAttributes.ad_offer_name);
                }

                if (appAttributes.ad_creative_id != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Creative_ID,
                        appAttributes.ad_creative_id);
                }

                if (appAttributes.ad_creative_name != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Creative_Name,
                        appAttributes.ad_creative_name);
                }

                if (appAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_CustomProperties,
                        appAttributes.customProperties);
                }

                if (appAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, appAttributes.checkId);
                }
            }

            if (attributes is AdClickAttributes adClickAttributes)
            {
                // 处理 AdClickAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE,
                    SolarEngine.Analytics.SEConstant_AdClick);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_AdPlatform, adClickAttributes.ad_platform);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_MediationPlatform,
                    adClickAttributes.mediation_platform);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_AdId, adClickAttributes.ad_id);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_AdType, adClickAttributes.ad_type);

                if (adClickAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_CustomProperties,
                        adClickAttributes.customProperties);
                }

                if (adClickAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, adClickAttributes.checkId);
                }
            }

            if (attributes is AppImpressionAttributes appImpressionAttributes)
            {
                // 处理 AppImpressionAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE, SolarEngine.Analytics.SEConstant_IAI);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdPlatform,
                    appImpressionAttributes.ad_platform);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_MediationPlatform,
                    appImpressionAttributes.mediation_platform);

                if (appImpressionAttributes.ad_appid != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdAppid, appImpressionAttributes.ad_appid);
                }

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdId, appImpressionAttributes.ad_id);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdType, appImpressionAttributes.ad_type);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdEcpm, appImpressionAttributes.ad_ecpm);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_CurrencyType,
                    appImpressionAttributes.currency_type);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_IsRendered,
                    appImpressionAttributes.is_rendered);

                if (appImpressionAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_CustomProperties,
                        appImpressionAttributes.customProperties);
                }

                if (appImpressionAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, appImpressionAttributes.checkId);
                }
            }

            if (attributes is ProductsAttributes productsAttributes)
            {
                // 处理 ProductsAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE, SolarEngine.Analytics.SEConstant_IAP);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_OrderId, productsAttributes.order_id);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_Amount, productsAttributes.pay_amount);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_Currency, productsAttributes.currency_type);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PayType, productsAttributes.pay_type);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PID, productsAttributes.product_id);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PName, productsAttributes.product_name);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PCount, productsAttributes.product_num);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_Paystatus, productsAttributes.paystatus);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_FailReason,
                    productsAttributes.fail_reason == null ? "" : productsAttributes.fail_reason);

                if (productsAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_CustomProperties,
                        productsAttributes.customProperties);
                }

                if (productsAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, productsAttributes.checkId);
                }
            }

            if (attributes is CustomAttributes customAttributes)
            {
                // 处理 CustomAttributes 类型的逻辑
                attributesDict.Add(SolarEngine.Analytics.SEConstant_EVENT_TYPE,
                    SolarEngine.Analytics.SEConstant_CUSTOM);

                attributesDict.Add(SolarEngine.Analytics.SEConstant_CUSTOM_EVENT_NAME,
                    customAttributes.custom_event_name);

                if (customAttributes.customProperties != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_Custom_CustomProperties,
                        customAttributes.customProperties);
                }

                if (customAttributes.checkId != null)
                {
                    attributesDict.Add(SolarEngine.Analytics.SEConstant_CHECK_ID, customAttributes.checkId);
                }
            }

          
            return attributesDict;
        }




        #endregion

    private static Dictionary<string,object> getIAPDic(ProductsAttributes attributes,bool isAddCustomProperties=true)
       {
           Dictionary<string, object> attributesDict = new Dictionary<string, object>();

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_OrderId, attributes.order_id);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_Amount, attributes.pay_amount);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_Currency, attributes.currency_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PayType, attributes.pay_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PID, attributes.product_id);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PName, attributes.product_name);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_PCount, attributes.product_num);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_Paystatus, attributes.paystatus);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_FailReason,
               attributes.fail_reason == null ? "" : attributes.fail_reason);

           if (attributes.customProperties != null&& isAddCustomProperties)
           {
               attributesDict.Add(SolarEngine.Analytics.SEConstant_IAP_CustomProperties, attributes.customProperties);
           }
           return  attributesDict;

       }

       private static Dictionary<string, object> getIAIDic(AppImpressionAttributes attributes, bool isAddCustomProperties = true)
       {
           Dictionary<string, object> attributesDict = new Dictionary<string, object>();

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdPlatform, attributes.ad_platform);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_MediationPlatform, attributes.mediation_platform);

           if (attributes.ad_appid != null)
           {
               attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdAppid, attributes.ad_appid);
           }

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdId, attributes.ad_id);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdType, attributes.ad_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_AdEcpm, attributes.ad_ecpm);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_CurrencyType, attributes.currency_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_IsRendered, attributes.is_rendered);

           if (attributes.customProperties != null&& isAddCustomProperties)
           {
               attributesDict.Add(SolarEngine.Analytics.SEConstant_IAI_CustomProperties, attributes.customProperties);
           }
           return attributesDict;
       }

       public static Dictionary<string, object> getAdClickDic(AdClickAttributes attributes, bool isAddCustomProperties = true)
       {
           
           Dictionary<string, object> attributesDict = new Dictionary<string, object>();

           attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_AdPlatform, attributes.ad_platform);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_MediationPlatform,
               attributes.mediation_platform);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_AdId, attributes.ad_id);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_AdType, attributes.ad_type);

           if (attributes.customProperties != null&& isAddCustomProperties)
           {
               attributesDict.Add(SolarEngine.Analytics.SEConstant_AdClick_CustomProperties,
                   attributes.customProperties);
           }
           return  attributesDict;
       }

       public static Dictionary<string, object> getRegisterDic(RegisterAttributes attributes, bool isAddCustomProperties = true)
       {
           Dictionary<string, object> attributesDict = new Dictionary<string, object>();

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Register_Type, attributes.register_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Register_Status, attributes.register_status);

           if (attributes.customProperties != null&& isAddCustomProperties)
           {
               attributesDict.Add(SolarEngine.Analytics.SEConstant_Register_CustomProperties,
                   attributes.customProperties);
           }
           return  attributesDict;
       }

       public static Dictionary<string, object> getLoginDic(LoginAttributes attributes, bool isAddCustomProperties = true)
       {
           Dictionary<string, object> attributesDict = new Dictionary<string, object>();

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Login_Type, attributes.login_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Login_Status, attributes.login_status);

           if (attributes.customProperties != null&& isAddCustomProperties)
           {
               attributesDict.Add(SolarEngine.Analytics.SEConstant_Login_CustomProperties,
                   attributes.customProperties);
           }
           return   attributesDict;
       }

       public static Dictionary<string, object> getOrderDic(OrderAttributes attributes,bool isAddCustomProperties = true)
       {
           Dictionary<string, object> attributesDict = new Dictionary<string, object>();

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_ID, attributes.order_id);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Currency_Type, attributes.currency_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Pay_Type, attributes.pay_type);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Status, attributes.status);

           attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_Pay_Amount, attributes.pay_amount);

           if (attributes.customProperties != null&&isAddCustomProperties)
           {
               attributesDict.Add(SolarEngine.Analytics.SEConstant_Order_CustomProperties,
                   attributes.customProperties);
           }
           return  attributesDict;
       }

       public static Dictionary<string, object> getAttrDic(AppAttributes attributes, bool isAddCustomProperties = true)
       {
    Dictionary<string, object> attributesDict = new Dictionary<string, object>();

            attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Network, attributes.ad_network);
            attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_AttributionPlatform,
                attributes.attribution_platform);

            if (attributes.sub_channel != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Sub_Channel, attributes.sub_channel);
            }

            if (attributes.ad_account_id != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Account_ID, attributes.ad_account_id);
            }

            if (attributes.ad_account_name != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Account_Name,
                    attributes.ad_account_name);
            }

            if (attributes.ad_campaign_id != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Campaign_ID, attributes.ad_campaign_id);
            }

            if (attributes.ad_campaign_name != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Campaign_Name,
                    attributes.ad_campaign_name);
            }

            if (attributes.ad_offer_id != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Offer_ID, attributes.ad_offer_id);
            }

            if (attributes.ad_offer_name != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Offer_Name, attributes.ad_offer_name);
            }

            if (attributes.ad_creative_id != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Creative_ID, attributes.ad_creative_id);
            }

            if (attributes.ad_creative_name != null)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_Creative_Name,
                    attributes.ad_creative_name);
            }

            if (attributes.customProperties != null&& isAddCustomProperties)
            {
                attributesDict.Add(SolarEngine.Analytics.SEConstant_AppAttr_Ad_CustomProperties,
                    attributes.customProperties);
            }
            return  attributesDict;

       }


       private static string getPresetEventName(SEConstant_Preset_EventType eventType)
       {
           string eventName = "";
           if (eventType == SEConstant_Preset_EventType.SEConstant_Preset_EventType_AppInstall)
           {
               eventName = "SEPresetEventTypeAppInstall";
           }
           else if (eventType == SEConstant_Preset_EventType.SEConstant_Preset_EventType_AppStart)
           {
               eventName = "SEPresetEventTypeAppStart";
           }
           else if (eventType == SEConstant_Preset_EventType.SEConstant_Preset_EventType_AppEnd)
           {
               eventName = "SEPresetEventTypeAppEnd";
           }
           else if (eventType == SEConstant_Preset_EventType.SEConstant_Preset_EventType_All)
           {
               eventName = "SEPresetEventTypeAppAll";
           }

           return eventName;
       }

      
    }
    
    

    [Serializable]
    public enum SEUserDeleteType
    {
        // 通过AccountId删除用户
        SEUserDeleteTypeByAccountId = 0,

        // 通过VisitorId删除用户
        SEUserDeleteTypeByVisitorId = 1,
    }


    [Serializable]
    public enum SEConstant_IAP_PayStatus
    {
        // 成功
        SEConstant_IAP_PayStatus_success = 1,
        // 失败
        SEConstant_IAP_PayStatus_fail = 2,
        // 恢复
        SEConstant_IAP_PayStatus_restored = 3      
    };

    [Serializable]
    public enum SEConstant_Preset_EventType
    {
        // _appInstall预置事件
        SEConstant_Preset_EventType_AppInstall ,
        // _appStart
        SEConstant_Preset_EventType_AppStart  ,
        // _appEnd
        SEConstant_Preset_EventType_AppEnd   ,
        // _appInstall、_appStart、_appEnd三者
        SEConstant_Preset_EventType_All  

    };

    public interface SEBaseAttributes
    {
        // checkId
        string checkId { get; set; }
    }

    [Serializable]
    public struct CustomAttributes : SEBaseAttributes
    {
        // 自定义事件名
        public string custom_event_name { get; set; }

        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }

        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }
    }

    [Serializable]
    public struct ProductsAttributes : SEBaseAttributes
    {
        // 商品名称 不可为空
        public string product_name { get; set; }
        // 商品 ID 不可为空
        public string product_id { get; set; }
        // 购买商品的数量 不可为空
        public int product_num { get; set; }
        // 支付的货币各类，遵循《ISO 4217国际标准》，如 CNY、USD... 不可为空
        public string currency_type { get; set; }
        // 本次购买由系统生成的订单 ID 不可为空
        public string order_id { get; set; }
        // 支付失败的原因 可以为空
        public string fail_reason { get; set; }
        // 支付状态 详见 SEConstant_IAP_PayStatus 枚举
        public SEConstant_IAP_PayStatus paystatus { get; set; }
        // 支付方式：如 alipay、weixin、applepay、paypal 等
        public string pay_type { get; set; }
        // 本次购买支付的金额
        public double pay_amount { get; set; }
        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }
        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }
    }

    [Serializable]
    public struct AppImpressionAttributes : SEBaseAttributes
    {
        // 变现平台名称 不可为空:
        // csj：穿山甲国内版、pangle：穿山甲国际版、tencent：腾讯优量汇、baidu：百度百青藤、kuaishou：快手、oppo：OPPO、vivo：vivo
        // mi：小米、huawei：华为、applovin：Applovin、sigmob：Sigmob、mintegral：Mintegral、oneway：OneWay、vungle：Vungle
        // facebook：Facebook、admob：AdMob、unity：UnityAds、is：IronSource、adtiming：AdTiming、klein：游可赢
        public string ad_platform { get; set; }
        //填充广告的聚合平台，变现广告由聚合平台填充时必填，其他情况默认填custom即可，不可为空
        public string mediation_platform { get; set; }
        // 变现平台的应用 ID 不可为空
        public string ad_appid { get; set; }
        // 变现平台的变现广告位 ID 可为空
        public string ad_id { get; set; }
        // 展示广告的类型 不可为空 
        // 激励视频:1, 开屏:2, 插屏:3, 全屏:4, Banner:5, 信息流:6, 短视频信息流:7, 大横幅:8, 视频贴片:9, 其它:0
        public int ad_type { get; set; }
        // 广告ECPM（广告千次展现的变现收入，0或负值表示没传）不可为空
        public double ad_ecpm { get; set; }
        // 展示收益的货币种类，遵循《ISO 4217国际标准》，如 CNY、USD... 不可为空
        public string currency_type { get; set; }
        // 广告是否渲染成功 不可为空 ,bool类型，true为渲染成功，false为渲染失败
        public bool is_rendered { get; set; }
        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }
        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }
    }

    [Serializable]
    public struct AdClickAttributes : SEBaseAttributes
    {
        // 变现平台名称 不可为空:
        // csj：穿山甲国内版、pangle：穿山甲国际版、tencent：腾讯优量汇、baidu：百度百青藤、kuaishou：快手、oppo：OPPO、vivo：vivo
        // mi：小米、huawei：华为、applovin：Applovin、sigmob：Sigmob、mintegral：Mintegral、oneway：OneWay、vungle：Vungle
        // facebook：Facebook、admob：AdMob、unity：UnityAds、is：IronSource、adtiming：AdTiming、klein：游可赢
        public string ad_platform { get; set; }

        // 展示广告的类型 不可为空 
        // 1:激励视频、2：开屏、3：插屏、4：全屏、5：Banner、6、信息流、7、短视频信息流、8、大横幅、9、视频贴片、0：其它
        public int ad_type { get; set; }

        // 变现平台的变现广告位 ID 可为空
        public string ad_id { get; set; }

        //填充广告的聚合平台，变现广告由聚合平台填充时必填，其他情况默认填custom即可，不可为空
        public string mediation_platform { get; set; }
        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }
        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }

    }

    [Serializable]
    public struct RegisterAttributes : SEBaseAttributes
    {
        // 注册类型，如：QQ、WeiXin
        public string register_type { get; set; }
        // 注册状态
        public string register_status { get; set; }
        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }

        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }
        
    }

    [Serializable]
    public struct LoginAttributes : SEBaseAttributes
    {
        // 登录类型
        public string login_type { get; set; }
        // 登录状态
        public string login_status { get; set; }
        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }

        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }
    }

    [Serializable]
    public struct OrderAttributes : SEBaseAttributes
    {
        // 订单 ID 不超过 128  字符
        public string order_id { get; set; }
        // 订单金额，单位：元
        public double pay_amount { get; set; }
        // 货币类型。遵循《ISO 4217国际标准》，如 CNY、USD
        public string currency_type { get; set; }
        // 支付类型
        public string pay_type { get; set; }
        // 订单状态
        public string status { get; set; }
        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }

        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }
    }


    [Serializable]
    public struct AppAttributes : SEBaseAttributes
    {
        // 投放广告的渠道 ID，需要与发行平台匹配
        public string ad_network { get; set; }
        // 投放广告的子渠道
        public string sub_channel { get; set; }
        // 投放广告的投放账号 ID
        public string ad_account_id { get; set; }
        // 投放广告的投放账号名称
        public string ad_account_name { get; set; }
        // 投放广告的广告计划 ID
        public string ad_campaign_id { get; set; }
        // 投放广告的广告计划名称
        public string ad_campaign_name { get; set; }
        // 投放广告的广告单元 ID
        public string ad_offer_id { get; set; }
        // 投放广告的广告单元名称
        public string ad_offer_name { get; set; }
        // 投放广告的广告创意 ID
        public string ad_creative_id { get; set; }
        // 投放广告的广告创意名称
        public string ad_creative_name { get; set; }
        // 监测平台	
        public string attribution_platform { get; set; }
        

        // 自定义属性, json字符串
        public Dictionary<string, object> customProperties { get; set; }

        // 实现 SEBaseAttributes 接口中的 checkId 属性
        public string checkId { get; set; }

    }

    [Serializable]
    public enum SERCMergeType
    {
        // 默认策略，读取缓存配置+默认配置跟服务端配置合并
        SERCMergeTypeDefault = 0,

        // App版本更新时，使用默认配置+服务端合并（丢弃缓存配置）
        SERCMergeTypeUser = 1,
    }

    [Serializable]
    public struct  RCConfig
    {
        // 线参数SDK启用开关，默认为关闭状态，必传字段
        public bool enable { get; set; }

        // SDK配置合并策略，默认情况下服务端配置跟本地缓存配置合并
        // ENUM：SERCMergeTypeUser 在App版本更新时会清除缓存配置
        // 可选字段
        public SERCMergeType mergeType { get; set; }

        // 自定义ID, 用来匹配用户在后台设置规则时设置的自定义ID，可选字段
        public Dictionary<string, object> customIDProperties { get; set; }

        // 自定义ID 事件属性，可选字段
        public Dictionary<string, object> customIDEventProperties { get; set; }

        // 自定义ID 用户属性，可选字段
        public Dictionary<string, object> customIDUserProperties { get; set; }

        // 自定义ID 设备属性，可选字段
        // public Dictionary<string, object> customIDDeviceProperties { get; set; }
    }

    [Serializable]
    public struct SEConfig
    {
        //仅小游戏需要此字段
        public MiniGameInitParams miniGameInitParams;
        // 开启 Debug 模式，默认为关闭状态，可选字段, Debug模式请勿发布到线上！！！
        public bool isDebugModel { get; set; }

        // 是否开启 本地调试日志（不设置时默认不开启 本地日志）
        public bool logEnabled { get; set; }

        // 是否为GDPR区域。默认为false，可选字段（仅海外版设置有效）
        public bool isGDPRArea { get; set; }

        // 是否支持coppa合规。默认为false，可选字段（仅海外版设置有效）
        public bool isCoppaEnabled { get; set; }

        // 是否支持Kids App应用。默认为false，可选字段（仅海外版设置有效）
        public bool isKidsAppEnabled { get; set; }

        // 是否允许2G上报数据。默认为false，可选字段
        public bool isEnable2GReporting { get; set; }

        // 是否开启延迟deeplink。默认为false，可选字段
        public bool delayDeeplinkEnable { get; set; }

        // iOS ATT 授权等待时间，默认不等待，可选字段；只有iOS调用有效。
        public int attAuthorizationWaitingInterval { get; set; }

        // iOS caid；只有iOS调用有效。（仅国内版设置有效）
        public string caid { get; set; }

        // 设置获取归因结果回调，可选字段
        public Analytics.SEAttributionCallback attributionCallback { get; set; }

        // 设置初始化完成回调, 可选
        public Analytics.SESDKInitCompletedCallback initCompletedCallback { get; set; }

    }
    
    
    public class Distinct
    {
        public string distinct_id;
        public int distinct_id_type;
    }
    public class MiniGameInitParams
    {
        public string unionid;
        public string openid;
        public string anonymous_openid;
  
 
    }
}
