using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TradeInventoryEntry : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI CostText;
    public Button Button;

    public void Init(ITradable tradable, float valueModifier, System.Action<ITradable> onClickAction)
    {
        LabelText.text = tradable.Label;
        CostText.text = Mathf.RoundToInt(tradable.GetMarketValue() * valueModifier).ToString();
        Button.onClick.AddListener(() => onClickAction.Invoke(tradable));
    }
}
