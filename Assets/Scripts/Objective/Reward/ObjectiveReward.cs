using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectiveReward
{
    public ObjectiveRewardDef Def { get; private set; }

    public void Init(ObjectiveRewardDef def)
    {
        Def = def;
        OnInit();
    }
    protected virtual void OnInit() { }

    public abstract void ApplyReward();

    public virtual string Label => Def.Label;
}
