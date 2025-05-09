using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestPenalty : IDraftable
{
    public QuestPenaltyDef Def { get; private set; }

    public void Init(QuestPenaltyDef def)
    {
        Def = def;
        OnInit();
    }
    protected virtual void OnInit() { }

    /// <summary>
    /// Gets executed when the quest with this penalty gets removed from the active quests.
    /// </summary>
    public virtual void OnRemoved() { }

    public abstract void ApplyPenalty();

    public virtual string Label => Def.Label;

    // IDraftable
    public abstract string DraftDisplay_Text { get; }
    public abstract Sprite DraftDisplay_Sprite { get; }
    public abstract GameObject DraftDisplay_Spinning3DObject { get; }
}
