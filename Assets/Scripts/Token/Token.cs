using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for(int i = 0; i < surfaces.Count; i++) Surfaces.Add(new TokenSurface(this, surfaces[i].Color, shape.NumSurfaces == 1 ? Vector3.zero : Shape.SurfaceLocalNormals[i], surfaces[i].Pattern));
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

    public Token GetCopy() => TokenGenerator.GenerateTokenCopy(this);

    /// <summary>
    /// Returns the resources this token provides when the given surface of it is in the spread.
    /// </summary>
    /// <returns></returns>
    public Dictionary<ResourceDef, int> GetResources(TokenSurface surface)
    {
        Dictionary<ResourceDef, int> resources = new Dictionary<ResourceDef, int>();

        // Validate
        if (surface == null) return new(); // Surface not yet determined because token is still in movement
        if (!Surfaces.Contains(surface)) throw new System.Exception("The given surface is not a surface of this token.");

        // Base resource from surface color
        if (surface.Color.Resource != null)
        {
            resources.Increment(surface.Color.Resource, surface.Color.ResourceBaseAmount);
        }

        // Scale by size
        foreach (ResourceDef resource in resources.Keys.ToList()) resources[resource] *= Size.EffectMultiplier;

        // Apply pattern offset and modifier
        if (surface.Pattern != null)
        {
            foreach (ResourceDef resource in resources.Keys.ToList()) resources[resource] *= Mathf.RoundToInt(surface.Pattern.GlobalResourceFactor);
        }

        // Resources from items
        if (Game.Instance.Items != null)
        {
            foreach (Item item in Game.Instance.Items)
            {
                resources.IncrementMultiple(item.GetTokenResourceModifiers(this));
            }
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
            if (Surfaces.Count == 1) return $"{Size.Label} {Surfaces[0].Label} {AffinityLabel}{Shape.Label}";
            else
            {
                string surfaces = "";
                foreach (TokenSurface surf in Surfaces) surfaces += $"{surf.Label}/";
                surfaces = surfaces.TrimEnd('/');
                return $"{Size.Label} {surfaces} {AffinityLabel}{Shape.Label}";
            }
        }
    }
    private string AffinityLabel => Affinity != null ? $"{Affinity.Label} " : "";
    public string LabelNoSurface => $"{Size.Label} {AffinityLabel}{Shape.Label}";
    public string LabelCap => Label.CapitalizeFirst();

    public string Description
    {
        get
        {
            string desc = "";
            for(int i = 0; i < Surfaces.Count; i++)
            {
                desc += Surfaces[i].Description;
                if (i < Surfaces.Count - 1) desc += "\n - OR - \n";
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
