//
// Copyright (c) 2019 Mail.Ru Group. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MTRGCustomParams.h"

NS_ASSUME_NONNULL_BEGIN

@interface MTRGMediationAdConfig : NSObject

@property(nonatomic, readonly, copy) NSString *placementId;
@property(nonatomic, readonly, copy, nullable) NSString *payload;
@property(nonatomic, readonly) NSDictionary<NSString *, NSString *> *serverParams;
@property(nonatomic, readonly, nullable) NSNumber *age;
@property(nonatomic, readonly) MTRGGender gender;
@property(nonatomic, readonly) BOOL userConsentSpecified;
@property(nonatomic, readonly) BOOL userConsent;
@property(nonatomic, readonly) BOOL userAgeRestricted;
@property(nonatomic, readonly) BOOL trackLocationEnabled;

+ (instancetype)configWithPlacementId:(NSString *)placementId
                              payload:(nullable NSString *)payload
                         serverParams:(NSDictionary<NSString *, NSString *> *)serverParams
                                  age:(nullable NSNumber *)age
                               gender:(MTRGGender)gender
                 userConsentSpecified:(BOOL)userConsentSpecified
                          userConsent:(BOOL)userConsent
                    userAgeRestricted:(BOOL)userAgeRestricted
                 trackLocationEnabled:(BOOL)trackLocationEnabled;

- (instancetype)init NS_UNAVAILABLE;

@end

NS_ASSUME_NONNULL_END