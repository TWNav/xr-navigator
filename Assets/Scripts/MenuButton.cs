using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : UnityEngine.UI.Button
{

    private MenuButtonEventSystemHandler menuButtonEventSystemHandler;

    private bool lastMenuButton = false;

    protected override void Start()
    {
        menuButtonEventSystemHandler = FindObjectOfType<MenuButtonEventSystemHandler>();
        base.Start();
    }


    void Update()
    {
        if(lastMenuButton)
        {
            if(menuButtonEventSystemHandler.lastSelectedMenuButton != this.gameObject)
            {
                lastMenuButton = false;
                ColorTween(this.colors.normalColor * this.colors.colorMultiplier);
            }
        }
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        Color color;
        switch (state)
        {
            case Selectable.SelectionState.Normal:
                if(menuButtonEventSystemHandler == null)
                {
                    color = this.colors.normalColor;
                }
                else if (menuButtonEventSystemHandler.lastSelectedMenuButton != null && 
                        menuButtonEventSystemHandler.lastSelectedMenuButton == this.gameObject)
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
                lastMenuButton = true;
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
