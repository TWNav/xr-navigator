﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using TMPro;

public class AnchorManager : MonoBehaviour
{
    public CloudSpatialAnchor currentCloudSpatialAnchor;
    private SpatialAnchorManager spatialAnchorManager;
    private AnchorConverter anchorConverter;
    private ARTapHandler aRTapHandler;
    [SerializeField]
    private TMP_Text anchorInfoText;

    private List<CloudSpatialAnchor> foundCloudSpatialAnchors = new List<CloudSpatialAnchor>();

    void Start()
    {
        spatialAnchorManager = FindObjectOfType<SpatialAnchorManager>();
        anchorConverter = FindObjectOfType<AnchorConverter>();
        aRTapHandler = FindObjectOfType<ARTapHandler>();
    }
    public async void DeleteCurrentAnchor()
    {
        Log.debug($"Try to Delete CloudSpatialAnchor: {currentCloudSpatialAnchor.Identifier} ");
        await spatialAnchorManager.DeleteAnchorAsync(currentCloudSpatialAnchor); 
        Log.debug($"CloudSpatialAnchor is Deleted: {currentCloudSpatialAnchor.Identifier}");
        anchorInfoText.GetComponent<FadeText>().SetText($"Anchor deleted \nLabel: {aRTapHandler.currentSelectedAnchor.GetComponent<AnchorProperties>().anchorLabel}");
        await anchorConverter.ResetSession();
        anchorConverter.FindAnchorsByLocation();
    }

    public void AddCloudSpatialAnchor(CloudSpatialAnchor cloudSpatialAnchor)
    {
        Log.debug($"Adding {cloudSpatialAnchor.Identifier} to foundCloudSpatialAnchors");
        foundCloudSpatialAnchors.Add(cloudSpatialAnchor);
    }

    public void ClearCloudSpatialAnchorList()
    {
        foundCloudSpatialAnchors.Clear();
        currentCloudSpatialAnchor = null;
    }
    public void SelectAnchor(string anchorIdentifier)
    {
        foreach (CloudSpatialAnchor csa in foundCloudSpatialAnchors)
        {
            if(csa.Identifier.Equals(anchorIdentifier))
            {
                Log.debug($"{csa.Identifier} is being selected");
                currentCloudSpatialAnchor = csa;
                break;
            }
        }
    }
}
