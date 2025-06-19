//
//  SolarEngineSDKUnityBridge.m
//  SolarEngineSDK
//
//  Created by MVP on 2022/1/24.
//

#import <Foundation/Foundation.h>



typedef void (*SEBridgeCallback)(int errorCode, const char * data);
typedef void (*SEBridgeInitCallback)(int errorCode);

NSString * const SEKeyFlutterEventType                                   = @"_event_type";

NSString * const SEKeyFlutterEventNameIAP                                = @"_appPur";
NSString * const SEKeyFlutterEventNameAdImpresstion                      = @"_appImp";
NSString * const SEKeyFlutterEventNameAdClick                            = @"_appClick";
NSString * const SEKeyFlutterEventNameAppAttr                            = @"_appAttr";
NSString * const SEKeyFlutterEventNameRegister                           = @"_appReg";
NSString * const SEKeyFlutterEventNameLogin                              = @"_appLogin";
NSString * const SEKeyFlutterEventNameOrder                              = @"_appOrder";
NSString * const SEKeyFlutterEventNameCustom                             = @"_custom_event";


NSString * const SEKeyFlutterKeyCustomProperties                         = @"_customProperties";
NSString * const SEKeyFlutterKeyCustomEventName                          = @"_customEventName";


NSString * const SEIAPEventProductID                        = @"_product_id";
NSString * const SEIAPEventProductName                      = @"_product_name";
NSString * const SEIAPEventProductCount                     = @"_product_num";
NSString * const SEIAPEventOrderID                          = @"_order_id";
NSString * const SEIAPEventCurrency                         = @"_currency_type";
NSString * const SEIAPEventPaystatus                        = @"_pay_status";
NSString * const SEIAPEventPayType                          = @"_pay_type";
NSString * const SEIAPEventProductPayAmount                 = @"_pay_amount";
NSString * const SEIAPEventFailReason                       = @"_fail_reason";


NSString * const SEIAPEventPayTypeAlipay                    = @"alipay";
NSString * const SEIAPEventPayTypeWeixin                    = @"weixin";
NSString * const SEIAPEventPayTypeApplePay                  = @"applepay";
NSString * const SEIAPEventPayTypePaypal                    = @"paypal";


NSString * const SEAdImpressionPropertyAdPlatform           = @"_ad_platform";
NSString * const SEAdImpressionPropertyAppID                = @"_ad_appid";
NSString * const SEAdImpressionPropertyPlacementID          = @"_ad_id";
NSString * const SEAdImpressionPropertyAdType               = @"_ad_type";
NSString * const SEAdImpressionPropertyEcpm                 = @"_ad_ecpm";
NSString * const SEAdImpressionPropertyCurrency             = @"_currency_type";
NSString * const SEAdImpressionPropertyMediationPlatform    = @"_mediation_platform";
NSString * const SEAdImpressionPropertyRendered             = @"_is_rendered";


NSString * const SEAppAttrPropertyIsAttr                    = @"_is_attr";
NSString * const SEAppAttrPropertyAdNetwork                 = @"_adnetwork";
NSString * const SEAppAttrPropertySubChannel                = @"_sub_channel";
NSString * const SEAppAttrPropertyAdAccountID               = @"_adaccount_id";
NSString * const SEAppAttrPropertyAdAccountName             = @"_adaccount_name";
NSString * const SEAppAttrPropertyAdCampaignID              = @"_adcampaign_id";
NSString * const SEAppAttrPropertyAdCampaignName            = @"_adcampaign_name";
NSString * const SEAppAttrPropertyAdOfferID                 = @"_adoffer_id";
NSString * const SEAppAttrPropertyAdOfferName               = @"_adoffer_name";
NSString * const SEAppAttrPropertyAdCreativeID              = @"_adcreative_id";
NSString * const SEAppAttrPropertyAdCreativeName            = @"_adcreative_name";
NSString * const SEAppAttrPropertyAttributionPlatform       = @"_attribution_platform";


NSString * const SERegisterPropertyType                     = @"_reg_type";
NSString * const SERegisterPropertyStatus                   = @"_status";


NSString * const SELoginPropertyType                        = @"_login_type";
NSString * const SELoginPropertyStatus                      = @"_status";


NSString * const SEOrderPropertyID                          = @"_order_id";
NSString * const SEOrderPropertyPayAmount                   = @"_pay_amount";
NSString * const SEOrderPropertyCurrencyType                = @"_currency_type";
NSString * const SEOrderPropertyPayType                     = @"_pay_type";
NSString * const SEOrderPropertyStatus                      = @"_status";




@interface SEWrapperManager : NSObject

@property (nonatomic,copy)NSString *sub_lib_version;
@property (nonatomic,copy)NSString *sdk_type;

+ (SEWrapperManager *)sharedInstance;

@end

@implementation SEWrapperManager

+ (SEWrapperManager *)sharedInstance {
    static SEWrapperManager * instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[self alloc] init];
    });
    return instance;
}

- (instancetype)init
{
    self = [super init];
    if (self) {
        _sdk_type = @"unity";
    }
    return self;
}

@end

// Helper function to get SDK instance
static id getSDKInstance() {
    Class sdkClass = NSClassFromString(@"SolarEngineSDK");
    if (!sdkClass) {
        NSLog(@"SolarEngineSDK class not found");
        return nil;
    }
    
    SEL sharedInstanceSel = NSSelectorFromString(@"sharedInstance");
    if (![sdkClass respondsToSelector:sharedInstanceSel]) {
        NSLog(@"sharedInstance method not found");
        return nil;
    }
    
    return [sdkClass performSelector:sharedInstanceSel];
}

// Helper function to safely call methods
static id safeCallMethod(id target, SEL selector, ...) {
    if (!target || !selector) return nil;
    
    if (![target respondsToSelector:selector]) {
        NSLog(@"Method %@ not found on target", NSStringFromSelector(selector));
        return nil;
    }
    
    NSMethodSignature *signature = [target methodSignatureForSelector:selector];
    NSInvocation *invocation = [NSInvocation invocationWithMethodSignature:signature];
    [invocation setTarget:target];
    [invocation setSelector:selector];
    
    va_list args;
    va_start(args, selector);
    
    for (int i = 2; i < signature.numberOfArguments; i++) {
        id arg = va_arg(args, id);
        [invocation setArgument:&arg atIndex:i];
    }
    
    va_end(args);
    
    [invocation invoke];
    
    if (signature.methodReturnLength) {
        id returnValue;
        [invocation getReturnValue:&returnValue];
        return returnValue;
    }
    
    return nil;
}

