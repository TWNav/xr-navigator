using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelButtons : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    [SerializeField]    
    private GameObject createButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
    public void RedirectToCreateMode()
    {
        createButton.GetComponent<MenuButton>().ClickButton();
        FindObjectOfType<UITopMenuController>().ManageButton();
        panel.SetActive(false);
    }
}
