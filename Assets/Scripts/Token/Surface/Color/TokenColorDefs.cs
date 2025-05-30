using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenColorDefs
{
    public static List<TokenColorDef> Defs => new List<TokenColorDef>()
    {
        new TokenColorDef()
        {
            DefName = "Black",
            Label = "black",
            Description = "Does absolutely nothing.",
            Color = new Color(0.2f, 0.2f, 0.2f),
        },

        new TokenColorDef()
        {
            DefName = "White",
            Label = "white",
            Description = "Awards movement points.",
            Color = new Color(0.9f, 0.9f, 0.9f),
            Resource = ResourceDefOf.MovementPoint,
            ResourceBaseAmount = 1,
        },

        new TokenColorDef()
        {
            DefName = "Gold",
            Label = "gold",
            Description = "Gives gold.",
            Color = new Color(1f, 0.84f, 0f),
            Resource = ResourceDefOf.Gold,
            ResourceBaseAmount = 1,
        },
    };
}
