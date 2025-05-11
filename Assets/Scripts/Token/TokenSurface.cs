using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A token surface represents a possible effect when "rolling" a token. A token is rolled when drawn and can be rerolled during the drawing phase through different ways.
/// <br/>A surface consists of a color that defines the main effect, and possible additional modifiers.
/// </summary>
public class TokenSurface
{
    public Token Token { get; private set; }
    public TokenColorDef Color { get; private set; }
    public Vector3 NormalDirection { get; private set; }

    public TokenSurface(Token token, TokenColorDef color, Vector3 normalDirection)
    {
        Token = token;
        SetColor(color);
        NormalDirection = normalDirection;
    }

    public TokenSurface(TokenColorDef color)
    {
        SetColor(color);
    }

    public void SetColor(TokenColorDef color)
    {
        Color = color;
    }

    public string Label => $"{Color.Label}";
    public string Description
    {
        get
        {
            string desc = "Does nothing";
            if (Color.Resource != null)
            {
                int amount = Color.ResourceBaseAmount * Token.Size.EffectMultiplier;
                desc = $"{amount} {(amount == 1 ? Color.Resource.LabelCap : Color.Resource.LabelPluralCap)}";
            }
            return desc;
        }
    }

}
