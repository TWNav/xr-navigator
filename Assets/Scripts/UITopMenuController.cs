using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITopMenuController : MonoBehaviour
{
    private AppController appController;
    private AppModeEvent appModeEvent; 

    [SerializeField]
    private GameObject AnchorList;
    [SerializeField]
    private GameObject AnchorOptions;

    [SerializeField]
    private GameObject addAnchorButton;
    [SerializeField]
    private GameObject navigationArrow;
    private AnchorManager anchorManager;

    [SerializeField]
    private TMP_Text anchorInfoText, distanceText;
    // Start is called before the first frame update
    void Start()
    {
        appController = FindObjectOfType<AppController>();
        AnchorList.SetActive(true);
        AnchorOptions.SetActive(false);
        navigationArrow.SetActive(false);
        distanceText.gameObject.SetActive(false);
        anchorInfoText.gameObject.SetActive(true);
        anchorManager = FindObjectOfType<AnchorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(appController.appMode == AppMode.Explore && anchorManager.currentCloudSpatialAnchor != null)
        {
            navigationArrow.SetActive(true);
        }
    }
    public void ManageButton()
    {
        appController.appMode = AppMode.Select;
        navigationArrow.SetActive(false);
        AnchorList.SetActive(true);
        AnchorOptions.SetActive(false);
        anchorInfoText.gameObject.SetActive(true);
        distanceText.gameObject.SetActive(false);
        addAnchorButton.SetActive(true);
        AnchorLerper anchorLerper = FindObjectOfType<AnchorLerper>();
        if(anchorLerper.hasAnchorSelected)
        {
            anchorLerper.SubmitAnchor();
        }
    }
    public void ExploreButton()
    {
        appController.appMode = AppMode.Explore;
        AnchorList.SetActive(true);
        AnchorOptions.SetActive(false);
        anchorInfoText.gameObject.SetActive(false);
        distanceText.gameObject.SetActive(true);
        addAnchorButton.SetActive(false);
        AnchorLerper anchorLerper = FindObjectOfType<AnchorLerper>();
        if(anchorLerper.hasAnchorSelected)
        {
            anchorLerper.SubmitAnchor();
        }
    } 
}
