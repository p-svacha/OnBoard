using MeshBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardGenerator
{
    public const float BOARD_SIZE = 500;
    public const float TILE_SIZE = 4f;
    public const float TILE_GAP = 3f;
    private const float CONNECT_LINE_WIDTH = 0.1f;

    private const float MAX_ANGLE_CHANGE = 50f;
    private const float T_SPLIT_CHANCE = 0.1f;
    private const float DEAD_END_CHANCE = 0.1f;

    private const float TOKEN_GIVER_CHANCE = 0.05f;
    private const float TOKEN_BIN_CHANCE = 0.05f;
    private const float SPIKES_CHANCE = 0.05f;

    // In generation
    private static Game Game;
    private static Board Board;
    private static BoardRegionDef RegionDef;
    private static BoardRegion CurrentRegion;
    private static List<Tile> CreatedTiles; // All tiles that have been created so far for a segment
    private static List<Tile> CurrentPathEnds; // All paths that are currently being expanded

    public static Board GenerateBoard(Game game)
    {
        Game = game;
        MeshBuilder boardBuilder = new MeshBuilder("Board");
        int submesh = boardBuilder.GetSubmesh(ResourceManager.LoadMaterial("Materials/Board"));
        boardBuilder.BuildPlane(submesh, new Vector3(-(BOARD_SIZE / 2f), 0f, -(BOARD_SIZE / 2f)), new Vector3(BOARD_SIZE, BOARD_SIZE));
        GameObject boardObject = boardBuilder.ApplyMesh();
        boardObject.layer = WorldManager.Layer_BoardSegment;
        boardObject.GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.45f, 0.1f);

        Board = boardObject.AddComponent<Board>();
        return Board;
    }

    public static void GenerateStartSegment()
    {
        List<BoardRegionDef> candidateDefs = new List<BoardRegionDef>(DefDatabase<BoardRegionDef>.AllDefs);
        BoardRegionDef chosenDef = candidateDefs.RandomElement();

        BoardRegion region = GenerateBoardRegion(chosenDef);
        region.Tiles[0].AddFeature(TileFeatureDefOf.Start);

        // Add region to board
        Board.AddRegion(region);

        // Animate
        TileBuildAnimator.AnimateRegionBuild(region, startTile: region.Tiles[0], dropHeight: 4f, dropDuration: 0.3f, startInterval: 0.1f);
    }

    public static void GenerateAndAttachRandomRegion(System.Action onAnimationComplete)
    {
        List<BoardRegionDef> candidateDefs = new List<BoardRegionDef>(DefDatabase<BoardRegionDef>.AllDefs);
        BoardRegionDef chosenDef = candidateDefs.RandomElement();

        BoardRegion region = GenerateBoardRegion(chosenDef);

        // Move region until it doesn't intersect current board
        float nudgeAngle = RandomAngle;
        float nudgeDistance = TILE_GAP * 0.25f;
        Vector3 nudgeOffset = new Vector3(Mathf.Sin(Mathf.Deg2Rad * nudgeAngle) * nudgeDistance, 0f, Mathf.Cos(Mathf.Deg2Rad * nudgeAngle) * nudgeDistance);
        Tile shortestConnection_existing;
        Tile shortestConnection_new;

        while (DoesIntersectBoard(region, out shortestConnection_existing, out shortestConnection_new))
        {
            region.transform.localPosition += nudgeOffset;
        }

        // Connect to board
        ConnectTilesBidirectional(shortestConnection_existing, shortestConnection_new);

        // Add region to board
        Board.AddRegion(region);

        // Animate
        TileBuildAnimator.AnimateRegionBuild(region, startTile: shortestConnection_new, dropHeight: 4f, dropDuration: 0.3f, startInterval: 0.1f, onComplete: onAnimationComplete);
    }

    public static BoardRegion GenerateBoardRegion(BoardRegionDef def)
    {
        GameObject regionObject = new GameObject(def.LabelCap);
        regionObject.transform.SetParent(Board.transform);
        CurrentRegion = (BoardRegion)regionObject.AddComponent(def.RegionClass);
        CurrentRegion.Init(def);

        // Save constraints for this segment
        RegionDef = def;

        // Reset generation lists
        CreatedTiles = new List<Tile>();
        CurrentPathEnds = new List<Tile>();

        // Create start tile
        Vector3 startPosition = new Vector3(0f, 0.05f, 0f);
        float startAngle = RandomAngle;
        Tile startTile = TileGenerator.GenerateTile(Game, CurrentRegion, startPosition, startAngle);
        CreatedTiles.Add(startTile);

        // Add initial path
        CurrentPathEnds.Add(startTile);

        // Keep expanding paths until none left
        while (CurrentPathEnds.Count > 0) ExpandRandomPath();

        // Add all created tiles to region
        foreach (Tile tile in CreatedTiles) CurrentRegion.AddTile(tile);

        return CurrentRegion;
    }

    private static float RandomAngle => Random.Range(0f, 360f);

    #region Graph

    private static void ExpandRandomPath()
    {
        // Choose random path
        Tile chosenPathEnd = CurrentPathEnds.RandomElement();
        CurrentPathEnds.Remove(chosenPathEnd);

        // Check dead end
        bool canBeDeadEnd = true;
        if (CreatedTiles.Count < RegionDef.MinTiles && CurrentPathEnds.Count == 0) canBeDeadEnd = false;
        bool mustBeDeadEnd = false;
        if (CreatedTiles.Count >= RegionDef.MaxTiles) mustBeDeadEnd = true;
        if (mustBeDeadEnd || (canBeDeadEnd && Random.value < DEAD_END_CHANCE))
        {
            return; // Dead end
        }

        // Check T split
        if (Random.value < T_SPLIT_CHANCE)
        {
            // Calculate split angle
            float splitAngle = 90f + Random.Range(-MAX_ANGLE_CHANGE, MAX_ANGLE_CHANGE);
            if (Random.value < 0.5f) splitAngle *= -1;
            float postSplitAngle = chosenPathEnd.ForwardAngle + splitAngle;

            // Create first tile after t split
            float xOffsetSplit = Mathf.Sin(postSplitAngle * Mathf.Deg2Rad) * TILE_GAP;
            float zOffsetSplit = Mathf.Cos(postSplitAngle * Mathf.Deg2Rad) * TILE_GAP;
            Vector3 tSplitPos = chosenPathEnd.WorldPosition + new Vector3(xOffsetSplit, 0f, zOffsetSplit);
            GenerateOrConnectTile(chosenPathEnd, tSplitPos, postSplitAngle);
        }

        // Continue path
        float angleChange = Random.Range(-MAX_ANGLE_CHANGE, MAX_ANGLE_CHANGE);
        float nextAngle = chosenPathEnd.ForwardAngle + angleChange;
        float xOffsetNext = Mathf.Sin(nextAngle * Mathf.Deg2Rad) * TILE_GAP;
        float zOffsetNext = Mathf.Cos(nextAngle * Mathf.Deg2Rad) * TILE_GAP;
        Vector3 nextPos = chosenPathEnd.WorldPosition + new Vector3(xOffsetNext, 0f, zOffsetNext);
        GenerateOrConnectTile(chosenPathEnd, nextPos, nextAngle);
    }

    /// <summary>
    /// Tries generating a tile that comes after and is connected to previousTile, at the given position with the given forward angle.
    /// <br/>If the new tile would be too close to another tile in this region, don't create it and just connect it to there instead.
    /// </summary>
    private static void GenerateOrConnectTile(Tile previousTile, Vector3 position, float angle)
    {
        // Check if new position is too close to an existing tile in this region
        foreach(Tile existingTiles in CreatedTiles)
        {
            float distance = Vector2.Distance(new Vector2(position.x, position.z), new Vector2(existingTiles.transform.position.x, existingTiles.transform.position.z));
            if(distance < TILE_GAP * 0.9f)
            {
                // New tile would be too close to existing tile - just connect to this tile and end path here
                ConnectTilesBidirectional(previousTile, existingTiles);
                return;
            }
        }

        // Generate new tile
        Tile newTile = TileGenerator.GenerateTile(Game, CurrentRegion, position, angle);
        AddRandomTileFeaturesTo(newTile);
        CreatedTiles.Add(newTile);
        CurrentPathEnds.Add(newTile);
        ConnectTilesBidirectional(previousTile, newTile);
    }

    private static void ConnectTilesBidirectional(Tile t1, Tile t2)
    {
        // Connect tiles in graph
        t1.Connect(t2);
        t2.Connect(t1);

        // Create a new GameObject to hold the LineRenderer
        GameObject lineObj = new GameObject("ConnectionLine");
        lineObj.transform.SetParent(CurrentRegion.transform, worldPositionStays: true);

        var lr = lineObj.AddComponent<LineRenderer>();

        // --- Configure the LineRenderer ---
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.SetPosition(0, t1.transform.position);
        lr.SetPosition(1, t2.transform.position);

        // Thin black line
        lr.startWidth = CONNECT_LINE_WIDTH;
        lr.endWidth = CONNECT_LINE_WIDTH;
        lr.startColor = Color.black;
        lr.endColor = Color.black;

        // Use an unlit color shader so it always shows as solid black
        var mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = Color.black;
        lr.material = mat;
    }

    #endregion

    #region Features

    private static void AddRandomTileFeaturesTo(Tile tile)
    {
        if (Random.value < TOKEN_GIVER_CHANCE) tile.AddSpecificTokenGiverFeature(TokenShapeDefOf.Pebble, new() { new(TokenColorDefOf.White) }, TokenSizeDefOf.Small);
        if (Random.value < TOKEN_BIN_CHANCE) tile.AddFeature(TileFeatureDefOf.TokenBin);
        if (Random.value < SPIKES_CHANCE) tile.AddFeature(TileFeatureDefOf.Spikes);
    }

    #endregion

    #region Region Placement

    private static bool DoesIntersectBoard(BoardRegion region, out Tile shortestConnection_existing, out Tile shortestConnection_new)
    {
        float shortestDistance = float.MaxValue;
        shortestConnection_existing = null;
        shortestConnection_new = null;

        foreach (Tile newTile in region.Tiles)
        {
            Vector2 newPos = new Vector2(newTile.transform.position.x, newTile.transform.position.z);
            foreach(Tile existingTile in Board.Tiles)
            {
                Vector2 existingPos = new Vector2(existingTile.transform.position.x, existingTile.transform.position.z);
                float distance = Vector2.Distance(newPos, existingPos);
                if(distance < shortestDistance)
                {
                    shortestDistance = distance;
                    shortestConnection_existing = existingTile;
                    shortestConnection_new = newTile;
                }

                if (distance <= TILE_GAP) return true;
            }
        }

        return false;
    }

    #endregion
}
