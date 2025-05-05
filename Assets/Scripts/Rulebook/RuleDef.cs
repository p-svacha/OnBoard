using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a rule of this def.
    /// </summary>
    public System.Type RuleClass { get; init; } = typeof(Rule);

    /// <summary>
    /// The maximum level a rule of this def can reach.
    /// </summary>
    public int MaxLevel { get; init; } = 5;

    /// <summary>
    /// The human-readable description of the effect that each level of this rule has.
    /// </summary>
    public List<string> LevelDescriptions { get; init; }
}
