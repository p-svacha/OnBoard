using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPenaltyDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a quest penalty of this def.
    /// </summary>
    public System.Type RewardClass { get; init; } = typeof(QuestPenalty);
}
