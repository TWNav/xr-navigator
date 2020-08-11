//
//  CameraManager.h
//  CocoaHelpers
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Thoughtworks. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVKit/AVKit.h>

NS_ASSUME_NONNULL_BEGIN

@interface CameraManager : NSObject
+ (AVAuthorizationStatus)getAuthorizationStatus;
+ (void)requestAuthorizationStatus;
+ (void)setCallback:(void (^)(AVAuthorizationStatus))callback;
@end

NS_ASSUME_NONNULL_END
