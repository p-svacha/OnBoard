using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    public Tile StartTile;

    /// <summary>
    /// The collecion of all board segments that make up the board.
    /// </summary>
    public List<BoardSegment> Segments;

    private void Awake()
    {
        Instance = this;
    }

    public void Init(Tile startTile)
    {
        StartTile = startTile;
    }

    public void AddSegment(BoardSegment segment)
    {
        Segments.Add(segment);
    }

    public List<Tile> GetAllTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (BoardSegment segment in Segments) tiles.AddRange(segment.Tiles);
        return tiles;
    }
    public Tile GetRandomTile()
    {
        return GetAllTiles().RandomElement();
    }
}
