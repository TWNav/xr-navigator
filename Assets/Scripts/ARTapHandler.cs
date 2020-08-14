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
            GameObject selectedAnchor = isValidAnchorPhyRaycast();
            if (selectedAnchor != null)
            {
                Debug.Log("CloudSpatialAnchor Found.");
                SelectAnchor(selectedAnchor);
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
        GameObject anchorRender = Instantiate(anchorContainerRender,tempAnchor.transform.position,tempAnchor.transform.rotation);
        anchorRender.transform.SetParent(tempAnchor.transform);
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
    private GameObject isValidAnchorPhyRaycast()
    {
        GameObject selectedAnchor = null;
        int hitMask = 1 << LayerMask.NameToLayer("AR Anchors");
        Debug.Log($"Layer returned : {hitMask}");
        var rayOrigin = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit[] rayCastHits = new RaycastHit[10];
        var physicsRayCastHits = Physics.RaycastNonAlloc(rayOrigin, rayCastHits, Mathf.Infinity, hitMask);

        if (rayCastHits[0].collider != null)
        {
            Debug.Log($"raycasthit[0] collider hit : {rayCastHits[0].collider.gameObject.name}");
            selectedAnchor = rayCastHits[0].collider.gameObject.GetComponentInParent<AnchorProperties>().gameObject;
        }
        return selectedAnchor;
    }
    private void SelectAnchor(GameObject anchorToSelect)
    {
        Debug.Log($"{anchorToSelect.GetComponent<AnchorProperties>().anchorID}");
        string anchorIdentifier = anchorToSelect.GetComponent<AnchorProperties>().anchorID;
        anchorManager.SelectAnchor(anchorIdentifier);
    }
}
