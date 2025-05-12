using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGoalDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a quest goal of this type.
    /// </summary>
    public System.Type GoalClass { get; init; } = typeof(QuestGoal);
}
