//
//  SolarEngineSDKUnityBridge.m
//  SolarEngineSDK
//
//  Created by MVP on 2022/1/24.
//

#import <Foundation/Foundation.h>
#import "SEBridgeCommon.h"



typedef void (*SEBridgeCallback)(int errorCode, const char * data);
typedef void (*SEBridgeInitCallback)(int errorCode);

NSString * const SEKeyUnityEventType                                   = @"_event_type";
NSString * const SEKeyUnityEventNameIAP                                = @"_appPur";
NSString * const SEKeyUnityEventNameAdImpresstion                      = @"_appImp";
NSString * const SEKeyUnityEventNameAdClick                            = @"_appClick";
NSString * const SEKeyUnityEventNameAppAttr                            = @"_appAttr";
NSString * const SEKeyUnityEventNameRegister                           = @"_appReg";
NSString * const SEKeyUnityEventNameLogin                              = @"_appLogin";
NSString * const SEKeyUnityEventNameOrder                              = @"_appOrder";
NSString * const SEKeyUnityEventNameCustom                             = @"_custom_event";
NSString * const SEKeyUnityKeyCustomProperties                         = @"_customProperties";
NSString * const SEKeyUnityKeyCustomEventName                          = @"_customEventName";




typedef NS_ENUM(NSUInteger, SEBridgePresetEventType) {
    AppInstall,
    AppStart,
    AppEnd,
    AppAll
};

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
static id getSolarEngineSDKInstance() {
    Class sdkClass = NSClassFromString(@"SolarEngineSDK");
    if (!sdkClass) {
        se_innerLog(@"SolarEngineSDK class not found");
        return nil;
    }
    
    SEL sharedInstanceSel = NSSelectorFromString(@"sharedInstance");
    if (![sdkClass respondsToSelector:sharedInstanceSel]) {
        se_innerLog(@"sharedInstance method not found");
        return nil;
    }
    
    return [sdkClass performSelector:sharedInstanceSel];
}

// Helper function to safely call methods (renamed to avoid conflicts)
static id se_safeCallSDKMethod(id target, SEL selector, ...) {
    if (!target || !selector) return nil;
    
    if (![target respondsToSelector:selector]) {
        se_innerLog(@"Method %@ not found on target", NSStringFromSelector(selector));
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
//        id returnValue;
//        [invocation getReturnValue:&returnValue];
//        
//       
//        return returnValue;
        
        void *returnValue;
        [invocation getReturnValue:&returnValue];
       return (__bridge id)returnValue;
    }
    
    return nil;
}

id seTrimValue(id __nullable value){
    if (!value || [value isEqual:[NSNull null]]) {
        return nil;
    }
    return value;
}

static id buildIAPAttribute(const char *IAPAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:IAPAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackIAPWithAttributes, error :%@",msg);
        return nil;
    }
    
    Class attributeClass = NSClassFromString(@"SEIAPEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SEIAPEventAttribute class not found");
        return nil;
    }
    
    id attribute = [[attributeClass alloc] init];
    
    // Set properties using KVC
    [attribute setValue:seTrimValue([dict objectForKey:@"productID"]) forKey:@"productID"];
    [attribute setValue:seTrimValue([dict objectForKey:@"productName"]) forKey:@"productName"];
    [attribute setValue:seTrimValue([dict objectForKey:@"orderId"]) forKey:@"orderId"];
    [attribute setValue:seTrimValue([dict objectForKey:@"currencyType"]) forKey:@"currencyType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"payType"]) forKey:@"payType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"failReason"]) forKey:@"failReason"];
    [attribute setValue:@([seTrimValue([dict objectForKey:@"payStatus"]) integerValue]) forKey:@"payStatus"];
    [attribute setValue:@([seTrimValue([dict objectForKey:@"productCount"]) integerValue]) forKey:@"productCount"];
    [attribute setValue:@([seTrimValue([dict objectForKey:@"payAmount"]) doubleValue]) forKey:@"payAmount"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];

    return attribute;
}

