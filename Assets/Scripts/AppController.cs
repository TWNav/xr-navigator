using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private GameObject existingAnchorOptions;

    [SerializeField]
    private GameObject anchorList;
    [SerializeField]
    private GameObject exploreButton;
    [SerializeField]
    private GameObject newAnchorOptions;
    // Start is called before the first frame update
    void Start()
    {
        appMode = AppMode.Select;
        ShowAnchorList();
    }

    void ModeSwitch(AppMode mode)
    {
        switch (mode)
        {
            case AppMode.Home:
                exploreButton.GetComponent<Button>().interactable = true;
                break;
            case AppMode.Create:
                exploreButton.GetComponent<Button>().interactable = true;
                break;
            case AppMode.Select:
                exploreButton.GetComponent<Button>().interactable = true;
                break;
            case AppMode.Explore:
                exploreButton.GetComponent<Button>().interactable = true; 
                break;
            case AppMode.Edit:
                exploreButton.GetComponent<Button>().interactable = false;
                break;

            default:
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void EnterCreateMode()
    {
        appMode = AppMode.Create;
        exploreButton.GetComponent<Button>().interactable = true;
    }
    public void EnterSelectMode()
    {
        appMode = AppMode.Select;
        exploreButton.GetComponent<Button>().interactable = true;
    }
    public void EnterHomeMode()
    {
        appMode = AppMode.Home;
        exploreButton.GetComponent<Button>().interactable = true;
    }
    public void EnterExploreMode()
    {
        appMode = AppMode.Explore;
        exploreButton.GetComponent<Button>().interactable = true;
    }
    public void EnterEditMode()
    {
        appMode = AppMode.Edit;
        exploreButton.GetComponent<Button>().interactable = false;
    }
    public void ShowExistingAnchorOptions()
    {
        anchorList.SetActive(false);
        existingAnchorOptions.SetActive(true);
        newAnchorOptions.SetActive(false);
    }
    public void ShowNewAnchorOptions()
    {
        anchorList.SetActive(false);
        newAnchorOptions.SetActive(true);
        existingAnchorOptions.SetActive(false);
    }
    public void ShowAnchorList()
    {
        anchorList.SetActive(true);
        existingAnchorOptions.SetActive(false);
        newAnchorOptions.SetActive(false);
    }
}
