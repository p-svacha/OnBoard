using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    public List<Tile> Tiles = new List<Tile>();
    public Tile StartTile;

    private void Awake()
    {
        Instance = this;
    }

    public void Init(Tile startTile)
    {
        StartTile = startTile;
    }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    public Tile GetRandomTile()
    {
        return Tiles.RandomElement();
    }
}
