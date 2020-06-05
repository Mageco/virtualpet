//
//  myTargetSDK 5.4.8
//
// Created by Timur on 2/1/18.
// Copyright (c) 2018 Mail.Ru Group. All rights reserved.
//

#import <Foundation/Foundation.h>

@class MTRGCustomParams;

NS_ASSUME_NONNULL_BEGIN

@interface MTRGBaseAd : NSObject

@property(nonatomic, readonly) MTRGCustomParams *customParams;
@property(nonatomic) BOOL trackLocationEnabled;

+ (void)setDebugMode:(BOOL)enabled;

+ (BOOL)isDebugMode;

@end

NS_ASSUME_NONNULL_END
