using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChapterGoal_AffinityTokens : ChapterGoal
{
    public TokenAffinityDef Affinity { get; private set; }
    public int NumTokens { get; private set; }

    protected override void OnInit(int chapter)
    {
        Affinity = DefDatabase<TokenAffinityDef>.AllDefs.RandomElement();
        NumTokens = (chapter / 3) + 3;
    }

    public override void OnLockInSpread(Spread spread)
    {
        if (spread.TableTokens.Keys.Where(t => t.Affinity == Affinity).Count() >= NumTokens)
        {
            Game.Instance.QueueCompleteChapter();
        }
    }

    public override string Description => $"Lock in a spread with at least <b>{NumTokens} {Affinity.Label} tokens</b>.";
}
