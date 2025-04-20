using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeature_Start : TileFeature
{
    public override void InitVisuals()
    {
        GameObject startTextPrefab = ResourceManager.LoadPrefab("Prefabs/TileFeatures/StartText");
        GameObject.Instantiate(startTextPrefab, transform);
    }
}
