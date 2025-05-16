using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meeple : MonoBehaviour
{
    public Tile Tile { get; private set; }

    public void SetPosition(Tile tile)
    {
        Tile = tile;
        transform.position = tile.transform.position;
    }
}
