using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterRewardDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a chapter reward of this type.
    /// </summary>
    public System.Type RewardClass { get; init; } = typeof(ChapterReward);
}
