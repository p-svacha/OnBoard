using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Permanent Elements")]
    public UI_TurnSpread TurnSpreadPanel;
    public UI_TokenPouchButton TokenPouchButton;
    public UI_GameLoopButton GameLoopButton;
    public UI_TurnPhaseResources TurnPhaseResources;
    public UI_ChapterDisplay ChapterDisplay;
    public UI_QuestPanel QuestPanel;
    public UI_ResourcesPanel ResourcesPanel;
    public UI_HealthDisplay HealthDisplay;
    public UI_ItemPanel ItemPanel;
    public UI_Rulebook Rulebook;
    public UI_TileInteractionMenu TileInteractionMenu;

    [Header("Windows")]
    public UI_Window_TokenReceived ItemReceivedDisplay;
    public UI_Window_TokenDraft TokenDraftDisplay;
    public UI_DraftWindow DraftWindow;

    private void Awake()
    {
        Instance = this;
    }
}
