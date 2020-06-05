//
//  SMAAdRequestParams.h
//  SmaatoSDKCore
//
//  Created by Smaato Inc on 01.11.19.
//  Copyright © 2019 Smaato Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 Optional parameters for advanced ad requests.
*/
@interface SMAAdRequestParams : NSObject

/// Unified Bidding unique identifier (see Smaato's Unified Bidding Publisher Setup (iOS))
@property (nonatomic, copy, nullable) NSString *ubUniqueId;

/**
 An adapter class name from where ad request originates. Default is \c nil

 @warning       The parameter should only be passed when Smaato SDK is secondary.
 The parameter will serve as an additional information to Smaato if any prompt support should be required for your
 application.
 */
@property (nonatomic, nullable, copy) NSString *mediationNetworkName;

/**
 An adapter version number from where ad request originates. Default is \c nil

 @warning       The parameter should only be passed when Smaato SDK is secondary.
 The parameter will serve as an additional information to Smaato if any prompt support should be required for your
 application.
 */
@property (nonatomic, nullable, copy) NSString *mediationAdapterVersion;

/**
 Third party SDK version for mediated ads. Default is \c nil

 @warning       The parameter should only be passed when Smaato SDK is secondary.
 The parameter will serve as an additional information to Smaato if any prompt support should be required for your
 application.
 */
@property (nonatomic, nullable, copy) NSString *mediationNetworkSDKVersion;

@end
