using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPenalty_BlackPebble : QuestPenalty
{
    protected override void ApplyPenalty()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_ReceiveToken(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.Black) }, TokenSizeDefOf.Small));
    }
}
