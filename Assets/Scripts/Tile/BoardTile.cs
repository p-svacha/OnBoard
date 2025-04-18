using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public BoardSegment Segment;
    public BoardTileDef Def;
    public List<BoardTile> ConnectedTiles;

    // Visual
    protected const float TILE_SIZE = 4f;
    private GameObject MovementHighlightFx;

    public void Init(BoardSegment segment, BoardTileDef def)
    {
        Segment = segment;
        Def = def;
        ConnectedTiles = new List<BoardTile>();

        OnInit();
        InitVisuals();
    }

    public void Connect(BoardTile tile)
    {
        ConnectedTiles.Add(tile);
    }

    /// <summary>
    /// Gets execeuted when initializing the tile.
    /// </summary>
    protected virtual void OnInit() { }

    /// <summary>
    /// Function used when initializing a tile to make it visually more interesting.
    /// </summary>
    public virtual void InitVisuals() { }

    /// <summary>
    /// The effect that gets executed when landing on this board tile.
    /// </summary>
    public virtual void OnLand() { }

    /// <summary>
    /// The effect that gets executed when passing over this board tile.
    /// </summary>
    public virtual void OnPass() { }

    public void HighlightAsMovementOption()
    {
        GameObject fxPrefab = ResourceManager.LoadPrefab("Prefabs/TileFX/FloatingCircles");
        MovementHighlightFx = GameObject.Instantiate(fxPrefab, transform);
        MovementHighlightFx.transform.localPosition = Vector3.zero;
    }
    public void UnhighlightAsMovementOption()
    {
        if (MovementHighlightFx != null) GameObject.Destroy(MovementHighlightFx.gameObject);
    }
}
