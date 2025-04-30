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
            Description = "+1 Redraw per turn",
            ItemClass = typeof(Item_LuckyScarf),
            Rarity = ItemRarity.Common,
        }
    };
}
