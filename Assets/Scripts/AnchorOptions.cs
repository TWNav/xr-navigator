using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [SerializeField]
    private GameObject DeleteConfirmPanel;

    private AnchorConverter anchorConverter;

    
    void Start() {
        anchorManager = FindObjectOfType<AnchorManager>();
        aRTapHandler = FindObjectOfType<ARTapHandler>();
        appController = FindObjectOfType<AppController>();
        anchorLerper = FindObjectOfType<AnchorLerper>();
        renameAnchorHandler = FindObjectOfType<RenameAnchorHandler>();

        anchorConverter = FindObjectOfType<AnchorConverter>();
        
    }
    public void RenameAnchor()
    {
        anchorLerper.SelectAnchor(aRTapHandler.currentSelectedAnchor);
        renameAnchorHandler.SetObjectToRename(aRTapHandler.currentSelectedAnchor);
        renameAnchorHandler.SelectInputField();
    }
    public void DeleteAnchor()
    {
        DeleteConfirmPanel.gameObject.SetActive(true);
    }
    public async void SaveAnchor()
    {
        anchorLerper.SubmitAnchor();
        Debug.Log("We are placing an anchor in to the cloud.");
        AnchorProperties anchorProperties = aRTapHandler.currentSelectedAnchor.GetComponent<AnchorProperties>();
        if(anchorProperties.anchorID != null)
        {
            await anchorConverter.UpdateExistingAnchor(anchorProperties.cloudSpatialAnchor,anchorProperties);
            Log.debug("Changing The Button Name");
            anchorProperties.button.GetComponentInChildren<TMP_Text>().text = anchorProperties.anchorLabel;
        }
        else{
            aRTapHandler.PlaceAnchor();
        }
        
        appController.EnterSelectMode();
        AnchorList.SetActive(true);
        AnchorOptionsUIElement.SetActive(false);

    }
    public void ConfirmDeleteAnchor()
    {
        Log.debug("confirming delete");
        anchorManager.DeleteCurrentAnchor();
        Log.debug("deleted");
        appController.EnterSelectMode();
        appController.ShowAnchorList();
        DeleteConfirmPanel.gameObject.SetActive(false);
    }
    public void CancelDeleteAnchor()
    {
        DeleteConfirmPanel.gameObject.SetActive(false);
    }
    public void CancelAnchorEdit()
    {
        appController.EnterSelectMode();
        appController.ShowAnchorList();
        var scaleString = aRTapHandler.currentSelectedAnchor.GetComponent<AnchorProperties>().cloudSpatialAnchor.AppProperties.SafeGet(AnchorProperties.ScaleKey);
        if(scaleString is string)
        {
            float x = float.Parse(scaleString);
            Log.debug($"anchor properties from cloud anchor local scale is : {x}");
            Log.debug($"Current Scale before Cancel Action :{aRTapHandler.currentSelectedAnchor.transform.localScale.x}");
            aRTapHandler.currentSelectedAnchor.transform.localScale = new Vector3(1f,1f,1f) * x;
            Log.debug($"Resetting Scale After Cancel Action to {x}");
        }
    }

}