static id buildAdImpressionAttribute(const char *adImpressionAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:adImpressionAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackAdImpressionWithAttributes, error :%@", msg);
        return nil;
    }

    Class attributeClass = NSClassFromString(@"SEAdImpressionEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SEAdImpressionEventAttribute class not found");
        return nil;
    }
    
    id attribute = [[attributeClass alloc] init];
    
    [attribute setValue:@([seTrimValue([dict objectForKey:@"adType"]) integerValue]) forKey:@"adType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"adNetworkPlatform"]) forKey:@"adNetworkPlatform"];
    [attribute setValue:seTrimValue([dict objectForKey:@"adNetworkAppID"]) forKey:@"adNetworkAppID"];
    [attribute setValue:seTrimValue([dict objectForKey:@"adNetworkPlacementID"]) forKey:@"adNetworkPlacementID"];
    [attribute setValue:seTrimValue([dict objectForKey:@"currency"]) forKey:@"currency"];
    [attribute setValue:seTrimValue([dict objectForKey:@"mediationPlatform"]) forKey:@"mediationPlatform"];
    [attribute setValue:@([seTrimValue([dict objectForKey:@"ecpm"]) doubleValue]) forKey:@"ecpm"];
    [attribute setValue:@([seTrimValue([dict objectForKey:@"rendered"]) boolValue]) forKey:@"rendered"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];
    
    return attribute;
}

static id buildAdClickAttribute(const char *adClickAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:adClickAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackAdClickWithAttributes, error :%@", msg);
        return nil;
    }

    Class attributeClass = NSClassFromString(@"SEAdClickEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SEAdClickEventAttribute class not found");
        return nil;
    }
    
    id attribute = [[attributeClass alloc] init];
    
    [attribute setValue:@([seTrimValue([dict objectForKey:@"adType"]) integerValue]) forKey:@"adType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"adNetworkPlatform"]) forKey:@"adNetworkPlatform"];
    [attribute setValue:seTrimValue([dict objectForKey:@"adNetworkPlacementID"]) forKey:@"adNetworkPlacementID"];
    [attribute setValue:seTrimValue([dict objectForKey:@"mediationPlatform"]) forKey:@"mediationPlatform"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];
    
    return attribute;
}

static id buildRegisterAttribute(const char *registerAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:registerAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackRegisterWithAttributes, error :%@", msg);
        return nil;
    }

    Class attributeClass = NSClassFromString(@"SERegisterEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SERegisterEventAttribute class not found");
        return nil;
    }
    
    id attribute = [[attributeClass alloc] init];
    [attribute setValue:seTrimValue([dict objectForKey:@"type"]) forKey:@"registerType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"status"]) forKey:@"registerStatus"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];
    
    return attribute;
}

static id buildLoginAttribute(const char *loginAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:loginAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackLoginWithAttributes, error :%@", msg);
        return nil;
    }

    Class attributeClass = NSClassFromString(@"SELoginEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SELoginEventAttribute class not found");
        return nil;
    }
    
    id attribute = [[attributeClass alloc] init];
    [attribute setValue:seTrimValue([dict objectForKey:@"type"]) forKey:@"loginType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"status"]) forKey:@"loginStatus"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];

    return attribute;
}

static id buildOrderAttribute(const char *orderAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:orderAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackOrderWithAttributes, error :%@", msg);
        return nil;
    }

    Class attributeClass = NSClassFromString(@"SEOrderEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SEOrderEventAttribute class not found");
        return nil;
    }
    
    id attribute = [[attributeClass alloc] init];
    [attribute setValue:seTrimValue([dict objectForKey:@"orderID"]) forKey:@"orderID"];
    [attribute setValue:@([seTrimValue([dict objectForKey:@"payAmount"]) doubleValue]) forKey:@"payAmount"];
    [attribute setValue:seTrimValue([dict objectForKey:@"currencyType"]) forKey:@"currencyType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"payType"]) forKey:@"payType"];
    [attribute setValue:seTrimValue([dict objectForKey:@"status"]) forKey:@"status"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];

    return attribute;
}

