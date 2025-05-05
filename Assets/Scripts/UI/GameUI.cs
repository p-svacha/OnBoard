using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Permanent Elements")]
    public UI_TurnDraw TurnDraw;
    public UI_TokenPouchButton TokenPouchButton;
    public UI_PostItButton GameLoopButton;
    public UI_TurnPhaseResources TurnPhaseResources;
    public UI_ChapterDisplay ChapterDisplay;
    public UI_QuestPanel QuestPanel;
    public UI_HealthDisplay HealthDisplay;
    public UI_ItemPanel ItemPanel;
    public UI_Rulebook Rulebook;

    [Header("Windows")]
    public UI_Window_TokenReceived ItemReceivedDisplay;
    public UI_Window_TokenDraft TokenDraftDisplay;
    public UI_Window_ChapterComplete ChapterCompleteWindow;

    private void Awake()
    {
        Instance = this;
    }
}
