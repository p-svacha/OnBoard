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
        },

        new QuestRewardDef()
        {
            DefName = "ReceiveRippledPattern",
            Label = "rippled pattern",
            Description = "Draft a surface from tokens in your pouch to apply the rippled pattern to.",
            RewardClass = typeof(QuestReward_RipplePattern),
            IsDraft = true,
        },
    };
}
