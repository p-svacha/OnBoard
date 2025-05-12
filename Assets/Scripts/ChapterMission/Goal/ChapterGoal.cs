using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChapterGoal
{
    public ChapterGoalDef Def { get; private set; }

    public void Init(ChapterGoalDef def, int chapter)
    {
        Def = def;
        OnInit(chapter);
    }

    /// <summary>
    /// Gets executed when this chapter goal gets created for chapter X.
    /// </summary>
    protected virtual void OnInit(int chapter) { }

    /// <summary>
    /// Gets executed when the chapter with this goal is started.
    /// </summary>
    public virtual void OnStarted() { }

    /// <summary>
    /// Gets executed when the chapter with this goal is completed.
    /// </summary>
    public virtual void OnCompleted() { }

    /// <summary>
    /// Gets executed once per turn when the spread is locked in between the preparation and action phase.
    /// </summary>
    public virtual void OnLockInSpread(Spread spread) { }

    public virtual string Description => Def.Description;
}
