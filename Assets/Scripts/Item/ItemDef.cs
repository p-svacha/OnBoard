using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating an item of this type.
    /// </summary>
    public Type ItemClass { get; init; } = typeof(Item);

    /// <summary>
    /// The rarity of the item. Rarers items are generally better and more impactful than more common ones, but also harder to get.
    /// </summary>
    public ItemRarity Rarity { get; init; }
}
