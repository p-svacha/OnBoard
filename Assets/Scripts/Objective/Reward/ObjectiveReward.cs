using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectiveReward
{
    public ObjectiveRewardDef Def { get; private set; }

    public ObjectiveReward(ObjectiveRewardDef def)
    {
        Def = def;
    }

    public abstract void ApplyReward();
}
