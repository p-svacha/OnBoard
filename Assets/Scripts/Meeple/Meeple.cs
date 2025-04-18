using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple : MonoBehaviour
{
    public Game Game { get; private set; }
    public BoardTile Tile { get; private set; }

    public void Init(Game game)
    {
        Game = game;
    }

    public void SetPosition(BoardTile tile)
    {
        Tile = tile;
        transform.position = tile.transform.position;
    }
}
