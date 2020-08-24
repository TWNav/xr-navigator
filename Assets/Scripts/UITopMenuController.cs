using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITopMenuController : MonoBehaviour
{
    private AppController appController;
    private AppModeEvent appModeEvent; 

    [SerializeField]
    private GameObject AnchorList;
    [SerializeField]
    private GameObject AnchorOptions;

    [SerializeField]
    private TMP_Text anchorInfoText, distanceText;
    // Start is called before the first frame update
    void Start()
    {
        appController = FindObjectOfType<AppController>();
        AnchorList.SetActive(true);
        AnchorOptions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ManageButton()
    {
        appController.appMode = AppMode.Select;
        AnchorList.SetActive(true);
        AnchorOptions.SetActive(false);
        anchorInfoText.gameObject.SetActive(true);
        distanceText.gameObject.SetActive(false);
    }
    public void ExploreButton()
    {
         appController.appMode = AppMode.Explore;
        AnchorList.SetActive(false);
        AnchorOptions.SetActive(false);
        anchorInfoText.gameObject.SetActive(false);
        distanceText.gameObject.SetActive(true);
    } 
}
