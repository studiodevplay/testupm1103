
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

   // string uri = "https://baidu.link.solar-engine.com/se/deeplink.html?sedp_urlscheme=sedp_urlscheme_applik&sedp_link=sedp_link_applink&download=download_applink&turl_id=turlid_applink&a=6&b=8&se_from=links";

    
   
    public void InitSDK()
    {

        Debug.Log(SolarEngineDemoLOG+" init click1");
     
       String AppKey = "4e8884227c819e0e";
      
    
        MiniGameInitParams initParams = new MiniGameInitParams();
        initParams.anonymous_openid = "anonymous_openid";
        initParams.unionid = "unionid";
        initParams.openid = "openid";
        SEConfig seConfig = new SEConfig();
        seConfig.attAuthorizationWaitingInterval = 120;
        
        ///如接入腾讯广告SDK，请添加以下代码
        TencentAdvertisingGameSDKInitParams tencentAdvertisingGameSDKInitParams = new TencentAdvertisingGameSDKInitParams();
        
        tencentAdvertisingGameSDKInitParams.user_action_set_id =1234567;
        tencentAdvertisingGameSDKInitParams.secret_key = "";
        tencentAdvertisingGameSDKInitParams.appid = "";
        
        
        initParams.tencentAdvertisingGameSDKInitParams= tencentAdvertisingGameSDKInitParams;
        
        initParams.reportingToTencentSdk = 1;
        initParams.isInitTencentAdvertisingGameSDK = false;
        
        
         seConfig.miniGameInitParams = initParams;
         
        RCConfig rc = new RCConfig();
        
        rc.enable = true;
        
        rc.mergeType= RCMergeType.ByUser;
        rc.customIDProperties = new Dictionary<string, object>()
        {
            { "customIDProperties", "test" }, { "age", 18 }
        };
        rc.customIDEventProperties = new Dictionary<string, object>()
        {
            { "customIDEventProperties", "test" }, { "age", 18 }
        };
        rc.customIDUserProperties = new Dictionary<string, object>()
        {
            { "customIDUserProperties", "test" }, { "age", 18 }
        };
        seConfig.logEnabled = true;
        seConfig.isDebugModel = true;
        
     
        //delayDeeplinkEnable
        seConfig.delayDeeplinkEnable = true;       
         setDelayDeeplinkCompletionHandler();
        
        Analytics.deeplinkCompletionHandler(deeplinkCallback);
        Analytics.delayDeeplinkCompletionHandler(delayDeeplinkCallback);
        
      

     
        //如果是在2022版本上开发者可以通过以下方式获取deeplink url并传递给SDK
      
        
        Analytics.SEAttributionCallback callback = new Analytics.SEAttributionCallback(attributionCallback);
        seConfig.attributionCallback = callback;

        Analytics.SESDKInitCompletedCallback initCallback = initSuccessCallback;
        seConfig.initCompletedCallback = initCallback;
      
        SolarEngine.Analytics.preInitSeSdk(AppKey);
        SolarEngine.Analytics.initSeSdk(AppKey, seConfig,rc);
       // SolarEngine.Analytics.initSeSdk(AppKey);
        setRemoteDefaultConfig();
      //  handleSchemeUrl(uri);
      
      Application.deepLinkActivated += handleSchemeUrl;

      //其他Unity版本
      handleSchemeUrl1(Application.absoluteURL);

      initRemoteConfig();
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
    class test
    {
        public string user_data;
        public string account_id;
        

    }


    public void setRemoteDefaultConfig()
    {
        SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
        test t = new test();
        t.user_data = "test";
        t.account_id= "test2";
        
        //list
        List<object> list = new List<object>();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        
        Dictionary<string, object> defaultConfig6 = new Dictionary<string, object>();
        defaultConfig6.Add("name", "test");
        defaultConfig6.Add("age", 1);
        defaultConfig6.Add("t", t);
        defaultConfig6.Add("list", list);
        SESDKRemoteConfig.Item itemString = remoteConfig.stringItem("teststring", "test");
        SESDKRemoteConfig.Item itemJson = remoteConfig.jsonItem("testjson", defaultConfig6);
        SESDKRemoteConfig.Item itemBool = remoteConfig.boolItem("testbool", true);
        SESDKRemoteConfig.Item itemInt = remoteConfig.intItem("testint", 1);
        SESDKRemoteConfig.Item[] defaultConfigArray= new SESDKRemoteConfig.Item[]{itemString,itemJson,itemBool,itemInt};
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
        //接入腾讯广告SDK
        RegisterAttributes.reportingToTencentSdk = 1;
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

        SolarEngine.Analytics.track("trackCustom", customProperties, preProperties);

        
        
        
        
    


    }




    public void trackAppAtrr()
    {

        Debug.Log(SolarEngineDemoLOG+ " trackAppAtrr click");
        AttAttributes attributes = new AttAttributes();
        attributes.ad_network = "toutiao";
        attributes.sub_channel = "103300";
        attributes.ad_account_id = "1655958321988611";
        attributes.ad_account_name = "xxx科技全量18";
        attributes.ad_campaign_id = "1680711982033293";
        attributes.ad_campaign_name = "小鸭快冲计划157-1024";
        attributes.ad_offer_id = "1685219082855528";
        attributes.ad_offer_name = "小鸭快冲单元406-1024";
        attributes.ad_creative_id = "1680128668901378";
        attributes.ad_creative_name = "自动创建20210901178921";
        attributes.ad_creative_name = "自动创建20210901178921";
        attributes.attribution_platform = "se";
        attributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackAppAttr(attributes);

    }
    
    public void trackAppReEngagement()
    {

      Dictionary<string,object>  attributes = getCustomProperties();
#if UNITY_OPENHARMONY&&!UNITY_EDITOR

        SolarEngine.Analytics.trackAppReEngagement(attributes);
#endif

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
        productsAttributes.paystatus =PayStatus.Success;
        productsAttributes.pay_type = "wechat";
        productsAttributes.pay_amount = 9.9;
        productsAttributes.customProperties = getCustomProperties();
        //接入腾讯广告SDK
        productsAttributes.reportingToTencentSdk = 1;
        Debug.Log(SolarEngineDemoLOG + "trackPurchase"+JsonConvert.SerializeObject(productsAttributes));

        // SolarEngine.Analytics.track(productsAttributes);
    }

    public void trackAdImpression()
    {
        Debug.Log(SolarEngineDemoLOG+ " trackAdImpression click");

        ImpressionAttributes impressionAttributes = new ImpressionAttributes();
        impressionAttributes.ad_platform = "AdMob";
        //impressionAttributes.ad_appid = "ad_appid";
        impressionAttributes.mediation_platform = "gromore";
        impressionAttributes.ad_id = "product_id";
        impressionAttributes.ad_type = 1;
        impressionAttributes.ad_ecpm = 0.8;
        impressionAttributes.currency_type = "CNY";
        impressionAttributes.is_rendered = true;
        impressionAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackAdImpression(impressionAttributes);

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
        Debug.Log(SolarEngineDemoLOG + " userInit click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K111", "V1");
        userProperties.Add("K211", "V2");
        userProperties.Add("K311", 2);
        string[] arr = new string[] { "狐狸", "四叶草" };

        userProperties.Add("Kj1", arr);
        SolarEngine.Analytics.userInit(userProperties);


        RegisterAttributes RegisterAttributes = new RegisterAttributes();
        RegisterAttributes.register_type = "QQ_test";
        RegisterAttributes.register_status = "success_test";
        RegisterAttributes.checkId = "szw";
        RegisterAttributes.customProperties = getCustomProperties();
        
        SolarEngine.Analytics.trackFirstEvent(RegisterAttributes);

        
        CustomAttributes custom = new CustomAttributes();
        custom.custom_event_name = "custom_event_name_test";
        Dictionary<string, object> preProperties = new Dictionary<string, object>();
        preProperties.Add("_pay_amount", 0.55);
        preProperties.Add("_currency_type", "USD");

        custom.preProperties = preProperties;
        custom.customProperties = getCustomProperties();
        custom.checkId = "aaa";
   
        SolarEngine.Analytics.trackFirstEvent(custom);

    }

    public void userUpdate()
    {
        Debug.Log(SolarEngineDemoLOG+ " userUpdate click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K111", "V1");
        userProperties.Add("K211", "V2");
        userProperties.Add("K311", 2);
        SolarEngine.Analytics.userUpdate(userProperties);

    }

    public void userAdd()
    {
        Debug.Log(SolarEngineDemoLOG+ " userAdd click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K111", 10);
        userProperties.Add("K211", 100);
        userProperties.Add("K311", 2);
        SolarEngine.Analytics.userAdd(userProperties);

    }

    public void userUnset()
    {
        Debug.Log(SolarEngineDemoLOG+ " userUnset click");

        SolarEngine.Analytics.userUnset(new string[] { "K11", "K21" });

    }

    public void userAppend()
    {
        Debug.Log(SolarEngineDemoLOG+ " userAppend click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K111", "V1");
        userProperties.Add("K211", "V2");
        userProperties.Add("K311", 2);
        SolarEngine.Analytics.userAppend(userProperties);



    }

    public void userDelete()
    {
        Debug.Log(SolarEngineDemoLOG+ " SEUserDeleteTypeByAccountId click");

        SolarEngine.Analytics.userDelete(UserDeleteType.ByAccountId);
        SolarEngine.Analytics.userDelete(UserDeleteType.ByVisitorId);
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
            // foreach (var VARIABLE in attribution)
            // {
                Debug.Log(SolarEngineDemoLOG+ " attribution : " + JsonConvert.SerializeObject(attribution));
            //}
        }
        else
        {
            // 归因失败
        }
    }

    private Dictionary<string, object> getCustomProperties()
    {

        Dictionary<string, object> properties = new Dictionary<string, object>();
        // properties.Add("K1", "V1");
        // properties.Add("K2", "V2");
        properties.Add("CustomProperties", "test");

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
    CreateButton("setOaId", SetOaIdHandler);
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
    CreateButton("Track AppRe", trackAppReEngagement);
    
    #if SOLARENGINE_WECHAT
    CreateButton("trackReActive", trackReActive);
    CreateButton("trackShare", trackShare);
    CreateButton("trackTutorialFinish", trackTutorialFinish);
    CreateButton("trackAddToWishlist", trackAddToWishlist);
    CreateButton("trackViewContentMall", trackViewContentMall);
    CreateButton("trackUpdateLevel", trackUpdateLevel);
    CreateButton("trackCreateRole", trackCreateRole);
    CreateButton("trackViewContentActivity", trackViewContentActivity);
    
    #endif
    
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
    Analytics.setPresetEvent(PresetEventType.All, propertiess);
    Analytics.setPresetEvent(PresetEventType.Start, null);
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
#if UNITY_OPENHARMONY&&!UNITY_EDITOR&&!SE_DIS_RC

Analytics.getVisitorId(getVisitorId);
 #else
    Debug.Log(SolarEngineDemoLOG+"getVisitorId : " +  Analytics.getVisitorId());
#endif
}