id seTrimValue(id __nullable value){
    if (!value || [value isEqual:[NSNull null]]) {
        return nil;
    }
    return value;
}

static SEIAPEventAttribute *buildIAPAttribute(const char *IAPAttribute) {
    
    NSString *jsonString = [NSString stringWithUTF8String:IAPAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackIAPWithAttributes, error :%@",msg);
        return nil;
    }
    
    
    
    SEIAPEventAttribute *attribute = nil;
#if __has_include( <SolarEngineSDK/SolarEngineSDK.h>)
    
    NSString *productID     = [dict objectForKey:SEIAPEventProductID];
    NSString *productName   = [dict objectForKey:SEIAPEventProductName];
    NSString *orderId       = [dict objectForKey:SEIAPEventOrderID];
    NSString *currencyType  = [dict objectForKey:SEIAPEventCurrency];
    NSString *payType       = [dict objectForKey:SEIAPEventPayType];
    NSString *failReason    = [dict objectForKey:SEIAPEventFailReason];
    
    NSNumber *payStatus     = [dict objectForKey:SEIAPEventPaystatus];
    NSNumber *productCount  = [dict objectForKey:SEIAPEventProductCount];
    NSNumber *payAmount     = [dict objectForKey:SEIAPEventProductPayAmount];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];

    attribute = [[SEIAPEventAttribute alloc] init];
    attribute.productID = seTrimValue(productID);
    attribute.productName = seTrimValue(productName);
    attribute.orderId = seTrimValue(orderId);
    attribute.currencyType = seTrimValue(currencyType);
    attribute.payType = seTrimValue(payType);
    attribute.payStatus = (SolarEngineIAPStatus)[seTrimValue(payStatus) integerValue];
    attribute.failReason = seTrimValue(failReason);
    attribute.payAmount = [seTrimValue(payAmount) doubleValue];
    attribute.productCount = [seTrimValue(productCount) integerValue];
    attribute.customProperties = seTrimValue(customProperties);
#endif
    
    return attribute;

}

static  id  buildAdImpressionAttribute(const char *adImpressionAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:adImpressionAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackAdImpressionWithAttributes, error :%@", msg);
        return nil;
    }
    

    Class impClass = NSClassFromString(@"SEAdImpressionEventAttribute");
     if (!impClass) {
         NSLog(@"SEAdImpressionEventAttribute class not found");
         return nil;
     }
     
    id attribute = [[impClass alloc] init];
    NSString *adNetworkPlatform = [dict objectForKey:SEAdImpressionPropertyAdPlatform];
    NSString *adNetworkAppID = [dict objectForKey:SEAdImpressionPropertyAppID];
    NSString *adNetworkPlacementID = [dict objectForKey:SEAdImpressionPropertyPlacementID];
    NSString *currency = [dict objectForKey:SEAdImpressionPropertyCurrency];
    
    NSNumber *adType = [dict objectForKey:SEAdImpressionPropertyAdType];
    NSNumber *ecpm = [dict objectForKey:SEAdImpressionPropertyEcpm];
    NSString *mediationPlatform = [dict objectForKey:SEAdImpressionPropertyMediationPlatform];
    NSNumber *rendered = [dict objectForKey:SEAdImpressionPropertyRendered];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];

    [attribute setValue:@([seTrimValue(adType) integerValue]) forKey:@"adType"];
    [attribute setValue:seTrimValue(adNetworkPlatform) forKey:@"adNetworkPlatform"];
    [attribute setValue:seTrimValue(adNetworkAppID) forKey:@"adNetworkAppID"];
    [attribute setValue:seTrimValue(adNetworkPlacementID) forKey:@"adNetworkPlacementID"];
    [attribute setValue:seTrimValue(currency) forKey:@"currency"];
    [attribute setValue:seTrimValue(mediationPlatform) forKey:@"mediationPlatform"];
    [attribute setValue:@([seTrimValue(ecpm) doubleValue]) forKey:@"ecpm"];
    [attribute setValue:@([seTrimValue(rendered) boolValue]) forKey:@"rendered"];
    [attribute setValue:seTrimValue(customProperties) forKey:@"customProperties"];
  
//    attribute.adNetworkPlatform = seTrimValue(adNetworkPlatform);
//    attribute.adNetworkAppID = seTrimValue(adNetworkAppID);
//    attribute.adNetworkPlacementID = seTrimValue(adNetworkPlacementID);
//    attribute.currency = seTrimValue(currency);
//    attribute.mediationPlatform = seTrimValue(mediationPlatform);
//    attribute.ecpm = [seTrimValue(ecpm) doubleValue];
//    attribute.rendered = [seTrimValue(rendered) boolValue];
//    attribute.customProperties = seTrimValue(customProperties);

    return attribute;

}


static SEAdClickEventAttribute *buildAdClickAttribute(const char *adClickAttribute) {
    
    NSString *jsonString = [NSString stringWithUTF8String:adClickAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackAdClickWithAttributes, error :%@", msg);
        return nil;
    }
    SEAdClickEventAttribute *attribute =nil;

    NSString *adNetworkPlatform = [dict objectForKey:SEAdImpressionPropertyAdPlatform];
    NSNumber *adType = [dict objectForKey:SEAdImpressionPropertyAdType];
    NSString *adNetworkPlacementID = [dict objectForKey:SEAdImpressionPropertyPlacementID];
    NSString *mediationPlatform = [dict objectForKey:SEAdImpressionPropertyMediationPlatform];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    
     [[SEAdClickEventAttribute alloc] init];
    attribute.adType = [seTrimValue(adType) integerValue];
    attribute.adNetworkPlatform = seTrimValue(adNetworkPlatform);
    attribute.adNetworkPlacementID = seTrimValue(adNetworkPlacementID);
    attribute.mediationPlatform = seTrimValue(mediationPlatform);
    attribute.customProperties = seTrimValue(customProperties);
#endif
    
    return attribute;
}

