//
//  LocationManager.m
//  CocoaHelpers
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Thoughtworks. All rights reserved.
//

#import "LocationManager.h"


@interface LocationManager ()<CLLocationManagerDelegate>
@property (strong, atomic) CLLocationManager *cllocManager;
@property (nonatomic) CLAuthorizationStatus authStatus;
@property (strong, atomic) void (^callback)(CLAuthorizationStatus);

+ (LocationManager*)sharedInstance;
- (CLAuthorizationStatus)getLocationAuthorizationStatus;
- (void)requestLocationAuthorizationStatus;
@end

@implementation LocationManager


#pragma mark - Init Concerns

+ (LocationManager *)sharedInstance {
  static LocationManager *locManager;
  static dispatch_once_t onceToken;
  dispatch_once(&onceToken, ^{
    locManager = [[LocationManager alloc] init];
  });
  return locManager;
}

- (instancetype)init {
  self = [super init];
  if (self) {
    self.cllocManager = [[CLLocationManager alloc] init];
    self.cllocManager.delegate = self;
    self.authStatus = kCLAuthorizationStatusNotDetermined;
  }
  return self;
}

#pragma mark - Public Class Facade

+ (CLAuthorizationStatus)getLocationAuthorizationStatus {
  return [[LocationManager sharedInstance] getLocationAuthorizationStatus];
}

+ (void)requestLocationAuthorizationStatus {
  [[LocationManager sharedInstance] requestLocationAuthorizationStatus];
}

+ (void)setCallback:(void (^)(CLAuthorizationStatus))callback {
  [[LocationManager sharedInstance] setCallback:callback];
}


#pragma mark - Instance Methods

- (CLAuthorizationStatus)getLocationAuthorizationStatus {
  return self.authStatus;
}

- (void)requestLocationAuthorizationStatus {
  [self.cllocManager requestWhenInUseAuthorization];
}


#pragma mark - CLLocationManagerDelegate

- (void)locationManager:(CLLocationManager *)manager didChangeAuthorizationStatus:(CLAuthorizationStatus)status {
  if (self.callback) {
    self.callback(status);
  }
}


@end
