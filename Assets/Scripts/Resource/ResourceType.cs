using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    /// <summary>
    /// Collectable resources are the most standard and straightforward type of resources. They represent something material and can be collected and used throughout the game.
    /// </summary>
    Collectable,

    /// <summary>
    /// Drawing phase resources are abstract non-material resources that only show up and can only be used during the drawing phase to manipulate the draw.
    /// </summary>
    DrawingPhaseResource,

    /// <summary>
    /// Drawing phase resources are abstract non-material resources that only show up and can only be used during the moving phase. They define how the meeple can and must move in a turn.
    /// </summary>
    MovingPhaseResource
}
