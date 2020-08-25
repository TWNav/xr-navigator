using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorButtonHandler : MonoBehaviour
{
    public AnchorProperties anchorProperties;
    private AppController appController;

    // Start is called before the first frame update
    void Start()
    {
        appController = FindObjectOfType<AppController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectAnchorByButton()
    {
        FindObjectOfType<ARTapHandler>().SelectAnchor(anchorProperties.gameObject);
        if(appController.appMode == AppMode.Select)
        {
            appController.EnterEditMode();
            appController.ShowAnchorOptions();
        }
    }
}
