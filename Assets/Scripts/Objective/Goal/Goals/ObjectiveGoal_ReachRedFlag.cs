using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectiveGoal_ReachRedFlag : ObjectiveGoal
{
    private TileFeature_RedFlag RedFlag;

    public ObjectiveGoal_ReachRedFlag(Tile flagTile = null) : base(ObjectiveGoalDefOf.ReachRedFlag)
    {
        if (flagTile == null) flagTile = Board.Instance.GetRandomTile();
        RedFlag = flagTile.AddRedFlag(this);
    }

    public override void OnRemoved()
    {
        RedFlag.Remove();
    }
}
