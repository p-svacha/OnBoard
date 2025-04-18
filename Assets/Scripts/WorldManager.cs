using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for identifying hovered objects.
/// </summary>
public static class WorldManager
{
    // Layers
    public static int Layer_Token;
    public static int Layer_BoardTile;
    public static int Layer_Meeple;
    public static int Layer_BoardSegment;

    // Hovered Objects
    public static GameObject HoveredObject { get; private set; }
    public static Meeple HoveredMeeple { get; private set; }
    public static BoardTile HoveredBoardTile { get; private set; }

    public static void Initialize()
    {
        Layer_Token = LayerMask.NameToLayer("Token");
        Layer_BoardTile = LayerMask.NameToLayer("BoardTile");
        Layer_Meeple = LayerMask.NameToLayer("Meeple");
        Layer_BoardSegment = LayerMask.NameToLayer("BoardSegment");
    }

    public static void UpdateHoveredObjects()
    {
        GameObject newHoveredObject = null;
        Meeple newHoveredMeeple = null;
        BoardTile newHoveredBoardTile = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            newHoveredObject = hit.collider.gameObject;

            if(newHoveredObject.layer == Layer_Meeple)
            {
                newHoveredMeeple = newHoveredObject.GetComponent<Meeple>();
            }
            if (newHoveredObject.layer == Layer_BoardTile)
            {
                newHoveredBoardTile = newHoveredObject.GetComponent<BoardTile>();
            }
        }

        HoveredObject = newHoveredObject;
        HoveredMeeple = newHoveredMeeple;
        HoveredBoardTile = newHoveredBoardTile;
    }
}
