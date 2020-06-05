//
//  myTargetSDK 5.4.8
//
// Created by Timur on 2/12/18.
// Copyright (c) 2018 Mail.Ru Group. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MyTargetSDK/MTRGNavigationType.h>

@class MTRGImageData;
@class MTRGNativePromoCard;

NS_ASSUME_NONNULL_BEGIN

@interface MTRGNativePromoBanner : NSObject

@property(nonatomic, readonly, copy, nullable) NSString *advertisingLabel;
@property(nonatomic, readonly, copy, nullable) NSString *ageRestrictions;
@property(nonatomic, readonly, copy, nullable) NSString *title;
@property(nonatomic, readonly, copy, nullable) NSString *descriptionText;
@property(nonatomic, readonly, copy, nullable) NSString *disclaimer;
@property(nonatomic, readonly, copy, nullable) NSString *category;
@property(nonatomic, readonly, copy, nullable) NSString *subcategory;
@property(nonatomic, readonly, copy, nullable) NSString *domain;
@property(nonatomic, readonly, copy, nullable) NSString *ctaText;
@property(nonatomic, readonly, nullable) NSNumber *rating;
@property(nonatomic, readonly) NSUInteger votes;
@property(nonatomic, readonly) MTRGNavigationType navigationType;
@property(nonatomic, readonly, nullable) MTRGImageData *icon;
@property(nonatomic, readonly, nullable) MTRGImageData *image;
@property(nonatomic, readonly) NSArray<MTRGNativePromoCard *> *cards;
@property(nonatomic, readonly) BOOL hasVideo;

@end

NS_ASSUME_NONNULL_END
