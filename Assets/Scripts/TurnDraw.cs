using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnDraw
{
    /// <summary>
    /// The set of tokens that are currently still in the pouch.
    /// </summary>
    public List<Token> PouchTokens;

    /// <summary>
    /// The set of tokens that are on the table and have their effects applied.
    /// </summary>
    public List<Token> TableTokens;

    /// <summary>
    /// The set of tokens that have been drawn from the pouch but discarded somehow.
    /// </summary>
    public List<Token> DiscardedTokens;

    /// <summary>
    /// The amount of every resource that gets awarded from this draw.
    /// </summary>
    public Dictionary<ResourceDef, int> Resources;

    public TurnDraw()
    {
        Resources = new Dictionary<ResourceDef, int>();
        DrawInitialTokens();
        UpdateResult();
    }

    /// <summary>
    /// Discards a token currently on the table and draws a new one.
    /// Returns the new token drawn.
    /// </summary>
    public Token RedrawToken(Token token)
    {
        if (PouchTokens.Count == 0) return null;

        DiscardToken(token);
        Token drawnToken = DrawToken();
        UpdateResult();

        GameUI.Instance.TurnDraw.Refresh();
        return drawnToken;
    }

    /// <summary>
    /// Each turn starts by drawing tokens out of your pouch.
    /// </summary>
    private void DrawInitialTokens()
    {
        int drawAmount = Game.Instance.DrawAmount;
        int pouchSize = Game.Instance.TokenPouch.Count();
        if (drawAmount > pouchSize) drawAmount = pouchSize;
        PouchTokens = new List<Token>(Game.Instance.TokenPouch);
        TableTokens = new List<Token>();
        DiscardedTokens = new List<Token>();
        for (int i = 0; i < drawAmount; i++)
        {
            DrawToken();
        }
    }

    /// <summary>
    /// Draws a random token from the pouch.
    /// </summary>
    private Token DrawToken()
    {
        Token token = PouchTokens.RandomElement();
        TableTokens.Add(token);
        PouchTokens.Remove(token);
        return token;
    }

    /// <summary>
    /// Discards a specific table token.
    /// </summary>
    private void DiscardToken(Token token)
    {
        DiscardedTokens.Add(token);
        TableTokens.Remove(token);
    }

    /// <summary>
    /// Updates all result values (like MovementPoints, Gold, etc.) from the currently drawn tokens.
    /// </summary>
    private void UpdateResult()
    {
        // Reset
        Resources.Clear();

        foreach (Token token in TableTokens)
        {
            // Add resource from color and size
            if(token.Color.Resource != null)
            {
                int amount = token.Color.ResourceBaseAmount * token.Size.EffectMultiplier;
                Resources.Increment(token.Color.Resource, amount);
            }
        }
    }
}
