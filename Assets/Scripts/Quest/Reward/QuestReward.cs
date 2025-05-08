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

    protected abstract void ApplyReward();
    

    public virtual string Label => Def.Label;

    // IDraftable
    public abstract string DraftDisplay_Text { get; }
    public abstract Sprite DraftDisplay_Sprite { get; }
    public abstract GameObject DraftDisplay_Spinning3DObject { get; }
    public void ApplySelection()
    {
        ApplyReward();
    }
}
