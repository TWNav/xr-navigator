using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class ResizeButtonFont : MonoBehaviour
{
    [SerializeField]
    private GameObject topButtonContainer;
    async void Start()
    {
        await ResizeFont();
    }

    public async Task ResizeFont()
    {
        await Task.Delay(10);
        Canvas.ForceUpdateCanvases(); 
        float smallestFontSizeInButtonWell = 1000f;
        float maxFontSizeInApp;
        var buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            var text = button.GetComponentInChildren<TMP_Text>();
            if (text.fontSize < smallestFontSizeInButtonWell)
            {
                smallestFontSizeInButtonWell = text.fontSize;
            }
        }

        if (!this.gameObject.Equals(topButtonContainer))
        {
            maxFontSizeInApp = topButtonContainer.GetComponentInChildren<TMP_Text>().fontSize;
            if(smallestFontSizeInButtonWell > maxFontSizeInApp) {
                smallestFontSizeInButtonWell = maxFontSizeInApp;
            }
        }
        
        foreach (Button button in buttons)
        {
            var text = button.GetComponentInChildren<TMP_Text>();
            text.enableAutoSizing = false;
            text.fontSize = smallestFontSizeInButtonWell;
        }
        return;
    }

    public void EnableAutoSizing()
    {
        var buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.GetComponentInChildren<TMP_Text>().enableAutoSizing = true;
        }
    }
}
