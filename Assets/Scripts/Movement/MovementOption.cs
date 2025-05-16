using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores a possibility for how a meeple can be moved by the player.
/// </summary>
public class MovementOption
{
    /// <summary>
    /// The meeple that would move in this option.
    /// </summary>
    public Meeple Meeple { get; private set; }

    /// <summary>
    /// The tile on which the meeple stands before doing this movement.
    /// </summary>
    public Tile StartTile { get; private set; }

    /// <summary>
    /// The full path of the movement, excluding the tile where the meeple stands before the movement and excluding the tile where the meeple would land.
    /// <br/>These are the tiles, where OnPass gets triggered.
    /// </summary>
    public List<Tile> PassedTiles { get; private set; }

    /// <summary>
    /// The tile where the meeple would arrive when the movement ends. This is the only tile where OnLand gets triggered.
    /// </summary>
    public Tile TargetTile { get; private set; }

    /// <summary>
    /// The total length of this movement path, equal to the amount of movement points it costs.
    /// </summary>
    public int Length => PassedTiles.Count + 1;

    /// <summary>
    /// Flag if this is a movement of a player meeple.
    /// </summary>
    public bool IsPlayerMovement { get; private set; }

    public MovementOption(Meeple meeple, Tile startTile, List<Tile> passedTiles, Tile targetTile, bool isPlayerMovement)
    {
        Meeple = meeple;
        StartTile = startTile;
        PassedTiles = passedTiles;
        TargetTile = targetTile;
        IsPlayerMovement = isPlayerMovement;
    }
}
