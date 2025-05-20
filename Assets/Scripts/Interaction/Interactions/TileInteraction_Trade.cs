using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInteraction_Trade : TileInteraction
{
    private Meeple_Merchant Merchant => (Meeple_Merchant)Meeple;

    protected override void OnExecute()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_Trade(Merchant.GetNewTradingSession()));
    }
}
