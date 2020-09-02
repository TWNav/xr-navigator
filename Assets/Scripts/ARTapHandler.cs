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

    private AnchorLerper anchorLerper;

    private AppController appController;
    private MaterialSwitcher materialSwitcher;
    private bool firstInputJustBegan => Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
    private bool singleInput => Input.touchCount == 1;
    private bool inputNotTouchingUIElement => eventSystem == null || !eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

    private bool uiButtonTouchEventStarted => Input.touchCount > 0 && eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && Input.GetTouch(0).phase == TouchPhase.Began;

    private bool stillInButtonTouch;
    private bool attemptingRescale = false;
    float timeWhenActiveAgain = 0f;
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
        anchorLerper = FindObjectOfType<AnchorLerper>();
    }

    // Update is called once per frame
    void Update()
    {
        currentAppMode = appController.appMode;

        if(Input.touchCount == 0)
        {
            attemptingRescale = false;
        }
        if(firstInputJustBegan && inputNotTouchingUIElement)
        {
            timeWhenActiveAgain = Time.time + 0.05f;
        }
        if(Time.time < timeWhenActiveAgain)
        {
            return;
        }

        if (uiButtonTouchEventStarted || stillInButtonTouch)
        {
            stillInButtonTouch = true;
            if (Input.touchCount == 0)
            {
                stillInButtonTouch = false;
            }
        }
        if (singleInput && inputNotTouchingUIElement && !stillInButtonTouch && !attemptingRescale)
        {
            switch (currentAppMode)
            {
                case AppMode.Create:
                    AttemptAnchorPlaceOrMove();
                    break;
                case AppMode.Edit:
                    AttemptAnchorPlaceOrMove();
                    break;
                case AppMode.Select:
                    SelectAnchorObjectByTouch();
                    break;
                default: return;
            }
        }
        if(Input.touchCount == 2)
        {
            if(currentAppMode == AppMode.Edit)
            {
                attemptingRescale = true;
                ScaleAnchor(currentSelectedAnchor);
            }
        }
    }

    public void ScaleAnchor(GameObject anchorToScale)
    {  
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
        float previousMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;

        float difference = currentMagnitude - previousMagnitude;

        float deltaScale =  .001f * difference;
        
        Debug.Log(deltaScale);
        float currentScale = anchorToScale.transform.localScale.x;
        float newScale = currentScale + deltaScale;
        if(newScale > 1.5f)
        {
            newScale = 1.5f;
        }
        else if (newScale < .25f)
        {
            newScale = .25f;
        }
        anchorToScale.transform.localScale = new Vector3(1f ,1f, 1f) * newScale;
    }

    private void AttemptAnchorPlaceOrMove()
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
            if(currentAppMode == AppMode.Edit)
            {
                return;
            }
            objectToPlace = Instantiate(new GameObject(), pose.position, pose.rotation);
            objectToPlace.AddComponent<AnchorProperties>();
            GameObject anchorRender = Instantiate(anchorContainerRenderDashedRing, new Vector3(0,0,0), Quaternion.Euler(0f,0f,0f));
            appController.EnterEditMode();
            appController.ShowAnchorOptions();

            anchorRender.transform.SetParent(objectToPlace.transform, false);
           
        }
        if(anchorLerper.hasAnchorSelected)
        {
            return;
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
