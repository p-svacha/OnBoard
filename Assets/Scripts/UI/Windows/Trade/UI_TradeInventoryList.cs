using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UI_TradeInventoryList : MonoBehaviour
{
    private Dictionary<ITradable, UI_TradeInventoryEntry> Entries;
    public List<ITradable> Content => Entries.Keys.ToList();
    private float ValueModifier;
    private System.Action<ITradable> OnClickAction; // What happens when the player clicks on a tradable in this list

    [Header("Elements")]
    public TextMeshProUGUI TitleText;
    public GameObject Container;

    [Header("Prefabs")]
    public UI_TradeInventoryEntry InventoryItemPrefab;

    public void Clear()
    {
        Entries = new Dictionary<ITradable, UI_TradeInventoryEntry>();
        HelperFunctions.DestroyAllChildredImmediately(Container);
    }

    public void Init(List<ITradable> items, float valueModifier, System.Action<ITradable> onClickAction = null, string titleOverride = "")
    {
        ValueModifier = valueModifier;
        OnClickAction = onClickAction;
        if (titleOverride != "") TitleText.text = titleOverride;

        Clear();
        foreach(ITradable item in items) AddEntry(item);
    }

    public void AddEntry(ITradable item)
    {
        UI_TradeInventoryEntry entry = GameObject.Instantiate(InventoryItemPrefab, Container.transform);
        entry.Init(item, ValueModifier, OnClickAction);
        Entries.Add(item, entry);
    }
    public void RemoveEntry(ITradable item)
    {
        GameObject.Destroy(Entries[item].gameObject);
        Entries.Remove(item);
    }
}
