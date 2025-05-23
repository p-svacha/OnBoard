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

    /// <summary>
    /// If greater than 0, this defines the maximum times the interaction can be used in a turn.
    /// </summary>
    public int MaxUsesPerTurn { get; init; } = -1;
}
