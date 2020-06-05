//
//  MTRGNativeAdLoader.h
//  myTargetSDK 5.4.8
//
//  Created by Andrey Seredkin on 31.05.2018.
//  Copyright © 2018 Mail.Ru Group. All rights reserved.
//

#import "MTRGNativeAd.h"

NS_ASSUME_NONNULL_BEGIN

@interface MTRGNativeAdLoader : MTRGBaseAd

@property(nonatomic) BOOL autoLoadImages;
@property(nonatomic) BOOL autoLoadVideo;
@property(nonatomic) MTRGAdChoicesPlacement adChoicesPlacement;

+ (instancetype)loaderForCount:(NSUInteger)count slotId:(NSUInteger)slotId;

- (instancetype)init NS_UNAVAILABLE;

- (void)loadWithCompletionBlock:(void (^)(NSArray<MTRGNativeAd *> *nativeAds))completionBlock;

@end

NS_ASSUME_NONNULL_END
