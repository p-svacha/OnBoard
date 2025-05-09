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

    // IDraftable
    public override string DraftDisplay_Text => Def.LabelCap;
    public override Sprite DraftDisplay_Sprite => null;
    public override GameObject DraftDisplay_Spinning3DObject => RewardToken.gameObject;
}
