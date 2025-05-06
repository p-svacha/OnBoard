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

    protected abstract void ApplyPenalty();

    public virtual string Label => Def.Label;

    // IDraftable
    public string DraftDisplay_Text => throw new System.NotImplementedException();
    public Sprite DraftDisplay_Sprite => throw new System.NotImplementedException();
    public GameObject DraftDisplay_Spinning3DObject => throw new System.NotImplementedException();
    public void ApplySelection()
    {
        ApplyPenalty();
    }
}
