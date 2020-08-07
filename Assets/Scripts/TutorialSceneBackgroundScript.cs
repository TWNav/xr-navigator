using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;





public class TutorialSceneBackgroundScript : MonoBehaviour
{
    private Task locationTask;
    private bool isLocationEndabled {
        get {
            return Input.location.isEnabledByUser;
        }
    }
    void Start() {
       locationTask = RequestLocationPermission();
    }
    void Update()
    {
        if(!isLocationEndabled) return;
        // TODO: Make this a click handler
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
    
            SwitchScene();
        }    
    }
    public Task RequestLocationPermission(){
        var t = new Task( () => {
        if(isLocationEndabled)
        {

            Debug.Log("Location enabled.");
            return;
        }
        Debug.Log("Location not endabled.");
        Input.location.Start();
        Debug.Log("Location requested.");
        while(Input.location.status == LocationServiceStatus.Initializing)
        {

        }

        Debug.Log($"Location request completed status: {isLocationEndabled}");
        locationTask = null;
    });
        t.Start();
        return t;
    }

    private void SwitchScene() {
        SceneManager.LoadScene("TWNavigatorScene");
    }
}
