using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppModeEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private AppMode appMode;
    public event onModeChangeDelegate onModeChange;
    public delegate void onModeChangeDelegate(AppMode newMode);

    public AppMode currentAppMode
    {
        get {return appMode;}
        set{
            if(appMode == value) return;
            appMode = value;
            if(onModeChange != null)
            onModeChange(appMode);
        }
    }
}
