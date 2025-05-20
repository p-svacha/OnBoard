using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_TradeInventoryList : MonoBehaviour
{
    private Dictionary<ITradable, UI_TradeInventoryEntry> Entries;
    public List<ITradable> Content => Entries.Keys.ToList();

    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_TradeInventoryEntry InventoryItemPrefab;

    public void Clear()
    {
        Entries = new Dictionary<ITradable, UI_TradeInventoryEntry>();
        HelperFunctions.DestroyAllChildredImmediately(Container);
    }

    public void Init(List<ITradable> items)
    {
        Clear();
        foreach(ITradable item in items) AddEntry(item);
    }

    private void AddEntry(ITradable item)
    {
        UI_TradeInventoryEntry entry = GameObject.Instantiate(InventoryItemPrefab, Container.transform);
        entry.Init(item);
        Entries.Add(item, entry);
    }

    
}
