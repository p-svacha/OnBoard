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
    public Meeple Meeple;

    /// <summary>
    /// The tile on which the meeple stands before doing this movement.
    /// </summary>
    public Tile StartTile;

    /// <summary>
    /// The full path of the movement, excluding the tile where the meeple stands before the movement and excluding the tile where the meeple would land.
    /// <br/>These are the tiles, where OnPass gets triggered.
    /// </summary>
    public List<Tile> PassedTiles;

    /// <summary>
    /// The tile where the meeple would arrive when the movement ends. This is the only tile where OnLand gets triggered.
    /// </summary>
    public Tile TargetTile;

    public MovementOption(Meeple meeple, Tile startTile, List<Tile> passedTiles, Tile targetTile)
    {
        Meeple = meeple;
        StartTile = startTile;
        PassedTiles = passedTiles;
        TargetTile = targetTile;
    }
}
