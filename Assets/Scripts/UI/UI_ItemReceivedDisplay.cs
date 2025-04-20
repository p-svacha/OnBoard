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
    public Button OkButton;

    private GameObject PreviewObject;

    private void Start()
    {
        gameObject.SetActive(false);
        OkButton.onClick.AddListener(Close);
    }

    public void ShowTokenReceived(Token token)
    {
        gameObject.SetActive(true);

        PreviewObject = TokenGenerator.GenerateTokenCopy(token, hidden: false, frozen: true).gameObject;
        InitPreviewObject();

        Subtitle.text = "a new token";
        Footer.text = token.Label;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void InitPreviewObject()
    {
        PreviewObject.layer = WorldManager.Layer_PreviewObject;
        PreviewObject.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        PreviewObject.AddComponent<SpinPreview>();
        PreviewObject.transform.position = new Vector3(0f, 200f, 0f);
    }
}
