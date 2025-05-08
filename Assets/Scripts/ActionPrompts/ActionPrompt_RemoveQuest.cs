using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_RemoveQuest : ActionPrompt
{
    private Quest Quest;

    public ActionPrompt_RemoveQuest(Quest quest)
    {
        Quest = quest;
    }

    public override void OnShow()
    {
        Game.Instance.DoRemoveQuest(Quest);
    }
}
