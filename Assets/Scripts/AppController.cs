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


    public AppMode appMode = AppMode.Select;
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
    [SerializeField]
    private GameObject createMapButton;
    // Start is called before the first frame update
    void Start()
    {
        appMode = AppMode.Select;
        ShowAnchorList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeTopBarState(bool isEnabled) {
        exploreButton.GetComponent<Button>().interactable = isEnabled;
        createMapButton.GetComponent<Button>().interactable = isEnabled;
    }

    public void EnterCreateMode()
    {
        appMode = AppMode.Create;
        ChangeTopBarState(true);
    }
    public void EnterSelectMode()
    {
        appMode = AppMode.Select;
        ChangeTopBarState(true);
    }
    public void EnterHomeMode()
    {
        appMode = AppMode.Home;
        ChangeTopBarState(true);
    }
    public void EnterExploreMode()
    {
        appMode = AppMode.Explore;
        ChangeTopBarState(true);
    }
    public void EnterEditMode()
    {
        appMode = AppMode.Edit;
        ChangeTopBarState(false);
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
