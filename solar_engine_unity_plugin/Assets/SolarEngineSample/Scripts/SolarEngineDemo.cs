
using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using SolarEngine;
using SolarEngine.MiniGames.info;
using UnityEngine.UI;
using Distinct = SolarEngine.Distinct;


public class SolarEngineDemo : MonoBehaviour
{
    public Texture2D texture;

    public static readonly  string  SolarEngineDemoLOG="SolarEngineDemo: ";

    private void Start()
    {
        InitSDK();
        Analytics.deeplinkCompletionHandler(deeplinkCallback);
        //
        ///Application.deepLinkActivated += handleSchemeUrl1;
        //如果是在2022版本上开发者可以通过以下方式获取deeplink url并传递给SDK
        handleSchemeUrl(Application.absoluteURL);
        
       
    }
    
    
   
    public void InitSDK()
    {

        Debug.Log(SolarEngineDemoLOG+" init click");
        //安卓
       String AppKey = "8b17c04df5df162a";
        //iOS
       // String AppKey = "455cd0c9843e503e";
       
    
        MiniGameInitParams initParams = new MiniGameInitParams();
        initParams.anonymous_openid = "anonymous_openid";
        initParams.unionid = "unionid";
        initParams.openid = "openid";
        SEConfig seConfig = new SEConfig();
        seConfig.attAuthorizationWaitingInterval = 120;
         seConfig.miniGameInitParams = initParams;
         
        RCConfig rc = new RCConfig();
        
        rc.enable = false;
        seConfig.logEnabled = true;
        seConfig.isDebugModel = false;
     
        //delayDeeplinkEnable
        // seConfig.delayDeeplinkEnable = true;       
        // setDelayDeeplinkCompletionHandler();
        
     
        
        Analytics.SEAttributionCallback callback = new Analytics.SEAttributionCallback(attributionCallback);
        seConfig.attributionCallback = callback;

        Analytics.SESDKInitCompletedCallback initCallback = initSuccessCallback;
        seConfig.initCompletedCallback = initCallback;
      
        SolarEngine.Analytics.preInitSeSdk(AppKey);
        SolarEngine.Analytics.initSeSdk(AppKey, seConfig);
        
      
        
    }

    private void setDelayDeeplinkCompletionHandler()
    {
        SolarEngine.Analytics.delayDeeplinkCompletionHandler(delayDeeplinkCallback);  //必须在初始化之前开启，否则无法监听到deeplink回调
    }

    private void delayDeeplinkCallback(int code, Dictionary<string, object> data)
    {
     Debug.Log(SolarEngineDemoLOG+code +JsonConvert.SerializeObject(data));
    }


    public void initRemoteConfig()
    {
       SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
        Dictionary<string, object> eventProperties = new Dictionary<string, object>
        {
            { "event", "value1" },
            { "k2", 2 }
        };

        remoteConfig.SetRemoteConfigEventProperties(eventProperties);
        Dictionary<string, object> propertiess = new Dictionary<string, object>
        {
            { "user", "value1" },
            { "k2", 2 }
        };
        remoteConfig.SetRemoteConfigUserProperties(propertiess);


    }



    public void setRemoteDefaultConfig()
    {
        SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
        Dictionary<string, object> defaultConfig1 = remoteConfig.stringItem("qq", "test");
        Dictionary<string, object> defaultConfig2 = remoteConfig.jsonItem("testjson", "{\"test\":\"test\"}");
        Dictionary<string, object> defaultConfig3 = remoteConfig.boolItem("testbool", true);
        Dictionary<string, object> defaultConfig4 = remoteConfig.intItem("testint", 1);
        
        
        Dictionary<string, object>[] defaultConfigArray = new Dictionary<string, object>[]
            { defaultConfig1, defaultConfig2, defaultConfig3, defaultConfig4 };
        
        remoteConfig.SetRemoteDefaultConfig(defaultConfigArray);

    }

 
   