static id buildAppAttrAttribute(const char *AppAttrAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:AppAttrAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackOrderWithAttributes, error :%@", msg);
        return nil;
    }

    Class attributeClass = NSClassFromString(@"SEAppAttrEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SEAppAttrEventAttribute class not found");
        return nil;
    }
    
    id appAttr = [[attributeClass alloc] init];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adNetwork"]) forKey:@"adNetwork"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"subChannel"]) forKey:@"subChannel"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adAccountID"]) forKey:@"adAccountID"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adAccountName"]) forKey:@"adAccountName"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adCampaignID"]) forKey:@"adCampaignID"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adCampaignName"]) forKey:@"adCampaignName"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adOfferID"]) forKey:@"adOfferID"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adOfferName"]) forKey:@"adOfferName"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adCreativeID"]) forKey:@"adCreativeID"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"adCreativeName"]) forKey:@"adCreativeName"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"attributionPlatform"]) forKey:@"attributionPlatform"];
    [appAttr setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];
    
    return appAttr;
}

static id buildCustomEventAttribute(const char *customAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:customAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"trackCustomWithAttributes, error :%@", msg);
        return nil;
    }

    Class attributeClass = NSClassFromString(@"SECustomEventAttribute");
    if (!attributeClass) {
        se_innerLog(@"SECustomEventAttribute class not found");
        return nil;
    }
    
    id attribute = [[attributeClass alloc] init];
    [attribute setValue:seTrimValue([dict objectForKey:@"_custom_event_name"]) forKey:@"eventName"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_customProperties"]) forKey:@"customProperties"];
    [attribute setValue:seTrimValue([dict objectForKey:@"_pre_properties"]) forKey:@"presetProperties"];
    
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
                se_innerLog(@"setPresetEvent, error :%@",msg);
                return;
            }
        }
    }
    
    NSString *_presetEventName = [NSString stringWithUTF8String:presetEventName];
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setPresetEvent:withProperties:");
    if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppInstall"]) {
        se_safeCallSDKMethod(sdk, selector,( int)AppInstall, dict);
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppStart"]) {
        se_safeCallSDKMethod(sdk, selector,( int)AppStart, dict);
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppEnd"]) {
        se_safeCallSDKMethod(sdk, selector, ( int)AppEnd, dict);
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppAll"]) {
        se_safeCallSDKMethod(sdk, selector,  (int)AppAll, dict);
    }
}

void __iOSSolarEngineSDKPreInit(const char * appKey, const char * SEUserId) {
    se_innerLog(@"__iOSSolarEngineSDKPreInit called");
    NSString *_appKey = [NSString stringWithUTF8String:appKey];
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"preInitWithAppKey:");
    se_safeCallSDKMethod(sdk, selector, _appKey);
}

void __iOSSolarEngineSDKInit(const char * appKey, const char * SEUserId, const char *seConfig, const char *rcConfig) {
    se_innerLog(@"__iOSSolarEngineSDKInit called");
    NSString *_appKey = [NSString stringWithUTF8String:appKey];
    
    NSString *_seConfig = [NSString stringWithUTF8String:seConfig];
    NSData *data = [_seConfig dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *seDict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_seConfig];
        se_innerLog(@"__iOSSolarEngineSDKInit, seConfig error :%@",msg);
        return;
    }
    
    // Create config object using runtime
    Class configClass = NSClassFromString(@"SEConfig");
    if (!configClass) {
        se_innerLog(@"SEConfig class not found");
        return;
    }
    
    id config = [[configClass alloc] init];
    
    // Set config properties using KVC
    [config setValue:@([seDict[@"isDebugModel"] boolValue]) forKey:@"isDebugModel"];
    [config setValue:@([seDict[@"logEnabled"] boolValue]) forKey:@"logEnabled"];
    
