//
//  MTRGNativeViewsFactory.h
//  myTargetSDK 5.4.8
//
//  Created by Anton Bulankin on 17.11.14.
//  Copyright (c) 2014 Mail.ru Group. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "MTRGNativePromoBanner.h"
#import "MTRGNewsFeedAdView.h"
#import "MTRGChatListAdView.h"
#import "MTRGContentStreamAdView.h"
#import "MTRGContentWallAdView.h"
#import "MTRGMediaAdView.h"
#import "MTRGContentStreamCardAdView.h"
#import "MTRGPromoCardCollectionView.h"

NS_ASSUME_NONNULL_BEGIN

@interface MTRGNativeViewsFactory : NSObject

+ (MTRGNewsFeedAdView *)createNewsFeedViewWithBanner:(MTRGNativePromoBanner *)promoBanner;

+ (MTRGChatListAdView *)createChatListViewWithBanner:(MTRGNativePromoBanner *)promoBanner;

+ (MTRGContentStreamAdView *)createContentStreamViewWithBanner:(MTRGNativePromoBanner *)promoBanner;

+ (MTRGContentWallAdView *)createContentWallViewWithBanner:(MTRGNativePromoBanner *)promoBanner;

+ (MTRGMediaAdView *)createMediaAdView;

+ (MTRGContentStreamCardAdView *)createContentStreamCardAdView;

+ (MTRGPromoCardCollectionView *)createPromoCardCollectionView;

@end

NS_ASSUME_NONNULL_END
