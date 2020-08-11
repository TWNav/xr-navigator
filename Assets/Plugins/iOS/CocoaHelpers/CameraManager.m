//
//  CameraManager.m
//  CocoaHelpers
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Thoughtworks. All rights reserved.
//

#import "CameraManager.h"

@interface CameraManager ()
@property (strong, atomic) void (^callback)(AVAuthorizationStatus);

+ (CameraManager*)sharedInstance;
- (AVAuthorizationStatus)getLocationAuthorizationStatus;
- (void)requestLocationAuthorizationStatus;
@end

@implementation CameraManager
#pragma mark - Init Concerns

+ (CameraManager *)sharedInstance {
  static CameraManager *locManager;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{
    locManager = [[CameraManager alloc] init];
  });
  return locManager;
}

#pragma mark - Public Class Facade

+ (AVAuthorizationStatus)getAuthorizationStatus {
  return [[CameraManager sharedInstance] getLocationAuthorizationStatus];
}

+ (void)requestAuthorizationStatus {
  [[CameraManager sharedInstance] requestLocationAuthorizationStatus];
}

+ (void)setCallback:(void (^)(AVAuthorizationStatus))callback {
  [[CameraManager sharedInstance] setCallback:callback];
}

#pragma mark - Instance Methods
- (AVAuthorizationStatus)getLocationAuthorizationStatus {
  return [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
}

- (void)requestLocationAuthorizationStatus {
  [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
    if (!self.callback) {
      return;
    }
    self.callback([self getLocationAuthorizationStatus]);
  }];
}

@end
