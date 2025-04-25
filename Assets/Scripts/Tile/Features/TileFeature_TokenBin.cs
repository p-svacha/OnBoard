using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeature_TokenBin : TileFeature
{
    private static float CENTER_DISTANCE = Tile.TILE_SIZE * 0.2f;

    public override void InitVisuals()
    {
        GameObject binPrefab = ResourceManager.LoadPrefab("Prefabs/TileFeatures/Trashcan");
        GameObject bin = GameObject.Instantiate(binPrefab, transform);

        float angle = Random.Range(0f, 360f);
        bin.transform.localPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * CENTER_DISTANCE, 0f, Mathf.Cos(angle * Mathf.Deg2Rad) * CENTER_DISTANCE);
    }

    public override void OnLand()
    {
        Game.Instance.QueueActionPrmpt(new ActionPrompt_DraftTokenToDiscard());
    }
}
