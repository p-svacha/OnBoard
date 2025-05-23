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

    public virtual string RewardDraftTitle => "Enjoy your reward";

    /// <summary>
    /// Returns all options that might be applied as a reward.
    /// </summary>
    public abstract List<IDraftable> GetRewardOptions();

    /// <summary>
    /// Gets executed when the reward draft is confirmed with this reward as a chosen option.
    /// </summary>
    public abstract void ApplyReward(IDraftable reward);

    /// <summary>
    /// Gets executed when the quest with this penalty gets removed from the active quests.
    /// </summary>
    public virtual void OnRemoved() { }

    public virtual bool IsDraft => Def.IsDraft;
    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;
}
