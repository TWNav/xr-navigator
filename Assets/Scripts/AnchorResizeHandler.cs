using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchorResizeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject currentSelectedAnchor;
    private ARTapHandler aRTapHandler;
    private float minSize = 0.25f;
    private float maxSize = 1.5f;
    void Start()
    {
        aRTapHandler = FindObjectOfType<ARTapHandler>();
        this.gameObject.GetComponent<Slider>().value = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSize()
    {
        currentSelectedAnchor = aRTapHandler.currentSelectedAnchor;
        float newScale = minSize + this.gameObject.GetComponent<Slider>().value * (maxSize-minSize);
        currentSelectedAnchor.transform.localScale = new Vector3(1f,1f,1f) * newScale;
    }
}
