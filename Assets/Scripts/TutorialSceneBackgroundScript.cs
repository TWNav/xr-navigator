using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public void OpenSettings() {
        Debug.Log("NYI");
    }

    private void CheckPermissions()
    {
        Debug.Log("NYI");
    }
    
    public void RequestPermissions()
    {
        Debug.Log("NYI");
    }

#endif
    private void SwitchScene()
    {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