static SERegisterEventAttribute *buildRegisterAttribute(const char *registerAttribute) {
    
    NSString *jsonString = [NSString stringWithUTF8String:registerAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackRegisterWithAttributes, error :%@", msg);
        return nil;
    }
    SERegisterEventAttribute *attribute =nil;
#if __has_include( <SolarEngineSDK/SolarEngineSDK.h>)

    NSString *type = [dict objectForKey:SERegisterPropertyType];
    NSString *status = [dict objectForKey:SERegisterPropertyStatus];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    attribute= [[SERegisterEventAttribute alloc] init];
    attribute.registerType = seTrimValue(type);
    attribute.registerStatus = seTrimValue(status);
    attribute.customProperties = seTrimValue(customProperties);
#endif
    return attribute;

}
static SELoginEventAttribute *buildLoginAttribute(const char *loginAttribute) {
    
    NSString *jsonString = [NSString stringWithUTF8String:loginAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackLoginWithAttributes, error :%@", msg);
        return nil;
    }
    SELoginEventAttribute *attribute =nil;
#if __has_include( <SolarEngineSDK/SolarEngineSDK.h>)

    NSString *type = [dict objectForKey:SELoginPropertyType];
    NSString *status = [dict objectForKey:SELoginPropertyStatus];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    
    attribute= [[SELoginEventAttribute alloc] init];
    attribute.loginType = seTrimValue(type);
    attribute.loginStatus = seTrimValue(status);
    attribute.customProperties = seTrimValue(customProperties);
#endif
    return attribute;
}
static SEOrderEventAttribute *buildOrderAttribute(const char *orderAttribute) {
    
    NSString *jsonString = [NSString stringWithUTF8String:orderAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackOrderWithAttributes, error :%@", msg);
        return nil;
    }
    SEOrderEventAttribute *attribute =nil;
#if __has_include( <SolarEngineSDK/SolarEngineSDK.h>)
    NSString *orderID = [dict objectForKey:SEOrderPropertyID];
    NSNumber *payAmount = [dict objectForKey:SEOrderPropertyPayAmount];
    NSString *currencyType = [dict objectForKey:SEOrderPropertyCurrencyType];
    NSString *payType = [dict objectForKey:SEOrderPropertyPayType];
    NSString *status = [dict objectForKey:SEOrderPropertyStatus];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    
    attribute = [[SEOrderEventAttribute alloc] init];
    attribute.orderID = seTrimValue(orderID);
    attribute.payAmount = [seTrimValue(payAmount) doubleValue];
    attribute.currencyType = seTrimValue(currencyType);
    attribute.payType = seTrimValue(payType);
    attribute.status = seTrimValue(status);
    attribute.customProperties = seTrimValue(customProperties);
#endif
    return attribute;
}
static SEAppAttrEventAttribute *buildAppAttrAttribute(const char *AppAttrAttribute) {
    
    NSString *jsonString = [NSString stringWithUTF8String:AppAttrAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackOrderWithAttributes, error :%@", msg);
        return nil;
    }
    SEAppAttrEventAttribute *appAttr=nil;
#if __has_include( <SolarEngineSDK/SolarEngineSDK.h>)

    NSString *adNetwork         = [dict objectForKey:SEAppAttrPropertyAdNetwork];
    NSString *subChannel        = [dict objectForKey:SEAppAttrPropertySubChannel];
    NSString *adAccountID       = [dict objectForKey:SEAppAttrPropertyAdAccountID];
    NSString *adAccountName     = [dict objectForKey:SEAppAttrPropertyAdAccountName];
    NSString *adCampaignID      = [dict objectForKey:SEAppAttrPropertyAdCampaignID];
    NSString *adCampaignName    = [dict objectForKey:SEAppAttrPropertyAdCampaignName];
    NSString *adOfferID         = [dict objectForKey:SEAppAttrPropertyAdOfferID];
    NSString *adOfferName       = [dict objectForKey:SEAppAttrPropertyAdOfferName];
    NSString *adCreativeID      = [dict objectForKey:SEAppAttrPropertyAdCreativeID];
    NSString *adCreativeName    = [dict objectForKey:SEAppAttrPropertyAdCreativeName];
    NSString *attributionPlatform = [dict objectForKey:SEAppAttrPropertyAttributionPlatform];

    NSDictionary *customProperties  = [dict objectForKey:@"_customProperties"];

    appAttr = [[SEAppAttrEventAttribute alloc] init];
    appAttr.adNetwork = seTrimValue(adNetwork);
    appAttr.subChannel = seTrimValue(subChannel);
    appAttr.adAccountID = seTrimValue(adAccountID);
    appAttr.adAccountName = seTrimValue(adAccountName);
    appAttr.adCampaignID = seTrimValue(adCampaignID);
    appAttr.adCampaignName = seTrimValue(adCampaignName);
    appAttr.adOfferID = seTrimValue(adOfferID);
    appAttr.adOfferName = seTrimValue(adOfferName);
    appAttr.adCreativeID = seTrimValue(adCreativeID);
    appAttr.adCreativeName = seTrimValue(adCreativeName);
    appAttr.attributionPlatform = seTrimValue(attributionPlatform);

    appAttr.customProperties = seTrimValue(customProperties);
#endif
    return appAttr;
}

static SECustomEventAttribute *buildCustomEventAttribute(const char *customAttribute) {
    
    NSString *jsonString = [NSString stringWithUTF8String:customAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackCustomWithAttributes, error :%@", msg);
        return nil;
    }
    SECustomEventAttribute *attribute=nil;
#if __has_include( <SolarEngineSDK/SolarEngineSDK.h>)
    NSString *eventName             = [dict objectForKey:@"_custom_event_name"];
    NSDictionary *customProperties  = [dict objectForKey:@"_customProperties"];
    NSDictionary *preProperties     = [dict objectForKey:@"_pre_properties"];
    
    attribute = [[SECustomEventAttribute alloc] init];
    attribute.eventName = seTrimValue(eventName);
    attribute.customProperties = seTrimValue(customProperties);
    attribute.presetProperties = seTrimValue(preProperties);
#endif
    return attribute;
}

extern "C" {

char* convertNSStringToCString(const NSString* nsString)
{
    if (nsString == NULL)
        return NULL;

    const char* nsStringUtf8 = [nsString UTF8String];
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);

    return cString;
}

