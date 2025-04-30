using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_LuckyScarf : Item
{
    public override Dictionary<ResourceDef, int> GetDrawPhaseResources()
    {
        return new Dictionary<ResourceDef, int>()
        {
            { ResourceDefOf.Redraw, 1 }
        };
    }
}
