//
//  UnityBridge.h
//  CocoaHelpers
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Thoughtworks. All rights reserved.
//

#import "LocationManager.h"
#import "CameraManager.h"

#ifdef __cplusplus
extern "C" {
#endif
  // LocationManager Interface
  typedef void (*CLAuthorizationStatusCallback)(CLAuthorizationStatus
                                  status);
  CLAuthorizationStatus framework_locationManager_getAuthorizationStatus();
  void framework_locationManager_requestAuthorizationStatus();
  void framework_locationManager_setCallback(CLAuthorizationStatusCallback callback);

// CameraManager Interface
  typedef void (*AVAuthorizationStatusCallback)(AVAuthorizationStatus
                                    status);

  AVAuthorizationStatus framework_cameraManager_getAuthorizationStatus(void);
  void framework_cameraManager_requestAuthorizationStatus(void);
  void framework_cameraManager_setCallback(AVAuthorizationStatusCallback callback);

// Application Interface
  void framework_application_openAppSettings();

#ifdef __cplusplus
}
#endif

