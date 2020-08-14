using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;

public class AnchorManager : MonoBehaviour
{
    private CloudSpatialAnchor currentCloudSpatialAnchor;
    private SpatialAnchorManager spatialAnchorManager;

    void Start()
    {
        spatialAnchorManager = FindObjectOfType<SpatialAnchorManager>();
    }
    public async void DeleteCurrentAnchor()
    {
        Debug.Log($"Try to Delete CloudSpatialAnchor: {currentCloudSpatialAnchor.Identifier} ");
        await spatialAnchorManager.DeleteAnchorAsync(currentCloudSpatialAnchor); 
        Debug.Log($"CloudSpatialAnchor is Deleted: {currentCloudSpatialAnchor.Identifier}");
    }
    public void SelectAnchor(CloudSpatialAnchor cloudSpatialAnchor)
    {
        Debug.Log($"{cloudSpatialAnchor.Identifier} is being selected");
        currentCloudSpatialAnchor = cloudSpatialAnchor;
    }
}