void __iOSSolarEngineSDKSetPresetEvent(const char *presetEventName, const char *properties)
{
    NSDictionary *dict = nil;
    if (properties != NULL) {
        NSString *_properties = [NSString stringWithUTF8String:properties];
        if (![_properties isEqualToString:@"null"]) {
            NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
            NSError *error = nil;
            dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
            if (error) {
                NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
                NSLog(@"setPresetEvent, error :%@",msg);
                return;
            }
        }
    }
    
    NSString *_presetEventName = [NSString stringWithUTF8String:presetEventName];
    id sdk = getSDKInstance();
    if (!sdk) return;
    
    SEL selector = nil;
    if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppInstall"]) {
        selector = NSSelectorFromString(@"setPresetEvent:withProperties:");
        safeCallMethod(sdk, selector, @"SEPresetEventTypeAppInstall", dict);
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppStart"]) {
        selector = NSSelectorFromString(@"setPresetEvent:withProperties:");
        safeCallMethod(sdk, selector, @"SEPresetEventTypeAppStart", dict);
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppEnd"]) {
        selector = NSSelectorFromString(@"setPresetEvent:withProperties:");
        safeCallMethod(sdk, selector, @"SEPresetEventTypeAppEnd", dict);
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppAll"]) {
        selector = NSSelectorFromString(@"setPresetEvent:withProperties:");
        safeCallMethod(sdk, selector, @"SEPresetEventTypeAppAll", dict);
    }
}

void __iOSSolarEngineSDKPreInit(const char * appKey, const char * SEUserId) {
    NSLog(@"__iOSSolarEngineSDKPreInit called");
    NSString *_appKey = [NSString stringWithUTF8String:appKey];
    
    id sdk = getSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"preInitWithAppKey:");
    safeCallMethod(sdk, selector, _appKey);
}

void __iOSSolarEngineSDKInit(const char * appKey, const char * SEUserId, const char *seConfig, const char *rcConfig) {
    NSLog(@"__iOSSolarEngineSDKInit called");
    NSString *_appKey = [NSString stringWithUTF8String:appKey];
    
    NSString *_seConfig = [NSString stringWithUTF8String:seConfig];
    NSData *data = [_seConfig dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *seDict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_seConfig];
        NSLog(@"__iOSSolarEngineSDKInit, seConfig error :%@",msg);
        return;
    }
    
    // Create config object using runtime
    Class configClass = NSClassFromString(@"SEConfig");
    if (!configClass) {
        NSLog(@"SEConfig class not found");
        return;
    }
    
    id config = [[configClass alloc] init];
    
    // Set config properties using KVC
    [config setValue:@([seDict[@"isDebugModel"] boolValue]) forKey:@"isDebugModel"];
    [config setValue:@([seDict[@"logEnabled"] boolValue]) forKey:@"logEnabled"];
    

    
    NSString *sub_lib_version = seDict[@"sub_lib_version"];
    if ([sub_lib_version isKindOfClass:[NSString class]]) {
        [SEWrapperManager sharedInstance].sub_lib_version = sub_lib_version;
    }
    
    NSString *sdk_type = seDict[@"sdk_type"];
    if ([sdk_type isKindOfClass:[NSString class]]) {
        [SEWrapperManager sharedInstance].sdk_type = sdk_type;
    }
    
    // Handle remote config
    if (rcConfig != NULL) {
        NSString *_rcConfig = [NSString stringWithUTF8String:rcConfig];
        NSData *rcData = [_rcConfig dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *rcDict = [NSJSONSerialization JSONObjectWithData:rcData options:0 error:&error];
        if (error) {
            NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_rcConfig];
            NSLog(@"__iOSSolarEngineSDKInit, rcConfig error :%@",msg);
            return;
        }
        
        Class remoteConfigClass = NSClassFromString(@"SERemoteConfig");
        if (!remoteConfigClass) {
            NSLog(@"SERemoteConfig class not found");
            return;
        }
        
        id remoteConfig = [[remoteConfigClass alloc] init];
        [remoteConfig setValue:@([rcDict[@"enable"] boolValue]) forKey:@"enable"];
        [remoteConfig setValue:@([seDict[@"logEnabled"] boolValue]) forKey:@"logEnabled"];
        [remoteConfig setValue:@([rcDict[@"mergeType"] integerValue]) forKey:@"mergeType"];
        [remoteConfig setValue:rcDict[@"customIDProperties"] forKey:@"customIDProperties"];
        [remoteConfig setValue:rcDict[@"customIDUserProperties"] forKey:@"customIDUserProperties"];
        [remoteConfig setValue:rcDict[@"customIDEventProperties"] forKey:@"customIDEventProperties"];
        
        [config setValue:remoteConfig forKey:@"remoteConfig"];
    }
    
    // Handle custom domain
    NSDictionary *customDomainDict = seDict[@"customDomain"];
    if ([customDomainDict isKindOfClass:[NSDictionary class]]) {
        Class customDomainClass = NSClassFromString(@"SECustomDomain");
        if (!customDomainClass) {
            NSLog(@"SECustomDomain class not found");
            return;
        }
        
        id customDomain = [[customDomainClass alloc] init];
        [customDomain setValue:@([customDomainDict[@"enable"] boolValue]) forKey:@"enable"];
        [customDomain setValue:customDomainDict[@"receiverDomain"] forKey:@"receiverDomain"];
        [customDomain setValue:customDomainDict[@"ruleDomain"] forKey:@"ruleDomain"];
        [customDomain setValue:customDomainDict[@"tcpRuleHost"] forKey:@"ruleTcpHost"];
        [customDomain setValue:customDomainDict[@"tcpReceiverHost"] forKey:@"receiverTcpHost"];
        [customDomain setValue:customDomainDict[@"tcpGatewayHost"] forKey:@"gatewayTcpHost"];
        
        [config setValue:customDomain forKey:@"customDomain"];
    }
    
    // Initialize SDK
    id sdk = getSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"startWithAppKey:config:");
    safeCallMethod(sdk, selector, _appKey, config);
}

