using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSegment : MonoBehaviour
{
    public Game Game;
    public List<BoardTile> Tiles;

    public void Init(Game game)
    {
        Game = game;
        Tiles = new List<BoardTile>();
    }

    public void AddTile(BoardTile tile)
    {
        Tiles.Add(tile);
    }

    internal BoardTile Last()
    {
        throw new NotImplementedException();
    }
}
