using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeature_RedFlag : TileFeature
{
    public QuestGoal_ReachRedFlag Goal;

    public void Init(QuestGoal_ReachRedFlag goal)
    {
        Goal = goal;
    }

    protected override void OnInitVisuals()
    {
        GameObject flagPrefab = ResourceManager.LoadPrefab("Prefabs/TileFeatures/RedFlag");
        GameObject.Instantiate(flagPrefab, transform);
    }

    public override void OnLand()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_QuestComplete(Goal.Quest));
    }
}
