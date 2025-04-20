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
    public static int Layer_PreviewObject;

    // Hovered Objects
    public static GameObject HoveredObject { get; private set; }
    public static Meeple HoveredMeeple { get; private set; }
    public static Tile HoveredBoardTile { get; private set; }

    public static void Initialize()
    {
        Layer_Token = LayerMask.NameToLayer("Token");
        Layer_BoardTile = LayerMask.NameToLayer("BoardTile");
        Layer_Meeple = LayerMask.NameToLayer("Meeple");
        Layer_BoardSegment = LayerMask.NameToLayer("BoardSegment");
        Layer_PreviewObject = LayerMask.NameToLayer("PreviewObject");
    }

    public static void UpdateHoveredObjects()
    {
        GameObject newHoveredObject = null;
        Meeple newHoveredMeeple = null;
        Tile newHoveredBoardTile = null;

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
                newHoveredBoardTile = newHoveredObject.GetComponent<Tile>();
            }
        }

        HoveredObject = newHoveredObject;
        HoveredMeeple = newHoveredMeeple;
        HoveredBoardTile = newHoveredBoardTile;
    }
}
