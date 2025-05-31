using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rulebook
{
    public static int MAX_RULES = 10;
    public static int DEFAULT_EXPANSION_INTERVAL = 6;

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
    public int NextExpansionIn => UpcomingExpansionTurn - (Game.Instance.GameState == GameState.PostTurn ? (Game.Instance.Turn + 1) : Game.Instance.Turn);

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
        DoSetupUpcomingExpansion();
        GameUI.Instance.Rulebook.Refresh();
    }

    public Rule GetRule(RuleDef ruleDef)
    {
        return ActiveRules.First(r => r.Def == ruleDef);
    }

    public void OnLockInSpread(Spread spread)
    {
        foreach (Rule r in ActiveRules) r.OnLockInSpread(spread);
    }

    /// <summary>
    /// Gets executed at the end of every turn.
    /// </summary>
    public void OnTurnPassed()
    {
        // Rulebook expansion
        if ((Game.Instance.Turn + 1) == UpcomingExpansionTurn)
        {
            ExecuteRulebookExpansion(UpcomingExpansionRule);
        }

        GameUI.Instance.Rulebook.Refresh();
    }

    /// <summary>
    /// Queues a rule expansion action prompt.
    /// </summary>
    public void ExecuteRulebookExpansion(Rule rule)
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_RulebookExpansion(rule));
    }

    /// <summary>
    /// Instantly performs a rulebook expansion and shows it in a window.
    /// </summary>
    public void DoExecuteRulebookExpansion(Rule rule)
    {
        // New rule
        if (!ActiveRules.Contains(rule))
        {
            ActiveRules.Add(rule);

            // Show draft window
            string title = $"A rule has been added";
            string subtitle = rule.LabelCap;
            List<IDraftable> options = rule.Levels.Select(l => (IDraftable)l).ToList();
            GameUI.Instance.DraftWindow.Show(title, subtitle, options, isDraft: false);

            // Hook
            rule.OnActivate(1);
        }

        // Level increase 
        else
        {
            UpcomingExpansionRule.IncreaseLevel();

            // Show draft window
            string title = $"A rule level has increased";
            string subtitle = rule.LabelCap;
            List<IDraftable> options = rule.Levels.Select(l => (IDraftable)l).ToList();
            GameUI.Instance.DraftWindow.Show(title, subtitle, options, isDraft: false);

            // Hook
            rule.OnActivate(rule.Level);
        }

        // Setup next rulebook expansion
        DoSetupUpcomingExpansion();

        // UI
        GameUI.Instance.Rulebook.Refresh();
    }

    /// <summary>
    /// Queues an action prompt that adds a future/upcoming rulebook expansion.
    /// </summary>
    public void SetupUpcomingExpansion()
    {

    }

    /// <summary>
    /// Instantly adds an upcoming rule expansion
    /// </summary>
    private void DoSetupUpcomingExpansion()
    {
        // Set the turn when the expansion happens
        UpcomingExpansionTurn = Game.Instance.Turn + ExpansionInterval;
        if (Game.Instance.GameState == GameState.PostTurn) UpcomingExpansionTurn++;

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


            if (attempts >= 20) addNewRule = false;
        }
        

        if (!addNewRule)
        {
            Debug.Log("Rulebook expansion will be level increase");
            UpcomingExpansionRule = ActiveRules.RandomElement();
        }

    }
}
