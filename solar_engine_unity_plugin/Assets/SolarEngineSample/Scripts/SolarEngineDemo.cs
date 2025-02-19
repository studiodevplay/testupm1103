
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
// using CloudGame;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolarEngine;
using SolarEngine.MiniGames.info;

// using StarkSDKSpace;
using UnityEngine.UI;
using Distinct = SolarEngine.Distinct;

public class SolarEngineDemo : MonoBehaviour
{
    public Texture2D texture;

    private string SolarEngineDemoLog = "SolarEngineDemo";

    public void InitSDKWithDebug()
    {
        // Debug.Log( SolarEngineDemoLog+"init"+StarkUtils.IsCloudRuntime()); 
        //
        // //DebugTool.LogDebug( SolarEngineDemoLog+"init",StarkUtils.IsCloudRuntime().ToString()); 
        //
        //
        // Debug.Log( SolarEngineDemoLog+"init"+StarkSDK.IsCloudInitOver); 
        
        // string filePath = Application.persistentDataPath + "/SolarEngineData.json";
        // Debug.Log("filePath"+ filePath);
       // Debug.Log( SolarEngineDemoLog+"init"+StarkUtils.IsCloudRuntime()); 
       //       
       //       //DebugTool.LogDebug( SolarEngineDemoLog+"init",StarkUtils.IsCloudRuntime().ToString()); 
       //       
       //       
       //       Debug.Log( SolarEngineDemoLog+"init"+StarkSDK.IsCloudInitOver); 
       //       
       //       // string filePath = Application.persistentDataPath + "/SolarEngineData.json";
       //       // Debug.Log("filePath"+ filePath);
       //       if (StarkSDK.IsCloudInitOver)
       //       {
       //           InitSDKCallback();
       //       }
       //       else
       //       {
       //           StarkSDK.StarkCloudInitOverEvent = () => {  InitSDKCallback();};
       //       }
       //       
       //       if (!StarkUtils.IsCloudRuntime())
       //       {
       //           Debug.Log("[SolarEngineDemo] init not cloud");
       //           InitSDKCallback(true);
       //       }
       //         if (StarkSDK.IsCloudInitOver)
       //  {
       //      InitSDKCallback(true);
       //  }
       //  else
       //  {
       //      StarkSDK.StarkCloudInitOverEvent = () => {  InitSDKCallback(true);};
       //  }
       //  
       //  if (!StarkUtils.IsCloudRuntime())
       //  {
       //      Debug.Log("[SolarEngineDemo] init not cloud");
       //      InitSDKCallback(true);
       //  }
       InitSDKCallback(true);
    
    }
    public void InitSDKWithRelease()
    {
        // Debug.Log( SolarEngineDemoLog+"init"+StarkUtils.IsCloudRuntime()); 
        //
        // //DebugTool.LogDebug( SolarEngineDemoLog+"init",StarkUtils.IsCloudRuntime().ToString()); 
        //
        //
        // Debug.Log( SolarEngineDemoLog+"init"+StarkSDK.IsCloudInitOver); 
        //
        // string filePath = Application.persistentDataPath + "/SolarEngineData.json";
        // Debug.Log("filePath"+ filePath);
        // if (StarkSDK.IsCloudInitOver)
        // {
        //     InitSDKCallback(false);
        // }
        // else
        // {
        //     StarkSDK.StarkCloudInitOverEvent = () => {  InitSDKCallback(false);};
        // }
        //
        // if (!StarkUtils.IsCloudRuntime())
        // {
        //     Debug.Log("[SolarEngineDemo] init not cloud");
        //     InitSDKCallback(false);
        // }
        //  InitSDKCallback();
        InitSDKCallback(false);
    
    }

