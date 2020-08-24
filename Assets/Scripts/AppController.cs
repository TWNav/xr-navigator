using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AppMode
{
    Home,
    Create,
    Select,
    Explore,
    Edit
}
public class AppController : MonoBehaviour
{


    public AppMode appMode;
    private AppModeEvent appModeEventSender;
    private ARTapHandler aRTapHandler;

    [SerializeField]
    private GameObject anchorOptions;

    [SerializeField]
    private GameObject anchorList;
    // Start is called before the first frame update
    void Start()
    {
        appMode = AppMode.Select;
        //appModeEventSender.onModeChange += ModeSwitch;
    }

    void ModeSwitch(AppMode mode)
    {
        switch (mode)
        {
            case AppMode.Home: break;
            case AppMode.Create: 

                            break;
            case AppMode.Select: break;
            case AppMode.Explore: break;
            default:
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void EnterCreateMode() {
        appMode = AppMode.Create;
    }
    public void EnterSelectMode() {
        appMode = AppMode.Select;
    }
    public void EnterHomeMode()
    {
        appMode = AppMode.Home;
    }
    public void EnterExploreMode()
    {
        appMode = AppMode.Explore;
    }
     public void EnterEditMode()
    {
        appMode = AppMode.Edit;
    }
    public void ShowAnchorOptions()
    {
        anchorList.SetActive(false);
        anchorOptions.SetActive(true);
    }
    public void ShowAnchorList()
    {
        anchorList.SetActive(true);
        anchorOptions.SetActive(false);
    }
}
