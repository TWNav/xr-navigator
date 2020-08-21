using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnchorLerper : MonoBehaviour
{


    private Transform anchorContainerTransform;
    [SerializeField]
    private Transform cameraContainerTransform;

    private Transform currentRenderTransform;

    private GameObject anchorToLerp;

    private GameObject renderToLerp;
    private Vector3 startingScale, targetScale;
    private Vector3 selectedAnchorOriginalScale;

    public float timeToFocus;
    private float currentLerpTime = 0;

    private bool inAnchorContainer = true;

    private bool lerpingFromContainerToCamera = false;
    private bool lerpingFromCameraToContainer = false;


    // Update is called once per frame
    void Update()
    {
        if(lerpingFromContainerToCamera)
        {
            lerpingFromContainerToCamera = !LerpToTransform(currentRenderTransform.transform, anchorContainerTransform.transform, cameraContainerTransform.transform);
        }
        else if (lerpingFromCameraToContainer)
        {
            lerpingFromCameraToContainer = !LerpToTransform(currentRenderTransform.transform, cameraContainerTransform.transform, anchorContainerTransform.transform);
        }
    }

    public bool LerpToTransform(Transform objectToLerp, Transform originTransform, Transform destinationTransform)
    {
        bool lerpComplete = true;

        currentLerpTime += Time.deltaTime;
        if(currentLerpTime > timeToFocus)
        {
            currentLerpTime = timeToFocus;
        }

        if(Vector3.Distance(objectToLerp.position, destinationTransform.position) > 0.001f)
        {
            objectToLerp.position = Vector3.Lerp(originTransform.position, destinationTransform.position, currentLerpTime / timeToFocus);
            objectToLerp.rotation = Quaternion.Lerp( originTransform.rotation, destinationTransform.rotation, currentLerpTime / timeToFocus);
            objectToLerp.localScale = Vector3.Lerp(startingScale, targetScale, currentLerpTime / timeToFocus);
            lerpComplete = false;
        }
        else{
            Log.debug("Resetting currentLerpTime");
            currentLerpTime = 0;
            if(!inAnchorContainer)
            {
                StopRotation();
            }
        }

        return lerpComplete;
    }

    private void StopRotation()
    {
        renderToLerp.GetComponentInChildren<RotateCircle>().enabled = false;
        Vector3 textRotation = renderToLerp.GetComponentInChildren<RotateCircle>().gameObject.transform.rotation.eulerAngles;
        Log.debug($"{textRotation}");
        renderToLerp.GetComponentInChildren<RotateCircle>().gameObject.transform.rotation = Quaternion.Euler(textRotation.x, textRotation.y, 0f);
    }


    public void SelectAnchor(GameObject anchorToLerp)
    {
        Debug.Log("Calling the lerp");
        if(inAnchorContainer && !(lerpingFromCameraToContainer || lerpingFromContainerToCamera))
        {
            Debug.Log($"anchorToLerp is : {anchorToLerp.GetInstanceID()}");
            renderToLerp = anchorToLerp.transform.GetChild(0).gameObject;
            anchorContainerTransform = anchorToLerp.transform;
            selectedAnchorOriginalScale = renderToLerp.transform.localScale;


            renderToLerp.transform.SetParent(cameraContainerTransform);
            currentRenderTransform = renderToLerp.transform;
            targetScale = new Vector3(1,1,1);
            startingScale = renderToLerp.transform.localScale;
            lerpingFromCameraToContainer = false;
            lerpingFromContainerToCamera = true;
            renderToLerp.GetComponentInChildren<TMP_Text>().enabled = false;
            renderToLerp.GetComponentInChildren<TMP_Text>().enabled = true;
            inAnchorContainer = false;
        }
    }

    public void SubmitAnchor()
    {
        if(!inAnchorContainer && !(lerpingFromCameraToContainer || lerpingFromContainerToCamera))
        {
            renderToLerp.transform.SetParent(anchorContainerTransform);
            currentRenderTransform = renderToLerp.transform;
            targetScale = selectedAnchorOriginalScale;
            startingScale = renderToLerp.transform.localScale;
            lerpingFromContainerToCamera = false;
            lerpingFromCameraToContainer = true;
            renderToLerp.GetComponentInChildren<TMP_Text>().enabled = false;
            renderToLerp.GetComponentInChildren<TMP_Text>().enabled = true;
            inAnchorContainer = true;
            renderToLerp.GetComponentInChildren<RotateCircle>().enabled = true;
        }
    }
}
