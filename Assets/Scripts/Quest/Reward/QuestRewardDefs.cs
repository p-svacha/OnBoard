using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestRewardDefs
{
    public static List<QuestRewardDef> Defs => new List<QuestRewardDef>()
    {
        new QuestRewardDef()
        {
            DefName = "SpecificToken",
            Label = "specific token",
            Description = "Receive a specific token",
            RewardClass = typeof(QuestReward_ReceiveToken),
        }
    };
}
