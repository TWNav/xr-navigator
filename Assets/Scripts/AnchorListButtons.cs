using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorListButtons : MonoBehaviour
{
     private AppController appController;
    [SerializeField]
    private GameObject AnchorList;
    [SerializeField]
    private GameObject AnchorOptions;
    private ARTapHandler aRTapHandler;
    [SerializeField]
    private GameObject anchorsAwayPanel;
    private bool hasAddedAnAnchorInSession;

    // Start is called before the first frame update
    void Start()
    {
        appController = FindObjectOfType<AppController>();
        aRTapHandler = FindObjectOfType<ARTapHandler>();
        anchorsAwayPanel.SetActive(false);
        hasAddedAnAnchorInSession = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddAnchor()
    {
        appController.EnterCreateMode();
        AnchorList.SetActive(false);
        if(!hasAddedAnAnchorInSession)
        {
            anchorsAwayPanel.SetActive(true);
            hasAddedAnAnchorInSession = true;
        }
        if(aRTapHandler.objectToPlace != null)
        {
            appController.ShowNewAnchorOptions();
        }
    }
}
