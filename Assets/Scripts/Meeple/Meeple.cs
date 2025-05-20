using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple : MonoBehaviour
{
    public Tile Tile { get; private set; }

    public void SetTile(Tile tile)
    {
        if (Tile != null) Tile.RemoveMeeple(this);
        Tile = tile;
        if (Tile != null) Tile.AddMeeple(this);
    }

    /// <summary>
    /// Returns all possible actions a player meeple can do during the action phase when standing on a tile with this meeple.
    /// </summary>
    public virtual List<TileInteraction> GetInteractions()
    {
        List<TileInteraction> interactions = new List<TileInteraction>();
        return interactions;
    }

    protected TileInteraction CreateTileInteraction(TileInteractionDef def)
    {
        TileInteraction interaction = (TileInteraction)System.Activator.CreateInstance(def.InteractionClass);
        interaction.Init(def, Tile, null, this);
        return interaction;
    }
}
