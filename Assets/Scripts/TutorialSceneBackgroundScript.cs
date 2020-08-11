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
    string[] permissions = { Permission.Camera };
    public void RequestPermissions()
    {
        AndroidRuntimePermissions.Permission[] results = new AndroidRuntimePermissions.Permission[permissions.Length];

        for (int i = 0; i < permissions.Length; i++)
        {
            Debug.Log($"Requesting access to {permissions[i]}");
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
            //TODO: Show Dialog
            Debug.Log("Show dialog to manually adjust permissions.");
            settingsPanel.SetActive(true);
        }
        else
        {
            SwitchScene();
            Debug.Log("Permissions Granted");
        }
    }

    private void CheckPermissions()
    {
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

    private CLAuthorizationStatus locationAuthorizationStatus = CLAuthorizationStatus.NotDetermined;
    private AVAuthorizationStatus avAuthorizationStatus = AVAuthorizationStatus.NotDetermined;



    

    static LocationAuthorizationDelegate locationDelegate;
    static AVAuthorizationDelegate aVAuthorizationDelegate;


    public void OpenSettings() {
        Debug.Log("NYI - Opening Settings");
    }

    private void CheckPermissions()
    {
        CocoaHelpersBridge.SetLocationAuthorizationCallback(LocationAuthCallback);
        CocoaHelpersBridge.SetCameraAuthorizationCallback(AVAuthCallback);

        avAuthorizationStatus = CocoaHelpersBridge.GetCameraAuthorizationStatus();
        locationAuthorizationStatus = CocoaHelpersBridge.GetLocationAuthorizationStatus();

        Debug.Log($"Location Authorization ${locationAuthorizationStatus}");
        Debug.Log($"AVAuthorization Status ${avAuthorizationStatus}");

        if (avAuthorizationStatus == AVAuthorizationStatus.Authorized && 
            (locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedAlways || locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedWhenInUse)
            ) {
            SwitchScene();
        }
    }
    
    public void RequestPermissions() {
        Debug.Log("Requesting Permissions Location");

        
        locationDelegate = LocationCallback;
        aVAuthorizationDelegate = AVCallback;
        Debug.Log($"Location Permission {locationAuthorizationStatus}");
        switch (locationAuthorizationStatus)
        {
            case CLAuthorizationStatus.NotDetermined:        
                Debug.Log($"Location Permission Requesting");                    
                CocoaHelpersBridge.RequestAuthorizationStatusLocation();
                return;
                
            case CLAuthorizationStatus.Denied:
                OpenSettings();
                return;
            default:
                break;
        }

        Debug.Log("Requesting Permissions Camera");

   
        Debug.Log($"Camera Permission {avAuthorizationStatus}");
        switch (avAuthorizationStatus) {
            case AVAuthorizationStatus.NotDetermined:                
                CocoaHelpersBridge.RequestAuthorizationStatusCamera();
                return;
            case AVAuthorizationStatus.Denied:
                ShowPermissionsNeedededPanel();
                return;
            default:
                break;
        }

        Debug.Log("Checking Permissions");


        if (avAuthorizationStatus == AVAuthorizationStatus.Authorized &&
            (locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedWhenInUse || locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedAlways  )) {
                SwitchScene();
            }
    }
    
    private void LocationCallback(CLAuthorizationStatus status) {
        locationAuthorizationStatus = status;
        Debug.Log($"Location Callback received {status}");
        RequestPermissions();
    } 

    [MonoPInvokeCallback(typeof(LocationAuthorizationDelegate))]
    public static void LocationAuthCallback(CLAuthorizationStatus status) {
        if (locationDelegate != null) {
            locationDelegate(status);
        }
    }

     public void AVCallback(AVAuthorizationStatus status) { 
        avAuthorizationStatus = status;
        Debug.Log($"Location Callback received {status}");
        RequestPermissions();
     }

    [MonoPInvokeCallback(typeof(AVAuthorizationDelegate))]
    public static void AVAuthCallback(AVAuthorizationStatus status) {
        if (locationDelegate != null) {
            aVAuthorizationDelegate(status);
        }
    }

    private void ShowPermissionsNeedededPanel() {
        Debug.Log("Show permissions neeeded");
    }
#endif
    private void SwitchScene()
    {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
