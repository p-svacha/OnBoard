using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TileInteractionOption : MonoBehaviour
{
    private TileInteraction Interaction;

    [Header("Elements")]
    public Button Button;
    public TextMeshProUGUI Text;
    public TooltipTarget Tooltip;

    public void Init(TileInteraction interaction)
    {
        Interaction = interaction;

        Button.onClick.AddListener(OnClick);
        Text.text = interaction.Label;
        Tooltip.Title = interaction.Label;
        Tooltip.Text = interaction.Description;

        if (!Interaction.IsAvailable)
        {
            Tooltip.Text += $"\n\n<color=\"red\">{Interaction.Validator()}</color>";
            Button.interactable = false;
        }
    }

    private void OnClick()
    {
        if (Interaction.IsAvailable) Interaction.Execute();
    }

    private void Update()
    {
        if (Interaction?.Tile == null) return;

        // World position offset (slightly above tile)
        float verticalOffset = 2f;
        Vector3 worldPos = Interaction.Tile.transform.position + Vector3.up * verticalOffset;
        if (Interaction.Feature != null) worldPos = Interaction.Feature.transform.position + Vector3.up * verticalOffset;


        // Convert to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;

        // Optional: Hide if behind camera
        if (screenPos.z < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        // Wobble effect
        float wobble = Mathf.Sin(Time.time * 2f) * 2f;
        transform.localPosition += new Vector3(0f, wobble, 0f);
    }
}
