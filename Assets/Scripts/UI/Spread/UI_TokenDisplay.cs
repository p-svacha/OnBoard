using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TokenDisplay : MonoBehaviour
{
    [Header("Elements")]
    public Image Token;
    public Image Affinity;
    public TooltipTarget Tooltip;

    public void Init(Token token, TokenSurface surface)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(token.Size.UiSize, 50);
        GetComponent<LayoutElement>().preferredHeight = token.Size.UiSize;

        Token.color = surface.Color.Color;

        Affinity.gameObject.SetActive(token.HasAffinity);
        if (token.HasAffinity) Affinity.color = token.Affinity.Color;

        Tooltip.Title = token.LabelCap;
        Tooltip.Text = surface.Description;
    }
}
