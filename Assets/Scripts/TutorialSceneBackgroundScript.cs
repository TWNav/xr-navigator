using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSceneBackgroundScript : MonoBehaviour
{
    void Update()
    {
        // TODO: Make this a click handler
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
            SwitchScene();
        }    
    }

    private void SwitchScene() {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
