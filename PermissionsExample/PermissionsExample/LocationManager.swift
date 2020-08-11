//
//  LocationManager.swift
//  PermissionsExample
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Shaun Hubbard. All rights reserved.
//

import Foundation
import CoreLocation


@objc class LocationManager: NSObject, CLLocationManagerDelegate, ObservableObject {
  private let locManager = CLLocationManager()

  override init() {
    super.init()
    locManager.delegate = self
  }

  func requestPermission() {
    locManager.requestWhenInUseAuthorization()
  }


  func locationManager(_ manager: CLLocationManager, didChangeAuthorization status: CLAuthorizationStatus) {
    objectWillChange.send()
    authStatus = status
  }

  var authStatus: CLAuthorizationStatus = .notDetermined
}
