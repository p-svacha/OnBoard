using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeature_Spikes : TileFeature
{
    public override void InitVisuals()
    {
        GameObject prefab = ResourceManager.LoadPrefab("Prefabs/TileFeatures/Spikes");
        GameObject obj = GameObject.Instantiate(prefab, transform);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    public override void OnLand()
    {
        Game.Instance.TakeDamage(1, new() { DamageTag.Spike });
    }
}
