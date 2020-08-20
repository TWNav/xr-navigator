using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorOptions : MonoBehaviour
{
    private AnchorManager anchorManager;
    void Start() {
        anchorManager = FindObjectOfType<AnchorManager>();
        
    }
    public void RenameAnchor()
    {
        Log.debug("Rename Function Not Implemented.");
    }
    public void DeleteAnchor()
    {

        anchorManager.DeleteCurrentAnchor();
    }
    public void SaveAnchor()
    {
        Log.debug("Save Function Not Implemented.");
    }

}
