using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReward_ReceiveToken : QuestReward
{
    public override void ApplyReward()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_TokenReceived(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small));
    }
}