    private bool isprinit = false;
    public void InitSDKCallback(bool isdebug=true)
    {

    

  
        string appkey = "";


        Debug.Log("[SolarEngineDemo] init click11111");
       //String AppKey = "0e0556667b592636";
       // String AppKey = "11";

      String AppKey = "e78185651df3202f";
 
        MiniGameInitParams initParams = new MiniGameInitParams();
        initParams.anonymous_openid = "anonymous_openid";
        initParams.unionid = "unionid";
        
        
        if(isdebug)
        initParams.openid = "debug123456789";
        else
        { initParams.openid = "release123456789";
        }
        SEConfig seConfig = new SEConfig();
       
      
        
        
        RCConfig rcConfig = new RCConfig();
        seConfig.logEnabled = true;
        seConfig.isDebugModel = isdebug;
        rcConfig.mergeType = RCMergeType.ByUser;
        rcConfig.enable = true;
        rcConfig.customIDProperties = new Dictionary<string, object> { { "c", 1 } };
        rcConfig.customIDEventProperties = new Dictionary<string, object> { { "e", 1 } };
        rcConfig.customIDUserProperties = new Dictionary<string, object> { { "u", 1 } };
        
        
        
        TencentAdvertisingGameSDKInitParams tencentAdvertisingGameSDKInitParams = new TencentAdvertisingGameSDKInitParams();
        tencentAdvertisingGameSDKInitParams.user_action_set_id = 1207670035;
        tencentAdvertisingGameSDKInitParams.secret_key = "4bcd1b1003c96d4d5344a520adfb3205";
        tencentAdvertisingGameSDKInitParams.appid = "wx146cc579585c8a5f";
        tencentAdvertisingGameSDKInitParams.tencentSdkIsAutoTrack = false;
        
        initParams.tencentAdvertisingGameSDKInitParams= tencentAdvertisingGameSDKInitParams;
        
        initParams.reportingToTencentSdk = 1;
        initParams.isInitTencentAdvertisingGameSDK = true;
        
        
        seConfig.miniGameInitParams = initParams;
        
        
        //
        Analytics.SEAttributionCallback callback = new Analytics.SEAttributionCallback(attributionCallback);
        seConfig.attributionCallback = callback;

        Analytics.SESDKInitCompletedCallback initCallback = initSuccessCallback;
        seConfig.initCompletedCallback = initCallback;
        seConfig.isGDPRArea = true;
        // SEAdapterInterface seAdapterInterface = new WeChatAdapter();
        // seAdapterInterface.deleteAll();
        // object obj = seAdapterInterface.getData("isprinit", typeof(int)) ;
       // Debug.Log(obj);
        //
        // if (obj==null)
        // {
        //     Debug.Log("预初始化");
            SolarEngine.Analytics.preInitSeSdk(AppKey);
            
        //     seAdapterInterface.saveData("isprinit",1);
        // }
     
        
       
        seConfig.caid = 
            "[{\"version\":\"20220111\",\"caid\":\"912ec803b2ce49e4a541068d495ab570\"},{\"version\":\"20211207\",\"caid\":\"e332a76c29654fcb7f6e6b31ced090c7\"}]";
        SolarEngine.Analytics.initSeSdk(AppKey, seConfig,rcConfig);
        setRemoteDefaultConfig();
        initRemoteConfig();


    }
    public void initRemoteConfig()
    {

        Dictionary<string, object> eventProperties = new Dictionary<string, object>
        {
            { "event", "value1" },
            { "k2", 2 }
        };
        SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
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
       
        //t
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
        
        

        SESDKRemoteConfig.Item itemString = remoteConfig.stringItem("test", "test");
        SESDKRemoteConfig.Item itemJson = remoteConfig.jsonItem("testjson", defaultConfig6);
        SESDKRemoteConfig.Item itemBool = remoteConfig.boolItem("testbool", true);
        SESDKRemoteConfig.Item itemInt = remoteConfig.intItem("testint", 1);
        SESDKRemoteConfig.Item[] defaultConfigArray= new SESDKRemoteConfig.Item[]{itemString,itemJson,itemBool,itemInt};
        
        
        remoteConfig.SetRemoteDefaultConfig(defaultConfigArray);
    
     
        //JObject
        JObject obj = new JObject();
        obj.Add("name", "test");
        obj.Add("age", 1);
        JToken t1 = JToken.FromObject(t);
        obj.Add("t1",  t1 );
        JToken list1 = JToken.FromObject(list);
        obj.Add("list1",  list1 );
        
        
       //Dictionary  
      
        
     
        //
        // Dictionary<string, object> defaultConfig3 = remoteConfig.boolItem("testbool", true);
        // Dictionary<string, object> defaultConfig4 = remoteConfig.intItem("testint", 1);


        // Dictionary<string, object>[] defaultConfigArray = new Dictionary<string, object>[]
        //     { defaultConfig1, defaultConfig6, defaultConfig3, defaultConfig4 };

       

    }

    public void trackAdClick()
    {
        Debug.Log("[SolarEngineDemo] trackAdClick click");

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
        Debug.Log("[SolarEngineDemo] trackRegister click");

        RegisterAttributes RegisterAttributes = new RegisterAttributes();
        RegisterAttributes.register_type = "QQ_test";
        RegisterAttributes.register_status = "success";
        RegisterAttributes.customProperties = getCustomProperties();
        RegisterAttributes.reportingToTencentSdk = 1;
        SolarEngine.Analytics.trackRegister(RegisterAttributes);



    }

    public void track()
    {

        Debug.Log("[SolarEngineDemo] track custom click");

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

        Debug.Log("[SolarEngineDemo] trackAppAtrr click");
        AttAttributes AppAttributes = new AttAttributes();
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
        Debug.Log("[SolarEngineDemo] trackIAP click");

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

        productsAttributes.reportingToTencentSdk = 1;
        
        SolarEngine.Analytics.trackIAP(productsAttributes);
    }

