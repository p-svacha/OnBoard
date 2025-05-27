using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ResourceDisplay : MonoBehaviour
{
    [Header("Elements")]
    public Image Icon;
    public TextMeshProUGUI Label;
    public TooltipTarget Tooltip;

    public void Init(ResourceDef res)
    {
        Init(res, Game.Instance.Resources[res]);
    }
    public void Init(KeyValuePair<ResourceDef, int> res)
    {
        Init(res.Key, res.Value);
    }
    public void Init(ResourceDef resource, int amount)
    {
        Icon.sprite = resource.Sprite;
        Label.text = amount.ToString();
        Tooltip.Title = resource.LabelCap;
        Tooltip.Text = resource.Description;
    }
}