#if TARGET_OS_IOS
    [config setValue:@([seDict[@"isCoppaEnabled"] boolValue]) forKey:@"setCoppaEnabled"];
    [config setValue:@([seDict[@"isKidsAppEnabled"] boolValue]) forKey:@"setKidsAppEnabled"];
    [config setValue:@([seDict[@"isEnable2GReporting"] boolValue]) forKey:@"enable2GReporting"];
    [config setValue:@([seDict[@"isGDPRArea"] boolValue]) forKey:@"isGDPRArea"];
    [config setValue:@([seDict[@"attAuthorizationWaitingInterval"] intValue]) forKey:@"attAuthorizationWaitingInterval"];
    
    [config setValue:@([seDict[@"delayDeeplinkEnable"] boolValue]) forKey:@"enableDelayDeeplink"];
#endif
    
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
            se_innerLog(@"__iOSSolarEngineSDKInit, rcConfig error :%@",msg);
            return;
        }
        
        Class remoteConfigClass = NSClassFromString(@"SERemoteConfig");
        if (!remoteConfigClass) {
            se_innerLog(@"SERemoteConfig class not found");
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
            se_innerLog(@"SECustomDomain class not found");
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
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"startWithAppKey:config:");
    se_safeCallSDKMethod(sdk, selector, _appKey, config);
}

void __iOSSESDKSetInitCompletedCallback(SEBridgeInitCallback callback) {
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setInitCompletedCallback:");
    se_safeCallSDKMethod(sdk, selector, ^(int code) {
        if (callback) {
            callback(code);
        }
    });
}

void __iOSSESDKRequestTrackingAuthorizationWithCompletionHandler(SEBridgeInitCallback callback) {
#if TARGET_OS_IOS
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"requestTrackingAuthorizationWithCompletionHandler:");
    se_safeCallSDKMethod(sdk, selector, ^(NSUInteger status) {
        if (callback) {
            callback((int)status);
        }
    });
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"__iOSSESDKRequestTrackingAuthorizationWithCompletionHandler");
#endif
}

void __iOSSESDKSetAttributionDataCallback(SEBridgeCallback callback) {
#if TARGET_OS_IOS
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setAttributionCallback:");
    se_safeCallSDKMethod(sdk, selector, ^(int code, NSDictionary * _Nullable attribution) {
        NSString *attData = nil;
        if ([attribution isKindOfClass:NSDictionary.class]) {
            NSData *data = [NSJSONSerialization dataWithJSONObject:attribution options:0 error:nil];
            if (data) {
                attData = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            }
        }
        
        if (callback) {
            callback(code, convertNSStringToCString(attData));
        }
    });
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"setAttributionCallback");
#endif
}

char* __iOSSolarEngineSDKGetAttribution(void)
{
#if TARGET_OS_IOS
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return nil;
    
    SEL selector = NSSelectorFromString(@"getAttributionData");
    NSDictionary *attribution = se_safeCallSDKMethod(sdk, selector);
    
    if (![attribution isKindOfClass:NSDictionary.class]) {
        return nil;
    }
    NSData *data = [NSJSONSerialization dataWithJSONObject:attribution options:0 error:nil];
    if (data == nil) {
        return nil;
    }
    NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    return convertNSStringToCString(dataString);
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"getAttributionData");
    return "";
#endif
}

void __iOSSolarEngineSDKSetGDPRArea(bool isGDPRArea) {
#if TARGET_OS_IOS
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setGDPRArea:");
    se_safeCallSDKMethod(sdk, selector, @(isGDPRArea));
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"setGDPRArea");
#endif
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
            se_innerLog(@"track, error :%@",msg);
            return;
        }
    }

    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"track:withProperties:");
    se_safeCallSDKMethod(sdk, selector, _eventName, dict);
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
            se_innerLog(@"track, error :%@",msg);
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
            se_innerLog(@"track, error :%@",msg);
            return;
        }
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"track:withCustomProperties:withPresetProperties:");
    se_safeCallSDKMethod(sdk, selector, _eventName, customProperties, preProperties);
}