    public void trackAdClick()
    {
        Debug.Log(SolarEngineDemoLOG+ "trackAdClick click");

        AdClickAttributes AdClickAttributes = new AdClickAttributes();
        AdClickAttributes.ad_platform = "izz";
        AdClickAttributes.mediation_platform = "gromore_test";
        AdClickAttributes.ad_id = "product_id_test";
        AdClickAttributes.ad_type = 1;
        AdClickAttributes.checkId = "123";
        AdClickAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackAdClick(AdClickAttributes);

    }

    public void trackRegister()
    {
        Debug.Log(SolarEngineDemoLOG+ " trackRegister click");

        RegisterAttributes RegisterAttributes = new RegisterAttributes();
        RegisterAttributes.register_type = "QQ_test";
        RegisterAttributes.register_status = "success_test";
        RegisterAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackRegister(RegisterAttributes);



    }

    public void track()
    {

        Debug.Log(SolarEngineDemoLOG+ " track custom click");

        Dictionary<string, object> customProperties = new Dictionary<string, object>();
        customProperties.Add("event001", 111);
        customProperties.Add("event002", "event002");
        customProperties.Add("event003", 1);

        Dictionary<string, object> preProperties = new Dictionary<string, object>();
        preProperties.Add("_pay_amount", 0.55);
        preProperties.Add("_currency_type", "USD");

        SolarEngine.Analytics.trackCustom("trackCustom", customProperties, preProperties);



    }




    public void trackAppAtrr()
    {

        Debug.Log(SolarEngineDemoLOG+ " trackAppAtrr click");
        AppAttributes AppAttributes = new AppAttributes();
        AppAttributes.ad_network = "toutiao";
        AppAttributes.sub_channel = "103300";
        AppAttributes.ad_account_id = "1655958321988611";
        AppAttributes.ad_account_name = "xxx科技全量18";
        AppAttributes.ad_campaign_id = "1680711982033293";
        AppAttributes.ad_campaign_name = "小鸭快冲计划157-1024";
        AppAttributes.ad_offer_id = "1685219082855528";
        AppAttributes.ad_offer_name = "小鸭快冲单元406-1024";
        AppAttributes.ad_creative_id = "1680128668901378";
        AppAttributes.ad_creative_name = "自动创建20210901178921";
        AppAttributes.ad_creative_name = "自动创建20210901178921";
        AppAttributes.attribution_platform = "se";
        AppAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackAppAttr(AppAttributes);

    }

    public void trackIAP()
    {
        Debug.Log(SolarEngineDemoLOG+ " trackIAP click");

        ProductsAttributes productsAttributes = new ProductsAttributes();
        productsAttributes.product_name = "product_name";
        productsAttributes.product_id = "product_id";
        productsAttributes.product_num = 8;
        productsAttributes.currency_type = "CNY";
        productsAttributes.order_id = "order_id";
        productsAttributes.fail_reason = "fail_reason";
        productsAttributes.paystatus = SEConstant_IAP_PayStatus.SEConstant_IAP_PayStatus_success;
        productsAttributes.pay_type = "wechat";
        productsAttributes.pay_amount = 9.9;
        productsAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackIAP(productsAttributes);
    }

    public void trackAdImpression()
    {
        Debug.Log(SolarEngineDemoLOG+ " trackAdImpression click");

        AppImpressionAttributes impressionAttributes = new AppImpressionAttributes();
        impressionAttributes.ad_platform = "AdMob";
        //impressionAttributes.ad_appid = "ad_appid";
        impressionAttributes.mediation_platform = "gromore";
        impressionAttributes.ad_id = "product_id";
        impressionAttributes.ad_type = 1;
        impressionAttributes.ad_ecpm = 0.8;
        impressionAttributes.currency_type = "CNY";
        impressionAttributes.is_rendered = true;
        impressionAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackIAI(impressionAttributes);

    }


