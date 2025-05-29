
import { SolarEngineConfig,SEPresetEventType,SolarEngineManager } from '@solarengine/core';
import { SEAdImpEventModel,SEAdClickEventModel,SEPurchaseEventModel,
    SERegisterEventModel,SEOrderEventModel,SELoginEventModel,SEAttributionEventModel,
    SECustomEventModel,SEAppReEngagementModel} from '@solarengine/core';

import {AttributionListener,InitSDKListener,DeepLinkListener,
    DeeplinkInfo,DelayDeepLinkListener}from '@solarengine/core';

import {RemoteConfig}from '@solarengine/core';



export const _log: boolean = true;
export const _SolarEngineLog: string = "[SeSDKBridge]:";



export class SEOpenHarmonyProxy {

    static getAttribution(): string{
        if (_log) {
            console.log(_SolarEngineLog, "getAttribution")
        }

        return  SolarEngineManager.getAttribution();
    }

    static preInit(appkey: string) {
        if (_log) {
            console.log(_SolarEngineLog, appkey)
        }
        SolarEngineManager.preInit(globalThis.context,appkey);
    }

    static initialize(appkey: string,config: string, rcConfig?: string) {
        if (_log) {
            console.log(`${_SolarEngineLog} config:${config} \n rcConfig: ${rcConfig}`);
        }
        try {
           const  configObj = SEOpenHarmonyProxy.parseJsonStrict(config);

            const solarEngineConfig = new SolarEngineConfig();
            solarEngineConfig.enable2GReporting = configObj[SolarEngineConfigConstant.Enable2GReporting];
            solarEngineConfig.isDebugMode = configObj[SolarEngineConfigConstant.IsDebugModel];
            solarEngineConfig.logEnabled = configObj[SolarEngineConfigConstant.LogEnabled];
            solarEngineConfig.enableDelayDeeplink = configObj[SolarEngineConfigConstant.EnableDelayDeeplink];

            // 处理远程配置
            if (rcConfig) {
                const rc = new RemoteConfig();
                const  rcConfigObj = SEOpenHarmonyProxy.parseJsonStrict(rcConfig);
                console.log(`${_SolarEngineLog}  \n rcConfig12222: ${JSON.stringify( rcConfigObj)}`);

                rc.enable = rcConfigObj[SolarEngineConfigConstant.RCEnable];
                rc.mergeType = rcConfigObj[SolarEngineConfigConstant.RCMergeType];
                rc.customIDProperties = rcConfigObj[SolarEngineConfigConstant.RCCustomIDProperties] as Record<string, Object> || {};
                console.log(`${_SolarEngineLog}  \n rcConfig111111: ${JSON.stringify( rcConfigObj[SolarEngineConfigConstant.RCCustomIDProperties])}`);

                rc.customIDEventProperties = rcConfigObj[SolarEngineConfigConstant.RCCustomIDEventProperties] as Record<string, Object> || {};
                rc.customIDUserProperties = rcConfigObj[SolarEngineConfigConstant.RCCustomIDUserProperties] as Record<string, Object> || {};

                solarEngineConfig.remoteConfig = rc;
                console.log(`${_SolarEngineLog}  \n rcConfig: ${JSON.stringify( solarEngineConfig.remoteConfig)}`);

            }
            SolarEngineManager.initialize(globalThis.context, appkey, solarEngineConfig);

        } catch (error) {
            console.error(`${_SolarEngineLog} initialize failed:`, error);
        }
    }



    static setOaId(oaid: string) {
        if (_log) {
            console.log(_SolarEngineLog, "setOaId", oaid);
        }
        SolarEngineManager.setOaId(oaid);
    }

    static setChannel(channel: string) {
        if (_log) {
            console.log(_SolarEngineLog, "setChannel", channel);
        }
        SolarEngineManager.setChannel(channel);
    }

    static async setVisitorID(visitor: string) {
        if (_log) {
            console.log(_SolarEngineLog, "setVisitorID", visitor);
        }

        await SolarEngineManager.setVisitorID(visitor);

    }




    static login(accountId: string) {
        if (_log) {
            console.log(_SolarEngineLog, "login", accountId);
        }
        SolarEngineManager.login(accountId);
    }

    static getAccountID(): string {
        if (_log) {
            console.log(_SolarEngineLog, "getAccountID");
        }
        return SolarEngineManager.getAccountID();
    }

