using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPenalty_BlackPebble : QuestPenalty
{
    private Token PenaltyToken;

    public QuestPenalty_BlackPebble()
    {
        PenaltyToken = TokenGenerator.GenerateToken(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.Black) }, TokenSizeDefOf.Small, hidden: true, frozen: true);
    }

    public override void ApplyPenalty()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_ReceiveToken(PenaltyToken));
    }

    public override void OnRemoved()
    {
        PenaltyToken.DestroySelf();
    }
}
