using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDefs
{
    public static List<ItemDef> Defs => new List<ItemDef>()
    {
        new ItemDef()
        {
            DefName = "LuckyScarf",
            Label = "lucky scarf",
            Description = "+1 redraw per turn",
            ItemClass = typeof(Item_LuckyScarf),
            Rarity = ItemRarity.Common,
        },

        new ItemDef()
        {
            DefName = "GlovesOfGreed",
            Label = "gloves of greed",
            Description = "Wealth tokens give +1 gold",
            ItemClass = typeof(Item_GlovesOfGreed),
            Rarity = ItemRarity.Common,
        },
    };
}
