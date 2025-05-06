using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_QuestComplete : ActionPrompt
{
    private Quest Quest;
    public ActionPrompt_QuestComplete(Quest quest)
    {
        Quest = quest;
    }

    public override void OnShow()
    {
        Game.Instance.CompleteQuest(Quest);
    }
}
