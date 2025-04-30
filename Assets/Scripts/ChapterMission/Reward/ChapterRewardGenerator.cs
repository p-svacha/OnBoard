using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChapterRewardGenerator
{
    public static List<ChapterReward> GenerateRewards(int chapter, int amount)
    {
        List<ChapterReward> rewards = new List<ChapterReward>();
        for(int i = 0; i < amount; i++)
        {
            rewards.Add(GenerateRandomReward());
        }
        return rewards;
    }

    private static ChapterReward GenerateRandomReward()
    {
        List<ChapterRewardDef> candidateDefs = new List<ChapterRewardDef>(DefDatabase<ChapterRewardDef>.AllDefs);
        ChapterRewardDef chosenDef = candidateDefs.RandomElement();

        ChapterReward reward = (ChapterReward)System.Activator.CreateInstance(chosenDef.RewardClass);
        reward.Init(chosenDef);

        return reward;
    }
}
