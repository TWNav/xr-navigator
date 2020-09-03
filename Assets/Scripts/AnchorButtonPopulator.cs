using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnchorButtonPopulator : MonoBehaviour
{
    [SerializeField]
    private GameObject contentContainer;
    [SerializeField]
    private GameObject anchorButtonPrefab;
    [SerializeField]
    private GameObject bottomButtonContainer;
    [SerializeField]
    private GameObject addAnchorButton;
    [SerializeField]
    private GameObject scrollBar = null;
    private GameObject canvas;
    private Rect canvasRect;

    private Dictionary<string, GameObject> existingButtons = new Dictionary<string, GameObject>();

    private float containerWidth, containerHeight;

    private List<AnchorProperties> anchorPropertiesList = new List<AnchorProperties>();
    [SerializeField]
    private HorizontalLayoutGroup horizontalLayoutGroup;


    // Start is called before the first frame update
    void Start()
    {
        canvas = bottomButtonContainer.transform.parent.gameObject;
        canvasRect = canvas.GetComponent<RectTransform>().rect;
        RectTransform contentContainerRectTransform = bottomButtonContainer.GetComponent<RectTransform>();
        containerHeight = contentContainerRectTransform.rect.height;
        containerWidth = contentContainerRectTransform.rect.width;
        Log.debug($"Height : {containerHeight},   Width : {containerWidth}");
        addAnchorButton.GetComponent<RectTransform>().sizeDelta = new Vector2(containerWidth/(2.5f), containerHeight*(.8f));
        scrollBar.GetComponent<Scrollbar>().value = 1f;
        
        int spacing = (int)(0.02 * bottomButtonContainer.GetComponent<RectTransform>().rect.width);
        horizontalLayoutGroup.padding.left = spacing;
        horizontalLayoutGroup.padding.right = spacing;
        horizontalLayoutGroup.spacing = spacing;

        
        

    }

    // Update is called once per frame
    void Update()
    {
        Rect currentCanvasRect = canvas.GetComponent<RectTransform>().rect;
        if (canvasRect.x != currentCanvasRect.x || canvasRect.y != currentCanvasRect.y) {
            // set scrollbar value
            ResizeButtons();
            canvasRect = currentCanvasRect;
        }
    }

    private async void ResizeButtons()
    {
        Log.debug("Resizing buttons");
        RectTransform contentContainerRectTransform = bottomButtonContainer.GetComponent<RectTransform>();
        containerHeight = contentContainerRectTransform.rect.height;
        containerWidth = contentContainerRectTransform.rect.width;
        var existingButtons = contentContainer.gameObject.GetComponentsInChildren<Button>();
        foreach(Button button in existingButtons)
        {
            Log.debug($"Resizing button : {button.gameObject.transform.name}");
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(containerWidth/(2.5f), containerHeight*(.8f));
        }
        Log.debug($"Value before: {scrollBar.GetComponent<Scrollbar>().value}");
        await Task.Delay(10);
        Canvas.ForceUpdateCanvases();
        scrollBar.GetComponent<Scrollbar>().value = 1f;
        Log.debug($"Value before: {scrollBar.GetComponent<Scrollbar>().value}");
        var buttonResizers = Resources.FindObjectsOfTypeAll(typeof(ResizeButtonFont));
        foreach(ResizeButtonFont buttonResizer in buttonResizers)
        {
            if(buttonResizer.gameObject.name.Equals("Top Button Container"))
            {
                Log.debug($"Trying to resize font within {buttonResizer.gameObject.name}");
                buttonResizer.EnableAutoSizing();
                await buttonResizer.ResizeFont();
            } 
        }
        foreach(ResizeButtonFont buttonResizer in buttonResizers)
        {
            if(!buttonResizer.gameObject.name.Equals("Top Button Container"))
            {
                Log.debug($"Trying to resize font within {buttonResizer.gameObject.name}");
                buttonResizer.EnableAutoSizing();
                await buttonResizer.ResizeFont();
            } 
        }
    }

    public async void AddAnchorToButtonList(AnchorProperties anchorProperties)
    {
        if(ButtonExists(anchorProperties))
        {
            Log.debug($"Button for {anchorProperties.name} exists");
            existingButtons.TryGetValue(anchorProperties.anchorID, out anchorProperties.button);
            anchorProperties.button.GetComponent<AnchorButtonHandler>().anchorProperties = anchorProperties;
            return;
        }
        Log.debug($"Button for {anchorProperties.name} does not exist");
        GameObject buttonToAdd = Instantiate(anchorButtonPrefab) as GameObject;
        buttonToAdd.name = $"Anchor Button: {anchorProperties.anchorLabel}";
        TMP_Text tmpText =  buttonToAdd.GetComponentInChildren<TMP_Text>();
        tmpText.text = anchorProperties.anchorLabel;
        tmpText.enableAutoSizing = false;
        tmpText.fontSize = addAnchorButton.GetComponentInChildren<TMP_Text>().fontSize;
        buttonToAdd.GetComponent<RectTransform>().sizeDelta = new Vector2(containerWidth/(2.5f), containerHeight*(.8f));
        buttonToAdd.transform.SetParent(contentContainer.transform);        
        anchorProperties.button = buttonToAdd;
        AnchorButtonHandler anchorButtonHandler = buttonToAdd.GetComponent<AnchorButtonHandler>();
        anchorButtonHandler.anchorProperties = anchorProperties;
        existingButtons.Add(anchorProperties.anchorID, buttonToAdd);
        buttonToAdd.transform.SetSiblingIndex(0);
        SortButtons();
        await Task.Delay(50);
        Canvas.ForceUpdateCanvases();
        scrollBar.GetComponent<Scrollbar>().value = 1f;   
    }

    private void SortButtons()
    {
        try {
            var buttons = contentContainer.GetComponentsInChildren<AnchorButtonHandler>();
            Array.Sort(buttons, (bl, br) => {
                return bl.anchorProperties.dateSeconds.CompareTo(br.anchorProperties.dateSeconds);
            });
            var idx = 0;
            foreach(var button in buttons) {
                button.gameObject.transform.SetSiblingIndex(idx++);
            }
        } catch(Exception err) {
            Log.error($"Error Sortring array ${err}");
        }
    }


    private bool ButtonExists(AnchorProperties anchorProperties)
    {
        return existingButtons.ContainsKey(anchorProperties.anchorID);
    }

    public void RemoveAnchorFromDictionary(string anchorID)
    {
        existingButtons.Remove(anchorID);
    }
}
