using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    public List<Tile> Tiles = new List<Tile>();
    public List<BoardRegion> Regions = new List<BoardRegion>();
    public Tile StartTile;

    private void Awake()
    {
        Instance = this;
    }

    public void Init(Tile startTile)
    {
        StartTile = startTile;
    }

    public void AddRegion(BoardRegion region)
    {
        Regions.Add(region);
        Tiles.AddRange(region.Tiles);
    }

    public Tile GetRandomTile()
    {
        return Tiles.RandomElement();
    }
}
