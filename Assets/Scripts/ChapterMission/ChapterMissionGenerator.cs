using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChapterMissionGenerator
{
    public static ChapterGoal GenerateChapterObectiveGoal(int chapter)
    {
        // Choose a goal
        ChapterGoalDef chosenGoalDef = DefDatabase<ChapterGoalDef>.AllDefs.RandomElement();
        ChapterGoal goal = (ChapterGoal)System.Activator.CreateInstance(chosenGoalDef.GoalClass);
        goal.Init(chosenGoalDef, chapter);

        return goal;
    }
}