    static logout() {
        if (_log) {
            console.log(_SolarEngineLog, "logout");
        }
        SolarEngineManager.logout();
    }

    static setSuperProperties(properties: string): void {

        if (_log) {
            console.log(_SolarEngineLog, "setSuperProperties ",properties);
        }
        try{
            let propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties);

            Object.keys(propertiesJsonObject).forEach(key => {
                const value:Object = propertiesJsonObject[key];
                SolarEngineManager.setSuperProperties(globalThis.context, key,value);
            });
        }
        catch(error){
            console.error(`${_SolarEngineLog} setSuperProperties failed:`, error);
        }

    }

    static unsetSuperProperty(publicKey: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "unsetSuperProperty", publicKey);
        }
        SolarEngineManager.unsetSuperProperty(globalThis.context, publicKey);
    }

    static clearSuperProperties(): void {
        if (_log) {
            console.log(_SolarEngineLog, "clearSuperProperties");
        }
        SolarEngineManager.clearSuperProperties(globalThis.context);
    }



    static setPresetEvent(presetEventType: string, customProperties:string): void {
        if (_log) {
            console.log(_SolarEngineLog, "setPresetEvent", presetEventType,customProperties);
        }
        let propertiesJsonObject:Record<string, Object>={};
        if(customProperties!=""){
            propertiesJsonObject  = SEOpenHarmonyProxy.parseJsonStrict(customProperties) as Record<string,object>||{};

        }

        switch (presetEventType) {
            case SEConstant.PresetEventTypeAppInstall:
                SolarEngineManager.setPresetEventProperties(SEPresetEventType.AppInstall, propertiesJsonObject );
                break;
            case SEConstant.PresetEventTypeAppStart:
                SolarEngineManager.setPresetEventProperties(SEPresetEventType.AppStart, propertiesJsonObject);
                break;
            case SEConstant.PresetEventTypeAppEnd:
                SolarEngineManager.setPresetEventProperties(SEPresetEventType.AppEnd, propertiesJsonObject);
                break;
            case SEConstant.PresetEventTypeAppAll:
                SolarEngineManager.setPresetEventProperties(SEPresetEventType.All, propertiesJsonObject);
                break;
        }

    }
    static trackImpEvent(properties:string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackAdImpression",properties);
        }
        let impModel = new SEAdImpEventModel();
        let propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties);
        impModel.adNetworkPlatform = propertiesJsonObject[SEConstant.IAI_AdPlatform];
        impModel.mediationPlatform = propertiesJsonObject[SEConstant.IAI_MediationPlatform];
        impModel.adType = propertiesJsonObject[SEConstant.IAI_AdType];
        impModel.adNetworkAppID = propertiesJsonObject[SEConstant.IAI_AdAppid];
        impModel.adNetworkADID =  propertiesJsonObject[SEConstant.IAI_AdId];
        impModel.ecpm =  propertiesJsonObject[SEConstant.IAI_AdEcpm];
        impModel.currencyType = propertiesJsonObject[SEConstant.IAI_CurrencyType];
        impModel.isRenderSuccess = propertiesJsonObject[SEConstant.IAI_IsRendered];
        const customProperties:Record<string,object> = propertiesJsonObject[SEConstant.Login_CustomProperties] || {} ;
        impModel.customProperties = customProperties;
        SolarEngineManager.trackAdImpression( impModel);
    }

    /**
     * 采集变现广告点击事件
     * @param o38 广告点击事件模型
     */
    static trackAdClickEvent(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackAdClick");
        }
        let clickEventModel = new SEAdClickEventModel();
        let propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties);

        clickEventModel.adNetworkPlatform = propertiesJsonObject[SEConstant.AdClick_AdPlatform];
        clickEventModel.mediationPlatform = propertiesJsonObject[SEConstant.AdClick_MediationPlatform];
        clickEventModel.adType = propertiesJsonObject[SEConstant.AdClick_AdType];
        clickEventModel.adNetworkADID = propertiesJsonObject[SEConstant.AdClick_AdId];

        const customProperties:Record<string, Object> = propertiesJsonObject[SEConstant.Login_CustomProperties] || {};
        clickEventModel.customProperties = customProperties;
        SolarEngineManager.trackAdClick( clickEventModel);
    }

    /**
     * 采集应用内购买事件
     * @param n38 购买事件模型
     */
    static trackPurchaseEvent(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackPurchase");
        }
        let purchaseEventModel = new SEPurchaseEventModel();
        let propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties);

        purchaseEventModel.orderId = propertiesJsonObject[SEConstant.Order_ID];
        purchaseEventModel.payAmount = propertiesJsonObject[SEConstant.Order_Pay_Amount];
        purchaseEventModel.currencyType = propertiesJsonObject[SEConstant.Order_Currency_Type];
        purchaseEventModel.payType = propertiesJsonObject[SEConstant.Order_Pay_Type];
        purchaseEventModel.productId = propertiesJsonObject[SEConstant.IAP_PID];
        purchaseEventModel.productName = propertiesJsonObject[SEConstant.IAP_PName];
        purchaseEventModel.productNum = propertiesJsonObject[SEConstant.IAP_PCount];
        purchaseEventModel.payStatus = propertiesJsonObject[SEConstant.IAP_Paystatus];
        const customProperties:Record<string, Object> = propertiesJsonObject[SEConstant.Login_CustomProperties] || {};
        purchaseEventModel.customProperties = customProperties;


        SolarEngineManager.trackPurchase( purchaseEventModel);
    }

    /**
     * 采集订单事件
     * @param m38 订单事件模型
     */
    static trackOrderEvent(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackOrder");
        }
        const orderEventModel = new SEOrderEventModel();
        const propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties); // 使用类的 JSON 解析方法

        // 映射订单事件属性（基于 SEConstant 常量）
        orderEventModel.orderId = propertiesJsonObject[SEConstant.Order_ID]; // 订单 ID
        orderEventModel.payAmount = propertiesJsonObject[SEConstant.Order_Pay_Amount]; // 支付金额
        orderEventModel.currencyType = propertiesJsonObject[SEConstant.Order_Currency_Type]; // 货币类型
        orderEventModel.payType = propertiesJsonObject[SEConstant.Order_Pay_Type]; // 支付类型
        orderEventModel.status = propertiesJsonObject[SEConstant.Order_Status]; // 订单状态

        // 处理自定义属性
        const customProperties:Record<string, Object> = propertiesJsonObject[SEConstant.Order_CustomProperties] || {};
        orderEventModel.customProperties = customProperties;

        // 调用 SDK 追踪订单事件
        SolarEngineManager.trackOrder(orderEventModel);
    }

    /**
     * 采集注册事件
     * @param l38 注册事件模型
     */
    static trackRegisterEvent(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackAppRegister");
        }
        const registerEventModel = new SERegisterEventModel();
        const propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties); // 使用类的 JSON 解析方法

        // 映射注册事件属性（基于 SEConstant 常量）
        registerEventModel.regType = propertiesJsonObject[SEConstant.Register_Type]; // 注册类型
        registerEventModel.status = propertiesJsonObject[SEConstant.Register_Status]; // 注册状态

        // 处理自定义属性
        const customProperties:Record<string, Object> = propertiesJsonObject[SEConstant.Register_CustomProperties] || {};
        registerEventModel.customProperties = customProperties;

        // 调用 SDK 追踪注册事件
        SolarEngineManager.trackAppRegister(registerEventModel);
    }
    /**
     * 采集登录事件
     * @param k38 登录事件模型
     */
    static trackLoginEvent(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackAppLogin");
        }
        const loginEventModel = new SELoginEventModel();
        const propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties); // 使用类的 JSON 解析方法

        // 映射登录事件属性（基于 SEConstant 常量）
        loginEventModel.loginType = propertiesJsonObject[SEConstant.Login_Type]; // 登录类型
        loginEventModel.status = propertiesJsonObject[SEConstant.Login_Status]; // 登录状态

        // 处理自定义属性
        const customProperties:Record<string, Object> = propertiesJsonObject[SEConstant.Login_CustomProperties] || {};
        loginEventModel.customProperties = customProperties;

        // 调用 SDK 追踪登录事件
        SolarEngineManager.trackAppLogin( loginEventModel);
    }


    static trackAppAttrEvent(properties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackAppAttribution");
        }
        let attributionEventModel = new SEAttributionEventModel();
        let propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties);

        attributionEventModel.adNetwork = propertiesJsonObject[SEConstant.AppAttr_Ad_Network];
        attributionEventModel.subChannel = propertiesJsonObject[SEConstant.AppAttr_Sub_Channel];
        attributionEventModel.adAccountId = propertiesJsonObject[SEConstant.AppAttr_Ad_Account_ID];
        attributionEventModel.adAccountName = propertiesJsonObject[SEConstant.AppAttr_Ad_Account_Name];
        attributionEventModel.adCampaignId = propertiesJsonObject[SEConstant.AppAttr_Ad_Campaign_ID];
        attributionEventModel.adCampaignName = propertiesJsonObject[SEConstant.AppAttr_Ad_Campaign_Name];
        attributionEventModel.adOfferId = propertiesJsonObject[SEConstant.AppAttr_Ad_Offer_ID];
        attributionEventModel.adOfferName = propertiesJsonObject[SEConstant.AppAttr_Ad_Offer_Name];
        attributionEventModel.adCreativeId = propertiesJsonObject[SEConstant.AppAttr_Ad_Creative_ID];
        attributionEventModel.adCreativeName = propertiesJsonObject[SEConstant.AppAttr_Ad_Creative_Name];
        attributionEventModel.attributionPlatform = propertiesJsonObject[SEConstant.AppAttr_AttributionPlatform];

        const customProperties:Record<string, Object> = propertiesJsonObject[SEConstant.Login_CustomProperties] || {};
        attributionEventModel.customProperties = customProperties;

        SolarEngineManager.trackAppAttribution(attributionEventModel);
    }

    static trackCustomEvent(customEventName:string ,customProperties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackCustom");
        }
        const customEventModel = new SECustomEventModel();
        const customPropertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(customProperties) as Record<string,object>;

        customEventModel.eventName = customEventName;

        customEventModel.customProperties =customPropertiesJsonObject;

        SolarEngineManager.trackCustom( customEventModel);
    }
    static trackCustomEventWithPreEventData(customEventName:string ,customProperties: string,preAttributes:string): void {
        if (_log) {
            console.log(_SolarEngineLog, "trackCustom");
        }
        const customEventModel = new SECustomEventModel();
        const customPropertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(customProperties);
        const preAttributesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(preAttributes);

        customEventModel.eventName = customEventName;

        customEventModel.preProperties = preAttributesJsonObject as Record<string,object>;
        customEventModel.customProperties =customPropertiesJsonObject as Record<string,object>;

        SolarEngineManager.trackCustom( customEventModel);
    }
    /**
     * 采集deeplink拉起成功事件
     * @param f38 DeepLink事件模型
     */
    static trackAppReEngagement(properties: string): void {

        if (_log) {
            console.log(_SolarEngineLog, "trackAppReEngagement",properties);
        }
        let propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties) as Record<string,object>;

        let model = new SEAppReEngagementModel();
        model.customProperties=propertiesJsonObject;
        SolarEngineManager.trackAppReEngagement(model);
    }

    static trackFirstEvent(properties: string): void {

        if (_log) {
            console.log(_SolarEngineLog, "trackFirstEvent",properties);
        }
        let propertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(properties);


        const type:string = propertiesJsonObject[SEConstant.First_EVENT_TYPE];
        switch (type) {
            case SEConstant.Register_EVENT:
                const registerEventModel = new SERegisterEventModel();

                registerEventModel.regType = propertiesJsonObject[SEConstant.Register_Type]; // 注册类型
                registerEventModel.status = propertiesJsonObject[SEConstant.Register_Status]; // 注册状态

                // 处理自定义属性
                registerEventModel.customProperties =  propertiesJsonObject[SEConstant.First_CustomProperties] || {};;

                registerEventModel.checkId =propertiesJsonObject[SEConstant.First_CHECK_ID];

                SolarEngineManager.trackFirstEvent(registerEventModel);
                break;
            case SEConstant.Custom_EVENT:
                const customEventModel = new SECustomEventModel();

                customEventModel.eventName =propertiesJsonObject[SEConstant.Custom_EVENT_NAME ]

                customEventModel.checkId=propertiesJsonObject[SEConstant.First_CHECK_ID];
                customEventModel.preProperties=propertiesJsonObject[SEConstant.Custom_EVENT_PRE];

                customEventModel.customProperties=propertiesJsonObject[SEConstant.First_CustomProperties] || {};

                SolarEngineManager.trackFirstEvent(customEventModel);






        }
    }

    // // ------------------------ 用户属性管理方法 ------------------------

    static userInit(userProperties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "userInit", userProperties);
        }

        const userPropertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(userProperties) as Record<string ,object>;
        Object.keys(userPropertiesJsonObject).forEach(key => {
            const value = userPropertiesJsonObject[key];
            if (_log) {
                console.log(`${_SolarEngineLog} ${key}: ${value}`);
            }

        });
        SolarEngineManager.userInit(userPropertiesJsonObject);
    }


    static userUpdate(userProperties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "userUpdate", userProperties);
        }
        const userPropertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(userProperties) as Record<string ,object>;
        SolarEngineManager.userUpdate(userPropertiesJsonObject);
    }
    //

    static userAdd(userProperties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "userAdd", userProperties);
        }
        const userPropertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(userProperties) as Record<string ,number>;

        SolarEngineManager.userAdd(userPropertiesJsonObject);
    }
    //

    static userAppend(userProperties: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "userAppend", userProperties);
        }
        const userPropertiesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(userProperties) as Record<string ,number>;

        SolarEngineManager.userAppend(userPropertiesJsonObject);
    }
    //

    static userUnset(keys: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "userUnset", keys);
        }
        const keyJsonObject = SEOpenHarmonyProxy.parseJsonStrict(keys) as string[];

        SolarEngineManager.userUnset( keyJsonObject);
    }
    //
    // /**
    //  * 删除用户
    //  */
    static userDelete(deleteType: number): void {
        if (_log) {
            console.log(_SolarEngineLog, "userDelete",deleteType);
        }
        SolarEngineManager.userDelete(deleteType);
    }

    // ------------------------ 时长事件方法 ------------------------
    /**
     * 开始记录时长事件
     * @param eventName 事件名称
     */
    static eventStart(eventName: string): void {
        if (_log) {
            console.log(_SolarEngineLog, "eventStart", eventName);
        }
        SolarEngineManager.eventStart( eventName);
    }

    /**
     * 结束记录时长事件
     * @param eventName 事件名称
     */
    static eventFinish(eventName: string, attributes:string ): void {
        if (_log) {
            console.log(_SolarEngineLog, "eventFinish", eventName,attributes);
        }
        const attributesJsonObject = SEOpenHarmonyProxy.parseJsonStrict(attributes) as Record<string,object>;

        SolarEngineManager.eventFinish(eventName, attributesJsonObject);
    }


    static reportEventImmediately(): void {
        if (_log) {
            console.log(_SolarEngineLog, "reportEventImmediately");
        }

        SolarEngineManager.reportEventImmediately();
    }
    static appDeeplinkOpenURI(url:string):void{
        if (_log) {
            console.log(_SolarEngineLog, "appDeeplinkOpenURI",url);
        }
        SolarEngineManager.appDeeplinkOpenURI(url);

    }

    // ------------------------ 异步方法 ------------------------

    static getVisitorID(callback:(id:string)=>void) {

        SolarEngineManager.getVisitorID().then(result => {
            if (_log) {
                console.log(_SolarEngineLog, "getVisitorID", result);
            }
            callback?.(result);
        })

    }

    static getDistinctId(callback:(id:string)=>void) {

        SolarEngineManager.getDistinctId().then(result => {
            if (_log) {
                console.log(_SolarEngineLog, "getDistinctId", result);
            }
            callback?.(result);
        })

    }
    static   getPresetProperties(callback:(properties:string)=>void)  {

        SolarEngineManager.getPresetProperties().then(result => {
            let   presetProperties=JSON.stringify(result);
            if (_log) {
                console.log(_SolarEngineLog, "getPresetProperties",presetProperties);
            }
            callback?.(presetProperties);
        });




    }

    static  setInitSDKListener(callback:(code:number)=>void){
        let onInitListener: InitSDKListener = {
            onInitComplete: (code: number) => {
                if (_log) {
                    console.log(_SolarEngineLog, "onInitComplete",code)
                }
                callback?.(code);
            }

        };
        SolarEngineManager.setInitSDKListener(onInitListener);

    }


    static  setAttributionListener(callback: (code: number,json: string) => void){
        let attributionListener: AttributionListener = {
            onAttributionSuccess(attribution:Record<string, Object>) {
                let attributionstr= JSON.stringify(attribution);
                if (_log) {
                    console.log(_SolarEngineLog, "onAttributionSuccess",attributionstr)
                }
                callback?.(0,attributionstr);

            },
            onAttributionFailed(errorCode: number) {
                if (_log) {
                    console.log(_SolarEngineLog, "onAttributionFailed： errorCode： ",errorCode)
                }
                callback?.(errorCode,"");
            }
        };

        SolarEngineManager.setAttributionListener(attributionListener);

    }

    static  setDeepLinkListener(callback:(code:number,deeplinkInfo:string)=>void){
        const deepLinkListener: DeepLinkListener = {
            onReceived(code: number, deeplinkInfo?: DeeplinkInfo) {

                const jsonString = JSON.stringify(deeplinkInfo);
                if (_log) {
                    console.log(_SolarEngineLog, "setDeepLinkListener",code,jsonString);
                }
                callback?.(code,jsonString);
            }
        };
        SolarEngineManager.setDeepLinkListener(deepLinkListener);

    }

    static  setDelayDeepLinkListener(callback:(code:number,deeplinkInfo:string)=>void){
        const delayDeepLinkListener: DelayDeepLinkListener = {
            onReceivedSuccess(attribution: Record<string, Object>) {
                const jsonString = JSON.stringify(attribution);
                if (_log) {
                    console.log(_SolarEngineLog, "setDelayDeepLinkListener onReceivedSuccess",jsonString);
                }
                callback?.(0,jsonString);
            },
            onReceivedFailed(onReceivedFailed: number) {
                if (_log) {
                    console.log(_SolarEngineLog, "setDelayDeepLinkListener onReceivedFailed",onReceivedFailed);
                }
                callback?.(onReceivedFailed,"");
            }
        };
        SolarEngineManager.setDelayDeepLinkListener(delayDeepLinkListener)

    }




    // ------------------------ 其他方法 ------------------------
    public  static parseJsonStrict(jsonString: string): object {
        let jsonObject:Record<string, Object>={};
        try {
            const parsed: object= JSON.parse(jsonString);
            if (typeof parsed !== 'object' || parsed === null) {
                if (_log) {
                    console.warn(_SolarEngineLog, "解析的JSON非对象，返回空对象");
                }
                return jsonObject;
            }
            return parsed;
        } catch (error) {
            if (_log) {
                console.error(_SolarEngineLog, "JSON解析失败:", error);
            }
            return jsonObject;
        }
    }





}

