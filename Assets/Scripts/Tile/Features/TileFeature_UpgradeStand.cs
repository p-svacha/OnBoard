using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileFeature_UpgradeStand : TileFeature
{
    protected override void OnInitVisuals()
    {
        PlaceObjectAroundTile("Prefabs/TileFeatures/ShopStand", offsetDistance: Tile.TILE_RADIUS * 1.6f, angleOffset: 180f);
    }

    public override List<TileInteraction> GetInteractions()
    {
        return new List<TileInteraction>()
        {
            new TileInteraction(
                Upgrade, Tile, this, "Upgrade", "Pay two upgrade the size of a drafted token.",
                resourceCost: new Dictionary<ResourceDef, int>() { { ResourceDefOf.Gold, 2 } }
            ),
        };
    }

    public void Upgrade()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_DraftToken("Upgrade Stand", "Choose which token to upgrade", GetDraftOptions(), OnDrafted));
    }

    private void OnDrafted(List<IDraftable> draftResult)
    {
        foreach(Token token in draftResult.Select(d => (Token)d))
        {
            Game.Instance.UpgradeTokenSize(token);
        }
    }

    private List<Token> GetDraftOptions()
    {
        List<Token> candidates = Game.Instance.TokenPouch.Where(t => t.Size != TokenSizeDefOf.Large).ToList();
        return candidates.RandomElements(Game.Instance.GetDraftOptionsAmount());
    }
}
