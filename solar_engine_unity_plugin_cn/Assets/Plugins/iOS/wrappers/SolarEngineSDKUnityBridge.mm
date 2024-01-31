//
//  SolarEngineSDKUnityBridge.m
//  SolarEngineSDK
//
//  Created by MVP on 2022/1/24.
//

#import <Foundation/Foundation.h>
#import <SolarEngineSDK/SolarEngineSDK.h>
#import <SESDKRemoteConfig/SESDKRemoteConfig.h>

typedef void (*SEBridgeCallback)(int errorCode, const char * data);

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

    SEIAPEventAttribute *attribute = [[SEIAPEventAttribute alloc] init];
    attribute.productID = productID;
    attribute.productName = productName;
    attribute.orderId = orderId;
    attribute.currencyType = currencyType;
    attribute.payType = payType;
    attribute.payStatus = (SolarEngineIAPStatus)[payStatus integerValue];
    attribute.failReason = failReason;
    attribute.payAmount = [payAmount doubleValue];
    attribute.productCount = [productCount integerValue];
    attribute.customProperties = customProperties;

    return attribute;

}

static SEAdImpressionEventAttribute *buildAdImpressionAttribute(const char *adImpressionAttribute) {
    NSString *jsonString = [NSString stringWithUTF8String:adImpressionAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackAdImpressionWithAttributes, error :%@", msg);
        return nil;
    }

    NSString *adNetworkPlatform = [dict objectForKey:SEAdImpressionPropertyAdPlatform];
    NSString *adNetworkAppID = [dict objectForKey:SEAdImpressionPropertyAppID];
    NSString *adNetworkPlacementID = [dict objectForKey:SEAdImpressionPropertyPlacementID];
    NSString *currency = [dict objectForKey:SEAdImpressionPropertyCurrency];
    
    NSNumber *adType = [dict objectForKey:SEAdImpressionPropertyAdType];
    NSNumber *ecpm = [dict objectForKey:SEAdImpressionPropertyEcpm];
    NSString *mediationPlatform = [dict objectForKey:SEAdImpressionPropertyMediationPlatform];
    NSNumber *rendered = [dict objectForKey:SEAdImpressionPropertyRendered];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];

    SEAdImpressionEventAttribute *attribute = [[SEAdImpressionEventAttribute alloc] init];
    attribute.adType = [adType integerValue];
    attribute.adNetworkPlatform = adNetworkPlatform;
    attribute.adNetworkAppID = adNetworkAppID;
    attribute.adNetworkPlacementID = adNetworkPlacementID;
    attribute.currency = currency;
    attribute.mediationPlatform = mediationPlatform;
    attribute.ecpm = [ecpm doubleValue];
    if (rendered) {
        attribute.rendered = [rendered boolValue];
    }
    attribute.customProperties = customProperties;
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

    NSString *adNetworkPlatform = [dict objectForKey:SEAdImpressionPropertyAdPlatform];
    NSNumber *adType = [dict objectForKey:SEAdImpressionPropertyAdType];
    NSString *adNetworkPlacementID = [dict objectForKey:SEAdImpressionPropertyPlacementID];
    NSString *mediationPlatform = [dict objectForKey:SEAdImpressionPropertyMediationPlatform];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    
    SEAdClickEventAttribute *attribute = [[SEAdClickEventAttribute alloc] init];
    attribute.adType = [adType integerValue];
    attribute.adNetworkPlatform = adNetworkPlatform;
    attribute.adNetworkPlacementID = adNetworkPlacementID;
    attribute.mediationPlatform = mediationPlatform;
    attribute.customProperties = customProperties;
    
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

    NSString *type = [dict objectForKey:SERegisterPropertyType];
    NSString *status = [dict objectForKey:SERegisterPropertyStatus];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    
    SERegisterEventAttribute *attribute = [[SERegisterEventAttribute alloc] init];
    attribute.registerType = type;
    attribute.registerStatus = status;
    attribute.customProperties = customProperties;
    
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

    NSString *type = [dict objectForKey:SELoginPropertyType];
    NSString *status = [dict objectForKey:SELoginPropertyStatus];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    
    SELoginEventAttribute *attribute = [[SELoginEventAttribute alloc] init];
    attribute.loginType = type;
    attribute.loginStatus = status;
    attribute.customProperties = customProperties;

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

    NSString *orderID = [dict objectForKey:SEOrderPropertyID];
    NSNumber *payAmount = [dict objectForKey:SEOrderPropertyPayAmount];
    NSString *currencyType = [dict objectForKey:SEOrderPropertyCurrencyType];
    NSString *payType = [dict objectForKey:SEOrderPropertyPayType];
    NSString *status = [dict objectForKey:SEOrderPropertyStatus];
    NSDictionary *customProperties = [dict objectForKey:@"_customProperties"];
    
    SEOrderEventAttribute *attribute = [[SEOrderEventAttribute alloc] init];
    attribute.orderID = orderID;
    attribute.payAmount = [payAmount doubleValue];
    attribute.currencyType = currencyType;
    attribute.payType = payType;
    attribute.status = status;
    attribute.customProperties = customProperties;

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

    SEAppAttrEventAttribute *appAttr = [[SEAppAttrEventAttribute alloc] init];
    appAttr.adNetwork = adNetwork;
    appAttr.subChannel = subChannel;
    appAttr.adAccountID = adAccountID;
    appAttr.adAccountName = adAccountName;
    appAttr.adCampaignID = adCampaignID;
    appAttr.adCampaignName = adCampaignName;
    appAttr.adOfferID = adOfferID;
    appAttr.adOfferName = adOfferName;
    appAttr.adCreativeID = adCreativeID;
    appAttr.adCreativeName = adCreativeName;
    appAttr.attributionPlatform = attributionPlatform;

    appAttr.customProperties = customProperties;
    
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

    NSString *eventName             = [dict objectForKey:@"_custom_event_name"];
    NSDictionary *customProperties  = [dict objectForKey:@"_customProperties"];

    SECustomEventAttribute *attribute = [[SECustomEventAttribute alloc] init];
    attribute.eventName = eventName;
    attribute.customProperties = customProperties;
    
    return attribute;
}

