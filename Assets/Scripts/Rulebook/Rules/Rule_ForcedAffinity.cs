using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rule_ForcedAffinity : Rule
{
    public override void OnLockInSpread(Spread spread)
    {
        if (!spread.TableTokens.Keys.Any(t => t.Affinity != null))
        {
            Game.Instance.TakeDamage(1);
        }

        if (Level >= 3)
        {
            List<Token> tokensToDiscard = spread.TableTokens.Keys.Where(t => !t.HasAffinity).ToList();
            foreach (Token t in tokensToDiscard) Game.Instance.DiscardToken(t);
        }
    }

    public override Dictionary<TileInteractionDef, Dictionary<ResourceDef, int>> GetTileInteractionCostModifiers()
    {
        if (Level >= 2)
        {
            return new Dictionary<TileInteractionDef, Dictionary<ResourceDef, int>>()
            {
                { TileInteractionDefOf.InfuseToken, new () { {ResourceDefOf.Gold, 1 } } }
            };
        }

        return new();
    }
}
