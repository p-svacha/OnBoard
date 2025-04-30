using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Elements")]
    public UI_TurnDraw TurnDraw;
    public UI_TokenPouchButton TokenPouchButton;
    public UI_PostItButton GameLoopButton;
    public UI_TurnPhaseResources TurnPhaseResources;
    public UI_ItemReceivedDisplay ItemReceivedDisplay;
    public UI_TokenDraftDisplay TokenDraftDisplay;
    public UI_ChapterDisplay ChapterDisplay;
    public UI_QuestPanel QuestPanel;
    public UI_HealthDisplay HealthDisplay;
    public UI_ItemPanel ItemPanel;

    private void Awake()
    {
        Instance = this;

        ItemReceivedDisplay.Initialize();
        TokenDraftDisplay.Initialize();
    }
}
