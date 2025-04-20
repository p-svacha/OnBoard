using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MajorGoalDefs
{
    public static List<MajorGoalDef> Defs => new List<MajorGoalDef>()
    {
        new MajorGoalDef()
        {
            DefName = "ReachRedFlag",
            Description = "Reach the red flag.",
            MajorGoalClass = typeof(MajorGoal_ReachRedFlag),
        }
    };
}