void __iOSSolarEngineSDKTrackAppReEngagement(const char *attributes)
{
    se_innerLog(@"__iOSSolarEngineSDKTrackAppReEngagement called");
#if TARGET_OS_IOS
    NSDictionary *customProperties = nil;
    if (attributes != NULL) {
        NSString *jsonString = [NSString stringWithUTF8String:attributes];
        NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error = nil;
        customProperties = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
        if (error) {
            NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
            se_innerLog(@"trackAppReEngagement, error :%@",msg);
            customProperties = nil;
        }
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackAppReEngagement:");
    se_safeCallSDKMethod(sdk, selector, customProperties);
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"trackAppReEngagement");
#endif
}

void __iOSSolarEngineSDKTrackFirstEventWithAttributes(const char *firstEventAttribute) {
    se_innerLog(@"__iOSSolarEngineSDKTrackFirstEventWithAttributes called");

    NSString *jsonString = [NSString stringWithUTF8String:firstEventAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        se_innerLog(@"TrackFirstEventWithAttributes, error :%@",msg);
        return;
    }
    
    NSString *eventType = [dict objectForKey:SEKeyUnityEventType];
    NSString *first_event_check_id  =  [dict objectForKey:@"_first_event_check_id"];

    id attribute = nil;
    
    if ([eventType isEqualToString:SEKeyUnityEventNameIAP]) {
        attribute = buildIAPAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyUnityEventNameAdImpresstion]) {
        attribute = buildAdImpressionAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyUnityEventNameAdClick]) {
        attribute = buildAdClickAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyUnityEventNameAppAttr]) {
        attribute = buildAppAttrAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyUnityEventNameRegister]) {
        attribute = buildRegisterAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyUnityEventNameLogin]) {
        attribute = buildLoginAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyUnityEventNameOrder]) {
        attribute = buildOrderAttribute(firstEventAttribute);
    } else if ([eventType isEqualToString:SEKeyUnityEventNameCustom]) {
        attribute = buildCustomEventAttribute(firstEventAttribute);
    }

    if (attribute) {
        if ([first_event_check_id isKindOfClass:NSString.class]) {
            [attribute setValue:first_event_check_id forKey:@"firstCheckId"];
        }
        
        id sdk = getSolarEngineSDKInstance();
        if (!sdk) return;
        
        SEL selector = NSSelectorFromString(@"trackFirstEvent:");
        se_safeCallSDKMethod(sdk, selector, attribute);
    } else {
        se_innerLog(@"TrackFirstEventWithAttributes attribute is nil");
    }
}

void __iOSSolarEngineSDKTrackIAPWithAttributes(const char *IAPAttribute)
{
    id attribute = buildIAPAttribute(IAPAttribute);
    if (!attribute) return;
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackIAPWithAttributes:");
    se_safeCallSDKMethod(sdk, selector, attribute);
}

void __iOSSolarEngineSDKTrackAdImpressionWithAttributes(const char *adImpressionAttribute)
{
    id attribute = buildAdImpressionAttribute(adImpressionAttribute);
    if (!attribute) return;
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackAdImpressionWithAttributes:");
    se_safeCallSDKMethod(sdk, selector, attribute);
}

void __iOSSolarEngineSDKTrackAdClickWithAttributes(const char *adClickAttribute) {
    id attribute = buildAdClickAttribute(adClickAttribute);
    if (!attribute) return;
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackAdClickWithAttributes:");
    se_safeCallSDKMethod(sdk, selector, attribute);
}

void __iOSSolarEngineSDKTrackRegisterWithAttributes(const char *registerAttribute) {
    id attribute = buildRegisterAttribute(registerAttribute);
    if (!attribute) return;
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackRegisterWithAttributes:");
    se_safeCallSDKMethod(sdk, selector, attribute);
}

void __iOSSolarEngineSDKTrackLoginWithAttributes(const char *loginAttribute) {
    id attribute = buildLoginAttribute(loginAttribute);
    if (!attribute) return;
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackLoginWithAttributes:");
    se_safeCallSDKMethod(sdk, selector, attribute);
}

