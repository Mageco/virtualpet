//
//  myTargetSDK 5.4.8
//
// Created by Timur on 3/22/18.
// Copyright (c) 2018 Mail.Ru Group. All rights reserved.
//

#import <UIKit/UIKit.h>

@class MTRGAdView;
@class MTRGCustomParams;

NS_ASSUME_NONNULL_BEGIN

typedef enum : NSUInteger
{
	MTRGAdSize_320x50 = 0,
	MTRGAdSize_300x250 = 1,
	MTRGAdSize_728x90 = 2
} MTRGAdSize;

@protocol MTRGAdViewDelegate <NSObject>

- (void)onLoadWithAdView:(MTRGAdView *)adView;

- (void)onNoAdWithReason:(NSString *)reason adView:(MTRGAdView *)adView;

@optional

- (void)onAdClickWithAdView:(MTRGAdView *)adView;

- (void)onAdShowWithAdView:(MTRGAdView *)adView;

- (void)onShowModalWithAdView:(MTRGAdView *)adView;

- (void)onDismissModalWithAdView:(MTRGAdView *)adView;

- (void)onLeaveApplicationWithAdView:(MTRGAdView *)adView;

@end

@interface MTRGAdView : UIView

@property(nonatomic, weak, nullable) id <MTRGAdViewDelegate> delegate;
@property(nonatomic, weak, nullable) UIViewController *viewController;
@property(nonatomic, readonly) MTRGCustomParams *customParams;
@property(nonatomic) BOOL trackLocationEnabled;
@property(nonatomic) BOOL mediationEnabled;
@property(nonatomic, readonly) MTRGAdSize adSize;
@property(nonatomic, readonly, nullable) NSString *adSource;

+ (void)setDebugMode:(BOOL)enabled;

+ (BOOL)isDebugMode;

+ (instancetype)adViewWithSlotId:(NSUInteger)slotId;

+ (instancetype)adViewWithSlotId:(NSUInteger)slotId adSize:(MTRGAdSize)adSize;

+ (instancetype)adViewWithSlotId:(NSUInteger)slotId withRefreshAd:(BOOL)refreshAd adSize:(MTRGAdSize)adSize;

- (instancetype)initWithSlotId:(NSUInteger)slotId;

- (instancetype)initWithSlotId:(NSUInteger)slotId adSize:(MTRGAdSize)adSize;

- (instancetype)initWithSlotId:(NSUInteger)slotId withRefreshAd:(BOOL)refreshAd adSize:(MTRGAdSize)adSize;

- (void)load;

- (void)loadFromBid:(NSString *)bidId;

@end

NS_ASSUME_NONNULL_END
