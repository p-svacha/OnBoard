using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_TokenReceived : ActionPrompt
{
    private TokenShapeDef Shape;
    private TokenColorDef Color;
    private TokenSizeDef Size;

    public ActionPrompt_TokenReceived(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size)
    {
        Shape = shape;
        Color = color;
        Size = size;
    }

    public override void OnShow()
    {
        Token newToken = Game.Instance.AddTokenToPouch(Shape, Color, Size);
        GameUI.Instance.ItemReceivedDisplay.ShowTokenReceived(newToken);
    }
}