void __iOSSESDKSetInitCompletedCallback(SEBridgeInitCallback callback) {
    
    id sdkInstance = getSDKInstance();
        if (!sdkInstance) {
            NSLog(@"Failed to get SDK instance");
            return;
        }

        SEL setCallbackSel = NSSelectorFromString(@"setInitCompletedCallback:");
        if (![sdkInstance respondsToSelector:setCallbackSel]) {
            NSLog(@"Method setInitCompletedCallback: not found");
            return;
        }
     void (^block)(int) = ^(int code) {
            if (callback) {
                callback(code);
            }
        };

        // 调用方法
         safeCallMethod(sdkInstance, setCallbackSel, block);
    
}

void __iOSSESDKRequestTrackingAuthorizationWithCompletionHandler(SEBridgeInitCallback callback) {

    NSLog(@"%@ is not supported on MacOS", @"__iOSSESDKRequestTrackingAuthorizationWithCompletionHandler");

}


void __iOSSESDKSetAttributionDataCallback(SEBridgeCallback callback) {


    NSLog(@"%@ is not supported on MacOS", @"setAttributionCallback");

}

char* __iOSSolarEngineSDKGetAttribution(void)
{

    NSLog(@"%@ is not supported on MacOS", @"getAttributionData");
    return "";

}


void __iOSSolarEngineSDKSetGDPRArea(bool isGDPRArea) {

    NSLog(@"%@ is not supported on MacOS", @"setGDPRArea");

}

void __iOSSolarEngineSDKTrack(const char *eventName, const char *attributes)
{
    NSString *_eventName = [NSString stringWithUTF8String:eventName];
    NSString *_attributes = [NSString stringWithUTF8String:attributes];
    
    NSData *data = [_attributes dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = nil;
    
    if (data){
        dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
        if (error) {
            NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_attributes];
            NSLog(@"track, error :%@",msg);
            return;
        }
    }
    
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }
    
    // 查找方法 - (void)track:(NSString *)event withProperties:(NSDictionary *)properties
    SEL trackSelector = NSSelectorFromString(@"track:withProperties:");
    
    // 使用 safeCallMethod 调用
    safeCallMethod(sdkInstance, trackSelector, _eventName, dict);
}

void __iOSSolarEngineSDKTrackCustomEventWithPreAttributes(const char *eventName, const char *customAttributes, const char *preAttributes)
{
    NSString *_eventName = [NSString stringWithUTF8String:eventName];
    NSString *_customAttributes = [NSString stringWithUTF8String:customAttributes];
    NSString *_preAttributes = [NSString stringWithUTF8String:preAttributes];

    NSData *customData = [_customAttributes dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *customProperties = nil;
    if (customData) {
        customProperties = [NSJSONSerialization JSONObjectWithData:customData options:0 error:&error];
        if (error) {
            NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_customAttributes];
            NSLog(@"track, error :%@",msg);
            return;
        }
    }
    
    NSData *preData = [_preAttributes dataUsingEncoding:NSUTF8StringEncoding];
    NSError *preError = nil;
    NSDictionary *preProperties = nil;
    if (preData) {
        preProperties = [NSJSONSerialization JSONObjectWithData:preData options:0 error:&error];
        if (preError) {
            NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_preAttributes];
            NSLog(@"track, error :%@",msg);
            return;
        }
    }

    id sdkInstance = getSDKInstance();
      if (!sdkInstance) {
          NSLog(@"Failed to get SDK instance");
          return;
      }

      SEL trackSelector = NSSelectorFromString(@"track:withCustomProperties:withPresetProperties:");
      if (![sdkInstance respondsToSelector:trackSelector]) {
          NSLog(@"Method not found: %@", NSStringFromSelector(trackSelector));
          return;
      }

      // 使用 safeCallMethod 调用带多个参数的方法
      safeCallMethod(sdkInstance, trackSelector, _eventName, customProperties, preProperties);
}

void __iOSSolarEngineSDKTrackAppReEngagement(const char *attributes)
{

    NSLog(@"%@ is not supported on MacOS", @"trackAppReEngagement");

}

void __iOSSolarEngineSDKTrackFirstEventWithAttributes(const char *firstEventAttribute) {
    
    NSLog(@"__iOSSolarEngineSDKTrackFirstEventWithAttributes called");

    NSString *jsonString = [NSString stringWithUTF8String:firstEventAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"TrackFirstEventWithAttributes, error :%@",msg);
        return;
    }
    
    NSString *eventType = [dict objectForKey:SEKeyFlutterEventType];
    NSString *first_event_check_id  =  [dict objectForKey:@"_first_event_check_id"];

    SEEventBaseAttribute *attribute = nil;
    
    if ([eventType isEqualToString:SEKeyFlutterEventNameIAP]) {
        attribute = buildIAPAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyFlutterEventNameAdImpresstion]) {
      //  attribute = buildAdImpressionAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyFlutterEventNameAdClick]) {
        attribute = buildAdClickAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyFlutterEventNameAppAttr]) {
        attribute = buildAppAttrAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyFlutterEventNameRegister]) {
        attribute = buildRegisterAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyFlutterEventNameLogin]) {
        attribute = buildLoginAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyFlutterEventNameOrder]) {
        attribute = buildOrderAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyFlutterEventNameCustom]) {
        attribute = buildCustomEventAttribute(firstEventAttribute);
    }

    if (attribute) {
        if ([first_event_check_id isKindOfClass:NSString.class]) {
            attribute.firstCheckId = first_event_check_id;
        }
       // [[SolarEngineSDK sharedInstance] trackFirstEvent:attribute];
        id sdkInstance = getSDKInstance();
           if (!sdkInstance) {
               NSLog(@"Failed to get SDK instance");
               return;
           }

           // 2. 准备 selector
           SEL trackSelector = NSSelectorFromString(@"trackFirstEvent:");
           if (![sdkInstance respondsToSelector:trackSelector]) {
               NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
               return;
           }

           // 3. 使用 safeCallMethod 安全调用带参数的方法
           safeCallMethod(sdkInstance, trackSelector, attribute);
    } else {
        NSLog(@"TrackFirstEventWithAttributes attribute is nil");
    }
}

