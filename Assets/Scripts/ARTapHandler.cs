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

    private GameObject anchorContainerRenderDashedRing;
    private ARRaycastManager aRRaycastManager;
    private Pose pose;
    private ARAnchorManager aRAnchorManager;
    private EventSystem eventSystem;
    private AnchorConverter anchorConverter;
    private AnchorManager anchorManager;
    public GameObject objectToPlace;
    public AppMode currentAppMode;

    private AppController appController;
    private MaterialSwitcher materialSwitcher;
    private bool inputTouchExists => Input.touchCount == 1;
    private bool inputNotTouchingUIElement => eventSystem == null || !eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

    private bool uiButtonTouchEventStarted => Input.touchCount > 0 && eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.GetTouch(0).phase == TouchPhase.Began;

    private bool stillInButtonTouch;
    [SerializeField]
    private Material defaultAnchorMaterial, selectedAnchorMaterial, deleteAnchorMaterial;
    public GameObject previousSelectedAnchor { get; set; }
    public GameObject currentSelectedAnchor { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        anchorConverter = FindObjectOfType<AnchorConverter>();
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRAnchorManager = GetComponent<ARAnchorManager>();
        eventSystem = FindObjectOfType<EventSystem>();
        anchorManager = FindObjectOfType<AnchorManager>();
        appController = FindObjectOfType<AppController>();
        materialSwitcher = FindObjectOfType<MaterialSwitcher>();
    }

    // Update is called once per frame
    void Update()
    {
        currentAppMode = appController.appMode;

        if (uiButtonTouchEventStarted || stillInButtonTouch)
        {
            stillInButtonTouch = true;
            if (Input.touchCount == 0)
            {
                stillInButtonTouch = false;
            }
        }
        if (inputTouchExists && inputNotTouchingUIElement && !stillInButtonTouch)
        {
            switch (currentAppMode)
            {
                case AppMode.Create:
                    AttemptAnchorMovement();
                    break;
                case AppMode.Select:
                    SelectAnchorObjectByTouch();
                    break;
                default: return;
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
    private void SelectAnchorObjectByTouch()
    {

        GameObject selectedAnchor = isValidAnchorPhyRaycast();
        Debug.Log($"selectedAnchor is : {selectedAnchor.GetInstanceID()}");
        if (selectedAnchor != null)
        {
            Debug.Log("CloudSpatialAnchor Found.");
            SelectAnchor(selectedAnchor);

            appController.EnterEditMode();
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
            objectToPlace.AddComponent<AnchorProperties>();
            GameObject anchorRender = Instantiate(anchorContainerRenderDashedRing, new Vector3(0,0,0), Quaternion.Euler(0f,0f,0f));
            appController.EnterEditMode();
            appController.ShowAnchorOptions();

            anchorRender.transform.SetParent(objectToPlace.transform, false);
           
        }
        
        objectToPlace.transform.position = pose.position;
        objectToPlace.transform.rotation = pose.rotation;
        SwitchSelectedAnchor(objectToPlace);
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
        Debug.Log($"AnchorConverter exists : {anchorConverter != null}");
        await anchorConverter.CreateCloudAnchor(cloudAnchor, objectToPlace.GetComponent<AnchorProperties>());
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
            var planeRotation = rayCastHits[0].transform.rotation.eulerAngles;
            result = true;
            pose = new Pose(rayCastHits[0].point, Quaternion.Euler(planeRotation));
            
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
    public void SelectAnchor(GameObject anchorToSelect)
    {
        if (anchorToSelect != null)
        {
            Debug.Log($"anchorToSelect is : {anchorToSelect.GetInstanceID()}");
            SwitchSelectedAnchor(anchorToSelect);
            Debug.Log($"{anchorToSelect.GetComponent<AnchorProperties>().anchorID}");
            string anchorIdentifier = anchorToSelect.GetComponent<AnchorProperties>().anchorID;
            anchorManager.SelectAnchor(anchorIdentifier);


        }

    }
    private void SwitchSelectedAnchor(GameObject objectSelected)
    {
        Log.debug($"The objectSelected is {objectSelected.GetComponent<AnchorProperties>().anchorLabel}");
        if (currentSelectedAnchor != null)
        {
            Log.debug($"The current Selected Anchor is {currentSelectedAnchor.GetComponent<AnchorProperties>().anchorLabel}");
        }
        if (currentSelectedAnchor != null && objectSelected == currentSelectedAnchor)
        {
            Log.debug("objectSelected Anchor is equal to currentSelected Anchor");
            return;
        }
        else
        {
            if (previousSelectedAnchor != null)
            {
                Log.debug($"The previousSelectedAnchor is {previousSelectedAnchor.GetComponent<AnchorProperties>().anchorLabel}");
            }
            else
            {
                Log.debug("Previous Selected Anchor is null");
            }
            previousSelectedAnchor = currentSelectedAnchor;

            if (currentSelectedAnchor != null)
            {
                Log.debug($"The currentSelectedAnchor after switch is {currentSelectedAnchor.GetComponent<AnchorProperties>().anchorLabel}");
            }
            else
            {
                Log.debug("Current Selected Anchor is null");
            }
            currentSelectedAnchor = objectSelected;
            if (previousSelectedAnchor != null)
            {
                materialSwitcher.SwitchAnchorRenderMaterial(previousSelectedAnchor, defaultAnchorMaterial);
                Log.debug("Previous Selected Anchor has changed to default material");
            }


            materialSwitcher.SwitchAnchorRenderMaterial(currentSelectedAnchor, selectedAnchorMaterial);
            Log.debug("Current Selected Anchor has changed to selected material");
        }

    }
}
