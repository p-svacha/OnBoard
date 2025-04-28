using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectiveGoalDefs
{
    public static List<ObjectiveGoalDef> Defs => new List<ObjectiveGoalDef>()
    {
        new ObjectiveGoalDef()
        {
            DefName = "ReachRedFlag",
            Description = "Reach the red flag.",
            GoalClass = typeof(ObjectiveGoal_ReachRedFlag),
        }
    };
}
