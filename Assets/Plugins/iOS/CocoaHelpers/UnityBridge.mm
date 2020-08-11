//
//  UnityBridge.m
//  CocoaHelpers
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Thoughtworks. All rights reserved.
//

#import "UnityBridge.h"

// LocationManager Interface
CLAuthorizationStatus framework_locationManager_getAuthorizationStatus() {
  return [LocationManager getLocationAuthorizationStatus];
}

void framework_locationManager_requestAuthorizationStatus() {
  [LocationManager requestLocationAuthorizationStatus];
}

void framework_locationManager_setCallback(CLAuthorizationStatusCallback callback) {
  [LocationManager setCallback:^(CLAuthorizationStatus status) {
    callback(status);
  }];
}


// CameraManager Interface
AVAuthorizationStatus framework_cameraManager_getAuthorizationStatus() {
  return [CameraManager getAuthorizationStatus];
}


void framework_cameraManager_requestAuthorizationStatus() {
  return [CameraManager requestAuthorizationStatus];
}

void framework_cameraManager_setCallback(AVAuthorizationStatusCallback callback) {
  [CameraManager setCallback:^(AVAuthorizationStatus status) {
    callback(status);
  }];
}
