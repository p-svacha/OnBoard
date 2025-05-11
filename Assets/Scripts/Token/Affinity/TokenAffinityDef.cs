using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each token can optionally have an affinity. Affinities don't do anything by themselves but interact with many other systems. They act as synergy potential.
/// </summary>
public class TokenAffinityDef : Def
{
    /// <summary>
    /// The color associated with the affinity.
    /// </summary>
    public Color Color { get; init; }
}
