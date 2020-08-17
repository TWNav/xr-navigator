using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.SpatialAnchors;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DistanceDebug : MonoBehaviour
{
    private AnchorManager anchorManager;

    private ARAnchorManager aRAnchorManager;
    private TMP_Text distanceText;

    // Start is called before the first frame update
    void Start()
    {
        anchorManager = FindObjectOfType<AnchorManager>();
        aRAnchorManager = FindObjectOfType<ARAnchorManager>();
        distanceText = GetComponent<TMP_Text>();
    }

    
    // Update is called once per frame
    void Update()
    {
        if(!(anchorManager.currentCloudSpatialAnchor is null))
        {
            foreach (ARAnchor anchor in aRAnchorManager.trackables ) 
            {
                AnchorProperties anchorProperties = anchor.gameObject.GetComponent<AnchorProperties>();
                
                if (anchorProperties.anchorID == anchorManager.currentCloudSpatialAnchor.Identifier)
                {
                    distanceText.text = System.Math.Round(Vector3.Distance(anchor.transform.position,Camera.main.transform.position) * 3.28084, 1) + " ft";
                    break;
                }
            
            }
            
        }
        else
        {
            distanceText.text = "No anchor selected";
        }
    }
}
