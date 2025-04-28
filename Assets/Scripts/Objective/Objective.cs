using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective
{
    public ObjectiveGoal Goal;
    public ObjectiveReward Reward;
    public int DeadlineTurn;
    public bool IsUiCollapsed;

    public Objective(ObjectiveGoal goal, ObjectiveReward reward, int deadline = -1)
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
