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
            new TileInteraction(
                Infuse, Tile, this, "Infuse", $"Pay to infuse a drafted token with the affinity {Affinity.Label}.",
                resourceCost: new Dictionary<ResourceDef, int>() { { ResourceDefOf.Gold, 2 } }
            ),
        };
    }

    public void Infuse()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_DraftToken("Infusion Fountain", $"Choose which token to infuse with {Affinity.Label}", GetDraftOptions(), OnDrafted));
    }

    private List<Token> GetDraftOptions()
    {
        List<Token> candidates = Game.Instance.TokenPouch.Where(t => t.Affinity != Affinity).ToList();
        return candidates.RandomElements(Game.Instance.GetDraftOptionsAmount());
    }

    private void OnDrafted(List<IDraftable> draftResult)
    {
        foreach (Token token in draftResult.Select(d => (Token)d))
        {
            Game.Instance.InfuseTokenAffinity(token, Affinity);
        }
    }

    public override string Label => $"{Affinity.Label} fountain";
    public override string Description => $"Pay 2 gold to infuse a drafted token with the affinity {Affinity.Label}.";
}
