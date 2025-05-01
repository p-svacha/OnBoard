using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRegionDef : Def
{
    /// <summary>
    /// The component that will be attached to a board region object region of this def.
    /// </summary>
    public System.Type RegionClass { get; init; } = typeof(BoardRegion);

    /// <summary>
    /// The minimun amount of tiles regions of this def must have.
    /// </summary>
    public int MinTiles;

    /// <summary>
    /// The maximum amount of tiles regions of this def can have.
    /// </summary>
    public int MaxTiles;

}
