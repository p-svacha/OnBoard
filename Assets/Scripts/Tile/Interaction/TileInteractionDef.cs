using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInteractionDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a tile interaction of this def.
    /// </summary>
    public System.Type InteractionClass { get; init; } = typeof(TileInteraction);

    /// <summary>
    /// The resource cost of performing this interaction.
    /// </summary>
    public Dictionary<ResourceDef, int> ResourceCost { get; init; } = new();
}
