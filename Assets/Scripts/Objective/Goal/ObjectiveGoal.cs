using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A goal represents a specific thing that has to be done to complete an objective.
/// </summary>
public abstract class ObjectiveGoal
{
    public ObjectiveGoalDef Def { get; private set; }
    public bool IsComplete { get; private set; }


    public void Init(ObjectiveGoalDef def)
    {
        Def = def;
        IsComplete = false;
        OnInit();
    }
    protected virtual void OnInit() { }

    public void SetAsComplete()
    {
        IsComplete = true;
    }

    /// <summary>
    /// Gets executed when the objective with this goal gets removed.
    /// </summary>
    public virtual void OnRemoved() { }
    public virtual string Description => Def.Description;
}
