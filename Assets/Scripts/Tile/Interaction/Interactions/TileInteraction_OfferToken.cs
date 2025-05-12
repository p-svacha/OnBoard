using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileInteraction_OfferToken : TileInteraction
{
    public TileFeature_Altar Altar => (TileFeature_Altar)Feature;

    protected override void OnExecute()
    {
        List<Token> eligibleTokens = GetEligibleTokens();
        if (eligibleTokens.Count() == 1)
        {
            DeliverToken(eligibleTokens[0]);
        }
        else if (eligibleTokens.Count() > 1)
        {
            Game.Instance.QueueActionPrompt(new ActionPrompt_DraftToken(Feature.LabelCap, "Choose which token to offer", GetEligibleTokens(), OnDrafted));
        }
    }

    private void OnDrafted(List<IDraftable> draftResult)
    {
        Token chosenToken = (Token)draftResult[0];
        DeliverToken(chosenToken);
    }

    private void DeliverToken(Token token)
    {
        Game.Instance.RemoveTokenFromPouch(token);
        Game.Instance.QueueCompleteChapter();
    }

    protected override bool CanExecute_CustomChecks(out string unavailableReason)
    {
        unavailableReason = "";
        if (GetEligibleTokens().Count() == 0)
        {
            unavailableReason = "No token matches the requirements.";
            return false;
        }
        return true;
    }

    private List<Token> GetEligibleTokens()
    {
        return Game.Instance.TokenPouch.Where(t => t.Shape == Altar.Shape && t.Size == Altar.Size && t.Surfaces.Any(s => s.Color == Altar.Color)).ToList();
    }
}
