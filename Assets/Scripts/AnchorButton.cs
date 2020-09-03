using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchorButton : UnityEngine.UI.Button
{
    private AnchorManager anchorManager;
    private ARTapHandler aRTapHandler;
    private AnchorButtonHandler anchorButtonHandler;
    private AnchorProperties buttonAnchorProperties ;

    private AnchorProperties currentSelectedAnchorProperties;

    protected override void Start()
    {
        aRTapHandler = FindObjectOfType<ARTapHandler>();
        base.Start();
        ColorTween(this.colors.normalColor * this.colors.colorMultiplier);
    }


    void Update()
    {
        if(aRTapHandler == null || aRTapHandler.currentSelectedAnchor == null)
        {
            ColorTween(this.colors.normalColor * this.colors.colorMultiplier);
            return;
        }
        buttonAnchorProperties = this.gameObject.GetComponent<AnchorButtonHandler>().anchorProperties;
        currentSelectedAnchorProperties = aRTapHandler.currentSelectedAnchor.GetComponent<AnchorProperties>();
        if (buttonAnchorProperties != null && currentSelectedAnchorProperties != this.gameObject.GetComponent<AnchorButtonHandler>().anchorProperties)
        {
            ColorTween(this.colors.normalColor * this.colors.colorMultiplier);
        }

    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if(aRTapHandler == null)
        {
            return;
        }
        Color color;
        switch (state)
        {
            case Selectable.SelectionState.Normal:
                if (aRTapHandler.currentSelectedAnchor == null)
                {
                    color = this.colors.normalColor;
                }
                else if (aRTapHandler.currentSelectedAnchor != null &&
                        aRTapHandler.currentSelectedAnchor.Equals(buttonAnchorProperties.gameObject))
                {
                    color = this.colors.selectedColor;
                }
                else
                {
                    color = this.colors.normalColor;
                }
                break;
            case Selectable.SelectionState.Highlighted:
                color = this.colors.highlightedColor;
                break;
            case Selectable.SelectionState.Pressed:
                color = this.colors.pressedColor;
                break;
            case Selectable.SelectionState.Disabled:
                color = this.colors.disabledColor;
                break;
            case Selectable.SelectionState.Selected:
                color = this.colors.selectedColor;
                break;
            default:
                color = Color.black;
                break;
        }
        if (base.gameObject.activeInHierarchy)
        {
            switch (this.transition)
            {
                case Selectable.Transition.ColorTint:
                    ColorTween(color * this.colors.colorMultiplier);
                    break;
            }
        }
    }

    private void ColorTween(Color targetColor)
    {
        if (this.targetGraphic == null)
        {
            this.targetGraphic = this.image;
        }

        base.image.color = targetColor;

    }
}
