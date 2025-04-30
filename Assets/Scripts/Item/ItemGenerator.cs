using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemGenerator
{
    public static Item GenerateRandomItem()
    {
        List<ItemDef> candidates = new List<ItemDef>(DefDatabase<ItemDef>.AllDefs);
        ItemDef chosenDef = candidates.RandomElement();

        Item item = (Item)System.Activator.CreateInstance(chosenDef.ItemClass);
        item.Init(chosenDef);

        return item;
    }
}
