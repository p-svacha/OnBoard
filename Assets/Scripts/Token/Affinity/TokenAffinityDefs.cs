using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenAffinityDefs
{
    public static List<TokenAffinityDef> Defs => new List<TokenAffinityDef>()
    {
        new TokenAffinityDef()
        {
            DefName = "Fury",
            Label = "fury",
            Description = "An affinity related to combat and dealing damage.",
            Color = new Color(0.75f, 0.25f, 0.25f),
            Sprite = ResourceManager.LoadSprite("Sprites/Affinities/Fury"),
        },

        new TokenAffinityDef()
        {
            DefName = "Growth",
            Label = "growth",
            Description = "An affinity related to healing and upgrading.",
            Color = new Color(0.35f, 0.6f, 0.35f),
            Sprite = ResourceManager.LoadSprite("Sprites/Affinities/Growth"),
        },

        new TokenAffinityDef()
        {
            DefName = "Wealth",
            Label = "wealth",
            Description = "An affinity related to gold, resources and items.",
            Color = new Color(0.85f, 0.7f, 0.35f),
            Sprite = ResourceManager.LoadSprite("Sprites/Affinities/Wealth"),
        },

        new TokenAffinityDef()
        {
            DefName = "Flow",
            Label = "flow",
            Description = "An affinity related to movement and drawing.",
            Color = new Color(0.45f, 0.55f, 0.85f),
            Sprite = ResourceManager.LoadSprite("Sprites/Affinities/Flow"),
        },
    };
}