    public void trackLogin()
    {
        Debug.Log(SolarEngineDemoLOG+ " trackLogin click");

        LoginAttributes LoginAttributes = new LoginAttributes();
        LoginAttributes.login_type = "QQ_test";
        LoginAttributes.login_status = "success1_test";
        LoginAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackLogin(LoginAttributes);

    }

    public void trackOrder()
    {
        Debug.Log(SolarEngineDemoLOG+ " trackOrderclick");

        OrderAttributes OrderAttributes = new OrderAttributes();
        OrderAttributes.order_id = "order_id_test";
        OrderAttributes.pay_amount = 10.5;
        OrderAttributes.currency_type = "CNY";
        OrderAttributes.pay_type = "AIP";
        OrderAttributes.status = "success";
        OrderAttributes.customProperties = getCustomProperties();

        SolarEngine.Analytics.trackOrder(OrderAttributes);

    }


    public void userInit()
    {
        Debug.Log(SolarEngineDemoLOG+ " userInit click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K1", "V1");
        userProperties.Add("K2", "V2");
        userProperties.Add("K3", 2);
        string[] arr = new string[] { "狐狸", "四叶草" };

        userProperties.Add("Kj", arr);
        SolarEngine.Analytics.userInit(userProperties);

    }

    public void userUpdate()
    {
        Debug.Log(SolarEngineDemoLOG+ " userUpdate click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K1", "V1");
        userProperties.Add("K2", "V2");
        userProperties.Add("K3", 2);
        SolarEngine.Analytics.userUpdate(userProperties);

    }

    public void userAdd()
    {
        Debug.Log(SolarEngineDemoLOG+ " userAdd click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K1", 10);
        userProperties.Add("K2", 100);
        userProperties.Add("K3", 2);
        SolarEngine.Analytics.userAdd(userProperties);

    }

    public void userUnset()
    {
        Debug.Log(SolarEngineDemoLOG+ " userUnset click");

        SolarEngine.Analytics.userUnset(new string[] { "K1", "K2" });

    }

    public void userAppend()
    {
        Debug.Log(SolarEngineDemoLOG+ " userAppend click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K1", "V1");
        userProperties.Add("K2", "V2");
        userProperties.Add("K3", 2);
        SolarEngine.Analytics.userAppend(userProperties);



    }

    public void userDelete()
    {
        Debug.Log(SolarEngineDemoLOG+ " SEUserDeleteTypeByAccountId click");

        SolarEngine.Analytics.userDelete(SEUserDeleteType.SEUserDeleteTypeByAccountId);
        SolarEngine.Analytics.userDelete(SEUserDeleteType.SEUserDeleteTypeByVisitorId);
    }



    private void initSuccessCallback(int code)
    {
        Debug.Log(SolarEngineDemoLOG+ "initSuccessCallback  code : " + code);

    }


    private void attributionCallback(int code, Dictionary<string, object> attribution)
    {
        Debug.Log(SolarEngineDemoLOG+ " errorCode : " + code);

        if (code == 0)
        {
            foreach (var VARIABLE in attribution)
            {
                Debug.Log(SolarEngineDemoLOG+ " attribution : " + VARIABLE);
            }
        }
        else
        {
            // 归因失败
        }
    }

    private Dictionary<string, object> getCustomProperties()
    {

        Dictionary<string, object> properties = new Dictionary<string, object>();
        properties.Add("K1", "V1");
        properties.Add("K2", "V2");
        properties.Add("K3", 2);

        return properties;
    }


    private const int Margin = 50;



    private Vector2 scrollPosition;

