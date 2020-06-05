//
//  Yodo1SaManager.h
//  SaSDKDemo
//
//  Created by yixian huang on 2020/3/26.
//  Copyright © 2020 yixian huang. All rights reserved.
//

#ifndef Yodo1SaManager_h
#define Yodo1SaManager_h
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface Yodo1SaManager : NSObject

/**
 init Sa SDK,debugMode:0 close debug,
 1 is debug,2 is debug and data import
 */
+ (void)initializeSdkServerURL:(NSString *)serverURL
                         debug:(int) debugMode;

/**
 Super properties
 */
+ (void)registerSuperProperties:(NSDictionary *)properties;

/**
 设置事件
 */
+ (void)track:(NSString *)eventId properties:(nullable NSDictionary *)properties;

/**
 记录激活属性
 */
+ (void)trackInstallation:(NSDictionary *)properties;

/**
 保留初次属性
 */
+ (void)profileSetOnce:(NSDictionary *)properties;

/**
 属性更新
 */
+ (void)profileSet:(NSDictionary *)properties;

@end

NS_ASSUME_NONNULL_END
#endif /* Yodo1SaManager_h */