void __iOSSolarEngineSDKTrackOrderWithAttributes(const char *orderAttribute) {
    id attribute = buildOrderAttribute(orderAttribute);
    if (!attribute) return;
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackOrderWithAttributes:");
    se_safeCallSDKMethod(sdk, selector, attribute);
}

void __iOSSolarEngineSDKTrackAppAttrWithAttributes(const char *AppAttrAttribute) {
    id appAttr = buildAppAttrAttribute(AppAttrAttribute);
    if (!appAttr) return;
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"trackAppAttrWithAttributes:");
    se_safeCallSDKMethod(sdk, selector, appAttr);
}

void __iOSSolarEngineSDKSetVisitorID(const char *visitorID)
{
    NSString *_visitorID = [NSString stringWithUTF8String:visitorID];
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setVisitorID:");
    se_safeCallSDKMethod(sdk, selector, _visitorID);
}

char* __iOSSolarEngineSDKVisitorID(void)
{
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return nil;
    
    SEL selector = NSSelectorFromString(@"visitorID");
    NSString *visitorID = se_safeCallSDKMethod(sdk, selector);
    return convertNSStringToCString(visitorID);
}

char* __iOSSolarEngineSDKGetDistinctId(void) {
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return nil;
    
    SEL selector = NSSelectorFromString(@"getDistinctId");
    NSString *visitorID = se_safeCallSDKMethod(sdk, selector);
    return convertNSStringToCString(visitorID);
}

char* __iOSSolarEngineSDKGetPresetProperties(void){
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return NULL;
    
    SEL selector = NSSelectorFromString(@"getPresetProperties");
    NSDictionary *presetProperties = se_safeCallSDKMethod(sdk, selector);
    
    NSData *data = [NSJSONSerialization dataWithJSONObject:presetProperties options:0 error:nil];
    if (data == nil) {
        return NULL;
    }
    NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    
    return convertNSStringToCString(dataString);
}

void __iOSSolarEngineSDKLoginWithAccountID(const char *accountID)
{
    NSString *_accountID = [NSString stringWithUTF8String:accountID];
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"loginWithAccountID:");
    se_safeCallSDKMethod(sdk, selector, _accountID);
}

char* __iOSSolarEngineSDKAccountID(void)
{
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return nil;
    
    SEL selector = NSSelectorFromString(@"accountID");
    NSString *accountID = se_safeCallSDKMethod(sdk, selector);
    return convertNSStringToCString(accountID);
}

void __iOSSolarEngineSDKLogout(void)
{
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"logout");
    se_safeCallSDKMethod(sdk, selector);
}

void __iOSSolarEngineSDKSetSuperProperties(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        se_innerLog(@"setSuperProperties, error :%@",msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        se_innerLog(@"setSuperProperties, error :%@",msg);
        return;
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setSuperProperties:");
    se_safeCallSDKMethod(sdk, selector, dict);
}

void __iOSSolarEngineSDKUnsetSuperProperty(const char *property)
{
    NSString *_property = [NSString stringWithUTF8String:property];
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"unsetSuperProperty:");
    se_safeCallSDKMethod(sdk, selector, _property);
}

void __iOSSolarEngineSDKClearSuperProperties(void)
{
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"clearSuperProperties");
    se_safeCallSDKMethod(sdk, selector);
}

void __iOSSolarEngineSDKReportEventImmediately(void)
{
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"reportEventImmediately");
    se_safeCallSDKMethod(sdk, selector);
}

void __iOSSolarEngineSDKUserInit(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        se_innerLog(@"userInit, error :%@",msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        se_innerLog(@"userInit, error :%@",msg);
        return;
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"userInit:");
    se_safeCallSDKMethod(sdk, selector, dict);
}

void __iOSSolarEngineSDKUserUpdate(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        se_innerLog(@"userUpdate, error :%@", msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        se_innerLog(@"userUpdate, error :%@", msg);
        return;
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"userUpdate:");
    se_safeCallSDKMethod(sdk, selector, dict);
}

