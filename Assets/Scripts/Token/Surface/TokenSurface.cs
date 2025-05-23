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
    public TokenSurfacePatternDef Pattern { get; private set; }

    public TokenSurface(Token token, TokenColorDef color, Vector3 normalDirection, TokenSurfacePatternDef pattern = null)
    {
        Token = token;
        SetColor(color);
        SetPattern(pattern);
        NormalDirection = normalDirection;
    }

    public TokenSurface(TokenColorDef color, TokenSurfacePatternDef pattern = null)
    {
        SetColor(color);
        SetPattern(pattern);
    }

    public void SetColor(TokenColorDef color)
    {
        Color = color;
    }

    public void SetPattern(TokenSurfacePatternDef pattern)
    {
        Pattern = pattern;
    }

    public string Label => $"{PatternLabel}{Color.Label}";
    /// <summary>
    /// Displays the surface and token name over two lines. (ie "Black surface \n of medium pebble).
    /// </summary>
    public string GetFullLabel(int secondLineFontSize = -1)
    {
        string sizeTag = secondLineFontSize == -1 ? "" : $"<size={secondLineFontSize}>";
        return $"{Label} surface\n{sizeTag}of {Token.LabelNoSurface}";
    }
    private string PatternLabel => Pattern == null ? "" : $"{Pattern.Label} ";
    public string Description
    {
        get
        {
            string desc = "Does nothing";
            Dictionary<ResourceDef, int> surfaceResources = Token.GetResources(this);
            if(surfaceResources.Count > 0)
            {
                desc = "";
                foreach(var res in surfaceResources)
                {
                    desc += $"{res.Value} {res.Key.LabelDynamicCap(res.Value)}\n";
                }
                desc.TrimEnd('\n');
            }
            return desc;
        }
    }

}
