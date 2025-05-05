using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rulebook
{
    public static int MAX_RULES = 10;
    public static int DEFAULT_EXPANSION_INTERVAL = 3;

    /// <summary>
    /// The list of all rules that are currently active in the rulebook.
    /// </summary>
    public List<Rule> ActiveRules;

    /// <summary>
    /// The turn when the next rulebook expansion happens.
    /// </summary>
    public int UpcomingExpansionTurn { get; private set; }

    /// <summary>
    /// The amount of turns until the next expansion.
    /// </summary>
    public int NextExpansionIn => UpcomingExpansionTurn - Game.Instance.Turn;

    /// <summary>
    /// Flag if the upcoming rulebook expansion is the addition of a completely new rule.
    /// <br/>If false, this means the upcoming expansion is the level increase of an existing rule.
    /// </summary>
    public bool IsUpcomingExpansionNewRule => !ActiveRules.Contains(UpcomingExpansionRule);

    /// <summary>
    /// The rule that will be affected by the next rulebook expansion.
    /// <br/>If this rule is already in the rulebook, the upcoming rulebook expansion will be a level increase of this rule.
    /// <br/>If this rule is not yet in the rulebook, it will be added in the upcoming rulebook expansion.
    /// </summary>
    public Rule UpcomingExpansionRule { get; private set; }

    /// <summary>
    /// The turn interval at which rulebook expansions happen.
    /// </summary>
    public int ExpansionInterval { get; private set; }

    public Rulebook()
    {
        ActiveRules = new List<Rule>();
        ExpansionInterval = DEFAULT_EXPANSION_INTERVAL;
        SetupUpcomingExpansion();
        GameUI.Instance.Rulebook.Refresh();
    }

    /// <summary>
    /// Gets executed at the end of every turn.
    /// </summary>
    public void OnTurnPassed()
    {
        // Rulebook expansion
        if(Game.Instance.Turn == UpcomingExpansionTurn)
        {
            ExecuteRulebookExpansion();
            SetupUpcomingExpansion();
        }

        GameUI.Instance.Rulebook.Refresh();
    }

    private void ExecuteRulebookExpansion()
    {
        if (IsUpcomingExpansionNewRule) ActiveRules.Add(UpcomingExpansionRule);
        else UpcomingExpansionRule.IncreaseLevel();
        GameUI.Instance.Rulebook.Refresh();
    }

    private void SetupUpcomingExpansion()
    {
        // Set the turn when the expansion happens
        UpcomingExpansionTurn = Game.Instance.Turn + ExpansionInterval;

        // Decide if to add new rule or increase existing rule level
        float newRuleChance = 0f;
        if (ActiveRules.Count == 0) newRuleChance = 1f; // Always add new rule if none added yet
        else if (ActiveRules.Count >= MAX_RULES) newRuleChance = 0f; // Always increase existing rule level if limit reached
        else // Chance based on current amount of rules
        {
            float chanceStep = 1f / MAX_RULES;
            newRuleChance = (MAX_RULES - ActiveRules.Count) * chanceStep;
        }
        bool addNewRule = Random.value <= newRuleChance;

        // Choose upcoming expansion def
        if(addNewRule)
        {
            Rule newRule = null;
            int attempts = 0;
            while (attempts++ <= 20 && (newRule == null || ActiveRules.Any(r => !r.CanCoexistWith(newRule))))
            {
                List<RuleDef> candidates = DefDatabase<RuleDef>.AllDefs;
                RuleDef chosenDef = candidates.RandomElement();
                newRule = (Rule)System.Activator.CreateInstance(chosenDef.RuleClass);
                newRule.Init(chosenDef);
            }
            UpcomingExpansionRule = newRule;


            if (attempts >= 20) UpcomingExpansionRule = ActiveRules.RandomElement();
        }
        else
        {
            UpcomingExpansionRule = ActiveRules.RandomElement();
        }

    }
}
