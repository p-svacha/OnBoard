using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileInteraction_InfuseToken : TileInteraction
{
    private TokenAffinityDef Affinity => ((TileFeature_InfusionFountain)Feature).Affinity;

    protected override void OnExecute()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_DraftToken(Feature.LabelCap, $"Choose which token to infuse with {Affinity.Label}", GetDraftOptions(), OnDrafted));
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
}
