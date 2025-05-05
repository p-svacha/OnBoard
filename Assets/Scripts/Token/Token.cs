using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    public TokenShapeDef Shape { get; private set; }
    public List<TokenSurface> Surfaces { get; private set; }
    public TokenSizeDef Size { get; private set; }

    public int ModelId { get; private set; }
    private float Scale;

    public MeshRenderer Renderer;
    public Rigidbody Rigidbody;
    public Collider Collider;

    /// <summary>
    /// If this token is in the player token pouch.
    /// </summary>
    public bool IsInPouch;

    /// <summary>
    /// If this token is a copy of another token, this refers to the original.
    /// </summary>
    public Token Original;

    public void Init(TokenShapeDef shape, List<TokenSurface> surfaces, TokenSizeDef size, int modelId, float scale)
    {
        Shape = shape;
        Surfaces = new List<TokenSurface>();
        for(int i = 0; i < surfaces.Count; i++) Surfaces.Add(new TokenSurface(this, surfaces[i].Color, shape.NumSurfaces == 1 ? Vector3.zero : Shape.SurfaceLocalNormals[i]));
        Size = size;

        ModelId = modelId;
        Scale = scale;

        Renderer = GetComponent<MeshRenderer>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<MeshCollider>();

        transform.localScale = new Vector3(Scale, Scale, Scale);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        transform.localScale = new Vector3(Scale, Scale, Scale);
        gameObject.SetActive(true);
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

    #region Getters

    public string Label
    {
        get
        {
            if(Surfaces.Count == 1) return $"{Size.Label} {Surfaces[0].Label} {Shape.Label}";
            else
            {
                string surfaces = "";
                foreach (TokenSurface surf in Surfaces) surfaces += $"{surf.Label}/";
                surfaces = surfaces.TrimEnd('/');
                return $"{Size.Label} {surfaces} {Shape.Label}";
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
}
