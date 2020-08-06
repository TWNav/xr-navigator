using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStyleScript : MonoBehaviour
{
    [SerializeField]
    private Material defaultColor, highlightColor;
 
    // Start is called before the first frame update
    void Start()
    {
        var button = GetComponent<Button>();
        var colorBlock = button.colors;
        colorBlock.normalColor = defaultColor.color;
        colorBlock.selectedColor = highlightColor.color;
        button.colors = colorBlock;
    }
}
