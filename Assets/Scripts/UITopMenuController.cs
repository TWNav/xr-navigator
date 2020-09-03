using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField]
    private GameObject noAnchorDetectedPanel;

    private bool isAnchorFound;
    private bool waitingToDetectAnchors;
    public float timeSpentWaiting = 0f;

    public int waitTime = 5;
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
        isAnchorFound = false;
        waitingToDetectAnchors = false;
        timeSpentWaiting = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingToDetectAnchors && !isAnchorFound)
        {
            timeSpentWaiting += Time.deltaTime;
            if (timeSpentWaiting <= (float)waitTime)
            {
                var propertyList = FindObjectsOfType<AnchorProperties>();
                foreach(AnchorProperties ap in propertyList)
                {
                    
                    if(ap.anchorID != null && ap.anchorID.Length > 1)
                    {
                        Log.debug($"AP anchor ID : {ap.anchorID} and the length is {ap.anchorID.Length}");
                        isAnchorFound  = true;
                        waitingToDetectAnchors = false;
                        break;
                    }
                }
            }
            else
            {
                waitingToDetectAnchors = false;
                noAnchorDetectedPanel.SetActive(true);
                var buttonsList = FindObjectsOfType<AnchorButtonHandler>();
                Log.debug($"{buttonsList.Length}");
                if(buttonsList.Length == 0)
                {
                    noAnchorDetectedPanel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "There are no anchors placed! Want to create some?";
                }
                else
                {
                    noAnchorDetectedPanel.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Re-locate your anchors to confirm they are saved in the cloud properly";
                }
            }
        }
        if (appController.appMode == AppMode.Explore && anchorManager.currentCloudSpatialAnchor != null)
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
        if (anchorLerper.hasAnchorSelected)
        {
            anchorLerper.SubmitAnchor();
        }
        waitingToDetectAnchors = false;
    }
    public async void ExploreButton()
    {

        appController.EnterExploreMode();
        AnchorList.SetActive(true);
        AnchorOptions.SetActive(false);
        anchorInfoText.gameObject.SetActive(false);
        distanceText.gameObject.SetActive(true);
        addAnchorButton.SetActive(false);
        AnchorLerper anchorLerper = FindObjectOfType<AnchorLerper>();
        if (anchorLerper.hasAnchorSelected)
        {
            anchorLerper.SubmitAnchor();
        }
        timeSpentWaiting = 0f;
        waitingToDetectAnchors = true;
        
    }
}