 void OnGUI()
{
    GUILayout.BeginArea(new Rect(Margin, 200, Screen.width - 2 * Margin, Screen.height));
    scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width),
        GUILayout.Height(Screen.height));

    GUIStyle buttonStyle = GUI.skin.button;
    buttonStyle.fontSize = 25;
    buttonStyle.fontStyle = FontStyle.Bold;
    buttonStyle.normal.background = texture;
    buttonStyle.normal.textColor = Color.black;

    GUILayout.BeginHorizontal();
    // 左列按钮
    GUILayout.BeginVertical();
    CreateButton("InitSDK", InitSDK);
    
    CreateButton("SetSuperProperties", SetSuperPropertiesHandler);
    CreateButton("SetPresetEvent", SetPresetEventHandler);
    CreateButton("Login", LoginHandler);
    CreateButton("GetAccount", GetAccountIdHandler);
    CreateButton("Logout", LogoutHandler);
    CreateButton("SetVisitor", SetVisitorIdHandler);
    CreateButton("GetVisitor", GetVisitorIdHandler);
    CreateButton("GetDistinct", GetDistinctIdHandler);
    CreateButton("EventStart", EventStartHandler);
    CreateButton("EventFinish", EventFinishHandler);
    CreateButton("SetChannel", SetChannelHandler);
    CreateButton("Unset Super Property", UnsetSuperPropertyHandler);
    CreateButton("Clear Super Properties", ClearSuperPropertiesHandler);
    CreateButton("Report Immediately", ReportEventImmediatelyHandler);
    CreateButton("GetAttribution", GetAttributionHandler);
    CreateButton("SetChannel", SetChannelAgainHandler);
    CreateButton("SetGaids", SetGaidHandler);
    CreateButton("SetGDPRArea", SetGDPRAreaHandle);

    CreateButton("SetReferrerTitle", SetReferrerTitleHandler);
    CreateButton("SetXcxPageTitle", SetXcxPageTitleHandler);
    CreateButton("GetPresetProperties", GetPresetPropertiesHandler);
    GUILayout.EndVertical();

    GUILayout.BeginVertical();
    CreateButton("Track Ad Click", TrackAdClickHandler);
    CreateButton("Track Register", TrackRegisterHandler);
    CreateButton("Track Custom", TrackCustomHandler);
    CreateButton("Track App Attributes", TrackAppAttrHandler);
    CreateButton("Track IAP", TrackIAPHandler);
    CreateButton("Track Ad Impression", TrackAdImpressionHandler);
    CreateButton("Track Login", TrackLoginHandler);
    CreateButton("Track Order", TrackOrderHandler);
    CreateButton("User Init", UserInitHandler);
    CreateButton("User Update", UserUpdateHandler);
    CreateButton("User Add", UserAddHandler);
    CreateButton("User Unset", UserUnsetHandler);
    CreateButton("User Append", UserAppendHandler);
    CreateButton("User Delete", UserDeleteHandler);
    CreateButton("Fast Fetch (Single)", FastFetchSingleHandler);
    CreateButton("Fast Fetch (All)", FastFetchAllHandler);
    CreateButton("Async Fetch (Single)", AsyncFetchSingleHandler);
    CreateButton("Async Fetch (All)", AsyncFetchAllHandler);
    
    
    CreateButton("iOS CallBack", iOSCallBackHandler);
    GUILayout.EndVertical();
    GUILayout.EndHorizontal();

    GUILayout.EndScrollView();
    GUILayout.EndArea();
}

private void CreateButton(string buttonText, Action onClickHandler)
{
    if (GUILayout.Button(buttonText, GUILayout.Height(70), GUILayout.Width(300)))
    {
        onClickHandler();
    }
    GUILayout.Space(5);
}

private void iOSCallBackHandler()
{
    SolarEngine.Analytics.updatePostbackConversionValue(1, mySkanCallback);
    Analytics.requestTrackingAuthorizationWithCompletionHandler( OnRequestTrackingAuthorizationCompletedHandler);
    Analytics.updateConversionValueCoarseValue(2,"value", UpdateConversionValueCoarseValue);
    Analytics.updateConversionValueCoarseValueLockWindow( 3,"value" ,true, UpdateConversionValueCoarseValueLockWindow);
    SolarEngine.Analytics.deeplinkCompletionHandler(deeplinkCallback);

}
private void mySkanCallback(int code ,string result)
{
    Debug.Log(SolarEngineDemoLOG+" mySkanCallback : " + result);
}
private void OnRequestTrackingAuthorizationCompletedHandler(int code)
    {
        Debug.Log(SolarEngineDemoLOG+ " OnRequestTrackingAuthorizationCompletedHandler : " + code);
        InitSDK();
    }
