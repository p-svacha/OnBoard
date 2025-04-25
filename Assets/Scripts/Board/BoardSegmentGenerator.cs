using MeshBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardSegmentGenerator
{
    public const float BOARD_SIZE = 500;
    public const float TILE_SIZE = 4f;
    public const float TILE_GAP = 3f;
    private const float CONNECT_LINE_WIDTH = 0.1f;

    private const float MAX_ANGLE_CHANGE = 20f;
    private const float T_SPLIT_CHANCE = 0.1f;
    private const float DEAD_END_CHANCE = 0.1f;

    private const float TOKEN_GIVER_CHANCE = 0.05f;
    private const float TOKEN_BIN_CHANCE = 0.05f;

    // In generation
    private static Game Game;
    private static Board Board;
    private static int MinTiles;
    private static int MaxTiles;
    private static List<Tile> CreatedTiles; // All tiles that have been created so far for a segment
    private static List<Tile> CurrentPathEnds; // All paths that are currently being expanded

    public static Board GenerateBoard(Game game)
    {
        MeshBuilder boardBuilder = new MeshBuilder("Board");
        int submesh = boardBuilder.GetSubmesh(ResourceManager.LoadMaterial("Materials/Board"));
        boardBuilder.BuildPlane(submesh, new Vector3(-(BOARD_SIZE / 2f), 0f, -(BOARD_SIZE / 2f)), new Vector3(BOARD_SIZE, BOARD_SIZE));
        GameObject boardObject = boardBuilder.ApplyMesh();
        boardObject.layer = WorldManager.Layer_BoardSegment;
        boardObject.GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.45f, 0.1f);

        Board board = boardObject.AddComponent<Board>();
        return board;
    }

    public static void GenerateStartSegment(Game game, Board board, int minTiles, int maxTiles)
    {
        // Save constraints for this segment
        MinTiles = minTiles;
        MaxTiles = maxTiles;
        Game = game;
        Board = board;

        // Reset generation lists
        CreatedTiles = new List<Tile>();
        CurrentPathEnds = new List<Tile>();

        // Create start tile
        Vector3 startPosition = new Vector3(0f, 0.05f, 0f);
        float startAngle = HelperFunctions.GetDirectionAngle(Direction.E);
        Tile startTile = TileGenerator.GenerateTile(game, Board, startPosition, startAngle);
        startTile.AddFeature(TileFeatureDefOf.Start);
        CreatedTiles.Add(startTile);

        // Add initial path
        CurrentPathEnds.Add(startTile);

        // Keep expanding paths until none left
        while (CurrentPathEnds.Count > 0) ExpandRandomPath();

        // Add all created tiles to segment
        foreach (Tile tile in CreatedTiles) Board.AddTile(tile);
    }

    private static void ExpandRandomPath()
    {
        // Choose random path
        Tile chosenPathEnd = CurrentPathEnds.RandomElement();
        CurrentPathEnds.Remove(chosenPathEnd);

        // Check dead end
        bool canBeDeadEnd = true;
        if (CreatedTiles.Count < MinTiles && CurrentPathEnds.Count == 0) canBeDeadEnd = false;
        bool mustBeDeadEnd = false;
        if (CreatedTiles.Count >= MaxTiles) mustBeDeadEnd = true;
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
            Tile tSplitTile = TileGenerator.GenerateTile(Game, Board, tSplitPos, postSplitAngle);
            AddRandomTileFeaturesTo(tSplitTile);
            CreatedTiles.Add(tSplitTile);
            CurrentPathEnds.Add(tSplitTile);

            // Connect
            ConnectTilesBidirectional(chosenPathEnd, tSplitTile);
        }

        // Continue path
        float angleChange = Random.Range(-MAX_ANGLE_CHANGE, MAX_ANGLE_CHANGE);
        float nextAngle = chosenPathEnd.ForwardAngle + angleChange;
        float xOffsetNext = Mathf.Sin(nextAngle * Mathf.Deg2Rad) * TILE_GAP;
        float zOffsetNext = Mathf.Cos(nextAngle * Mathf.Deg2Rad) * TILE_GAP;
        Vector3 nextPos = chosenPathEnd.WorldPosition + new Vector3(xOffsetNext, 0f, zOffsetNext);
        Tile nextTile = TileGenerator.GenerateTile(Game, Board, nextPos, nextAngle);
        AddRandomTileFeaturesTo(nextTile);
        CreatedTiles.Add(nextTile);
        CurrentPathEnds.Add(nextTile);

        // Connect
        ConnectTilesBidirectional(chosenPathEnd, nextTile);
    }

    private static void AddRandomTileFeaturesTo(Tile tile)
    {
        if(Random.value < TOKEN_GIVER_CHANCE)
        {
            tile.AddSpecificTokenGiverFeature(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
        }
        if(Random.value < TOKEN_BIN_CHANCE)
        {
            tile.AddFeature(TileFeatureDefOf.TokenBin);
        }
    }

    private static void ConnectTilesBidirectional(Tile t1, Tile t2)
    {
        // Connect tiles in graph
        t1.Connect(t2);
        t2.Connect(t1);

        // Create a new GameObject to hold the LineRenderer
        GameObject lineObj = new GameObject("ConnectionLine");
        lineObj.transform.SetParent(Board.transform, worldPositionStays: true);

        var lr = lineObj.AddComponent<LineRenderer>();

        // --- Configure the LineRenderer ---
        lr.useWorldSpace = true;
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
}