export class SolarEngineConfigConstant
{
    public static readonly Enable2GReporting = "isEnable2GReporting";
    public static readonly IsDebugModel       = "isDebugModel";
    public static readonly LogEnabled         = "logEnabled";
    public static readonly EnableDelayDeeplink= "delayDeeplinkEnable";

    public static readonly RcConfig                  = "rcConfig";
    public static readonly RCEnable                  = "enable";
    public static readonly RCMergeType               = "mergeType";
    public static readonly RCCustomIDProperties      = "customIDProperties";
    public static readonly RCCustomIDEventProperties = "customIDEventProperties";
    public static readonly RCCustomIDUserProperties  = "customIDUserProperties";

}
export class SEConstant {


    public static readonly PresetEventTypeAppInstall: string = "SEPresetEventTypeAppInstall";
    public static readonly PresetEventTypeAppStart: string = "SEPresetEventTypeAppStart";
    public static readonly PresetEventTypeAppEnd: string = "SEPresetEventTypeAppEnd";
    public static readonly PresetEventTypeAppAll: string = "SEPresetEventTypeAppAll";


    public static  readonly  First_EVENT_TYPE:string="_event_type";
    public static  readonly  First_CHECK_ID:string ="_first_event_check_id";
    public static readonly First_CustomProperties: string = "_customProperties";

