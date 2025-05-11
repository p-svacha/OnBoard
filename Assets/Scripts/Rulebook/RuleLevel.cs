using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains no inherent logic, acts as a container to hold rule descriptions to make levels displayable in the modular draft window.
/// </summary>
public class RuleLevel : IDraftable
{
    public Rule Rule { get; private set; }
    public int Level { get; private set; }
    public string Description { get; private set; }

    public RuleLevel(Rule rule, int level, string description)
    {
        Rule = rule;
        Level = level;
        Description = description;
    }

    // IDraftable
    public virtual string DraftDisplay_Title
    {
        get
        {
            string s = $"Level {Level}";
            if (Rule.Level < Level) s += "\n<size=16>inactive</size>";
            else
            {
                s = "<color=#DD0000>" + s + "\n<size=16>active</size>";
            }
            return s;
        }
    }
    public virtual string DraftDisplay_Text => Description;
    public virtual Sprite DraftDisplay_Sprite => null;
    public virtual GameObject DraftDisplay_Spinning3DObject => null;
}
