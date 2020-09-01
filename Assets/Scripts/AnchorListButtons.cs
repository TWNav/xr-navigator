using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorListButtons : MonoBehaviour
{
     private AppController appController;
    [SerializeField]
    private GameObject AnchorList;
    [SerializeField]
    private GameObject AnchorOptions;
    private ARTapHandler aRTapHandler;
    [SerializeField]
    private GameObject anchorsAwayPanel;

    // Start is called before the first frame update
    void Start()
    {
        appController = FindObjectOfType<AppController>();
        aRTapHandler = FindObjectOfType<ARTapHandler>();
        anchorsAwayPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddAnchor()
    {
        appController.appMode = AppMode.Create;
        AnchorList.SetActive(false);
        anchorsAwayPanel.SetActive(true);
        if(aRTapHandler.objectToPlace != null)
        {
            appController.ShowAnchorOptions();
        }
    }
}
