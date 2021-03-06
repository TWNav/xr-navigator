﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
#endif

public class PermissionHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;

    void Start()
    {
        settingsPanel.SetActive(false);
        CheckPermissions();        
    }

#if UNITY_ANDROID
    string[] permissions = { Permission.Camera , Permission.FineLocation} ;
    public void RequestPermissions()
    {
        AndroidRuntimePermissions.Permission[] results = new AndroidRuntimePermissions.Permission[permissions.Length];

        for (int i = 0; i < permissions.Length; i++)
        {
            Log.debug($"Requesting access to {permissions[i]}");
            results[i] = AndroidRuntimePermissions.RequestPermission(permissions[i]);
        }

        ArrayList deniedPermission = new ArrayList();
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] != AndroidRuntimePermissions.Permission.Granted)
            {
                deniedPermission.Add(permissions[i]);
            }
        }

        if (deniedPermission.Count > 0)
        {
           ShowPermissionsNeededPanel();
        }
        else
        {
            SwitchScene();
            Log.debug("Permissions Granted");
        }
    }

    private void CheckPermissions()
    {
        var lat = Input.location.lastData.latitude;
        AndroidRuntimePermissions.Permission[] checkedPermissions = AndroidRuntimePermissions.CheckPermissions(permissions);
        bool granted = true;
        for (int i = 0; i < checkedPermissions.Length; i++)
        {
            if (checkedPermissions[i] != AndroidRuntimePermissions.Permission.Granted)
            {
                granted = false;
                break;
            }
        }
        if (granted)
        {
            SwitchScene();
        }
    }

    public void OpenSettings()
    {
        AndroidRuntimePermissions.OpenSettings();
        settingsPanel.SetActive(false);
    }

#elif UNITY_IOS

    private CLAuthorizationStatus locationAuthorizationStatus {
        get {
            return CocoaHelpersBridge.GetLocationAuthorizationStatus();
        }
    }
    private AVAuthorizationStatus avAuthorizationStatus {
        get {
            return CocoaHelpersBridge.GetCameraAuthorizationStatus();
        }
    }

    private bool accessAuthorized {
        get {
            return avAuthorizationStatus == AVAuthorizationStatus.Authorized && 
            (locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedAlways || locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedWhenInUse);
        }
    }

    static LocationAuthorizationDelegate locationDelegate;
    static AVAuthorizationDelegate aVAuthorizationDelegate;

    public void OpenSettings() {
        CocoaHelpersBridge.OpenAppSettings();
    }

    private void SetDelegatesAndCallbacks() {
        locationDelegate = LocationCallback;
        aVAuthorizationDelegate = AVCallback;
        CocoaHelpersBridge.SetLocationAuthorizationCallback(LocationAuthCallback);
        CocoaHelpersBridge.SetCameraAuthorizationCallback(AVAuthCallback);
        
    }

    private void CheckPermissions() {
        if (accessAuthorized) {
            Log.debug("Authorized for access to system permissions");
            SwitchScene();
        }
    }
    
    public void RequestPermissions() {
        SetDelegatesAndCallbacks();
        Log.debug($"Location Permission {locationAuthorizationStatus}");
        switch (locationAuthorizationStatus)
        {
            case CLAuthorizationStatus.NotDetermined:                
                Log.debug("Requesting Permissions Location");
                CocoaHelpersBridge.RequestAuthorizationStatusLocation();
                return;
                
            case CLAuthorizationStatus.Denied:
                ShowPermissionsNeededPanel();
                return;
            default:
                break;
        }

        Log.debug("Requesting Permissions Camera");

   
        Log.debug($"Camera Permission {avAuthorizationStatus}");
        switch (avAuthorizationStatus) {
            case AVAuthorizationStatus.NotDetermined:   
              Log.debug("Requesting Permissions Camera");             
                CocoaHelpersBridge.RequestAuthorizationStatusCamera();
                return;
            case AVAuthorizationStatus.Denied:
                ShowPermissionsNeededPanel();
                return;
            default:
                break;
        }
        CheckPermissions();
    }
    
    private void LocationCallback(CLAuthorizationStatus status) {
        Log.debug($"Location Callback received {status}");
        RequestPermissions();
    } 

    [MonoPInvokeCallback(typeof(LocationAuthorizationDelegate))]
    public static void LocationAuthCallback(CLAuthorizationStatus status) {
        if (locationDelegate != null) {
            locationDelegate(status);
        }
    }

     public void AVCallback(AVAuthorizationStatus status) { 
        Log.debug($"Camera Callback received {status}");
        RequestPermissions();
     }

    [MonoPInvokeCallback(typeof(AVAuthorizationDelegate))]
    public static void AVAuthCallback(AVAuthorizationStatus status) {
        if (locationDelegate != null) {
            aVAuthorizationDelegate(status);
        }
    }


#endif

    private void ShowPermissionsNeededPanel() {
        Log.debug("ShowPermissionsNeededPanel");
         settingsPanel.SetActive(true);      
    }
    private void SwitchScene()
    {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
