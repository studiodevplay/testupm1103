//
//  SEBridgeCommon.m
//  SolarEngineSDK
//
//  Created by MVP on 2022/1/24.
//

#import "SEBridgeCommon.h"

// Custom logging function with prefix
void se_innerLog(NSString *format, ...) {
    va_list args;
    va_start(args, format);
    NSString *message = [[NSString alloc] initWithFormat:format arguments:args];
    va_end(args);
    
    NSString *prefixedMessage = [NSString stringWithFormat:@"[SolarEngineBridge]: %@", message];
    NSLog(@"%@", prefixedMessage);
} 