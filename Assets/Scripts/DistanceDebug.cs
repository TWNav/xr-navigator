using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.SpatialAnchors;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DistanceDebug : MonoBehaviour
{
    AnchorManager anchorManager;

    ARAnchorManager aRAnchorManager;

    // Start is called before the first frame update
    void Start()
    {
        anchorManager = FindObjectOfType<AnchorManager>();
        aRAnchorManager = FindObjectOfType<ARAnchorManager>();
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
                    TMP_Text distanceText = GetComponent<TMP_Text>();
                    distanceText.text = System.Math.Round(Vector3.Distance(anchor.transform.position,Camera.main.transform.position) * 3.28084, 1) + " ft";
                    break;
                }
            }
            
        }
    }
}
