using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileFeature_UpgradeStand : TileFeature
{
    protected override void OnInitVisuals()
    {
        PlaceObjectAroundTile("Prefabs/TileFeatures/ShopStand", offsetDistance: Tile.TILE_RADIUS * 1.6f, angleOffset: 180f);
    }

    public override List<TileInteraction> GetInteractions()
    {
        return new List<TileInteraction>()
        {
            CreateTileInteraction(TileInteractionDefOf.UpgradeToken),
        };
    }


}
