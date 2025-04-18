using MeshBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardTileGenerator
{
    public static BoardTile GenerateTile(Game game, BoardSegment segment, BoardTileDef tileDef, Vector2 localPosition)
    {
        GameObject tilePrefab = ResourceManager.LoadPrefab("Prefabs/Tile");
        GameObject tileObject = GameObject.Instantiate(tilePrefab, segment.transform);
        tileObject.transform.localPosition = localPosition;
        var dynamicTileComponent = tileObject.AddComponent(tileDef.BoardTileClass);
        BoardTile tile = dynamicTileComponent as BoardTile;

        tile.Init(segment, tileDef);

        return tile;
    }
}
