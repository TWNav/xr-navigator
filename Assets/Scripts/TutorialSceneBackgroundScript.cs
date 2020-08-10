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
    void Start() {
        CheckPermissions();
    }

    void Update()
    {
        // TODO: Make this a click handler
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RequestPermissions();
        }
    }

#if UNITY_ANDROID
    string[] permissions = { Permission.Camera };
    private void RequestPermissions()
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
            if (results[i] == AndroidRuntimePermissions.Permission.Denied)
            {
                deniedPermission.Add(permissions[i]);
            }
        }

        if (deniedPermission.Count > 0)
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

    private void CheckPermissions()
    {
        AndroidRuntimePermissions.Permission[] checkedPermissions = AndroidRuntimePermissions.CheckPermissions(permissions);
        bool granted = true;
        for (int i = 0; i < checkedPermissions.Length; i++) {
            if(checkedPermissions[i] != AndroidRuntimePermissions.Permission.Granted) {
                granted = false;
                break;
            }
        }
        if(granted) {
            SwitchScene();
        }
    }
#endif
    private void SwitchScene()
    {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
