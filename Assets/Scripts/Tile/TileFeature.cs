using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tile feature is a component on a board tile containing logic on how the tile behaves.
/// </summary>
public abstract class TileFeature : MonoBehaviour
{
    public Tile Tile { get; private set; }
    public TileFeatureDef Def { get; private set; }

    /// <summary>
    /// Use this to initialize the feature on a tile.
    /// </summary>
    public void Init(Tile tile, TileFeatureDef def)
    {
        Tile = tile;
        Def = def;
    }

    /// <summary>
    /// Gets executed when initializing the feature with unspecified parameters.
    /// </summary>
    public virtual void SetRandomParameters() { }

    /// <summary>
    /// Function used when initializing a feature to make it visually more interesting.
    /// </summary>
    public void RefreshVisuals()
    {
        HelperFunctions.DestroyAllChildredImmediately(gameObject);
        OnInitVisuals();
        AddCollidersAndTooltipToChildren(gameObject);
    }

    protected virtual void OnInitVisuals() { }

    /// <summary>
    /// The effect that gets executed when landing on this feature.
    /// </summary>
    public virtual void OnLand() { }

    /// <summary>
    /// The effect that gets executed when passing over this feature.
    /// </summary>
    public virtual void OnPass() { }

    /// <summary>
    /// Remove this feature from the tile and the game.
    /// </summary>
    public void Remove()
    {
        Tile.RemoveFeature(this);
    }

    /// <summary>
    /// Adds a mesh collider and 3D tooltip to all children.
    /// </summary>
    private void AddCollidersAndTooltipToChildren(GameObject obj, bool includeRoot = true)
    {
        if (includeRoot)
        {
            if (obj.GetComponent<MeshFilter>() != null)
            {
                obj.AddComponent<MeshCollider>();
                TooltipTarget3D tooltip = obj.AddComponent<TooltipTarget3D>();
                tooltip.Title = LabelCap;
                tooltip.Text = Description;
            }
        }
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            AddCollidersAndTooltipToChildren(obj.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Returns if meeples are allowed to always stop on this tile.
    /// </summary>
    public virtual bool CanMeepleStopHere()
    {
        return Def.MeepleCanStopOn;
    }

    /// <summary>
    /// Returns all possible actions a meeple can do during the moving phase when standing on a tile with this feature.
    /// </summary>
    /// <returns></returns>
    public virtual List<TileInteraction> GetInteractions()
    {
        return new List<TileInteraction>();
    }

    protected GameObject PlaceObjectAroundTile(string prefabPath, float offsetDistance, float angleOffset = 0)
    {
        GameObject prefab = ResourceManager.LoadPrefab(prefabPath);
        GameObject obj = GameObject.Instantiate(prefab, transform);
        obj.transform.rotation = Quaternion.Euler(0f, Tile.ForwardAngle + angleOffset, 0f);
        obj.transform.localPosition = Vector3.zero;

        float offsetAngle = Tile.ForwardAngle + 90;
        float x = Mathf.Sin(Mathf.Deg2Rad * offsetAngle) * offsetDistance;
        float y = Mathf.Cos(Mathf.Deg2Rad * offsetAngle) * offsetDistance;
        transform.localPosition = new Vector3(x, 0f, y);

        return obj;
    }

    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public virtual string Description => Def.Description;
}
