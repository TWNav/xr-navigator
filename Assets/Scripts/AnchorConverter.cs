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

    [SerializeField]
    private GameObject arAnchorContainerRender;
    private SpatialAnchorManager spatialAnchorManager;

    private ARAnchorManager aRAnchorManager;
    private AnchorManager anchorManager;
    private bool anchorManagerIsSetup = false;

    private bool anchorFirstTimeFound = false;

    

    // Start is called before the first frame update
    void Start()
    {
        spatialAnchorManager = GetComponent<SpatialAnchorManager>();
        ARSession aRSession = FindObjectOfType<ARSession>();
        aRAnchorManager = FindObjectOfType<ARAnchorManager>();
        anchorManager = FindObjectOfType<AnchorManager>();
        ARSession.stateChanged += AnchorConverter_SessionStateChange;


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
        if(args.Status == LocateAnchorStatus.Located)
        {
            Log.debug($"Anchor Located : {args.Anchor.Identifier}");

            if(args.Identifier == null || args.Anchor == null)
            {
                Log.debug("Anchor or Identifier is null");
                return;
            }

            anchorManager.AddCloudSpatialAnchor(args.Anchor);

            foreach(ARAnchor anchor in aRAnchorManager.trackables)
            {
                if(anchor == null)
                {
                    break;
                }
                if(anchor.transform.position == args.Anchor.GetPose().position)
                {
                    Log.debug($"Trying to add anchor. {anchor.name}");
                    AnchorProperties anchorProperties = anchor.gameObject.GetComponent<AnchorProperties>();
                    anchorProperties.anchorID = args.Anchor.Identifier;
                    Log.debug($"AnchorID : {anchorProperties.anchorID}");
                    GameObject anchorRender = Instantiate(arAnchorContainerRender,anchor.transform.position,anchor.transform.rotation);
                    Log.debug($"Instantiate Render. {anchor.name}");
                    anchorRender.transform.SetParent(anchor.transform);
                    Log.debug($"Assign Parent for Render. {anchor.name}");
                    break;
                }
            }
            anchorFirstTimeFound = true;
            Log.debug($"Checking first AnchorFirstTimeFound :{anchorFirstTimeFound}");
        }
    }
 
    private void ConfigureSensors()
    {
        PlatformLocationProvider sensorProvider = new PlatformLocationProvider();
        spatialAnchorManager.Session.LocationProvider = sensorProvider;
        sensorProvider.Sensors.GeoLocationEnabled = CheckLocationPermissions();
        sensorProvider.Start();
    }

    //method to find anchors based on geolocation + wifi
    public async void FindAnchorsByLocation()
    {
        Log.debug("Getting ActiveWatchers");

        if(anchorFirstTimeFound)
        {
            await ResetSession();
        }
        Log.debug("Finding Anchors By Location");
        //set a neardevicecriteria to look for anchors within 5 meters
        //can return max of 25 anchors to be searching for at once time here
        NearDeviceCriteria nearDeviceCriteria = new NearDeviceCriteria();
        nearDeviceCriteria.DistanceInMeters = 25;
        nearDeviceCriteria.MaxResultCount = 35;
        AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();
        anchorLocateCriteria.NearDevice = nearDeviceCriteria;
        Log.debug($"Chen is about to crash ");
        spatialAnchorManager.Session.CreateWatcher(anchorLocateCriteria);
        Log.debug("Chen is crashing");

    }

    public async Task ResetSession()
    {
        foreach(ARAnchor anchor in aRAnchorManager.trackables)
        {
            if(anchor == null)
            {
                break;
            }
            Log.debug($"-----ARFoundation removing {anchor.trackableId}");
            aRAnchorManager.RemoveAnchor(anchor);
            Log.debug("-----ARFoundation removed anchor ");
        }
        anchorManager.ClearCloudSpatialAnchorList();
        Log.debug($"-----SpatialAnchorManager Session is about to Reset:{anchorFirstTimeFound}");
        spatialAnchorManager.Session.Stop();
        await spatialAnchorManager.ResetSessionAsync();
        await spatialAnchorManager.StartSessionAsync();
        Log.debug($"-----SpatialAnchorManager Reset Complete:{anchorFirstTimeFound}");
    }

    public bool CheckLocationPermissions()
    {
        bool permissionsGranted = false;
        #if UNITY_ANDROID
        permissionsGranted = AndroidRuntimePermissions.CheckPermission(UnityEngine.Android.Permission.FineLocation) == AndroidRuntimePermissions.Permission.Granted;
        #elif UNITY_IOS
        CLAuthorizationStatus locationAuthorizationStatus = CocoaHelpersBridge.GetLocationAuthorizationStatus();
        permissionsGranted =  (locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedAlways || locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedWhenInUse);
        #endif
        Log.debug($"Location Permission : {permissionsGranted}");
        return permissionsGranted;
    }

    public async Task CreateCloudAnchor(CloudSpatialAnchor cloudAnchor)
    {
        if(cloudAnchor==null)
        {
            Log.debug("Cloud anchor is null");
            return;
        }
        
        while (!spatialAnchorManager.IsReadyForCreate)
        {
            Log.debug($"Not enough environmental data : {spatialAnchorManager.SessionStatus.RecommendedForCreateProgress}");
            await Task.Delay(333);
        }

        try 
        {
            Log.debug("Trying to create cloud anchor");
            await spatialAnchorManager.Session.CreateAnchorAsync(cloudAnchor);
        }
        catch (Exception ex)
        {
            Log.debug(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
