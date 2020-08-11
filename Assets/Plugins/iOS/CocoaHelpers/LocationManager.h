//
//  LocationManager.h
//  CocoaHelpers
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Thoughtworks. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

NS_ASSUME_NONNULL_BEGIN

@interface LocationManager : NSObject
+ (CLAuthorizationStatus)getLocationAuthorizationStatus;
+ (void)requestLocationAuthorizationStatus;
+ (void)setCallback:(void (^)(CLAuthorizationStatus))callback;
@end

NS_ASSUME_NONNULL_END
