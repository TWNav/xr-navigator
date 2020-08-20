using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorOptions : MonoBehaviour
{
    AppController appController;
    private AnchorManager anchorManager;
    private ARTapHandler aRTapHandler;
    [SerializeField]
    private GameObject AnchorList;
    [SerializeField]
    private GameObject AnchorOptionsUIElement;

    
    void Start() {
        anchorManager = FindObjectOfType<AnchorManager>();
        aRTapHandler = FindObjectOfType<ARTapHandler>();
        appController = FindObjectOfType<AppController>();
        
    }
    public void RenameAnchor()
    {
        Log.debug("Rename Function Not Implemented.");
    }
    public void DeleteAnchor()
    {
        anchorManager.DeleteCurrentAnchor();
        appController.ShowAnchorList();
    }
    public void SaveAnchor()
    {
        Log.debug("We are placing an anchor in to the cloud.");
        aRTapHandler.PlaceAnchor();
        appController.appMode = AppMode.Select;
        AnchorList.SetActive(true);
        AnchorOptionsUIElement.SetActive(false);        
    }

}
