using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveRewardDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating an objective reward of this type.
    /// </summary>
    public Type RewardClass { get; init; } = typeof(ObjectiveReward);
}
