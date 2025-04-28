using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestGenerator
{
    public static Objective GenerateQuest()
    {
        // Choose a goal
        ObjectiveGoalDef chosenGoalDef = DefDatabase<ObjectiveGoalDef>.AllDefs.RandomElement();
        ObjectiveGoal goal = (ObjectiveGoal)System.Activator.CreateInstance(chosenGoalDef.GoalClass);
        goal.Init(chosenGoalDef);

        // Choose a reward
        ObjectiveRewardDef chosenRewardDef = DefDatabase<ObjectiveRewardDef>.AllDefs.RandomElement();
        ObjectiveReward reward = (ObjectiveReward)System.Activator.CreateInstance(chosenRewardDef.RewardClass);
        reward.Init(chosenRewardDef);

        return new Objective(goal, reward);
    }
}
