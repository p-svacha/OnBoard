using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to everything that has a persistent trading inventory.
/// </summary>
public interface ITrader
{
    public string Label { get; }
    public List<ITradable> GetTradeInventory();
}
