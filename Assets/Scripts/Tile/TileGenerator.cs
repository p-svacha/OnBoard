using MeshBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileGenerator
{
    public static Tile GenerateTile(Game game, Board board, Vector3 localPosition, float forwardAngle)
    {
        GameObject tilePrefab = ResourceManager.LoadPrefab("Prefabs/Tile");
        GameObject tileObject = GameObject.Instantiate(tilePrefab, board.transform);
        tileObject.layer = WorldManager.Layer_BoardTile;
        tileObject.transform.localPosition = localPosition;
        Tile tile = tileObject.AddComponent<Tile>();

        tile.Init(game, localPosition, forwardAngle);

        return tile;
    }


    /// <summary>
    /// Creates a new game object on the tile with the feature component of the specified def attached and initialized.
    /// </summary>
    public static TileFeature CreateTileFeature(Tile tile, TileFeatureDef def)
    {
        GameObject featureObject = new GameObject(def.DefName);
        featureObject.transform.SetParent(tile.transform);
        featureObject.transform.localPosition = Vector3.zero;

        var component = featureObject.AddComponent(def.TileFeatureClass) as TileFeature;
        if (component == null)
        {
            Debug.LogError($"Failed to create TileFeature: {def.TileFeatureClass} is not a TileFeature.");
            GameObject.Destroy(featureObject);
            return null;
        }

        component.Init(tile, def);
        return component;
    }
}