extern "C" {

char* convertNSStringToCString(const NSString* nsString)
{
    if (nsString == NULL)
        return NULL;

    const char* nsStringUtf8 = [nsString UTF8String];
    //create a null terminated C string on the heap so that our string's memory isn't wiped out right after method's return
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
    if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppInstall"]) {
        [[SolarEngineSDK sharedInstance] setPresetEvent:SEPresetEventTypeAppInstall withProperties:dict];
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppStart"]) {
        [[SolarEngineSDK sharedInstance] setPresetEvent:SEPresetEventTypeAppStart withProperties:dict];
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppEnd"]) {
        [[SolarEngineSDK sharedInstance] setPresetEvent:SEPresetEventTypeAppEnd withProperties:dict];
    } else if ([_presetEventName isEqualToString:@"SEPresetEventTypeAppAll"]) {
        [[SolarEngineSDK sharedInstance] setPresetEvent:SEPresetEventTypeAppAll withProperties:dict];
    }
}

//void __iOSSolarEngineSDKInit(const char *appKey, const char *SEUserId, const char *seConfig)
//{
//    NSLog(@"__iOSSolarEngineSDKInit called");
//    NSString *_appKey = [NSString stringWithUTF8String:appKey];
//    NSString *_SEUserId = [NSString stringWithUTF8String:SEUserId];
//
//
//    NSString *_seConfig = [NSString stringWithUTF8String:seConfig];
//    NSData *data = [_seConfig dataUsingEncoding:NSUTF8StringEncoding];
//    NSError *error = nil;
//    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
//    if (error) {
//        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_seConfig];
//        NSLog(@"__iOSSolarEngineSDKInit, error :%@",msg);
//        return;
//    }
//
//    NSLog(@"se-dict = %@",dict);
//
//    SEConfig *config = [[SEConfig alloc] init];
//    config.isDebug = dict[@"isDebug"];
//
//
//    SERemoteConfig *remoteConfig = [[SERemoteConfig alloc] init];
//    remoteConfig.enable = YES;
//    config.remoteConfig = remoteConfig;
//
//    [[SolarEngineSDK sharedInstance] startWithAppKey:_appKey userId:_SEUserId config:config];
//}

void __iOSSolarEngineSDKPreInit(const char * appKey, const char * SEUserId) {
 
    NSLog(@"__iOSSolarEngineSDKPreInit called");
    NSString *_appKey = [NSString stringWithUTF8String:appKey];
    
    [[SolarEngineSDK sharedInstance] preInitWithAppKey:_appKey];

}

