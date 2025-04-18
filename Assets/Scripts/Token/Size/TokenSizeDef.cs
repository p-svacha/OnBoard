using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenSizeDef : Def
{
    /// <summary>
    /// The amount that token 3D GameObjects with this size get scaled. 
    /// </summary>
    public float Scale { get; init; }

    /// <summary>
    /// The size in pixels that tokens with this size get displayed in the TurnDraw UI.
    /// </summary>
    public int UiSize { get; init; }

    /// <summary>
    /// How much the effect of the token gets multiplied by this size.
    /// </summary>
    public int EffectMultiplier { get; init; }
}
