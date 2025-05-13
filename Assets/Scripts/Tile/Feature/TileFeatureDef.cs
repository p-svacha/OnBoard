using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeatureDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a tile feature of this type.
    /// </summary>
    public Type TileFeatureClass { get; init; } = typeof(Tile);

    /// <summary>
    /// If true, a meeple is always allowed to stop on a tile with this feature, even if not all movement points are used up.
    /// </summary>
    public bool MeepleCanStopOn { get; init; }

    /// <summary>
    /// The list of interactions that can be performed when on the same tile as this feature.
    /// </summary>
    public List<TileInteractionDef> Interactions { get; init; } = new();
}
