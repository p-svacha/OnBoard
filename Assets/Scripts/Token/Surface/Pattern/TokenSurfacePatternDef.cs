using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each token surface can optionally have a pattern additional to it's color, that somehow modifiers the behaviour, when that surface is rolled.
/// </summary>
public class TokenSurfacePatternDef : Def
{
    /// <summary>
    /// The amount this pattern modifies all resource output of the surface.
    /// </summary>
    public float GlobalResourceFactor { get; init; } = 1f;
}
