using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestGenerator
{
    public static Quest GenerateQuest()
    {
        // Choose a goal
        QuestGoalDef chosenGoalDef = DefDatabase<QuestGoalDef>.AllDefs.RandomElement();
        QuestGoal goal = (QuestGoal)System.Activator.CreateInstance(chosenGoalDef.GoalClass);
        goal.Init(chosenGoalDef);

        // Choose a reward
        QuestRewardDef chosenRewardDef = DefDatabase<QuestRewardDef>.AllDefs.RandomElement();
        QuestReward reward = (QuestReward)System.Activator.CreateInstance(chosenRewardDef.RewardClass);
        reward.Init(chosenRewardDef);

        return new Quest(goal, reward);
    }
}
