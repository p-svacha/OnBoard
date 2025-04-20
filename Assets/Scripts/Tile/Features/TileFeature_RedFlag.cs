using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeature_RedFlag : TileFeature
{
    public MajorGoal_ReachRedFlag Goal;

    public void Init(MajorGoal_ReachRedFlag goal)
    {
        Goal = goal;
    }

    public override void InitVisuals()
    {
        GameObject flagPrefab = ResourceManager.LoadPrefab("Prefabs/TileFeatures/RedFlag");
        GameObject.Instantiate(flagPrefab, transform);
    }

    public override void OnLand()
    {
        if (Game.Instance.CurrentMajorGoal == Goal)
        {
            Game.Instance.SetMajorGoalAsComplete();
        }
    }
}
