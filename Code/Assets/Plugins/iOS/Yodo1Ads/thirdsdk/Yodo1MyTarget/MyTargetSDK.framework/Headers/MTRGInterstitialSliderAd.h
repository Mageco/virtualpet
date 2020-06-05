//
//  myTargetSDK 5.4.8
//
// Created by Timur on 3/16/18.
// Copyright (c) 2018 Mail.Ru Group. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MTRGBaseAd.h"

@class MTRGInterstitialSliderAd;

NS_ASSUME_NONNULL_BEGIN

@protocol MTRGInterstitialSliderAdDelegate <NSObject>

- (void)onLoadWithInterstitialSliderAd:(MTRGInterstitialSliderAd *)interstitialSliderAd;

- (void)onNoAdWithReason:(NSString *)reason interstitialSliderAd:(MTRGInterstitialSliderAd *)interstitialSliderAd;

@optional

- (void)onClickWithInterstitialSliderAd:(MTRGInterstitialSliderAd *)interstitialSliderAd;

- (void)onCloseWithInterstitialSliderAd:(MTRGInterstitialSliderAd *)interstitialSliderAd;

- (void)onDisplayWithInterstitialSliderAd:(MTRGInterstitialSliderAd *)interstitialSliderAd;

- (void)onLeaveApplicationWithInterstitialSliderAd:(MTRGInterstitialSliderAd *)interstitialSliderAd;

@end

@interface MTRGInterstitialSliderAd : MTRGBaseAd

@property(nonatomic, weak, nullable) id <MTRGInterstitialSliderAdDelegate> delegate;

+ (instancetype)interstitialSliderAdWithSlotId:(NSUInteger)slotId;

- (instancetype)initWithSlotId:(NSUInteger)slotId;

- (void)load;

- (void)showWithController:(UIViewController *)controller;

- (void)showModalWithController:(UIViewController *)controller;

- (void)close;

@end

NS_ASSUME_NONNULL_END
