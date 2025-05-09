using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReward_ReceiveToken : QuestReward
{
    private Token RewardToken;

    public QuestReward_ReceiveToken()
    {
        RewardToken = TokenGenerator.GenerateToken(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small, hidden: true, frozen: true);
    }

    public override void ApplyReward()
    {
        Game.Instance.QueueActionPrompt(new ActionPrompt_ReceiveToken(RewardToken));
    }

    public override void OnRemoved()
    {
        RewardToken.DestroySelf();
    }

    public override string Label => RewardToken.Label;
}
