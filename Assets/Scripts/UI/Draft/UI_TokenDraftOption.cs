using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TokenDraftOption : MonoBehaviour
{
    public RawImage RawImage;
    public GameObject SelectionIndicator;
    public Button Button;

    public void Init(UI_TokenDraftDisplay parent, Token token)
    {
        ObjectPreviewManager.ShowToken(RawImage, token);
        SelectionIndicator.SetActive(false);
        Button.onClick.AddListener(() => parent.SetSelectedToken(token));
        GetComponent<TooltipTarget>().Title = token.LabelCap;
        GetComponent<TooltipTarget>().Text = token.Description;
    }

    public void SetSelected(bool value)
    {
        SelectionIndicator.SetActive(value);
    }
}
