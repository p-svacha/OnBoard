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
            NumSurfaces = 1
        },

        new TokenShapeDef()
        {
            DefName = "Coin",
            Label = "coin",
            Description = "A coin that has 2 different sides, thus 2 possible outcomes.",
            NumSurfaces = 2,
            SurfaceLocalNormals = new List<Vector3>()
            {
                Vector3.up,
                Vector3.down
            }
        },
    };
}
