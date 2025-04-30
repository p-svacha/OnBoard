using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A goal represents a specific thing that has to be done to complete an objective.
/// </summary>
public abstract class QuestGoal
{
    public QuestGoalDef Def { get; private set; }


    public void Init(QuestGoalDef def)
    {
        Def = def;
        OnInit();
    }
    protected virtual void OnInit() { }

    /// <summary>
    /// Gets executed when the objective with this goal gets removed.
    /// </summary>
    public virtual void OnRemoved() { }
    public virtual string Description => Def.Description;
}
