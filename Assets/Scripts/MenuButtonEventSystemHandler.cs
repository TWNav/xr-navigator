using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonEventSystemHandler : MonoBehaviour
{
    private EventSystem eventSystem;
    public GameObject lastSelectedMenuButton = null;
    private MenuButton[] menuButtonScripts;

    private ArrayList menuButtons = new ArrayList();
    void Start()
    {
        eventSystem = GetComponent<EventSystem>();
        menuButtonScripts = FindObjectsOfType<MenuButton>();
        for(int i = 0; i < menuButtonScripts.Length; i++)
        {
            menuButtons.Add(menuButtonScripts[i].gameObject);
            if(menuButtonScripts[i].gameObject.name.Equals("Manage Button"))
            {
                lastSelectedMenuButton = menuButtonScripts[i].gameObject;
            }
        }
        Log.debug($"Menu Buttons detected : {menuButtons.Count}");
    }

    void Update()
    {
        if (eventSystem != null)
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                if(menuButtons.Contains(eventSystem.currentSelectedGameObject) && eventSystem.currentSelectedGameObject != lastSelectedMenuButton)
                {
                    lastSelectedMenuButton = eventSystem.currentSelectedGameObject;
                    Log.debug($"Menu button clicked.  Switching lastSelectedMenuButton to {lastSelectedMenuButton.gameObject.name}");
                }
            }
            else
            {
                eventSystem.SetSelectedGameObject(lastSelectedMenuButton);
            }
        }
    }
}
