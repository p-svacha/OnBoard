using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoardTileDefs
{
    public static List<BoardTileDef> Defs => new List<BoardTileDef>()
    {
        new BoardTileDef()
        {
            DefName = "EmptyTile",
            Label = "empty tile",
            Description = "A boring tile where nothing ever happens."
        },

        new BoardTileDef()
        {
            DefName = "TokenTile",
            Label = "token tile",
            Description = "A tile that adds a specific kind of token to your bag.",
            BoardTileClass = typeof(BoardTile_TokenTile),
        }
    };
}
