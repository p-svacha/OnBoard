using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileFeature_InfusionFountain : TileFeature
{
    public TokenAffinityDef Affinity { get; private set; }

    protected override void OnInitVisuals()
    {
        GameObject obj = PlaceObjectAroundTile("Prefabs/TileFeatures/InfusionFountain", offsetDistance: Tile.TILE_RADIUS * 2, angleOffset: 180f);
        HelperFunctions.GetChild(obj, "Liquid").GetComponent<MeshRenderer>().material.color = Affinity.Color;
    }

    public override void SetRandomParameters()
    {
        Affinity = DefDatabase<TokenAffinityDef>.AllDefs.RandomElement();
    }

    public override List<TileInteraction> GetInteractions()
    {
        return new List<TileInteraction>()
        {
            CreateTileInteraction(TileInteractionDefOf.InfuseToken),
        };
    }

    

    public override string Label => $"{Affinity.Label} fountain";
    public override string Description => $"Pay 2 gold to infuse a drafted token with the affinity {Affinity.Label}.";
}
