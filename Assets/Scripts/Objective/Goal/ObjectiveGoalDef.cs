using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGoalDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating an objective goal of this type.
    /// </summary>
    public Type GoalClass { get; init; } = typeof(ObjectiveGoal);
}
