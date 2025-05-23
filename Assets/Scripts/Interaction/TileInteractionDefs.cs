using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileInteractionDefs
{
    public static List<TileInteractionDef> Defs => new List<TileInteractionDef>()
    {
        new TileInteractionDef()
        {
            DefName = "OfferToken",
            Label = "offer token",
            Description = "Deliver the requested token to complete the chapter.",
            InteractionClass = typeof(TileInteraction_OfferToken),
        },

        new TileInteractionDef()
        {
            DefName = "UpgradeToken",
            Label = "upgrade token",
            Description = "Pay to upgrade the size of a drafted token.",
            InteractionClass = typeof(TileInteraction_UpgradeToken),
            ResourceCost = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Gold, 2 }
            },
            MaxUsesPerTurn = 1,
        },

        new TileInteractionDef()
        {
            DefName = "InfuseToken",
            Label = "infuse token",
            Description = "Pay to infuse a drafted token with a specific affinity.",
            InteractionClass = typeof(TileInteraction_InfuseToken),
            ResourceCost = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Gold, 2 }
            },
            MaxUsesPerTurn = 1,
        },

        new TileInteractionDef()
        {
            DefName = "Trade",
            Label = "trade",
            Description = "Buy and sell things.",
            InteractionClass = typeof(TileInteraction_Trade),
        },
    };
}
