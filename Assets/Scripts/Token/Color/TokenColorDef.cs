using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenColorDef : Def
{
    /// <summary>
    /// The actual material color that tokens with this color have.
    /// </summary>
    public Color Color { get; init; }

    /// <summary>
    /// The resource that gets awarded to the player when drawing a token of this color.
    /// </summary>
    public ResourceDef Resource { get; init; }

    /// <summary>
    /// The amount of the resource that is awarded for a small tokens with this color. This value will be multiplied by the EffectMultiplier of the token size.
    /// </summary>
    public int ResourceBaseAmount { get; init; }
}