void __iOSSolarEngineSDKTrackIAPWithAttributes(const char *IAPAttribute)
{
    
    NSLog(@"__iOSSolarEngineSDKTrackIAPWithAttributes called");

    NSString *jsonString = [NSString stringWithUTF8String:IAPAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackIAPWithAttributes, error :%@",msg);
        return;
    }

    // Create config object using runtime

    
    SEIAPEventAttribute* attribute = buildIAPAttribute(IAPAttribute);
    //[[SolarEngineSDK sharedInstance] trackIAPWithAttributes:attribute];
    
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 2. 准备 selector
       SEL trackSelector = NSSelectorFromString(@"trackIAPWithAttributes:");
       if (![sdkInstance respondsToSelector:trackSelector]) {
           NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
           return;
       }

       // 3. 使用 safeCallMethod 安全调用带参数的方法
       safeCallMethod(sdkInstance, trackSelector, attribute);
}

void __iOSSolarEngineSDKTrackAdImpressionWithAttributes(const char *adImpressionAttribute)
{
    NSString *jsonString = [NSString stringWithUTF8String:adImpressionAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackAdImpressionWithAttributes, error :%@", msg);
        return;
    }

    id  attribute = buildAdImpressionAttribute(adImpressionAttribute);
  //  SEAdImpressionEventAttribute *attribute = buildAdImpressionAttribute(adImpressionAttribute);
  //  [[SolarEngineSDK sharedInstance] trackAdImpressionWithAttributes:attribute];
    
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 2. 准备 selector
       SEL trackSelector = NSSelectorFromString(@"trackAdImpressionWithAttributes:");
       if (![sdkInstance respondsToSelector:trackSelector]) {
           NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
           return;
       }

       // 3. 使用 safeCallMethod 安全调用带参数的方法
    safeCallMethod(sdkInstance, trackSelector, attribute);
}

void __iOSSolarEngineSDKTrackAdClickWithAttributes(const char *adClickAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:adClickAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackAdClickWithAttributes, error :%@", msg);
        return;
    }
    
    SEAdClickEventAttribute *attribute = buildAdClickAttribute(adClickAttribute);
   // [[SolarEngineSDK sharedInstance] trackAdClickWithAttributes:attribute];
  
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 2. 准备 selector
       SEL trackSelector = NSSelectorFromString(@"trackAdClickWithAttributes:");
       if (![sdkInstance respondsToSelector:trackSelector]) {
           NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
           return;
       }

       // 3. 使用 safeCallMethod 安全调用带参数的方法
       safeCallMethod(sdkInstance, trackSelector, attribute);

}

void __iOSSolarEngineSDKTrackRegisterWithAttributes(const char *registerAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:registerAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackRegisterWithAttributes, error :%@", msg);
        return;
    }

    SERegisterEventAttribute *attribute = buildRegisterAttribute(registerAttribute);
   // [[SolarEngineSDK sharedInstance] trackRegisterWithAttributes:attribute];
    
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 2. 准备 selector
       SEL trackSelector = NSSelectorFromString(@"trackRegisterWithAttributes:");
       if (![sdkInstance respondsToSelector:trackSelector]) {
           NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
           return;
       }

       // 3. 使用 safeCallMethod 安全调用带参数的方法
       safeCallMethod(sdkInstance, trackSelector, attribute);
}

void __iOSSolarEngineSDKTrackLoginWithAttributes(const char *loginAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:loginAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackLoginWithAttributes, error :%@", msg);
        return;
    }

    SELoginEventAttribute *attribute = buildLoginAttribute(loginAttribute);
   // [[SolarEngineSDK sharedInstance] trackLoginWithAttributes:attribute];
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 2. 准备 selector
       SEL trackSelector = NSSelectorFromString(@"trackLoginWithAttributes:");
       if (![sdkInstance respondsToSelector:trackSelector]) {
           NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
           return;
       }

       // 3. 使用 safeCallMethod 安全调用带参数的方法
       safeCallMethod(sdkInstance, trackSelector, attribute);
}

void __iOSSolarEngineSDKTrackOrderWithAttributes(const char *orderAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:orderAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackOrderWithAttributes, error :%@", msg);
        return;
    }

    SEOrderEventAttribute *attribute = buildOrderAttribute(orderAttribute);
 //   [[SolarEngineSDK sharedInstance] trackOrderWithAttributes:attribute];
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 2. 准备 selector
       SEL trackSelector = NSSelectorFromString(@"trackOrderWithAttributes:");
       if (![sdkInstance respondsToSelector:trackSelector]) {
           NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
           return;
       }

       // 3. 使用 safeCallMethod 安全调用带参数的方法
       safeCallMethod(sdkInstance, trackSelector, attribute);
}

void __iOSSolarEngineSDKTrackAppAttrWithAttributes(const char *AppAttrAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:AppAttrAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackOrderWithAttributes, error :%@", msg);
        return;
    }

    SEAppAttrEventAttribute *appAttr = buildAppAttrAttribute(AppAttrAttribute);
  //  [[SolarEngineSDK sharedInstance] trackAppAttrWithAttributes:appAttr];
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 2. 准备 selector
       SEL trackSelector = NSSelectorFromString(@"trackAppAttrWithAttributes:");
       if (![sdkInstance respondsToSelector:trackSelector]) {
           NSLog(@"Method %@ not found", NSStringFromSelector(trackSelector));
           return;
       }

       // 3. 使用 safeCallMethod 安全调用带参数的方法
       safeCallMethod(sdkInstance, trackSelector, appAttr);
}


void __iOSSolarEngineSDKSetVisitorID(const char *visitorID)
{
    NSString *_visitorID = [NSString stringWithUTF8String:visitorID];
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }
    
    SEL setVisitorIDSelector = NSSelectorFromString(@"setVisitorID:");
    if (![sdkInstance respondsToSelector:setVisitorIDSelector]) {
        NSLog(@"Method setVisitorID: not found");
        return;
    }
    
    safeCallMethod(sdkInstance, setVisitorIDSelector, _visitorID);
}

char* __iOSSolarEngineSDKVisitorID(void)
{
    id sdkInstance = getSDKInstance();
        if (!sdkInstance) {
            NSLog(@"Failed to get SDK instance");
            return NULL;
        }

        SEL getVisitorIDSelector = NSSelectorFromString(@"visitorID");
        if (![sdkInstance respondsToSelector:getVisitorIDSelector]) {
            NSLog(@"Method visitorID not found");
            return NULL;
        }

     
        NSString *result = [sdkInstance performSelector:getVisitorIDSelector];

        return convertNSStringToCString(result);
    
}

