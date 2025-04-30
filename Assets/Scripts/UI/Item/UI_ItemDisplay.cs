using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemDisplay : MonoBehaviour
{
    [Header("Elements")]
    public Image Icon;
    public TooltipTarget Tooltip;

    public void Init(Item item)
    {
        Icon.sprite = item.Sprite;
        Tooltip.Title = item.Label;
        Tooltip.Text = item.Descrption;
    }
}
