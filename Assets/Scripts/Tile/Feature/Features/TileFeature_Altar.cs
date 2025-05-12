using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileFeature_Altar : TileFeature
{
    public TokenShapeDef Shape { get; private set; }
    public TokenColorDef Color { get; private set; }
    public TokenSizeDef Size { get; private set; }

    protected override void OnInitVisuals()
    {
        PlaceObjectAroundTile("Prefabs/TileFeatures/Altar", offsetDistance: Tile.TILE_RADIUS * 1.6f);
    }

    public void SetRequiredTokenInfo(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size)
    {
        Shape = shape;
        Color = color;
        Size = size;
    }

    public override List<TileInteraction> GetInteractions()
    {
        return new List<TileInteraction>()
        {
           CreateTileInteraction(TileInteractionDefOf.OfferToken)
        };
    }
}
