using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Rule : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI RuleText;
    public TextMeshProUGUI TurnText;

    public TooltipTarget LevelTooltip;
    public TooltipTarget RuleTooltip;
    public TooltipTarget TurnTooltip;

    public void InitActiveRule(Rule rule)
    {
        LevelText.text = rule.Level.ToString();
        RuleText.text = rule.LabelCap;
        TurnText.text = "";

        LevelTooltip.Title = $"Rule Severity";
        LevelTooltip.Text = $"This rule is currently at level {rule.Level}. Each level makes its effect stronger, more disruptive, or introduces new mechanics.";

        RuleTooltip.Title = rule.LabelCap;
        RuleTooltip.Text = rule.Description;

        TurnTooltip.Disabled = true;
    }

    public void InitUpcomingRule(Rule rule, bool isNew, int increase, int turns)
    {
        string lvlText = "";
        for (int i = 0; i < increase; i++) lvlText += "+";
        LevelText.text = lvlText;
        RuleText.text = rule.LabelCap;
        TurnText.text = $"in {turns}";

        LevelTooltip.Title = $"Rule Expansion";
        if (isNew) LevelTooltip.Text = $"This rule will be added to the rulebook as a new rule with an initial level of {increase}.";
        else LevelTooltip.Text = $"The level of this rule will be increased by {increase}.";

        RuleTooltip.Title = rule.LabelCap;
        RuleTooltip.Text = rule.Description;

        TurnTooltip.Title = $"Rule Expansion";
        TurnTooltip.Text = $"This rulebook expansion will happen in {turns} turns. Prepare and plan accordingly.";
    }
}
