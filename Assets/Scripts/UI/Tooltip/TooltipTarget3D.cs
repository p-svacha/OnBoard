using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TooltipTarget3D : MonoBehaviour
{
    [Tooltip("Which tooltip style to show")]
    public Tooltip.TooltipType Type;
    [Tooltip("Header text")]
    public string Title;
    [Tooltip("Body text")]
    public string Text;

    [Tooltip("Seconds to wait before showing the tooltip")]
    public float Delay = 0.5f;

    bool _isHovered;
    float _hoverTime;

    void OnMouseEnter()
    {
        _isHovered = true;
        _hoverTime = 0f;
    }

    void OnMouseExit()
    {
        HideTooltip();
    }

    void Update()
    {
        if (!_isHovered) return;

        _hoverTime += Time.deltaTime;
        if (_hoverTime >= Delay)
        {
            ShowTooltip();
            _isHovered = false;   // only show once until re‑enter
        }
    }

    void ShowTooltip()
    {
        if (Tooltip.Instance.gameObject.activeSelf) return;

        Tooltip.Instance.gameObject.SetActive(true);
        Tooltip.Instance.Init(Type, Title, Text);
    }

    void HideTooltip()
    {
        _isHovered = false;
        _hoverTime = 0f;
        Tooltip.Instance.gameObject.SetActive(false);
    }
}