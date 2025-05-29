using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RuleDefs
{
    public static List<RuleDef> Defs => new List<RuleDef>()
    {
        new RuleDef()
        {
            DefName = "SpikedPaths",
            Label = "spiked paths",
            RuleClass = typeof(Rule_SpikedPaths),
            MaxLevel = 3,
            LevelDescriptions = new List<string>()
            {
                "Spikes are more common in future board expansions. Immediately spawn 5 spikes across the board.",
                "Spikes deal an additional half heart damage. Immediately spawn 5 spikes across the board.",
                "Spikes deal an additional half heart damage. Immediately spawn 5 spikes across the board.",
            },
        },

        new RuleDef()
        {
            DefName = "ForcedAffinity",
            Label = "forced affinity",
            RuleClass = typeof(Rule_ForcedAffinity),
            MaxLevel = 3,
            LevelDescriptions = new List<string>()
            {
                "If your token spread contains no affinities, lose half a heart.",
                "Affinity infusions at infusion fountains cost +1 Gold.",
                "Tokens without an affinity have no effect when drawn.",
            },
        },

        new RuleDef()
        {
            DefName = "ThePursuer",
            Label = "the pursuer",
            RuleClass = typeof(Rule_ThePursuer),
            MaxLevel = 5,
            LevelDescriptions = new List<string>()
            {
                "A Pursuer spawns on a random tile far from the player. Each turn, it moves 1 tile toward the player meeple. If at the end of turn it is on the same tile as the player, take 1 damage.",
                "The pursuer moves an additional tile per turn.",
                "The pursuer also deals damage when on an adjacent tile of the player.",
                "Another pursuer spawns.",
                "Pursuers move an additional tile per turn.",
            }
        }
    };
}