char* __iOSSolarEngineSDKGetDistinctId(void) {
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return NULL;
       }

       SEL getDistinctIdSelector = NSSelectorFromString(@"getDistinctId");
       if (![sdkInstance respondsToSelector:getDistinctIdSelector]) {
           NSLog(@"Method getDistinctId not found");
           return NULL;
       }

       // 使用 performSelector 调用 getter 方法（无参数）
       NSString *result = [sdkInstance performSelector:getDistinctIdSelector];

       return convertNSStringToCString(result);
}

char* __iOSSolarEngineSDKGetPresetProperties(void){
    id sdkInstance = getSDKInstance();
     if (!sdkInstance) {
         NSLog(@"Failed to get SDK instance");
         return NULL;
     }

     SEL getPresetPropertiesSelector = NSSelectorFromString(@"getPresetProperties");
     if (![sdkInstance respondsToSelector:getPresetPropertiesSelector]) {
         NSLog(@"Method getPresetProperties not found");
         return NULL;
     }
    NSDictionary *presetProperties = [sdkInstance performSelector:getPresetPropertiesSelector];

       if (!presetProperties || ![presetProperties isKindOfClass:[NSDictionary class]]) {
           NSLog(@"Invalid preset properties returned");
           return NULL;
       }

    NSData *data = [NSJSONSerialization dataWithJSONObject:presetProperties options:0 error:nil];
    if (data == nil) {
        return NULL;
    }
    NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    
    return convertNSStringToCString(dataString);
}

void __iOSSolarEngineSDKLoginWithAccountID(const char *accountID)
{
    // 转换 C 字符串为 NSString
       NSString *_accountID = [NSString stringWithUTF8String:accountID];
       if (!_accountID) {
           NSLog(@"Invalid accountID string");
           return;
       }

       // 获取 SDK 实例
       id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       // 查找方法 selector
       SEL loginSelector = NSSelectorFromString(@"loginWithAccountID:");
       if (![sdkInstance respondsToSelector:loginSelector]) {
           NSLog(@"Method loginWithAccountID: not found");
           return;
       }

       // 反射调用方法
       safeCallMethod(sdkInstance, loginSelector, _accountID);
}

char* __iOSSolarEngineSDKAccountID(void)
{
    id sdkInstance = getSDKInstance();
        if (!sdkInstance) {
            NSLog(@"Failed to get SDK instance");
            return NULL;
        }

        SEL accountIDSelector = NSSelectorFromString(@"accountID");
        if (![sdkInstance respondsToSelector:accountIDSelector]) {
            NSLog(@"Method accountID not found");
            return NULL;
        }

        // 调用 getter 方法获取 accountID
        NSString *accountID = [sdkInstance performSelector:accountIDSelector];

        return convertNSStringToCString(accountID);
}

void __iOSSolarEngineSDKLogout(void)
{
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       SEL logoutSelector = NSSelectorFromString(@"logout");
       if (![sdkInstance respondsToSelector:logoutSelector]) {
           NSLog(@"Method logout not found");
           return;
       }

       // 使用 safeCallMethod 调用无参数方法
       safeCallMethod(sdkInstance, logoutSelector);
}

void __iOSSolarEngineSDKSetSuperProperties(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        NSLog(@"setSuperProperties, error :%@",msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        NSLog(@"setSuperProperties, error :%@",msg);
        return;
    }
    id sdkInstance = getSDKInstance();
      if (!sdkInstance) {
          NSLog(@"Failed to get SDK instance");
          return;
      }

      SEL setSuperPropertiesSelector = NSSelectorFromString(@"setSuperProperties:");
      if (![sdkInstance respondsToSelector:setSuperPropertiesSelector]) {
          NSLog(@"Method setSuperProperties: not found");
          return;
      }

      // 使用 safeCallMethod 调用带参数的方法
      safeCallMethod(sdkInstance, setSuperPropertiesSelector, dict);
}

void __iOSSolarEngineSDKUnsetSuperProperty(const char *property)
{
    NSString *_property = [NSString stringWithUTF8String:property];
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       SEL selector = NSSelectorFromString(@"unsetSuperProperty:");
       if (![sdkInstance respondsToSelector:selector]) {
           NSLog(@"Method unsetSuperProperty: not found");
           return;
       }

       safeCallMethod(sdkInstance, selector, _property);
}

void __iOSSolarEngineSDKClearSuperProperties(void)
{
    id sdkInstance = getSDKInstance();
        if (!sdkInstance) {
            NSLog(@"Failed to get SDK instance");
            return;
        }

        SEL selector = NSSelectorFromString(@"clearSuperProperties");
        if (![sdkInstance respondsToSelector:selector]) {
            NSLog(@"Method clearSuperProperties not found");
            return;
        }

        safeCallMethod(sdkInstance, selector);
}

void __iOSSolarEngineSDKReportEventImmediately(void)
{
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       SEL selector = NSSelectorFromString(@"reportEventImmediately");
       if (![sdkInstance respondsToSelector:selector]) {
           NSLog(@"Method reportEventImmediately not found");
           return;
       }

       // 使用 safeCallMethod 调用无参数方法
       safeCallMethod(sdkInstance, selector);
}

void __iOSSolarEngineSDKUserInit(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];

    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        NSLog(@"userInit, error :%@",msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        NSLog(@"userInit, error :%@",msg);
        return;
    }
    
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }

       SEL userInitSelector = NSSelectorFromString(@"userInit:");
       if (![sdkInstance respondsToSelector:userInitSelector]) {
           NSLog(@"Method userInit: not found");
           return;
       }

       // 使用 safeCallMethod 调用带参数的方法
       safeCallMethod(sdkInstance, userInitSelector, dict);
}

void __iOSSolarEngineSDKUserUpdate(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        NSLog(@"userUpdate, error :%@", msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        NSLog(@"userUpdate, error :%@", msg);
        return;
    }
    id sdkInstance = getSDKInstance();
       if (!sdkInstance) {
           NSLog(@"Failed to get SDK instance");
           return;
       }
    SEL userUpdateSelector = NSSelectorFromString(@"userUpdate:");
       if (![sdkInstance respondsToSelector:userUpdateSelector]) {
           NSLog(@"Method userUpdate: not found");
           return;
       }

       // 使用 safeCallMethod 调用带参数的方法
       safeCallMethod(sdkInstance, userUpdateSelector, dict);

}

