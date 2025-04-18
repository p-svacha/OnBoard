using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TokenPouchButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button Button;
    private Image Image;

    private void Start()
    {
        Button = GetComponent<Button>();
        Image = GetComponent<Image>();
        Button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (Game.Instance.IsTokenPouchOpen) Game.Instance.CloseTokenPouch();
        else Game.Instance.OpenTokenPouch();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Image.sprite = ResourceManager.LoadSprite("Sprites/TokenPouch/TokenPouchOpen");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Game.Instance.IsTokenPouchOpen) Image.sprite = ResourceManager.LoadSprite("Sprites/TokenPouch/TokenPouchClosed");
    }
}
