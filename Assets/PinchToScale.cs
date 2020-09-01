using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PinchToScale : MonoBehaviour
{
    [SerializeField]
    private GameObject anchorPrefab;

    private GameObject lastPlacedAnchor;
    private EventSystem eventSystem;
    private Pose pose;

    private bool firstInputJustBegan => Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;

    private bool singleInput => Input.touchCount == 1;
    private bool inputNotTouchingUIElement => eventSystem == null || !eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

    private bool attemptingRescale = false;

    float timeWhenActiveAgain = 0f;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 0)
        {
            attemptingRescale = false;
        }
        if(firstInputJustBegan)
        {
            timeWhenActiveAgain = Time.time + 0.2f;
        }
        if(Time.time < timeWhenActiveAgain)
        {
            return;
        }
        if(singleInput && inputNotTouchingUIElement && !attemptingRescale)
        {
            AttemptAnchorMovement();
        }
        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float previousMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;

            float difference = currentMagnitude - previousMagnitude;

            ScaleAnchor(.001f * difference, lastPlacedAnchor);
            attemptingRescale = true;
        }
    }

    public void ScaleAnchor(float deltaScale, GameObject anchorToScale)
    {  
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

    private void AttemptAnchorMovement()
    {
        if (isValidPositionPhyRaycast())
        {
            PlaceOrMoveGameObject();
            return;
        }
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

     private void PlaceOrMoveGameObject()
    {
        if (lastPlacedAnchor == null)
        {
            lastPlacedAnchor = Instantiate(new GameObject(), pose.position, pose.rotation);
            lastPlacedAnchor.AddComponent<AnchorProperties>();
            GameObject anchorRender = Instantiate(anchorPrefab, new Vector3(0,0,0), Quaternion.Euler(0f,0f,0f));
            anchorRender.transform.SetParent(lastPlacedAnchor.transform, false);      
        }
        lastPlacedAnchor.transform.position = pose.position;
        lastPlacedAnchor.transform.rotation = pose.rotation;
    }

}
