using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenGenerator
{
    private static int MAX_PEBBLE_ID = 7;
    private static float MAX_SCALE_MODIFIER = 0.03f;

    public static Token GenerateToken(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size, bool hidden = true)
    {
        int shapeId = Random.Range(1, MAX_PEBBLE_ID + 1);
        string prefabPath = $"Prefabs/Tokens/Pebble{shapeId:00}";
        GameObject tokenPrefab = ResourceManager.LoadPrefab(prefabPath);
        GameObject tokenObject = GameObject.Instantiate(tokenPrefab);
        float scale = size.Scale + Random.Range(-MAX_SCALE_MODIFIER, MAX_SCALE_MODIFIER);
        tokenObject.GetComponent<MeshRenderer>().material.color = color.Color;
        Token newToken = tokenObject.AddComponent<Token>();
        newToken.Init(shape, color, size, scale);

        if(hidden) newToken.Hide();

        return newToken;
    }
}
