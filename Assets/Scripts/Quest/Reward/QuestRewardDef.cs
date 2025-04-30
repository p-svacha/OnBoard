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
}
