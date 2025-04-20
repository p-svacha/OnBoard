using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MajorGoal_ReachRedFlag : MajorGoal
{
    private TileFeature_RedFlag RedFlag;

    public MajorGoal_ReachRedFlag(Tile flagTile = null) : base(MajorGoalDefOf.ReachRedFlag)
    {
        if (flagTile == null) flagTile = Board.Instance.GetRandomTile();
        RedFlag = flagTile.AddRedFlag(this);
    }

    public override void OnRemoved()
    {
        RedFlag.Remove();
    }
}
