using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorButtonHandler : MonoBehaviour
{
    public AnchorProperties anchorProperties;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectAnchorByButton()
    {
        FindObjectOfType<ARTapHandler>().SelectAnchor(anchorProperties.gameObject);
    }
}
