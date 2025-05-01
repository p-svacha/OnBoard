using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoardRegion : MonoBehaviour
{
    public BoardRegionDef Def { get; private set; }
    public List<Tile> Tiles = new List<Tile>();

    public void Init(BoardRegionDef def)
    {
        Def = def;
        OnInit();
    }
    protected virtual void OnInit() { }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

}
