using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChapterReward : IDraftable
{
    public ChapterRewardDef Def { get; private set; }

    public void Init(ChapterRewardDef def)
    {
        Def = def;
        OnInit();
    }
    protected virtual void OnInit() { }

    public abstract void ApplyReward();
    

    // IDraftable
    public virtual string DraftDisplay_Text { get; } = null;
    public virtual Sprite DraftDisplay_Sprite { get; } = null;
    public virtual GameObject DraftDisplay_Spinning3DObject { get; } = null;
}
