using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddAnchorButtonTextColorSwitcher : Button
{
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        Color color;
        switch (state)
        {
            case Selectable.SelectionState.Normal:
                color = Color.white;
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
        TMP_Text text = GetComponentInChildren<TMP_Text>();

        text.color = targetColor;

    }
}
