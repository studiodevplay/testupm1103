//
//  SEConfigForCN.h
//  SolarEngineSDK
//
//  Created by Mobvista on 2023/9/14.
//

#import <Foundation/Foundation.h>
#import "SEConfig.h"

@interface SEConfig (ForCN)

/// caid的格式为json字符串，示例如下：
/*
 [
     {
        "version":"20220111",
        "caid":"912ec803b2ce49e4a541068d495ab570"
    },
    {
        "version":"20211207",
        "caid":"e332a76c29654fcb7f6e6b31ced090c7"
    }
 ]
*/

/// 移动互联网广告标识，开发者需要自行获取 caid 后在初始化 sdk 时传入，该属性为非必传属性
@property (nonatomic, copy) NSString *caid;

@end
