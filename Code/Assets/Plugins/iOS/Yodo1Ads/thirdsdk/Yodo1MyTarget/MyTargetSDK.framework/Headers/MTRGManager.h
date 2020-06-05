//
//  MTRGManager.h
//  myTargetSDK 5.4.8
//
//  Created by Anton Bulankin on 18.09.15.
//  Copyright © 2015 Mail.ru. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface MTRGManager : NSObject

+ (NSDictionary<NSString *, NSString *> *)getFingerprintParams;

+ (void)trackUrl:(NSString *)url;

@end

NS_ASSUME_NONNULL_END
