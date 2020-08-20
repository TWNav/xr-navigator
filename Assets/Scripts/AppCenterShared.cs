using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppCenterShared : MonoBehaviour
{
    AppCenterBehavior appCenter = null;
    private void Start() {
        Debug.Log("Should find AppCenterBehavior.");
        
        appCenter = FindObjectOfType<AppCenterBehavior>();
        if (appCenter) {
            Debug.Log("Did find AppCenterBehavior.");
        } else {
            Debug.Log("Did NOT find AppCenterBehavior.");
        }
    }

}
