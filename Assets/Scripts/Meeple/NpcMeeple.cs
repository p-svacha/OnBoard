using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcMeeple : Meeple
{
    /// <summary>
    /// Gets executed at the end of the turn.
    /// </summary>
    public abstract void OnEndTurn();
}
