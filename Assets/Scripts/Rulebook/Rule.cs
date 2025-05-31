using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Rule : IDraftable
{
    /// <summary>
    /// The level defines the severity and difficulty of this rule.
    /// </summary>
    public int Level { get; private set; }
    public RuleDef Def { get; private set; }

    public List<RuleLevel> Levels { get; private set; }

    public void Init(RuleDef def)
    {
        Def = def;
        Level = 1;
        OnInit();

        Levels = new List<RuleLevel>();
        for (int i = 1; i <= def.MaxLevel; i++) Levels.Add(new RuleLevel(this, i, Def.LevelDescriptions[i - 1]));
    }
    protected virtual void OnInit() { }

    public void IncreaseLevel()
    {
        Level++;
    }

    /// <summary>
    /// Gets executed when the given level of the rule starts being active.
    /// </summary>
    public virtual void OnActivate(int level) { }


    #region Rule Effects

    /// <summary>
    /// Gets executed once per turn when the spread is locked in between the preparation and action phase.
    /// </summary>
    public virtual void OnLockInSpread(Spread spread) { }

    public virtual Dictionary<TileFeatureDef, float> GetTileFeatureProbabilityModifiers()
    {
        return new Dictionary<TileFeatureDef, float>();
    }

    public virtual Dictionary<DamageTag, int> GetDamageModifiers()
    {
        return new Dictionary<DamageTag, int>();
    }

    public virtual Dictionary<TileInteractionDef, Dictionary<ResourceDef, int>> GetTileInteractionCostModifiers()
    {
        return new Dictionary<TileInteractionDef, Dictionary<ResourceDef, int>>();
    }

    #endregion

    #region Getters

    /// <summary>
    /// Returns if it is allowed for this rule to be active at the same time as the given other rule.
    /// </summary>
    public virtual bool CanCoexistWith(Rule otherRule)
    {
        return Def != otherRule.Def;
    }

    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public string Description
    {
        get
        {
            string desc = "";
            for(int i = 1; i <= Def.MaxLevel; i++)
            {
                if (i > 1) desc += "\n\n";
                desc += $"<u>Level {i}</u>: {Def.LevelDescriptions[i - 1]}";
            }
            return desc;
        }
    }

    #endregion

    // IDraftable
    public virtual string DraftDisplay_Title => LabelCap;
    public virtual string DraftDisplay_Text => Description;
    public virtual Sprite DraftDisplay_Sprite => null;
    public virtual GameObject DraftDisplay_Spinning3DObject => null;
}
