using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

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
            Debug.Log($"Anchor Located : {args.Anchor.Identifier}");

            if(args.Identifier == null || args.Anchor == null)
            {
                Debug.Log("Anchor or Identifier is null");
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
                    Debug.Log($"Trying to add anchor. {anchor.name}");
                    AnchorProperties anchorProperties = anchor.gameObject.GetComponent<AnchorProperties>();
                    anchorProperties.anchorID = args.Anchor.Identifier;
                    Debug.Log($"AnchorID : {anchorProperties.anchorID}");
                    GameObject anchorRender = Instantiate(arAnchorContainerRender,anchor.transform.position,anchor.transform.rotation);
                    Debug.Log($"Instantiate Render. {anchor.name}");
                    anchorRender.transform.SetParent(anchor.transform);
                    Debug.Log($"Assign Parent for Render. {anchor.name}");
                    break;
                }
            }
            anchorFirstTimeFound = true;
            Debug.Log($"Checking first AnchorFirstTimeFound :{anchorFirstTimeFound}");
        }
    }
 
    private void ConfigureSensors()
    {
        PlatformLocationProvider sensorProvider = new PlatformLocationProvider();
        spatialAnchorManager.Session.LocationProvider = sensorProvider;
        sensorProvider.Sensors.GeoLocationEnabled = CheckLocationPermissions();
        //sensorProvider.Start();
    }

    //method to find anchors based on geolocation + wifi
    public async void FindAnchorsByLocation()
    {
        Debug.Log("Getting ActiveWatchers");

        if(anchorFirstTimeFound)
        {
            await ResetSession();
        }
        Debug.Log("Finding Anchors By Location");
        //set a neardevicecriteria to look for anchors within 5 meters
        //can return max of 25 anchors to be searching for at once time here
        NearDeviceCriteria nearDeviceCriteria = new NearDeviceCriteria();
        nearDeviceCriteria.DistanceInMeters = 25;
        nearDeviceCriteria.MaxResultCount = 35;
        AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();
        anchorLocateCriteria.NearDevice = nearDeviceCriteria;
        Debug.Log($"Chen is about to crash ");
        spatialAnchorManager.Session.CreateWatcher(anchorLocateCriteria);
        Debug.Log("Chen is crashing");

    }

    public async Task ResetSession()
    {
        foreach(ARAnchor anchor in aRAnchorManager.trackables)
        {
            if(anchor == null)
            {
                break;
            }
            Debug.Log($"-----ARFoundation removing {anchor.trackableId}");
            aRAnchorManager.RemoveAnchor(anchor);
            Debug.Log("-----ARFoundation removed anchor ");
        }
        anchorManager.ClearCloudSpatialAnchorList();
        Debug.Log($"-----SpatialAnchorManager Session is about to Reset:{anchorFirstTimeFound}");
        spatialAnchorManager.StopSession();
        await spatialAnchorManager.ResetSessionAsync();
        ConfigureSensors();
        await spatialAnchorManager.StartSessionAsync();
        Debug.Log($"-----SpatialAnchorManager Reset Complete:{anchorFirstTimeFound}");
    }

    public bool CheckLocationPermissions()
    {
        bool permissionsGranted = false;
        #if UNITY_ANDROID
        permissionsGranted = Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        #elif UNITY_IOS
        CLAuthorizationStatus locationAuthorizationStatus = CocoaHelpersBridge.GetLocationAuthorizationStatus();
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
            await spatialAnchorManager.CreateAnchorAsync(cloudAnchor);
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
