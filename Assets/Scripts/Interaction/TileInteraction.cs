using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A tile interaction represents a possible action a player can do during the movement phase while a meeple stands on the tile.
/// </summary>
public abstract class TileInteraction
{
    public TileInteractionDef Def { get; private set; }

    /// <summary>
    /// The tile this interaction belongs to.
    /// </summary>
    public Tile Tile { get; private set; }

    /// <summary>
    /// The tile feature this interaction belongs to. May be null.
    /// </summary>
    public TileFeature Feature { get; private set; }

    /// <summary>
    /// The meeple this interaction belongs to. May be null.
    /// </summary>
    public Meeple Meeple { get; private set; }

    public bool NoRemainingUsesThisTurn { get; private set; }

    /// <summary>
    /// The action that gets executed when the player clicks the button to perform this interaction.
    /// </summary>
    protected abstract void OnExecute();

    /// <summary>
    /// The validator function to check if the interaction is currently available.
    /// <br/>If unavailable, a reason will be returned as an out parameter.
    /// </summary>
    public bool CanExecute(out string unavailableReason)
    {
        unavailableReason = "";

        // Check resource cost
        foreach (var res in ResourceCost)
        {
            if (Game.Instance.Resources[res.Key] < res.Value)
            {
                unavailableReason = $"Not enough {res.Key.LabelPlural}";
                return false;
            }
        }

        // Check remaining uses
        if (NoRemainingUsesThisTurn)
        {
            string limit = Def.MaxUsesPerTurn == 1 ? "once" : $"{Def.MaxUsesPerTurn} times";
            unavailableReason = $"Can't be used more than {limit} per turn.";
            return false;
        }

        // Check custom checks
        if (!CanExecute_CustomChecks(out unavailableReason))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// May contain additional interaction-specific checks why this interaction may not be available.
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanExecute_CustomChecks(out string unavailableReason)
    {
        unavailableReason = "";
        return true;
    }

    /// <summary>
    /// How much resources need to be spent to perform this interaction.
    /// </summary>
    protected virtual Dictionary<ResourceDef, int> ResourceCost => Def.ResourceCost;

    public void Init(TileInteractionDef def, Tile tile, TileFeature feature, Meeple meeple, bool noUsesRemaining = false)
    {
        Def = def;
        Tile = tile;
        Feature = feature;
        Meeple = meeple;
        NoRemainingUsesThisTurn = noUsesRemaining;
    }

    public void Execute()
    {
        // Pay cost
        foreach(var res in ResourceCost)
        {
            Game.Instance.RemoveResource(res.Key, res.Value);
        }

        // Log usage
        if (Feature != null) Feature.InteractionsUsedThisTurn.Increment(Def);

        // Execute interaction
        OnExecute();

        // Show action prompts
        GameUI.Instance.TileInteractionMenu.Hide();
        Game.Instance.ShowNextActionPrompt();
    }

    public Dictionary<ResourceDef, int> GetResourceCosts()
    {
        // Base costs
        Dictionary<ResourceDef, int> resCosts = new Dictionary<ResourceDef, int>(Def.ResourceCost);

        // Rule modifiers
        foreach(Rule r in Game.Instance.Rulebook.ActiveRules)
        {
            Dictionary<ResourceDef, int> modifiers = null;
            if (r.GetTileInteractionCostModifiers().TryGetValue(Def, out modifiers))
            {
                resCosts.IncrementMultiple(modifiers);
            }
        }

        // Final result
        return resCosts;
    }

    public virtual string Label => Def.Label;
    public virtual string Description => Def.Description;

}
