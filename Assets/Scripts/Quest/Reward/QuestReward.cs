using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestReward
{
    public QuestRewardDef Def { get; private set; }

    public void Init(QuestRewardDef def)
    {
        Def = def;
        OnInit();
    }
    protected virtual void OnInit() { }

    public abstract void ApplyReward();

    public virtual string Label => Def.Label;
}
