using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestGoal Goal;
    public QuestReward Reward;
    public int DeadlineTurn;
    public bool IsUiCollapsed;

    public Quest(QuestGoal goal, QuestReward reward, int deadline = -1)
    {
        Goal = goal;
        Reward = reward;
        DeadlineTurn = deadline;
    }

    public string GetShortDescription()
    {
        return "";
    }
}
