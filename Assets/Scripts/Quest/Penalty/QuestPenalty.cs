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
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;

    // IDraftable
    public virtual string DraftDisplay_Title => Label;
    public virtual string DraftDisplay_Text => null;
    public virtual Sprite DraftDisplay_Sprite => null;
    public virtual GameObject DraftDisplay_Spinning3DObject => null;
}