    public void trackAdImpression()
    {
        Debug.Log("[SolarEngineDemo] trackAdImpression click");

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
        Debug.Log("[SolarEngineDemo] trackLogin click");

        LoginAttributes LoginAttributes = new LoginAttributes();
        LoginAttributes.login_type = "QQ_test";
        LoginAttributes.login_status = "success1_test";
        LoginAttributes.customProperties = getCustomProperties();
        SolarEngine.Analytics.trackLogin(LoginAttributes);

    }

    public void trackOrder()
    {
        Debug.Log("[SolarEngineDemo] trackOrderclick");

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
        Debug.Log("[SolarEngineDemo] userInit click");

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
        Debug.Log("[SolarEngineDemo] userUpdate click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K1", "V1");
        userProperties.Add("K2", "V2");
        userProperties.Add("K3", 2);
        SolarEngine.Analytics.userUpdate(userProperties);

    }

    public void userAdd()
    {
        Debug.Log("[SolarEngineDemo] userAdd click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K1", 10);
        userProperties.Add("K2", 100);
        userProperties.Add("K3", 2);
        SolarEngine.Analytics.userAdd(userProperties);

    }

    public void userUnset()
    {
        Debug.Log("[SolarEngineDemo] userUnset click");

        SolarEngine.Analytics.userUnset(new string[] { "K1", "K2" });

    }

    public void userAppend()
    {
        Debug.Log("[SolarEngineDemo] userAppend click");

        Dictionary<string, object> userProperties = new Dictionary<string, object>();
        userProperties.Add("K1", "V1");
        userProperties.Add("K2", "V2");
        userProperties.Add("K3", 2);
        SolarEngine.Analytics.userAppend(userProperties);



    }

    public void userDelete()
    {
        Debug.Log("[SolarEngineDemo] SEUserDeleteTypeByAccountId click");

        SolarEngine.Analytics.userDelete(UserDeleteType.ByAccountId);
        SolarEngine.Analytics.userDelete(UserDeleteType.ByVisitorId);
    }



    private void initSuccessCallback(int code)
    {
        Debug.Log("SEUnity:initSuccessCallback  code : " + code);

    }


