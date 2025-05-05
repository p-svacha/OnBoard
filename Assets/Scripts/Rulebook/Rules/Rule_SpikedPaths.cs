using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule_SpikedPaths : Rule
{
    public override Dictionary<TileFeatureDef, float> GetTileFeatureProbabilityModifiers()
    {
        return new Dictionary<TileFeatureDef, float>()
        {
            { TileFeatureDefOf.Spikes, 0.2f },
        };
    }

    public override Dictionary<DamageTag, int> GetDamageModifiers()
    {
        if (Level == 1) return new Dictionary<DamageTag, int>();
        if (Level == 2) return new Dictionary<DamageTag, int>() { { DamageTag.Spike, 1 } };
        if (Level == 3) return new Dictionary<DamageTag, int>() { { DamageTag.Spike, 2 } };
        throw new System.NotImplementedException();
    }
}
