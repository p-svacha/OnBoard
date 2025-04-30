using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChapterRewardDefs
{
    public static List<ChapterRewardDef> Defs => new List<ChapterRewardDef>()
    {
        new ChapterRewardDef()
        {
            DefName = "Item",
            Label = "item",
            Description = "Receive a specific item.",
            RewardClass = typeof(ChapterReward_Item),
        }
    };
}
