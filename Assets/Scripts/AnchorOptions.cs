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

    private AnchorLerper anchorLerper;
    private RenameAnchorHandler renameAnchorHandler;

    
    void Start() {
        anchorManager = FindObjectOfType<AnchorManager>();
        aRTapHandler = FindObjectOfType<ARTapHandler>();
        appController = FindObjectOfType<AppController>();
        anchorLerper = FindObjectOfType<AnchorLerper>();
        renameAnchorHandler = FindObjectOfType<RenameAnchorHandler>();
        
    }
    public void RenameAnchor()
    {
        anchorLerper.SelectAnchor(aRTapHandler.currentSelectedAnchor);
        renameAnchorHandler.SetObjectToRename(aRTapHandler.currentSelectedAnchor);
        renameAnchorHandler.SelectInputField();
    }
    public void DeleteAnchor()
    {
        anchorManager.DeleteCurrentAnchor();
        appController.EnterSelectMode();
        appController.ShowAnchorList();
    }
    public void SaveAnchor()
    {
        
        anchorLerper.SubmitAnchor();
        Debug.Log("We are placing an anchor in to the cloud.");
        aRTapHandler.PlaceAnchor();
        appController.EnterSelectMode();
        AnchorList.SetActive(true);
        AnchorOptionsUIElement.SetActive(false);

    }

}
