using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TradeInventoryEntry : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI CostText;

    public void Init(ITradable item)
    {
        LabelText.text = item.Label;
        CostText.text = item.MarketValue.ToString();
    }
}
