using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionPrompt_DraftTokenToDiscard : ActionPrompt
{
    public override void OnShow()
    {
        // Options
        List<Token> options = new List<Token>();
        List<Token> candidates = new List<Token>(Game.Instance.TokenPouch);
        int drawAmount = Game.Instance.GetDraftOptionsAmount();
        for (int i = 0; i < drawAmount; i++)
        {
            Token chosenToken = candidates.RandomElement();
            candidates.Remove(chosenToken);
            options.Add(chosenToken);
        }

        // Window
        GameUI.Instance.DraftWindow.Show("Discard", "Choose a token to discard", options.Select(t => (IDraftable)t).ToList(), isDraft: true, callback: OnDrafted);
    }

    private void OnDrafted(List<IDraftable> draftResult)
    {
        foreach (Token token in draftResult.Select(d => (Token)d))
        {
            Game.Instance.RemoveTokenFromPouch(token);
        }
    }
}
