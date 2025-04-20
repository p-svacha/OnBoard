using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MajorGoalDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a major goal of this type.
    /// </summary>
    public Type MajorGoalClass { get; init; } = typeof(MajorGoal);
}
