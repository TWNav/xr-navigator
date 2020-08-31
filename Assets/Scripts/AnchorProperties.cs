using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.SpatialAnchors;
using UnityEngine;

public class AnchorProperties : MonoBehaviour, IComparable
{
    public static string AnchorLabelKey = "anchorLabel";
    public static string DateKey = "date";
    public static string ScaleKey = "scale";

    public string anchorID;
    public string anchorLabel = "New Anchor";

    public string dateSecondsString = "0";

    public CloudSpatialAnchor cloudSpatialAnchor;
    public GameObject button;

    public double dateSeconds => double.Parse(dateSecondsString);


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLabel()
    {
        
    }

    public int CompareTo(object obj) {
        if (obj is null) return 1;

        AnchorProperties ap = obj as AnchorProperties;
        if (ap is null) {
            throw new ArgumentException("Should be AnchorProperties!");
        } else {
            return ap.dateSeconds.CompareTo(this.dateSeconds);
        }
    }
}
