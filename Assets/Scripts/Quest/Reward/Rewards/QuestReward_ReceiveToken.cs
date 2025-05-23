using System;
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

    public override List<IDraftable> GetRewardOptions()
    {
        return new List<IDraftable>() { RewardToken };
    }

    public override void ApplyReward(IDraftable reward)
    {
        Game.Instance.AddTokenToPouch(((Token)reward).GetCopy());
    }

    public override void OnRemoved()
    {
        RewardToken.DestroySelf();
    }

    public override string Label => RewardToken.Label;
}
