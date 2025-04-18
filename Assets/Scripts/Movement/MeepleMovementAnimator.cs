using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeepleMovementAnimator
{
    private const float HOP_DURATION = 0.5f;

    /// <summary>
    /// Animate a meeple along a MovementOption path with a smooth tilt fade‑in/out.
    /// </summary>
    public static void AnimateMove(MovementOption option, Action onComplete)
    {
        var meeple = option.Meeple;

        // Build route: start + passed + target
        var route = new List<Vector3> { option.StartTile.transform.position };
        foreach (var t in option.PassedTiles) route.Add(t.transform.position);
        route.Add(option.TargetTile.transform.position);

        // Tiles for logical updates
        var pathTiles = new List<BoardTile>(option.PassedTiles) { option.TargetTile };

        meeple.StartCoroutine(AnimateRoute(meeple, route, pathTiles, onComplete));
    }

    private static IEnumerator AnimateRoute(
        Meeple meeple,
        List<Vector3> route,
        List<BoardTile> pathTiles,
        Action onComplete)
    {
        var tf = meeple.transform;
        var upright = Quaternion.identity;

        for (int i = 0; i < route.Count - 1; i++)
        {
            // ensure perfectly upright at tile
            tf.rotation = upright;

            Vector3 startPos = route[i];
            Vector3 endPos = route[i + 1];

            // higher arc peak
            Vector3 mid = Vector3.Lerp(startPos, endPos, 0.5f);
            mid.y += UnityEngine.Random.Range(1.0f, 2.0f);

            // random tilt axis & max angle
            Vector3 axis = UnityEngine.Random.onUnitSphere;
            float maxDeg = UnityEngine.Random.Range(15f, 45f);

            float elapsed = 0f;
            while (elapsed < HOP_DURATION)
            {
                float u = elapsed / HOP_DURATION;
                float easeU = Mathf.SmoothStep(0f, 1f, u);

                // arc interpolation
                Vector3 a = Vector3.Lerp(startPos, mid, easeU);
                Vector3 b = Vector3.Lerp(mid, endPos, easeU);
                tf.position = Vector3.Lerp(a, b, easeU);

                // sinusoidal tilt factor: 0→1→0
                float tiltFactor = Mathf.Sin(easeU * Mathf.PI);
                float angle = maxDeg * tiltFactor;
                tf.rotation = Quaternion.AngleAxis(angle, axis);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // snap and reset
            tf.position = endPos;
            tf.rotation = upright;

            // logical update
            meeple.SetPosition(pathTiles[i]);
        }

        onComplete?.Invoke();
    }
}
