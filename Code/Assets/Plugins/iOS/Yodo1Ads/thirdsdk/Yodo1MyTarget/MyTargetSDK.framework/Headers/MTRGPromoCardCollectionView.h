//
//  MTRGPromoCardCollectionView.h
//  myTargetSDK 5.4.8
//
//  Created by Andrey Seredkin on 02.11.16.
//  Copyright © 2016 Mail.ru Group. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MTRGNativePromoCard.h"
#import "MTRGPromoCardSliderProtocol.h"

NS_ASSUME_NONNULL_BEGIN

@interface MTRGPromoCardCollectionView : UICollectionView <MTRGPromoCardSliderProtocol, UICollectionViewDelegate, UICollectionViewDataSource, UICollectionViewDelegateFlowLayout>

@property (nonatomic, weak, nullable) id <MTRGPromoCardSliderDelegate> cardSliderDelegate;
@property (nonatomic) BOOL isCardsClickable;

+ (instancetype)create;

- (void)setCards:(NSArray<MTRGNativePromoCard *> *)cards;

@end

NS_ASSUME_NONNULL_END
