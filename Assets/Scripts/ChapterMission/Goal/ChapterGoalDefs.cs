using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChapterGoalDefs
{
    public static List<ChapterGoalDef> Defs => new List<ChapterGoalDef>()
    {
        new ChapterGoalDef()
        {
            DefName = "Offering",
            Description = "Deliver a specific kind of token to a designated altar tile.",
            GoalClass = typeof(ChapterGoal_Offering)
        }
    };
}
