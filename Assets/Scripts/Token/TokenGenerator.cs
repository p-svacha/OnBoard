using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TokenGenerator
{
    private static float MAX_SCALE_MODIFIER = 0.02f;

    public static Token GenerateTokenCopy(Token orig, bool randomModel = false, bool isStatic = false, bool hidden = true, bool frozen = false)
    {
        Token token = GenerateToken(orig.Shape, orig.Surfaces, orig.Size, randomModel ? -1 : orig.ModelId, isStatic, hidden, frozen);
        token.Original = orig;
        return token;
    }


    public static Token GenerateToken(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size, int modelId = -1, bool isStatic = false, bool hidden = true, bool frozen = false)
    {
        if (modelId == -1)
        {
            List<GameObject> prefabs = Resources.LoadAll<GameObject>($"Prefabs/Tokens/{shape.DefName}").ToList();
            modelId = Random.Range(1, prefabs.Count + 1);
        }
        string prefabPath = $"Prefabs/Tokens/{shape.DefName}/{shape.DefName}{modelId:00}";
        GameObject tokenPrefab = ResourceManager.LoadPrefab(prefabPath);
        GameObject tokenObject = GameObject.Instantiate(tokenPrefab);
        tokenObject.layer = WorldManager.Layer_Token;

        // Size
        float scale = size.Scale + Random.Range(-MAX_SCALE_MODIFIER, MAX_SCALE_MODIFIER);

        // Surfaces
        MeshRenderer renderer = tokenObject.GetComponent<MeshRenderer>();
        if (surfaces.Count != shape.NumSurfaces) throw new System.Exception($"The token shape {shape.DefName} requires {shape.NumSurfaces} surfaces, but {surfaces.Count} were provided.");
        for(int i = 0; i < surfaces.Count; i++)
        {
            renderer.materials[shape.SurfaceMaterialIndices[i]].color = surfaces[i].Color.Color;
        }

        // MeshCollider
        if (tokenObject.GetComponent<Collider>() == null)
        {
            MeshCollider col = tokenObject.AddComponent<MeshCollider>();
            col.convex = true;
        }

        // Rigidbody
        if (!isStatic)
        {
            Rigidbody rb = tokenObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        // Token component
        Token newToken = tokenObject.AddComponent<Token>();
        newToken.Init(shape, surfaces, size, modelId, scale);

        // Tooltip
        if (!isStatic)
        {
            TooltipTarget3D tooltipTarget = tokenObject.AddComponent<TooltipTarget3D>();
            tooltipTarget.Title = newToken.LabelCap;
            tooltipTarget.Text = newToken.Description;
        }
        
        if (hidden) newToken.Hide();
        if (frozen) newToken.Freeze();
        return newToken;
    }
}
