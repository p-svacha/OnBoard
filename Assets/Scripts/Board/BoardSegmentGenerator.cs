using MeshBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardSegmentGenerator
{
    public static float BOARD_SEGMENT_SIZE = 50;
    public static float TILE_GAP = 3f;

    public static BoardSegment GenerateStartSegment(Game game, Board board, Vector2Int coordinates, int numTiles)
    {
        MeshBuilder boardBuilder = new MeshBuilder("BoardSegment");
        int submesh = boardBuilder.GetSubmesh(ResourceManager.LoadMaterial("Materials/Board"));
        boardBuilder.BuildPlane(submesh, new Vector3(-(BOARD_SEGMENT_SIZE / 2f), 0f, -(BOARD_SEGMENT_SIZE / 2f)), new Vector3(BOARD_SEGMENT_SIZE, BOARD_SEGMENT_SIZE));
        GameObject boardObject = boardBuilder.ApplyMesh();
        boardObject.transform.SetParent(board.transform);
        boardObject.layer = WorldManager.Layer_BoardSegment;
        boardObject.transform.position = new Vector3(coordinates.x * BOARD_SEGMENT_SIZE, 0f, coordinates.y * BOARD_SEGMENT_SIZE);
        boardObject.GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.45f, 0.1f);

        BoardSegment boardSegment = boardObject.AddComponent<BoardSegment>();
        boardSegment.Init(game, board);

        // Create tiles
        // Start on connection points and generate tiles from there
        Vector3 localPos = new Vector3((BOARD_SEGMENT_SIZE / 2f) - (TILE_GAP / 2f), 0.05f, Random.Range(-0.5f, 0.5f));
        Tile previousTile = null;
        for(int i = 0; i < numTiles; i++)
        {
            // Generate tile
            Tile tile = TileGenerator.GenerateTile(game, boardSegment, localPos);
            boardSegment.AddTile(tile);

            // Connect to previous
            if (i > 0)
            {
                tile.Connect(previousTile);
                previousTile.Connect(tile);
            }

            // Add features
            if (i == numTiles - 1)
            {
                tile.AddFeature(TileFeatureDefOf.Start);
            }
            if (i == (numTiles - 3))
            {
                tile.AddSpecificTokenGiverFeature(TokenShapeDefOf.Pebble, TokenColorDefOf.White, TokenSizeDefOf.Small);
            }

            // Calculate next pos
            Vector3 nextPos = localPos + new Vector3(-TILE_GAP, 0f, Random.Range(-0.5f, 0.5f));
            localPos = nextPos;

            previousTile = tile;
        }

        return boardSegment;
    }
}
