using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
#endif


public class TutorialSceneBackgroundScript : MonoBehaviour
{
    void Update()
    {
        // TODO: Make this a click handler
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            RequestPermissions();
        }
    }

#if UNITY_ANDROID
    private void RequestPermissions()
    {
        // string[] permissions = { Permission.Camera };

        // AndroidRuntimePermissions.Permission[] results = new AndroidRuntimePermissions.Permission[permissions.Length];

        // for (int i = 0; i < permissions.Length; i++)
        // {
        //     Debug.Log($"Requesting access to {permissions[i]}");
        //     results[i] = AndroidRuntimePermissions.RequestPermission(permissions[i]);
        // }
        AndroidRuntimePermissions.Permission[] results = AndroidRuntimePermissions.RequestPermissions(Permission.FineLocation, Permission.Camera);

        bool deniedPermission = false;
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i] == AndroidRuntimePermissions.Permission.Denied)
            {
                deniedPermission = true;
            }
        }

        if (deniedPermission)
        {
            //TODO: Show Dialog
            Debug.Log("Show dialog to manually adjust permissions.");
        }
        else
        {
            SwitchScene();
            Debug.Log("Permissions Granted");
        }
    }
#endif
    private void SwitchScene()
    {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
