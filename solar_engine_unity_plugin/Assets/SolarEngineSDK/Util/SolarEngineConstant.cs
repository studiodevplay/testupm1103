using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SolarEngine
{
    public partial class Analytics : MonoBehaviour
    {
        public static readonly string sdk_version = "1.2.9.1";
        public static readonly string SolorEngine = $"[SolarEngineSDK_Unity {sdk_version} :]";
        private static readonly string SEConstant_CHECK_ID = "_first_event_check_id";
        private static readonly string SEConstant_EVENT_TYPE = "_event_type";


        private static readonly string SEConstant_CUSTOM_EVENT_NAME = "_custom_event_name";
        private static readonly string SEConstant_Pre_Properties = "_pre_properties";
        private static readonly string SEConstant_CUSTOM = "_custom_event";
        private static readonly string SEConstant_Custom_CustomProperties = "_customProperties";

        private static readonly string SEConstant_IAP = "_appPur";
        private static readonly string SEConstant_IAP_PName = "_product_name";
        private static readonly string SEConstant_IAP_PID = "_product_id";
        private static readonly string SEConstant_IAP_PCount = "_product_num";
        private static readonly string SEConstant_IAP_Currency = "_currency_type";
        private static readonly string SEConstant_IAP_OrderId = "_order_id";
        private static readonly string SEConstant_IAP_FailReason = "_fail_reason";
        private static readonly string SEConstant_IAP_PayType = "_pay_type";
        private static readonly string SEConstant_IAP_Amount = "_pay_amount";
        private static readonly string SEConstant_IAP_Paystatus = "_pay_status";
        private static readonly string SEConstant_IAP_CustomProperties = "_customProperties";

        private static readonly string SEConstant_IAI = "_appImp";
        private static readonly string SEConstant_IAI_AdPlatform = "_ad_platform";
        private static readonly string SEConstant_IAI_MediationPlatform = "_mediation_platform";
        private static readonly string SEConstant_IAI_AdAppid = "_ad_appid";
        private static readonly string SEConstant_IAI_AdId = "_ad_id";
        private static readonly string SEConstant_IAI_AdType = "_ad_type";
        private static readonly string SEConstant_IAI_AdEcpm = "_ad_ecpm";
        private static readonly string SEConstant_IAI_CurrencyType = "_currency_type";
        private static readonly string SEConstant_IAI_IsRendered = "_is_rendered";
        private static readonly string SEConstant_IAI_CustomProperties = "_customProperties";

        private static readonly string SEConstant_AdClick = "_appClick";
        private static readonly string SEConstant_AdClick_AdPlatform = "_ad_platform";
        private static readonly string SEConstant_AdClick_MediationPlatform = "_mediation_platform";
        private static readonly string SEConstant_AdClick_AdId = "_ad_id";
        private static readonly string SEConstant_AdClick_AdType = "_ad_type";
        private static readonly string SEConstant_AdClick_CustomProperties = "_customProperties";

        private static readonly string SEConstant_Register = "_appReg";
        private static readonly string SEConstant_Register_Type = "_reg_type";
        private static readonly string SEConstant_Register_Status = "_status";
        private static readonly string SEConstant_Register_CustomProperties = "_customProperties";

        private static readonly string SEConstant_Login = "_appLogin";
        private static readonly string SEConstant_Login_Type = "_login_type";
        private static readonly string SEConstant_Login_Status = "_status";
        private static readonly string SEConstant_Login_CustomProperties = "_customProperties";

        private static readonly string SEConstant_Order = "_appOrder";
        private static readonly string SEConstant_Order_ID = "_order_id";
        private static readonly string SEConstant_Order_Pay_Amount = "_pay_amount";
        private static readonly string SEConstant_Order_Currency_Type = "_currency_type";
        private static readonly string SEConstant_Order_Pay_Type = "_pay_type";
        private static readonly string SEConstant_Order_Status = "_status";
        private static readonly string SEConstant_Order_CustomProperties = "_customProperties";


        private static readonly string SEConstant_AppAttr = "_appAttr";
        private static readonly string SEConstant_AppAttr_Ad_Network = "_adnetwork";
        private static readonly string SEConstant_AppAttr_Sub_Channel = "_sub_channel";
        private static readonly string SEConstant_AppAttr_Ad_Account_ID = "_adaccount_id";
        private static readonly string SEConstant_AppAttr_Ad_Account_Name = "_adaccount_name";
        private static readonly string SEConstant_AppAttr_Ad_Campaign_ID = "_adcampaign_id";
        private static readonly string SEConstant_AppAttr_Ad_Campaign_Name = "_adcampaign_name";
        private static readonly string SEConstant_AppAttr_Ad_Offer_ID = "_adoffer_id";
        private static readonly string SEConstant_AppAttr_Ad_Offer_Name = "_adoffer_name";
        private static readonly string SEConstant_AppAttr_Ad_Creative_ID = "_adcreative_id";
        private static readonly string SEConstant_AppAttr_Ad_Creative_Name = "_adcreative_name";
        private static readonly string SEConstant_AppAttr_AttributionPlatform = "_attribution_platform";

        private static readonly string SEConstant_AppAttr_Ad_CustomProperties = "_customProperties";
        
        
        
    }
}