void __iOSSolarEngineSDKUserAdd(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        se_innerLog(@"userAdd, error :%@",msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        se_innerLog(@"userAdd, error :%@",msg);
        return;
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"userAdd:");
    se_safeCallSDKMethod(sdk, selector, dict);
}

void __iOSSolarEngineSDKUserUnset(const char *keys)
{
    NSString *_keys = [NSString stringWithUTF8String:keys];
    NSData *data = [_keys dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSArray *array = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_keys];
        se_innerLog(@"userUnset, error :%@", msg);
        return;
    }
    
    if (![array isKindOfClass:NSArray.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an array",_keys];
        se_innerLog(@"userUnset, error :%@", msg);
        return;
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"userUnset:");
    se_safeCallSDKMethod(sdk, selector, array);
}

void __iOSSolarEngineSDKUserAppend(const char *properties)
{
    NSString *_properties = [NSString stringWithUTF8String:properties];
    NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
        se_innerLog(@"userAppend, error :%@", msg);
        return;
    }
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_properties];
        se_innerLog(@"userAppend, error :%@", msg);
        return;
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"userAppend:");
    se_safeCallSDKMethod(sdk, selector, dict);
}

void __iOSSolarEngineSDKUserDelete(int deleteType)
{
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;

    SEL selector = NSSelectorFromString(@"userDelete:");
    se_safeCallSDKMethod(sdk, selector, deleteType);
}

void __iOSSolarEngineSDKEventStart(const char *eventName)
{
    NSString *_eventName = [NSString stringWithUTF8String:eventName];
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"eventStart:");
    se_safeCallSDKMethod(sdk, selector, _eventName);
}

void __iOSSolarEngineSDKEventFinish(const char *eventJSONStr)
{
    NSString *_eventJSONStr = [NSString stringWithUTF8String:eventJSONStr];
    NSData *data = [_eventJSONStr dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_eventJSONStr];
        se_innerLog(@"eventFinish, error :%@",msg);
        return;
    }
    
    if (![dict isKindOfClass:NSDictionary.class]) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_eventJSONStr];
        se_innerLog(@"eventFinish, error :%@", msg);
        return;
    }
    
    NSString *_eventName = [dict objectForKey:@"eventName"];
    NSDictionary *_attributes = [dict objectForKey:@"attributes"];
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"eventFinish:properties:");
    se_safeCallSDKMethod(sdk, selector, _eventName, _attributes);
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
                se_innerLog(@"eventFinish, error :%@",msg);
            }
            
            if (![dict isKindOfClass:NSDictionary.class]) {
                dict = nil;
                NSString *msg = [NSString stringWithFormat:@"%@ is not an dict",_eventJSONStr];
                se_innerLog(@"eventFinish, error :%@", msg);
            }
            _properties = dict;
        }
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"eventFinish:properties:");
    se_safeCallSDKMethod(sdk, selector, _eventName, _properties);
}

void __iOSSESDKupdatePostbackConversionValue(int conversionValue, SEBridgeCallback callback) {
#if TARGET_OS_IOS
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"updatePostbackConversionValue:completionHandler:");
    se_safeCallSDKMethod(sdk, selector, @(conversionValue), ^(NSError * _Nonnull error) {
        if (callback) {
            callback((int)error.code, convertNSStringToCString(error.description));
        }
    });
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"updatePostbackConversionValue");
#endif
}

void __iOSSESDKupdateConversionValueCoarseValue(int fineValue, const char *  coarseValue, SEBridgeCallback callback) {
#if TARGET_OS_IOS
    NSString *_coarseValue = nil;
    if (coarseValue != NULL) {
        _coarseValue = [NSString stringWithUTF8String:coarseValue];
    }

    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"updatePostbackConversionValue:coarseValue:completionHandler:");
    se_safeCallSDKMethod(sdk, selector, @(fineValue), _coarseValue, ^(NSError * _Nonnull error) {
        if (callback) {
            callback((int)error.code, convertNSStringToCString(error.description));
        }
    });
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"updatePostbackConversionValue");
#endif
}

