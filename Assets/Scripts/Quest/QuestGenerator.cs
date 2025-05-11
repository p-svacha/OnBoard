using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestGenerator
{
    public static float DeadlineChance = 0.5f;
    public static int MinDeadlineTime = 3;
    public static int MaxDeadlineTime = 15;

    public static float PenaltyChance = 0.5f; // Gets checked for only if there is a deadline

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

        // Choose a deadline
        int deadlineTurn = -1;
        bool hasDeadline = Random.value < DeadlineChance;
        if (hasDeadline)
        {
            int time = Random.Range(MinDeadlineTime, MaxDeadlineTime + 1);
            deadlineTurn = Game.Instance.Turn + time;
        }

        // Choose a failure penalty
        QuestPenalty penalty = null;
        if (hasDeadline)
        {
            bool hasPenalty = Random.value < PenaltyChance;
            if(hasPenalty)
            {
                QuestPenaltyDef chosenPenaltyDef = DefDatabase<QuestPenaltyDef>.AllDefs.RandomElement();
                penalty = (QuestPenalty)System.Activator.CreateInstance(chosenPenaltyDef.RewardClass);
                penalty.Init(chosenPenaltyDef);
            }
        }

        return new Quest(goal, reward, deadlineTurn, penalty);
    }
}
