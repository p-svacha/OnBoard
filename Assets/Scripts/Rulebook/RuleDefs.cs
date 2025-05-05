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
        }
    };
}
