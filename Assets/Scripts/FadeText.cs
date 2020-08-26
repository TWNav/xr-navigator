using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textToFade;

    public void SetText(string message)
    {
        ShowText();
        textToFade.text = message;
        FadeOut();
    }

    //Fade time in seconds
    private float fadeOutTime = 1f;
    public float waitTime = 5f;
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void ShowText()
    {
        TMP_Text text = textToFade;
        Color originalColor = text.color;
        originalColor.a = 1f;
        text.color = originalColor;
    }
    private IEnumerator FadeOutRoutine()
    {
        TMP_Text text = textToFade;
        Color originalColor = text.color;
        for (float t = 0.01f; t < (waitTime + fadeOutTime); t += Time.deltaTime)
        {
            if (t > waitTime)
            {
                Color tempColor = text.color;
                tempColor.a = Mathf.Lerp(originalColor.a, Color.clear.a, Mathf.Min(1, t - waitTime / fadeOutTime));
                text.color = tempColor;
            }
            yield return null;
        }
    }
}
