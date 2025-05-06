using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChapterGoal
{
    public ChapterGoalDef Def { get; private set; }

    public void Init(ChapterGoalDef def)
    {
        Def = def;
        OnInit();
    }

    /// <summary>
    /// Gets executed when this chapter goal gets created.
    /// </summary>
    protected virtual void OnInit() { }

    /// <summary>
    /// Gets executed when the chapter with this goal is started.
    /// </summary>
    public virtual void OnStarted() { }

    /// <summary>
    /// Gets executed when the chapter with this goal is completed.
    /// </summary>
    public virtual void OnCompleted() { }

    public virtual string Description => Def.Description;
}
