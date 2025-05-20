using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITradable
{
    public string Label { get; }
    public int GetMarketValue();
}
