using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour, IDraftable, ITradable
{
    public TokenShapeDef Shape { get; private set; }
    public List<TokenSurface> Surfaces { get; private set; }
    public TokenSizeDef Size { get; private set; }
    public TokenAffinityDef Affinity { get; private set; }

    public int ModelId { get; private set; }
    private float Scale;

    public MeshRenderer Renderer;
    public Rigidbody Rigidbody;
    public Collider Collider;
    public AffinityGlowFX AffinityGlow;

    /// <summary>
    /// If this token is in the player token pouch.
    /// </summary>
    public bool IsInPouch;

    /// <summary>
    /// If this token is a copy of another token, this refers to the original.
    /// </summary>
    public Token Original;

    public void Init(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size, TokenAffinityDef affinity, int modelId, AffinityGlowFX affinityGlow)
    {
        Renderer = GetComponent<MeshRenderer>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<MeshCollider>();
        AffinityGlow = affinityGlow;

        Shape = shape;
        Surfaces = new List<TokenSurface>();
        for(int i = 0; i < surfaces.Count; i++) Surfaces.Add(new TokenSurface(this, surfaces[i].Color, shape.NumSurfaces == 1 ? Vector3.zero : Shape.SurfaceLocalNormals[i]));
        SetSize(size);
        SetAffinity(affinity);

        ModelId = modelId;
        transform.localScale = new Vector3(Scale, Scale, Scale);
    }

    public void SetSize(TokenSizeDef size)
    {
        Size = size;
        Scale = size.Scale + Random.Range(-TokenGenerator.MAX_SCALE_MODIFIER, TokenGenerator.MAX_SCALE_MODIFIER);
    }

    public void SetAffinity(TokenAffinityDef affinity)
    {
        Affinity = affinity;
        AffinityGlow.gameObject.SetActive(Affinity != null);
        if (Affinity != null) AffinityGlow.GlowColor = Affinity.Color;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        AffinityGlow.gameObject.SetActive(false);
    }

    public void Show()
    {
        transform.localScale = new Vector3(Scale, Scale, Scale);
        gameObject.SetActive(true);
        AffinityGlow.gameObject.SetActive(Affinity != null);
    }

    /// <summary>
    /// Disable physics and collision
    /// </summary>
    public void Freeze()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.isKinematic = true;

        Collider.enabled = false;
    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }

    #region Getters

    /// <summary>
    /// Returns the resources this token provides when the given surface of it is in the spread.
    /// </summary>
    /// <returns></returns>
    public Dictionary<ResourceDef, int> GetResources(TokenSurface surface)
    {
        // Validate
        if (surface == null) return new(); // Surface not yet determined because token is still in movement
        if (!Surfaces.Contains(surface)) throw new System.Exception("The given surface is not a surface of this token.");

        // Base resources from color and size
        Dictionary<ResourceDef, int> resources = new Dictionary<ResourceDef, int>();
        if (surface.Color.Resource != null)
        {
            int amount = surface.Color.ResourceBaseAmount * Size.EffectMultiplier;
            resources.Increment(surface.Color.Resource, amount);
        }

        // Resources from items
        foreach(Item item in Game.Instance.Items)
        {
            resources.IncrementMultiple(item.GetTokenResourceModifiers(this));
        }

        // Final result
        return resources;
    }

    public bool HasAffinity => Affinity != null;

    public int GetMarketValue()
    {
        return 10;
    }

    public string Label
    {
        get
        {
            string affinity = Affinity != null ? $"{Affinity.Label} " : "";
            if (Surfaces.Count == 1) return $"{Size.Label} {Surfaces[0].Label} {affinity}{Shape.Label}";
            else
            {
                string surfaces = "";
                foreach (TokenSurface surf in Surfaces) surfaces += $"{surf.Label}/";
                surfaces = surfaces.TrimEnd('/');
                return $"{Size.Label} {surfaces} {affinity}{Shape.Label}";
            }
        }
    }
    public string LabelCap => Label.CapitalizeFirst();

    public string Description
    {
        get
        {
            string desc = "";
            for(int i = 0; i < Surfaces.Count; i++)
            {
                desc += Surfaces[i].Description;
                if (i < Surfaces.Count - 1) desc += "\nOR\n";
            }
            return desc;
        }
    }

    #endregion

    // IDraftable
    public string DraftDisplay_Title => LabelCap;
    public string DraftDisplay_Text => null;
    public Sprite DraftDisplay_Sprite => null;
    public GameObject DraftDisplay_Spinning3DObject => gameObject;
}
