using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterGoalDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a chapter goal of this type.
    /// </summary>
    public System.Type GoalClass { get; init; } = typeof(ChapterGoal);
}
