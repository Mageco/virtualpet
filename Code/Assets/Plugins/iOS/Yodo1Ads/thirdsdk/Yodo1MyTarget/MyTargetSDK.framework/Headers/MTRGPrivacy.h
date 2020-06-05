//
//  MTRGPrivacy.h
//  myTargetSDK 5.4.8
//
//  Created by Andrey Seredkin on 28.05.2018.
//  Copyright © 2018 Mail.Ru Group. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface MTRGPrivacy : NSObject

+ (BOOL)isConsentSpecified;

+ (BOOL)userConsent;

+ (void)setUserConsent:(BOOL)userConsent;

+ (BOOL)userAgeRestricted;

+ (void)setUserAgeRestricted:(BOOL)userAgeRestricted;

@end

NS_ASSUME_NONNULL_END
