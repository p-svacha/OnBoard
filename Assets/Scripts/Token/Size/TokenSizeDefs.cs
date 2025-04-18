using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenSizeDefs
{
    public static List<TokenSizeDef> Defs => new List<TokenSizeDef>()
    {
        new TokenSizeDef()
        {
            DefName = "Small",
            Label = "small",
            Description = "The smallest of sizes with the weakest possible effect.",
            Scale = 0.35f,
            UiSize = 20,
            EffectMultiplier = 1,
        },

        new TokenSizeDef()
        {
            DefName = "Medium",
            Label = "medium",
            Description = "Medium sized for a slightly stronger effect.",
            Scale = 0.7f,
            UiSize = 35,
            EffectMultiplier = 2,
        },

        new TokenSizeDef()
        {
            DefName = "Big",
            Label = "big",
            Description = "Big in size for a big effect.",
            Scale = 1.05f,
            UiSize = 50,
            EffectMultiplier = 3,
        },

        new TokenSizeDef()
        {
            DefName = "Large",
            Label = "large",
            Description = "The biggest of sizes for a very big effect.",
            Scale = 1.5f,
            UiSize = 65,
            EffectMultiplier = 5,
        },
    };
}
