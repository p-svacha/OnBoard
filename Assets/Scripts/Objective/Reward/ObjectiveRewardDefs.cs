using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectiveRewardDefs
{
    public static List<ObjectiveRewardDef> Defs => new List<ObjectiveRewardDef>()
    {
        new ObjectiveRewardDef()
        {
            DefName = "SpecificToken",
            Label = "specific token",
            Description = "Receive a specific token",
            RewardClass = typeof(ObjectiveReward_ReceiveToken),
        }
    };
}