void getVisitorId(string visitorId)
{
    Debug.Log(SolarEngineDemoLOG+"getVisitorId : " + visitorId);
    
}
private void GetDistinctIdHandler()
{
#if UNITY_OPENHARMONY&&!UNITY_EDITOR

    Analytics.getDistinctId(_distinct);
    #else
  
    Debug.Log(SolarEngineDemoLOG+"getDistinctId : " +   Analytics.getDistinctId());

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

private void SetOaIdHandler()
{
    Analytics.setOaid("testOAID");
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
#if UNITY_OPENHARMONY&&!UNITY_EDITOR

   Analytics.getPresetProperties(GetPresetPropertie);
    #else
    Dictionary<string, object> dic = Analytics.getPresetProperties();
    if (dic != null)
    {
        Debug.Log(SolarEngineDemoLOG+JsonConvert.SerializeObject(dic));
    }

#endif
 
}
private void GetPresetPropertie(Dictionary<string, object> dic)
{
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
        // string str = remoteConfig.FastFetchRemoteConfig("test");
        // Debug.Log(SolarEngineDemoLOG+str);
        
#if UNITY_OPENHARMONY&&!UNITY_EDITOR&&!SE_DIS_RC

    
            remoteConfig.FastFetchRemoteConfig("testint", onFetchRemoteConfigCallbacks);
            remoteConfig.FastFetchRemoteConfig("testjson", onFetchRemoteConfigCallbacks1);
        
            remoteConfig.FastFetchRemoteConfig("testbool", onFetchRemoteConfigCallbacks1);
            remoteConfig.FastFetchRemoteConfig("teststring", onFetchRemoteConfigCallbacks1);
        #else
        Debug.Log(SolarEngineDemoLOG+"testint : " + remoteConfig.FastFetchRemoteConfig("testint"));
        Debug.Log(SolarEngineDemoLOG+"testjson : " + remoteConfig.FastFetchRemoteConfig("testjson"));
        Debug.Log(SolarEngineDemoLOG+"testbool : " + remoteConfig.FastFetchRemoteConfig("testbool"));
        Debug.Log(SolarEngineDemoLOG+"teststring : " + remoteConfig.FastFetchRemoteConfig("teststring"));

      #endif  
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
        // Dictionary<string, object> str = remoteConfig.FastFetchRemoteConfig();
        // foreach (var VARIABLE in str)
        // {
        //     Debug.Log(SolarEngineDemoLOG+VARIABLE.Key + " " + VARIABLE.Value);
        // }
#if UNITY_OPENHARMONY&&!UNITY_EDITOR&&!SE_DIS_RC

        remoteConfig.FastAllFetchRemoteConfig(onFetchRemoteConfigCallback);
        #else
        Dictionary<string, object> str = remoteConfig.FastFetchRemoteConfig();
        foreach (var VARIABLE in str)
        {
            Debug.Log(SolarEngineDemoLOG+VARIABLE.Key + " " + VARIABLE.Value);
        }
        
#endif
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
        remoteConfig.AsyncFetchRemoteConfig("testjson", onFetchRemoteConfigCallbacks1);
        
        remoteConfig.AsyncFetchRemoteConfig("testbool", onFetchRemoteConfigCallbacks1);
        remoteConfig.AsyncFetchRemoteConfig("teststring", onFetchRemoteConfigCallbacks1);

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


#region 腾讯回传

public void trackReActive()
{
    ReActiveAttributes registerData = new ReActiveAttributes
    {
        reportingToTencentSdk=2,
        backFlowDay=5,
        customProperties = new Dictionary<string, object> { { "one", "1" } }
    };
    SolarEngine.Analytics.trackReActive(registerData);
}

public void trackAddToWishlist()
{
    AddToWishlistAttributes addToWishlistData = new AddToWishlistAttributes
    {
        reportingToTencentSdk=1,
        addToWishlistType = SolarEngine.Analytics.WishlistType_MY,
        customProperties = new Dictionary<string, object> { { "one", "1" } }
    };
    SolarEngine.Analytics.trackAddToWishlist(addToWishlistData);
}

public void trackShare()
{
    ShareAttributes shareData = new ShareAttributes
    {
        reportingToTencentSdk=1,
        mpShareTarget =SolarEngine.Analytics.ShareTarget_APP_MESSAGE,
        customProperties = new Dictionary<string, object> { { "one", "1" } }
    };
    SolarEngine.Analytics.trackShare(shareData);
}

public void trackCreateRole()
{
    CreateRoleAttributes createRoleData = new CreateRoleAttributes
    {
        reportingToTencentSdk=1,
        mpRoleName = "role_name",
    };
    SolarEngine.Analytics.trackCreateRole(createRoleData);
}

public void trackViewContentActivity()
{
    ViewContentActivitAttributes viewContentActivityData = new ViewContentActivitAttributes
    {
        reportingToTencentSdk=1,
        customProperties = new Dictionary<string, object> { { "one", "1" } }
    };
    SolarEngine.Analytics.trackViewContentActivity(viewContentActivityData);
}

public void trackTutorialFinish()
{
    TutorialFinishAttributes tutorialFinishData = new TutorialFinishAttributes
    {
        reportingToTencentSdk=1,
    };
    SolarEngine.Analytics.trackTutorialFinish(tutorialFinishData);
}


public void trackUpdateLevel()
{
    UpdateLevelAttributes updateLevelData = new UpdateLevelAttributes
    {
        reportingToTencentSdk=1,
        beforeUpgrade = 10,
        afterUpgrade = 20,
        customProperties = new Dictionary<string, object> { { "one", "1" } }
    };
    SolarEngine.Analytics.trackUpdateLevel(updateLevelData);
}


public void trackViewContentMall()
{
    ViewContentMallAttributes viewContentMallData = new ViewContentMallAttributes
    {
        reportingToTencentSdk=1,
        customProperties = new Dictionary<string, object> { { "one", "1" } }
    };
    SolarEngine.Analytics.trackViewContentMall(viewContentMallData);
}





#endregion


#region CallBack

private void handleSchemeUrl(string url)
{
    //将url传递给SDK，注意此步骤一定要在SDK初始化成功之后执行，否则不生效，可参考下放添加deeplink回调示例使用方法
    Debug.Log($"{SolarEngineDemoLOG}   handleSchemeUrl    "+url);
    SolarEngine.Analytics.handleDeepLinkUrl(url);
}

private void handleSchemeUrl1(string url)
{
    //将url传递给SDK，注意此步骤一定要在SDK初始化成功之后执行，否则不生效，可参考下放添加deeplink回调示例使用方法
    Debug.Log($"{SolarEngineDemoLOG}   handleSchemeUrl1    "+url);
    SolarEngine.Analytics.handleDeepLinkUrl(url);
}

private void onFetchRemoteConfigCallbacks(string result)
{
    // 异步获取参数下发的回调为 string 类型，需要开发者根据自己的业务配置进行属性转换，此处以 bool 类型为例
    Debug.Log(SolarEngineDemoLOG+result);
}

private void onFetchRemoteConfigCallbacks1(string result)
{
    // 异步获取参数下发的回调为 string 类型，需要开发者根据自己的业务配置进行属性转换，此处以 bool 类型为例
    Debug.Log(SolarEngineDemoLOG+"1   "+result);
}

private void onFetchRemoteConfigCallback(Dictionary<string, object> result)
{
    // 异步获取参数下发的回调为 string 类型，需要开发者根据自己的业务配置进行属性转换，此处以 bool 类型为例
    string str = JsonConvert.SerializeObject(result);
    Debug.Log(SolarEngineDemoLOG+str);
}


private void _distinct(string distinct)
{
    Debug.Log(string.Format(SolarEngineDemoLOG+"distinct_id: {0} \n ", distinct));
}


private void deeplinkCallback(int code, Dictionary<string, object> data)
{
  
    Debug.Log(SolarEngineDemoLOG + code + " " + JsonConvert.SerializeObject(data) );

}
#endregion


    }
  

    
    


    

