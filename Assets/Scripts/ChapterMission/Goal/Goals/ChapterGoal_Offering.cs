using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterGoal_Offering : ChapterGoal
{
    public TokenShapeDef Shape { get; private set; }
    public TokenColorDef Color { get; private set; }
    public TokenSizeDef Size { get; private set; }

    public TileFeature_Altar TargetAltar { get; private set; }

    protected override void OnInit()
    {
        // Generate the kind of token that needs to be delivered
        Shape = DefDatabase<TokenShapeDef>.AllDefs.RandomElement();
        Color = DefDatabase<TokenColorDef>.AllDefs.RandomElement();
        Size = DefDatabase<TokenSizeDef>.AllDefs.RandomElement();
    }

    public override void OnStarted()
    {
        // Place alter
        Tile targetTile = Game.Instance.Board.GetRandomTile();
        TargetAltar = targetTile.AddFeature(TileFeatureDefOf.Altar) as TileFeature_Altar;
    }

    public override string Description => $"Deliver a <b>{Size.Label} {Shape.Label}</b> with a <b>{Color.Label}</b> surface to the altar.";
}
