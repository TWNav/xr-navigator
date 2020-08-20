using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors;

public class ARTapHandler : MonoBehaviour
{
    [SerializeField]

    private GameObject anchorContainerRender;
    private ARRaycastManager aRRaycastManager;
    private Pose pose;
    private ARAnchorManager aRAnchorManager;
    private EventSystem eventSystem;
    private AnchorConverter anchorConverter;
    private AnchorManager anchorManager;
    private GameObject objectToPlace;
    public AppMode currentAppMode;

    private AppController appController;
    private bool inputTouchExists => Input.touchCount == 1;
    private bool inputNotTouchingUIElement => eventSystem == null || !eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

    // Start is called before the first frame update
    void Start()
    {
        anchorConverter = FindObjectOfType<AnchorConverter>();
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRAnchorManager = GetComponent<ARAnchorManager>();
        eventSystem = FindObjectOfType<EventSystem>();
        anchorManager = FindObjectOfType<AnchorManager>();
        appController = FindObjectOfType<AppController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentAppMode = appController.appMode;
        if (inputTouchExists && inputNotTouchingUIElement)
        {
            switch(currentAppMode)
            {
                case AppMode.Create : AttemptAnchorMovement();
                                        break;
                case AppMode.Select:  SelectAnchor();
                                        break;
                default : return;                                
            }
        }
    }

    private void AttemptAnchorMovement()
    {
        if (isValidPositionPhyRaycast())
            {
                PlaceOrMoveGameObject();
                return;
            }
    }
    private void SelectAnchor()
    {
        GameObject selectedAnchor = isValidAnchorPhyRaycast();
        if (selectedAnchor != null)
        {
            Debug.Log("CloudSpatialAnchor Found.");
            SelectAnchor(selectedAnchor);
            Debug.Log("CloudSpatialAnchor Selected.");
            appController.ShowAnchorOptions();
            return;
        }

    }
    private void PlaceOrMoveGameObject()
    {
        if (objectToPlace == null)
        {
            objectToPlace = Instantiate(new GameObject(), pose.position, pose.rotation);
            GameObject anchorRender = Instantiate(anchorContainerRender, objectToPlace.transform.position, objectToPlace.transform.rotation);
            anchorRender.transform.SetParent(objectToPlace.transform);
        }
        objectToPlace.transform.position = pose.position;
        objectToPlace.transform.rotation = pose.rotation;
    }
    public async void PlaceAnchor()
    {

        CloudNativeAnchor cna = objectToPlace.AddComponent<CloudNativeAnchor>();
        if (cna.CloudAnchor == null)
        {
            Log.debug("Calling Native to Cloud");
            cna.NativeToCloud();
        }
        Log.debug($"CNA : {cna.enabled}");
        CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;
        Log.debug($"AnchorConverter exists : {anchorConverter != null}");
        await anchorConverter.CreateCloudAnchor(cloudAnchor);
        Destroy(objectToPlace);
        anchorConverter.FindAnchorsByLocation();


    }
    private bool isValidPositionPhyRaycast()
    {
        bool result = false;
        int hitMask = 1 << LayerMask.NameToLayer("AR Planes");
        Log.debug($"Layer returned : {hitMask}");
        var rayOrigin = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit[] rayCastHits = new RaycastHit[10];
        var physicsRayCastHits = Physics.RaycastNonAlloc(rayOrigin, rayCastHits, Mathf.Infinity, hitMask);

        if (rayCastHits[0].collider != null)
        {
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            result = true;
            pose = new Pose(rayCastHits[0].point, Quaternion.LookRotation(cameraBearing));
        }
        return result;
    }
    private GameObject isValidAnchorPhyRaycast()
    {
        GameObject selectedAnchor = null;
        int hitMask = 1 << LayerMask.NameToLayer("AR Anchors");
        Log.debug($"Layer returned : {hitMask}");
        var rayOrigin = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit[] rayCastHits = new RaycastHit[10];
        var physicsRayCastHits = Physics.RaycastNonAlloc(rayOrigin, rayCastHits, Mathf.Infinity, hitMask);

        if (rayCastHits[0].collider != null)
        {
            Log.debug($"raycasthit[0] collider hit : {rayCastHits[0].collider.gameObject.name}");
            selectedAnchor = rayCastHits[0].collider.gameObject.GetComponentInParent<AnchorProperties>().gameObject;
        }
        return selectedAnchor;
    }
    private void SelectAnchor(GameObject anchorToSelect)
    {
        Log.debug($"{anchorToSelect.GetComponent<AnchorProperties>().anchorID}");
        string anchorIdentifier = anchorToSelect.GetComponent<AnchorProperties>().anchorID;
        anchorManager.SelectAnchor(anchorIdentifier);
    }
}
