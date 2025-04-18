using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    public TokenShapeDef Shape { get; private set; }
    public TokenColorDef Color { get; private set; }
    public TokenSizeDef Size { get; private set; }
    private float Scale;

    public MeshRenderer Renderer;
    public Rigidbody Rigidbody;
    public MeshCollider Collider;

    public void Init(TokenShapeDef shape, TokenColorDef color, TokenSizeDef size, float scale)
    {
        Shape = shape;
        Color = color;
        Size = size;
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

    public string Label => $"{Size.Label} {Color.Label} {Shape.Label}";
    public string LabelCap => Label.CapitalizeFirst();
}
