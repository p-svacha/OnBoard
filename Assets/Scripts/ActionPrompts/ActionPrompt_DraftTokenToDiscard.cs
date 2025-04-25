using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_DraftTokenToDiscard : ActionPrompt
{
    public override void OnShow()
    {
        GameUI.Instance.TokenDraftDisplay.ShowDraftToDiscard();
    }
}