void __iOSSolarEngineSDKUserAdd(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];

    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        NSLog(@"userAdd, error :%@",msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        NSLog(@"userAdd, error :%@",msg);
        return;
    }
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }

    SEL userAddSelector = NSSelectorFromString(@"userAdd:");
    if (![sdkInstance respondsToSelector:userAddSelector]) {
        NSLog(@"Method userAdd: not found");
        return;
    }

    safeCallMethod(sdkInstance, userAddSelector, dict);

}

void __iOSSolarEngineSDKUserUnset(const char *keys)
{
    NSString *_keys = [NSString stringWithUTF8String:keys];

    NSData *data = [_keys dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSArray *array = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_keys];
        NSLog(@"userUnset, error :%@", msg);
        return;
    }
    
    if (![array isKindOfClass:NSArray.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an array",_keys];
        NSLog(@"userUnset, error :%@", msg);
        return;
    }
    
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }

    SEL userUnsetSelector = NSSelectorFromString(@"userUnset:");
    if (![sdkInstance respondsToSelector:userUnsetSelector]) {
        NSLog(@"Method userUnset: not found");
        return;
    }

    safeCallMethod(sdkInstance, userUnsetSelector, array);
}

void __iOSSolarEngineSDKUserAppend(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        NSLog(@"userAppend, error :%@", msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        NSLog(@"userAppend, error :%@", msg);
        return;
    }
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }

    SEL userAppendSelector = NSSelectorFromString(@"userAppend:");
    if (![sdkInstance respondsToSelector:userAppendSelector]) {
        NSLog(@"Method userAppend: not found");
        return;
    }

    safeCallMethod(sdkInstance, userAppendSelector, dict);
}

void __iOSSolarEngineSDKUserDelete(int deleteType)
{
    
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }

    SEL userDeleteSelector = NSSelectorFromString(@"userDelete:");
    if (![sdkInstance respondsToSelector:userDeleteSelector]) {
        NSLog(@"Method userDelete: not found");
        return;
    }

    // 将 int 转换为 NSNumber 以适配 Objective-C 方法调用
   // NSNumber *deleteTypeNumber = @(deleteType);
    safeCallMethod(sdkInstance, userDeleteSelector, deleteType);
}

void __iOSSolarEngineSDKEventStart(const char *eventName)
{
    NSString *_eventName = [NSString stringWithUTF8String:eventName];
    id sdkInstance = getSDKInstance();
        if (!sdkInstance) {
            NSLog(@"Failed to get SDK instance");
            return;
        }

        SEL eventStartSelector = NSSelectorFromString(@"eventStart:");
        if (![sdkInstance respondsToSelector:eventStartSelector]) {
            NSLog(@"Method eventStart: not found");
            return;
        }

        safeCallMethod(sdkInstance, eventStartSelector, _eventName);
}

void __iOSSolarEngineSDKEventFinish(const char *eventJSONStr)
{
    NSString *_eventJSONStr = [NSString stringWithUTF8String:eventJSONStr];
    NSData *data = [_eventJSONStr dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_eventJSONStr];
        NSLog(@"eventFinish, error :%@",msg);
        return;
    }
    
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_eventJSONStr];
        NSLog(@"eventFinish, error :%@", msg);
        return;
    }
    
    NSString *_eventName = [dict objectForKey:@"eventName"];
    NSDictionary *_attributes = [dict objectForKey:@"attributes"];
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }

    SEL selector = NSSelectorFromString(@"eventFinish:properties:");
    if (![sdkInstance respondsToSelector:selector]) {
        NSLog(@"Method eventFinish:properties: not found");
        return;
    }

    safeCallMethod(sdkInstance, selector, _eventName, _attributes);
}

void __iOSSolarEngineSDKEventFinishNew(const char *eventName, const char *properties)
{
    NSString *_eventName = [NSString stringWithUTF8String:eventName];
    NSDictionary *_properties = nil;
    if (properties != NULL){
        NSString *_eventJSONStr = [NSString stringWithUTF8String:properties];
        if (_eventJSONStr && ![_eventJSONStr isEqualToString:@"null"]){
            NSData *data = [_eventJSONStr dataUsingEncoding:NSUTF8StringEncoding];
            
            NSError *error = nil;
            NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
            
            if (error) {
                dict = nil;
                NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_eventJSONStr];
                NSLog(@"eventFinish, error :%@",msg);
            }
            
            if (![dict isKindOfClass:NSDictionary.class]) {
                dict = nil;
                NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_eventJSONStr];
                NSLog(@"eventFinish, error :%@", msg);
            }
            _properties = dict;
        }
    }
    id sdkInstance = getSDKInstance();
    if (!sdkInstance) {
        NSLog(@"Failed to get SDK instance");
        return;
    }

    SEL selector = NSSelectorFromString(@"eventFinish:properties:");
    if (![sdkInstance respondsToSelector:selector]) {
        NSLog(@"Method eventFinish:properties: not found");
        return;
    }

    safeCallMethod(sdkInstance, selector, _eventName, _properties);
}


void __iOSSESDKupdatePostbackConversionValue(int conversionValue, SEBridgeCallback callback) {

    NSLog(@"%@ is not supported on MacOS", @"updatePostbackConversionValue");

}

void __iOSSESDKupdateConversionValueCoarseValue(int fineValue, const char *  coarseValue, SEBridgeCallback callback) {

    NSLog(@"%@ is not supported on MacOS", @"updatePostbackConversionValue");

}

void __iOSSESDKupdateConversionValueCoarseValueLockWindow(int fineValue, const char *  coarseValue, bool lockWindow, SEBridgeCallback callback) {

    NSLog(@"%@ is not supported on MacOS", @"updatePostbackConversionValue");

}


void __iOSSolarEngineSDKDeeplinkParseCallback(SEBridgeCallback callback) {



    NSLog(@"%@ is not supported on MacOS", @"setDeepLinkCallback");

    
}


void __iOSSolarEngineSDKDelayDeeplinkParseCallback(SEBridgeCallback callback) {

    
    NSLog(@"%@ is not supported on MacOS", @"setDelayDeeplinkDeepLinkCallbackWithSuccess");


}


}




