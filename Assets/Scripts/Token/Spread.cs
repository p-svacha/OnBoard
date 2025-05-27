using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spread
{
    /// <summary>
    /// The set of tokens that are currently still in the pouch.
    /// </summary>
    public List<Token> PouchTokens;

    /// <summary>
    /// The set of tokens that are on the table, with information about which surface of the token is currently rolled.
    /// </summary>
    public Dictionary<Token, TokenSurface> TableTokens;

    /// <summary>
    /// The set of tokens that have been drawn from the pouch but discarded somehow.
    /// </summary>
    public List<Token> DiscardedTokens;

    /// <summary>
    /// The amount of every resource that gets awarded from this draw.
    /// </summary>
    public Dictionary<ResourceDef, int> Resources;

    public Spread()
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

        return drawnToken;
    }

    /// <summary>
    /// Each turn starts by drawing tokens out of your pouch.
    /// </summary>
    private void DrawInitialTokens()
    {
        int drawAmount = Game.Instance.DrawAmount;
        int pouchSize = Game.Instance.TokenPouch.Size;
        if (drawAmount > pouchSize) drawAmount = pouchSize;
        PouchTokens = new List<Token>(Game.Instance.TokenPouch.Tokens);
        TableTokens = new Dictionary<Token, TokenSurface>();
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
        TableTokens.Add(token, null); // Do not assign which surface is rolled yet, token has to land physically first
        PouchTokens.Remove(token);
        return token;
    }

    /// <summary>
    /// Discards a specific table token.
    /// </summary>
    public void DiscardToken(Token token)
    {
        DiscardedTokens.Add(token);
        TableTokens.Remove(token);
        UpdateResult();
    }

    public void SetRolledSurface(Token token, TokenSurface surface)
    {
        if (!TableTokens.ContainsKey(token)) return;

        TableTokens[token] = surface;
        UpdateResult();
    }

    /// <summary>
    /// Updates all result values (like MovementPoints, Gold, etc.) from the currently drawn tokens.
    /// </summary>
    private void UpdateResult()
    {
        // Reset
        Resources.Clear();

        foreach (var t in TableTokens)
        {
            Token token = t.Key;
            TokenSurface surface = t.Value;

            Resources.IncrementMultiple(token.GetResources(surface));
        }
    }

    public bool AreAllTokensResting()
    {
        return TableTokens.All(t => t.Value != null);
    }

    public Dictionary<ResourceDef, int> GetMovingPhaseResources()
    {
        return Resources.Where(x => x.Key.Type == ResourceType.MovingPhaseResource).ToDictionary(x => x.Key, x => x.Value);
    }
    public Dictionary<ResourceDef, int> GetCollectableResources()
    {
        return Resources.Where(x => x.Key.Type == ResourceType.Collectable).ToDictionary(x => x.Key, x => x.Value);
    }
}
