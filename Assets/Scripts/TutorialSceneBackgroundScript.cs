using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Runtime.InteropServices;
using AOT;

#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
#endif

public class TutorialSceneBackgroundScript : MonoBehaviour
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
            DebugLog($"Requesting access to {permissions[i]}");
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
            Debug.Log("Permissions Granted");
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
            DebugLog("Authorized for access to system permissions");
            SwitchScene();
        }
    }
    
    public void RequestPermissions() {
        SetDelegatesAndCallbacks();
        DebugLog($"Location Permission {locationAuthorizationStatus}");
        switch (locationAuthorizationStatus)
        {
            case CLAuthorizationStatus.NotDetermined:                
                DebugLog("Requesting Permissions Location");
                CocoaHelpersBridge.RequestAuthorizationStatusLocation();
                return;
                
            case CLAuthorizationStatus.Denied:
                ShowPermissionsNeededPanel();
                return;
            default:
                break;
        }

        DebugLog("Requesting Permissions Camera");

   
        DebugLog($"Camera Permission {avAuthorizationStatus}");
        switch (avAuthorizationStatus) {
            case AVAuthorizationStatus.NotDetermined:   
              DebugLog("Requesting Permissions Camera");             
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
        DebugLog($"Location Callback received {status}");
        RequestPermissions();
    } 

    [MonoPInvokeCallback(typeof(LocationAuthorizationDelegate))]
    public static void LocationAuthCallback(CLAuthorizationStatus status) {
        if (locationDelegate != null) {
            locationDelegate(status);
        }
    }

     public void AVCallback(AVAuthorizationStatus status) { 
        DebugLog($"Camera Callback received {status}");
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
        DebugLog("ShowPermissionsNeededPanel");
         settingsPanel.SetActive(true);      
    }
    private void SwitchScene()
    {
        SceneManager.LoadScene("TWNavigatorScene");
    }


    private static void DebugLog(string message) {
        Debug.Log($"D/{message}");
    }
}
