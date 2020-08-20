using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppCenterShared : MonoBehaviour
{
    AppCenterBehavior appCenter = null;
    private void Start() {
        Log.debug("Should find AppCenterBehavior.");
        
        appCenter = FindObjectOfType<AppCenterBehavior>();
        if (appCenter) {
            Log.debug("Did find AppCenterBehavior.");
        } else {
            Log.debug("Did NOT find AppCenterBehavior.");
        }
    }

}
