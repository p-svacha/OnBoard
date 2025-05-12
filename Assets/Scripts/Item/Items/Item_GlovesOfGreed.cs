using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_GlovesOfGreed : Item
{
    public override Dictionary<ResourceDef, int> GetTokenResourceModifiers(Token t)
    {
        if(t.Affinity == TokenAffinityDefOf.Wealth)
        {
            return new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Gold, 1 }
            };
        }
        return new();
    }
}