    private void attributionCallback(int code, Dictionary<string, object> attribution)
    {
        Debug.Log("SEUnity: errorCode : " + code);

        if (code == 0)
        {
            foreach (var VARIABLE in attribution)
            {
                Debug.Log("SEUnity: attribution : " + VARIABLE);
                //DebugTool.LogDebug("SEUnity: attribution", VARIABLE.ToString());
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
    CreateButton("InitD", InitSDKWithDebug);
        CreateButton("InitD Release", InitSDKWithRelease);
    
    CreateButton("Set Super Properties", SetSuperPropertiesHandler);
    CreateButton("Set Preset Event", SetPresetEventHandler);
    CreateButton("Login", LoginHandler);
    CreateButton("Logout", LogoutHandler);
    CreateButton("Get Account ID", GetAccountIdHandler);
    CreateButton("Set Visitor ID", SetVisitorIdHandler);
    CreateButton("Get Distinct ID", GetDistinctIdHandler);
    CreateButton("Event Start", EventStartHandler);
    CreateButton("Event Finish", EventFinishHandler);
    CreateButton("Set Channel", SetChannelHandler);
    CreateButton("Unset Super Property", UnsetSuperPropertyHandler);
    CreateButton("Clear Super Properties", ClearSuperPropertiesHandler);
    CreateButton("Report Immediately", ReportEventImmediatelyHandler);
    CreateButton("Get Attribution", GetAttributionHandler);
    CreateButton("Set Channel Again", SetChannelAgainHandler);
    CreateButton("Set Referrer Title", SetReferrerTitleHandler);
    CreateButton("Set Xcx Page Title", SetXcxPageTitleHandler);
    CreateButton("Get Preset Properties", GetPresetPropertiesHandler);
    GUILayout.EndVertical();

    // 右列按钮
    GUILayout.BeginVertical();
    CreateButton("Track Ad Click", TrackAdClickHandler);
    CreateButton("Track Register", TrackRegisterHandler);
    CreateButton("Track Custom", TrackCustomHandler);
    CreateButton("Track App Attributes", TrackAppAttrHandler);
    CreateButton("Track IAP", TrackIAPHandler);
    CreateButton("Track Ad Impression", TrackAdImpressionHandler);
    CreateButton("Track Login", TrackLoginHandler);
    CreateButton("Track Order", TrackOrderHandler);
    
    
    CreateButton("trackReActive", trackReActive);
    CreateButton("trackShare", trackShare);
    CreateButton("trackTutorialFinish", trackTutorialFinish);
    CreateButton("trackAddToWishlist", trackAddToWishlist);
    CreateButton("trackViewContentMall", trackViewContentMall);
    CreateButton("trackUpdateLevel", trackUpdateLevel);
    CreateButton("trackCreateRole", trackCreateRole);
    CreateButton("trackViewContentActivity", trackViewContentActivity);
    
    
    
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
    
    
    CreateButton("iOS CallBack",  iOSCallBackHandler1);
    GUILayout.EndVertical();
    GUILayout.EndHorizontal();

    GUILayout.EndScrollView();
    GUILayout.EndArea();
}

 private void iOSCallBackHandler1()
 {
     // StartCoroutine(iOSCallBackHandler());
     
     Analytics.requestTrackingAuthorizationWithCompletionHandler( OnRequestTrackingAuthorizationCompletedHandler);
     //yield return new WaitForSeconds(0.5f);
     InitSDKCallback();
     Debug.Log( " iOS CallBack ");
 }
 // private IEnumerator iOSCallBackHandler()
 // {
 //    
 //     Analytics.requestTrackingAuthorizationWithCompletionHandler( OnRequestTrackingAuthorizationCompletedHandler);
 //     //yield return new WaitForSeconds(0.5f);
 //     InitSDKCallback();
 //     Debug.Log( " iOS CallBack ");
 //
 //
 // }
 private void OnRequestTrackingAuthorizationCompletedHandler(int code)
 {
     Debug.Log( " OnRequestTrackingAuthorizationCompletedHandler : " + code);
   
 }

private void CreateButton(string buttonText, Action onClickHandler)
{
    if (GUILayout.Button(buttonText, GUILayout.Height(50), GUILayout.Width(200)))
    {
        onClickHandler();
    }
    GUILayout.Space(5);
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
    Debug.Log(Analytics.getAccountId());
    //DebugTool.LogDebug( SolarEngineDemoLog,Analytics.getAccountId());
    
}

private void SetVisitorIdHandler()
{
    Analytics.setVisitorId("99999999999");
}

private void GetDistinctIdHandler()
{
    Analytics.getDistinctId(_distinct);

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
    foreach (var variable in dic)
    {
        Debug.Log(variable);
        //DebugTool.LogDebug( SolarEngineDemoLog,variable.ToString());
    }
}

private void SetChannelAgainHandler()
{
    Analytics.setChannel("google");
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
    Debug.Log(str);
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
    try
    {
        SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
        string str = remoteConfig.FastFetchRemoteConfig("testint");
        Debug.Log(str);
        //DebugTool.LogDebug( SolarEngineDemoLog,str);
    }
    catch (Exception e)
    {
        Debug.LogError("Error in FastFetchSingleHandler: " + e.Message);
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
            Debug.Log(VARIABLE.Key + " " + VARIABLE.Value);
            //DebugTool.LogDebug( SolarEngineDemoLog,VARIABLE.Key + " " + VARIABLE.Value);
        }
    }
    catch (Exception e)
    {
        Debug.LogError("Error in FastFetchAllHandler: " + e.Message);
    }
}

private void AsyncFetchSingleHandler()
{
    try
    {
        SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
        remoteConfig.AsyncFetchRemoteConfig("testint", onFetchRemoteConfigCallbacks);
        
    }
    catch (Exception e)
    {
        Debug.LogError("Error in AsyncFetchSingleHandler: " + e.Message);
    }
}

private void AsyncFetchAllHandler()
{
    try
    {
        SESDKRemoteConfig remoteConfig = new SESDKRemoteConfig();
        remoteConfig.AsyncFetchRemoteConfig(onFetchRemoteConfigCallback);
    }
    catch (Exception e)
    {
        Debug.LogError("Error in AsyncFetchAllHandler: " + e.Message);
    }
}

private void onFetchRemoteConfigCallbacks(string result)
{
    // 异步获取参数下发的回调为 string 类型，需要开发者根据自己的业务配置进行属性转换，此处以 bool 类型为例
    Debug.Log(result);
    //DebugTool.LogDebug( SolarEngineDemoLog,result);
}

private void onFetchRemoteConfigCallback(Dictionary<string, object> result)
{
    // 异步获取参数下发的回调为 string 类型，需要开发者根据自己的业务配置进行属性转换，此处以 bool 类型为例
    string str = JsonConvert.SerializeObject(result);
    Debug.Log(str);
    //DebugTool.LogDebug( SolarEngineDemoLog,str);
}


private void _distinct(Distinct distinct)
{
    Debug.Log(string.Format("distinct_id: {0} \n distinct_id_type: {1}", distinct.distinct_id, distinct.distinct_id_type));
    
    //DebugTool.LogDebug( SolarEngineDemoLog,string.Format("distinct_id: {0} \n distinct_id_type: {1}", distinct.distinct_id, distinct.distinct_id_type));
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

    }
  

    
    


    

