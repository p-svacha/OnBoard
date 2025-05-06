using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReward_ReceiveToken : QuestReward
{
    protected override void ApplyReward()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_ReceiveToken(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small));
    }
}
