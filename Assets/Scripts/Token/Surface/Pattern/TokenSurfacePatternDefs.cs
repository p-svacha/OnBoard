using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenSurfacePatternDefs
{
    public static List<TokenSurfacePatternDef> Defs => new List<TokenSurfacePatternDef>()
    {
        new TokenSurfacePatternDef()
        {
            DefName = "Rippled",
            Label = "rippled",
            Description = "Surface activates twice.",
            GlobalResourceFactor = 2f,
        }
    };
}
