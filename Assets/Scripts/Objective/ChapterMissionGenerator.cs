using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChapterMissionGenerator
{
    public static ObjectiveGoal GenerateChapterObectiveGoal(int chapter)
    {
        // Choose a goal
        ObjectiveGoalDef chosenGoalDef = DefDatabase<ObjectiveGoalDef>.AllDefs.RandomElement();
        ObjectiveGoal goal = (ObjectiveGoal)System.Activator.CreateInstance(chosenGoalDef.GoalClass);
        goal.Init(chosenGoalDef);

        return goal;
    }
}
