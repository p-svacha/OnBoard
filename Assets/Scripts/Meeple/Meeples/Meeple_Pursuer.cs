using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple_Pursuer : NpcMeeple
{
    private Rule_ThePursuer _Rule;
    public Rule_ThePursuer Rule
    {
        get
        {
            if (_Rule == null) _Rule = (Rule_ThePursuer)Game.Instance.Rulebook.GetRule(RuleDefOf.ThePursuer);
            return _Rule;
        }
    }

    public override void OnTurnPassed()
    {
    }

    protected override int MovementSpeed => Rule.GetPursuerMovementSpeed();
}
