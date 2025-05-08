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

    /// <summary>
    /// Gets executed whenever a turn ends.
    /// </summary>
    public void OnTurnPassed()
    {
        if (Game.Instance.Turn >= DeadlineTurn - 1)
        {
            Game.Instance.FailQuest(this);
        }
    }

    /// <summary>
    /// Gets executed when this quest gets removed from the active quests.
    /// </summary>
    public void OnRemoved()
    {
        Goal.OnRemoved();
        Reward.OnRemoved();
        if(Penalty != null) Penalty.OnRemoved();
    }

    public string GetShortDescription()
    {
        return "";
    }

    public bool HasProgress => false;
    public bool HasDeadline => DeadlineTurn != -1;
    public bool HasPenalty => Penalty != null;
}
