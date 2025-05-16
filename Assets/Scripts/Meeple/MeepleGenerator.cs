using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeepleGenerator
{
    public static NpcMeeple GenerateNpcMeeple(MeepleDef def)
    {
        GameObject prefab = ResourceManager.LoadPrefab($"Prefabs/Meeples/{def.DefName}");
        GameObject meepleObj = GameObject.Instantiate(prefab);
        NpcMeeple meeple = (NpcMeeple)meepleObj.AddComponent(def.MeepleClass);
        meeple.Init(def);
        InitMeepleObject(meepleObj, meeple);
        return meeple;
    }

    /// <summary>
    /// Sets the layer and adds a mesh collider and 3D tooltip to all children.
    /// </summary>
    private static void InitMeepleObject(GameObject obj, NpcMeeple meeple, bool includeRoot = true)
    {
        if (includeRoot)
        {
            obj.layer = WorldManager.Layer_Meeple;
            if (obj.GetComponent<MeshFilter>() != null)
            {
                obj.AddComponent<MeshCollider>();
                TooltipTarget3D tooltip = obj.AddComponent<TooltipTarget3D>();
                tooltip.Title = meeple.LabelCap;
                tooltip.Text = meeple.Description;
            }
        }
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            InitMeepleObject(obj.transform.GetChild(i).gameObject, meeple);
        }
    }
}
