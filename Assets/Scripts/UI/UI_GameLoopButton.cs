using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_GameLoopButton : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI Text;
    public Image Image;
    public TooltipTarget Tooltip;

    private bool IsDisabled;

    private void Start()
    {
        Button.onClick.AddListener(OnClick);
        Tooltip.Type = global::Tooltip.TooltipType.TextOnly;
    }

    private void OnClick()
    {
        if (IsDisabled) return;
        if (Game.Instance.GameState == GameState.PreTurn) Game.Instance.DrawInitialTokens();
        else if (Game.Instance.GameState == GameState.PreparationPhase) Game.Instance.LockInSpread();
        else if (Game.Instance.GameState == GameState.ActionPhase) Game.Instance.EndActionPhase();
        Tooltip.HideTooltip();
    }

    public void RefreshTextAndTooltip()
    {
        switch(Game.Instance.GameState)
        {
            case GameState.PreTurn:
                Text.text = "Draw Tokens";
                Tooltip.Text = "Start the turn by drawing a new set of tokens from your pouch.";
                break;

            case GameState.PreparationPhase:
                Text.text = "Lock In Spread";
                Tooltip.Text = "Confirm your drawn tokens and activate their effects to begin the action phase.";
                break;

            case GameState.ActionPhase:
                Text.text = "End Action Phase";
                Tooltip.Text = "End your turn.";
                break;

            case GameState.PostTurn:
                Text.text = "Processing…";
                Tooltip.Text = "Resolving events, rules, quests, and effects. Please wait.";
                break;
        }
        
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
