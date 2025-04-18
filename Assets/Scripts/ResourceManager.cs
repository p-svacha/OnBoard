using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Class used to dynamically load materials from resources on demand. All materials are cached after the first load.
/// </summary>
public static class ResourceManager
{
    private static Dictionary<string, Material> CachedMaterials = new Dictionary<string, Material>();
    public static Material LoadMaterial(string resourcePath)
    {
        // cached
        if (CachedMaterials.TryGetValue(resourcePath, out Material mat)) return mat;

        // not yet cached
        Material newMat = Resources.Load<Material>(resourcePath);
        if (newMat == null) throw new System.Exception($"Failed to load material {resourcePath}.");
        CachedMaterials.Add(resourcePath, newMat);
        return newMat;
    }

    private static Dictionary<string, Texture2D> CachedTextures = new Dictionary<string, Texture2D>();
    public static Texture2D LoadTexture(string resourcePath)
    {
        // cached
        if (CachedTextures.TryGetValue(resourcePath, out Texture2D tex)) return tex;

        // not yet cached
        Texture2D newTex = Resources.Load<Texture2D>(resourcePath);
        if (newTex == null) throw new System.Exception($"Failed to load texture {resourcePath}.");
        CachedTextures.Add(resourcePath, newTex);
        return newTex;
    }

    private static Dictionary<string, GameObject> CachedPrefabs = new Dictionary<string, GameObject>();
    public static GameObject LoadPrefab(string resourcePath)
    {
        // cached
        if (CachedPrefabs.TryGetValue(resourcePath, out GameObject obj)) return obj;

        // not yet cached
        GameObject loadedPrefabs = Resources.Load<GameObject>(resourcePath);
        if (loadedPrefabs == null) throw new System.Exception($"Failed to load GameObject {resourcePath}.");
        CachedPrefabs.Add(resourcePath, loadedPrefabs);
        return loadedPrefabs;
    }

    public static void ClearCache()
    {
        CachedMaterials.Clear();
        CachedTextures.Clear();
        CachedPrefabs.Clear();
    }
}

