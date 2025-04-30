using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestGoal_ReachRedFlag : QuestGoal
{
    private TileFeature_RedFlag RedFlag;

    protected override void OnInit()
    {
        Tile flagTile = Board.Instance.GetRandomTile();
        RedFlag = flagTile.AddRedFlag(this);
    }

    public override void OnRemoved()
    {
        RedFlag.Remove();
    }
}
