using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Game Game { get; private set; }

    public List<TileFeature> Features;

    // Graph
    public Vector3 WorldPosition;
    public float ForwardAngle;
    public List<Tile> ConnectedTiles;

    // Visual
    public const float TILE_SIZE = BoardSegmentGenerator.TILE_SIZE;
    private GameObject MovementHighlightFx;

    public void Init(Game game, Vector3 worldPosition, float forwardAngle)
    {
        Game = game;
        ConnectedTiles = new List<Tile>();
        Features = new List<TileFeature>();

        WorldPosition = worldPosition;
        ForwardAngle = forwardAngle;
    }

    public void Connect(Tile tile)
    {
        ConnectedTiles.Add(tile);
    }

    /// <summary>
    /// Returns if meeples may always stop on this tile.
    /// </summary>
    public bool CanMeepleStopHere()
    {
        return Features.Any(f => f.CanMeepleStopHere());
    }

    /// <summary>
    /// The effect that gets executed when landing on this board tile.
    /// </summary>
    public void OnLand()
    {
        foreach (TileFeature feature in Features) feature.OnLand();
    }

    /// <summary>
    /// The effect that gets executed when passing over this board tile.
    /// </summary>
    public virtual void OnPass()
    {
        foreach (TileFeature feature in Features) feature.OnPass();
    }

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

    #region Features

    /// <summary>
    /// Add a feature of a specific def with random parameters.
    /// </summary>
    public TileFeature AddFeature(TileFeatureDef def)
    {
        TileFeature feature = TileGenerator.CreateTileFeature(this, def);
        feature.SetRandomParameters();
        feature.InitVisuals();
        Features.Add(feature);
        return feature;
    }

    public void RemoveFeature(TileFeature feature)
    {
        GameObject.Destroy(feature.gameObject);
        Features.Remove(feature);
    }

    /// <summary>
    /// Adds a feature that landing on this tile awards a specific token.
    /// </summary>
    public TileFeature_SpecificTokenGiver AddSpecificTokenGiverFeature(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size)
    {
        TileFeature_SpecificTokenGiver feature = TileGenerator.CreateTileFeature(this, TileFeatureDefOf.SpecificTokenGiver) as TileFeature_SpecificTokenGiver;
        feature.InitToken(shape, color, size);
        feature.InitVisuals();
        Features.Add(feature);
        return feature;
    }


    public TileFeature_RedFlag AddRedFlag(QuestGoal_ReachRedFlag goal)
    {
        TileFeature_RedFlag feature = TileGenerator.CreateTileFeature(this, TileFeatureDefOf.RedFlag) as TileFeature_RedFlag;
        feature.Init(goal);
        feature.InitVisuals();
        Features.Add(feature);
        return feature;
    }

    #endregion
}