    public static  readonly  Register_EVENT:string="_appReg";
    public static  readonly  Custom_EVENT:string ="_custom_event";
    public static  readonly  Custom_EVENT_NAME:string ="_custom_event_name";
    public static  readonly  Custom_EVENT_PRE:string ="_pre_properties";









    // 应用内广告相关常量
    public static readonly IAI_AdPlatform: string = "_ad_platform";
    public static readonly IAI_MediationPlatform: string = "_mediation_platform";
    public static readonly IAI_AdAppid: string = "_ad_appid";
    public static readonly IAI_AdId: string = "_ad_id";
    public static readonly IAI_AdType: string = "_ad_type";
    public static readonly IAI_AdEcpm: string = "_ad_ecpm";
    public static readonly IAI_CurrencyType: string = "_currency_type";
    public static readonly IAI_IsRendered: string = "_is_rendered";
    public static readonly IAI_CustomProperties: string = "_customProperties";
    public static readonly IAP_PName: string = "_product_name";
    // 产品ID
    public static readonly IAP_PID: string = "_product_id";
    // 产品数量
    public static readonly IAP_PCount: string = "_product_num";
    // 货币类型
    public static readonly IAP_Currency: string = "_currency_type";
    // 订单ID
    public static readonly IAP_OrderId: string = "_order_id";
    // 失败原因
    public static readonly IAP_FailReason: string = "_fail_reason";
    // 支付类型
    public static readonly IAP_PayType: string = "_pay_type";
    // 支付金额
    public static readonly IAP_Amount: string = "_pay_amount";
    // 支付状态
    public static readonly IAP_Paystatus: string = "_pay_status";
    // 自定义属性
    public static readonly IAP_CustomProperties: string = "_customProperties";