private void UpdateConversionValueCoarseValue(int errorCode, String errorMsg)
    {
        Debug.Log(SolarEngineDemoLOG+ " UpdateConversionValueCoarseValue : " + errorCode + " " + errorMsg);
       
    }
private void UpdateConversionValueCoarseValueLockWindow(int errorCode, String errorMsg)
{
    Debug.Log(SolarEngineDemoLOG+ " UpdateConversionValueCoarseValueLockWindow : " + errorCode + " " + errorMsg);
}

private void SetSuperPropertiesHandler()
{
    Dictionary<string, object> properties = new Dictionary<string, object>
    {
        { "Preset1", "这是 Super Properties" },
        { "propertySuper2", 2 }
    };
    Analytics.setSuperProperties(properties);

    Dictionary<string, object> propertiess = new Dictionary<string, object>
    {
        { "Preset1", "test" },
        { "propertySuper2", 999 }
    };
    Analytics.setSuperProperties(propertiess);
}

private void SetPresetEventHandler()
{
    Dictionary<string, object> propertiess = new Dictionary<string, object>
    {
        { "Preset1", "这是 Preset" },
        { "Preset2", 9.99 }
    };
    Analytics.setPresetEvent(SEConstant_Preset_EventType.SEConstant_Preset_EventType_All, propertiess);
    Analytics.setPresetEvent(SEConstant_Preset_EventType.SEConstant_Preset_EventType_AppStart, null);
}

private void LoginHandler()
{
    Analytics.login("12334555");
}

private void LogoutHandler()
{
    Analytics.logout();
}


private void GetAccountIdHandler()
{
    Debug.Log(SolarEngineDemoLOG+Analytics.getAccountId());
}

private void SetVisitorIdHandler()
{
    Analytics.setVisitorId("99999999999");
}

private void GetVisitorIdHandler()
{
    Debug.Log(SolarEngineDemoLOG+Analytics.getVisitorId());
}
private void GetDistinctIdHandler()
{
#if SOLARENGINE_BYTEDANCE || SOLARENGINE_WECHAT||SOLARENGINE_KUAISHOU
    Analytics.getDistinctId(_distinct);
#else
   Debug.Log( SolarEngineDemoLOG+ Analytics.getDistinctId());
#endif
}

private void EventStartHandler()
{
    Analytics.eventStart("testEvent");
}

private void EventFinishHandler()
{
    Analytics.eventFinish("testEvent", getCustomProperties());
}

private void SetChannelHandler()
{
    Analytics.setChannel("google");
}

private void UnsetSuperPropertyHandler()
{
    Analytics.unsetSuperProperty("Preset1");
}

private void ClearSuperPropertiesHandler()
{
    Analytics.clearSuperProperties();
}

private void ReportEventImmediatelyHandler()
{
    Analytics.reportEventImmediately();
}

private void GetAttributionHandler()
{
    Dictionary<string, object> dic = Analytics.getAttribution();
    if (dic != null)
    {
        Debug.Log(SolarEngineDemoLOG+JsonConvert.SerializeObject(dic));
    }

}

private void SetChannelAgainHandler()
{
    Analytics.setChannel("google");
}

private void SetGaidHandler()
{
    Analytics.setGaid("testgaid");
}

private void SetGDPRAreaHandle()
{
    Analytics.setGDPRArea(false);
}
private void SetReferrerTitleHandler()
{
    Analytics.setReferrerTitle("setReferrerTitle");

}

private void SetXcxPageTitleHandler()
{
    Analytics.setXcxPageTitle("setXcxPageTitle");

}

