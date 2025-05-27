using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TokenPouch : MonoBehaviour
{
    /// <summary>
    /// The contents of the token pouch.
    /// </summary>
    public List<Token> Tokens = new List<Token>();

    public void AddToken(Token token)
    {
        Tokens.Add(token);
        token.transform.SetParent(transform);
    }
    public void RemoveToken(Token token)
    {
        Tokens.Remove(token);
    }

    #region Getters

    /// <summary>
    /// The amount of tokens in the pouch.
    /// </summary>
    public int Size => Tokens.Count();

    public List<Token> GetTokensExcept(TokenSizeDef size) => Tokens.Where(t => t.Size != size).ToList();
    public List<Token> GetTokensExcept(TokenAffinityDef affinity) => Tokens.Where(t => t.Affinity != affinity).ToList();
    public List<TokenSurface> GetTokenSurfacesExcept(TokenSurfacePatternDef pattern) => Tokens.SelectMany(t => t.Surfaces).Where(s => s.Pattern != pattern).ToList();

    #endregion
}
