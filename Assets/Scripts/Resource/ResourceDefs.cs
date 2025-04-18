using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceDefs
{
    public static List<ResourceDef> Defs => new List<ResourceDef>()
    {
        new ResourceDef()
        {
            DefName = "MovementPoint",
            Label = "movement point",
            LabelPlural = "movement points",
            Description = "The amount of tiles that have to be moved this turn.",
        }
    };
}
