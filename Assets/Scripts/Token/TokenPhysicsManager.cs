using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenPhysicsManager
{
    private static List<Token> ThrownTokens;

    public static void ThrowTokens(Game game)
    {
        if (ThrownTokens == null) ThrownTokens = new List<Token>();

        foreach (Token token in game.CurrentDrawResult.DrawnTokens)
        {
            Token copy = TokenGenerator.GenerateTokenCopy(token);
            copy.transform.position = GetRandomThrowSpawnPosition();
            copy.transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
            copy.Show();
            copy.Rigidbody.AddForce(GetRandomThrowForceDirection() * GetRandomThrowForce(), ForceMode.Impulse);
            ThrownTokens.Add(copy);
        }
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

    public static IEnumerator CollectTokens(Game game)
    {
        List<Token> collectedTokens = new List<Token>(ThrownTokens);

        // 1) disable physics & start each lift+implode
        foreach (Token token in collectedTokens)
        {
            var rb = token.Rigidbody;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            var col = token.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            game.StartCoroutine(LiftAndImplode(token));
        }

        // 2) wait for all to finish: max lift (1.2s) + max implode (0.3s) + buffer
        yield return new WaitForSeconds(1.6f);

        foreach (Token token in collectedTokens) ThrownTokens.Remove(token);
    }

    private static IEnumerator LiftAndImplode(Token token)
    {
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

        // Hide visually
        token.Hide();

        // Reset for next throw
        var rb = token.Rigidbody;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        var col = token.GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }
}
