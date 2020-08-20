using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

// TODO: Turn int into enum

#if UNITY_IOS
public enum CLAuthorizationStatus { 
    NotDetermined = 0, 
    Restricted = 1, 
    Denied = 2, 
    AuthorizedAlways = 3,
    AuthorizedWhenInUse = 4,
    Unknown
};


public delegate void LocationAuthorizationDelegate(CLAuthorizationStatus status);


public enum AVAuthorizationStatus { 
    NotDetermined = 0, 
    Restricted = 1, 
    Denied = 2, 
    Authorized = 3,
    Unknown
};

public delegate void AVAuthorizationDelegate(AVAuthorizationStatus status);

//public delegate void AuthorizationDelegate(int status);

#endif


public class CocoaHelpersBridge : MonoBehaviour
{

    #if UNITY_IOS
    [DllImport("__Internal")]
    private static extern CLAuthorizationStatus framework_locationManager_getAuthorizationStatus();

    [DllImport("__Internal")]
    private static extern void framework_locationManager_requestAuthorizationStatus();

    [DllImport("__Internal")]
    private static extern void framework_locationManager_setCallback(LocationAuthorizationDelegate del);

    [DllImport("__Internal")]
    private static extern AVAuthorizationStatus framework_cameraManager_getAuthorizationStatus();
      
    [DllImport("__Internal")]
    private static extern void framework_cameraManager_requestAuthorizationStatus();
    [DllImport("__Internal")]
    private static extern void framework_cameraManager_setCallback(AVAuthorizationDelegate del);

    [DllImport("__Internal")]
    private static extern void framework_application_openAppSettings();
    

    // CLLocation Interop


    public static CLAuthorizationStatus GetLocationAuthorizationStatus() {
        #if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {             
            DebugLog("Called");
            return framework_locationManager_getAuthorizationStatus();
        }
        #endif
        return CLAuthorizationStatus.Unknown;
    }

    public static void RequestAuthorizationStatusLocation() {
        #if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {             
            DebugLog("Called");
            framework_locationManager_requestAuthorizationStatus ();
        }
        #endif
    }

    public static void SetLocationAuthorizationCallback(LocationAuthorizationDelegate del) {
        #if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {             
            DebugLog("Called");
            framework_locationManager_setCallback (del);
        }
        #endif
    }


    // AVKit Interop

    public static AVAuthorizationStatus GetCameraAuthorizationStatus() {
        #if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {             
            DebugLog("Called");
            return framework_cameraManager_getAuthorizationStatus();
        }
        #endif
        return AVAuthorizationStatus.Unknown;
    }

    public static void RequestAuthorizationStatusCamera() {
        #if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {             
            DebugLog("Called");
            framework_cameraManager_requestAuthorizationStatus ();
        }
        #endif
    }

    public static void SetCameraAuthorizationCallback(AVAuthorizationDelegate del) {
        #if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {             
            DebugLog("Called");
            framework_cameraManager_setCallback (del);
        }
        #endif
    }

    public static void OpenAppSettings() {
        #if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer) {             
            DebugLog("Called");
            framework_application_openAppSettings();
        }
        #endif
    }


    private static void DebugLog(string message) {
        var st = new StackTrace();
        var sf = st.GetFrame(1);
        Log.debug($"D/{sf}->{message}");
    }
    #endif
}
