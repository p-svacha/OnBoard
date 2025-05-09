using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A tile interaction represents a possible action a player can do during the movement phase while a meeple stands on the tile.
/// </summary>
public class TileInteraction
{
    /// <summary>
    /// The action that gets executed when the player clicks the button to perform this interaction.
    /// </summary>
    public Action OnExecute;

    /// <summary>
    /// The validator function to check if the interaction is currently available.
    /// <br/>If available, this function will return an empty string.
    /// <br/>If not, this function will return the reason of why it is invalid, which will be shown in the tooltip.
    /// </summary>
    public Func<string> Validator;

    /// <summary>
    /// Returns if this tile interaction can performed.
    /// </summary>
    public bool IsAvailable => Validator() == "";

    /// <summary>
    /// The tile this interaction belongs to.
    /// </summary>
    public Tile Tile { get; private set; }

    /// <summary>
    /// The tile feature this interaction belongs to. May be null.
    /// </summary>
    public TileFeature Feature { get; private set; }

    public string Label { get; private set; }
    public string Description { get; private set; }

    public TileInteraction(Action onExecute, Func<string> validator, Tile tile, TileFeature feature, string label, string description)
    {
        OnExecute = onExecute;
        Validator = validator;

        Tile = tile;
        Feature = feature;
        Label = label;
        Description = description;
    }

    public void Execute()
    {
        OnExecute();

        GameUI.Instance.TileInteractionMenu.Hide();
        Game.Instance.ShowNextActionPrompt();
    }

}
