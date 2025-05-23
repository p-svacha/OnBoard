using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestReward_RipplePattern : QuestReward
{
    public override string RewardDraftTitle => "Choose a surface to apply the Rippled pattern to";
    public override List<IDraftable> GetRewardOptions()
    {
        List<TokenSurface> candidates = Game.Instance.TokenPouch.GetTokenSurfacesExcept(TokenSurfacePatternDefOf.Rippled);
        List<TokenSurface> options = candidates.RandomElements(Game.Instance.GetDraftOptionsAmount());
        return options.Select(s => (IDraftable)s).ToList();
    }
    public override void ApplyReward(IDraftable reward)
    {
        Game.Instance.SetTokenSurfacePattern((TokenSurface)reward, TokenSurfacePatternDefOf.Rippled);
    }
}
