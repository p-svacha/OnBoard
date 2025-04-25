using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemReceivedDisplay : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Subtitle;
    public TextMeshProUGUI Footer;
    public RawImage TokenDisplayImage;
    public Button OkButton;

    private GameObject PreviewObject;

    public void Initialize()
    {
        gameObject.SetActive(false);
        OkButton.onClick.AddListener(Close);
    }

    public void ShowTokenReceived(Token token)
    {
        gameObject.SetActive(true);

        PreviewObject = ObjectPreviewManager.ShowToken(TokenDisplayImage, token);

        Subtitle.text = "a new token";
        Footer.text = token.Label;
    }

    public void Close()
    {
        ObjectPreviewManager.ClearPreview(PreviewObject);
        gameObject.SetActive(false);
        Game.Instance.CompleteCurrentActionPrompt();
    }
}
