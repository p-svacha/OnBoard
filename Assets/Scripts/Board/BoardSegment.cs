using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSegment : MonoBehaviour
{
    public Game Game;
    public Board Board;
    public List<Tile> Tiles;

    public void Init(Game game, Board board)
    {
        Board = board;
        Game = game;
        Tiles = new List<Tile>();
    }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    internal Tile Last()
    {
        throw new NotImplementedException();
    }
}
