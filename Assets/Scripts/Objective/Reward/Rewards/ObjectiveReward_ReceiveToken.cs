using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveReward_ReceiveToken : ObjectiveReward
{
    public override void ApplyReward()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_TokenReceived(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small));
    }
}
