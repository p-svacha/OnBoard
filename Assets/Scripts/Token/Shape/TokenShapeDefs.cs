using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenShapeDefs
{
    public static List<TokenShapeDef> Defs => new List<TokenShapeDef>()
    {
        new TokenShapeDef()
        {
            DefName = "Pebble",
            Label = "pebble",
            Description = "A pebble that always does the same when drawn.",
            NumSides = 1
        }
    };
}
