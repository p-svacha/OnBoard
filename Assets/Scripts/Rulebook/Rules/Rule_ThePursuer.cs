using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule_ThePursuer : Rule
{
    public override void OnActivate(int level)
    {
        if (level == 1) Game.Instance.QueueActionPrompt(new ActionPrompt_AddMeeple(MeepleDefOf.Pursuer, Game.Instance.Board.GetRandomTile()));
    }

    public int GetPursuerMovementSpeed()
    {
        if (Level < 2) return 1;
        if (Level < 5) return 2;
        return 3;
    }
}