void __iOSSESDKupdateConversionValueCoarseValueLockWindow(int fineValue, const char *  coarseValue, bool lockWindow, SEBridgeCallback callback) {
#if TARGET_OS_IOS
    NSString *_coarseValue = nil;
    if (coarseValue != NULL) {
        _coarseValue = [NSString stringWithUTF8String:coarseValue];
    }
    
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"updatePostbackConversionValue:coarseValue:lockWindow:completionHandler:");
    se_safeCallSDKMethod(sdk, selector, @(fineValue), _coarseValue, @(lockWindow), ^(NSError * _Nonnull error) {
        if (callback) {
            callback((int)error.code, convertNSStringToCString(error.description));
        }
    });
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"updatePostbackConversionValue");
#endif
}

void __iOSSolarEngineSDKDeeplinkParseCallback(SEBridgeCallback callback) {
#if TARGET_OS_IOS
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setDeepLinkCallback:");
    se_safeCallSDKMethod(sdk, selector, ^(int code, id deeplinkInfo) {
        NSString *dData = nil;
        if (code == 0){
            NSMutableDictionary *deeplinkData = [NSMutableDictionary dictionary];
            
            if ([deeplinkInfo valueForKey:@"from"]) {
                [deeplinkData setObject:[deeplinkInfo valueForKey:@"from"] forKey:@"from"];
            }
            if ([deeplinkInfo valueForKey:@"sedpLink"]) {
                [deeplinkData setObject:[deeplinkInfo valueForKey:@"sedpLink"] forKey:@"sedpLink"];
            }
            if ([deeplinkInfo valueForKey:@"turlId"]) {
                [deeplinkData setObject:[deeplinkInfo valueForKey:@"turlId"] forKey:@"turlId"];
            }
            if ([deeplinkInfo valueForKey:@"customParams"]) {
                [deeplinkData setObject:[deeplinkInfo valueForKey:@"customParams"] forKey:@"customParams"];
            }
            
            NSData *data = [NSJSONSerialization dataWithJSONObject:deeplinkData options:0 error:nil];
            if (data) {
                dData = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            }
        }
        
        if (callback) {
            callback(code, convertNSStringToCString(dData));
        }
    });
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"setDeepLinkCallback");
#endif
}

void __iOSSolarEngineSDKDelayDeeplinkParseCallback(SEBridgeCallback callback) {
#if TARGET_OS_IOS
    id sdk = getSolarEngineSDKInstance();
    if (!sdk) return;
    
    SEL selector = NSSelectorFromString(@"setDelayDeeplinkDeepLinkCallbackWithSuccess:fail:");
    se_safeCallSDKMethod(sdk, selector, ^(id deeplinkInfo) {
        NSString *dData = nil;
        NSMutableDictionary *deeplinkData = [NSMutableDictionary dictionary];
        
        if ([deeplinkInfo valueForKey:@"sedpUrlscheme"]) {
            [deeplinkData setObject:[deeplinkInfo valueForKey:@"sedpUrlscheme"] forKey:@"sedpUrlscheme"];
        }
        if ([deeplinkInfo valueForKey:@"sedpLink"]) {
            [deeplinkData setObject:[deeplinkInfo valueForKey:@"sedpLink"] forKey:@"sedpLink"];
        }
        if ([deeplinkInfo valueForKey:@"turlId"]) {
            [deeplinkData setObject:[deeplinkInfo valueForKey:@"turlId"] forKey:@"turlId"];
        }
        
        NSData *data = [NSJSONSerialization dataWithJSONObject:deeplinkData options:0 error:nil];
        if (data) {
            dData = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        }
        
        if (callback) {
            callback(0, convertNSStringToCString(dData));
        }
        
    }, ^(NSError * _Nullable error) {
        if (callback) {
            callback((int)error.code, convertNSStringToCString(nil));
        }
    });
#elif TARGET_OS_MAC
    se_innerLog(@"%@ is not supported on MacOS", @"setDelayDeeplinkDeepLinkCallbackWithSuccess");
#endif
}



}
