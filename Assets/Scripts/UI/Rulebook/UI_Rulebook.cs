using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Rulebook : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Elements")]
    public Button Button;
    public Image ButtonIcon;
    public GameObject RuleListContainer;
    public GameObject ActiveRulesContainer;
    public GameObject UpcomingRulesContainer;

    [Header("Prefabs")]
    public UI_Rule RulePrefab;
    public Sprite OpenSprite;
    public Sprite ClosedSprite;

    public bool IsOpen { get; private set; }
    private bool IsMouseOverButton;

    private void Awake()
    {
        Button.onClick.AddListener(Toggle);
        IsOpen = false;
    }

    private void Toggle()
    {
        IsOpen = !IsOpen;
        Refresh();
        Tooltip.Instance.gameObject.SetActive(false);
        HelperFunctions.UnfocusNonInputUiElements();
    }

    public void Refresh()
    {
        if (IsOpen)
        {
            RuleListContainer.SetActive(true);
            ButtonIcon.sprite = OpenSprite;

            HelperFunctions.DestroyAllChildredImmediately(ActiveRulesContainer, skipElements: 1);
            foreach(Rule rule in Game.Instance.Rulebook.ActiveRules)
            {
                UI_Rule ruleDisplay = GameObject.Instantiate(RulePrefab, ActiveRulesContainer.transform);
                ruleDisplay.InitActiveRule(rule);
            }

            HelperFunctions.DestroyAllChildredImmediately(UpcomingRulesContainer, skipElements: 1);
            UI_Rule upcomingRuleDisplay = GameObject.Instantiate(RulePrefab, UpcomingRulesContainer.transform);
            upcomingRuleDisplay.InitUpcomingRule(Game.Instance.Rulebook.UpcomingExpansionRule, Game.Instance.Rulebook.IsUpcomingExpansionNewRule, increase: 1, Game.Instance.Rulebook.NextExpansionIn);
        }
        else
        {
            RuleListContainer.SetActive(false);
            if (IsMouseOverButton) ButtonIcon.sprite = OpenSprite;
            else ButtonIcon.sprite = ClosedSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOverButton = true;
        Refresh();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOverButton = false;
        Refresh();
    }
}
