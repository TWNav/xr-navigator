//
//  ContentView.swift
//  PermissionsExample
//
//  Created by Shaun Hubbard on 8/11/20.
//  Copyright © 2020 Shaun Hubbard. All rights reserved.
//

import SwiftUI
import AVKit

struct ContentView: View {

  @EnvironmentObject var locationManager: LocationManager

  @State var cameraAuthorized: Bool = false

    var body: some View {
      VStack {
        Spacer()
        Button(action: self.requestLocationPermission, label: { Text("Request Location Permission") })
        Text("Location Permission \(locationAuthStatus)")

        Spacer()
        Button(action: self.requestCameraPermission, label: { Text("Request Camera Permission") })
        Text("Camera Permission Status \(cameraAuthStatus)")
        Spacer()
      }
    }

  private func requestLocationPermission() {
    locationManager.requestPermission()
  }

  private func requestCameraPermission() {
    AVCaptureDevice.requestAccess(for: .video) { (authorizied) in
      self.cameraAuthorized = authorizied
    }
  }

  private var locationAuthStatus: String {
    switch locationManager.authStatus {
    case .authorizedAlways:
      return "Always"
    case .authorizedWhenInUse:
      return "When In Use"
    case .denied:
      return "Denied"
    case .notDetermined:
      return "Not Determined"
    case .restricted:
      return "Restricted"
    }
  }

  private var cameraAuthStatus: String {
    if cameraAuthorized {
      return "YES"
    } else {
      return "NO"
    }
  }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        ContentView()
    }
}
