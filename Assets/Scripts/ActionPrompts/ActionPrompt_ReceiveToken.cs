using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_ReceiveToken : ActionPrompt
{
    private TokenShapeDef Shape;
    private List<TokenSurface> Surfaces;
    private TokenSizeDef Size;
    private TokenAffinityDef Affinity;

    public ActionPrompt_ReceiveToken(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size, TokenAffinityDef affinity)
    {
        Shape = shape;
        Surfaces = surfaces;
        Size = size;
        Affinity = affinity;
    }

    public ActionPrompt_ReceiveToken(Token tokenData)
    {
        Shape = tokenData.Shape;
        Surfaces = tokenData.Surfaces;
        Size = tokenData.Size;
        Affinity = tokenData.Affinity;
    }

    public override void OnShow()
    {
        Token newToken = Game.Instance.AddTokenToPouch(Shape, Surfaces, Size, Affinity);
        GameUI.Instance.DraftWindow.Show("You received a new token", "", new() { newToken }, isDraft: false);
    }
}
