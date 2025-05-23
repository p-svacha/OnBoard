using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TokenDisplay : MonoBehaviour
{
    [Header("Elements")]
    public Image Token;
    public Image Affinity;
    public Image Pattern;
    public TooltipTarget Tooltip;

    public void Init(Token token, TokenSurface surface)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(token.Size.UiSize, 50);
        GetComponent<LayoutElement>().preferredHeight = token.Size.UiSize;

        // Color
        Token.color = surface.Color.Color;

        // Affinity
        Affinity.gameObject.SetActive(token.HasAffinity);
        if (token.HasAffinity) Affinity.color = token.Affinity.Color;

        // Pattern
        Pattern.gameObject.SetActive(surface.Pattern != null);
        if (surface.Pattern != null)
        {
            Sprite sprite = HelperFunctions.TextureToSprite(ResourceManager.LoadTexture($"Textures/TokenSurfacePattern/{surface.Pattern.DefName}"));
            Pattern.sprite = sprite;
        }

        Tooltip.Title = surface.GetFullLabel(secondLineFontSize: 14);
        Tooltip.Text = surface.Description;
    }
}
