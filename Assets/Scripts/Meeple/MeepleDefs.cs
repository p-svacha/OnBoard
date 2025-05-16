using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeepleDefs
{
    public static List<MeepleDef> Defs => new List<MeepleDef>()
    {
        new MeepleDef()
        {
            DefName = "Merchant",
            Label = "merchant",
            Description = "A neutral meeple who wanders the board. If you're on the same tile, you can interact to trade tokens or resources.",
            MeepleClass = typeof(Meeple_Merchant),
            Interactions = new List<TileInteractionDef>()
            {

            },
            MovementSpeedMin = 1,
            MovementSpeedMax = 2,
        }
    };
}
