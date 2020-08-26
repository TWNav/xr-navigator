using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchAnchorRenderMaterial(GameObject objectToSwitch, Material material)
    {
        var renderers= objectToSwitch.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            if(renderer.gameObject.GetComponent<TMP_Text>() == null)
            {
                renderer.material = material;
            }
            else
            {
                renderer.gameObject.GetComponent<TMP_Text>().color = material.color;
            }
        } 
    }
}
