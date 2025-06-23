//
//  SolarEngineSDKUnityBridge.m
//  SolarEngineSDK
//
//  Created by MVP on 2022/1/24.
//

#include <string.h>
#include <stdlib.h>

#import <Foundation/Foundation.h>
#import "SEBridgeCommon.h"

typedef void (*FetchRemoteConfigCallback)(const char * result);

// Helper function to get RemoteConfig instance
static id getRemoteConfigInstance() {
    Class remoteConfigClass = NSClassFromString(@"SESDKRemoteConfig");
    if (!remoteConfigClass) {
        se_innerLog(@"SESDKRemoteConfig class not found");
        return nil;
    }
    
    SEL sharedInstanceSel = NSSelectorFromString(@"sharedInstance");
    if (![remoteConfigClass respondsToSelector:sharedInstanceSel]) {
        se_innerLog(@"sharedInstance method not found");
        return nil;
    }
    
    return [remoteConfigClass performSelector:sharedInstanceSel];
}

// Helper function to safely call methods (renamed to avoid conflicts)
static id safeCallRemoteConfigMethod(id target, SEL selector, ...) {
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
//        se_innerLog(@"111111111111111111");
//        [invocation getReturnValue:&returnValue];
//        se_innerLog(@"222222222222222222");
//        return returnValue;
        void *returnValue;
                       [invocation getReturnValue:&returnValue];
       return (__bridge id)returnValue;

        
    }
    
    return nil;
}

static char *reteinStr(const char *str) {
    if (str == NULL) {
        return NULL;
    }

    char *ret = (char *)malloc(strlen(str) + 1);
    strcpy(ret, str);
    return ret;
}

extern "C" {

    void __iOSSESDKSetDefaultConfig (const char * config) {
        
        if (config == NULL) {
            se_innerLog(@"Error: _iOSSESDKSetDefaultConfig: config must not be empty.");
            return;
        }
        
        NSArray *defaultConfig = nil;
        NSString *_config = [NSString stringWithUTF8String:config];
        if (![_config isEqualToString:@"null"]) {
            NSData *data = [_config dataUsingEncoding:NSUTF8StringEncoding];
            NSError *error = nil;
            defaultConfig = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
            if (error) {
                NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_config];
                se_innerLog(@"__iOSSESDKSetRemoteConfigUserProperties, error :%@",msg);
                return;
            }
        }
        
        id remoteConfig = getRemoteConfigInstance();
        if (!remoteConfig) return;
        
        SEL selector = NSSelectorFromString(@"setDefaultConfig:");
        safeCallRemoteConfigMethod(remoteConfig, selector, defaultConfig);
    }

    void __iOSSESDKSetRemoteConfigEventProperties(const char *properties) {
        NSDictionary *dict = nil;
        if (properties != NULL) {
            NSString *_properties = [NSString stringWithUTF8String:properties];
            if (![_properties isEqualToString:@"null"]) {
                NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
                NSError *error = nil;
                dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
                if (error) {
                    NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
                    se_innerLog(@"__iOSSESDKSetRemoteConfigUserProperties, error :%@",msg);
                    return;
                }
            }
        }

        id remoteConfig = getRemoteConfigInstance();
        if (!remoteConfig) return;
        
        SEL selector = NSSelectorFromString(@"setRemoteConfigEventProperties:");
        safeCallRemoteConfigMethod(remoteConfig, selector, dict);
    }

    void __iOSSESDKSetRemoteConfigUserProperties(const char *properties) {
        NSDictionary *dict = nil;
        if (properties != NULL) {
            NSString *_properties = [NSString stringWithUTF8String:properties];
            if (![_properties isEqualToString:@"null"]) {
                NSData *data = [_properties dataUsingEncoding:NSUTF8StringEncoding];
                NSError *error = nil;
                dict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
                if (error) {
                    NSString *msg = [NSString stringWithFormat:@"%@ is not an invalid JSON data",_properties];
                    se_innerLog(@"__iOSSESDKSetRemoteConfigUserProperties, error :%@",msg);
                    return;
                }
            }
        }

        id remoteConfig = getRemoteConfigInstance();
        if (!remoteConfig) return;
        
        SEL selector = NSSelectorFromString(@"setRemoteConfigUserProperties:");
        safeCallRemoteConfigMethod(remoteConfig, selector, dict);
    }

    char * __iOSSESDKFastFetchRemoteConfig(const char *key) {
        NSString *keyString = [NSString stringWithUTF8String:key];
        
        id remoteConfig = getRemoteConfigInstance();
        if (!remoteConfig) return NULL;
        
        SEL selector = NSSelectorFromString(@"fastFetchRemoteConfig:");
        id data = safeCallRemoteConfigMethod(remoteConfig, selector, keyString);
        
        NSString *msg = [NSString stringWithFormat:@"%@", data];
        
        if ([data isKindOfClass:[NSDictionary class]] || [data isKindOfClass:[NSArray class]]) {
            NSData *d = [NSJSONSerialization dataWithJSONObject:data options:0 error:nil];
            if (d) {
                msg = [[NSString alloc] initWithData:d encoding:NSUTF8StringEncoding];
            }
        }
        
        return reteinStr([msg cStringUsingEncoding:NSUTF8StringEncoding]);
    }
    
    void __iOSSESDKAsyncFetchRemoteConfig(const char * key , FetchRemoteConfigCallback callback) {
        NSString *keyString = [NSString stringWithUTF8String:key];
        
        id remoteConfig = getRemoteConfigInstance();
        if (!remoteConfig) return;
        
        SEL selector = NSSelectorFromString(@"asyncFetchRemoteConfig:completionHandler:");
        safeCallRemoteConfigMethod(remoteConfig, selector, keyString, ^(id  _Nonnull data) {
            NSString *msg = [NSString stringWithFormat:@"%@", data];
            if ([data isKindOfClass:[NSDictionary class]] || [data isKindOfClass:[NSArray class]]) {
                NSData *d = [NSJSONSerialization dataWithJSONObject:data options:0 error:nil];
                if (d) {
                    msg = [[NSString alloc] initWithData:d encoding:NSUTF8StringEncoding];
                }
            }
            callback([msg cStringUsingEncoding:NSUTF8StringEncoding]);
        });
    }

    char * __iOSSESDKFastFetchAllRemoteConfig() {
        id remoteConfig = getRemoteConfigInstance();
        if (!remoteConfig) return NULL;
        
        SEL selector = NSSelectorFromString(@"fastFetchRemoteConfig");
        NSDictionary *dict = safeCallRemoteConfigMethod(remoteConfig, selector);
        
        NSData *data = [NSJSONSerialization dataWithJSONObject:dict options:0 error:nil];
        if (data == nil) {
            return NULL;
        }
        NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
        return reteinStr([dataString cStringUsingEncoding:NSUTF8StringEncoding]);
    }

    void __iOSSESDKAsyncFetchAllRemoteConfig(FetchRemoteConfigCallback callback) {
        id remoteConfig = getRemoteConfigInstance();
        if (!remoteConfig) return;
        
        SEL selector = NSSelectorFromString(@"asyncFetchRemoteConfigWithCompletionHandler:");
        safeCallRemoteConfigMethod(remoteConfig, selector, ^(NSDictionary * _Nonnull dict) {
            if (dict) {
                NSData *data = [NSJSONSerialization dataWithJSONObject:dict options:0 error:nil];
                if (data != nil) {
                    NSString *dataString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
                    callback(reteinStr([dataString cStringUsingEncoding:NSUTF8StringEncoding]));
                } else {
                    callback(NULL);
                }
            } else {
                callback(NULL);
            }
        });
    }

}