    // ------------------------ 广告点击事件常量 ------------------------
    /** 广告平台 */
    public static readonly AdClick_AdPlatform: string = "_ad_platform";
    /** 广告中介平台 */
    public static readonly AdClick_MediationPlatform: string = "_mediation_platform";
    /** 广告ID */
    public static readonly AdClick_AdId: string = "_ad_id";
    /** 广告类型 */
    public static readonly AdClick_AdType: string = "_ad_type";
    /** 自定义属性 */
    public static readonly AdClick_CustomProperties: string = "_customProperties";

    // ------------------------ 注册事件常量 ------------------------
    /** 注册事件名称 */
    public static readonly Register_EventName: string = "_appReg";
    /** 注册类型 */
    public static readonly Register_Type: string = "_reg_type";
    /** 注册状态 */
    public static readonly Register_Status: string = "_status";
    /** 注册自定义属性 */
    public static readonly Register_CustomProperties: string = "_customProperties";

    // ------------------------ 登录事件常量 ------------------------
    /** 登录事件名称 */
    public static readonly Login_EventName: string = "_appLogin";
    /** 登录类型 */
    public static readonly Login_Type: string = "_login_type";
    /** 登录状态 */
    public static readonly Login_Status: string = "_status";
    /** 登录自定义属性 */
    public static readonly Login_CustomProperties: string = "_customProperties";

