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
        anchorManager.DeleteCurrentAnchor();
        appController.EnterSelectMode();
        appController.ShowAnchorList();
        DeleteConfirmPanel.gameObject.SetActive(false);
    }
    public void CancelDeleteAnchor()
    {
        DeleteConfirmPanel.gameObject.SetActive(false);
    }

}
