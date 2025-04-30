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
        if (Game.Instance.GameState == GameState.PreDraw) Game.Instance.DrawInitialTokens();
        else if (Game.Instance.GameState == GameState.DrawingPhase) Game.Instance.ConfirmDraw();
        else if (Game.Instance.GameState == GameState.MovingPhase) Game.Instance.EndTurn();
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    public void Disable()
    {
        Image.sprite = ResourceManager.LoadSprite("Sprites/UIFrame/UIFrame_Disabled");
        Text.color = new Color(0.3f, 0.3f, 0.3f);
        IsDisabled = true;
    }
    public void Enable()
    {
        Image.sprite = ResourceManager.LoadSprite("Sprites/UIFrame/UIFrame");
        Text.color = new Color(0.2f, 0.1f, 0.0f);
        IsDisabled = false;
    }
}
