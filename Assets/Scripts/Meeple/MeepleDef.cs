using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeepleDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a meeple of this type.
    /// </summary>
    public System.Type MeepleClass { get; init; } = typeof(NpcMeeple);

    /// <summary>
    /// The list of interactions that can be performed when on the same tile as this meeple.
    /// </summary>
    public List<TileInteractionDef> Interactions { get; init; } = new();

    /// <summary>
    /// The minimum amount of tiles the meeple moves at the end of the turn.
    /// </summary>
    public int MovementSpeedMin { get; init; }

    /// <summary>
    /// The maximum amount of tiles the meeple moves at the end of the turn.
    /// </summary>
    public int MovementSpeedMax { get; init; }

    /// <summary>
    /// The behaviour of how and where this meeple moves.
    /// </summary>
    public MeepleMovementType MovementType { get; init; } = MeepleMovementType.RandomWander;
}
