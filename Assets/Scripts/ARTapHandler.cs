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
    private ARRaycastManager aRRaycastManager;
    private Pose pose;
    private ARAnchorManager aRAnchorManager;
    private EventSystem eventSystem;
    private AnchorConverter anchorConverter;

    private bool inputDidBegin {
        get {
            return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
    }

    private bool inputNotTouchingUIElement
    {
        get{
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
    }

    // Update is called once per frame
    void Update()
    {
        if(inputDidBegin && inputNotTouchingUIElement)
        {
           PlaceAnchor();
        }
    }
    
    private async void PlaceAnchor()
    {
        if(isValidPositionPhyRaycast())
        {
            GameObject tempAnchor = Instantiate(new GameObject(), pose.position, pose.rotation);
            CloudNativeAnchor cna = tempAnchor.AddComponent<CloudNativeAnchor>();
            if(cna.CloudAnchor == null)
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
        } 
    }
    private bool isValidPositionARRaycast()
    {
        bool result = false;
        var rayOrigin = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        List<ARRaycastHit> rayCastHits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(rayOrigin, rayCastHits,TrackableType.Planes);

        if(rayCastHits.Count > 0)
        {
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x,0,cameraForward.z).normalized;
            result = true;
            pose = rayCastHits[0].pose;
            pose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        return result;
    }
     private bool isValidPositionPhyRaycast()
    {
        bool result = false;
        int hitMask = 1 << LayerMask.NameToLayer("AR Planes");
        Debug.Log($"Layer returned : {hitMask}");
        var rayOrigin = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit[] rayCastHits = new RaycastHit[10];
        var physicsRayCastHits =  Physics.RaycastNonAlloc(rayOrigin,rayCastHits,Mathf.Infinity,hitMask);

        if(rayCastHits[0].collider != null)
        {
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x,0,cameraForward.z).normalized;
            result = true;
            pose = new Pose(rayCastHits[0].point,Quaternion.LookRotation(cameraBearing));
        }
        return result;
    }
}
