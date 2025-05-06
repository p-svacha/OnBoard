using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFeature_Altar : TileFeature
{
    public override void InitVisuals()
    {
        GameObject altarPrefab = ResourceManager.LoadPrefab("Prefabs/TileFeatures/Altar");
        GameObject altar = GameObject.Instantiate(altarPrefab, Tile.transform);
        altar.transform.rotation = Quaternion.Euler(0f, Tile.ForwardAngle, 0f);

        float offsetAngle = Tile.ForwardAngle + 90;
        float offsetDistance = Tile.TILE_RADIUS * 1.6f;
        float x = Mathf.Sin(Mathf.Deg2Rad * offsetAngle) * offsetDistance;
        float y = Mathf.Cos(Mathf.Deg2Rad * offsetAngle) * offsetDistance;
        altar.transform.localPosition = new Vector3(x, 0f, y);
    }
}
