using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileBuildAnimator
{
    class Connection
    {
        public LineRenderer lr;
        public Tile a, b;
        public bool shown;
    }

    /// <summary>
    /// Animate a region’s tiles falling into place in BFS order from startTile.
    /// Connections only appear once both ends have landed.
    /// </summary>
    /// <param name="region">The region whose Tiles and Lines to animate.</param>
    /// <param name="startTile">Where the drop animation begins (BFS root). If null, uses region.Tiles[0].</param>
    /// <param name="dropHeight">How far above their final spot tiles start.</param>
    /// <param name="dropDuration">How long each tile’s drop takes.</param>
    /// <param name="startInterval">How many seconds between starting each tile’s drop.</param>
    public static void AnimateRegionBuild(
        BoardRegion region,
        Tile startTile = null,
        float dropHeight = 2f,
        float dropDuration = 0.3f,
        float startInterval = 0.05f,
        System.Action onComplete = null)
    {
        if (startTile == null && region.Tiles.Count > 0)
            startTile = region.Tiles[0];

        region.StartCoroutine(AnimateRegionCoroutine(
            region, startTile, dropHeight, dropDuration, startInterval, onComplete
        ));
    }

    private static IEnumerator AnimateRegionCoroutine(
        BoardRegion region,
        Tile startTile,
        float dropHeight,
        float dropDuration,
        float startInterval,
       System.Action onComplete)
    {
        var tiles = region.Tiles;

        // 1) Hide all tiles
        foreach (var tile in tiles)
            tile.gameObject.SetActive(false);

        // 2) Gather all connection LineRenderers under this region
        var allLRs = region.GetComponentsInChildren<LineRenderer>(true);
        var connections = new List<Connection>();
        foreach (var lr in allLRs)
        {
            // assume every LineRenderer child is a connection
            // find its two endpoint tiles by matching positions
            Vector3 p0 = lr.useWorldSpace
                ? lr.GetPosition(0)
                : lr.transform.TransformPoint(lr.GetPosition(0));
            Vector3 p1 = lr.useWorldSpace
                ? lr.GetPosition(1)
                : lr.transform.TransformPoint(lr.GetPosition(1));

            Tile tileA = null, tileB = null;
            foreach (var t in tiles)
            {
                if (tileA == null && Vector3.Distance(t.transform.position, p0) < 0.01f) tileA = t;
                if (tileB == null && Vector3.Distance(t.transform.position, p1) < 0.01f) tileB = t;
                if (tileA != null && tileB != null) break;
            }
            if (tileA != null && tileB != null)
            {
                lr.enabled = false;
                connections.Add(new Connection { lr = lr, a = tileA, b = tileB, shown = false });
            }
        }

        // 3) Build BFS order
        var ordered = new List<Tile>();
        var queue = new Queue<Tile>();
        var seen = new HashSet<Tile>();
        queue.Enqueue(startTile);
        seen.Add(startTile);
        while (queue.Count > 0)
        {
            var t = queue.Dequeue();
            ordered.Add(t);
            foreach (var n in t.ConnectedTiles)
            {
                if (!region.Tiles.Contains(n))
                {
                    continue;
                }
                if (!seen.Contains(n))
                {
                    seen.Add(n);
                    queue.Enqueue(n);
                }
            }
        }

        // 4) Kick off each tile’s drop at staggered times
        var dropped = new HashSet<Tile>();
        for (int i = 0; i < ordered.Count; i++)
        {
            var tile = ordered[i];
            float delay = i * startInterval;
            region.StartCoroutine(HandleTileDrop(
                tile, dropHeight, dropDuration, delay, dropped, connections
            ));
        }

        // 5) Wait until the very last tile has finished dropping
        float totalTime = (ordered.Count - 1) * startInterval + dropDuration;
        yield return new WaitForSeconds(totalTime);

        // 6) Invoke the callback if provided
        onComplete?.Invoke();
    }

    private static IEnumerator HandleTileDrop(
        Tile tile,
        float dropHeight,
        float dropDuration,
        float delay,
        HashSet<Tile> dropped,
        List<Connection> connections)
    {
        yield return new WaitForSeconds(delay);

        var go = tile.gameObject;
        Vector3 end = go.transform.position;
        Vector3 start = end + Vector3.up * dropHeight;

        go.transform.position = start;
        go.SetActive(true);

        // drop
        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            float u = elapsed / dropDuration;
            float eased = Mathf.SmoothStep(0f, 1f, u);
            go.transform.position = Vector3.Lerp(start, end, eased);

            elapsed += Time.deltaTime;
            yield return null;
        }
        go.transform.position = end;

        // mark as dropped
        dropped.Add(tile);

        // reveal any connections now that both ends are down
        foreach (var conn in connections)
        {
            if (!conn.shown
             && dropped.Contains(conn.a)
             && dropped.Contains(conn.b))
            {
                conn.lr.enabled = true;
                conn.shown = true;
            }
        }
    }
}
