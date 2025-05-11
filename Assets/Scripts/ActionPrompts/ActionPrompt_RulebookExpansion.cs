using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_RulebookExpansion : ActionPrompt
{
    private Rule Rule;

    public ActionPrompt_RulebookExpansion(Rule rule)
    {
        Rule = rule;
    }

    public override void OnShow()
    {
        Game.Instance.Rulebook.DoExecuteRulebookExpansion(Rule);
    }
}