private void GetPresetPropertiesHandler()
{
    Dictionary<string, object> dic = Analytics.getPresetProperties();
    string str = JsonConvert.SerializeObject(dic);
    Debug.Log(SolarEngineDemoLOG+str);
}

private void TrackAdClickHandler()
{
    trackAdClick();
}

private void TrackRegisterHandler()
{
    trackRegister();
}

private void TrackCustomHandler()
{
    track();
}

private void TrackAppAttrHandler()
{
    trackAppAtrr();
}

private void TrackIAPHandler()
{
    trackIAP();
}

private void TrackAdImpressionHandler()
{
    trackAdImpression();
}

private void TrackLoginHandler()
{
    trackLogin();
}

private void TrackOrderHandler()
{
    trackOrder();
}

private void UserInitHandler()
{
    userInit();
}

private void UserUpdateHandler()
{
    userUpdate();
}

private void UserAddHandler()
{
    userAdd();
}

private void UserUnsetHandler()
{
    userUnset();
}

private void UserAppendHandler()
{
    userAppend();
}

private void UserDeleteHandler()
{
    userDelete();
}

private void FastFetchSingleHandler()
{
    SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
    try
    {
        string str = remoteConfig.FastFetchRemoteConfig("qq");
        Debug.Log(SolarEngineDemoLOG+str);
    }
    catch (Exception e)
    {
        Debug.LogError(SolarEngineDemoLOG+"Error in FastFetchSingleHandler: " + e.Message);
    }
}

private void FastFetchAllHandler()
{
    try
    {
        SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
        Dictionary<string, object> str = remoteConfig.FastFetchRemoteConfig();
        foreach (var VARIABLE in str)
        {
            Debug.Log(SolarEngineDemoLOG+VARIABLE.Key + " " + VARIABLE.Value);
        }
    }
    catch (Exception e)
    {
        Debug.LogError(SolarEngineDemoLOG+"Error in FastFetchAllHandler: " + e.Message);
    }
}

private void AsyncFetchSingleHandler()
{
    SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
    try
    {
        remoteConfig.AsyncFetchRemoteConfig("testint", onFetchRemoteConfigCallbacks);
    }
    catch (Exception e)
    {
        Debug.LogError("Error in AsyncFetchSingleHandler: " + e.Message);
    }
}

private void AsyncFetchAllHandler()
{
    SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
    try
    {
        remoteConfig.AsyncFetchRemoteConfig(onFetchRemoteConfigCallback);
    }
    catch (Exception e)
    {
        Debug.LogError("Error in AsyncFetchAllHandler: " + e.Message);
    }
}




#region CallBack

private void handleSchemeUrl(string url)
{
    //将url传递给SDK，注意此步骤一定要在SDK初始化成功之后执行，否则不生效，可参考下放添加deeplink回调示例使用方法
    SolarEngine.Analytics.handleDeepLinkUrl(url);
}

private void onFetchRemoteConfigCallbacks(string result)
{
    // 异步获取参数下发的回调为 string 类型，需要开发者根据自己的业务配置进行属性转换，此处以 bool 类型为例
    Debug.Log(SolarEngineDemoLOG+result);
}

private void onFetchRemoteConfigCallback(Dictionary<string, object> result)
{
    // 异步获取参数下发的回调为 string 类型，需要开发者根据自己的业务配置进行属性转换，此处以 bool 类型为例
    string str = JsonConvert.SerializeObject(result);
    Debug.Log(SolarEngineDemoLOG+str);
}


private void _distinct(Distinct distinct)
{
    Debug.Log(string.Format(SolarEngineDemoLOG+"distinct_id: {0} \n distinct_id_type: {1}", distinct.distinct_id, distinct.distinct_id_type));
}


private void deeplinkCallback(int code, Dictionary<string, object> data)
{
  
    Debug.Log(SolarEngineDemoLOG + code + " " + JsonConvert.SerializeObject(data) );

}
#endregion


    }
  

    
    


    

