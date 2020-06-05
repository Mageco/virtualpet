//
//  myTargetSDK 5.4.8
//
// Created by Timur on 3/5/18.
// Copyright (c) 2018 MailRu Group. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MTRGBaseAd.h"

@class MTRGInterstitialAd;

NS_ASSUME_NONNULL_BEGIN

@protocol MTRGInterstitialAdDelegate <NSObject>

- (void)onLoadWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd;

- (void)onNoAdWithReason:(NSString *)reason interstitialAd:(MTRGInterstitialAd *)interstitialAd;

@optional

- (void)onClickWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd;

- (void)onCloseWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd;

- (void)onVideoCompleteWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd;

- (void)onDisplayWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd;

- (void)onLeaveApplicationWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd;

@end

@interface MTRGInterstitialAd : MTRGBaseAd

@property(nonatomic) BOOL mediationEnabled;
@property(nonatomic, weak, nullable) id <MTRGInterstitialAdDelegate> delegate;
@property(nonatomic, readonly, nullable) NSString *adSource;

+ (instancetype)interstitialAdWithSlotId:(NSUInteger)slotId;

- (instancetype)initWithSlotId:(NSUInteger)slotId;

- (void)load;

- (void)loadFromBid:(NSString *)bidId;

- (void)showWithController:(UIViewController *)controller;

- (void)showModalWithController:(UIViewController *)controller;

- (void)close;

@end

NS_ASSUME_NONNULL_END
