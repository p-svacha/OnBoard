using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestGoalDefs
{
    public static List<QuestGoalDef> Defs => new List<QuestGoalDef>()
    {
        new QuestGoalDef()
        {
            DefName = "ReachRedFlag",
            Description = "Reach the red flag.",
            GoalClass = typeof(QuestGoal_ReachRedFlag),
        }
    };
}
