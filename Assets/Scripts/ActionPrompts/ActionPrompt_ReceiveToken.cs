using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_ReceiveToken : ActionPrompt
{
    private TokenShapeDef Shape;
    private List<TokenSurface> Surfaces;
    private TokenSizeDef Size;

    public ActionPrompt_ReceiveToken(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size)
    {
        Shape = shape;
        Surfaces = surfaces;
        Size = size;
    }

    public override void OnShow()
    {
        Token newToken = Game.Instance.AddTokenToPouch(Shape, Surfaces, Size);
        GameUI.Instance.ItemReceivedDisplay.ShowTokenReceived(newToken);
    }
}
