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
            Type = ResourceType.MovingPhaseResource,
        },

        new ResourceDef()
        {
            DefName = "Redraw",
            Label = "redraw",
            LabelPlural = "redraws",
            Description = "During drawing phase, a redraw allows to discard a drawn token and draw a new one from the pouch.",
            Type = ResourceType.DrawingPhaseResource,
        },

        new ResourceDef()
        {
            DefName = "Gold",
            Label = "gold",
            LabelPlural = "gold",
            Description = "A versatile currency used for various interactions, upgrades, and opportunities. You'll never have quite enough.",
            Type = ResourceType.Collectable,
        }
    };
}
