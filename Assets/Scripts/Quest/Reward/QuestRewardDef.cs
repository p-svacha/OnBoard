using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestRewardDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a quest reward of this type.
    /// </summary>
    public Type RewardClass { get; init; } = typeof(QuestReward);

    /// <summary>
    /// Flag if as part of the reward, the player has to choose an option from GetRewardOptions(). If false, all are applied.
    /// </summary>
    public bool IsDraft { get; init; } = false;
}
