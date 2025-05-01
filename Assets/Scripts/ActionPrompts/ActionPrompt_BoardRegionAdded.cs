using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_BoardRegionAdded : ActionPrompt
{
    public override void OnShow()
    {
        BoardGenerator.GenerateAndAttachRandomRegion(OnAnimationComplete);
    }

    private void OnAnimationComplete()
    {
        Game.Instance.CompleteCurrentActionPrompt();
    }
}
