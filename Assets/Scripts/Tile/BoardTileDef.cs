using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTileDef : Def
{
    /// <summary>
    /// The class that will be instantiated when creating a board tile of this type.
    /// </summary>
    public Type BoardTileClass { get; init; } = typeof(BoardTile);
}
