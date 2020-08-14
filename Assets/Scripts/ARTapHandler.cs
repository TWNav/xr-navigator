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
    
    private bool inputDidBegin
    {
        get
        {
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
    }

    private bool inputNotTouchingUIElement
    {
        get
        {
            return eventSystem == null || !eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anchorConverter = FindObjectOfType<AnchorConverter>();
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRAnchorManager = GetComponent<ARAnchorManager>();
        eventSystem = FindObjectOfType<EventSystem>();
        anchorManager = FindObjectOfType<AnchorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputDidBegin && inputNotTouchingUIElement)
        {
            CloudSpatialAnchor cloudSpatialAnchor = isValidAnchorPhyRaycast();
            if (cloudSpatialAnchor != null)
            {
                Debug.Log("CloudSpatialAnchor Found.");
                SelectAnchor(cloudSpatialAnchor);
                Debug.Log("CloudSpatialAnchor Selected.");
                return;
            }
            if (isValidPositionPhyRaycast())
            {
                PlaceAnchor();
                return;
            }

        }
    }

    private async void PlaceAnchor()
    {

        GameObject tempAnchor = Instantiate(new GameObject(), pose.position, pose.rotation);
        GameObject anchorRender = Instantiate(anchorContainerRender,pose.position,pose.rotation);
        anchorRender.transform.parent = tempAnchor.transform;
        CloudNativeAnchor cna = tempAnchor.AddComponent<CloudNativeAnchor>();
        if (cna.CloudAnchor == null)
        {
            Debug.Log("Calling Native to Cloud");
            cna.NativeToCloud();
        }
        Debug.Log($"CNA : {cna.enabled}");
        CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;
        Debug.Log($"Cloud ID : {cloudAnchor.Identifier}");
        Debug.Log($"AnchorConverter exists : {anchorConverter != null}");
        await anchorConverter.CreateCloudAnchor(cloudAnchor);
        Destroy(tempAnchor);
        anchorConverter.FindAnchorsByLocation();


    }
    private bool isValidPositionPhyRaycast()
    {
        bool result = false;
        int hitMask = 1 << LayerMask.NameToLayer("AR Planes");
        Debug.Log($"Layer returned : {hitMask}");
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
    private CloudSpatialAnchor isValidAnchorPhyRaycast()
    {
        CloudSpatialAnchor cloudSpatialAnchor = null;
        int hitMask = 1 << LayerMask.NameToLayer("AR Anchors");
        Debug.Log($"Layer returned : {hitMask}");
        var rayOrigin = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit[] rayCastHits = new RaycastHit[10];
        var physicsRayCastHits = Physics.RaycastNonAlloc(rayOrigin, rayCastHits, Mathf.Infinity, hitMask);

        if (rayCastHits[0].collider != null)
        {
            Debug.Log($"raycasthit[0] collider hit : {rayCastHits[0].collider.gameObject.name}");
            //FindObjectsOfType<CloudNativeAnchor>();
            CloudNativeAnchor [] list = FindObjectsOfType<CloudNativeAnchor>();
            Debug.Log($"The length of CloudNativeAnchor list : {list.Length}");
            //var csaList = FindObjectsOfType(typeof(CloudSpatialAnchor));
            //Debug.Log($"The length of CloudSpatialAnchor list : {csaList.Length}");
            CloudNativeAnchor cna = rayCastHits[0].collider.gameObject.GetComponentInParent<CloudNativeAnchor>();
            Debug.Log($"Cna exists in parent : {cna!=null}");
            Debug.Log($"{cna.CloudAnchor.Identifier}");
            cna.NativeToCloud();
            cloudSpatialAnchor = cna.CloudAnchor;
            Debug.Log($"cloudSpatialAnchor hit : {cloudSpatialAnchor.Identifier}");
        }
        return cloudSpatialAnchor;
    }
    private void SelectAnchor(CloudSpatialAnchor cloudSpatialAnchor)
    {
        Debug.Log($"{cloudSpatialAnchor.Identifier}");
        anchorManager.SelectAnchor(cloudSpatialAnchor);
    }
}
