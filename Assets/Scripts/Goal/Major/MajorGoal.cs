using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MajorGoal
{
    public MajorGoalDef Def { get; private set; }
    public bool IsComplete { get; private set; }

    public MajorGoal(MajorGoalDef def)
    {
        Def = def;
        IsComplete = false;
    }

    public void SetAsComplete()
    {
        IsComplete = true;
    }

    /// <summary>
    /// Gets executed when ending the chapter with this goal.
    /// </summary>
    public virtual void OnRemoved() { }
    public virtual string Description => Def.Description;
}
