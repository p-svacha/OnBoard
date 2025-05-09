using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestReward : IDraftable
{
    public QuestRewardDef Def { get; private set; }

    public void Init(QuestRewardDef def)
    {
        Def = def;
        OnInit();
    }
    protected virtual void OnInit() { }

    /// <summary>
    /// Gets executed when the quest with this penalty gets removed from the active quests.
    /// </summary>
    public virtual void OnRemoved() { }

    public abstract void ApplyReward();
    

    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;

    // IDraftable
    public virtual string DraftDisplay_Text => Label;
    public virtual Sprite DraftDisplay_Sprite => null;
    public virtual GameObject DraftDisplay_Spinning3DObject => null;
}
