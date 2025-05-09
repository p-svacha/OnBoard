using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// An action prompt that shows a given selection of tokens to draft from.
/// </br>It will return the token the player has selected.
/// </summary>
public class ActionPrompt_DraftToken : ActionPrompt
{
    private string Title;
    private string Subtitle;
    private List<Token> Tokens;

    /// <summary>
    /// The function that gets executed when closing / confirming the draft window. The IDraftables passed represent the chosen options.
    /// </summary>
    private System.Action<List<IDraftable>> Callback;

    public ActionPrompt_DraftToken(string title, string subtitle, List<Token> tokens, System.Action<List<IDraftable>> callback)
    {
        Title = title;
        Subtitle = subtitle;
        Tokens = tokens;
        Callback = callback;
    }

    public override void OnShow()
    {
        GameUI.Instance.DraftWindow.Show(Title, Subtitle, Tokens.Select(t => (IDraftable)t).ToList(), isDraft: true, Callback);
    }
}
