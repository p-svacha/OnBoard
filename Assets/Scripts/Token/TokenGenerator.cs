using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TokenGenerator
{
    private static int MAX_PEBBLE_ID = 7;
    private static float MAX_SCALE_MODIFIER = 0.03f;

    public static Token GenerateTokenCopy(Token token, bool randomModel = false, bool hidden = true, bool frozen = false)
    {
        return GenerateToken(token.Shape, token.Color, token.Size, randomModel ? -1 : token.ModelId, hidden, frozen);
    }


    public static Token GenerateToken(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size, int modelId = -1, bool hidden = true, bool frozen = false)
    {
        if (modelId == -1) modelId = Random.Range(1, MAX_PEBBLE_ID + 1);
        string prefabPath = $"Prefabs/Tokens/Pebble{modelId:00}";
        GameObject tokenPrefab = ResourceManager.LoadPrefab(prefabPath);
        GameObject tokenObject = GameObject.Instantiate(tokenPrefab);
        float scale = size.Scale + Random.Range(-MAX_SCALE_MODIFIER, MAX_SCALE_MODIFIER);
        tokenObject.GetComponent<MeshRenderer>().material.color = color.Color;
        Token newToken = tokenObject.AddComponent<Token>();
        newToken.Init(shape, color, size, modelId, scale);

        // Tooltip
        TooltipTarget3D tooltipTarget = tokenObject.AddComponent<TooltipTarget3D>();
        tooltipTarget.Title = newToken.LabelCap;
        tooltipTarget.Text = newToken.Description;

        if (hidden) newToken.Hide();
        if (frozen) newToken.Freeze();

        return newToken;
    }
}
