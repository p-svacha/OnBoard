using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPrompt_ChapterComplete : ActionPrompt
{
    public override void OnShow()
    {
        Game.Instance.DoCompleteChapter();
    }
}
