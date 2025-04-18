using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenShapeDef : Def
{
    /// <summary>
    /// How many different possible results can appear when a token with this shape is drawn. Each side can have a different color.
    /// </summary>
    public int NumSides { get; init; }
}
