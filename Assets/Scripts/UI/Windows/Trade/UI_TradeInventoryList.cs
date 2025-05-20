using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_TradeInventoryList : MonoBehaviour
{
    private Dictionary<ITradable, UI_TradeInventoryEntry> Entries;
    public List<ITradable> Content => Entries.Keys.ToList();
    private float ValueModifier;
    private System.Action<ITradable> OnClickAction; // What happens when the player clicks on a tradable in this list

    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_TradeInventoryEntry InventoryItemPrefab;

    public void Clear()
    {
        Entries = new Dictionary<ITradable, UI_TradeInventoryEntry>();
        HelperFunctions.DestroyAllChildredImmediately(Container);
    }

    public void Init(List<ITradable> items, float valueModifier, System.Action<ITradable> onClickAction)
    {
        ValueModifier = valueModifier;
        OnClickAction = onClickAction;

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