void __iOSSolarEngineSDKInit(const char * appKey, const char * SEUserId, const char *seConfig, const char *rcConfig) {
    
    NSLog(@"__iOSSolarEngineSDKInit called");
    NSString *_appKey = [NSString stringWithUTF8String:appKey];
    NSString *_SEUserId = [NSString stringWithUTF8String:SEUserId];
    
    
    NSString *_seConfig = [NSString stringWithUTF8String:seConfig];
    NSData *data = [_seConfig dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *seDict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_seConfig];
        NSLog(@"__iOSSolarEngineSDKInit, seConfig error :%@",msg);
        return;
    }
    
    SEConfig *config = [[SEConfig alloc] init];
    config.isDebugModel = [seDict[@"isDebugModel"] boolValue];
    config.logEnabled = [seDict[@"logEnabled"] boolValue];
    config.disableRecordLog = [seDict[@"disableRecordLog"] boolValue];
    config.enable2GReporting = [seDict[@"isEnable2GReporting"] boolValue];

    NSDictionary *rcDict = nil;
    if (rcConfig != NULL) {
        NSString *_rcConfig = [NSString stringWithUTF8String:rcConfig];
        NSData *data = [_rcConfig dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error = nil;
        rcDict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
        if (error) {
            NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_rcConfig];
            NSLog(@"__iOSSolarEngineSDKInit, rcConfig error :%@",msg);
            return;
        }
        
        SERemoteConfig *remoteConfig = [[SERemoteConfig alloc] init];
        remoteConfig.enable = [rcDict[@"enable"] boolValue];
        remoteConfig.mergeType = (SERCMergeType)[rcDict[@"mergeType"] integerValue];

        remoteConfig.customIDProperties = rcDict[@"customIDProperties"];
        remoteConfig.customIDUserProperties = rcDict[@"customIDUserProperties"];
        remoteConfig.customIDEventProperties = rcDict[@"customIDEventProperties"];
        
        config.remoteConfig = remoteConfig;
    }
    
    
    [[SolarEngineSDK sharedInstance] startWithAppKey:_appKey userId:_SEUserId config:config];
}

void __iOSSESDKSetAttributionDataCallback(SEBridgeCallback callback) {
    
    [[SolarEngineSDK sharedInstance] setAttributionCallback:^(int code, NSDictionary * _Nullable attribution) {
            NSString *attData = nil;
            if ([attribution isKindOfClass:NSDictionary.class]) {
                NSData *data = [NSJSONSerialization dataWithJSONObject:attribution options:0 error:nil];
                if (data) {
                    attData = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
                }
            }
        
        if (callback) {
            callback(code,convertNSStringToCString(attData));
        }
    }];
}

char* __iOSSolarEngineSDKGetAttribution(void)
{
    NSDictionary *attribution = [[SolarEngineSDK sharedInstance] getAttributionData];
    if (![attribution isKindOfClass:NSDictionary.class]) {
        return nil;
    }
    NSData *data = [NSJSONSerialization dataWithJSONObject:attribution options:0 error:nil];
    if (data == nil) {
        return nil;
    }
    NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    return convertNSStringToCString(dataString);
}


void __iOSSolarEngineSDKSetGDPRArea(bool isGDPRArea) {
    [[SolarEngineSDK sharedInstance] setGDPRArea:isGDPRArea];
}

void __iOSSolarEngineSDKTrack(const char *eventName, const char *attributes)
{
    NSString *_eventName = [NSString stringWithUTF8String:eventName];
    NSString *_attributes = [NSString stringWithUTF8String:attributes];

    NSData *data = [_attributes dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];

    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_attributes];
        NSLog(@"track, error :%@",msg);
        return;
    }

    [[SolarEngineSDK sharedInstance] track:_eventName withProperties:dict];
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
        attribute = buildAdImpressionAttribute(firstEventAttribute);
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
        [[SolarEngineSDK sharedInstance] trackFirstEvent:attribute];
    } else {
        NSLog(@"TrackFirstEventWithAttributes attribute is nil");
    }
}

void __iOSSolarEngineSDKTrackIAPWithAttributes(const char *IAPAttribute)
{
    NSString *jsonString = [NSString stringWithUTF8String:IAPAttribute];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];

    NSError *error = nil;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (error) {
        NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data", jsonString];
        NSLog(@"trackIAPWithAttributes, error :%@",msg);
        return;
    }

    SEIAPEventAttribute *attribute = buildIAPAttribute(IAPAttribute);
    [[SolarEngineSDK sharedInstance] trackIAPWithAttributes:attribute];
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

    SEAdImpressionEventAttribute *attribute = buildAdImpressionAttribute(adImpressionAttribute);
    [[SolarEngineSDK sharedInstance] trackAdImpressionWithAttributes:attribute];
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
    [[SolarEngineSDK sharedInstance] trackAdClickWithAttributes:attribute];
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
    [[SolarEngineSDK sharedInstance] trackRegisterWithAttributes:attribute];
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
    [[SolarEngineSDK sharedInstance] trackLoginWithAttributes:attribute];
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
    [[SolarEngineSDK sharedInstance] trackOrderWithAttributes:attribute];
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
    [[SolarEngineSDK sharedInstance] trackAppAttrWithAttributes:appAttr];
}


