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
    private bool isLocationEnabled {
        get {
            return Input.location.isEnabledByUser;
        }
    }
    void Start() {
       RequestPermissions();
    }
    void Update()
    {
        if(!isLocationEnabled) return;
        // TODO: Make this a click handler
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
    
            SwitchScene();
        }    
    }

    private void RequestPermissions()
    {
        #if UNITY_ANDROID
        RequestPermissionIfNotGiven(Permission.FineLocation);
        RequestPermissionIfNotGiven(Permission.Camera);
        #elif UNITY_IOS
        //TODO: Implement permissions for iOS
        Debug.Log("Request Permissions iOS");
        #endif
    }

#if UNITY_ANDROID
    private void RequestPermissionIfNotGiven(string permission)
    {
        if(!Permission.HasUserAuthorizedPermission(permission))
        {
            Debug.Log($"Requesting access to {permission}");
            Permission.RequestUserPermission(permission);
            //TODO: Find a better way to recognize when permission has been granted
            while(!Permission.HasUserAuthorizedPermission(permission))
            {
                Thread.Sleep(500);
            }
            Debug.Log(String.Format("Permission for {0} is {1}", permission, Permission.HasUserAuthorizedPermission(permission)));
        }
        else
        {
            Debug.Log($"Already had permission to {permission}");
        }
    }
#endif
    private void SwitchScene() {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
