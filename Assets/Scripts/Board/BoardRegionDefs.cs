using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardRegionDefs
{
    public static List<BoardRegionDef> Defs => new List<BoardRegionDef>()
    {
        new BoardRegionDef()
        {
            DefName = "Swamp",
            Label = "swamp",
            Description = "A region that is very slow to pass through.",
            RegionClass = typeof(BoardRegion_Swamp),
            MinTiles = 18,
            MaxTiles = 22,
            TileFeatureProbabilities = new Dictionary<TileFeatureDef, float>()
            {
                { TileFeatureDefOf.SpecificTokenGiver, 0.05f },
                { TileFeatureDefOf.TokenBin, 0.05f },
                { TileFeatureDefOf.Spikes, 0.05f },
                { TileFeatureDefOf.UpgradeStand, 0.05f },
                { TileFeatureDefOf.InfusionFountain, 0.05f },
            }
        }
    };
}
