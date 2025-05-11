using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DraftOption : MonoBehaviour
{
    [Header("Elements")]
    public Image SelectionFrame;
    public Button Button;
    public TextMeshProUGUI Title;
    public Image Image;
    public GameObject RawImageContainer;
    public RawImage RawImage;
    public TextMeshProUGUI Text;

    public void Init(UI_Draft draft, IDraftable option)
    {
        Button.onClick.AddListener(() => draft.SetSelectedOption(option));

        // Title
        if (option.DraftDisplay_Title != null && option.DraftDisplay_Title != "")
        {
            Title.gameObject.SetActive(true);
            Title.text = option.DraftDisplay_Title;
        }
        else Title.gameObject.SetActive(false);

        // Sprite
        if (option.DraftDisplay_Sprite != null)
        {
            Image.gameObject.SetActive(true);
            Image.sprite = option.DraftDisplay_Sprite;
        }
        else Image.gameObject.SetActive(false);

        // 3D Object
        if (option.DraftDisplay_Spinning3DObject != null)
        {
            RawImageContainer.gameObject.SetActive(true);
            ObjectPreviewManager.ShowGameObject(RawImage, option.DraftDisplay_Spinning3DObject);
        }
        else RawImageContainer.gameObject.SetActive(false);

        // Text
        if (option.DraftDisplay_Text != null && option.DraftDisplay_Text != "")
        {
            Text.gameObject.SetActive(true);
            Text.text = option.DraftDisplay_Text;
        }
        else Text.gameObject.SetActive(false);
    }

    public void SetSelected(bool value)
    {
        SelectionFrame.sprite = value ? ResourceManager.LoadSprite("Sprites/UIFrame/UIFrameButton") : ResourceManager.LoadSprite("Sprites/UIFrame/UIFrame");
    }
}