void __iOSSolarEngineSDKSetVisitorID(const char *visitorID)
{
    NSString *_visitorID = [NSString stringWithUTF8String:visitorID];
    [[SolarEngineSDK sharedInstance] setVisitorID:_visitorID];
}

char* __iOSSolarEngineSDKVisitorID(void)
{
    NSString *visitorID = [[SolarEngineSDK sharedInstance] visitorID];
    return convertNSStringToCString(visitorID);
}

char* __iOSSolarEngineSDKGetDistinctId(void) {
    
    NSString *visitorID = [[SolarEngineSDK sharedInstance] getDistinctId];
    return convertNSStringToCString(visitorID);
}

char* __iOSSolarEngineSDKGetPresetProperties(void){
    NSDictionary *presetProperties = [[SolarEngineSDK sharedInstance] getPresetProperties];
    
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
    [[SolarEngineSDK sharedInstance] loginWithAccountID:_accountID];
}

char* __iOSSolarEngineSDKAccountID(void)
{
    NSString *accountID = [[SolarEngineSDK sharedInstance] accountID];
    return convertNSStringToCString(accountID);
}

void __iOSSolarEngineSDKLogout(void)
{
    [[SolarEngineSDK sharedInstance] logout];
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
    [[SolarEngineSDK sharedInstance] setSuperProperties:dict];
}

void __iOSSolarEngineSDKUnsetSuperProperty(const char *property)
{
    NSString *_property = [NSString stringWithUTF8String:property];
    [[SolarEngineSDK sharedInstance] unsetSuperProperty:_property];
}

void __iOSSolarEngineSDKClearSuperProperties(void)
{
    [[SolarEngineSDK sharedInstance] clearSuperProperties];
}

void __iOSSolarEngineSDKReportEventImmediately(void)
{
    [[SolarEngineSDK sharedInstance] reportEventImmediately];
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
    
    [[SolarEngineSDK sharedInstance] userInit:dict];
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
    [[SolarEngineSDK sharedInstance] userUpdate:dict];
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
    [[SolarEngineSDK sharedInstance] userAdd:dict];
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
    
    [[SolarEngineSDK sharedInstance] userUnset:array];
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
    [[SolarEngineSDK sharedInstance] userAppend:dict];
}

void __iOSSolarEngineSDKUserDelete(int deleteType)
{
    
    [[SolarEngineSDK sharedInstance] userDelete:deleteType];
}

void __iOSSolarEngineSDKEventStart(const char *eventName)
{
    NSString *_eventName = [NSString stringWithUTF8String:eventName];
    [[SolarEngineSDK sharedInstance] eventStart:_eventName];
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
    [[SolarEngineSDK sharedInstance] eventFinish:_eventName properties:_attributes];
}

void __iOSSESDKupdatePostbackConversionValue(int conversionValue, SEBridgeCallback callback) {
    
    [[SolarEngineSDK sharedInstance] updatePostbackConversionValue:conversionValue completionHandler:^(NSError * _Nonnull error) {
        if (callback) {
            callback((int)error.code,convertNSStringToCString(error.description));
        }
    }];
}

void __iOSSESDKupdateConversionValueCoarseValue(int fineValue, const char *  coarseValue, SEBridgeCallback callback) {
    
    NSString *_coarseValue = nil;
    if (coarseValue != NULL) {
        _coarseValue = [NSString stringWithUTF8String:coarseValue];
    }

    [[SolarEngineSDK sharedInstance] updatePostbackConversionValue:fineValue coarseValue:_coarseValue completionHandler:^(NSError * _Nonnull error) {
        if (callback) {
            callback((int)error.code,convertNSStringToCString(error.description));
        }
    }];
}

void __iOSSESDKupdateConversionValueCoarseValueLockWindow(int fineValue, const char *  coarseValue, bool lockWindow, SEBridgeCallback callback) {
    
    NSString *_coarseValue = nil;
    if (coarseValue != NULL) {
        _coarseValue = [NSString stringWithUTF8String:coarseValue];
    }
    [[SolarEngineSDK sharedInstance] updatePostbackConversionValue:fineValue coarseValue:_coarseValue lockWindow:lockWindow completionHandler:^(NSError * _Nonnull error) {
        if (callback) {
            callback((int)error.code,convertNSStringToCString(error.description));
        }
    }];
    
}
}
