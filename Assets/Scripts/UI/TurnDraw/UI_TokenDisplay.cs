using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TokenDisplay : MonoBehaviour
{
    public void Init(Token token, TokenSurface surface)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(token.Size.UiSize, 50);
        GetComponent<LayoutElement>().preferredHeight = token.Size.UiSize;

        Image image = GetComponent<Image>();
        image.color = surface.Color.Color;
    }
}
