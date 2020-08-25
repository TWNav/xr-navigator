using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotator : MonoBehaviour
{
    AnchorManager anchorManager;
    Vector3 destinationLocation;
    // Start is called before the first frame update
    void Start()
    {
        anchorManager = FindObjectOfType<AnchorManager>();
    }

    // Update is called once per frame
    void Update()
    {

        var anchorProps = FindObjectsOfType<AnchorProperties>();
        foreach(AnchorProperties anchorProperties in anchorProps)
        {
            if(anchorProperties.anchorID.Equals(anchorManager.currentCloudSpatialAnchor.Identifier))
            {
                Log.debug($"Setting destinationLocation to {anchorProperties.anchorLabel}");
                destinationLocation = anchorProperties.gameObject.transform.position;
            }
        }
        float distanceToDestination = Vector3.Distance(destinationLocation, Camera.main.transform.position);
        this.transform.parent.LookAt(destinationLocation);
    }
}
