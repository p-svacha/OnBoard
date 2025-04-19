using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PostItButton : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI Text;
    public Image Image;

    private bool IsDisabled;

    private void Start()
    {
        Button.onClick.AddListener(EndTurnButton_OnClick);
    }

    private void EndTurnButton_OnClick()
    {
        if (IsDisabled) return;
        if (Game.Instance.GameState == GameState.WaitingForDraw) Game.Instance.StartTurn();
        else if (Game.Instance.GameState == GameState.DrawInteraction) Game.Instance.ConfirmDraw();
        else if (Game.Instance.GameState == GameState.Movement) Game.Instance.EndTurn();
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    public void Disable()
    {
        Image.sprite = ResourceManager.LoadSprite("Sprites/PostItWhite");
        Text.color = new Color(0.3f, 0.3f, 0.3f);
        IsDisabled = true;
    }
    public void Enable()
    {
        Image.sprite = ResourceManager.LoadSprite("Sprites/PostIt1");
        Text.color = new Color(0.2f, 0.1f, 0.0f);
        IsDisabled = false;
    }
}
