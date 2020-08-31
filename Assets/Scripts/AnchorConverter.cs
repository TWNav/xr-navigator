using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class AnchorConverter : MonoBehaviour
{

    [SerializeField]
    private GameObject arAnchorContainerRender;
    [SerializeField]
    private TMP_Text anchorInfoText;
    [SerializeField]
    private GameObject progressBar;
    private SpatialAnchorManager spatialAnchorManager;
    private ARAnchorManager aRAnchorManager;
    private AnchorManager anchorManager;
    private bool anchorManagerIsSetup = false;

    private bool anchorFirstTimeFound = false;
    [SerializeField]
    private GameObject tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        spatialAnchorManager = GetComponent<SpatialAnchorManager>();
        ARSession aRSession = FindObjectOfType<ARSession>();
        aRAnchorManager = FindObjectOfType<ARAnchorManager>();
        anchorManager = FindObjectOfType<AnchorManager>();
        ARSession.stateChanged += AnchorConverter_SessionStateChange;
        tutorialManager.gameObject.SetActive(true);

    }

    private async void AttemptSetup()
    {
        if (anchorManagerIsSetup)
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
        if (args.Status == LocateAnchorStatus.Located)
        {
            Log.debug($"Anchor Located : {args.Anchor.Identifier}");

            if (args.Identifier == null || args.Anchor == null)
            {
                Log.debug("Anchor or Identifier is null");
                return;
            }

            anchorManager.AddCloudSpatialAnchor(args.Anchor);

            foreach (ARAnchor anchor in aRAnchorManager.trackables)
            {
                if (anchor == null)
                {
                    break;
                }
                if (anchor.transform.position == args.Anchor.GetPose().position)
                {

                    Debug.Log($"Trying to add anchor. {anchor.name}");
                    AnchorProperties anchorProperties = anchor.gameObject.GetComponent<AnchorProperties>();

                    Debug.Log($"AnchorID : {anchorProperties.anchorID}");
                    Debug.Log($"Instantiate Render. {anchor.name}");
                    GetAnchorProperties(args.Anchor, anchorProperties);
                    GameObject anchorRender = Instantiate(arAnchorContainerRender, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f,0f,0f));
                    if (anchorProperties.anchorLabel != null && anchorProperties.anchorLabel.Length > 0)
                    {
                        anchorRender.GetComponentInChildren<TMPro.TMP_Text>().text = RenameAnchorHandler.LoopLabel(anchorProperties.anchorLabel);
                    }

                    anchorRender.transform.SetParent(anchor.transform,false);
                    Log.debug($" {anchorProperties.anchorLabel} 's anchorRender scale is :{anchorRender.gameObject.transform.localScale.x}, anchor scale is : {anchor.transform.localScale.x}");
                    FindObjectOfType<AnchorButtonPopulator>().AddAnchorToButtonList(anchorProperties);
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

    }

    //method to find anchors based on geolocation + wifi
    public async void FindAnchorsByLocation()
    {
        Log.debug("Getting ActiveWatchers");

        if (anchorFirstTimeFound)
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
        Debug.Log($"Creating Watcher ");
        spatialAnchorManager.Session.CreateWatcher(anchorLocateCriteria);
        Debug.Log("Watcher is created");
        anchorFirstTimeFound = true;
    }

    public async Task ResetSession()
    {
        foreach (ARAnchor anchor in aRAnchorManager.trackables)
        {
            if (anchor == null)
            {
                break;
            }
            Log.debug($"-----ARFoundation removing {anchor.trackableId}");
            aRAnchorManager.RemoveAnchor(anchor);
            Log.debug("-----ARFoundation removed anchor ");
        }
        anchorManager.ClearCloudSpatialAnchorList();
        Log.debug($"-----SpatialAnchorManager Session is about to Reset:{anchorFirstTimeFound}");
        spatialAnchorManager.StopSession();
        await spatialAnchorManager.ResetSessionAsync();
        ConfigureSensors();
        await spatialAnchorManager.StartSessionAsync();
        Log.debug($"-----SpatialAnchorManager Reset Complete:{anchorFirstTimeFound}");
        Log.debug("Reset Anchor Lerper");
        FindObjectOfType<AnchorLerper>().ResetLerper();
    }

    public bool CheckLocationPermissions()
    {
        bool permissionsGranted = false;
#if UNITY_ANDROID
        permissionsGranted = Permission.HasUserAuthorizedPermission(Permission.FineLocation);
#elif UNITY_IOS
        CLAuthorizationStatus locationAuthorizationStatus = CocoaHelpersBridge.GetLocationAuthorizationStatus();
        permissionsGranted = (locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedAlways || locationAuthorizationStatus == CLAuthorizationStatus.AuthorizedWhenInUse);
#endif
        Log.debug($"Location Permission : {permissionsGranted}");
        return permissionsGranted;
    }  

    private void UpdateAnchorProperties(CloudSpatialAnchor cloudAnchor, AnchorProperties anchorProperties)
    {
        Log.debug($"anchor properties local anchor local scale is : {anchorProperties.gameObject.transform.localScale.x.ToString()}");
        cloudAnchor.AppProperties[AnchorProperties.ScaleKey] = anchorProperties.gameObject.transform.localScale.x.ToString();
        cloudAnchor.AppProperties[AnchorProperties.DateKey] = anchorProperties.date;
        if (anchorProperties.anchorLabel != null)
        {
            cloudAnchor.AppProperties[AnchorProperties.AnchorLabelKey] = anchorProperties.anchorLabel;
        }
    }
    private void GetAnchorProperties(CloudSpatialAnchor cloudAnchor, AnchorProperties anchorProperties)
    {
        anchorProperties.anchorID = cloudAnchor.Identifier;

        anchorProperties.anchorLabel = cloudAnchor.AppProperties.SafeGet(AnchorProperties.AnchorLabelKey);
        anchorProperties.dateSecondsString = cloudAnchor.AppProperties.SafeGet(AnchorProperties.DateKey);
        anchorProperties.cloudSpatialAnchor = cloudAnchor;
        
        var scaleString = cloudAnchor.AppProperties.SafeGet(AnchorProperties.ScaleKey);
        if(scaleString is string)
        {
            float x = float.Parse(scaleString);
            Log.debug($"anchor properties from cloud anchor local scale is : {x}");
            anchorProperties.gameObject.transform.localScale = new Vector3(1f,1f,1f) * x;
            Log.debug($"anchor properties from cloud anchor  after local scale is : {anchorProperties.gameObject.transform.localScale}");
        }
       
    }
    public async Task CreateCloudAnchor(CloudSpatialAnchor cloudAnchor, AnchorProperties anchorProperties)
    {
        anchorProperties.dateSecondsString = $"{(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}";
        UpdateAnchorProperties(cloudAnchor, anchorProperties);
        if (cloudAnchor == null)
        {
            Log.debug("Cloud anchor is null");
            return;
        }
        tutorialManager.gameObject.SetActive(true);

        anchorInfoText.GetComponentInParent<FadeText>().ShowText();
        while (!spatialAnchorManager.IsReadyForCreate)
        {
            progressBar.SetActive(true);
            Log.debug($"Not enough environmental data : {spatialAnchorManager.SessionStatus.RecommendedForCreateProgress}");
            anchorInfoText.text = $"Look around to gather more data:";
            progressBar.GetComponent<Slider>().value = spatialAnchorManager.SessionStatus.RecommendedForCreateProgress;
            await Task.Delay(333);
        }
        tutorialManager.gameObject.SetActive(false);

        try
        {
            Log.debug("Trying to create cloud anchor");
            anchorInfoText.text = $"Trying to create cloud anchor";
            progressBar.SetActive(false);
            await spatialAnchorManager.CreateAnchorAsync(cloudAnchor);
            anchorInfoText.GetComponentInParent<FadeText>().SetText("Anchor created!");
        }
        catch (Exception ex)
        {
            Log.debug(ex.Message);
            anchorInfoText.GetComponentInParent<FadeText>().SetText("Oops there was problem created anchor!");
        }
    }
    public async Task UpdateExistingAnchor(CloudSpatialAnchor cloudAnchor, AnchorProperties anchorProperties)
    {
        UpdateAnchorProperties(cloudAnchor, anchorProperties);
        try
        {
            Log.debug("Trying to update current cloudAnchor");
            anchorInfoText.text = $"Trying to update cloud anchor";
            //progressBar.SetActive(false);
            await spatialAnchorManager.Session.UpdateAnchorPropertiesAsync(cloudAnchor);
            anchorInfoText.GetComponentInParent<FadeText>().SetText("Anchor updated!");
        }
        catch (Exception ex)
        {
            Log.debug(ex.Message);
            anchorInfoText.GetComponentInParent<FadeText>().SetText("Oops there was problem updated anchor!");
        }
        FindAnchorsByLocation();

    }

}
