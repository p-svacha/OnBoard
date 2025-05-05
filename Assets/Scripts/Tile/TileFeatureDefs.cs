using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileFeatureDefs
{
    public static List<TileFeatureDef> Defs => new List<TileFeatureDef>()
    {
        new TileFeatureDef()
        {
            DefName = "Start",
            Label = "start",
            Description = "The tile where everything started. Does nothing except remind you of your roots.",
            TileFeatureClass = typeof(TileFeature_Start),
        },

        new TileFeatureDef()
        {
            DefName = "SpecificTokenGiver",
            Label = "", // handled by feature class
            Description = "Landing on the tile adds a specific kind of token to your pouch.",
            TileFeatureClass = typeof(TileFeature_SpecificTokenGiver),
        },

        new TileFeatureDef()
        {
            DefName = "RedFlag",
            Label = "red flag",
            Description = "Symbolizes the target tile for goals.",
            TileFeatureClass = typeof(TileFeature_RedFlag),
            MeepleCanStopOn = true
        },

        new TileFeatureDef()
        {
            DefName = "TokenBin",
            Label = "token bin",
            Description = "When landing on the tile, draft a token from your pouch to discard.",
            TileFeatureClass = typeof(TileFeature_TokenBin),
        },

        new TileFeatureDef()
        {
            DefName = "Spikes",
            Label = "spikes",
            Description = "Landing on this tile causes you to lose half a heart.",
            TileFeatureClass = typeof(TileFeature_Spikes),
        }
    };
}
