using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestGoal Goal;
    public QuestReward Reward;
    public QuestPenalty Penalty;
    public int DeadlineTurn;
    public bool IsUiCollapsed;

    public Quest(QuestGoal goal, QuestReward reward, int deadlineTurn, QuestPenalty penalty)
    {
        Goal = goal;
        Goal.Quest = this;
        Reward = reward;
        DeadlineTurn = deadlineTurn;
        Penalty = penalty;
    }

    public string GetShortDescription()
    {
        return "";
    }

    public bool HasProgress => false;
    public bool HasDeadline => DeadlineTurn != -1;
    public bool HasPenalty => Penalty != null;
}