    // ------------------------ 订单事件常量 ------------------------
    /** 订单事件名称 */
    public static readonly Order_EventName: string = "_appOrder";
    /** 订单ID */
    public static readonly Order_ID: string = "_order_id";
    /** 支付金额 */
    public static readonly Order_Pay_Amount: string = "_pay_amount";
    /** 货币类型 */
    public static readonly Order_Currency_Type: string = "_currency_type";
    /** 支付类型 */
    public static readonly Order_Pay_Type: string = "_pay_type";
    /** 订单状态 */
    public static readonly Order_Status: string = "_status";
    /** 订单自定义属性 */
    public static readonly Order_CustomProperties: string = "_customProperties";

    // ------------------------ 应用归因事件常量 ------------------------
    /** 归因事件名称 */
    public static readonly AppAttr_EventName: string = "_appAttr";
    /** 广告网络 */
    public static readonly AppAttr_Ad_Network: string = "_adnetwork";
    /** 子渠道 */
    public static readonly AppAttr_Sub_Channel: string = "_sub_channel";
    /** 广告账户ID */
    public static readonly AppAttr_Ad_Account_ID: string = "_adaccount_id";
    /** 广告账户名称 */
    public static readonly AppAttr_Ad_Account_Name: string = "_adaccount_name";
    /** 广告活动ID */
    public static readonly AppAttr_Ad_Campaign_ID: string = "_adcampaign_id";
    /** 广告活动名称 */
    public static readonly AppAttr_Ad_Campaign_Name: string = "_adcampaign_name";
    /** 广告计划ID */
    public static readonly AppAttr_Ad_Offer_ID: string = "_adoffer_id";
    /** 广告计划名称 */
    public static readonly AppAttr_Ad_Offer_Name: string = "_adoffer_name";
    /** 广告创意ID */
    public static readonly AppAttr_Ad_Creative_ID: string = "_adcreative_id";
    /** 广告创意名称 */
    public static readonly AppAttr_Ad_Creative_Name: string = "_adcreative_name";
    /** 归因平台 */
    public static readonly AppAttr_AttributionPlatform: string = "_attribution_platform";
    /** 归因自定义属性 */
    public static readonly AppAttr_Ad_CustomProperties: string = "_customProperties";



}

