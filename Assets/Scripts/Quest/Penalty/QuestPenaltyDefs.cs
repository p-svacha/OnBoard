using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestPenaltyDefs
{
    public static List<QuestPenaltyDef> Defs => new List<QuestPenaltyDef>()
    {
        new QuestPenaltyDef()
        {
            DefName = "BlackPebble",
            Label = "1 small black pebble",
            Description = "A useless pebble is added to your pouch.",
            RewardClass = typeof(QuestPenalty_BlackPebble),
        }
    };
}
