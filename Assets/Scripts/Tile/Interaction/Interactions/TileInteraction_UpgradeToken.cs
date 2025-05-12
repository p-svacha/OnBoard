using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileInteraction_UpgradeToken : TileInteraction
{
    protected override void OnExecute()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_DraftToken(Feature.LabelCap, "Choose which token to upgrade", GetDraftOptions(), OnDrafted));
    }

    private void OnDrafted(List<IDraftable> draftResult)
    {
        foreach (Token token in draftResult.Select(d => (Token)d))
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
