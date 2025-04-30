using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChapterMissionGenerator
{
    public static QuestGoal GenerateChapterObectiveGoal(int chapter)
    {
        // Choose a goal
        QuestGoalDef chosenGoalDef = DefDatabase<QuestGoalDef>.AllDefs.RandomElement();
        QuestGoal goal = (QuestGoal)System.Activator.CreateInstance(chosenGoalDef.GoalClass);
        goal.Init(chosenGoalDef);

        return goal;
    }
}
