using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    /// <summary>
    /// Returns all possible movement paths starting from startPosition (excluding startPosition, including tile where meeple lands).  
    /// <br/>- Any path that uses up exactly movementPoints steps, or
    /// <br/>- Any shorter path that ends on a tile where CanMeepleStopHere() is true (only if allowStops = true)  
    /// <br/>Never immediately back‑tracks to prevPosition.
    /// </summary>
    public static List<List<Tile>> GetPaths(Tile position, Tile prevPosition, int movementPoints, bool allowStops = true)
    {
        var paths = new List<List<Tile>>();

        // No movement points → no valid paths
        if (movementPoints <= 0)
            return paths;

        foreach (Tile next in position.ConnectedTiles)
        {
            // don't immediately reverse
            if (prevPosition != null && next == prevPosition)
                continue;

            // 1) full‑length paths (use your last movement point here)
            if (movementPoints == 1)
            {
                paths.Add(new List<Tile> { next });
            }
            else
            {
                // 2) early‑stop paths: you can stop here any time if allowed
                if (allowStops && next.CanMeepleStopHere())
                {
                    paths.Add(new List<Tile> { next });
                }

                // 3) longer paths: spend one point and recurse
                var subPaths = GetPaths(next, position, movementPoints - 1);
                foreach (var sub in subPaths)
                {
                    var path = new List<Tile> { next };
                    path.AddRange(sub);
                    paths.Add(path);
                }
            }
        }

        return paths;
    }
}
