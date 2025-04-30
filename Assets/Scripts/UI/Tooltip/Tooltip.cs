using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Tooltip : MonoBehaviour
{
    // Singleton
    public static Tooltip Instance;

    [Header("Elements")]
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Text;

    [Header("Configuration")]
    public int MaxWidth = 200;

    private float Width;
    private float Height;

    private const int MOUSE_OFFSET = 5; // px
    private const int SCREEN_EDGE_OFFSET = 5; // px

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Init(TooltipType type, string title, string text)
    {
        // Show/hide title text
        Title.gameObject.SetActive(type == TooltipType.TitleAndText);
        Title.text = title;
        Text.text = text;

        RectTransform rect = GetComponent<RectTransform>();

        // 1) Force a max width initially
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, MaxWidth);

        // 2) Update the TMP text so it wraps if necessary
        Title.ForceMeshUpdate();
        Text.ForceMeshUpdate();

        // 3) Measure the actual space needed *after* wrapping
        float neededWidth = Mathf.Max(Title.preferredWidth + 50, Text.preferredWidth + 50);

        // 4) Clamp the width if the content is smaller than max width
        float finalWidth = Mathf.Min(neededWidth, MaxWidth);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalWidth);

        // Initial placement
        rect.ForceUpdateRectTransforms();
        RepositionTooltip();

        // Show it
        gameObject.SetActive(true);
    }

    private void Update()
    {
        // If the tooltip is active, keep it following the mouse
        if (gameObject.activeSelf)
        {
            RepositionTooltip();
        }
    }

    /// <summary>
    /// Reposition the tooltip near the mouse, clamping to screen edges.
    /// </summary>
    private void RepositionTooltip()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 position = Input.mousePosition + new Vector3(MOUSE_OFFSET, MOUSE_OFFSET, 0);

        // Fit on screen
        Width = rect.rect.width;
        Height = rect.rect.height;

        // If tooltip would go off the right edge, nudge left
        if (position.x + Width > Screen.width - SCREEN_EDGE_OFFSET)
            position.x = Screen.width - Width - SCREEN_EDGE_OFFSET;

        // If it would go off the top
        if (position.y + Height > Screen.height - SCREEN_EDGE_OFFSET)
            position.y = Screen.height - Height - SCREEN_EDGE_OFFSET;

        transform.position = position;
    }


    public enum TooltipType
    {
        TitleAndText,
        TextOnly
    }
}

