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
    public UI_SelectionPanel SelectionPanel;

    [Header("Windows")]
    public UI_DraftWindow DraftWindow;
    public UI_TradeWindow TradeWindow;

    private void Awake()
    {
        Instance = this;
    }

    public void HideAllElements()
    {
        TurnSpreadPanel.gameObject.SetActive(false);
        TokenPouchButton.gameObject.SetActive(false);
        GameLoopButton.gameObject.SetActive(false);
        TurnPhaseResources.gameObject.SetActive(false);
        ChapterDisplay.gameObject.SetActive(false);
        QuestPanel.gameObject.SetActive(false);
        ResourcesPanel.gameObject.SetActive(false);
        HealthDisplay.gameObject.SetActive(false);
        ItemPanel.gameObject.SetActive(false);
        Rulebook.gameObject.SetActive(false);
        TileInteractionMenu.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
    }

    public void ShowAllElements()
    {
        TurnSpreadPanel.gameObject.SetActive(true);
        TokenPouchButton.gameObject.SetActive(true);
        GameLoopButton.gameObject.SetActive(true);
        TurnPhaseResources.gameObject.SetActive(true);
        ChapterDisplay.gameObject.SetActive(true);
        QuestPanel.Refresh();
        ResourcesPanel.Refresh();
        HealthDisplay.gameObject.SetActive(true);
        ItemPanel.gameObject.SetActive(true);
        Rulebook.gameObject.SetActive(true);
        TileInteractionMenu.gameObject.SetActive(true);
    }
}
