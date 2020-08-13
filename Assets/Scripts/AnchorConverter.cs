using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AnchorConverter : MonoBehaviour
{

    private SpatialAnchorManager spatialAnchorManager;

    private bool anchorManagerIsSetup = false;

    // Start is called before the first frame update
    async void Start()
    {
        spatialAnchorManager = GetComponent<SpatialAnchorManager>();
        ARSession aRSession = FindObjectOfType<ARSession>();
        ARSession.stateChanged += AnchorConverter_SessionStateChange;


    }

    IEnumerator WaitForARSession()
    {
        Debug.Log("Waiting for SessionTracking");
        yield return new WaitUntil(() => ARSession.state == ARSessionState.SessionTracking);
        Debug.Log("Session state = tracking");
    }

    private async void AttemptSetup()
    {
        if(anchorManagerIsSetup)
        {
            return;
        }

        //if the session doesn't exist, create session
        if (spatialAnchorManager.Session == null)
        {
            await spatialAnchorManager.CreateSessionAsync();
        }
        spatialAnchorManager.AnchorLocated += AnchorConverter_AnchorLocated;
        await spatialAnchorManager.StartSessionAsync();
        ConfigureSensors();
        anchorManagerIsSetup = true;
        FindAnchorsByLocation();
    }

    private void AnchorConverter_SessionStateChange(ARSessionStateChangedEventArgs obj)
    {
        switch (obj.state)
        {
            case ARSessionState.SessionTracking:
                AttemptSetup();
                break;
            default:
                break;
        }
    }

    private void AnchorConverter_AnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        Debug.Log($"Anchor Located : {args.Anchor.Identifier}");
    }

    private void ConfigureSensors()
    {
        PlatformLocationProvider sensorProvider = new PlatformLocationProvider();
        spatialAnchorManager.Session.LocationProvider = sensorProvider;
        sensorProvider.Sensors.GeoLocationEnabled = CheckLocationPermissions();
        sensorProvider.Start();
    }

    //method to find anchors based on geolocation + wifi
    public void FindAnchorsByLocation()
    {
        Debug.Log("Finding Anchors By Location");
        //set a neardevicecriteria to look for anchors within 5 meters
        //can return max of 25 anchors to be searching for at once time here
        NearDeviceCriteria nearDeviceCriteria = new NearDeviceCriteria();
        nearDeviceCriteria.DistanceInMeters = 25;
        nearDeviceCriteria.MaxResultCount = 35;
        AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();
        anchorLocateCriteria.NearDevice = nearDeviceCriteria;
        spatialAnchorManager.Session.CreateWatcher(anchorLocateCriteria);
    }

    public bool CheckLocationPermissions()
    {
        bool permissionsGranted = false;
        #if UNITY_ANDROID
        permissionsGranted = AndroidRuntimePermissions.CheckPermission(UnityEngine.Android.Permission.FineLocation) == AndroidRuntimePermissions.Permission.Granted;
        #elif UNITY_IOS
        CLAuthorizationStatus locationAuthorizationStatus = CocoaHelpersBridge.GetLocationAuthorizationStatus()
        permissionsGranted =  (locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedAlways || locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedWhenInUse);
        #endif
        Debug.Log($"Location Permission : {permissionsGranted}");
        return permissionsGranted;
    }

    public async Task CreateCloudAnchor(CloudSpatialAnchor cloudAnchor)
    {
        if(cloudAnchor==null)
        {
            Debug.Log("Cloud anchor is null");
            return;
        }
        
        while (!spatialAnchorManager.IsReadyForCreate)
        {
            Debug.Log($"Not enough environmental data : {spatialAnchorManager.SessionStatus.RecommendedForCreateProgress}");
            await Task.Delay(333);
        }

        try 
        {
            Debug.Log("Trying to create cloud anchor");
            await spatialAnchorManager.Session.CreateAnchorAsync(cloudAnchor);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
