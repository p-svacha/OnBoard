using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenPhysicsManager
{
    private static List<Token> ThrownTokens;

    public static void Initialize()
    {
        ThrownTokens = new List<Token>();
    }

    #region Throw

    public static void ThrowInitialTokens(Game game)
    {
        List<Token> tokensToThrow = new List<Token>(game.CurrentDraw.TableTokens.Keys);
        foreach (Token token in tokensToThrow)
        {
            ThrowToken(token);
        }
    }

    public static void ThrowToken(Token token)
    {
        Token copy = TokenGenerator.GenerateTokenCopy(token);
        ThrownTokens.Add(copy);

        // spawn
        copy.transform.position = GetRandomThrowSpawnPosition();
        copy.transform.rotation = Random.rotation;
        copy.Show();

        ApplySpawnImpulses(copy.Rigidbody);

        // watch until it comes to rest
        copy.StartCoroutine(WatchForLanding(copy));
    }

    private static IEnumerator WatchForLanding(Token copy)
    {
        // make sure gravity is on
        copy.Rigidbody.useGravity = true;

        // give the physics one tick
        yield return new WaitForFixedUpdate();

        // now watch until it really settles...
        while (true)
        {
            // 1) if it fell through the floor, rethrow it
            if (copy.transform.position.y < -10f)
            {
                Rethrow(copy);
                // wait a frame for the new impulse
                yield return new WaitForFixedUpdate();
                continue;
            }

            // 2) if it's still moving or spinning, keep waiting
            if (copy.Rigidbody.velocity.sqrMagnitude > 0.01f ||
                copy.Rigidbody.angularVelocity.sqrMagnitude > 0.01f)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            // 3) settled! break out
            break;
        }

        // finally, assign its rolled face
        AssignRolledSurface(copy);
    }

    private static void Rethrow(Token copy)
    {
        // teleport back up
        copy.transform.position = GetRandomThrowSpawnPosition();
        copy.transform.rotation = Random.rotation;

        var rb = copy.Rigidbody;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        ApplySpawnImpulses(rb);
    }

    private static void ApplySpawnImpulses(Rigidbody rb)
    {
        // apply linear impulse
        Vector3 forceDir = GetRandomThrowForceDirection();
        rb.AddForce(forceDir * GetRandomThrowForce(), ForceMode.Impulse);

        // apply rotational impulse
        float torque = Random.Range(350f, 650f);
        rb.AddTorque(Random.onUnitSphere * torque, ForceMode.Impulse);
    }

    private static void AssignRolledSurface(Token copy)
    {
        // transform world up into token‐local space:
        Vector3 localUp = copy.transform.InverseTransformDirection(Vector3.up).normalized;

        // find the index whose normal is closest to that direction
        int bestIndex = 0;
        float bestDot = -1f;
        for (int i = 0; i < copy.Surfaces.Count; i++)
        {
            float dot = Vector3.Dot(localUp, copy.Surfaces[i].NormalDirection.normalized);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestIndex = i;
            }
        }

        Debug.Log($"Token {copy.Label} has landed: The surface is {copy.Surfaces[bestIndex].Label}.");

        // write it into the current draw result
        Game.Instance.SetRolledTokenSurface(copy, bestIndex);
    }

    private static Vector3 GetRandomThrowSpawnPosition()
    {
        float backDistance = 4f; // Distance tokens are thrown towards behind currently looked at position
        float rightDistance = 4f; // Distance tokens are thrown towards to the right of currently looked at position

        float minHeight = 4f; // Minimum height the tokens are from thrown into from
        float maxHeight = 6f; // Maximum height the tokens are from thrown into from
        float maxOffset = 2f; // Maximum random horizontal offset applied to position

        float angleRad = Mathf.Deg2Rad * CameraHandler.Instance.CurrentAngle;
        Vector3 frontOffset = new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad)) * backDistance;
        Vector3 rightOffset = new Vector3(-Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * rightDistance;

        Vector3 spawnPos = CameraHandler.Instance.CurrentPosition + (-frontOffset) + rightOffset;
        spawnPos += new Vector3(Random.Range(-maxOffset, maxOffset), Random.Range(minHeight, maxHeight), Random.Range(-maxOffset, maxOffset));
        return spawnPos;
    }

    private static Vector3 GetRandomThrowForceDirection()
    {
        float maxRandomOffset = 0.02f; // Maximum random offset applied to each dimension in the direction

        float angleRad = Mathf.Deg2Rad * CameraHandler.Instance.CurrentAngle;
        Vector3 rightDir = new Vector3(-Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
        return -rightDir + new Vector3(Random.Range(-maxRandomOffset, maxRandomOffset), Random.Range(-maxRandomOffset, maxRandomOffset), Random.Range(-maxRandomOffset, maxRandomOffset));
    }

    private static float GetRandomThrowForce()
    {
        return Random.Range(4.5f, 6.5f);
    }

    #endregion

    #region Collect

    public static IEnumerator CollectTokens(Game game)
    {
        List<Token> collectedTokens = new List<Token>(ThrownTokens);

        // 1) disable physics & start each lift+implode
        foreach (Token token in collectedTokens)
        {
            game.StartCoroutine(LiftAndImplode(token));
        }

        // 2) wait for all to finish: max lift (1.2s) + max implode (0.3s) + buffer
        yield return new WaitForSeconds(1.6f);

        foreach (Token token in collectedTokens)
        {
            ThrownTokens.Remove(token);
            token.DestroySelf();
        }
    }

    public static IEnumerator LiftAndImplode(Token token)
    {
        token.Freeze();

        // --- Lift phase ---
        Vector3 startPos = token.transform.position;
        Vector3 bagOffset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0f,
            Random.Range(-0.5f, 0.5f)
        );
        Vector3 endPos = startPos
                         + Vector3.up * Random.Range(3f, 5f)
                         + bagOffset;

        float liftDuration = Random.Range(0.8f, 1.2f);
        float elapsed = 0f;
        while (elapsed < liftDuration)
        {
            float t = elapsed / liftDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);
            token.transform.position = Vector3.Lerp(startPos, endPos, ease);
            elapsed += Time.deltaTime;
            yield return null;
        }
        token.transform.position = endPos;

        // --- Implode (scale-down) phase ---
        float implodeDuration = 0.3f;
        elapsed = 0f;
        Vector3 startScale = token.transform.localScale;
        while (elapsed < implodeDuration)
        {
            float t2 = elapsed / implodeDuration;
            float ease2 = Mathf.SmoothStep(0f, 1f, t2);
            token.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, ease2);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hide
        token.Hide();
    }

    #endregion
}
