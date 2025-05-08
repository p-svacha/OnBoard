using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_QuestFailed : ActionPrompt
{
    private Quest Quest;
    public ActionPrompt_QuestFailed(Quest quest)
    {
        Quest = quest;
    }

    public override void OnShow()
    {
        Game.Instance.FailQuest(Quest);
    }
}
